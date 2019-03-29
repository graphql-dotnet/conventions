using GraphQL.Types;
using GraphQL.Validation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.Conventions
{
    public class DocumentExecuter : IDocumentExecuter
    {
        private readonly GraphQLEngine _engine;
        private readonly IDependencyInjector _dependencyInjector;

        public DocumentExecuter(GraphQLEngine engine, IDependencyInjector dependencyInjector)
        {
            _engine = engine;
            _dependencyInjector = dependencyInjector;
        }

        public async Task<ExecutionResult> ExecuteAsync(ISchema schema, object root, string query, string operationName, Inputs inputs, object userContext, CancellationToken cancellationToken, IEnumerable<IValidationRule> rules)
        {
            return await _engine
                .NewExecutor()
                .WithDependencyInjector(_dependencyInjector)
                .WithRootObject(root)
                .WithQueryString(query)
                .WithOperationName(operationName)
                .WithInputs(inputs)
                .WithCancellationToken(cancellationToken)
                .WithValidationRules(rules)
                .Execute();
        }

        public async Task<ExecutionResult> ExecuteAsync(ExecutionOptions options)
        {
            return await _engine
                .NewExecutor()
                .WithDependencyInjector(_dependencyInjector)
                .WithRootObject(options.Root)
                .WithQueryString(options.Query)
                .WithOperationName(options.OperationName)
                .WithInputs(options.Inputs)
                .WithCancellationToken(options.CancellationToken)
                .WithValidationRules(options.ValidationRules)
                .Execute();
        }

        public Task<ExecutionResult> ExecuteAsync(Action<ExecutionOptions> configure)
        {
            throw new NotImplementedException();
        }
    }
}