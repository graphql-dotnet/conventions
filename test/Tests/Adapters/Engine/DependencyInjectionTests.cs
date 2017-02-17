using System.Reflection;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;

namespace GraphQL.Conventions.Tests.Adapters.Engine
{
    public class DependencyInjectionTests : TestBase
    {
        [Test]
        public void Can_Construct_And_Describe_Schema_With_Injections()
        {
            var engine = GraphQLEngine.New<Query>();
            var schema = engine.Describe();
            schema.ShouldEqualWhenReformatted(@"
            type Query {
                field: String
            }
            ");
        }

        [Test]
        public async void Can_Execute_Query_On_Schema_With_Injections()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("{ field }")
                .WithDependencyInjector(new DependencyInjector())
                .Execute();

            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("field", "Some Value");
        }

        [Test]
        public void Can_Construct_And_Describe_Schema_With_Injections_In_Generic_Methods()
        {
            var engine = GraphQLEngine.New<QueryWithDIFields>();
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

        [Test]
        public async void Can_Execute_Query_On_Schema_With_Injections_In_Generic_Methods()
        {
            var engine = GraphQLEngine.New<QueryWithDIFields>();
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