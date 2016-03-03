using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using Xunit;

namespace GraphQL.Conventions.Tests.Builders
{
    public class SchemaConstructorTests : ConstructionTestBase
    {
        [Fact]
        public void Can_Construct_Schema()
        {
            var schema = Schema<SchemaType1>();
            schema.ShouldHaveQueries(2);
            schema.ShouldHaveMutations(0);
            schema.Query.ShouldHaveFieldWithName("foo");
            schema.Query.ShouldHaveFieldWithName("bar");
        }

        [Fact]
        public void Can_Combine_Schemas()
        {
            var schema = Schema<SchemaType1, SchemaType2>();
            schema.ShouldHaveQueries(3);
            schema.ShouldHaveMutations(1);
            schema.Query.ShouldHaveFieldWithName("foo");
            schema.Query.ShouldHaveFieldWithName("bar");
            schema.Query.ShouldHaveFieldWithName("baz");
            schema.Mutation.ShouldHaveFieldWithName("updateSomething");
        }

        class SchemaType1
        {
            public QueryType1 Query { get; }

            public MutationType1 Mutation { get; }
        }

        class QueryType1
        {
            public string Foo => "Test";

            public int Bar => 12345;
        }

        class MutationType1
        {
        }

        class SchemaType2
        {
            public QueryType2 Query { get; }

            public MutationType2 Mutation { get; }
        }

        class QueryType2
        {
            public bool Baz => false;
        }

        class MutationType2
        {
            public bool UpdateSomething() => false;
        }
    }
}
