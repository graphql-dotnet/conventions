using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Conventions
{
    class CachedRegistry<TKey, TValue>
    {
        private object _lock = new object();

        private readonly Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();

        public TValue AddEntity(TKey key, TValue value)
        {
            lock (_lock)
            {
                _cache[key] = value;
            }
            return value;
        }

        public TValue GetOrAddEntity(TKey key, Func<TValue> valueGenerator)
        {
            TValue value;
            lock (_lock)
            {
                if (!_cache.TryGetValue(key, out value))
                {
                    value = _cache[key] = valueGenerator();
                }
                return value;
            }
        }

        public TValue GetEntity(TKey key)
        {
            TValue value;
            lock (_lock)
            {
                return _cache.TryGetValue(key, out value) ? value : default(TValue);
            }
        }

        public IEnumerable<string> Keys => _cache.Keys.Select(k => k.ToString()).ToArray();

        public IEnumerable<TValue> Entities => _cache.Values;
    }
}
