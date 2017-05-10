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

        public override object WrapValue(GraphEntityInfo entityInfo, GraphTypeInfo typeInfo, object value)
        {
            var input = value as Dictionary<string, object>;
            if (typeInfo.IsInputType && input != null)
            {
                var obj = Activator.CreateInstance(typeInfo.GetTypeRepresentation().AsType());
                foreach (var field in typeInfo.Fields.Where(field => !field.IsIgnored))
                {
                    object fieldValue;
                    if (!input.TryGetValue(field.Name, out fieldValue))
                    {
                        if (!field.Type.IsNullable && field.DefaultValue == null)
                        {
                            throw new Exception($"Value for non-nullable field '{field.Name}' not provided.");
                        }
                        fieldValue = field.DefaultValue;
                    }
                    var propertyInfo = field.AttributeProvider as PropertyInfo;
                    if (propertyInfo == null)
                    {
                        throw new Exception($"Invalid field '{field.Name}' on input object; must be a property.");
                    }
                    propertyInfo.SetValue(obj, _parent.Wrap(field, field.Type, fieldValue));
                }
                return obj;
            }
            return value;
        }
    }
}