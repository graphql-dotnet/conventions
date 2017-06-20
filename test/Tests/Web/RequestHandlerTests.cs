using System.Threading.Tasks;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Web;
using GraphQL.Validation.Complexity;

namespace GraphQL.Conventions.Tests.Web
{
    public class RequestHandlerTests : TestBase
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

        class TestQuery
        {
            public string Hello => "World";
        }
    }
}
