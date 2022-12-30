using GraphQL.Conventions;
using GraphQL.Conventions.Tests.Server.Data.Repositories;
using GraphQL.Conventions.Tests.Server.Schema;
using GraphQL.Conventions.Web;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleWebApp.Tests;

public class SimpleWebAppSchemaCreationTests
{
    [Fact]
    public async Task TestSimpleWebAppAsync()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IBookRepository>(new BookRepository());
        services.AddSingleton<IAuthorRepository>(new AuthorRepository());
        using var serviceProvider = services.BuildServiceProvider();

        var requestHandler = RequestHandler
            .New()
            .WithServiceProvider(serviceProvider)
            .WithQueryAndMutation<Query, Mutation>()
            .Generate();

        var request = Request.New(new QueryInput { QueryString = "{ __schema { types { name } } }" });
        var result = await requestHandler.ProcessRequestAsync(request, null, serviceProvider);

        Assert.False(result.HasErrors);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task TestSimpleWebAppNewAsync()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IBookRepository>(new BookRepository());
        services.AddSingleton<IAuthorRepository>(new AuthorRepository());
        using var serviceProvider = services.BuildServiceProvider();

        var schema = GraphQLEngine
            .New()
            .WithQueryAndMutation<Query, Mutation>()
            .GetSchema();
        var executer = new GraphQL.DocumentExecuter();

        var result = await executer.ExecuteAsync(new()
        {
            Schema = schema,
            Query = "{ __schema { types { name } } }",
            RequestServices = serviceProvider,
        });

        Assert.Null(result.Errors);
        Assert.NotNull(result.Data);
    }
}
