using GraphQL.Conventions;
using GraphQL.Conventions.Tests;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
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
            var result = await engine
                .NewExecutor()
                .WithQueryString("query { queryData }")
                .Execute();

            result.ExposeExceptions.ShouldBeFalse("By default exposing exceptions should be disabled.");
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
        }

        class Query
        {
            public string QueryData() => throw new CustomException();
        }

        class CustomException : Exception { }
    }
}
