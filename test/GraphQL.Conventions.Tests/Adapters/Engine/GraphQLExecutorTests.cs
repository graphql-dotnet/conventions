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
                .WithQueryString("query Test($arg: Int!, $arg: Int!) { hello(var: $arg) }")
                .WithVariables(new Dictionary<string, object> { { "arg", 1 } })
                .DisableValidation()
                .ExecuteAsync();

            result.Errors.ShouldBeNull();
            result.Data.ShouldNotBeNull();
            result.Data.ShouldHaveFieldWithValue("hello", "World");
        }

        [Test]
        public async Task Can_Enable_Validation()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("query Test($arg: Int!, $arg: Int!) { hello(var: $arg) }")
                .WithVariables(new Dictionary<string, object> { { "arg", 1 } })
                .EnableValidation()
                .ExecuteAsync();

            result.Errors.ShouldNotBeNull();
            result.Errors.Count.ShouldEqual(1);
            result.Errors.First().Message.ShouldEqual("There can be only one variable named 'arg'");
        }

        public class Query
        {
            public string Hello(int var) => "World";
        }
    }
}
