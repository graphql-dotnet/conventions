using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Execution;
using GraphQL.Validation;

namespace GraphQL.Conventions.Adapters.Engine.Listeners.DataLoader
{
    class DataLoaderListener : DocumentExecutionListenerBase
    {
        //public override async Task BeforeExecutionStepAwaitedAsync(IDictionary<string, object> userContext, CancellationToken token)
        public override async Task AfterValidationAsync(IExecutionContext context, IValidationResult validationResult)
        {
            var userContext = (IDictionary<string, object>)context.UserContext;
            var key = typeof(IUserContext).FullName;
            if (userContext.ContainsKey(key) && userContext[key] is IDataLoaderContextProvider provider)
                await provider.FetchData(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
