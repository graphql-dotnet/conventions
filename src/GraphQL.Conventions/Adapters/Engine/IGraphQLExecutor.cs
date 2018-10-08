using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Execution;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;

namespace GraphQL.Conventions
{
    public interface IGraphQLExecutor<TResult>
    {
        IGraphQLExecutor<TResult> WithRequest(string requestBody);

        IGraphQLExecutor<TResult> WithQueryString(string queryString);

        IGraphQLExecutor<TResult> WithOperationName(string operationName);

        IGraphQLExecutor<TResult> WithInputs(Dictionary<string, object> inputs);

        IGraphQLExecutor<TResult> WithRootObject(object rootValue);

        IGraphQLExecutor<TResult> WithUserContext(IUserContext userContext);

        IGraphQLExecutor<TResult> WithCancellationToken(CancellationToken token);

        IGraphQLExecutor<TResult> WithDependencyInjector(IDependencyInjector injector);

        IGraphQLExecutor<TResult> WithValidationRules(IEnumerable<IValidationRule> rules);

        IGraphQLExecutor<TResult> WithComplexityConfiguration(ComplexityConfiguration complexityConfiguration);

        IGraphQLExecutor<TResult> WithListeners(params IDocumentExecutionListener[] listeners);

        IGraphQLExecutor<TResult> EnableValidation(bool enableValidation = true);

        IGraphQLExecutor<TResult> DisableValidation();

        IGraphQLExecutor<TResult> EnableProfiling(bool enableProfiling = true);

        IGraphQLExecutor<TResult> DisableProfiling();
        
        Task<TResult> Execute();
    }
}
