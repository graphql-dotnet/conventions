using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.Conventions
{
    public interface IDataLoaderContextProvider
    {
        Task FetchData(CancellationToken token);
    }
}