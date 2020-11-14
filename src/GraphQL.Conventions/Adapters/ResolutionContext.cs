using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Adapters
{
    public class ResolutionContext : IResolutionContext
    {
        private static readonly object Lock = new object();

        public ResolutionContext(GraphFieldInfo fieldInfo, ResolveFieldContext<object> fieldContext)
        {
            FieldContext = fieldContext;
            FieldInfo = fieldInfo;
        }

        public object Source => FieldContext.Source;

        public object GetArgument(string name, object defaultValue = null)
        {
            lock (Lock)
            {
                if (FieldContext.Arguments != null &&
                    FieldContext.Arguments.TryGetValue(name, out var value))
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
            lock (Lock)
            {
                if (FieldContext.Arguments == null)
                {
                    FieldContext.Arguments = new Dictionary<string, object>();
                }
                FieldContext.Arguments[name] = value;
            }
        }

        public object RootValue => FieldContext.RootValue;

        public IUserContext UserContext => FieldContext.GetUserContext();

        public IDependencyInjector DependencyInjector => FieldContext.GetDependencyInjector();

        public GraphFieldInfo FieldInfo { get; }

        public CancellationToken CancellationToken => FieldContext.CancellationToken;

        public IEnumerable<string> Path => FieldContext.Path.Select(o => o?.ToString());

        public ResolveFieldContext<object> FieldContext { get; }
    }
}
