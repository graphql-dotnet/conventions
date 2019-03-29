using GraphQL;
using GraphQL.Conventions;
using GraphQL.Conventions.Adapters;
using GraphQL.Conventions.Builders;
using GraphQL.Http;
using GraphQL.Server;
using GraphQL.Server.Internal;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Server.Transports.WebSockets;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SubscriptionExample.Core;

namespace SubscriptionExample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var typeAdapter = new GraphTypeAdapter();
            var constructor = new SchemaConstructor<ISchema, IGraphType>(typeAdapter);
            var schema = constructor.Build(typeof(SchemaDefinition<GraphQl.Query, GraphQl.Mutation, GraphQl.Subscription>));
            var graphQLEngine = new GraphQLEngine()
                .WithFieldResolutionStrategy(FieldResolutionStrategy.Normal)
                .WithQuery<GraphQl.Query>()
                .WithMutation<GraphQl.Mutation>()
                .WithSubscription<GraphQl.Subscription>()
                .BuildSchema();

            services.AddSingleton<MessageService>();
            services.AddSingleton(schema);
            services.AddMvc();
            services.AddTransient<IDependencyInjector, Injector>();
            services.AddSingleton(provider => graphQLEngine);
            services.AddSingleton<IDocumentExecuter, GraphQL.Conventions.DocumentExecuter>();
            services.AddSingleton<IDocumentWriter>(x =>
            {
                var jsonSerializerSettings = x.GetRequiredService<IOptions<JsonSerializerSettings>>();
                return new DocumentWriter(Formatting.None, jsonSerializerSettings.Value);
            });
            services.AddTransient(typeof(IGraphQLExecuter<>), typeof(DefaultGraphQLExecuter<>));
            services.AddTransient<IWebSocketConnectionFactory<ISchema>, WebSocketConnectionFactory<ISchema>>();
            services.AddTransient<IOperationMessageListener, LogMessagesListener>();
            services.AddTransient<IOperationMessageListener, ProtocolMessageListener>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseWebSockets();
            app.UseGraphQLWebSockets<ISchema>();

            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions()
            {
                Path = "/ui/playground",
                GraphQLEndPoint = "/graphql"
            });
        }
    }
}