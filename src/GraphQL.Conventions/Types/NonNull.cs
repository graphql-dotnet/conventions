using System;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    public struct NonNull<T> : IEquatable<NonNull<T>>, INonNull
        where T : class
    {
        public NonNull(T value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public T Value { get; private set; }

        public object ObjectValue => Value;

        public static object ConvertToNonNull(object value) =>
            new NonNull<T>((T)value);

        public static object ConvertFromNonNull(object value) =>
            ((NonNull<T>)value).Value;

        public static implicit operator NonNull<T>(T value) =>
            new NonNull<T>(value);

        public static implicit operator T(NonNull<T> value) =>
            value.Value;

        public static bool operator ==(NonNull<T> v1, NonNull<T> v2) =>
            v1.Equals(v2);

        public static bool operator !=(NonNull<T> v1, NonNull<T> v2) =>
            !v1.Equals(v2);

        public override bool Equals(object obj)
        {
            if (obj is NonNull<T> otherValue)
            {
                return Value == otherValue.Value;
            }
            return false;
        }

        public bool Equals(NonNull<T> other) =>
            Value == other.Value;

        public override int GetHashCode() =>
            Value.GetHashCode();

        public override string ToString() =>
            Value?.ToString();
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
