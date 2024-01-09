using GraphQL.Conventions;
using GraphQL.Conventions.Tests.Server;
using GraphQL.Conventions.Tests.Server.Data.Repositories;
using GraphQL.Conventions.Tests.Server.Schema;
using GraphQL.Conventions.Web;

namespace SimpleWebApp.Tests;

public class SimpleWebAppSchemaCreationTests
{
    [Fact]
    public async Task TestSimpleWebAppAsync()
    {
        var dependencyInjector = new DependencyInjector();
        dependencyInjector.Register<IBookRepository>(new BookRepository());
        dependencyInjector.Register<IAuthorRepository>(new AuthorRepository());

        var requestHandler = RequestHandler
            .New()
            .WithDependencyInjector(dependencyInjector)
            .WithQueryAndMutation<Query, Mutation>()
            .Generate();

        var request = Request.New(new QueryInput { QueryString = "{ __schema { types { name } } }" });
        var result = await requestHandler.ProcessRequestAsync(request, null, dependencyInjector);

        Assert.False(result.HasErrors);
        Assert.True(result.IsValid);
    }
}
