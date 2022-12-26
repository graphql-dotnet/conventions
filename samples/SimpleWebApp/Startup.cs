using GraphQL.Conventions.Tests.Server.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQL.Conventions.Tests.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<BookRepository>();
            services.AddSingleton<AuthorRepository>();
            services.AddGraphQL(b => b
                .AddSystemTextJson()
                .AddConventionsSchema<Schema.Query, Schema.Mutation>()
                .AddUserContextBuilder(_ => new UserContext()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseGraphQL();
            app.UseGraphQLPlayground("/");
        }
    }
}
