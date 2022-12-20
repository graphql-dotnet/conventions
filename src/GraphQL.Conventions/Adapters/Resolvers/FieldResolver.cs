using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Conventions.Attributes.Execution.Unwrappers;
using GraphQL.Conventions.Extensions;
using GraphQL.Conventions.Handlers;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Resolvers;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions.Adapters
{
    public class FieldResolver : IFieldResolver
    {
        protected static readonly ExecutionFilterAttributeHandler ExecutionFilterHandler =
            new ExecutionFilterAttributeHandler();

        protected static readonly IUnwrapper Unwrapper = new ValueUnwrapper();

        protected readonly GraphFieldInfo _fieldInfo;

        public FieldResolver(GraphFieldInfo fieldInfo)
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
            var resolutionContext = new ResolutionContext(_fieldInfo, context);
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
                .Select(arg => context.GetArgument(arg));

            if (fieldInfo.IsExtensionMethod)
            {
                arguments = new[] { source }.Concat(arguments);
            }
            var result = methodInfo?.InvokeEnhanced(source, arguments.ToArray());
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
                source = context.DependencyInjector?.GetService(declaringType.GetTypeInfo()) ?? fieldInfo.SchemaInfo.TypeResolutionDelegate(declaringType);
            }
            source = Unwrapper.Unwrap(source);
            return source;
        }
    }
}
