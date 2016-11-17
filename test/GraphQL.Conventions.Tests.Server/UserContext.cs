using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Conventions.Adapters.Engine.Listeners.DataLoader;
using GraphQL.Conventions.Execution;

namespace GraphQL.Conventions.Tests.Server
{
    public class UserContext : IUserContext, IDataLoaderContextProvider
    {
        public List<string> TouchedIds { get; private set; } = new List<string>();

        public Task FetchData(CancellationToken token)
        {
            System.Console.WriteLine($"FetchData(): {string.Join(", ", TouchedIds)}");
            return Task.CompletedTask;
        }
    }
}
