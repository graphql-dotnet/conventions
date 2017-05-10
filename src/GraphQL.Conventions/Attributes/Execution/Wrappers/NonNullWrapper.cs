using System;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes.Execution.Wrappers
{
    public class NonNullWrapper : WrapperBase
    {
        public override object WrapValue(GraphEntityInfo entityInfo, GraphTypeInfo typeInfo, object value)
        {
            if (!typeInfo.IsNullable)
            {
                if (value == null)
                {
                    throw new ArgumentException($"Null value provided for non-nullable {GetEntityDescription(entityInfo)} '{entityInfo.Name}'.");
                }
                return NonNull.Construct(typeInfo, value);
            }
            return value;
        }
    }
}