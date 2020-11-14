using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    class CachedRegistry<TKey, TValue>
    {
        private readonly object _lock = new object();

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
            lock (_lock)
            {
                if (!_cache.TryGetValue(key, out var value))
                {
                    value = _cache[key] = valueGenerator();
                }

                return value;
            }
        }

        public TValue GetEntity(TKey key)
        {
            lock (_lock)
            {
                return _cache.TryGetValue(key, out var value) ? value : default;
            }
        }

        public IEnumerable<TValue> Entities
        {
            get
            {
                lock (_lock)
                {
                    return _cache.Values;
                }
            }
        }
    }
}
