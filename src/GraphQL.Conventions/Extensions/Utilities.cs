namespace GraphQL.Conventions.Extensions
{
    public static class Utilities
    {
        public static string IdentifierForTypeOrNull<T>(this string id) =>
            id.IsIdentifierForType<T>() ? id.IdentifierForType<T>() : null;

        public static string IdentifierForTypeOrNull<T>(this NonNull<string> id) =>
            id.IsIdentifierForType<T>() ? id.IdentifierForType<T>() : null;

        public static string IdentifierForType<T>(this string id) =>
            new Id(id).IdentifierForType<T>();

        public static string IdentifierForType<T>(this NonNull<string> id) =>
            id.Value.IdentifierForType<T>();

        public static bool IsIdentifierForType<T>(this string id) =>
            new Id(id).IsIdentifierForType<T>();

        public static bool IsIdentifierForType<T>(this NonNull<string> id) =>
            id.Value.IsIdentifierForType<T>();
    }
}
