using System.Reflection;
using GraphQL.Conventions.Attributes.MetaData.Utilities;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes
{
    public abstract class MetaDataAttributeBase : AttributeBase, IMetaDataAttribute, IMappableTarget
    {
        private readonly EntityMapper _entityMapper;

        public MetaDataAttributeBase()
            : base(AttributeApplicationPhase.MetaDataDerivation)
        {
            _entityMapper = new EntityMapper(this);
        }

        protected MetaDataAttributeBase(AttributeApplicationPhase phase)
            : base(phase)
        {
            _entityMapper = new EntityMapper(this);
        }

        public virtual bool ShouldBeApplied(GraphEntityInfo entity)
        {
            return true;
        }

        public virtual void DeriveMetaData(GraphEntityInfo entity)
        {
            MapEntity(entity);
        }

        public virtual void MapArgument(GraphArgumentInfo entity, ParameterInfo parameterInfo)
        {
        }

        public virtual void MapField(GraphFieldInfo entity, MemberInfo memberInfo)
        {
        }

        public virtual void MapEnumMember(GraphFieldInfo entity, FieldInfo fieldInfo)
        {
        }

        public virtual void MapType(GraphTypeInfo entity, TypeInfo typeInfo)
        {
        }

        protected void MapEntity(GraphEntityInfo entity)
        {
            _entityMapper.MapEntity(entity);
        }
    }
}
