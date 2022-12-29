using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using GraphQL.Conventions;
using Tests.Templates;
using Tests.Types;

namespace Tests.Adapters
{
    public class EventStreamResolverTest : TestBase
    {
        [Test]
        public async Task Can_Subscribe()
        {
            var engine = GraphQLEngine.New(new GraphQL.DocumentExecuter())
                .WithQuery<TestQuery>()
                .WithSubscription<Subscription>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("subscription { test }")
                .ExecuteAsync();

            Assert.AreEqual(1, result.Streams.Count);
        }
    }

    internal class Subscription
    {
        private readonly Subject<string> _subject;

        public Subscription()
        {
            _subject = new Subject<string>();
        }

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public IObservable<string> Test => _subject;
    }
}
