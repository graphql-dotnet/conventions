using DataLoaderWithEFCore.Data;
using DataLoaderWithEFCore.Data.Repositories;
using DataLoaderWithEFCore.GraphApi;
using DataLoaderWithEFCore.GraphApi.Schema;
using GraphQL;
using GraphQL.Conventions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IActorRepository, ActorRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();

builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(Mappings));

builder.Services.AddGraphQL(b => b
    .AddSystemTextJson()
    .AddErrorInfoProvider(option => option.ExposeExceptionDetails = true)
    .AddDataLoader()
    .ConfigureExecutionOptions(options =>
    {
        var logger = options.RequestServices!.GetRequiredService<ILogger<Program>>();
        options.UnhandledExceptionDelegate = ctx =>
        {
            logger.LogError($"GraphQL Unhandled Exception: {ctx.ErrorMessage}.", ctx.OriginalException);
            return Task.CompletedTask;
        };
    })
    .AddConventionsSchema(s => s
        .WithFieldResolutionStrategy(FieldResolutionStrategy.Normal)
        .WithQuery<Query>()
        .WithMutation<Mutation>()));

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseGraphQL("/api/graph");
app.UseGraphQLPlayground(options: new GraphQL.Server.Ui.Playground.PlaygroundOptions { GraphQLEndPoint = "/api/graph" });

await app.RunAsync();
