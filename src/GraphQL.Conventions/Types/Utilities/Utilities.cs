using System.Collections.Generic;
using System.Linq;
using GraphQL.Conventions.Relay;

namespace GraphQL.Conventions
{
    public static class Utilities
    {
        public static NonNull<IEnumerable<T>> NonNull<T>(IEnumerable<T> items) => items.ToList();

        public static NonNull<List<T>> NonNull<T>(List<T> items) => items;

        public static NonNull<string> NonNull(string str) => str;

        public static Id Id<T>(string unencodedIdentifier, bool serializeUsingColon = true) =>
            GraphQL.Conventions.Id.New<T>(unencodedIdentifier, serializeUsingColon);

        public static Cursor Cursor<T>(string unencodedIdentifier, bool serializeUsingColon = true) =>
            GraphQL.Conventions.Relay.Cursor.New<T>(unencodedIdentifier, serializeUsingColon);

        public static Id Id(string encodedIdentifier) => (Id)encodedIdentifier;

        public static Cursor Cursor(string encodedCursor) => (Cursor)encodedCursor;

        public static Id? NullableId(string encodedIdentifier) => (Id?)encodedIdentifier;

        public static Cursor? NullableCursor(string encodedCursor) => (Cursor?)encodedCursor;
    }
}
