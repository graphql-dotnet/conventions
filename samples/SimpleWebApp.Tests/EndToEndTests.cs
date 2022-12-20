using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace SubscriptionExample.Tests;

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
