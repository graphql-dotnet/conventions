using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Conventions.Attributes.Execution.Unwrappers;
using GraphQL.Conventions.Attributes.Execution.Wrappers;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions.Handlers
{
    class ExecutionFilterAttributeHandler
    {
        private static readonly IWrapper Wrapper = new ValueWrapper();

        private static readonly IUnwrapper Unwrapper = new ValueUnwrapper();

        public Task<object> Execute(IResolutionContext resolutionContext, Func<IResolutionContext, object> executor)
        {
            var fieldInfo = resolutionContext.FieldInfo;
            foreach (var argument in fieldInfo.Arguments)
            {
                ResolveArgument(argument, resolutionContext);
            }
            var start = (FieldResolutionDelegate)(async ctx =>
            {
                var result = executor(ctx);
                if (result is Task)
                {
                    result = await result.GetTaskResult().ConfigureAwait(false);
                }
                return Unwrapper.Unwrap(result);
            });
            var resolver = start;
            foreach (var executionFilter in fieldInfo.ExecutionFilters.AsEnumerable().Reverse())
            {
                var previousResolver = resolver;
                resolver = ctx => executionFilter.Execute(ctx, previousResolver);
            }
            return resolver(resolutionContext);
        }

        private void ResolveArgument(GraphArgumentInfo argument, IResolutionContext resolutionContext)
        {
            if (argument.IsInjected)
            {
                object obj;
                var argumentType = (TypeInfo)argument.Type.AttributeProvider;
                if (argumentType.AsType() == typeof(IResolutionContext) ||
                    argumentType.ImplementedInterfaces.Any(iface => iface == typeof(IResolutionContext)))
                {
                    obj = resolutionContext;
                }
                else if (argumentType.AsType() == typeof(IUserContext) ||
                    argumentType.ImplementedInterfaces.Any(iface => iface == typeof(IUserContext)))
                {
                    obj = resolutionContext.UserContext;
                }
                else
                {
                    obj = resolutionContext.DependencyInjector?.Resolve(argumentType);
                }
                resolutionContext.SetArgument(argument.Name, obj);
            }
            else
            {
                var argumentValue = resolutionContext.GetArgument(argument);
                resolutionContext.SetArgument(argument.Name, Wrapper.Wrap(argument, argument.Type, argumentValue, true));
            }
        }
    }
}
