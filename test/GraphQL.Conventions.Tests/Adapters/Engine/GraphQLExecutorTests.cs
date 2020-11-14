using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Conventions;
using Tests.Templates;
using Tests.Templates.Extensions;

namespace Tests.Adapters.Engine
{
    public class GraphQLExecutorTests : TestBase
    {
        [Test]
        public async Task Can_Disable_Validation()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("{ test }")
                .DisableValidation()
                .ExecuteAsync();

            result.Data.ShouldNotBeNull();
            var count = (result.Data as Dictionary<string, object>)?.Count;
            count.ShouldEqual(0);

            result.Errors.ShouldBeNull();
        }

        [Test]
        public async Task Can_Enable_Validation()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("{ test }")
                .EnableValidation()
                .ExecuteAsync();

            result.Errors.ShouldNotBeNull();
            result.Errors.Count.ShouldEqual(1);
            result.Errors.First().Message.ShouldEqual("Cannot query field \"test\" on type \"Query\".");
        }
    }
}
