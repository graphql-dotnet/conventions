using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Attributes.Execution.Unwrappers;
using GraphQL.Conventions.Attributes.Execution.Wrappers;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;

namespace GraphQL.Conventions.Handlers
{
    class ExecutionFilterAttributeHandler
    {
        private static long _correlationId;

        private static object _lock = new object();

        private static readonly IWrapper _wrapper = new ValueWrapper();

        private static readonly IUnwrapper _unwrapper = new ValueUnwrapper();

        public async Task<ExecutionContext> Execute(GraphFieldInfo field, IResolutionContext resolutionContext, Func<IResolutionContext, object> executor)
        {
            foreach (var argument in field.Arguments)
            {
                var argumentExecutionContext = ResolveArgument(argument, resolutionContext);
                if (argumentExecutionContext.Exception != null)
                {
                    return new ExecutionContext(field, resolutionContext)
                    {
                        Exception = argumentExecutionContext.Exception,
                    };
                }
            }

            var executionContext = new ExecutionContext(field, resolutionContext);
            var filterAttributes = field
                .ExecutionFilters
                .Where(attribute => attribute.IsEnabled(executionContext))
                .ToList();

            if (Enumerable.Any(filterAttributes, attribute => !attribute.ShouldExecute(executionContext)))
            {
                return executionContext;
            }

            var correlationId = GetNextCorrelationId();

            ProtectedInvocation(executionContext, () =>
            {
                foreach (var attribute in filterAttributes)
                {
                    attribute.BeforeExecution(executionContext, correlationId);
                }
            });

            ProtectedInvocation(executionContext, () =>
            {
                executionContext.Result = executor(executionContext.ResolutionContext);
                executionContext.DidExecute = true;
            });

            if (executionContext.DidSucceed)
            {
                var result = executionContext.Result;
                if (result is Task)
                {
                    await ProtectedAsyncInvocation(executionContext, async () =>
                    {
                        executionContext.Result = await executionContext.Result.GetTaskResult().ConfigureAwait(false);
                        return true;
                    }).ConfigureAwait(false);
                }
            }

            ProtectedInvocation(executionContext, () =>
            {
                foreach (var attribute in filterAttributes.Reverse<IExecutionFilterAttribute>())
                {
                    attribute.AfterExecution(executionContext, correlationId);
                }
            });

            executionContext.Result = _unwrapper.Unwrap(executionContext.Result);
            return executionContext;
        }

        private ExecutionContext ResolveArgument(GraphArgumentInfo argument, IResolutionContext resolutionContext)
        {
            var executionContext = new ExecutionContext(argument, resolutionContext);

            if (argument.IsInjected)
            {
                object obj;
                var argumentType = argument.Type.AttributeProvider as TypeInfo;
                if (argumentType.AsType() == typeof(IResolutionContext))
                {
                    obj = resolutionContext;
                }
                else if (argumentType.AsType() == typeof(IUserContext))
                {
                    obj = resolutionContext.UserContext;
                }
                else
                {
                    obj = argument.TypeResolver.DependencyInjector?.Resolve(argumentType);
                }
                resolutionContext.SetArgument(argument.Name, obj);
                return executionContext;
            }

            object argumentValue = resolutionContext.GetArgument(argument.Name, argument.DefaultValue);

            var filterAttributes = argument
                .ExecutionFilters
                .Where(attribute => attribute.IsEnabled(executionContext))
                .ToList();

            var correlationId = GetNextCorrelationId();

            ProtectedInvocation(executionContext, () =>
            {
                foreach (var attribute in filterAttributes)
                {
                    attribute.BeforeExecution(executionContext, correlationId);
                }

                resolutionContext.SetArgument(argument.Name, _wrapper.Wrap(argument.Type, argumentValue));

                foreach (var attribute in filterAttributes.Reverse<IExecutionFilterAttribute>())
                {
                    attribute.AfterExecution(executionContext, correlationId);
                }
            });

            return executionContext;
        }

        private void ProtectedInvocation(ExecutionContext executionContext, Action action)
        {
            if (executionContext.Exception != null)
            {
                return;
            }
            try
            {
                action();
            }
            catch (Exception ex)
            {
                executionContext.Exception = ex;
            }
        }

        private async Task<bool> ProtectedAsyncInvocation(ExecutionContext executionContext, Func<Task<bool>> action)
        {
            if (executionContext.Exception != null)
            {
                return false;
            }
            try
            {
                return await action().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                executionContext.Exception = ex;
                return false;
            }
        }

        private long GetNextCorrelationId()
        {
            long correlationId;
            lock (_lock)
            {
                correlationId = ++_correlationId;
            }
            return correlationId;
        }
    }
}
