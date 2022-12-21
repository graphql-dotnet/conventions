using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Conventions;
using GraphQL.Execution;
using GraphQL.Types;
using GraphQLParser.AST;
using Microsoft.Extensions.DependencyInjection;
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
                .WithServiceProvider(new DependencyInjector())
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
                .WithServiceProvider(new DependencyInjector("Injector1"));

            var executor2 = engine
                .NewExecutor()
                .WithQueryString("{ field }")
                .WithServiceProvider(new DependencyInjector("Injector2"));

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
                .WithServiceProvider(new DependencyInjector("Injector"));

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
                .WithServiceProvider(new DependencyInjector())
                .ExecuteAsync();

            result.ShouldHaveNoErrors();
            result.Data.ShouldHaveFieldWithValue("withDependency", 3);
        }

        private class Query
        {
            private readonly IRepository _repository;

            public Query(IRepository repository)
            {
                _repository = repository;
            }

            public string Field => _repository.GetValue();
        }

        private interface IRepository
        {
            string GetValue();
        }

        private class Repository : IRepository
        {
            private readonly string _value;

            public Repository(string value)
            {
                _value = value;
            }

            public string GetValue() => _value;
        }

        private class QueryWithDIFields
        {
            public int WithDependency([Inject] IDependency d) => d.Get(3);
        }

        private interface IDependency
        {
            T Get<T>(T value);
        }

        private class Dependency : IDependency
        {
            public T Get<T>(T value) => value;
        }

        private class DependencyInjector : IServiceProvider
        {
            private readonly string _value;

            public DependencyInjector(string value = "Some Value")
            {
                _value = value;
            }

            public object GetService(Type type)
            {
                if (type == typeof(Query))
                {
                    return new Query(new Repository(_value));
                }
                if (type == typeof(IDependency))
                {
                    return new Dependency();
                }
                if (type == typeof(ChildDependencyInjector))
                {
                    return new ChildDependencyInjector($"{_value}->ChildScope");
                }
                if (type.GetConstructor(Type.EmptyTypes) != null)
                {
                    return Activator.CreateInstance(type);
                }
                return null;
            }
        }

        private class ChildDependencyInjector : DependencyInjector
        {
            public ChildDependencyInjector(string value) : base(value) { }
        }

        private class ScopedDocumentExecuter : GraphQL.DocumentExecuter
        {
            protected override IExecutionStrategy SelectExecutionStrategy(ExecutionContext context)
            {
                var injector = GetChildInjector(context.RequestServices);
                return new ScopedExecutionStrategy(injector, base.SelectExecutionStrategy(context));
            }

            private IServiceProvider GetChildInjector(IServiceProvider serviceProvider)
                => serviceProvider.GetService<ChildDependencyInjector>() ?? serviceProvider;

            private class ScopedExecutionStrategy : IExecutionStrategy
            {
                private readonly IServiceProvider _serviceProvider;
                private readonly IExecutionStrategy _innerStrategy;

                public ScopedExecutionStrategy(IServiceProvider serviceProvider, IExecutionStrategy innerStrategy)
                {
                    _serviceProvider = serviceProvider;
                    _innerStrategy = innerStrategy;
                }


                public Task<ExecutionResult> ExecuteAsync(ExecutionContext context)
                {
                    return _innerStrategy.ExecuteAsync(new ExecutionContext(context)
                    {
                        RequestServices = _serviceProvider
                    });
                }

                public async Task ExecuteNodeTreeAsync(ExecutionContext context, ExecutionNode rootNode)
                {
                    throw new NotImplementedException();
                }

                Dictionary<string, (GraphQLField field, FieldType fieldType)> IExecutionStrategy.GetSubFields(ExecutionContext executionContext, ExecutionNode executionNode)
                {
                    return _innerStrategy.GetSubFields(executionContext, executionNode);
                }
            }
        }
    }
}
