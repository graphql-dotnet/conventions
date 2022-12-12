namespace GraphQL.Conventions.Attributes.Execution.Unwrappers
{
    public interface IUnwrapper
    {
        object Unwrap(object value);
    }
}
