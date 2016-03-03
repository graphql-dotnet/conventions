using System.Collections.Generic;
using System.Linq;
using GraphQL.Conventions.Adapters.Engine;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Types;
using Xunit;

namespace GraphQL.Conventions.Tests.Adapters.Engine
{
    public class GraphQLExecutorTests : TestBase
    {
        [Fact]
        public async void Can_Disable_Validation()
        {
            var engine = new GraphQLEngine(typeof(SchemaDefinition<Query>));
            var result = await engine
                .NewExecutor()
                .WithQueryString("{ test }")
                .UseValidation(false)
                .Execute();

            result.Data.ShouldNotBeNull();
            var dict = result.Data as Dictionary<string, object>;
            dict.Count.ShouldEqual(0);

            result.Errors.ShouldBeNull();
        }

        [Fact]
        public async void Can_Enable_Validation()
        {
            var engine = new GraphQLEngine(typeof(SchemaDefinition<Query>));
            var result = await engine
                .NewExecutor()
                .WithQueryString("{ test }")
                .UseValidation(true)
                .Execute();

            result.Errors.ShouldNotBeNull();
            result.Errors.Count.ShouldEqual(1);
            result.Errors.First().Message.ShouldEqual("Cannot query field \"test\" on type \"Query\".");
        }
    }
}