using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes.Execution.Wrappers
{
    public class NonNullWrapper : WrapperBase
    {
        public override object WrapValue(GraphTypeInfo typeInfo, object value)
        {
            if (!typeInfo.IsNullable)
            {
                return NonNull.Construct(typeInfo, value);
            }
            return value;
        }
    }
}