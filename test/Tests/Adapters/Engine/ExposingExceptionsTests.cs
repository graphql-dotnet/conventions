using GraphQL.Conventions;
using GraphQL.Conventions.Tests;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Adapters.Engine
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
                .Execute();

            result.ExposeExceptions.ShouldBeFalse("By default exposing exceptions should be disabled.");

            var message = JObject.FromObject(result)["errors"].First.Value<string>("message");

            bool anyStackTracePart = message.Contains("at Tests.Adapters.Engine.ExposingExceptionsTests.Query.QueryData() in") ||
                message.Contains("stack trace") ||
                message.Contains("at");

            anyStackTracePart.ShouldBeFalse($"There should be no stack trace in error messages when {nameof(result.ExposeExceptions)} is disabled.");
        }

        [Test]
        public async Task Will_expose_exceptions_when_enabled()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .WithExposedExceptions()
                .NewExecutor()
                .WithQueryString("query { queryData }")
                .Execute();

            result.ExposeExceptions.ShouldBeTrue($"{nameof(result.ExposeExceptions)} should be enabled when {nameof(GraphQLEngine.WithExposedExceptions)} is called.");
            JObject.FromObject(result)["errors"].First.Value<string>("message")
                .Contains("at Tests.Adapters.Engine.ExposingExceptionsTests.Query.QueryData() in")
                .ShouldBeTrue($"There should be stack trace in error messages when {nameof(result.ExposeExceptions)} is enabled.");
        }
        
        class Query
        {
            public string QueryData() => throw new CustomException();
        }

        class CustomException : Exception { }
    }
}
