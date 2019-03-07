using GraphQL.Conventions.Attributes.Execution.Unwrappers;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Relay;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Resolvers;
using GraphQL.Subscription;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GraphQL.Conventions.Adapters.Resolvers
{
    internal class EventStreamResolver : FieldResolver, IEventStreamResolver
    {
        public EventStreamResolver(GraphFieldInfo fieldInfo) : base(fieldInfo)
        {
        }

        public IObservable<object> Subscribe(ResolveEventStreamContext context)
        {
            var source = GetSource(_fieldInfo, context);
            return (IObservable<object>)source.GetPropertyValue(_fieldInfo.Name);
        }

        private object GetSource(GraphFieldInfo fieldInfo, ResolveEventStreamContext context)
        {
            var source = context.Source;
            if (source == null ||
                source.GetType() == typeof(ImplementViewerAttribute.QueryViewer) ||
                source.GetType() == typeof(ImplementViewerAttribute.MutationViewer) ||
                source.GetType() == typeof(ImplementViewerAttribute.SubscriptionViewer))
            {
                var declaringType = fieldInfo.DeclaringType.TypeRepresentation.AsType();
                source = (context.UserContext as UserContextWrapper)
                    .DependencyInjector
                    ?.Resolve(declaringType.GetTypeInfo()) ?? fieldInfo.SchemaInfo.TypeResolutionDelegate(declaringType);
            }
            source = Unwrapper.Unwrap(source);
            return source;
        }
    }
}