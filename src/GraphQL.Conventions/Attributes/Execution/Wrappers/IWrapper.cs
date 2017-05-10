using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes.Execution.Wrappers
{
    public interface IWrapper
    {
        object Wrap(GraphEntityInfo entity, GraphTypeInfo type, object value);
    }
}