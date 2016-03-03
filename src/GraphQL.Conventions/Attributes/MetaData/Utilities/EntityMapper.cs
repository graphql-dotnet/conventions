using System;
using System.Reflection;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes.MetaData.Utilities
{
    public class EntityMapper
    {
        private readonly IMappableTarget _mappableTarget;

        public EntityMapper(IMappableTarget mappableTarget)
        {
            _mappableTarget = mappableTarget;
        }

        public void MapEntity(GraphEntityInfo entity)
        {
            var argumentEntity = entity as GraphArgumentInfo;            
            var parameterInfo = entity.AttributeProvider as ParameterInfo;
            if (argumentEntity != null && parameterInfo != null)
            {
                _mappableTarget.MapArgument(argumentEntity, parameterInfo);
                return;
            }

            var fieldEntity = entity as GraphFieldInfo;
            if (fieldEntity != null)
            {
                var memberInfo = entity.AttributeProvider as MemberInfo;
                if (memberInfo is PropertyInfo || memberInfo is MethodInfo)
                {
                    _mappableTarget.MapField(fieldEntity, memberInfo);
                    return;
                }

                var fieldInfo = entity.AttributeProvider as FieldInfo;
                if (fieldInfo != null)
                {
                    if (fieldInfo.IsLiteral)
                    {
                        _mappableTarget.MapEnumMember(fieldEntity, fieldInfo);
                    }
                    else
                    {
                        _mappableTarget.MapField(fieldEntity, fieldInfo);
                    }
                    return;
                }
            }

            var typeEntity = entity as GraphTypeInfo;            
            var typeInfo = entity.AttributeProvider as TypeInfo;
            if (typeEntity != null && typeInfo != null)
            {
                _mappableTarget.MapType(typeEntity, typeInfo);
                return;
            }

            throw new ArgumentException("Unable to map provided object", nameof(entity.AttributeProvider));
        }
    }
}
