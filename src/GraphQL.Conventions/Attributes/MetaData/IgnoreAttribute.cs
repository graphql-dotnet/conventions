using System;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Types.Descriptors;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    [AttributeUsage(Fields, AllowMultiple = true)]
    public class IgnoreAttribute : MetaDataAttributeBase
    {
        public IgnoreAttribute()
            : base(AttributeApplicationPhase.Initialization)
        {
        }

        public override void DeriveMetaData(GraphEntityInfo entity)
        {
            entity.IsIgnored = true;
        }
    }
}
