using GraphQL.Resolvers;
using System;
using System.Threading.Tasks;

namespace GraphQL.Conventions.Adapters.Resolvers
{
    public class EventStreamResolver : ISourceStreamResolver
    {
        private readonly IFieldResolver _fieldResolver;

        public EventStreamResolver(IFieldResolver fieldResolver)
        {
            _fieldResolver = fieldResolver;
        }

        public async ValueTask<IObservable<object>> ResolveAsync(IResolveFieldContext context)
        {
            var result = await _fieldResolver.ResolveAsync(context);
            if (result is Task<object>)
            {
                result = (result as Task<object>).Result;
            }
            return (IObservable<object>)result;
        }
    }
}