using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes.Execution.Wrappers
{
    public interface IWrapper
    {
        object Wrap(GraphTypeInfo type, object value);
    }
}