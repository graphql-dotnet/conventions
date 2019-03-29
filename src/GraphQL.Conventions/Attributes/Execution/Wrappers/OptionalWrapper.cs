using System;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions.Attributes.Execution.Wrappers
{
    public class OptionalWrapper : WrapperBase
    {
        public override object WrapValue(GraphEntityInfo entityInfo, GraphTypeInfo typeInfo, object value, bool isSpecified)
        {
            if (typeInfo.TypeRepresentation.IsGenericType(typeof(Optional<>)))
            {
                return Optional.Construct(typeInfo, value, isSpecified);
            }
            return value;
        }
    }
}