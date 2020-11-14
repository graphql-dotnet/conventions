using System;
using System.Reflection;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    public struct Optional<T> : IOptional
    {
        private readonly T _value;
        private readonly bool _isSpecified;

        public Optional(T value, bool isSpecified)
        {
            _value = value;
            _isSpecified = isSpecified;
        }

        public T Value => _value;

        public bool IsSpecified => _isSpecified;

        public object ObjectValue => _value;

        public override int GetHashCode() =>
            _value.GetHashCode();

        public override string ToString() =>
            _value?.ToString();

        static Optional()
        {
            ValidateType();
        }

        public static void ValidateType()
        {
            var typeInfo = typeof(T).GetTypeInfo();
            if ((typeInfo.IsValueType && !typeInfo.IsGenericType(typeof(Nullable<>))) || typeInfo.IsGenericType(typeof(NonNull<>)))
            {
                throw new InvalidOperationException(
                    string.Format("Cannot instantiate with non-nullable type: {0}",
                        typeof(T)));
            }
        }
    }

    public static class Optional
    {
        public static object Construct(GraphTypeInfo typeInfo, object value, bool isSpecified)
        {
            if (typeInfo.IsNullable &&
                typeInfo.TypeRepresentation.IsGenericType(typeof(Optional<>)))
            {
                var genericType = typeInfo.TypeRepresentation.GenericTypeArguments[0];
                if (genericType.GetTypeInfo().IsGenericType(typeof(Nullable<>)) && value != null)
                {
                    // make value nullable to satisfy Optional constructor
                    value = Activator.CreateInstance(genericType, new[] { value });
                }
                return Activator.CreateInstance(typeInfo.TypeRepresentation.AsType(), new[] { value, isSpecified });
            }
            else
            {
                return value;
            }
        }
    }
}
