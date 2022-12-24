using System.Threading.Tasks;
using GraphQL;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SubscriptionExample.Core;
using SubscriptionExample.GraphQl;

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
            services.AddGraphQL(builder => builder
                .AddSystemTextJson()
                .AddErrorInfoProvider(option => option.ExposeExceptionDetails = true)
                .AddDataLoader()
                // .UseApolloTracing()
                .ConfigureExecutionOptions(options =>
                {
                    var logger = options.RequestServices.GetRequiredService<ILogger<Startup>>();
                    options.UnhandledExceptionDelegate = ctx =>
                    {
                        logger.LogError($"GraphQL Unhandled Exception: {ctx.ErrorMessage}.", ctx.OriginalException);
                        return Task.CompletedTask;
                    };
                })
                .AddConventionsSchema(s => s
                    .WithFieldResolutionStrategy(FieldResolutionStrategy.Normal)
                    .WithQuery<Query>()
                    .WithMutation<Mutation>()
                    .WithSubscription<Subscription>())
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWebSockets();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
                endpoints.MapGraphQLPlayground();
            });
        }
    }
}
