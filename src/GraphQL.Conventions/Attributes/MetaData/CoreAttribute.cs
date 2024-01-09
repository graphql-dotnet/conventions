using System;
using System.Reflection;
using GraphQL.Conventions.Attributes.Collectors;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(Everywhere, AllowMultiple = true, Inherited = true)]
    public class CoreAttribute : MetaDataAttributeBase, IDefaultAttribute
    {
        public CoreAttribute()
            : base(AttributeApplicationPhase.Initialization)
        {
        }

        public override void MapType(GraphTypeInfo type, TypeInfo typeInfo)
        {
            var typeRepresentation = typeInfo.GetTypeRepresentation();
            if (typeRepresentation.IsSubclassOf(typeof(Union)))
            {
                DeclareUnionType(type, typeRepresentation);
            }
        }

        private void DeclareUnionType(GraphTypeInfo entity, TypeInfo typeInfo)
        {
            var unionType = typeInfo.BaseType?.GetTypeInfo();
            if (unionType != null &&
                unionType.Name.StartsWith(nameof(Union), StringComparison.Ordinal) &&
                unionType.IsSubclassOf(typeof(Union)) &&
                unionType.IsGenericType)
            {
                foreach (var type in unionType.GenericTypeArguments)
                {
                    entity.AddUnionType(entity.TypeResolver.DeriveType(type.GetTypeInfo()));
                }
            }
        }
    }
}
