using GraphQL.Conventions.Adapters;
using GraphQL.Conventions.Builders;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Types;

namespace GraphQL.Conventions.Tests.Builders
{
    public class SchemaConstructorTests : ConstructionTestBase
    {
        [Test]
        public void Can_Construct_Schema()
        {
            var schema = Schema<SchemaType1>();
            schema.ShouldHaveQueries(2);
            schema.ShouldHaveMutations(0);
            schema.Query.ShouldHaveFieldWithName("foo");
            schema.Query.ShouldHaveFieldWithName("bar");
            schema.Query.ShouldNotHaveFieldWithName("ignored");
        }

        [Test]
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

        [Test]
        public void Can_Ignore_Types_From_Unwanted_Namespaces()
        {
            var schema = new SchemaConstructor<ISchema, IGraphType>(new GraphTypeAdapter()).Build(
                typeof(SchemaType1),
                typeof(SchemaType2),
                typeof(Unwanted.SchemaType3)
            );

            schema.ShouldHaveQueries(4);
            schema.ShouldHaveMutations(2);
            schema.Query.ShouldHaveFieldWithName("foo");
            schema.Query.ShouldHaveFieldWithName("bar");
            schema.Query.ShouldHaveFieldWithName("baz");
            schema.Query.ShouldHaveFieldWithName("bazIgnored");
            schema.Mutation.ShouldHaveFieldWithName("updateSomething");
            schema.Mutation.ShouldHaveFieldWithName("updateSomethingIgnored");



            // Ignore all types from the 'Unwanted' namespace.

            var unwantedNamespaces = new[] {
                "GraphQL.Conventions.Tests.Builders.U",
                "GraphQL.Conventions.Tests.Builders.Un",
                "GraphQL.Conventions.Tests.Builders.Unw",
                "GraphQL.Conventions.Tests.Builders.Unwan",
                "GraphQL.Conventions.Tests.Builders.Unwant",
                "GraphQL.Conventions.Tests.Builders.Unwante",
                "GraphQL.Conventions.Tests.Builders.Unwanted"
            };

            foreach (var namespaceStartFragment in unwantedNamespaces)
            {
                schema = new SchemaConstructor<ISchema, IGraphType>(new GraphTypeAdapter())
                    .IgnoreTypesFromNamespacesStartingWith(namespaceStartFragment)
                    .Build(
                        typeof(SchemaType1),
                        typeof(SchemaType2),
                        typeof(Unwanted.SchemaType3)
                    );

                schema.ShouldHaveQueries(3);
                schema.ShouldHaveMutations(1);
                schema.Query.ShouldHaveFieldWithName("foo");
                schema.Query.ShouldHaveFieldWithName("bar");
                schema.Query.ShouldHaveFieldWithName("baz");
                schema.Query.ShouldNotHaveFieldWithName("bazIgnored");
                schema.Mutation.ShouldHaveFieldWithName("updateSomething");
                schema.Mutation.ShouldNotHaveFieldWithName("updateSomethingIgnored");
            }
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

            public void Ignored() { }
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

    namespace Unwanted
    {
        class SchemaType3
        {
            public QueryType3 Query { get; }

            public MutationType3 Mutation { get; }
        }

        class QueryType3
        {
            public bool BazIgnored => false;
        }

        class MutationType3
        {
            public bool UpdateSomethingIgnored() => false;
        }
    }
}
