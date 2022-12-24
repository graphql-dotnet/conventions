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
        var serviceProvider = services.BuildServiceProvider();

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
}
