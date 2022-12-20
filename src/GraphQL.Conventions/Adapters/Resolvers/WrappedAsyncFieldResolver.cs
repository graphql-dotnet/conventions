using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Conventions.Attributes.Execution.Unwrappers;
using GraphQL.Conventions.Extensions;
using GraphQL.Conventions.Handlers;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;
using GraphQL.Resolvers;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions.Adapters
{
    internal class WrappedAsyncFieldResolver : IFieldResolver
    {
        private static readonly ExecutionFilterAttributeHandler ExecutionFilterHandler =
            new ExecutionFilterAttributeHandler();

        private static readonly IUnwrapper Unwrapper = new ValueUnwrapper();

        private readonly GraphFieldInfo _fieldInfo;

        public WrappedAsyncFieldResolver(GraphFieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public async ValueTask<object> ResolveAsync(IResolveFieldContext context)
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
            var resolutionContext = new ResolutionContext(_fieldInfo, new ResolveFieldContext<object>(context));
            return await ExecutionFilterHandler.Execute(resolutionContext, resolver);
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

            if (fieldInfo.Type.IsTask && methodInfo != null)
            {
                var argumentTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToList();
                var argumentConstants = arguments.Select((arg, i) => Expression.Constant(arg, argumentTypes[i]));
                var methodCall = Expression.Call(Expression.Constant(source), methodInfo, argumentConstants);
                var resolutionTask = Expression.Lambda(methodCall).Compile();
                return Task.Run(() => AsyncHelpers.RunTask(resolutionTask, fieldInfo.Type.TypeParameter()));
            }

            return methodInfo?.InvokeEnhanced(source, arguments.ToArray());
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
                source = context.DependencyInjector?.GetService(declaringType) ?? fieldInfo.SchemaInfo.TypeResolutionDelegate(declaringType);
            }
            source = Unwrapper.Unwrap(source);
            return source;
        }
    }
}
