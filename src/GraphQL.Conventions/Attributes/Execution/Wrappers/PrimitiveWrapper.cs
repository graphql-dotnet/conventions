using System;
using System.ComponentModel;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions.Attributes.Execution.Wrappers
{
    public class PrimitiveWrapper : WrapperBase
    {
        public override object WrapValue(GraphEntityInfo entityInfo, GraphTypeInfo typeInfo, object value, bool isSpecified)
        {
            if (value == null || !typeInfo.IsPrimitive || typeInfo.IsEnumerationType) return value;

            try
            {
                var targetType = typeInfo.GetTypeRepresentation().AsType();

                var converter = TypeDescriptor.GetConverter(targetType);
                if (converter.CanConvertFrom(value.GetType()))
                    return converter.ConvertFrom(value);

                if (value is IConvertible)
                    return Convert.ChangeType(value, targetType);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Unable to cast {GetEntityDescription(entityInfo)} '{entityInfo.Name}' to '{typeInfo.Name}'.", ex);
            }
            return value;
        }
    }
}
