using System;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(Everywhere, AllowMultiple = true, Inherited = true)]
    public class DescriptionAttribute : MetaDataAttributeBase
    {
        private readonly string _description;

        public DescriptionAttribute(string description)
        {
            _description = description;
        }

        public override void DeriveMetaData(GraphEntityInfo entity)
        {
            entity.Description = _description;
        }
    }
}
