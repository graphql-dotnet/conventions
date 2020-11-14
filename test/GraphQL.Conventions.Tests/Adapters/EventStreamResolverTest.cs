using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Conventions;
using GraphQL.Conventions.Adapters.Resolvers;
using GraphQL.Subscription;
using Tests.Templates;

namespace Tests.Adapters
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
                .ExecuteAsync();

            Assert.AreEqual(1, ((SubscriptionExecutionResult)result).Streams.Count);
        }
    }

    class Subscription
    {
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public IObservable<string> Test { get; }
    }
}
