using System;
using System.Reflection;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions
{
    [AttributeUsage(Fields, AllowMultiple = true, Inherited = true)]
    public class DefaultValueAttribute : MetaDataAttributeBase
    {
        private readonly object _defaultValue;

        public DefaultValueAttribute(object defaultValue)
            : base(AttributeApplicationPhase.Initialization)
        {
            _defaultValue = defaultValue;
        }

        public override void MapField(GraphFieldInfo fieldInfo, MemberInfo memberInfo)
        {
            fieldInfo.DefaultValue = _defaultValue;
        }
    }
}
