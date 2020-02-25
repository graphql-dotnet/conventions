using System;
using System.Linq;
using System.Reflection;
using GraphQL.Conventions.Attributes.Execution.Unwrappers;
using GraphQL.Conventions.Attributes.Execution.Wrappers;
using GraphQL.Conventions.Handlers;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GraphQL.Conventions.Adapters
{
    public class FieldResolver : IFieldResolver<object>
    {
        protected static readonly ExecutionFilterAttributeHandler ExecutionFilterHandler =
            new ExecutionFilterAttributeHandler();

        private static readonly IWrapper Wrapper = new ValueWrapper();

        protected static readonly IUnwrapper Unwrapper = new ValueUnwrapper();

        protected readonly GraphFieldInfo _fieldInfo;

        public FieldResolver(GraphFieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public virtual object Resolve(ResolveFieldContext context)
        {
            return Resolve(new ResolveFieldContext<object>(context));
        }

        public object Resolve(ResolveFieldContext<object> context)
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
                .Select(arg => context.GetArgument(arg));

            if (fieldInfo.IsExtensionMethod)
            {
                arguments = new[] { source }.Concat(arguments);
            }
            var result = methodInfo?.Invoke(source, arguments.ToArray());
            return result;
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