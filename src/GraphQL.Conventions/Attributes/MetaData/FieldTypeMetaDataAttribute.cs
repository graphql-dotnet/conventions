using System;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public abstract class FieldTypeMetaDataAttribute : GraphQLAttribute
    {
        public abstract string Key();

        public abstract object Value();
    }
}
