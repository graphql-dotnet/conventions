using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Execution;

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
        public async Task Executor_Should_Use_UserContext_Injector()
        {
            var engine = new GraphQLEngine(documentExecutor: new ScopedDocumentExecuter())
                .WithQuery<Query>()
                .BuildSchema();

            var executor1 = engine
                .NewExecutor()
                .WithQueryString("{ field }")
                .WithDependencyInjector(new DependencyInjector("Injector"));

            var result1 = await executor1.Execute();

            result1.ShouldHaveNoErrors();
            result1.Data.ShouldHaveFieldWithValue("field", "Injector->ChildScope");
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
                if (typeInfo.AsType() == typeof(ChildDependencyInjector))
                {
                    return new ChildDependencyInjector($"{_value}->ChildScope");
                }
                return null;
            }
        }

        class ChildDependencyInjector : DependencyInjector
        {
            public ChildDependencyInjector(string value) : base(value) { }
        }

        class ScopedDocumentExecuter : GraphQL.DocumentExecuter
        {
            protected override IExecutionStrategy SelectExecutionStrategy(ExecutionContext context)
            {
                var injector = GetChildInjector(context.UserContext as IDependencyInjectorAccessor);
                return new ScopedExecutionStrategy(injector, base.SelectExecutionStrategy(context));
            }

            private IDependencyInjector GetChildInjector(IDependencyInjectorAccessor dependencyInjectorAccessor)
                => dependencyInjectorAccessor?.DependencyInjector.Resolve<ChildDependencyInjector>() ?? dependencyInjectorAccessor?.DependencyInjector;

            private class ScopedExecutionStrategy : IExecutionStrategy
            {
                private readonly IDependencyInjector _injector;
                private readonly IExecutionStrategy _innerStrategy;

                public ScopedExecutionStrategy(IDependencyInjector injector, IExecutionStrategy innerStrategy)
                {
                    _injector = injector;
                    _innerStrategy = innerStrategy;
                }

                public async Task<ExecutionResult> ExecuteAsync(ExecutionContext context)
                {
                    var outerUserContextWrapper = context.UserContext;
                    var userContext = (context.UserContext as IUserContextAccessor)?.UserContext;

                    try
                    {
                        context.UserContext = UserContextWrapper.Create(userContext, _injector);
                        return await _innerStrategy.ExecuteAsync(context);
                    }
                    finally
                    {
                        context.UserContext = outerUserContextWrapper;
                    }
                }
            }
        }
    }
}