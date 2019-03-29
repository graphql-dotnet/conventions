using System;
using System.Collections.Generic;
using System.Threading;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Types;

namespace GraphQL.Conventions.Adapters
{
    public class ResolutionContext : IResolutionContext
    {
        private static object _lock = new object();

        public ResolutionContext(GraphFieldInfo fieldInfo, ResolveFieldContext<object> fieldContext)
        {
            FieldContext = fieldContext;
            FieldInfo = fieldInfo;
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

        public object GetArgument(GraphArgumentInfo argument)
        {
            var value = GetArgument(argument.Name, argument.DefaultValue);
            if (!argument.Type.IsNullable && value == null)
            {
                throw new ArgumentException($"Null value provided for non-nullable argument '{argument.Name}'.");
            }
            return value;
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

        public IUserContext UserContext => (FieldContext.UserContext as UserContextWrapper)?.UserContext;

        public IDependencyInjector DependencyInjector => (FieldContext.UserContext as UserContextWrapper)?.DependencyInjector;

        public GraphFieldInfo FieldInfo { get; private set; }

        public CancellationToken CancellationToken => FieldContext.CancellationToken;

        public IEnumerable<string> Path => FieldContext.Path;

        public ResolveFieldContext<object> FieldContext { get; private set; }
    }
}