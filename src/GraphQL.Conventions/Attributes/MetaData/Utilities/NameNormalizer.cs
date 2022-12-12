using System.Text.RegularExpressions;

namespace GraphQL.Conventions.Attributes.MetaData.Utilities
{
    public class NameNormalizer : INameNormalizer
    {
        public string AsTypeName(string name)
        {
            name = NormalizeString(name);
            return name.Length > 0
                ? $"{char.ToUpperInvariant(name[0])}{name.Substring(1)}"
                : string.Empty;
        }

        public string AsFieldName(string name)
        {
            name = NormalizeString(name);
            return name.Length > 0
                ? $"{char.ToLowerInvariant(name[0])}{name.Substring(1)}"
                : string.Empty;
        }

        public string AsArgumentName(string name)
        {
            return AsFieldName(name);
        }

        public string AsEnumMemberName(string name)
        {
            return Regex
                .Replace(NormalizeString(name), @"([A-Z])([A-Z][a-z])|([a-z0-9])([A-Z])", "$1$3_$2$4")
                .ToUpperInvariant();
        }

        private static string NormalizeString(string str)
        {
            str = str?.Trim();
            return string.IsNullOrWhiteSpace(str)
                ? string.Empty
                : NormalizeTypeName(str);
        }

        private static string NormalizeTypeName(string name)
        {
            var tickIndex = name.IndexOf('`');
            return tickIndex >= 0
                ? name.Substring(0, tickIndex)
                : name;
        }
    }
}
