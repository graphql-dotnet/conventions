using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Profiling;
using GraphQL.Conventions.Types.Resolution;

namespace GraphQL.Conventions.Adapters.Engine
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

        IGraphQLExecutor<TResult> WithProfiler(IProfiler profiler);

        IGraphQLExecutor<TResult> UseValidation(bool useValidation = true);

        Task<TResult> Execute();
    }
}
