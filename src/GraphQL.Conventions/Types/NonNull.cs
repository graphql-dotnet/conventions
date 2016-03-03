using System;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions.Types
{
    public struct NonNull<T> : IEquatable<NonNull<T>>, INonNull
        where T : class
    {
        private readonly T _value;

        public NonNull(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _value = value;
        }

        public T Value => _value;

        public object ObjectValue => _value;

        public static object ConvertToNonNull(object value) =>
            new NonNull<T>((T)value);

        public static object ConvertFromNonNull(object value) =>
            ((NonNull<T>)value).Value;

        public static implicit operator NonNull<T>(T value) =>
            new NonNull<T>(value);

        public static implicit operator T(NonNull<T> value) =>
            value._value;

        public static bool operator ==(NonNull<T> v1, NonNull<T> v2) =>
            v1.Equals(v2);

        public static bool operator !=(NonNull<T> v1, NonNull<T> v2) =>
            !v1.Equals(v2);

        public override bool Equals(object other)
        {
            if (other is NonNull<T>)
            {
                var otherValue = (NonNull<T>)other;
                return _value == otherValue._value;
            }
            return false;
        }

        public bool Equals(NonNull<T> other) =>
            _value == other._value;

        public override int GetHashCode() =>
            _value.GetHashCode();

        public override string ToString() =>
            _value?.ToString();
    }

    public static class NonNull
    {
        public static object Construct(GraphTypeInfo typeInfo, object value)
        {
            if (!typeInfo.IsNullable &&
                typeInfo.TypeRepresentation.IsGenericType(typeof(NonNull<>)))
            {
                return Activator.CreateInstance(typeInfo.TypeRepresentation.AsType(), new[] { value });
            }
            else
            {
                return value;
            }
        }
    }
}
