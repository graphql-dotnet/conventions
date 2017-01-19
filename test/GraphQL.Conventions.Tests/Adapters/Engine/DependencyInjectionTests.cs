using System.Reflection;
using GraphQL.Conventions.Adapters.Engine;
using GraphQL.Conventions.Attributes.MetaData;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types;
using GraphQL.Conventions.Types.Resolution;
using Xunit;

namespace GraphQL.Conventions.Tests.Adapters.Engine
{
    public class DependencyInjectionTests : TestBase
    {
        [Fact]
        public void Can_Construct_And_Describe_Schema_With_Injections()
        {
            var engine = new GraphQLEngine();
            engine.BuildSchema(typeof(SchemaDefinition<Query>));
            var schema = engine.Describe();
            schema.ShouldEqualWhenReformatted(@"
            type Query {
              field: String
            }
            ");
        }

        [Fact]
        public async void Can_Execute_Query_On_Schema_With_Injections()
        {
            var engine = new GraphQLEngine();
            engine.BuildSchema(typeof(SchemaDefinition<Query>));
            var result = await engine
                .NewExecutor()
                .WithQueryString("{ field }")
                .WithDependencyInjector(new DependencyInjector())
                .Execute();

            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("field", "Some Value");
        }

        [Fact]
        public void Can_Construct_And_Describe_Schema_With_Injections_In_Generic_Methods()
        {
            var engine = new GraphQLEngine();
            engine.BuildSchema(typeof(SchemaDefinition<QueryWithDIFields>));
            var schema = engine.Describe();
            schema.ShouldEqualWhenReformatted(@"
            schema {
              query: QueryWithDIFields

            }
            type QueryWithDIFields {
              withDependency: Int!
            }
            ");
        }

        [Fact]
        public async void Can_Execute_Query_On_Schema_With_Injections_In_Generic_Methods()
        {
            var engine = new GraphQLEngine();
            engine.BuildSchema(typeof(SchemaDefinition<QueryWithDIFields>));
            var result = await engine
                .NewExecutor()
                .WithQueryString("{ withDependency }")
                .WithDependencyInjector(new DependencyInjector())
                .Execute();

            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("withDependency", 3);
        }

        class Query
        {
            private readonly IRepository _repository;

            public Query(IRepository repository)
            {
                _repository = repository;
            }

            public string Field => _repository.GetValue();
        }

        interface IRepository
        {
            string GetValue();
        }

        class Repository : IRepository
        {
            public string GetValue()
            {
                return "Some Value";
            }
        }

        class QueryWithDIFields
        {
            public int WithDependency([Inject] IDependency d) => d.Get(3);
        }

        interface IDependency
        {
            T Get<T>(T value);
        }

        class Dependency : IDependency
        {
            public T Get<T>(T value) => value;
        }

        class DependencyInjector : IDependencyInjector
        {
            public object Resolve(TypeInfo typeInfo)
            {
                if (typeInfo.AsType() == typeof(Query))
                {
                    return new Query(new Repository());
                }
                if (typeInfo.AsType() == typeof(IDependency))
                {
                    return new Dependency();
                }
                return null;
            }
        }
    }
}