using System;
using System.Reflection;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Types.Resolution
{
    public interface ITypeResolver
    {
        GraphSchemaInfo DeriveSchema(TypeInfo typeInfo);

        GraphTypeInfo DeriveType(TypeInfo typeInfo);

        GraphEntityInfo ApplyAttributes(GraphEntityInfo entityInfo);

        void RegisterType(TypeInfo typeInfo, TypeRegistration typeRegistration);

        void RegisterType<TType>(TypeRegistration typeRegistration);

        void RegisterScalarType<TType>(string typeName);

        TypeRegistration LookupType(TypeInfo typeInfo);

        void IgnoreTypesFromNamespacesStartingWith(params string[] namespacesToIgnore);

        void AddExtensions(Type typeExtensions);

        GraphSchemaInfo ActiveSchema { get; set; }
    }
}
