using System.Collections;
using System.Collections.Generic;

namespace GraphQL.Conventions
{
    public class UserContextBase : IUserContext
    {
        protected IDictionary<string, object> Dictionary { get; set; } = new Dictionary<string, object>();

        object IDictionary<string, object>.this[string key] { get => Dictionary[key]; set => Dictionary[key] = value; }
        ICollection<string> IDictionary<string, object>.Keys => Dictionary.Keys;
        ICollection<object> IDictionary<string, object>.Values => Dictionary.Values;
        int ICollection<KeyValuePair<string, object>>.Count => Dictionary.Count;
        bool ICollection<KeyValuePair<string, object>>.IsReadOnly => Dictionary.IsReadOnly;
        void IDictionary<string, object>.Add(string key, object value) => Dictionary.Add(key, value);
        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item) => Dictionary.Add(item);
        void ICollection<KeyValuePair<string, object>>.Clear() => Dictionary.Clear();
        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item) => Dictionary.Contains(item);
        bool IDictionary<string, object>.ContainsKey(string key) => Dictionary.ContainsKey(key);
        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => Dictionary.CopyTo(array, arrayIndex);
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => Dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Dictionary).GetEnumerator();
        bool IDictionary<string, object>.Remove(string key) => Dictionary.Remove(key);
        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item) => Dictionary.Remove(item);
        bool IDictionary<string, object>.TryGetValue(string key, out object value) => Dictionary.TryGetValue(key, out value);
    }
}
