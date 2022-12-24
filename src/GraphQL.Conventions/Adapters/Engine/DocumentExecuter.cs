using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Types;
using GraphQL.Validation;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    [Obsolete("Please use the GraphQL.DocumentExecuter class directly.")]
    public class DocumentExecuter : IDocumentExecuter
    {
        private readonly GraphQLEngine _engine;
        private readonly IServiceProvider _serviceProvider;

        public DocumentExecuter(GraphQLEngine engine, IServiceProvider serviceProvider)
        {
            _engine = engine;
            _serviceProvider = serviceProvider;
        }

        public async Task<ExecutionResult> ExecuteAsync(ISchema schema, object root, string query, string operationName, Inputs inputs, object userContext, CancellationToken cancellationToken, IEnumerable<IValidationRule> rules)
        {
            return await _engine
                .NewExecutor()
                .WithServiceProvider(_serviceProvider)
                .WithRootObject(root)
                .WithQueryString(query)
                .WithOperationName(operationName)
                .WithVariables(inputs)
                .WithCancellationToken(cancellationToken)
                .WithValidationRules(rules)
                .ExecuteAsync();
        }

        public async Task<ExecutionResult> ExecuteAsync(ExecutionOptions options)
        {
            return await _engine
                .NewExecutor()
                .WithServiceProvider(_serviceProvider)
                .WithRootObject(options.Root)
                .WithQueryString(options.Query)
                .WithOperationName(options.OperationName)
                .WithVariables(options.Variables)
                .WithCancellationToken(options.CancellationToken)
                .WithValidationRules(options.ValidationRules)
                .ExecuteAsync();
        }

        public Task<ExecutionResult> ExecuteAsync(Action<ExecutionOptions> configure)
        {
            throw new NotImplementedException();
        }
    }
}
