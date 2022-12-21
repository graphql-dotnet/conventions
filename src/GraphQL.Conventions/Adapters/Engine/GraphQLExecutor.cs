using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Execution;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    public class GraphQLExecutor : IGraphQLExecutor<ExecutionResult>
    {
        private readonly GraphQLEngine _engine;

        private readonly IRequestDeserializer _requestDeserializer;

        private object _rootObject;

        private IUserContext _userContext;

        private string _queryString;

        private string _operationName;

        private Inputs _inputs;

        private CancellationToken _cancellationToken;

        private IServiceProvider _serviceProvider;

        private bool _enableValidation = true;

        private bool _enableProfiling;

        private IEnumerable<IValidationRule> _validationRules;

        private ComplexityConfiguration _complexityConfiguration;

        private IEnumerable<IDocumentExecutionListener> _documentExecutionListeners;

        public GraphQLExecutor(GraphQLEngine engine, IRequestDeserializer requestDeserializer)
        {
            _engine = engine;
            _requestDeserializer = requestDeserializer;
        }

        public IGraphQLExecutor<ExecutionResult> WithRequest(string requestBody)
        {
            var query = _requestDeserializer.GetQueryFromRequestBody(requestBody);
            _queryString = query.QueryString;
            _operationName = query.OperationName;
            return WithVariables(query.Variables);
        }

        public IGraphQLExecutor<ExecutionResult> WithQueryString(string queryString)
        {
            _queryString = queryString;
            return this;
        }

        public IGraphQLExecutor<ExecutionResult> WithOperationName(string operationName)
        {
            _operationName = operationName;
            return this;
        }

        public IGraphQLExecutor<ExecutionResult> WithVariables(Dictionary<string, object> inputs)
        {
            return WithVariables(new Inputs(inputs ?? new Dictionary<string, object>()));
        }

        public IGraphQLExecutor<ExecutionResult> WithVariables(Inputs inputs)
        {
            _inputs = inputs;
            return this;
        }

        public IGraphQLExecutor<ExecutionResult> WithRootObject(object rootValue)
        {
            _rootObject = rootValue;
            return this;
        }

        public IGraphQLExecutor<ExecutionResult> WithUserContext(IUserContext userContext)
        {
            _userContext = userContext;
            return this;
        }

        public IGraphQLExecutor<ExecutionResult> WithCancellationToken(CancellationToken token)
        {
            _cancellationToken = token;
            return this;
        }

        [Obsolete("Please use WithServiceProvider instead.")]
        public IGraphQLExecutor<ExecutionResult> WithDependencyInjector(IDependencyInjector injector)
        {
            _serviceProvider = new DependencyInjectorWrapper(injector);
            return this;
        }

        private class DependencyInjectorWrapper : IServiceProvider
        {
            private readonly IDependencyInjector _injector;

            public DependencyInjectorWrapper(IDependencyInjector injector)
            {
                _injector = injector;
            }

            public object GetService(Type serviceType) => _injector.Resolve(serviceType.GetTypeInfo());
        }

        public IGraphQLExecutor<ExecutionResult> WithServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            return this;
        }

        public IGraphQLExecutor<ExecutionResult> WithValidationRules(IEnumerable<IValidationRule> rules)
        {
            _validationRules = rules;
            return this;
        }

        public IGraphQLExecutor<ExecutionResult> WithListeners(params IDocumentExecutionListener[] listeners)
        {
            _documentExecutionListeners = listeners;
            return this;
        }

        public IGraphQLExecutor<ExecutionResult> EnableValidation(bool enableValidation = true)
        {
            _enableValidation = enableValidation;
            return this;
        }

        public IGraphQLExecutor<ExecutionResult> WithComplexityConfiguration(ComplexityConfiguration complexityConfiguration)
        {
            _complexityConfiguration = complexityConfiguration;
            return this;
        }

        public IGraphQLExecutor<ExecutionResult> DisableValidation()
        {
            return EnableValidation(false);
        }

        public IGraphQLExecutor<ExecutionResult> EnableProfiling(bool enableProfiling = true)
        {
            _enableProfiling = enableProfiling;
            return this;
        }

        public IGraphQLExecutor<ExecutionResult> DisableProfiling()
        {
            return EnableProfiling(false);
        }

        public Task<ExecutionResult> ExecuteAsync()
            => _engine.ExecuteAsync(
                _rootObject, _queryString, _operationName, _inputs, _userContext, _serviceProvider,
                enableValidation: _enableValidation,
                enableProfiling: _enableProfiling,
                rules: _validationRules,
                complexityConfiguration: _complexityConfiguration,
                cancellationToken: _cancellationToken,
                listeners: _documentExecutionListeners);

        public Task<IValidationResult> ValidateAsync() => _engine.ValidateAsync(_queryString);
    }
}
