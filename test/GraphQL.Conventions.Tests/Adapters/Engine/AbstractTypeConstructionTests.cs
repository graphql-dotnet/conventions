using System.Threading.Tasks;
using GraphQL.Conventions;
using Tests.Templates;
using Tests.Templates.Extensions;

// ReSharper disable UnusedMember.Local

namespace Tests.Adapters.Engine
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
                commonField(value: Int!): Int!
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
                .WithQueryString("{ commonField(value: 1) someOtherField }")
                .EnableValidation()
                .ExecuteAsync();

            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("commonField", 1);
            result.Data.ShouldHaveFieldWithValue("someOtherField", string.Empty);
        }

        abstract class EntityQuery<T>
        {
            public T CommonField(T value) => value;
        }

        class Query : EntityQuery<int>
        {
            public string SomeOtherField => string.Empty;
        }
    }
}
