GraphQL Conventions Library for .NET
====================================

[![Build status](https://ci.appveyor.com/api/projects/status/a8oyaoubntd6ft9n/branch/master?svg=true)](https://ci.appveyor.com/project/tlil87/conventions/branch/master)

## Introduction
[GraphQL .NET](https://www.github.com/graphql-dotnet/graphql-dotnet) has been around for a while. This library is a complementary layer on top that allows you to automatically wrap your .NET classes into GraphQL schema definitions using existing property getters and methods as field resolvers.

## Getting Started

```cs
[ImplementViewer(OperationType.Query)]
public class Query
{
    [Description("Retrieve book by its globally unique ID.")]
    public Task<Book> Book(UserContext context, Id id) =>
        context.Get<Book>(id);

    [Description("Retrieve author by his/her globally unique ID.")]
    public Task<Author> Author(UserContext context, Id id) =>
        context.Get<Author>(id);

    [Description("Search for books and authors.")]
    public Connection<SearchResult> Search(
        UserContext context,
        [Description("Title or last name.")] NonNull<string> forString,
        [Description("Only return search results after given cursor.")] Cursor? after,
        [Description("Return the first N results.")] int? first)
    {
        return context
            .Search(forString.Value)
            .Select(node => new SearchResult {Instance = node})
            .ToConnection(first ?? 5, after);
    }
}

var engine = new GraphQLEngine()
    .BuildSchema(typeof(SchemaDefinition<Query>));

var result = await engine
    .NewExecutor()
    .WithUserContext(userContext)
    .WithDependencyInjector(dependencyInjector)
    .WithRequest(requestBody)
    .Execute();
```