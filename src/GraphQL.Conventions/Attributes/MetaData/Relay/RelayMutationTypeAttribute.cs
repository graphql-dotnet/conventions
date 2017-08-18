using System;
using System.Reflection;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Relay
{
    [AttributeUsage(Types, AllowMultiple = true, Inherited = true)]
    public class RelayMutationTypeAttribute : MetaDataAttributeBase
    {
        public RelayMutationTypeAttribute()
            : base(AttributeApplicationPhase.Override)
        {
        }

        public override void MapType(GraphTypeInfo entity, TypeInfo typeInfo)
        {
            foreach (var field in entity.Fields)
            {
                field.ExecutionFilters.Add(new RelayMutationAttribute());
            }
        }
    }
}
