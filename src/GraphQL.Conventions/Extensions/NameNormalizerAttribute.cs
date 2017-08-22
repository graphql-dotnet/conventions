using System.Reflection;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Attributes.Collectors;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Extensions
{
    public class NameNormalizerAttribute : MetaDataAttributeBase, IDefaultAttribute
    {
        public NameNormalizerAttribute()
            : base(AttributeApplicationPhase.Override)
        {
        }

        public override void MapType(GraphTypeInfo entity, TypeInfo typeInfo)
        {
            if (entity.Name?.EndsWith("Dto") ?? false)
            {
                entity.Name = entity.Name.Remove(entity.Name.Length - 3);
            }
        }
    }
}