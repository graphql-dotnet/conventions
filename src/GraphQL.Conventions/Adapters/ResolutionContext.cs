using System.Collections.Generic;
using System.Threading;
using GraphQL.Conventions.Execution;
using GraphQL.Types;

namespace GraphQL.Conventions.Adapters
{
    public class ResolutionContext : IResolutionContext
    {
        private static object _lock = new object();

        public ResolutionContext(ResolveFieldContext fieldContext)
        {
            FieldContext = fieldContext;
        }

        public object Source => FieldContext.Source;

        public object GetArgument(string name, object defaultValue = null)
        {
            lock (_lock)
            {
                object value;
                if (FieldContext.Arguments != null &&
                    FieldContext.Arguments.TryGetValue(name, out value))
                {
                    return value;
                }
                return defaultValue;
            }
        }

        public void SetArgument(string name, object value)
        {
            lock (_lock)
            {
                if (FieldContext.Arguments == null)
                {
                    FieldContext.Arguments = new Dictionary<string, object>();
                }
                FieldContext.Arguments[name] = value;
            }
        }

        public object RootValue => FieldContext.RootValue;

        public object UserContext => FieldContext.UserContext;

        public CancellationToken CancellationToken => FieldContext.CancellationToken;

        public ResolveFieldContext FieldContext { get; private set; }
    }
}