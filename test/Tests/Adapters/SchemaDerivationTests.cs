using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution;

namespace GraphQL.Conventions.Tests.Adapters
{
    public class SchemaDerivationTests : ConstructionTestBase
    {
        [Test]
        public void Can_Derive_Schema_With_Query_Only()
        {
            var typeResolver = new TypeResolver();
            var schema = Schema(new GraphSchemaInfo
            {
                Query = typeResolver.DeriveType<SimpleQueryType>(),
            });

            schema.ShouldHaveQueries(1);
            schema.Query.Name.ShouldEqual(nameof(SimpleQueryType));
            schema.ShouldHaveMutations(0);
            schema.ShouldHaveSubscriptons(0);
        }

        [Test]
        public void Can_Derive_Schema_Without_Query()
        {
            var typeResolver = new TypeResolver();
            var schema = Schema(new GraphSchemaInfo
            {
                Mutation = typeResolver.DeriveType<SimpleMutationType>(),
            });

            schema.ShouldHaveQueries(0);
            schema.ShouldHaveMutations(1);
            schema.Mutation.Name.ShouldEqual(nameof(SimpleMutationType));
            schema.ShouldHaveSubscriptons(0);
        }

        [Test]
        public void Can_Derive_Schema_With_Query_And_Mutation()
        {
            var typeResolver = new TypeResolver();
            var schema = Schema(new GraphSchemaInfo
            {
                Query = typeResolver.DeriveType<SimpleQueryType>(),
                Mutation = typeResolver.DeriveType<SimpleMutationType>(),
            });

            schema.ShouldHaveQueries(1);
            schema.Query.Name.ShouldEqual(nameof(SimpleQueryType));
            schema.ShouldHaveMutations(1);
            schema.Mutation.Name.ShouldEqual(nameof(SimpleMutationType));
            schema.ShouldHaveSubscriptons(0);
        }

        [Test]
        public void Can_Derive_Schema_With_Query_And_Subscription()
        {
            var typeResolver = new TypeResolver();
            var schema = Schema(new GraphSchemaInfo
            {
                Query = typeResolver.DeriveType<SimpleQueryType>(),
                Subscription = typeResolver.DeriveType<SimpleSubscriptionType>(),
            });

            schema.ShouldHaveQueries(1);
            schema.Query.Name.ShouldEqual(nameof(SimpleQueryType));
            schema.ShouldHaveMutations(0);
            schema.ShouldHaveSubscriptons(1);
            schema.Subscription.Name.ShouldEqual(nameof(SimpleSubscriptionType));
        }

        class SimpleQueryType
        {
            public string TestQuery() => string.Empty;
        }

        class SimpleMutationType
        {
            public string TestMutation() => string.Empty;
        }

        class SimpleSubscriptionType
        {
            public string TestSubscription() => string.Empty;
        }
    }
}