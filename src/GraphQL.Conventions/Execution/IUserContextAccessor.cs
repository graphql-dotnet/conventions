namespace GraphQL.Conventions.Execution
{
    public interface IUserContextAccessor
    {
        IUserContext UserContext { get; }
    }
}