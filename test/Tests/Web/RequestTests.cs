using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Web;

namespace GraphQL.Conventions.Tests.Web
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
            request.IsValid.ShouldEqual(true);
            request.QueryString.ShouldEqual(string.Empty);
        }
    }
}
