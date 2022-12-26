using System.Threading.Tasks;
using GraphQL;
using GraphQL.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SubscriptionExample.Core;
using SubscriptionExample.GraphQl;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MessageService>();
builder.Services.AddGraphQL(b => b
    .AddSystemTextJson()
    .AddErrorInfoProvider(option => option.ExposeExceptionDetails = true)
    .AddDataLoader()
    .ConfigureExecutionOptions(options =>
    {
        var logger = options.RequestServices.GetRequiredService<ILogger<Program>>();
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
        .WithSubscription<Subscription>()));

var app = builder.Build();
app.UseGraphQL();
app.UseGraphQLPlayground();

await app.RunAsync();
