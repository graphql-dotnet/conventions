using System.Threading.Tasks;
using GraphQL;
using GraphQL.Conventions;
using GraphQL.Conventions.Adapters;
using GraphQL.Conventions.Builders;
using GraphQL.Conventions.Types.Resolution;
using GraphQL.DataLoader;
using GraphQL.MicrosoftDI;
using GraphQL.NewtonsoftJson;
using GraphQL.Server;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SubscriptionExample.Core;
using SubscriptionExample.GraphQl;
using DocumentExecuter = GraphQL.Conventions.DocumentExecuter;

namespace SubscriptionExample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MessageService>();

            // Graph QL Server Services
            services.AddGraphQL(builder =>
            {
                builder
                    .AddHttpMiddleware<ISchema, GraphQLHttpMiddleware<ISchema>>()
                    .AddWebSocketsHttpMiddleware<ISchema>()
                    .AddDefaultEndpointSelectorPolicy()
                    .AddSystemTextJson()
                    .AddErrorInfoProvider(option =>
                    {
                        option.ExposeExceptionStackTrace = true;
                    })
                    .AddDataLoader()
                    .AddWebSockets()
                    .ConfigureExecutionOptions(options =>
                    {
                        options.EnableMetrics = true;
                        var logger = options.RequestServices.GetRequiredService<ILogger<Startup>>();
                        options.UnhandledExceptionDelegate = ctx =>
                        {
                            logger.LogError($"GraphQL Unhandled Exception: {ctx.ErrorMessage}.", ctx.OriginalException);
                            return Task.CompletedTask;
                        };
                    });
            });

            // Graph QL Convention: Engine and Schema
            var engine = new GraphQLEngine()
                .WithFieldResolutionStrategy(FieldResolutionStrategy.Normal)
                .WithQuery<Query>()
                .WithMutation<Mutation>()
                .WithSubscription<Subscription>()
                .BuildSchema();

            var schema = engine.GetSchema();
            
            // Add Graph QL Convention Services
            services.AddSingleton(engine);
            services.AddSingleton(schema);
            services.AddTransient<IDependencyInjector, Injector>();

            // Replace GraphQL Server with GraphQL Convention Document Executer
            services.Replace(new ServiceDescriptor(typeof(IDocumentExecuter), typeof(DocumentExecuter), ServiceLifetime.Singleton));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWebSockets();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQLWebSockets<ISchema>();
                endpoints.MapGraphQL<ISchema>();
                endpoints.MapGraphQLPlayground();
            });
        }
    }
}