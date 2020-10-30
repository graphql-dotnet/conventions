using System;
using System.Threading.Tasks;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using Newtonsoft.Json.Linq;

namespace GraphQL.Conventions.Tests.Adapters.Engine
{
    public class ExposingExceptionsTests : TestBase
    {
        [Test]
        public async Task Will_not_expose_exceptions_when_not_enabled()
        {
            var engine = GraphQLEngine.New<Query>();

            GraphQL.ExecutionResult result = await engine
                .NewExecutor()
                .WithQueryString("query { queryData }")
                .ExecuteAsync();

            var message = JObject.FromObject(result)["Errors"].First.Value<string>("Message");

            bool anyStackTracePart = message.Contains("at Tests.Adapters.Engine.ExposingExceptionsTests.Query.QueryData() in") ||
                message.Contains("stack trace") ||
                message.Contains("at");

            anyStackTracePart.ShouldBeFalse($"There should be no stack trace in error messages when is disabled.");
        }

        [Test]
        public async Task Will_expose_exceptions_when_enabled()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("query { queryData }")
                .ExecuteAsync();

            JObject.FromObject(result)["Errors"].First.Value<string>("Message")
                .Contains(typeof(CustomException).FullName)
                .ShouldBeTrue($"There should be stack trace in error messages when is enabled.");
        }
        
        class Query
        {
            public string QueryData() => throw new CustomException();
        }

        class CustomException : Exception { }
    }
}
