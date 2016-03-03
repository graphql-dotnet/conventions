using System;
using System.Reflection;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Attributes.MetaData
{
    [AttributeUsage(Types, AllowMultiple = true, Inherited = true)]
    public class InputTypeAttribute : MetaDataAttributeBase
    {
        public InputTypeAttribute()
            : base(AttributeApplicationPhase.Initialization)
        {
        }

        public override void MapType(GraphTypeInfo entity, TypeInfo typeInfo)
        {
            entity.IsInputType = true;
            entity.IsOutputType = false;

            foreach (var field in entity.Fields)
            {
                if (field.AttributeProvider is MethodInfo)
                {
                    field.IsIgnored = true;
                }
            }
        }
    }
}
