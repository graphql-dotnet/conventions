using System.Threading.Tasks;
using GraphQL.Execution;
using GraphQL.Validation;

namespace GraphQL.Conventions.Adapters.Engine.Listeners.DataLoader
{
    internal class DataLoaderListener : DocumentExecutionListenerBase
    {
        public override async Task AfterValidationAsync(IExecutionContext context, IValidationResult validationResult)
        {
            var key = typeof(IUserContext).FullName ?? nameof(IUserContext);
            if (context.UserContext.ContainsKey(key) && context.UserContext[key] is IDataLoaderContextProvider provider)
                await provider.FetchData(context.CancellationToken).ConfigureAwait(false);
        }
    }
}
