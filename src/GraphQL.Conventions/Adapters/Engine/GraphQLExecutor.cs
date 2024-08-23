using System;
using System.Collections.Generic;
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

        private IDependencyInjector _dependencyInjector;

        private bool _enableValidation = true;

        private bool _enableProfiling;

        private IEnumerable<IValidationRule> _validationRules;

        private LegacyComplexityConfiguration _complexityConfiguration;
        private ComplexityOptions _complexityOptions;

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

        public IGraphQLExecutor<ExecutionResult> WithDependencyInjector(IDependencyInjector injector)
        {
            _dependencyInjector = injector;
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

        [Obsolete("Please use the WithComplexityOptions method instead.")]
        public IGraphQLExecutor<ExecutionResult> WithComplexityConfiguration(LegacyComplexityConfiguration complexityConfiguration)
        {
            _complexityConfiguration = complexityConfiguration;
            return this;
        }

        public IGraphQLExecutor<ExecutionResult> WithComplexityOptions(ComplexityOptions complexityOptions)
        {
            _complexityOptions = complexityOptions;
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
                _rootObject, _queryString, _operationName, _inputs, _userContext, _dependencyInjector,
                enableValidation: _enableValidation,
                enableProfiling: _enableProfiling,
                rules: _validationRules,
                complexityConfiguration: _complexityConfiguration,
                complexityOptions: _complexityOptions,
                cancellationToken: _cancellationToken,
                listeners: _documentExecutionListeners);

        public Task<IValidationResult> ValidateAsync() => _engine.ValidateAsync(_queryString);
    }
}
