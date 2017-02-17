using System.Reflection;
using GraphQL.Conventions.Types.Resolution;

namespace GraphQL.Conventions.Types.Descriptors
{
    public abstract class GraphEntityInfo
    {
        protected GraphEntityInfo(ITypeResolver typeResolver, ICustomAttributeProvider attributeProvider = null)
        {
            TypeResolver = typeResolver;
            AttributeProvider = attributeProvider;
            IsProfilable = AttributeProvider is MethodInfo;
            SchemaInfo = typeResolver.ActiveSchema;
        }

        public ITypeResolver TypeResolver { get; private set; }

        public ICustomAttributeProvider AttributeProvider { get; private set; }

        public GraphSchemaInfo SchemaInfo { get; set; }

        public bool IsIgnored { get; set; }

        public bool IsProfilable { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string DeprecationReason { get; set; }

        public bool IsDeprecated => !string.IsNullOrEmpty(DeprecationReason);

        public GraphTypeInfo Type { get; set; }

        public object Value { get; set; }

        public override string ToString() => $"{GetType().Name}({Name}: {Type.Name})";
    }
}
