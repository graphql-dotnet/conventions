using System.Threading.Tasks;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;
using Tests.Templates;
using Tests.Templates.Extensions;
using Tests.Types;

// ReSharper disable UnusedType.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Tests.Attributes.Execution.Relay
{
    public class DynamicMutationRegistrationTests : TestBase
    {
        [Test]
        public async Task Can_Pass_On_ClientMutationId_For_Relay_Nullable_Mutations()
        {
            var query = @"
                mutation _ {
                    foo(input: { clientMutationId: ""some-mutation-id-1"", a: 1 }) {
                        clientMutationId
                        result
                    }
                    bar(input: { clientMutationId: ""some-mutation-id-2"", b: 2 }) {
                        clientMutationId
                        result
                    }
                }";

            var mutations = new[]
            {
                typeof(SchemaDefinitionWithMutation<FooMutation>),
                typeof(SchemaDefinitionWithMutation<BarMutation>),
            };
            var engine = GraphQLEngine.New()
                .WithQuery<TestQuery>()
                .BuildSchema(mutations);

            var result = await engine
                .NewExecutor()
                .WithQueryString(query)
                .ExecuteAsync();

            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("foo", "clientMutationId", "some-mutation-id-1");
            result.Data.ShouldHaveFieldWithValue("foo", "result", 2);
            result.Data.ShouldHaveFieldWithValue("bar", "clientMutationId", "some-mutation-id-2");
            result.Data.ShouldHaveFieldWithValue("bar", "result", 102);
        }

        // Scaffolding

        private interface IMutation { }

        private interface IMutation<TInput, TOutput>
            where TInput : class, IRelayMutationInputObject
            where TOutput : class, IRelayMutationOutputObject
        {
            [Ignore]
            TOutput Mutate(NonNull<TInput> input);
        }

        private abstract class RelayInput : IRelayMutationInputObject
        {
            public string ClientMutationId { get; set; }
        }

        private abstract class RelayOutput : IRelayMutationOutputObject
        {
            public string ClientMutationId { get; set; }
        }

        // Foo

        [RelayMutationType]
        private class FooMutation : IMutation<FooInput, FooOutput>
        {
            [Name("foo")]
            public FooOutput Mutate(NonNull<FooInput> input) =>
                new FooOutput { Result = 2 * input.Value.A };
        }

        private class FooInput : RelayInput
        {
            public int A { get; set; }
        }

        private class FooOutput : RelayOutput
        {
            public int Result { get; set; }
        }

        // Bar

        [RelayMutationType]
        private class BarMutation : IMutation<BarInput, BarOutput>
        {
            [Name("bar")]
            public BarOutput Mutate(NonNull<BarInput> input) =>
                new BarOutput { Result = 100 + input.Value.B };
        }

        private class BarInput : RelayInput
        {
            public int B { get; set; }
        }

        private class BarOutput : RelayOutput
        {
            public int Result { get; set; }
        }

        private class Query
        {
            public string Hello => "World";
        }
    }
}
