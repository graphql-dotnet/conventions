using System.Linq;
using System.Reflection;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Attributes.Collectors;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Handlers
{
    class MetaDataAttributeHandler : MetaDataAttributeHandler<IMetaDataAttribute> { }

    class MetaDataAttributeHandler<TAttribute>
        where TAttribute : IMetaDataAttribute
    {
        private readonly AttributeCollector<TAttribute> _collector = new AttributeCollector<TAttribute>();
        private readonly AttributeCollector<IExecutionFilterAttribute> _executionFilterCollector =
            new AttributeCollector<IExecutionFilterAttribute>();

        public bool HasAttribute<T>(ICustomAttributeProvider obj)
        {
            var attributes = _collector.CollectAttributes(obj);
            return attributes.Any(attr => attr.GetType() == typeof(T));
        }

        public void DeriveMetaData(GraphEntityInfo entity, ICustomAttributeProvider obj)
        {
            var attributes = _collector.CollectAttributes(obj);
            foreach (var attribute in attributes.Where(attribute => attribute.ShouldBeApplied(entity)))
            {
                attribute.DeriveMetaData(entity);
                if (entity.IsIgnored)
                {
                    break;
                }
            }

            var fieldInfo = entity as GraphFieldInfo;
            if (fieldInfo != null)
            {
                fieldInfo.ExecutionFilters.AddRange(_executionFilterCollector.CollectAttributes(obj));
            }
        }
    }
}
