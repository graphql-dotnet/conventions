using System.Linq;
using System.Reflection;
using GraphQL.Conventions.Attributes.Execution.Unwrappers;
using GraphQL.Conventions.Attributes.Execution.Wrappers;
using GraphQL.Conventions.Attributes.MetaData.Relay;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Types.Resolution.Evaluators
{
    class FieldResolver : IFieldResolver
    {
        private static readonly IWrapper _wrapper = new ValueWrapper();

        private static readonly IUnwrapper _unwrapper = new ValueUnwrapper();

        public object GetValue(GraphFieldInfo fieldInfo, IResolutionContext context)
        {
            var source = GetSource(fieldInfo, context);
            var propertyInfo = fieldInfo.AttributeProvider as PropertyInfo;
            return propertyInfo.GetValue(source);
        }

        public object CallMethod(GraphFieldInfo fieldInfo, IResolutionContext context)
        {
            var source = GetSource(fieldInfo, context);
            var methodInfo = fieldInfo.AttributeProvider as MethodInfo;

            var arguments = fieldInfo
                .Arguments
                .Select(arg => context.GetArgument(arg.Name, arg.DefaultValue));

            return methodInfo.Invoke(source, arguments.ToArray());
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
                source = fieldInfo.SchemaInfo.TypeResolutionDelegate(declaringType);
            }
            source = _unwrapper.Unwrap(source);
            return source;
        }
    }
}
