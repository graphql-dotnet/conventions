using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Conventions;
using GraphQL.Conventions.Adapters.Engine.ErrorTransformations;
using GraphQL.Conventions.Execution;
using Tests.Templates;
using Tests.Templates.Extensions;

namespace Tests.Adapters.Engine
{
    public class CustomErrorTransformationTests : TestBase
    {
        [Test]
        public async Task Will_Use_Default_Error_Transformation_When_Not_Provided()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .NewExecutor()
                .WithQueryString("query { queryData }")
                .ExecuteAsync();

            result.Errors.ShouldNotBeNull();
            result.Errors.Count.ShouldEqual(1);
            var error = result.Errors.First();
            error.ShouldBeOfType<ExecutionError>();
            error.InnerException.ShouldBeOfType<FieldResolutionException>();
            error.InnerException?.InnerException.ShouldBeOfType<CustomException>();
        }

        [Test]
        public async Task Will_Use_Custom_Error_Transformation_When_Provided()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .WithCustomErrorTransformation(new CustomErrorTransformation())
                .NewExecutor()
                .WithQueryString("query { queryData }")
                .ExecuteAsync();

            result.Errors.ShouldNotBeNull();
            result.Errors.Count.ShouldEqual(1);
            var error = result.Errors.First();
            error.ShouldBeOfType<ExecutionError>();
            error.InnerException.ShouldBeOfType<CustomException>();
        }

        class Query
        {
            // ReSharper disable once UnusedMember.Local
            public string QueryData() => throw new CustomException();
        }

        class CustomErrorTransformation : IErrorTransformation
        {
            public ExecutionErrors Transform(ExecutionErrors errors)
                => errors;
        }

        class CustomException : Exception { }
    }
}
