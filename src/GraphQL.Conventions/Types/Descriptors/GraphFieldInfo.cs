using System.Collections.Generic;
using System.Reflection;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Types.Resolution;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions.Types.Descriptors
{
    public class GraphFieldInfo : GraphEntityInfo
    {
        private object _defaultValue = null;

        public GraphFieldInfo(ITypeResolver typeResolver, MemberInfo field = null)
            : base(typeResolver, field)
        {
        }

        public GraphTypeInfo DeclaringType { get; set; }

        public object DefaultValue
        {
            get { return _defaultValue ?? Value; }
            set { _defaultValue = value; }
        }

        public List<GraphArgumentInfo> Arguments { get; } =
            new List<GraphArgumentInfo>();

        public List<IExecutionFilterAttribute> ExecutionFilters { get; } =
            new List<IExecutionFilterAttribute>();

        public bool IsMethod => AttributeProvider is MethodInfo;

        public bool IsExtensionMethod => (AttributeProvider as MethodInfo).IsExtensionMethod();

        public override string ToString() => $"{nameof(GraphFieldInfo)}:{Name}";
    }
}
