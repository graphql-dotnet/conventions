using System.Collections.Generic;
using System.Reflection;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Types.Resolution;

namespace GraphQL.Conventions.Types.Descriptors
{
    public class GraphArgumentInfo : GraphEntityInfo
    {
        public GraphArgumentInfo(ITypeResolver typeResolver, ParameterInfo parameter = null)
            : base(typeResolver, parameter)
        {
        }

        public object DefaultValue { get; set; }

        public List<IExecutionFilterAttribute> ExecutionFilters { get; } =
            new List<IExecutionFilterAttribute>();

        public bool IsInjected { get; set; }
    }
}
