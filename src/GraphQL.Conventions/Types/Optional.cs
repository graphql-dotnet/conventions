using System;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions
{
    public struct Optional<T> : /*IEquatable<Optional<T>>,*/ IOptional
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

        /*
        public static implicit operator T(Optional<T> value) =>
            value._value;

        public static bool operator ==(Optional<T> v1, Optional<T> v2) =>
            v1.Equals(v2);

        public static bool operator !=(Optional<T> v1, Optional<T> v2) =>
            !v1.Equals(v2);

        public override bool Equals(object obj)
        {
            if (obj is Optional<T>)
            {
                var otherValue = (Optional<T>)obj;
                return _value == otherValue._value;
            }
            return false;
        }

        public bool Equals(Optional<T> other) =>
            _value == other._value;
            */
        public override int GetHashCode() =>
            _value.GetHashCode();

        public override string ToString() =>
            _value?.ToString();
    }

    public static class Optional
    {
        public static object Construct(GraphTypeInfo typeInfo, object value, bool isSpecified)
        {
            if (typeInfo.IsNullable &&
                typeInfo.TypeRepresentation.IsGenericType(typeof(Optional<>)))
            {
                return Activator.CreateInstance(typeInfo.TypeRepresentation.AsType(), new[] { value, isSpecified });
            }
            else
            {
                return value;
            }
        }
    }
}
