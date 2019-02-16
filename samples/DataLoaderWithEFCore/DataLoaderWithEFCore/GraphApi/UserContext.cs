using System.Threading;
using System.Threading.Tasks;
using GraphQL.Conventions;
using GraphQL.DataLoader;

namespace DataLoaderWithEFCore.GraphApi
{
    public class UserContext : IUserContext, IDataLoaderContextProvider
    {
        public DataLoaderContext _context { get; private set; }

        public UserContext(DataLoaderContext context)
        {
            _context = context;
        }

        public Task FetchData(CancellationToken token)
        {
            return _context.DispatchAllAsync(token);
        }
    }
}
