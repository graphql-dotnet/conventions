using System;
using System.Collections.Generic;

namespace GraphQL.Conventions.Types.Descriptors
{
    public class GraphSchemaInfo
    {
        private readonly Dictionary<string, object> _attributes = new Dictionary<string, object>();

        private Func<Type, object> _typeResolutionDelegate;

        public GraphTypeInfo Query { get; set; }

        public GraphTypeInfo Mutation { get; set; }

        public GraphTypeInfo Subscription { get; set; }

        public List<GraphTypeInfo> Types { get; private set; } = new List<GraphTypeInfo>();

        public Func<Type, object> TypeResolutionDelegate
        {
            get => _typeResolutionDelegate ?? (type => Activator.CreateInstance(type));
            set => _typeResolutionDelegate = value;
        }

        public object this[string key]
        {
            get => GetAttribute<object>(key);
            set => _attributes[key] = value;
        }

        public T GetAttribute<T>(string key, T defaultValue = default)
        {
            if (_attributes.TryGetValue(key, out object value))
            {
                return value is T t ? t : defaultValue;
            }
            return defaultValue;
        }

        public override string ToString() => nameof(GraphSchemaInfo);
    }
}
