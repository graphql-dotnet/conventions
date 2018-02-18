using System.Collections.Generic;
using GraphQL.Conventions.Tests.Templates;
using GraphQL.Conventions.Tests.Templates.Extensions;
using GraphQL.Conventions.Web;
using GraphQL.Validation;
using Newtonsoft.Json;

namespace GraphQL.Conventions.Tests.Web
{
    public class ResponseTests : TestBase
    {
        [Test]
        public void Can_Instantiate_Response_Object_From_Execution_Result()
        {
            var request = Request.New("{\"query\":\"{}\"}");
            var result = new ExecutionResult();
            result.Data = new Dictionary<string, object>();
            var response = new Response(request, result);
            response.HasData.ShouldEqual(true);
            response.HasErrors.ShouldEqual(false);
        }

        [Test]
        public void Can_Instantiate_Response_Object_From_Validation_Result()
        {
            var request = Request.New("{\"query\":\"{}\"}");
            var result = new ValidationResult();
            result.Errors.Add(new ExecutionError("Test"));
            var response = new Response(request, result);
            response.ValidationResult.Errors.Count.ShouldEqual(1);
        }

        [Test]
        public void Can_Instantiate_Response_Object_With_Extra_Data()
        {
            var request = Request.New("{\"query\":\"{}\"}");
            var result = new ExecutionResult
            {
                Data = new Dictionary<string, object>(),
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
            response.HasData.ShouldEqual(true);
            response.HasErrors.ShouldEqual(false);
            response.Body.ShouldEqual("{\"data\":{},\"extensions\":{\"trace\":{\"foo\":1,\"bar\":{\"baz\":\"hello\"}}}}");
        }
    }
}
