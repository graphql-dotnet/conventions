using System;
using System.Collections.Generic;
using System.Reflection;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Relay;

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

        public IDependencyInjector DependencyInjector { get; set; }

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
            RegisterScalarType<DateTime>(TypeNames.Date);
            RegisterScalarType<TimeSpan>(TypeNames.TimeSpan);
            RegisterScalarType<Id>(TypeNames.Id);
            RegisterScalarType<Cursor>(TypeNames.Cursor);
            RegisterScalarType<Url>(TypeNames.Url);
            RegisterScalarType<Uri>(TypeNames.Uri);
        }
    }
}
