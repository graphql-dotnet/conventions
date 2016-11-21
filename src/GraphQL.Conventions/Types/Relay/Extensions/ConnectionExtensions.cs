using System;
using System.Collections.Generic;
using System.Linq;

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

        public static Connection<T> ToConnection<T>(this IEnumerable<T> enumerable, int first, Cursor? after)
        {
            var results = enumerable.ToList();
            var firstValue = first;
            var afterIndex = int.Parse(after?.CursorForType<T>() ?? "0");
            var startIndex = afterIndex + 1;
            var endIndex = Math.Min(afterIndex + firstValue, results.Count);
            if (endIndex < startIndex)
            {
                endIndex = startIndex;
            }

            return new Connection<T>
            {
                TotalCount = results.Count,
                PageInfo = new PageInfo
                {
                    HasNextPage = results.Count - afterIndex > firstValue,
                    HasPreviousPage = afterIndex > 0,
                    StartCursor = Cursor.New<T>(1 + afterIndex),
                    EndCursor = Cursor.New<T>(endIndex),
                },
                Edges = results.Skip(afterIndex).Take(firstValue).Select((result, index) => new Edge<T>
                {
                    Cursor = Cursor.New<T>(afterIndex + index + 1),
                    Node = result,
                }),
            };
        }
    }
}
