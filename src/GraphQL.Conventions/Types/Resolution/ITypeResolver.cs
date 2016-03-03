using System.Collections.Generic;
using System.Reflection;
using GraphQL.Conventions.Profiling;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Evaluators;

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

        IFieldResolver FieldResolver { get; }

        List<IProfiler> Profilers { get; }

        IDependencyInjector DependencyInjector { get; set; }

        GraphSchemaInfo ActiveSchema { get; set; }
    }
}
