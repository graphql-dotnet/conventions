using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Types.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GraphQL.Conventions.Types.Resolution
{
    public class TypeResolver : ITypeResolver
    {
        private readonly ObjectReflector _reflector;

        private readonly Dictionary<Type, TypeRegistration> _typeMap = new Dictionary<Type, TypeRegistration>();

        public TypeResolver()
        {
            _reflector = new ObjectReflector(this);
            RegisterKnownTypes();
        }

        public GraphSchemaInfo DeriveSchema(TypeInfo typeInfo) => _reflector.GetSchema(typeInfo);

        public GraphTypeInfo DeriveType(TypeInfo typeInfo) => _reflector.GetType(typeInfo);

        public GraphTypeInfo DeriveType<TType>() => DeriveType(typeof(TType).GetTypeInfo());

        public GraphEntityInfo ApplyAttributes(GraphEntityInfo entityInfo) =>
            _reflector.ApplyAttributes(entityInfo);

        public void RegisterType(TypeInfo typeInfo, TypeRegistration typeRegistration)
        {
            _typeMap[typeInfo.AsType()] = typeRegistration;
        }

        public void RegisterType<TType>(TypeRegistration typeRegistration)
        {
            RegisterType(typeof(TType).GetTypeInfo(), typeRegistration);
        }

        public void RegisterScalarType<TType>(string typeName)
        {
            RegisterType<TType>(new TypeRegistration
            {
                Name = typeName,
                IsScalar = true,
            });
        }

        public TypeRegistration LookupType(TypeInfo typeInfo)
        {
            TypeRegistration registration;
            if (_typeMap.TryGetValue(typeInfo.AsType(), out registration))
            {
                return registration;
            }
            return null;
        }

        public void RegisterAttributesInAssembly(Type assemblyType)
        {
            _reflector.DiscoverAndRegisterDefaultAttributesInAssembly(assemblyType);
        }

        public GraphSchemaInfo ActiveSchema { get; set; }

        private void RegisterKnownTypes()
        {
            RegisterScalarType<string>(TypeNames.String);
            RegisterScalarType<bool>(TypeNames.Boolean);
            RegisterScalarType<sbyte>(TypeNames.Integer);
            RegisterScalarType<byte>(TypeNames.Integer);
            RegisterScalarType<short>(TypeNames.Integer);
            RegisterScalarType<ushort>(TypeNames.Integer);
            RegisterScalarType<int>(TypeNames.Integer);
            RegisterScalarType<uint>(TypeNames.Integer);
            RegisterScalarType<long>(TypeNames.Integer);
            RegisterScalarType<ulong>(TypeNames.Integer);
            RegisterScalarType<float>(TypeNames.Float);
            RegisterScalarType<double>(TypeNames.Float);
            RegisterScalarType<decimal>(TypeNames.Float);
            RegisterScalarType<DateTime>(TypeNames.DateTime);
            RegisterScalarType<DateTimeOffset>(TypeNames.DateTimeOffset);
            RegisterScalarType<TimeSpan>(TypeNames.TimeSpan);
            RegisterScalarType<Id>(TypeNames.Id);
            RegisterScalarType<Cursor>(TypeNames.Cursor);
            RegisterScalarType<Url>(TypeNames.Url);
            RegisterScalarType<Uri>(TypeNames.Uri);
            RegisterScalarType<Guid>(TypeNames.Guid);
        }

        public void IgnoreTypesFromNamespacesStartingWith(params string[] namespacesToIgnore)
        {
            if (namespacesToIgnore == null || !namespacesToIgnore.Any())
                return;

            foreach (var @namespace in namespacesToIgnore.Distinct())
                _reflector.IgnoredNamespaces.Add(@namespace);
        }

        public void AddExtensions(Type typeExtensions) =>
            _reflector.AddExtensions(typeExtensions.GetTypeInfo());
    }
}
