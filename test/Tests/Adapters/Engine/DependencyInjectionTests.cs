using System.Reflection;
using System.Threading.Tasks;
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
        public async Task Can_Execute_Query_On_Schema_With_Injections()
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
        public async Task Each_Executor_Should_Use_Its_Own_Injector()
        {
            var engine = GraphQLEngine.New<Query>();

            var executor1 = engine
                .NewExecutor()
                .WithQueryString("{ field }")
                .WithDependencyInjector(new DependencyInjector("Injector1"));

            var executor2 = engine
                .NewExecutor()
                .WithQueryString("{ field }")
                .WithDependencyInjector(new DependencyInjector("Injector2"));

            var result1 = await executor1.Execute();
            var result2 = await executor2.Execute();

            result1.ShouldHaveNoErrors();
            result1.Data.ShouldHaveFieldWithValue("field", "Injector1");

            result2.ShouldHaveNoErrors();
            result2.Data.ShouldHaveFieldWithValue("field", "Injector2");
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
        public async Task Can_Execute_Query_On_Schema_With_Injections_In_Generic_Methods()
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
            private readonly string _value;

            public Repository(string value)
            {
                _value = value;
            }

            public string GetValue() => _value;
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
            private readonly string _value;

            public DependencyInjector(string value = "Some Value")
            {
                _value = value;
            }

            public object Resolve(TypeInfo typeInfo)
            {
                if (typeInfo.AsType() == typeof(Query))
                {
                    return new Query(new Repository(_value));
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