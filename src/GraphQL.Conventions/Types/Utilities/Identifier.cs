using System;
using System.Text;
using System.Text.RegularExpressions;

namespace GraphQL.Conventions.Types.Utilities
{
    public static class Identifier
    {
        private static readonly Regex Regex = new Regex(@"^(.*?)([0-9]*)$", RegexOptions.Compiled);

        public static string Encode(string unencodedIdentifier)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(unencodedIdentifier ?? string.Empty));
        }

        public static string Decode(string encodedIdentifier)
        {
            try
            {
                return string.IsNullOrWhiteSpace(encodedIdentifier)
                    ? string.Empty
                    : Encoding.UTF8.GetString(Convert.FromBase64String(encodedIdentifier));
            }
            catch
            {
                return string.Empty;
            }
        }

        public static int Compare(string unencodedIdentifier1, string unencodedIdentifier2)
        {
            if (unencodedIdentifier1 == null || unencodedIdentifier2 == null)
            {
                return string.Compare(
                    unencodedIdentifier1 ?? string.Empty,
                    unencodedIdentifier2 ?? string.Empty,
                    StringComparison.Ordinal);
            }

            if (unencodedIdentifier1.Length != unencodedIdentifier2.Length)
            {
                var colon1 = unencodedIdentifier1.IndexOf(':');
                var colon2 = unencodedIdentifier2.IndexOf(':');
                if (colon1 >= 0 && colon2 >= 0 && colon1 == colon2)
                {
                    if (unencodedIdentifier1.Substring(0, colon1) == unencodedIdentifier2.Substring(0, colon2))
                    {
                        var value1 = unencodedIdentifier1.Substring(colon1 + 1);
                        var value2 = unencodedIdentifier2.Substring(colon2 + 1);
                        long intValue1;
                        long intValue2;
                        if (long.TryParse(value1, out intValue1) && long.TryParse(value2, out intValue2))
                        {
                            return Math.Sign(intValue1 - intValue2);
                        }
                    }
                }
                else if (colon1 == -1 && colon2 == -1)
                {
                    var parts1 = GetComponents(unencodedIdentifier1);
                    var parts2 = GetComponents(unencodedIdentifier2);
                    if (parts1.Item2 != null && parts2.Item2 != null)
                    {
                        int typeComparison = string.Compare(parts1.Item1, parts2.Item1, StringComparison.Ordinal);
                        return typeComparison == 0
                            ? Math.Sign(parts1.Item2.Value - parts2.Item2.Value)
                            : typeComparison;
                    }
                }
            }
            return string.Compare(unencodedIdentifier1, unencodedIdentifier2, StringComparison.Ordinal);
        }

        private static Tuple<string, long?> GetComponents(string unencodedIdentifier)
        {
            var parts = Regex.Match(unencodedIdentifier);
            if (parts.Success && parts.Groups.Count == 3)
            {
                long intValue;
                if (long.TryParse(parts.Groups[2].Value, out intValue))
                {
                    return Tuple.Create(parts.Groups[1].Value, (long?)intValue);
                }
            }
            return Tuple.Create(unencodedIdentifier, (long?)null);
        }
    }
}
