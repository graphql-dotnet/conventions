using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Conventions.Types.Resolution;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions.Types.Descriptors
{
    public class GraphTypeInfo : GraphEntityInfo
    {
        private readonly List<GraphTypeInfo> _interfaces = new List<GraphTypeInfo>();

        private readonly List<GraphTypeInfo> _possibleTypes = new List<GraphTypeInfo>();

        private readonly List<GraphTypeInfo> _unionTypes = new List<GraphTypeInfo>();

        public GraphTypeInfo(ITypeResolver typeResolver, TypeInfo type)
            : base(typeResolver, type)
        {
            TypeRepresentation = type ?? (TypeInfo)AttributeProvider;
            DeriveMetaData();
        }

        public bool IsRegisteredType { get; set; }

        public bool IsPrimitive { get; set; }

        public bool IsNullable { get; set; }

        public bool IsTask { get; set; }

        public bool IsOutputType { get; set; }

        public bool IsInputType { get; set; }

        public bool IsScalarType => IsOutputType && IsInputType;

        public bool IsInterfaceType { get; private set; }

        public bool IsUnionType { get; set; }

        public bool IsEnumerationType { get; set; }

        public bool IsListType { get; set; }

        public bool IsArrayType { get; set; }

        public List<GraphTypeInfo> Interfaces => _interfaces;

        public List<GraphTypeInfo> PossibleTypes => _possibleTypes;

        public List<GraphTypeInfo> UnionTypes => IsUnionType ? PossibleTypes : new List<GraphTypeInfo>();

        public List<GraphFieldInfo> Fields { get; internal set; } = new List<GraphFieldInfo>();

        public TypeInfo TypeRepresentation { get; set; }

        public GraphTypeInfo TypeParameter { get; set; }

        public object DefaultValue =>
            TypeRepresentation.IsValueType && !IsNullable && !TypeRepresentation.IsGenericType(typeof(NonNull<>))
                ? Activator.CreateInstance(TypeRepresentation.AsType())
                : null;

        public void AddInterface(GraphTypeInfo typeInfo)
        {
            _interfaces.Add(typeInfo);
            typeInfo.AddPossibleType(this);

            if (typeInfo.IsInputType && !typeInfo.IsOutputType)
            {
                IsInputType = true;
                IsOutputType = false;
            }
        }

        public void AddPossibleType(GraphTypeInfo typeInfo)
        {
            _possibleTypes.Add(typeInfo);
        }

        public void AddUnionType(GraphTypeInfo typeInfo)
        {
            IsUnionType = true;
            AddPossibleType(typeInfo);
        }

        public override string ToString() => $"{nameof(GraphTypeInfo)}:{Name}";

        private void DeriveMetaData()
        {
            var type = TypeRepresentation;

            if (type.IsGenericType(typeof(Task<>)))
            {
                IsTask = true;
                type = type.TypeParameter();
            }

            if (type.IsGenericType(typeof(Nullable<>)))
            {
                IsNullable = true;
                type = type.TypeParameter();
                IsPrimitive = type.IsPrimitiveGraphType();
            }
            else if (type.IsGenericType(typeof(NonNull<>)))
            {
                IsNullable = false;
                type = type.TypeParameter();
                IsPrimitive = type.IsPrimitiveGraphType();
            }
            else
            {
                IsNullable = !type.IsValueType;
                IsPrimitive = type.IsPrimitiveGraphType();
            }

            if (type.IsEnumerableGraphType())
            {
                IsListType = true;
                IsArrayType = type.IsArray;
                IsPrimitive = true;
                TypeParameter = TypeResolver.DeriveType(type.TypeParameter());
            }
            else
            {
                IsListType = false;
            }

            var typeRegistration = TypeResolver.LookupType(type);
            IsRegisteredType = typeRegistration != null;
            IsOutputType = true;
            IsInputType = IsPrimitive || type.IsValueType || (typeRegistration?.IsScalar ?? false);
            IsInterfaceType = !IsListType && type.IsInterface;
            IsEnumerationType = type.IsEnum;
            Name = typeRegistration?.Name;
        }
    }
}
