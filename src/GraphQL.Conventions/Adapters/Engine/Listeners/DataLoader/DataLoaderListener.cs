using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Execution;

namespace GraphQL.Conventions.Adapters.Engine.Listeners.DataLoader
{
    class DataLoaderListener : DocumentExecutionListenerBase<IDictionary<string, object>>
    {
        public override async Task BeforeExecutionStepAwaitedAsync(IDictionary<string, object> userContext, CancellationToken token)
        {
            var key = typeof(IUserContext).FullName;
            if (userContext.ContainsKey(key) && userContext[key] is IDataLoaderContextProvider provider)
                await provider.FetchData(token).ConfigureAwait(false);
        }
    }
}
