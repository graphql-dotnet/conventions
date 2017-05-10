using System;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions.Attributes.Execution.Wrappers
{
    public class PrimitiveWrapper : WrapperBase
    {
        public override object WrapValue(GraphEntityInfo entityInfo, GraphTypeInfo typeInfo, object value)
        {
            if (typeInfo.IsPrimitive &&
                !typeInfo.IsEnumerationType &&
                value is IConvertible)
            {
                try
                {
                    return Convert.ChangeType(value, typeInfo.GetTypeRepresentation().AsType());
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Unable to cast {GetEntityDescription(entityInfo)} '{entityInfo.Name}' to '{typeInfo.Name}'.", ex);
                }
            }
            return value;
        }
    }
}