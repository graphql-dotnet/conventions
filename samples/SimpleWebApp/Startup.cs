using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GraphQL.Conventions.Tests.Server.Data.Repositories;
using GraphQL.Conventions.Web;

namespace GraphQL.Conventions.Tests.Server
{
    public class Startup
    {
        private IRequestHandler _requestHandler;

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            var dependencyInjector = new DependencyInjector();
            dependencyInjector.Register<IBookRepository>(new BookRepository());
            dependencyInjector.Register<IAuthorRepository>(new AuthorRepository());

            _requestHandler = RequestHandler
                .New()
                .WithDependencyInjector(dependencyInjector)
                .WithQueryAndMutation<Schema.Query, Schema.Mutation>()
                .Generate();

            app.Run(HandleRequest);
        }

        private async Task HandleRequest(HttpContext context)
        {
            if (string.Compare(context.Request.Method, "OPTIONS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                context.Response.StatusCode = 200;
                return;
            }

            if (string.Compare(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase) != 0)
            {
                context.Response.StatusCode = 400;
                return;
            }

            var streamReader = new StreamReader(context.Request.Body);
            var body = streamReader.ReadToEnd();
            var userContext = new UserContext();
            var result = await _requestHandler
                .ProcessRequest(Request.New(body), userContext);
            context.Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync(result.Body);
        }
    }
}
