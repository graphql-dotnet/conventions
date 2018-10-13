using System.Threading;
using System.Threading.Tasks;
using GraphQL.Conventions;
using GraphQL.DataLoader;

namespace DataLoaderWithEFCore.GraphApi
{
    public class UserContext : IUserContext, IDataLoaderContextProvider
    {
        private readonly DataLoaderContext _dataLoaderContext;

        public UserContext(DataLoaderContext dataLoaderContext)
        {
            _dataLoaderContext = dataLoaderContext;
        }

        public Task FetchData(CancellationToken token)
        {
            _dataLoaderContext.DispatchAll(token);
            return Task.CompletedTask;
        }
    }
}
