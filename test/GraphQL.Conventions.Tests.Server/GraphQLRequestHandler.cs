using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GraphQL.Conventions.Tests.Server
{
    public class GraphQLRequestHandler
    {
        private readonly GraphQLEngine _engine;

        private readonly IDependencyInjector _dependencyInjector;

        public GraphQLRequestHandler(IDependencyInjector dependencyInjector, params Type[] schemaTypes)
        {
            _engine = new GraphQLEngine();
            _engine.BuildSchema(schemaTypes);
            _dependencyInjector = dependencyInjector;
        }

        public async Task HandleRequest(HttpContext context)
        {
            if (string.Compare(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase) != 0)
            {
                context.Response.StatusCode = 400;
                return;
            }

            var userContext = new UserContext();
            using (var streamReader = new StreamReader(context.Request.Body))
            {
                var requestBody = await streamReader.ReadToEndAsync();
                var result = await _engine
                    .NewExecutor()
                    .WithUserContext(userContext)
                    .WithDependencyInjector(_dependencyInjector)
                    .WithRequest(requestBody)
                    .Execute();
                var responseBody = _engine.SerializeResult(result);
                context.Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                context.Response.StatusCode = result.Errors?.Count > 0 ? 400 : 200;
                await context.Response.WriteAsync(responseBody);
            }
        }
    }
}
