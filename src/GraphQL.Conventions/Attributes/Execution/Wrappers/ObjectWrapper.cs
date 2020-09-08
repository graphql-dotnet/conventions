using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;
using GraphQL.Language.AST;

namespace GraphQL.Conventions.Attributes.Execution.Wrappers
{
    public class ObjectWrapper : WrapperBase
    {
        public ObjectWrapper(IWrapper parent)
            : base(parent)
        {
        }

        public override object WrapValue(GraphEntityInfo entityInfo, GraphTypeInfo typeInfo, object value, bool isSpecified)
        {
            bool isNull = value == null;
            bool shouldReturnValue = isNull;
            
            if (shouldReturnValue)
            {
                return value;
            }
            
            Type valueType = value.GetType();
            
            bool isPrimitive = typeInfo.IsPrimitive;
            isPrimitive |= valueType.GetTypeInfo().IsPrimitive;
            isPrimitive |= value is string;
            
            shouldReturnValue |= isPrimitive;
            
            if(shouldReturnValue)
            {
                return value;
            }
            
            bool isNonNull = typeof(NonNull<string>).Name == typeInfo.TypeRepresentation.Name;
            shouldReturnValue |= isNonNull
                ? typeInfo.TypeRepresentation.GenericTypeArguments.First() == valueType
                : typeInfo.TypeRepresentation.AsType() == valueType;
            shouldReturnValue |= isPrimitive || isNull;

            if (shouldReturnValue)
            {
                return value;
            }

            if (typeInfo.IsScalarType)
            {
                return Activator.CreateInstance(typeInfo.GetTypeRepresentation().AsType(), value);
            }
            else if (typeInfo.IsInputType && value is Dictionary<string, object> input)
            {
                var obj = Activator.CreateInstance(typeInfo.GetTypeRepresentation().AsType());
                foreach (var field in typeInfo.Fields.Where(field => !field.IsIgnored))
                {
                    object fieldValue;
                    isSpecified = true;
                    if (!input.TryGetValue(field.Name, out fieldValue))
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
            return value;
        }
    }
}