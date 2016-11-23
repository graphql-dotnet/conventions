GraphQL Conventions Library for .NET
====================================

[![Build status](https://ci.appveyor.com/api/projects/status/a8oyaoubntd6ft9n/branch/master?svg=true)](https://ci.appveyor.com/project/tlil87/conventions/branch/master) [![NuGet version](https://img.shields.io/nuget/v/GraphQL.Conventions.svg)](https://www.nuget.org/packages/GraphQL.Conventions)

## Introduction
[GraphQL .NET](https://www.github.com/graphql-dotnet/graphql-dotnet) has been around for a while. This library is a complementary layer on top that allows you to automatically wrap your .NET classes into GraphQL schema definitions using existing property getters and methods as field resolvers.

In short, this project builds on top of the following components:

 * The [GraphQL](https://github.com/graphql-dotnet/graphql-dotnet) library written by [Joe McBride](https://github.com/joemcbride) (MIT licence)
 * The GraphQL [lexer/parser](http://github.com/graphql-dotnet/parser) originally written by [Marek Magdziak](https://github.com/mkmarek) (MIT licence)

## Installation

Download and install the package from [NuGet](https://www.nuget.org/packages/GraphQL.Conventions):

```powershell
PS> Install-Package GraphQL.Conventions
```

The following targets are available:

 * .NET Framework 4.5
 * .NET Platform Standard 1.5
 * .NET Core 1.0

## Getting Started

Implement your query type:

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
            .Select(node => new SearchResult { Instance = node })
            .ToConnection(first ?? 5, after);
    }
}
```

Construct your schema and run your query:

```cs
var engine = new GraphQLEngine()
    .BuildSchema(typeof(SchemaDefinition<Query>));

var result = await engine
    .NewExecutor()
    .WithUserContext(userContext)
    .WithDependencyInjector(dependencyInjector)
    .WithRequest(requestBody)
    .Execute();
```

## Examples

More detailed examples can be found in the [unit tests](https://github.com/graphql-dotnet/conventions/tree/master/test/GraphQL.Conventions.Tests) and the included [test server](https://github.com/graphql-dotnet/conventions/tree/master/test/GraphQL.Conventions.Tests.Server).
