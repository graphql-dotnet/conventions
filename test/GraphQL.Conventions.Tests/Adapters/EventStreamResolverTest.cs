using GraphQL.Conventions.Adapters.Resolvers;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Subscription;
using GraphQL.Types;
using System;
using System.Threading.Tasks;

namespace GraphQL.Conventions.Tests.Adapters
{
    public class EventStreamResolverTest : TestBase
    {
        [Test]
        public void Can_Resolve_Value()
        {
            var result = "testring";
            var context = new ResolveFieldContext { Source = result };
            var resolver = new EventStreamResolver(null);

            Assert.AreEqual(result, resolver.Resolve(context));
        }

        [Test]
        public async Task Can_Subscribe()
        {
            var engine = GraphQLEngine.New().WithSubscription<Subscription>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("subscription { test }")
                .Execute();

            Assert.AreEqual(1, ((SubscriptionExecutionResult)result).Streams.Count);
        }
    }

    class Subscription
    {
        public IObservable<string> Test { get; }
    }
}
