using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Resolvers;
using GraphQL.Subscription;
using GraphQL.Types;
using System;
using System.Threading.Tasks;

namespace GraphQL.Conventions.Adapters.Resolvers
{
    internal class EventStreamResolver : FieldResolver, IEventStreamResolver
    {
        public EventStreamResolver(GraphFieldInfo fieldInfo) : base(fieldInfo)
        {
        }

        public IObservable<object> Subscribe(ResolveEventStreamContext context)
        {
            var result = Resolve(context);
            if (result is Task<object>)
            {
                result = (result as Task<object>).Result;
            }
            return (IObservable<object>)result;
        }

        public override object Resolve(ResolveFieldContext context)
        {
            return context.Source;
        }
    }
}