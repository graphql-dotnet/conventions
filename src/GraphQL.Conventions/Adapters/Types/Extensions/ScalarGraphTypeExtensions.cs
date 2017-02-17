namespace GraphQL.Conventions.Adapters.Types.Extensions
{
    public static class ScalarGraphTypeExtensions
    {
        public static string StripQuotes(this string value)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > 2 && value.StartsWith("\"") && value.EndsWith("\""))
            {
                return value.Substring(1, value.Length - 2);
            }
            return value;
        }
    }
}
