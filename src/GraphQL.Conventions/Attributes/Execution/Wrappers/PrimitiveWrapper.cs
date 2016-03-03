using System;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions.Attributes.Execution.Wrappers
{
    public class PrimitiveWrapper : WrapperBase
    {
        public override object WrapValue(GraphTypeInfo typeInfo, object value)
        {
            if (typeInfo.IsPrimitive &&
                !typeInfo.IsEnumerationType &&
                value is IConvertible)
            {
                return Convert.ChangeType(value, typeInfo.GetTypeRepresentation().AsType());
            }
            return value;
        }
    }
}