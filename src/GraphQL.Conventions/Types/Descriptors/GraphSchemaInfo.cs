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
            get { return _typeResolutionDelegate ?? (type => Activator.CreateInstance(type)); }
            set { _typeResolutionDelegate = value; }
        }

        public object this[string key]
        {
            get { return GetAttribute<object>(key); }
            set { _attributes[key] = value; }
        }

        public T GetAttribute<T>(string key, T defaultValue = default)
        {
            object value;
            if (_attributes.TryGetValue(key, out value))
            {
                return value is T ? (T)value : defaultValue;
            }
            return defaultValue;
        }

        public override string ToString() => nameof(GraphSchemaInfo);
    }
}
