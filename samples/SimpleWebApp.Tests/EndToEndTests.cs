using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace SimpleWebApp.Tests;

public class EndToEndTests
{
    [Fact]
    public async Task Search()
    {
        var query = """
            {
              search (forString:"") {
                items {
                  __typename
                  ... on Book {
                    id
                    title
                  }
                  ... on Author {
                    id
                    firstName
                    lastName
                  }
                }
              }
            }
            """;

        var expected = """
            {
              "data": {
                "search": {
                  "items": [
                    {
                      "__typename": "Book",
                      "id": "Qm9vazox",
                      "title": "A Lone Wolf in the Forest"
                    },
                    {
                      "__typename": "Book",
                      "id": "Qm9vazoy",
                      "title": "Surfer Boy"
                    },
                    {
                      "__typename": "Book",
                      "id": "Qm9vazoz",
                      "title": "GraphQL, a Love Story"
                    },
                    {
                      "__typename": "Book",
                      "id": "Qm9vazo0",
                      "title": "Dare I Say What?"
                    },
                    {
                      "__typename": "Author",
                      "id": "QXV0aG9yOjE=",
                      "firstName": "Benny",
                      "lastName": "Frank"
                    }
                  ]
                }
              }
            }
            """;

        using var webApp = new WebApplicationFactory<GraphQL.Conventions.Tests.Server.Program>();
        using var server = webApp.Server;
        server.AllowSynchronousIO = true; // for Newtonsoft.Json support
        await VerifyGraphQLPostAsync(server, "/graphql", query, expected).ConfigureAwait(false);
    }

    [Fact]
    public async Task MutationTest()
    {
        var query1 = """
            mutation {
              addAuthor(input: {
                author: {
                  firstName: "Jane"
                  lastName: "Austen"
                }
              }) {
                __typename
                clientMutationId
                author {
                  __typename
                  id
                  firstName
                  lastName
                }
              }
            }
            """;

        var expected1 = """
            {
              "data": {
                "addAuthor": {
                  "__typename": "AddAuthorResult",
                  "clientMutationId": null,
                  "author": {
                    "__typename": "Author",
                    "id": "QXV0aG9yOjE=",
                    "firstName": "Jane",
                    "lastName": "Austen"
                  }
                }
              }
            }
            """;

        var query2 = """
            mutation {
              addBook(
                input: {
                  clientMutationId: "abc"
                  book: {
                    title: "Pride and Prejudice"
                    releaseDate: "1813-01-28T00:00:00"
                    authors: ["QXV0aG9yOjE="]
                  }
                }
              ) {
                __typename
                clientMutationId
                book {
                  __typename
                  id
                  title
                  releaseDate
                  authors {
                    id
                    firstName
                    lastName
                  }
                }
              }
            }
            """;

        var expected2 = """
            {
              "data": {
                "addBook": {
                  "__typename": "AddBookResult",
                  "clientMutationId": "abc",
                  "book": {
                    "__typename": "Book",
                    "id": "Qm9vazo1",
                    "title": "Pride and Prejudice",
                    "releaseDate": "1813-01-28T00:00:00Z",
                    "authors": [
                      {
                        "id": "QXV0aG9yOjE=",
                        "firstName": "Jane",
                        "lastName": "Austen"
                      }
                    ]
                  }
                }
              }
            }
            """;

        using var webApp = new WebApplicationFactory<GraphQL.Conventions.Tests.Server.Program>();
        using var server = webApp.Server;
        await VerifyGraphQLPostAsync(server, "/graphql", query1, expected1).ConfigureAwait(false);
        await VerifyGraphQLPostAsync(server, "/graphql", query2, expected2).ConfigureAwait(false);
    }

    private static async Task VerifyGraphQLPostAsync(
        TestServer server,
        string url,
        string query,
        string expected,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        using var client = server.CreateClient();
        var body = JsonSerializer.Serialize(new { query });
        var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = content;
        using var response = await client.SendAsync(request).ConfigureAwait(false);
        Assert.Equal(statusCode, response.StatusCode);
        var ret = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var jsonExpected = JsonSerializer.Serialize(JsonSerializer.Deserialize<JsonElement>(expected));
        var jsonActual = JsonSerializer.Serialize(JsonSerializer.Deserialize<JsonElement>(ret));
        Assert.Equal(jsonExpected, jsonActual);
    }
}
