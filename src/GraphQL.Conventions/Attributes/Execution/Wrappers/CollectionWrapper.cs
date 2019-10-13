using System;
using System.Collections;
using System.Collections.Generic;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions.Attributes.Execution.Wrappers
{
    public class CollectionWrapper : WrapperBase
    {
        public CollectionWrapper(IWrapper parent)
            : base(parent)
        {
        }

        public override object WrapValue(GraphEntityInfo entityInfo, GraphTypeInfo typeInfo, object value, bool isSpecified)
        {
            if (typeInfo.IsListType)
            {
                if (!(value is IEnumerable input))
                {
                    return null;
                }
                var elementType = typeInfo.TypeParameter.TypeRepresentation.AsType();
                var listType = typeof(List<>).MakeGenericType(elementType);
                var list = Activator.CreateInstance(listType) as IList;
                foreach (var obj in input)
                {
                    list.Add(_parent.Wrap(entityInfo, typeInfo.TypeParameter, obj, true));
                }
                return typeInfo.IsArrayType
                    ? list.ConvertToArrayRuntime(elementType)
                    : list;
            }
            return value;
        }
    }
}
