using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    public interface IDataLoaderContextProvider : IUserContext
    {
        Task FetchData(CancellationToken token);
    }
}
