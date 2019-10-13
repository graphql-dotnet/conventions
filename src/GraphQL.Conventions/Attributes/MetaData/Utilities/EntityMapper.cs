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
            if (entity is GraphArgumentInfo argumentEntity && entity.AttributeProvider is ParameterInfo parameterInfo)
            {
                _mappableTarget.MapArgument(argumentEntity, parameterInfo);
                return;
            }

            if (entity is GraphFieldInfo fieldEntity)
            {
                var memberInfo = entity.AttributeProvider as MemberInfo;
                if (memberInfo is PropertyInfo || memberInfo is MethodInfo)
                {
                    _mappableTarget.MapField(fieldEntity, memberInfo);
                    return;
                }

                if (entity.AttributeProvider is FieldInfo fieldInfo)
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

            if (entity is GraphTypeInfo typeEntity && entity.AttributeProvider is TypeInfo typeInfo)
            {
                _mappableTarget.MapType(typeEntity, typeInfo);
                return;
            }

            throw new ArgumentException("Unable to map provided object.");
        }
    }
}
