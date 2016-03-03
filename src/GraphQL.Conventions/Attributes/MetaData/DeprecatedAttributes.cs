using System;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes.MetaData
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
