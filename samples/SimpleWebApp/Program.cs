using GraphQL;
using GraphQL.Conventions;
using GraphQL.Conventions.Tests.Server;
using GraphQL.Conventions.Tests.Server.Data.Repositories;
using GraphQL.Conventions.Tests.Server.Schema;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IBookRepository, BookRepository>();
builder.Services.AddSingleton<IAuthorRepository, AuthorRepository>();
builder.Services.AddGraphQL(b => b
    .AddSystemTextJson()
    .AddConventionsSchema<Query, Mutation>()
    .AddUserContextBuilder(_ => new UserContext()));
builder.Logging.AddConsole();

var app = builder.Build();
app.UseGraphQL();
app.UseGraphQLPlayground("/");

await app.RunAsync();
