using GraphQL.Conventions.Web;
using Tests.Templates;
using Tests.Templates.Extensions;

namespace Tests.Web
{
    public class RequestTests : TestBase
    {
        [Test]
        public void Can_Instantiate_Request_Object_From_String()
        {
            var request = Request.New("{\"query\":\"{}\"}");
            request.IsValid.ShouldEqual(true);
            request.QueryString.ShouldEqual("{}");
            request.Variables.ShouldEqual(null);
            request.Extensions.ShouldEqual(null);
        }

        [Test]
        public void Can_Instantiate_Request_Object_From_Data_String_Without_Variables()
        {
            var request = Request.New("{\"data\":{\"query\":\"{}\"}}");
            request.IsValid.ShouldEqual(true);
            request.QueryString.ShouldEqual("{}");
            request.Variables.ShouldEqual(null);
            request.Extensions.ShouldEqual(null);
        }

        [Test]
        public void Can_Instantiate_Request_Object_From_Data_String_With_Variables()
        {
            var request = Request.New("{\"data\":{\"query\":\"{ foo(baz: 1) { bar } }\",\"variables\":{\"arg\":10}}}");
            request.IsValid.ShouldEqual(true);
            request.QueryString.ShouldEqual("{ foo(baz: 1) { bar } }");
            request.Variables.ShouldNotEqual(null);
            request.Variables["arg"].ShouldEqual(10);
            request.Extensions.ShouldEqual(null);
        }

        [Test]
        public void Can_Instantiate_Request_Object_From_Data_String_With_Extensions()
        {
            var request = Request.New("{\"data\":{\"query\":\"{ foo(baz: 1) { bar } }\",\"variables\":{\"arg\":10}}}, \"extensions\": {\"traceId\": 100}");
            request.IsValid.ShouldEqual(true);
            request.QueryString.ShouldEqual("{ foo(baz: 1) { bar } }");
            request.Variables.ShouldNotEqual(null);
            request.Variables["arg"].ShouldEqual(10);
            request.Extensions.ShouldNotEqual(null);
            request.Extensions["traceId"].ShouldEqual(100);
        }

        [Test]
        public void Cannot_Instantiate_Request_Object_From_Invalid_String()
        {
            var request = Request.New("\"invalid_query\":\"{}\"");
            request.IsValid.ShouldEqual(false);
        }

        [Test]
        public void Cannot_Derive_Query_From_Invalid_String()
        {
            var request = Request.New("{\"invalid_query\":\"{}\"}");
            request.IsValid.ShouldEqual(false);
            request.QueryString.ShouldEqual(string.Empty);
        }
    }
}
