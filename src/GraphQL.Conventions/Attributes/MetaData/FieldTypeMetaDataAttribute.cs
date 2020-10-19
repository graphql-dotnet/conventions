using System;

namespace GraphQL.Conventions
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public abstract class FieldTypeMetaDataAttribute : GraphQLAttribute
    {
        public abstract string Key();

        public abstract object Value();
    }
}