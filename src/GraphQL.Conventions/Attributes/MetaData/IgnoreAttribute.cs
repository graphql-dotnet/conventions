using System;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(Fields, AllowMultiple = true, Inherited = true)]
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
