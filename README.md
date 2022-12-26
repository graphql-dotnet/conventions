GraphQL Conventions Library for .NET
====================================

## Introduction
[GraphQL .NET](https://www.github.com/graphql-dotnet/graphql-dotnet) has been around for a while. This library is a complementary layer on top that allows you to automatically wrap your .NET classes into GraphQL schema definitions using existing property getters and methods as field resolvers.

In short, this project builds on top of the following components:

 * The [GraphQL](https://github.com/graphql-dotnet/graphql-dotnet) library written by [Joe McBride](https://github.com/joemcbride) (MIT licence)
 * The GraphQL [lexer/parser](http://github.com/graphql-dotnet/parser) originally written by [Marek Magdziak](https://github.com/mkmarek) (MIT licence)

>**Disclaimer:**
>I am providing code in this repository to you under an open source licence ([MIT](LICENSE.md)). Because this is my personal repository, the licence you receive to my code is from me and not my employer (Facebook).

## Installation

Download and install the package from [NuGet](https://www.nuget.org/packages/GraphQL.Conventions):

```powershell
PS> Install-Package GraphQL.Conventions
```

This project targets [.NET Standard] 2.0.

[.NET Standard]: https://docs.microsoft.com/en-us/dotnet/standard/net-standard

## Getting Started

Implement your query type:

```csharp
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

Configure your service provider:

```csharp
using GraphQL;
using GraphQL.Conventions;

services.AddGraphQL(b => b
    .AddConventionsSchema<Query>()
    .AddSystemTextJson());
```

Optionally you may add a configuration delegate to the `AddConventionsSchema`
call for additional options:

```csharp
services.AddGraphQL(b => b
    .AddConventionsSchema<Query>(s => s
        .WithMutation<Mutation>())
    .AddSystemTextJson());
```

Use the [GraphQL.Server.Transports.AspNetCore](https://github.com/graphql-dotnet/server) project to host
an endpoint at `/graphql` and user interface at `/ui/playground`:

```csharp
app.UseGraphQL();
app.UseGraphQLPlayground();
```

## Examples

More detailed examples can be found in the [unit tests](https://github.com/graphql-dotnet/conventions/tree/master/test/GraphQL.Conventions.Tests).
