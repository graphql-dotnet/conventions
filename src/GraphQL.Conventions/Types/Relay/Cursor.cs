using System;
using System.Linq;

namespace GraphQL.Conventions.Relay
{
    [Description("Cursor used in pagination.")]
    public struct Cursor
        : IComparable, IComparable<Cursor>, IEquatable<Cursor>
    {
        private readonly Id _id;

        private Cursor(Type type, string value, bool? serializeUsingColon = null)
        {
            _id = Id.New(type, value, serializeUsingColon);
        }

        public Cursor(string encodedCursor)
        {
            _id = new Id(encodedCursor);
        }

        public override bool Equals(object other) =>
            other is Cursor ? Equals((Cursor)other) : false;

        public bool Equals(Cursor other) =>
            _id.Equals(other._id);

        public int CompareTo(object other) =>
            other is Cursor ? CompareTo((Cursor)other) : -1;

        public int CompareTo(Cursor other) =>
            _id.CompareTo(other._id);

        public override int GetHashCode() =>
            _id.GetHashCode();

        public override string ToString() =>
            _id.ToString();

        public bool IsCursorForType(Type type) =>
            _id.IsIdentifierForType(type);

        public bool IsCursorForType<TType>() =>
            IsCursorForType(typeof(TType));

        public string CursorForType(Type type)
        {
            var typeName = Id.GetTypeName(type);
            if (!_id.IsIdentifierForType(type))
            {
                throw new ArgumentException(
                    $"Expected cursor of type '{typeName}' (unencoded identifier '{_id._unencodedIdentifier}').");
            }
            return _id.IdentifierForType(type);
        }

        public string CursorForType<TType>() =>
            CursorForType(typeof(TType));

        public int? IntegerForCursor<TType>()
        {
            int intVal;
            return int.TryParse(CursorForType(typeof(TType)), out intVal) ? intVal : (int?)null;
        }

        public long? LongForCursor<TType>()
        {
            long intVal;
            return long.TryParse(CursorForType(typeof(TType)), out intVal) ? intVal : (long?)null;
        }

        public static Cursor New(Type type, string index, bool? serializeUsingColon = null) =>
            new Cursor(type, index, serializeUsingColon);

        public static Cursor New<TType>(string index, bool? serializeUsingColon = null) =>
            New(typeof(TType), index, serializeUsingColon);

        public static Cursor New(Type type, int index, bool? serializeUsingColon = null) =>
            New(type, index.ToString(), serializeUsingColon);

        public static Cursor New<TType>(int index, bool? serializeUsingColon = null) =>
            New<TType>(index.ToString(), serializeUsingColon);

        public static Cursor New(Type type, long index, bool? serializeUsingColon = null) =>
            New(type, index.ToString(), serializeUsingColon);

        public static Cursor New<TType>(long index, bool? serializeUsingColon = null) =>
            New<TType>(index.ToString(), serializeUsingColon);

        public static Cursor New(Type type, DateTime dateTime, bool? serializeUsingColon = null) =>
            New(type, dateTime.Ticks, serializeUsingColon);

        public static Cursor New<TType>(DateTime dateTime, bool? serializeUsingColon = null) =>
            New<TType>(dateTime.Ticks, serializeUsingColon);

        public static Cursor? Coalesce(params Cursor?[] cursors) =>
            cursors.FirstOrDefault(cursor => cursor.HasValue);

        public static implicit operator Cursor(string encodedCursor) =>
            new Cursor(encodedCursor);

        public static implicit operator Cursor?(string encodedCursor) =>
            string.IsNullOrWhiteSpace(encodedCursor) ? (Cursor?)null : new Cursor(encodedCursor);

        public static bool operator ==(Cursor c1, Cursor c2) =>
            c1.Equals(c2);

        public static bool operator !=(Cursor c1, Cursor c2) =>
            !c1.Equals(c2);

        public static bool operator <(Cursor c1, Cursor c2) =>
            c1.CompareTo(c2) < 0;

        public static bool operator >(Cursor c1, Cursor c2) =>
            c1.CompareTo(c2) > 0;

        public static bool operator <=(Cursor c1, Cursor c2) =>
            c1.CompareTo(c2) <= 0;

        public static bool operator >=(Cursor c1, Cursor c2) =>
            c1.CompareTo(c2) >= 0;
    }
}
