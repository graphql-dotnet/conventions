using DataLoaderWithEFCore.Data;
using DataLoaderWithEFCore.Data.Repositories;
using DataLoaderWithEFCore.GraphApi;
using GraphQL.Conventions;
using GraphQL.DataLoader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Schema = DataLoaderWithEFCore.GraphApi.Schema;

namespace DataLoaderWithEFCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MovieDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IActorRepository, ActorRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IMovieRepository, MovieRepository>();

            services.AddSingleton(_ => new GraphQLEngine()
                .WithFieldResolutionStrategy(FieldResolutionStrategy.Normal)
                .BuildSchema(typeof(SchemaDefinition<Schema.Query, Schema.Mutation>)));

            services.AddScoped<Schema.Query>();
            services.AddScoped<Schema.Mutation>();

            services.AddScoped<DataLoaderContext>();
            services.AddAutoMapper(typeof(Mappings));
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseGraphQLPlayground(options: new GraphQL.Server.Ui.Playground.PlaygroundOptions { GraphQLEndPoint = "/api/graph" });
            app.UseEndpoints(configure => configure.MapControllers());
        }
    }
}
