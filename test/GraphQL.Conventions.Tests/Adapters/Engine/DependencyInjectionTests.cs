using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Conventions;
using GraphQL.Conventions.Execution;
using GraphQL.Execution;
using GraphQL.Language.AST;
using Tests.Templates;
using Tests.Templates.Extensions;

// ReSharper disable UnusedMember.Local

namespace Tests.Adapters.Engine
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
                .ExecuteAsync();

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

            var result1 = await executor1.ExecuteAsync();
            var result2 = await executor2.ExecuteAsync();

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

            var result1 = await executor1.ExecuteAsync();

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
                .ExecuteAsync();

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
                if (typeInfo.GetConstructor(Type.EmptyTypes) != null)
                {
                    return Activator.CreateInstance(typeInfo.AsType());
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
                var injector = GetChildInjector(context.GetDependencyInjector());
                return new ScopedExecutionStrategy(injector, base.SelectExecutionStrategy(context));
            }

            private IDependencyInjector GetChildInjector(IDependencyInjector dependencyInjector)
                => dependencyInjector.Resolve<ChildDependencyInjector>() ?? dependencyInjector;

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
                    var key = typeof(IDependencyInjector).FullName ?? nameof(IDependencyInjector);
                    var outerInjector = context.UserContext[key];

                    try
                    {
                        context.UserContext[key] = _injector;
                        return await _innerStrategy.ExecuteAsync(context);
                    }
                    finally
                    {
                        context.UserContext[key] = outerInjector;
                    }
                }

                public Dictionary<string, Field> GetSubFields(ExecutionContext executionContext, ExecutionNode executionNode)
                {
                    return _innerStrategy.GetSubFields(executionContext, executionNode);
                }
            }
        }
    }
}
