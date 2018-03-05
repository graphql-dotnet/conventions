using System;
using System.Reflection;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions
{
    [AttributeUsage(FieldsAndParameters, AllowMultiple = true, Inherited = true)]
    public class NonNullAttribute : MetaDataAttributeBase
    {
        public NonNullAttribute() : base(AttributeApplicationPhase.MetaDataDerivation) { }

        public override void MapType(GraphTypeInfo entity, TypeInfo typeInfo)
        {
            entity.IsNullable = false;
        }
    }
}
