using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.Conventions.Execution
{
    public class UserContextWrapper : IUserContextAccessor, IDependencyInjectorAccessor
    {
        public static UserContextWrapper Create(IUserContext userContext, IDependencyInjector dependencyInjector)
        {
            if (userContext is IDataLoaderContextProvider)
            {
                return new UserContextWithDataLoaderContextProvider
                {
                    UserContext = userContext,
                    DependencyInjector = dependencyInjector
                };
            }

            return new UserContextWrapper
            {
                UserContext = userContext,
                DependencyInjector = dependencyInjector
            };
        }

        public IUserContext UserContext { get; private set; }

        public IDependencyInjector DependencyInjector { get; private set; }

        private class UserContextWithDataLoaderContextProvider : UserContextWrapper, IDataLoaderContextProvider
        {
            public Task FetchData(CancellationToken token) => (UserContext as IDataLoaderContextProvider)?.FetchData(token) ?? Task.FromResult(0);
        }
    }
}
