using System;
using System.Reflection;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions
{
    [AttributeUsage(Parameters, AllowMultiple = true, Inherited = true)]
    public class InjectAttribute : MetaDataAttributeBase
    {
        public InjectAttribute()
            : base(AttributeApplicationPhase.Initialization)
        {
        }

        public override void MapArgument(GraphArgumentInfo argumentInfo, ParameterInfo parameterInfo)
        {
            argumentInfo.IsInjected = true;
            argumentInfo.Type.IsIgnored = true;
        }
    }
}
