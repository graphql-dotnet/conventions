using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;

namespace GraphQL.Conventions.Tests.Attributes.MetaData.Relay
{
    public class ImplementViewerAttributeTests : TestBase
    {
        [Test]
        public void Can_Generate_A_Viewer_Node_For_A_Schema()
        {
            GetSchemaDefinition(false).ShouldEqualWhenReformatted(@"
            schema {
              query: Query1
            }
            type Query1 {
              intToString(value: Int!): String
              viewer: QueryViewer
            }
            type QueryViewer {
              intToString(value: Int!): String
            }
            ");
        }

        [Test]
        public async Task Can_Use_The_Viewer_Node_For_A_Schema()
        {
            var result = await ExecuteQuery(false, @"
            {
              intToString(value: 5)
              viewer {
                intToString(value: 5)
              }
            }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("intToString", "5");
            result.Data.ShouldHaveFieldWithValue("viewer", "intToString", "5");
        }

        [Test]
        public void Can_Generate_A_Viewer_Node_For_Multiple_Schemas()
        {
            GetSchemaDefinition(true).ShouldEqualWhenReformatted(@"
            type Query {
              floatToString(value: Float!): String
              intToString(value: Int!): String
              viewer: QueryViewer
            }
            type QueryViewer {
              floatToString(value: Float!): String
              intToString(value: Int!): String
            }
            ");
        }

        [Test]
        public async Task Can_Use_The_Viewer_Node_For_Multiple_Schemas()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var result = await ExecuteQuery(true, @"
            {
              floatToString(value: 3.14)
              intToString(value: 5)
              viewer {
                floatToString(value: 3.14)
                intToString(value: 5)
              }
            }");
            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("intToString", "5");
            result.Data.ShouldHaveFieldWithValue("floatToString", "3.14");
            result.Data.ShouldHaveFieldWithValue("viewer", "intToString", "5");
            result.Data.ShouldHaveFieldWithValue("viewer", "floatToString", "3.14");
        }

        [Test]
        public void Can_Generate_Viewers_For_Multiple_Operations()
        {
            GetSchemaDefinition(true, true).ShouldEqualWhenReformatted(@"
            type Mutation {
              doSomething(value: Boolean): Boolean
              doSomethingElse(value: Boolean): Boolean
              viewer: MutationViewer
            }
            type MutationViewer {
              doSomething(value: Boolean): Boolean
              doSomethingElse(value: Boolean): Boolean
            }
            type Query {
              floatToString(value: Float!): String
              intToString(value: Int!): String
              viewer: QueryViewer
            }
            type QueryViewer {
              floatToString(value: Float!): String
              intToString(value: Int!): String
            }
            ");
        }

        private string GetSchemaDefinition(bool useMultiple, bool includeMutations = false)
        {
            if (includeMutations)
            {
                var engine = GraphQLEngine
                    .New()
                    .WithQueryAndMutation<Query1, Mutation1>()
                    .WithQueryAndMutation<Query2, Mutation2>();
                return engine.Describe();
            }
            else
            {
                var engine = GraphQLEngine
                    .New()
                    .WithQuery<Query1>();
                if (useMultiple)
                {
                    engine.WithQuery<Query2>();
                }
                return engine.Describe();
            }
        }

        private async Task<ExecutionResult> ExecuteQuery(bool useMultiple, string query)
        {
            var engine = new GraphQLEngine();
            if (useMultiple)
            {
                engine.BuildSchema(typeof(SchemaDefinition<Query1>), typeof(SchemaDefinition<Query2>));
            }
            else
            {
                engine.BuildSchema(typeof(SchemaDefinition<Query1>));
            }
            var result = await engine
                .NewExecutor()
                .WithQueryString(query)
                .ExecuteAsync();

            return result;
        }

        [ImplementViewer(OperationType.Query)]
        class Query1
        {
            public string IntToString(int value) => value.ToString();
        }

        [ImplementViewer(OperationType.Query)]
        class Query2
        {
            public string FloatToString(float value) => value.ToString();
        }

        [ImplementViewer(OperationType.Mutation)]
        class Mutation1
        {
            public bool? DoSomething(bool? value) => value;
        }

        [ImplementViewer(OperationType.Mutation)]
        class Mutation2
        {
            public bool? DoSomethingElse(bool? value) => value;
        }
    }
}
