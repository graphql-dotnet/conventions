using System;

namespace GraphQL.Conventions.Types.Relay.Extensions
{
    public static class ConnectionExtensions
    {
        public static void ValidateParameters<T>(this Connection<T> connection, int? first, int? last, Cursor? after, Cursor? before)
        {
            if (first.HasValue && last.HasValue)
            {
                throw new ArgumentException("Cannot use `first` in conjunction with `last`.");
            }

            if (after.HasValue && before.HasValue)
            {
                throw new ArgumentException("Cannot use `after` in conjunction with `before`.");
            }

            if (first.HasValue && before.HasValue)
            {
                throw new ArgumentException("Cannot use `first` in conjunction with `before`.");
            }

            if (last.HasValue && after.HasValue)
            {
                throw new ArgumentException("Cannot use `last` in conjunction with `after`.");
            }
        }
    }
}
