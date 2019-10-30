using System;
using System.Threading.Tasks;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;

namespace GraphQL.Conventions.Tests.Adapters.Engine
{
    public class AbstractTypeConstructionTests : TestBase
    {
        [Test]
        public void Can_Construct_And_Describe_Schema_From_Abstract_Types()
        {
            var engine = GraphQLEngine.New<Query>();
            var schema = engine.Describe();
            schema.ShouldEqualWhenReformatted(@"
            type Query {
                commonField(value: DateTime!): DateTime!
                someOtherField: String
            }
            ");
        }

        [Test]
        public async Task Can_Execute_Query_On_Schema_From_Abstract_Types()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("{ commonField(value: \"1970-01-01T00:00:00Z\") someOtherField }")
                .EnableValidation()
                .ExecuteAsync();

            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("commonField", new DateTime(1970, 1, 1));
            result.Data.ShouldHaveFieldWithValue("someOtherField", string.Empty);
        }

        abstract class EntityQuery<T>
        {
            public T CommonField(T value) => value;
        }

        class Query : EntityQuery<DateTime>
        {
            public string SomeOtherField => string.Empty;
        }
    }
}