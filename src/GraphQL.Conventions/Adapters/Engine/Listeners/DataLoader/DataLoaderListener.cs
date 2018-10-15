using System.Threading;
using System.Threading.Tasks;
using GraphQL.Execution;

namespace GraphQL.Conventions.Adapters.Engine.Listeners.DataLoader
{
    class DataLoaderListener : DocumentExecutionListenerBase<IDataLoaderContextProvider>
    {
        public override async Task BeforeExecutionStepAwaitedAsync(
            IDataLoaderContextProvider userContext,
            CancellationToken token)
        {
            await userContext.FetchData(token).ConfigureAwait(false);
        }
    }
}
