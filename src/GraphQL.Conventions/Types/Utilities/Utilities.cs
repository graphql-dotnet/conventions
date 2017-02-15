using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Conventions
{
    public static class Utilities
    {
        public static NonNull<IEnumerable<T>> NonNull<T>(IEnumerable<T> items) => items.ToList();

        public static NonNull<List<T>> NonNull<T>(List<T> items) => items;

        public static NonNull<string> NonNull(string str) => str;

        public static Id Id(string encodedIdentifier) => new Id(encodedIdentifier);

        public static Id Id<T>(string unencodedIdentifier, bool serializeUsingColon = true) =>
            GraphQL.Conventions.Id.New<T>(unencodedIdentifier, serializeUsingColon);
    }
}
