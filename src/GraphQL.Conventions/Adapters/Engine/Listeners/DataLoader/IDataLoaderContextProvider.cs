using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.Conventions.Adapters.Engine.Listeners.DataLoader
{
    public interface IDataLoaderContextProvider
    {
        Task FetchData(CancellationToken token);
    }
}