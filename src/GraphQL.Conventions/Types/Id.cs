using System;
using System.ComponentModel;
using GraphQL.Conventions.Attributes.MetaData.Utilities;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    [TypeConverter(typeof(IdConverter))]
    public struct Id : IComparable, IComparable<Id>, IEquatable<Id>
    {
        public static bool SerializeUsingColon { get; set; } = true;

        private static readonly INameNormalizer Normalizer = new NameNormalizer();

        internal readonly string _unencodedIdentifier;

        private readonly string _encodedIdentifier;

        private Id(Type type, string identifier, bool? serializeUsingColon = null)
            : this(GetTypeName(type), identifier, serializeUsingColon)
        {
        }

        private Id(string typeName, string identifier, bool? serializeUsingColon = null)
        {
            _unencodedIdentifier = serializeUsingColon ?? SerializeUsingColon
                ? $"{typeName}:{identifier}"
                : $"{typeName}{identifier}";
            _encodedIdentifier = Types.Utilities.Identifier.Encode(_unencodedIdentifier);
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException($"Invalid blank identifier '{_unencodedIdentifier}'.");
            }
            if (string.IsNullOrWhiteSpace(_unencodedIdentifier))
            {
                throw new ArgumentException($"Unable to encode identifier '{_unencodedIdentifier}'.");
            }
        }

        public Id(string encodedIdentifier)
        {
            _encodedIdentifier = encodedIdentifier;
            _unencodedIdentifier = Types.Utilities.Identifier.Decode(encodedIdentifier);
            if (string.IsNullOrWhiteSpace(_unencodedIdentifier))
            {
                throw new ArgumentException($"Unable to decode identifier '{encodedIdentifier}'.");
            }
        }

        public override bool Equals(object obj) =>
            obj is Id id ? Equals(id) : false;

        public bool Equals(Id other) =>
            _encodedIdentifier.Equals(other._encodedIdentifier);

        public int CompareTo(object other) =>
            other is Id id ? CompareTo(id) : -1;

        public int CompareTo(Id other) =>
            Types.Utilities.Identifier.Compare(_unencodedIdentifier, other._unencodedIdentifier);

        public override int GetHashCode() =>
            _encodedIdentifier.GetHashCode();

        public override string ToString() =>
            _encodedIdentifier;

        public bool IsIdentifierForType(Type type) =>
            _unencodedIdentifier.Contains(":")
                ? _unencodedIdentifier.StartsWith(GetTypeName(type) + ":", StringComparison.Ordinal)
                : _unencodedIdentifier.StartsWith(GetTypeName(type), StringComparison.Ordinal);

        public bool IsIdentifierForType<TType>() =>
            IsIdentifierForType(typeof(TType));

        public string IdentifierForType(Type type)
        {
            var typeName = GetTypeName(type);
            if (!IsIdentifierForType(type))
            {
                throw new ArgumentException(
                    $"Expected identifier of type '{typeName}' (unencoded identifier '{_unencodedIdentifier}').");
            }
            var underlyingIdentifier = _unencodedIdentifier.Remove(0, typeName.Length).TrimStart(':');
            if (string.IsNullOrWhiteSpace(underlyingIdentifier))
            {
                throw new ArgumentException($"Invalid blank identifier '{_unencodedIdentifier}'.");
            }
            return underlyingIdentifier;
        }

        public string IdentifierForType<TType>() =>
            IdentifierForType(typeof(TType));

        public static Id New(Type type, string identifier, bool? serializeUsingColon = null) =>
            new Id(type, identifier, serializeUsingColon);

        public static Id New<TType>(string identifier, bool? serializeUsingColon = null) =>
            New(typeof(TType), identifier, serializeUsingColon);

        public static Id New(Type type, int identifier, bool? serializeUsingColon = null) =>
            New(type, identifier.ToString(), serializeUsingColon);

        public static Id New<TType>(int identifier, bool? serializeUsingColon = null) =>
            New<TType>(identifier.ToString(), serializeUsingColon);

        public static Id New(Type type, long identifier, bool? serializeUsingColon = null) =>
            New(type, identifier.ToString(), serializeUsingColon);

        public static Id New<TType>(long identifier, bool? serializeUsingColon = null) =>
            New<TType>(identifier.ToString(), serializeUsingColon);

        public static implicit operator Id(string encodedIdentifier) =>
            new Id(encodedIdentifier);

        public static implicit operator Id?(string encodedIdentifier) =>
            string.IsNullOrWhiteSpace(encodedIdentifier) ? (Id?)null : new Id(encodedIdentifier);

        public static bool operator ==(Id id1, Id id2) =>
            id1.Equals(id2);

        public static bool operator !=(Id id1, Id id2) =>
            !id1.Equals(id2);

        public static bool operator <(Id id1, Id id2) =>
            id1.CompareTo(id2) < 0;

        public static bool operator >(Id id1, Id id2) =>
            id1.CompareTo(id2) > 0;

        public static bool operator <=(Id id1, Id id2) =>
            id1.CompareTo(id2) <= 0;

        public static bool operator >=(Id id1, Id id2) =>
            id1.CompareTo(id2) >= 0;

        internal static string GetTypeName(Type type) =>
            Normalizer.AsTypeName(type.Name);
    }

    public class IdConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            switch (value)
            {
                case string s:
                    return new Id(s);

                case null:
                    return null;

                default:
                    throw new NotSupportedException($"Invalid conversion from {value.GetType().FullName} to {typeof(Id).FullName}.");
            }
        }
    }
}
