using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Execution;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;

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

        private CancellationToken _cancellationToken = default(CancellationToken);

        private IDependencyInjector _dependencyInjector;

        private bool _enableValidation = true;

        private bool _enableProfiling = false;

        private IEnumerable<IValidationRule> _validationRules = null;

        private ComplexityConfiguration _complexityConfiguration = null;

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
            return this.WithInputs(query.Variables);
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

        public IGraphQLExecutor<ExecutionResult> WithInputs(Dictionary<string, object> inputs)
        {
            _inputs = inputs != null ? new Inputs(inputs) : new Inputs();
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

        public IGraphQLExecutor<ExecutionResult> WithComplexityConfiguration(ComplexityConfiguration complexityConfiguration)
        {
            _complexityConfiguration = complexityConfiguration;
            return this;
        }

        public IGraphQLExecutor<ExecutionResult> DisableValidation()
        {
            return this.EnableValidation(false);
        }

        public IGraphQLExecutor<ExecutionResult> EnableProfiling(bool enableProfiling = true)
        {
            _enableProfiling = enableProfiling;
            return this;
        }

        public IGraphQLExecutor<ExecutionResult> DisableProfiling()
        {
            return this.EnableProfiling(false);
        }

        public async Task<ExecutionResult> Execute() =>
            await _engine
                .Execute(
                    _rootObject, _queryString, _operationName, _inputs, _userContext, _dependencyInjector,
                    enableValidation: _enableValidation,
                    enableProfiling: _enableProfiling,
                    rules: _validationRules,
                    complexityConfiguration: _complexityConfiguration,
                    cancellationToken: _cancellationToken,
                    listeners: _documentExecutionListeners)
                .ConfigureAwait(false);

        public IValidationResult Validate() => _engine.Validate(_queryString);
    }
}