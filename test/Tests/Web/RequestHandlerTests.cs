using System.Threading.Tasks;
using GraphQL.Conventions.Extensions;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Web;
using GraphQL.Validation.Complexity;
using System;

namespace GraphQL.Conventions.Tests.Web
{
    public class RequestHandlerTests : TestBase, IDisposable
    {
        [Test]
        public async Task Can_Run_Query()
        {
            var request = Request.New("{ \"query\": \"{ hello }\" }");
            var response = await RequestHandler
                .New()
                .WithQuery<TestQuery>()
                .Generate()
                .ProcessRequest(request, null);

            response.ExecutionResult.Data.ShouldHaveFieldWithValue("hello", "World");
            response.Body.ShouldEqual("{\"data\":{\"hello\":\"World\"}}");
            response.Errors.Count.ShouldEqual(0);
            response.Warnings.Count.ShouldEqual(0);
        }

        [Test]
        public async Task Can_Run_Query_And_Report_Validation_Violations_As_Warnings()
        {
            var request = Request.New("{ \"query\": \"query test($foo: String) { a: hello b: hello }\" }");
            var response = await RequestHandler
                .New()
                .WithQuery<TestQuery>()
                .WithoutValidation(true)
                .Generate()
                .ProcessRequest(request, null);

            response.Body.ShouldEqual("{\"data\":{\"a\":\"World\",\"b\":\"World\"}}");
            response.Errors.Count.ShouldEqual(0);
            response.Warnings.Count.ShouldEqual(1);
            response.Warnings[0].ToString().ShouldEqual("GraphQL.Validation.ValidationError: Variable \"$foo\" is never used in operation \"$test\".");
        }

        [Test]
        public async Task Can_Run_Simple_Query_Using_ComplexityConfiguration()
        {
            var request = Request.New("{ \"query\": \"{ hello }\" }");
            var response = await RequestHandler
                .New()
                .WithQuery<TestQuery>()
                .WithComplexityConfiguration(new ComplexityConfiguration { MaxDepth = 2 })
                .Generate()
                .ProcessRequest(request, null);

            response.ExecutionResult.Data.ShouldHaveFieldWithValue("hello", "World");
            response.Body.ShouldEqual("{\"data\":{\"hello\":\"World\"}}");
            response.Errors.Count.ShouldEqual(0);
            response.Warnings.Count.ShouldEqual(0);
        }

        [Test]
        public async void Cannot_Run_Too_Complex_Query_Using_ComplexityConfiguration()
        {
            var request = Request.New("{ \"query\": \"{ hello { is_it_me { youre_looking_for } } }\" }");
            var response = await RequestHandler
                .New()
                .WithQuery<TestQuery>()
                .WithComplexityConfiguration(new ComplexityConfiguration { MaxDepth = 1 })
                .Generate()
                .ProcessRequest(request, null);

            response.Errors.Count.ShouldEqual(1);
            response.Errors[0].Message.ShouldEqual("Query is too nested to execute. Depth is 2 levels, maximum allowed on this endpoint is 1.");
        }

        [Test]
        public async void Can_Enrich_With_Profiling_Information()
        {
            var request = Request.New("{ \"query\": \"{ a: foo(ms: 10) b: foo(ms: 20) }\" }");
            var response = await RequestHandler
                .New()
                .WithQuery<ProfiledQuery>()
                .WithProfiling()
                .Generate()
                .ProcessRequest(request, null);
            response.EnrichWithProfilingInformation();
            response.Body.ShouldContain("\"extra\":{\"profile\":");
        }

        [Test]
        public void Throws_Exception_When_Query_Has_Void_Field()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var request = Request.New("{ \"query\": \"{ hello }\" }");
                var response = RequestHandler
                    .New()
                    .WithQuery<TestQueryWithVoidField>()
                    .Generate()
                    .ProcessRequest(request, null).Result;
            });
        }

        [Test]
        public async void Can_Ignore_Fields_With_Void_ReturnType()
        {
            var request = Request.New("{ \"query\": \"{ hello }\" }");
            var response = await RequestHandler
                .New()
                .IgnoreFieldsWithVoidReturnType()
                .WithQuery<TestQueryWithVoidField>()
                .Generate()
                .ProcessRequest(request, null);
            response.Body.ShouldContain("{\"data\":{\"hello\":\"World\"}}");
        }

        public void Dispose()
        {
            ReflectorSettingsExtensions.ResetIgnoreFieldWithVoidReturnType();
        }


        class TestQuery
        {
            public string Hello => "World";
        }

        class TestQueryWithVoidField : TestQuery
        {
            public void Goodbye() { }
        }

        class ProfiledQuery
        {
            public async Task<int> Foo(int ms)
            {
                await Task.Delay(ms);
                return ms;
            }
        }
    }
}
