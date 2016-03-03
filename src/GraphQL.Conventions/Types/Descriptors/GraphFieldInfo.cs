using System.Collections.Generic;
using System.Reflection;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Types.Resolution;

namespace GraphQL.Conventions.Types.Descriptors
{
    public class GraphFieldInfo : GraphEntityInfo
    {
        public GraphFieldInfo(ITypeResolver typeResolver, MemberInfo field = null)
            : base(typeResolver, field)
        {
        }

        public GraphTypeInfo DeclaringType { get; set; }

        public object DefaultValue => Value ?? Type.DefaultValue;

        public List<GraphArgumentInfo> Arguments { get; } =
            new List<GraphArgumentInfo>();

        public List<IExecutionFilterAttribute> ExecutionFilters { get; } =
            new List<IExecutionFilterAttribute>();

        public bool IsMethod => AttributeProvider is MethodInfo;
    }
}
