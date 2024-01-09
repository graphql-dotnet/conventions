using System;
using System.Reflection;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Types.Descriptors;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    [AttributeUsage(Parameters, AllowMultiple = true)]
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
