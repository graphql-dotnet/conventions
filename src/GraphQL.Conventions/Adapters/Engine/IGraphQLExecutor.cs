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
    public interface IGraphQLExecutor<TResult>
    {
        IGraphQLExecutor<TResult> WithRequest(string requestBody);

        IGraphQLExecutor<TResult> WithQueryString(string queryString);

        IGraphQLExecutor<TResult> WithOperationName(string operationName);

        IGraphQLExecutor<TResult> WithVariables(Inputs inputs);

        IGraphQLExecutor<TResult> WithVariables(Dictionary<string, object> inputs);

        IGraphQLExecutor<TResult> WithRootObject(object rootValue);

        IGraphQLExecutor<TResult> WithUserContext(IUserContext userContext);

        IGraphQLExecutor<TResult> WithCancellationToken(CancellationToken token);

        IGraphQLExecutor<TResult> WithDependencyInjector(IDependencyInjector injector);

        IGraphQLExecutor<TResult> WithValidationRules(IEnumerable<IValidationRule> rules);

        [Obsolete("Please use the WithComplexityOptions method instead.")]
        IGraphQLExecutor<TResult> WithComplexityConfiguration(LegacyComplexityConfiguration complexityConfiguration);

        IGraphQLExecutor<TResult> WithComplexityOptions(ComplexityOptions complexityOptions);

        IGraphQLExecutor<TResult> WithListeners(params IDocumentExecutionListener[] listeners);

        IGraphQLExecutor<TResult> EnableValidation(bool enableValidation = true);

        IGraphQLExecutor<TResult> DisableValidation();

        IGraphQLExecutor<TResult> EnableProfiling(bool enableProfiling = true);

        IGraphQLExecutor<TResult> DisableProfiling();

        Task<TResult> ExecuteAsync();

        Task<IValidationResult> ValidateAsync();
    }
}
