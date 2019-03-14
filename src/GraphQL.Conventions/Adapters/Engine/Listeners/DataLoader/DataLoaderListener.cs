using System.Threading;
using System.Threading.Tasks;
using GraphQL.Execution;

namespace GraphQL.Conventions.Adapters.Engine.Listeners.DataLoader
{
    class DataLoaderListener : DocumentExecutionListenerBase<IDataLoaderContextProvider>
    {
        public override Task BeforeExecutionStepAwaitedAsync(
            IDataLoaderContextProvider userContext,
            CancellationToken token)
        {
            return userContext.FetchData(token);
        }
    }
}
