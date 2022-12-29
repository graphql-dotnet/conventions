using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Conventions.Web;
using GraphQL.Validation;
using Tests.Templates;
using Tests.Templates.Extensions;

namespace Tests.Web
{
    public class ResponseTests : TestBase
    {
        [Test]
        public void Can_Instantiate_Response_Object_From_Execution_Result()
        {
            var request = Request.New("{\"query\":\"{}\"}");
            var result = new ExecutionResult { Data = new Dictionary<string, object>() };
            var response = new Response(request, result);
            response.HasData.ShouldEqual(true);
            response.HasErrors.ShouldEqual(false);
        }

        [Test]
        public void Can_Instantiate_Response_Object_From_Validation_Result()
        {
            var request = Request.New("{\"query\":\"{}\"}");
            var result = new ValidationResult(Enumerable.Empty<ValidationError>());
            result.Errors.Add(new ExecutionError("Test"));
            var response = new Response(request, result);
            response.ValidationResult.Errors.Count.ShouldEqual(1);
        }

        [Test]
        public async void Can_Instantiate_Response_Object_With_No_Data()
        {
            var request = Request.New("{\"query\":\"{ }\"}");
            var result = new ExecutionResult
            {
                Extensions = new Dictionary<string, object>
                {
                    { "trace", new
                        {
                            foo = 1,
                            bar = new
                            {
                                baz = "hello",
                            },
                        }
                    }
                }
            };
            var response = new Response(request, result);
            response.HasData.ShouldEqual(false);
            response.HasErrors.ShouldEqual(false);

            var body = response.GetBody();
            body.ShouldEqual("{\"extensions\":{\"trace\":{\"foo\":1,\"bar\":{\"baz\":\"hello\"}}}}");
        }
    }
}
