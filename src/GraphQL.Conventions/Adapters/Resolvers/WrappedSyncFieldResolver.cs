using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GraphQL.Conventions.Attributes.Execution.Unwrappers;
using GraphQL.Conventions.Attributes.Execution.Wrappers;
using GraphQL.Conventions.Handlers;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GraphQL.Conventions.Adapters
{
    class WrappedSyncFieldResolver : IFieldResolver
    {
        private static readonly ExecutionFilterAttributeHandler ExecutionFilterHandler =
            new ExecutionFilterAttributeHandler();

        private static readonly IWrapper Wrapper = new ValueWrapper();

        private static readonly IUnwrapper Unwrapper = new ValueUnwrapper();

        private readonly GraphFieldInfo _fieldInfo;

        public WrappedSyncFieldResolver(GraphFieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public object Resolve(ResolveFieldContext context)
        {
            Func<IResolutionContext, object> resolver;
            if (_fieldInfo.IsMethod)
            {
                resolver = ctx => CallMethod(_fieldInfo, ctx);
            }
            else
            {
                resolver = ctx => GetValue(_fieldInfo, ctx);
            }
            var resolutionContext = new ResolutionContext(_fieldInfo, context);
            return ExecutionFilterHandler.Execute(resolutionContext, resolver);
        }

        private object GetValue(GraphFieldInfo fieldInfo, IResolutionContext context)
        {
            var source = GetSource(fieldInfo, context);
            var propertyInfo = fieldInfo.AttributeProvider as PropertyInfo;
            return propertyInfo?.GetValue(source);
        }

        private object CallMethod(GraphFieldInfo fieldInfo, IResolutionContext context)
        {
            var source = GetSource(fieldInfo, context);
            var methodInfo = fieldInfo.AttributeProvider as MethodInfo;

            var arguments = fieldInfo
                .Arguments
                .Select(context.GetArgument);

            if (fieldInfo.Type.IsTask)
            {
                var argumentTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToList();
                var argumentConstants = arguments.Select((arg, i) => Expression.Constant(arg, argumentTypes[i]));
                var methodCall = Expression.Call(Expression.Constant(source), methodInfo, argumentConstants);
                var resolutionTask = Expression.Lambda(methodCall).Compile();
                return AsyncHelpers.RunTask(resolutionTask, fieldInfo.Type.TypeParameter());
            }

            return methodInfo?.Invoke(source, arguments.ToArray());
        }

        private object GetSource(GraphFieldInfo fieldInfo, IResolutionContext context)
        {
            var source = context.Source;
            if (source == null ||
                source.GetType() == typeof(ImplementViewerAttribute.QueryViewer) ||
                source.GetType() == typeof(ImplementViewerAttribute.MutationViewer) ||
                source.GetType() == typeof(ImplementViewerAttribute.SubscriptionViewer))
            {
                var declaringType = fieldInfo.DeclaringType.TypeRepresentation.AsType();
                source = context.DependencyInjector?.Resolve(declaringType.GetTypeInfo()) ?? fieldInfo.SchemaInfo.TypeResolutionDelegate(declaringType);
            }
            source = Unwrapper.Unwrap(source);
            return source;
        }
    }
}
