using GraphQL;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SubscriptionExample.Controllers
{
    [ApiController]
    [Route("graphql")]
    public sealed class GraphController : ControllerBase
    {
        private readonly GraphQLEngine _engine;
        private readonly IDependencyInjector _injector;

        public GraphController(GraphQLEngine engine, IDependencyInjector injector)
        {
            _engine = engine;
            _injector = injector;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            string requestBody;
            using (var reader = new StreamReader(Request.Body))
                requestBody = await reader.ReadToEndAsync();

            ExecutionResult result = await _engine
                .NewExecutor()
                .WithDependencyInjector(_injector)
                .WithRequest(requestBody)
                .Execute();

            var responseBody = _engine.SerializeResult(result);

            HttpStatusCode statusCode = HttpStatusCode.OK;

            if (result.Errors?.Any() ?? false)
            {
                statusCode = HttpStatusCode.InternalServerError;
                if (result.Errors.Any(x => x.Code == "VALIDATION_ERROR"))
                    statusCode = HttpStatusCode.BadRequest;
                else if (result.Errors.Any(x => x.Code == "UNAUTHORIZED_ACCESS"))
                    statusCode = HttpStatusCode.Forbidden;
            }

            return new ContentResult
            {
                Content = responseBody,
                ContentType = "application/json; charset=utf-8",
                StatusCode = (int)statusCode
            };
        }
    }
}