using System.Threading.Tasks;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;

namespace GraphQL.Conventions.Tests.Attributes.Execution.Relay
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
            var engine = GraphQLEngine.New().BuildSchema(mutations);
            var result = await engine
                .NewExecutor()
                .WithQueryString(query)
                .Execute();

            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("foo", "clientMutationId", "some-mutation-id-1");
            result.Data.ShouldHaveFieldWithValue("foo", "result", 2);
            result.Data.ShouldHaveFieldWithValue("bar", "clientMutationId", "some-mutation-id-2");
            result.Data.ShouldHaveFieldWithValue("bar", "result", 102);
        }

        // Scaffolding

        interface IMutation { }

        interface IMutation<TInput, TOutput>
            where TInput : class, IRelayMutationInputObject
            where TOutput : class, IRelayMutationOutputObject
        {
            [Ignore]
            TOutput Mutate(NonNull<TInput> input);
        }

        abstract class RelayInput : IRelayMutationInputObject
        {
            public string ClientMutationId { get; set; }
        }

        abstract class RelayOutput : IRelayMutationOutputObject
        {
            public string ClientMutationId { get; set; }
        }

        // Foo

        [RelayMutationType]
        class FooMutation : IMutation<FooInput, FooOutput>
        {
            [Name("foo")]
            public FooOutput Mutate(NonNull<FooInput> input) =>
                new FooOutput { Result = 2 * input.Value.A };
        }

        class FooInput : RelayInput
        {
            public int A { get; set; }
        }

        class FooOutput : RelayOutput
        {
            public int Result { get; set; }
        }

        // Bar

        [RelayMutationType]
        class BarMutation : IMutation<BarInput, BarOutput>
        {
            [Name("bar")]
            public BarOutput Mutate(NonNull<BarInput> input) =>
                new BarOutput { Result = 100 + input.Value.B };
        }

        class BarInput : RelayInput
        {
            public int B { get; set; }
        }

        class BarOutput : RelayOutput
        {
            public int Result { get; set; }
        }
    }
}
