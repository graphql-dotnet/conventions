using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Conventions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<V> Select<U, V>(this NonNull<IEnumerable<U>> list, Func<U, V> selector) =>
            list.Value.Select(selector);

        public static IEnumerable<V> Select<U, V>(this NonNull<List<U>> list, Func<U, V> selector) =>
            list.Value.Select(selector);

        public static IEnumerable<U> Where<U>(this NonNull<IEnumerable<U>> list, Func<U, bool> predicate) =>
            list.Value.Where(predicate);

        public static IEnumerable<U> Where<U>(this NonNull<List<U>> list, Func<U, bool> predicate) =>
            list.Value.Where(predicate);

        public static U FirstOrDefault<U>(this NonNull<IEnumerable<U>> list, Func<U, bool> predicate) =>
            list.Value.FirstOrDefault(predicate);

        public static U FirstOrDefault<U>(this NonNull<List<U>> list, Func<U, bool> predicate) =>
            list.Value.FirstOrDefault(predicate);

        public static U First<U>(this NonNull<IEnumerable<U>> list, Func<U, bool> predicate) =>
            list.Value.First(predicate);

        public static U First<U>(this NonNull<List<U>> list, Func<U, bool> predicate) =>
            list.Value.First(predicate);
    }
}
