using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.Conventions
{
    public interface IDataLoaderContextProvider : IUserContext
    {
        Task FetchData(CancellationToken token);
    }
}