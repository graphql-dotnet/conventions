using System;
using System.Reflection;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(Parameters, AllowMultiple = true, Inherited = true)]
    public class InjectAttribute : MetaDataAttributeBase
    {
        public InjectAttribute()
            : base(AttributeApplicationPhase.Initialization)
        {
        }

        public override void MapArgument(GraphArgumentInfo argument, ParameterInfo parameterInfo)
        {
            argument.IsInjected = true;
            argument.Type.IsIgnored = true;
        }
    }
}
