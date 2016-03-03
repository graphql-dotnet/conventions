using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GraphQL.Conventions.Types;
using GraphQL.Conventions.Tests.Server.Data.Repositories;

namespace GraphQL.Conventions.Tests.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            var dependencyInjector = new DependencyInjector();
            dependencyInjector.Register<IBookRepository>(new BookRepository());
            dependencyInjector.Register<IAuthorRepository>(new AuthorRepository());

            var requestHandler = new GraphQLRequestHandler(dependencyInjector,
                typeof(SchemaDefinition<Schema.Query, Schema.Mutation>));

            app.Run(requestHandler.HandleRequest);
        }
    }
}
