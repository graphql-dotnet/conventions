using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Conventions.Relay
{
    public static class ConnectionExtensions
    {
        public static Connection<T> ToConnection<T>(
            this IEnumerable<T> enumerable,
            int? first = null,
            Cursor? after = null,
            int? last = null,
            Cursor? before = null,
            int? totalCount = null)
        {
            ValidateParameters(first, after, last, before);
            var connection = new ConnectionImpl<T>(enumerable, first, after, last, before);
            return new Connection<T>
            {
                Edges = connection.Edges,
                PageInfo = connection.PageInfo,
                TotalCount = totalCount,
            };
        }

        public static Connection<T> ToConnection<T>(
            this IEnumerable<Edge<T>> edges,
            bool hasNextPage,
            bool hasPreviousPage = false,
            int? totalCount = null,
            Cursor? defaultCursorIfEmpty = null)
        {
            var edgeList = edges.ToList();
            if (!defaultCursorIfEmpty.HasValue)
            {
                defaultCursorIfEmpty = Cursor.New<T>(0);
            }
            return new Connection<T>
            {
                Edges = edgeList,
                TotalCount = totalCount,
                PageInfo = new PageInfo
                {
                    StartCursor = edgeList.Any()
                        ? edgeList.Min(edge => edge.Cursor)
                        : defaultCursorIfEmpty.Value,
                    EndCursor = edgeList.Any()
                        ? edgeList.Max(edge => edge.Cursor)
                        : defaultCursorIfEmpty.Value,
                    HasNextPage = hasNextPage,
                    HasPreviousPage = hasPreviousPage,
                },
            };
        }

        public static Connection<TNew> Transform<T, TNew>(
            this Connection<T> connection,
            Func<T, TNew> transform)
        {
            var edges = connection
                .Edges
                .Select(edge => new Edge<TNew>
                {
                    Cursor = edge.Cursor,
                    Node = transform(edge.Node),
                });

            return new Connection<TNew>
            {
                Edges = edges,
                PageInfo = connection.PageInfo,
                TotalCount = connection.TotalCount,
            };
        }

        private static void ValidateParameters(int? first, Cursor? after, int? last, Cursor? before)
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

        private class ConnectionImpl<TNode> : Connection<TNode>
        {
            private readonly IEnumerable<Tuple<string, TNode>> _collection;

            public ConnectionImpl(IEnumerable<Tuple<string, TNode>> collection, int? first, Cursor? after, int? last, Cursor? before)
            {
                Cursor? minCursor = null;
                Cursor? maxCursor = null;
                _collection = collection ?? new List<Tuple<string, TNode>>();

                var edges = _collection
                    .Select(t => new Edge<TNode>
                    {
                        Cursor = Cursor.New<TNode>(t.Item1),
                        Node = t.Item2,
                    });

                if (after.HasValue)
                {
                    var lastEntryBefore = edges
                        .TakeWhile(edge => edge.Cursor <= after.Value)
                        .LastOrDefault();

                    if (lastEntryBefore != null)
                    {
                        minCursor = lastEntryBefore.Cursor;
                    }

                    edges = edges.SkipWhile(edge => edge.Cursor <= after.Value);
                }

                if (before.HasValue)
                {
                    var beforeEdges = edges
                        .TakeWhile(edge => edge.Cursor <= before.Value)
                        .ToList();

                    maxCursor = beforeEdges.LastOrDefault()?.Cursor;
                    edges = beforeEdges.TakeWhile(edge => edge.Cursor < before.Value);
                }

                if (first.HasValue)
                {
                    var edgesList = edges.Take(first.Value + 1).ToList();
                    maxCursor = edgesList.LastOrDefault()?.Cursor;
                    edgesList = edgesList.Take(first.Value).ToList();
                    edges = edgesList;
                }

                if (last.HasValue)
                {
                    var edgesList = edges.ToList();
                    var count = edgesList.Count;
                    if (last.Value < count)
                    {
                        minCursor = edgesList
                            .Skip(Math.Max(0, count - last.Value - 2))
                            .FirstOrDefault()?
                            .Cursor ?? Cursor.New<TNode>(0);
                        edges = edgesList.Skip(count - last.Value);
                    }
                    else
                    {
                        edges = edgesList;
                        minCursor = edgesList.FirstOrDefault()?.Cursor;
                    }
                }

                var paginatedEdges = edges.ToList();
                Edges = paginatedEdges;

                if (minCursor == null)
                {
                    minCursor = paginatedEdges.FirstOrDefault()?.Cursor ?? Cursor.New<TNode>(0);
                }
                if (maxCursor == null)
                {
                    maxCursor = paginatedEdges.Any() ? paginatedEdges.Max(n => n.Cursor) : Cursor.New<TNode>(0);
                }

                if (paginatedEdges.Any())
                {
                    var minPageCursor = paginatedEdges.Min(edge => edge.Cursor);
                    var maxPageCursor = paginatedEdges.Max(edge => edge.Cursor);

                    PageInfo = new PageInfo
                    {
                        StartCursor = minPageCursor,
                        EndCursor = maxPageCursor,
                        HasNextPage = maxPageCursor < maxCursor,
                        HasPreviousPage = minPageCursor > minCursor,
                    };
                }
                else
                {
                    var zeroCursor = Cursor.New<TNode>(0);
                    PageInfo = new PageInfo
                    {
                        StartCursor = after ?? before ?? minCursor ?? zeroCursor,
                        EndCursor = before ?? after ?? maxCursor ?? zeroCursor,
                    };

                    var startValue = PageInfo.Value.StartCursor.IntegerForCursor<TNode>() ?? 0;
                    var endValue = PageInfo.Value.EndCursor.IntegerForCursor<TNode>() ?? 0;

                    if (before.HasValue)
                    {
                        var cursorValue = Cursor.New<TNode>(Math.Max(0, Math.Min(startValue - 1, endValue - 1)));
                        PageInfo.Value.StartCursor = PageInfo.Value.EndCursor = cursorValue;
                    }
                    else if (after.HasValue)
                    {
                        var cursorValue = Cursor.New<TNode>(Math.Max(0, Math.Max(startValue + 1, endValue + 1)));
                        PageInfo.Value.StartCursor = PageInfo.Value.EndCursor = cursorValue;
                    }
                    else if (first.HasValue)
                    {
                        var cursorValue = Cursor.New<TNode>(Math.Max(0, Math.Min(startValue - 1, endValue - 1)));
                        PageInfo.Value.StartCursor = PageInfo.Value.EndCursor = cursorValue;
                    }
                    else if (last.HasValue)
                    {
                        var cursorValue = Cursor.New<TNode>(Math.Max(0, Math.Max(startValue + 1, endValue + 1)));
                        PageInfo.Value.StartCursor = PageInfo.Value.EndCursor = cursorValue;
                    }

                    PageInfo.Value.HasNextPage =
                        (first.GetValueOrDefault(-1) == 0)
                        ? PageInfo.Value.EndCursor <= maxCursor
                        : PageInfo.Value.EndCursor < maxCursor;

                    PageInfo.Value.HasPreviousPage =
                        PageInfo.Value.StartCursor > minCursor;
                }
            }

            public ConnectionImpl(IEnumerable<TNode> collection, int? first, Cursor? after, int? last, Cursor? before)
                : this(collection.Select((item, index) => Tuple.Create($"{index + 1}", item)), first, after, last, before)
            { }
        }
    }
}
