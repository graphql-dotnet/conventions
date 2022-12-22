using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Execution;

namespace GraphQL.Conventions.Adapters
{
    public class ResolutionContext : IResolutionContext
    {
        private static readonly object Lock = new object();

        public ResolutionContext(GraphFieldInfo fieldInfo, IResolveFieldContext fieldContext)
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
                    return value.Value;
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
                var fieldContext = FieldContext is ResolveFieldContext context
                    ? context
                    : new ResolveFieldContext(FieldContext);

                fieldContext.Arguments ??= new Dictionary<string, ArgumentValue>();
                fieldContext.Arguments[name] = new ArgumentValue(value, ArgumentSource.Variable);
                FieldContext = fieldContext;
            }
        }

        public object RootValue => FieldContext.RootValue;

        public IUserContext UserContext => FieldContext.UserContext as IUserContext;

        public IServiceProvider ServiceProvider => FieldContext.RequestServices;

        public GraphFieldInfo FieldInfo { get; }

        public CancellationToken CancellationToken => FieldContext.CancellationToken;

        public IEnumerable<string> Path => FieldContext.Path.Select(o => o?.ToString());

        public IResolveFieldContext FieldContext { get; private set; }
    }
}
