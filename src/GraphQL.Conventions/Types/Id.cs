using System;
using GraphQL.Conventions.Attributes.MetaData.Utilities;

namespace GraphQL.Conventions.Types
{
    public struct Id
        : IComparable, IComparable<Id>, IEquatable<Id>
    {
        public static bool SerializeUsingColon { get; set; } = true;

        private readonly static INameNormalizer _normalizer = new NameNormalizer();

        private readonly string _unencodedIdentifier;

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
            _encodedIdentifier = Utilities.Identifier.Encode(_unencodedIdentifier);
        }

        public Id(string encodedIdentifier)
        {
            _encodedIdentifier = encodedIdentifier;
            _unencodedIdentifier = Utilities.Identifier.Decode(encodedIdentifier);
            if (string.IsNullOrWhiteSpace(_unencodedIdentifier))
            {
                throw new ArgumentException($"Unable to decode identifier '{encodedIdentifier}'.");
            }
        }

        public override bool Equals(object other) =>
            other is Id ? Equals((Id)other) : false;

        public bool Equals(Id other) =>
            _encodedIdentifier.Equals(other._encodedIdentifier);

        public int CompareTo(object other) =>
            other is Id ? CompareTo((Id)other) : -1;

        public int CompareTo(Id other) =>
            Utilities.Identifier.Compare(_unencodedIdentifier, other._unencodedIdentifier);

        public override int GetHashCode() =>
            _encodedIdentifier.GetHashCode();

        public override string ToString() =>
            _encodedIdentifier;

        public bool IsIdentifierForType(Type type) =>
            _unencodedIdentifier.StartsWith(GetTypeName(type));

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
            return _unencodedIdentifier.Remove(0, typeName.Length).TrimStart(':');
        }

        public string IdentifierForType<TType>() =>
            IdentifierForType(typeof(TType));

        public string UnencodedIdentifier => _unencodedIdentifier;

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
            _normalizer.AsTypeName(type.Name);
    }
}
