using System;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Types.Descriptors;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    [AttributeUsage(Everywhere, AllowMultiple = true)]
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
