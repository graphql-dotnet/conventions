using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;
using Tests.Templates;
using Tests.Templates.Extensions;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Tests.Attributes.Execution.Relay
{
    public class RelayMutationAttributeTests : TestBase
    {
        [Test]
        public async Task Can_Pass_On_ClientMutationId_For_Relay_Nullable_Mutations()
        {
            var result = await ExecuteMutation(@"
                mutation _ {
                    doSomething(input: { clientMutationId: ""some-mutation-id-1"", action: ADD }) {
                        clientMutationId
                        wasSuccessful
                    }
                }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("doSomething", "clientMutationId", "some-mutation-id-1");
            result.Data.ShouldHaveFieldWithValue("doSomething", "wasSuccessful", true);
        }

        [Test]
        public async Task Can_Pass_On_ClientMutationId_For_Relay_NonNullable_Mutations()
        {
            var result = await ExecuteMutation(@"
                mutation _ {
                    nonNullableDoSomething(input: { clientMutationId: ""some-mutation-id-2"", action: REMOVE }) {
                        clientMutationId
                        wasSuccessful
                    }
                }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nonNullableDoSomething", "clientMutationId", "some-mutation-id-2");
            result.Data.ShouldHaveFieldWithValue("nonNullableDoSomething", "wasSuccessful", false);
        }

        [Test]
        public async Task Can_Pass_On_ClientMutationId_For_Relay_Task_Mutations()
        {
            var result = await ExecuteMutation(@"
                mutation _ {
                    taskDoSomething(input: { clientMutationId: ""some-mutation-id-3"", action: UPDATE }) {
                        clientMutationId
                        wasSuccessful
                    }
                }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("taskDoSomething", "clientMutationId", "some-mutation-id-3");
            result.Data.ShouldHaveFieldWithValue("taskDoSomething", "wasSuccessful", true);
        }

        [Test]
        public async Task Can_Pass_On_ClientMutationId_For_Relay_Nullable_Mutations_With_Type_Decoration()
        {
            var result = await ExecuteMutation<MutationType>(@"
                mutation _ {
                    doSomething(input: { clientMutationId: ""some-mutation-id-1"", action: ADD }) {
                        clientMutationId
                        wasSuccessful
                    }
                }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("doSomething", "clientMutationId", "some-mutation-id-1");
            result.Data.ShouldHaveFieldWithValue("doSomething", "wasSuccessful", true);
        }

        [Test]
        public async Task Can_Pass_On_ClientMutationId_For_Relay_NonNullable_Mutations_With_Type_Decoration()
        {
            var result = await ExecuteMutation<MutationType>(@"
                mutation _ {
                    nonNullableDoSomething(input: { clientMutationId: ""some-mutation-id-2"", action: REMOVE }) {
                        clientMutationId
                        wasSuccessful
                    }
                }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("nonNullableDoSomething", "clientMutationId", "some-mutation-id-2");
            result.Data.ShouldHaveFieldWithValue("nonNullableDoSomething", "wasSuccessful", false);
        }

        [Test]
        public async Task Can_Pass_On_ClientMutationId_For_Relay_Task_Mutations_With_Type_Decoration()
        {
            var result = await ExecuteMutation<MutationType>(@"
                mutation _ {
                    taskDoSomething(input: { clientMutationId: ""some-mutation-id-3"", action: UPDATE }) {
                        clientMutationId
                        wasSuccessful
                    }
                }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("taskDoSomething", "clientMutationId", "some-mutation-id-3");
            result.Data.ShouldHaveFieldWithValue("taskDoSomething", "wasSuccessful", true);
        }

        private async Task<ExecutionResult> ExecuteMutation<T>(string query, Dictionary<string, object> inputs = null)
        {
            var engine = GraphQLEngine
                .New()
                .WithMutation<T>();
            var result = await engine
                .NewExecutor()
                .WithQueryString(query)
                .WithInputs(inputs)
                .ExecuteAsync();

            return result;
        }

        private async Task<ExecutionResult> ExecuteMutation(string query, Dictionary<string, object> inputs = null)
        {
            return await ExecuteMutation<Mutation>(query, inputs);
        }

        class Mutation
        {
            [RelayMutation]
            public DoSomethingOutput DoSomething(DoSomethingInput input) =>
                new DoSomethingOutput
                {
                    WasSuccessful = input.Action == ActionType.Add ||
                                    input.Action == ActionType.Update,
                };

            [RelayMutation]
            public NonNull<DoSomethingOutput> NonNullableDoSomething(NonNull<DoSomethingInput> input) =>
                DoSomething(input);

            [RelayMutation]
            public async Task<NonNull<DoSomethingOutput>> TaskDoSomething(NonNull<DoSomethingInput> input)
            {
                await Task.Delay(1);
                return DoSomething(input);
            }
        }

        [RelayMutationType]
        class MutationType
        {
            public DoSomethingOutput DoSomething(DoSomethingInput input) =>
                new DoSomethingOutput
                {
                    WasSuccessful = input.Action == ActionType.Add ||
                                    input.Action == ActionType.Update,
                };

            public NonNull<DoSomethingOutput> NonNullableDoSomething(NonNull<DoSomethingInput> input) =>
                DoSomething(input);

            public async Task<NonNull<DoSomethingOutput>> TaskDoSomething(NonNull<DoSomethingInput> input)
            {
                await Task.Delay(1);
                return DoSomething(input);
            }
        }

        class DoSomethingInput : IRelayMutationInputObject
        {
            public string ClientMutationId { get; set; }

            public ActionType Action { get; set; }
        }

        class DoSomethingOutput : IRelayMutationOutputObject
        {
            public string ClientMutationId { get; set; }

            public bool WasSuccessful { get; set; }
        }

        enum ActionType
        {
            Add,
            Update,
            Remove,
        }
    }
}
