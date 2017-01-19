using System;
using GraphQL.Conventions.Adapters.Engine;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types;
using Xunit;

namespace GraphQL.Conventions.Tests.Adapters.Engine
{
    public class AbstractTypeConstructionTests : TestBase
    {
        [Fact]
        public void Can_Construct_And_Describe_Schema_From_Abstract_Types()
        {
            var engine = new GraphQLEngine();
            engine.BuildSchema(typeof(SchemaDefinition<Query>));
            var schema = engine.Describe();
            schema.ShouldEqualWhenReformatted(@"
            type Query {
              commonField: Date!
              someOtherField: String
            }
            ");
        }

        [Fact]
        public async void Can_Execute_Query_On_Schema_From_Abstract_Types()
        {
            var engine = new GraphQLEngine();
            engine.BuildSchema(typeof(SchemaDefinition<Query>));
            var result = await engine
                .NewExecutor()
                .WithQueryString("{ commonField someOtherField }")
                .EnableValidation()
                .Execute();

            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("commonField", default(DateTime));
            result.Data.ShouldHaveFieldWithValue("someOtherField", string.Empty);
        }

        abstract class EntityQuery<T>
        {
            public T CommonField => default(T);
        }

        class Query : EntityQuery<DateTime>
        {
            public string SomeOtherField => string.Empty;
        }
    }
}