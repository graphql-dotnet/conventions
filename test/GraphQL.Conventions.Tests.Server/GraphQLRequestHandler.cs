using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using GraphQL.Conventions.Adapters.Engine;
using GraphQL.Conventions.Types.Resolution;

namespace GraphQL.Conventions.Tests.Server
{
    public class GraphQLRequestHandler
    {
        private readonly GraphQLEngine _engine;

        private readonly IDependencyInjector _dependencyInjector;

        public GraphQLRequestHandler(IDependencyInjector dependencyInjector, params Type[] schemaTypes)
        {
            _engine = new GraphQLEngine(schemaTypes);
            _dependencyInjector = dependencyInjector;
        }

        public async Task HandleRequest(HttpContext context)
        {
            if (context.Request.Method != "POST")
            {
                context.Response.StatusCode = 400;
                return;
            }

            using (var streamReader = new StreamReader(context.Request.Body))
            {
                var requestBody = streamReader.ReadToEnd();
                var responseBody = await _engine
                    .NewExecutor()
                    .WithDependencyInjector(_dependencyInjector)
                    .WithRequest(requestBody)
                    .ExecuteAndSerializeResponse();
                context.Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                await context.Response.WriteAsync(responseBody);
            }
        }
    }
}
