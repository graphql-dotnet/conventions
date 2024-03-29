using GraphQL.Conventions.Adapters;
using GraphQL.Conventions.Builders;
using GraphQL.Types;
using Tests.Templates;
using Tests.Templates.Extensions;
// ReSharper disable UnassignedGetOnlyAutoProperty

// ReSharper disable UnusedMember.Local

namespace Tests.Builders
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
            schema.Initialize();
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

            schema.Initialize();
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
                "Tests.Builders.U",
                "Tests.Builders.Un",
                "Tests.Builders.Unw",
                "Tests.Builders.Unwan",
                "Tests.Builders.Unwant",
                "Tests.Builders.Unwante",
                "Tests.Builders.Unwanted"
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

                schema.Initialize();
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

        [Test]
        public void Can_Ignore_Unwanted_Types()
        {
            // Ignore specific types from the 'Unwanted' namespace.

            var schema = new SchemaConstructor<ISchema, IGraphType>(new GraphTypeAdapter())
                    .IgnoreTypes((t, m) =>
                    {
                        // Ignore based on the type:
                        if (t == typeof(Unwanted.QueryType3))
                            return true;

                        // Ignore based on name of the method:
                        if (m != null && m.Name == "UpdateSomethingIgnored")
                            return true;

                        return false;
                    })
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

        private class SchemaType1
        {
            public QueryType1 Query { get; }

            public MutationType1 Mutation { get; }
        }

        private class QueryType1
        {
            public string Foo => "Test";

            public int Bar => 12345;

            public void Ignored() { }
        }

        private class MutationType1
        {
        }

        private class SchemaType2
        {
            public QueryType2 Query { get; }

            public MutationType2 Mutation { get; }
        }

        private class QueryType2
        {
            public bool Baz => false;
        }

        private class MutationType2
        {
            public bool UpdateSomething() => false;
        }
    }

    namespace Unwanted
    {
        internal class SchemaType3
        {
            public QueryType3 Query { get; }

            public MutationType3 Mutation { get; }
        }

        internal class QueryType3
        {
            public bool BazIgnored => false;
        }

        internal class MutationType3
        {
            public bool UpdateSomethingIgnored() => false;
        }
    }
}
