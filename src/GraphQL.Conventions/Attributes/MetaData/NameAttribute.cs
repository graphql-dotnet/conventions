using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Attributes.Collectors;
using GraphQL.Conventions.Attributes.MetaData.Utilities;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions
{
    [AttributeUsage(Everywhere, AllowMultiple = true, Inherited = true)]
    public class NameAttribute : MetaDataAttributeBase, IDefaultAttribute
    {
        private static INameNormalizer _nameNormalizer = new NameNormalizer();

        private readonly string _name;

        public NameAttribute()
        {
            _name = null;
        }

        public NameAttribute(string name)
        {
            _name = name;
        }

        public override void DeriveMetaData(GraphEntityInfo entity)
        {
            if (!string.IsNullOrWhiteSpace(_name))
            {
                entity.Name = _name;
            }
            else
            {
                MapEntity(entity);
            }
        }

        public override void MapArgument(GraphArgumentInfo entity, ParameterInfo parameterInfo)
        {
            entity.Name = _nameNormalizer.AsArgumentName(parameterInfo.Name);
        }

        public override void MapField(GraphFieldInfo entity, MemberInfo memberInfo)
        {
            entity.Name = _nameNormalizer.AsFieldName(memberInfo.Name);
        }

        public override void MapEnumMember(GraphFieldInfo entity, FieldInfo fieldInfo)
        {
            entity.Name = _nameNormalizer.AsEnumMemberName(fieldInfo.Name);
        }

        public override void MapType(GraphTypeInfo entity, TypeInfo typeInfo)
        {
            if (entity.IsRegisteredType)
            {
                return;
            }

            if (typeInfo.IsGenericType(typeof(Task<>)))
            {
                typeInfo = typeInfo.TypeParameter();
            }

            var typeName = _nameNormalizer.AsTypeName(typeInfo.Name);
            if (typeInfo.IsGenericType)
            {
                var genericTypeNames = typeInfo
                    .GenericTypeArguments
                    .Select(type => DeriveTypeName(entity, type.GetTypeInfo()));

                if (typeInfo.IsGenericType(typeof(Nullable<>)) ||
                    typeInfo.IsGenericType(typeof(NonNull<>)))
                {
                    entity.Name = genericTypeNames.First();
                }
                else
                {
                    entity.Name = string.Join(string.Empty, genericTypeNames) + typeName;
                }
            }
            else
            {
                entity.Name = typeName;
            }
        }

        private static string DeriveTypeName(GraphEntityInfo entityInfo, TypeInfo typeInfo)
        {
            var entity = entityInfo.TypeResolver.DeriveType(typeInfo);
            entityInfo.TypeResolver.ApplyAttributes(entity);
            return entity.Name;
        }
    }
}
