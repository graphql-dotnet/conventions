namespace GraphQL.Conventions.Execution
{
    internal class UserContextWrapper
    {
        public UserContextWrapper(IUserContext userContext, IDependencyInjector dependencyInjector)
        {
            UserContext = userContext;
            DependencyInjector = dependencyInjector;
        }

        public IUserContext UserContext { get; }

        public IDependencyInjector DependencyInjector { get; }
    }
}
