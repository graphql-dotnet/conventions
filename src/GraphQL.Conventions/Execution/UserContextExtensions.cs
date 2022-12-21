using GraphQL.Execution;

namespace GraphQL.Conventions.Execution
{
    public static class UserContextExtensions
    {
        public static IUserContext GetUserContext(this IProvideUserContext context) => context.UserContext as IUserContext;
    }
}
