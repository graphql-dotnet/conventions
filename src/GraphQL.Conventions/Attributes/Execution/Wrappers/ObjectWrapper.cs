using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions.Attributes.Execution.Wrappers
{
    public class ObjectWrapper : WrapperBase
    {
        public ObjectWrapper(IWrapper parent)
            : base(parent)
        {
        }

        public override object WrapValue(GraphEntityInfo entityInfo, GraphTypeInfo typeInfo, object value, bool _)
        {
            var isNull = value == null;
            var shouldReturnValue = isNull;

            if (shouldReturnValue) return null;

            var valueType = value.GetType();

            var isPrimitive = typeInfo.IsPrimitive;
            isPrimitive |= valueType.GetTypeInfo().IsPrimitive;
            isPrimitive |= value is string;

            shouldReturnValue |= isPrimitive;

            if (shouldReturnValue)
            {
                return value;
            }

            var isNonNull = typeof(NonNull<string>).Name == typeInfo.TypeRepresentation.Name;
            shouldReturnValue |= isNonNull
                ? typeInfo.TypeRepresentation.GenericTypeArguments.First() == valueType
                : typeInfo.TypeRepresentation.AsType() == valueType;

            if (shouldReturnValue)
            {
                return value;
            }

            if (typeInfo.IsScalarType)
            {
                return Activator.CreateInstance(typeInfo.GetTypeRepresentation().AsType(), value);
            }

            if (!typeInfo.IsInputType || !(value is Dictionary<string, object> input)) return value;

            var obj = Activator.CreateInstance(typeInfo.GetTypeRepresentation().AsType());
            foreach (var field in typeInfo.Fields.Where(field => !field.IsIgnored))
            {
                var isSpecified = true;
                if (!input.TryGetValue(field.Name, out var fieldValue))
                {
                    if (!field.Type.IsNullable && field.DefaultValue == null)
                    {
                        throw new Exception($"Value for non-nullable field '{field.Name}' not provided.");
                    }
                    isSpecified = false;
                    fieldValue = field.DefaultValue;
                }

                if (!(field.AttributeProvider is PropertyInfo propertyInfo))
                {
                    throw new Exception($"Invalid field '{field.Name}' on input object; must be a property.");
                }
                propertyInfo.SetValue(obj, _parent.Wrap(field, field.Type, fieldValue, isSpecified));
            }
            return obj;
        }
    }
}
