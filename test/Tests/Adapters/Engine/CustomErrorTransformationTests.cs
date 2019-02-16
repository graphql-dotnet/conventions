using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Conventions;
using GraphQL.Conventions.Adapters.Engine.ErrorTransformations;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Tests;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;

namespace GraphQL.Conventions.Tests.Adapters.Engine
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
                .Execute();

            result.Errors.ShouldNotBeNull();
            result.Errors.Count.ShouldEqual(1);
            var error = result.Errors.First();
            error.ShouldBeOfType<ExecutionError>();
            error.InnerException.ShouldBeOfType<FieldResolutionException>();
            error.InnerException.InnerException.ShouldBeOfType<CustomException>();
        }

        [Test]
        public async Task Will_Use_Custom_Error_Transformation_When_Provided()
        {
            var engine = GraphQLEngine.New<Query>();
            var result = await engine
                .WithCustomErrorTransformation(new CustomErrorTransformation())
                .NewExecutor()
                .WithQueryString("query { queryData }")
                .Execute();

            result.Errors.ShouldNotBeNull();
            result.Errors.Count.ShouldEqual(1);
            var error = result.Errors.First();
            error.ShouldBeOfType<ExecutionError>();
            error.InnerException.ShouldBeOfType<TargetInvocationException>();
            error.InnerException.InnerException.ShouldBeOfType<CustomException>();
        }

        class Query
        {
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
