using System.Threading.Tasks;
using GraphQL.Conventions.Extensions;
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
                .ProcessRequest(request, null, null);

            response.ExecutionResult.Data.ShouldHaveFieldWithValue("hello", "World");
            response.Body.ShouldEqual("{\"data\":{\"hello\":\"World\"}}");
            response.Errors.Count.ShouldEqual(0);
            response.Warnings.Count.ShouldEqual(0);
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
                .ProcessRequest(request, null, null);

            response.ExecutionResult.Data.ShouldHaveFieldWithValue("hello", "World");
            response.Body.ShouldEqual("{\"data\":{\"hello\":\"World\"}}");
            response.Errors.Count.ShouldEqual(0);
            response.Warnings.Count.ShouldEqual(0);
        }

        [Test]
        public async Task Cannot_Run_Too_Complex_Query_Using_ComplexityConfiguration()
        {
            var request = Request.New("{ \"query\": \"{ sub { sub { end } } }\" }");
            var response = await RequestHandler
                .New()
                .WithQuery<TestQuery>()
                .WithComplexityConfiguration(new ComplexityConfiguration { MaxDepth = 1 })
                .Generate()
                .ProcessRequest(request, null, null);

            response.Errors.Count.ShouldEqual(1);
            response.Errors[0].Message.ShouldEqual("Query is too nested to execute. Depth is 2 levels, maximum allowed on this endpoint is 1.");
        }

        [Test]
        public async Task Can_Enrich_With_Profiling_Information()
        {
            var request = Request.New("{ \"query\": \"{ a: foo(ms: 10) b: foo(ms: 20) }\" }");
            var response = await RequestHandler
                .New()
                .WithQuery<ProfiledQuery>()
                .WithProfiling()
                .Generate()
                .ProcessRequest(request, null, null);
            response.EnrichWithProfilingInformation();
            response.Body.ShouldContain("\"extensions\":{\"profile\":");
        }

        [Test]
        public async Task Can_Ignore_Types_From_Unwanted_Namespaces()
        {
            // Include all types from CompositeQuery
            var request = Request.New("{ \"query\": \"{ earth { hello } mars { hello } } \" }");
            var response = await RequestHandler
                .New()
                .WithQuery<CompositeQuery>()
                .Generate()
                .ProcessRequest(request, null, null);
            response.Body.ShouldEqual("{\"data\":{\"earth\":{\"hello\":\"World\"},\"mars\":{\"hello\":\"World From Mars\"}}}");

            // Exclude types from 'Unwanted' namespace, i.e. TypeQuery2 from CompositeQuery schema
            request = Request.New("{ \"query\": \"{ earth { hello } mars { hello } } \" }");
            response = await RequestHandler
                .New()
                .IgnoreTypesFromNamespacesStartingWith("GraphQL.Conventions.Tests.Web.Unwanted")
                .WithQuery<CompositeQuery>()
                .Generate()
                .ProcessRequest(request, null, null);
            response.Errors.Count.ShouldEqual(1);
            response.Errors[0].Message.ShouldContain("Cannot query field \"hello\" on type \"TestQuery2\".");
            response.Body.ShouldContain("VALIDATION_ERROR");
        }

        [Test]
        public async Task Can_Run_Query_With_Type_Extensions()
        {
            var request = Request.New("{ \"query\": \"{ helloExtended(v: 10) }\" }");
            var response = await RequestHandler
                .New()
                .WithQuery<SimpleQuery>()
                .WithQueryExtensions(typeof(QueryExtensions))
                .Generate()
                .ProcessRequest(request, null, null);

            response.ExecutionResult.Data.ShouldHaveFieldWithValue("helloExtended", "Extended-10");
            response.Body.ShouldEqual("{\"data\":{\"helloExtended\":\"Extended-10\"}}");
            response.Errors.Count.ShouldEqual(0);
            response.Warnings.Count.ShouldEqual(0);
        }

        [Test]
        public async Task Can_Run_Query_With_Nested_Type_Extensions()
        {
            var request = Request.New("{ \"query\": \"{ helloType(v: 1) { myName }}\" }");
            var response = await RequestHandler
                .New()
                .WithQuery<SimpleQuery>()
                .WithQueryExtensions(typeof(QueryExtensions))
                .Generate()
                .ProcessRequest(request, null, null);

            response.Body.ShouldEqual("{\"data\":{\"helloType\":{\"myName\":\"Name-1\"}}}");
            response.Errors.Count.ShouldEqual(0);
            response.Warnings.Count.ShouldEqual(0);
        }

        [Test]
        public void Can_Construct_And_Describe_Schema_With_Extensions()
        {
            var schema = RequestHandler
                .New()
                .WithQuery<SimpleQuery>()
                .WithQueryExtensions(typeof(QueryExtensions))
                .Generate()
                .DescribeSchema(false, false, false);

            schema.ShouldEqualWhenReformatted(@"
                schema {
                    query: SimpleQuery
                }
                type HelloType {
                    myName: String
                }
                type SimpleQuery {
                    hello: String
                    helloExtended(v: Int!): String
                    helloType(v: Int!): HelloType
                }
            ");
        }

        class TestQuery
        {
            public string Hello => "World";

            public Nested Sub => new Nested();
        }

        class Nested
        {
            public Nested Sub => new Nested();

            public string End => string.Empty;
        }

        class ProfiledQuery
        {
            public async Task<int> Foo(int ms)
            {
                await Task.Delay(ms);
                return ms;
            }
        }

        class CompositeQuery
        {
            public TestQuery Earth => new TestQuery();
            public Unwanted.TestQuery2 Mars => new Unwanted.TestQuery2();
        }
    }

    class SimpleQuery
    {
        public string Hello => "World";
        public HelloType HelloType(int v) => new HelloType(v);
    }

    class HelloType
    {
        [Ignore]
        public int HiddenVersion { get; set; }

        public HelloType(int v)
        {
            HiddenVersion = v;
        }
    }

    static class QueryExtensions
    {
        public static string HelloExtended(this SimpleQuery query, int v) => $"Extended-{v}";

        public static string MyName(this HelloType helloType) => $"Name-{helloType.HiddenVersion}";
    }

    namespace Unwanted
    {
        class TestQuery2
        {
            public string Hello => "World From Mars";
        }
    }
}
