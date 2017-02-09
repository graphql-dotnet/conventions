using System;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions
{
    [AttributeUsage(Everywhere, AllowMultiple = true, Inherited = true)]
    public class DeprecatedAttribute : MetaDataAttributeBase
    {
        private readonly string _deprecationReason;

        public DeprecatedAttribute(string reason)
        {
            _deprecationReason = reason;
        }

        public override void DeriveMetaData(GraphEntityInfo entity)
        {
            entity.DeprecationReason = _deprecationReason;
        }
    }
}
