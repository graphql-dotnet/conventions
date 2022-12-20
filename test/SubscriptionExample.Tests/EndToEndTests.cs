using System.Net;
using System.Text.Json;
using GraphQL.Transport;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace SubscriptionExample.Tests;

public class EndToEndTests
{
    [Fact]
    public async Task CanPostMessages()
    {
        var query1 = @"
            {
              messages {
                author
                text
              }
            }";

        var expected1 = @"
            {
              ""data"": {
                ""messages"": []
              }
            }";

        var query2 = @"
            mutation {
              addMessage(message: { author: ""John"", text: ""Hello World"" }) {
                author
                text
              }
            }";

        var expected2 = @"
            {
              ""data"": {
                ""addMessage"": {
                  ""author"": ""John"",
                  ""text"": ""Hello World""
                }
              }
            }";

        var query3 = query1;

        var expected3 = @"
            {
              ""data"": {
                ""messages"": [
                  {
                    ""author"": ""John"",
                    ""text"": ""Hello World""
                  }
                ]
              }
            }";

        var query4 = @"
            mutation {
              addMessage(message: { author: ""Jane"", text: ""I like turtles"" }) {
                author
                text
              }
            }";

        var expected4 = @"
            {
              ""data"": {
                ""addMessage"": {
                  ""author"": ""Jane"",
                  ""text"": ""I like turtles""
                }
              }
            }";

        var query5 = query1;

        var expected5 = @"
            {
              ""data"": {
                ""messages"": [
                  {
                    ""author"": ""John"",
                    ""text"": ""Hello World""
                  },
                  {
                    ""author"": ""Jane"",
                    ""text"": ""I like turtles""
                  }
                ]
              }
            }";

        var querySubscription = @"
            subscription {
              messageUpdate {
                author
                text
              }
            }";

        var expectedSubscription = new string[]
        {
            @"{
              ""data"": {
                ""messageUpdate"": {
                  ""author"": ""John"",
                  ""text"": ""Hello World""
                }
              }
            }",
            @"{
              ""data"": {
                ""messageUpdate"": {
                  ""author"": ""Jane"",
                  ""text"": ""I like turtles""
                }
              }
            }"
        };

        using var webApp = new WebApplicationFactory<Program>();
        using var server = webApp.Server;
        var websocketTask = VerifyGraphQLWebSocketsAsync(server, querySubscription, expectedSubscription);
        await VerifyGraphQLPostAsync(server, query: query1, expected: expected1).ConfigureAwait(false);
        await VerifyGraphQLPostAsync(server, query: query2, expected: expected2).ConfigureAwait(false);
        await VerifyGraphQLPostAsync(server, query: query3, expected: expected3).ConfigureAwait(false);
        await VerifyGraphQLPostAsync(server, query: query4, expected: expected4).ConfigureAwait(false);
        await VerifyGraphQLPostAsync(server, query: query5, expected: expected5).ConfigureAwait(false);
        await websocketTask.ConfigureAwait(false);
    }

    private static async Task VerifyGraphQLPostAsync(
        TestServer server,
        string url = "/graphql",
        string query = "{count}",
        string expected = @"{""data"":{""count"":0}}",
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

    private static async Task VerifyGraphQLWebSocketsAsync(
        TestServer server,
        string query,
        IEnumerable<string> expectedMessages,
        string url = "/graphql")
    {
        var webSocketClient = server.CreateWebSocketClient();
        webSocketClient.ConfigureRequest = request => request.Headers["Sec-WebSocket-Protocol"] = "graphql-transport-ws";
        webSocketClient.SubProtocols.Add("graphql-transport-ws");
        using var webSocket = await webSocketClient.ConnectAsync(new Uri(server.BaseAddress, url), default).ConfigureAwait(false);

        // send CONNECTION_INIT
        await webSocket.SendMessageAsync(new OperationMessage
        {
            Type = "connection_init",
        });

        // wait for CONNECTION_ACK
        var message = await webSocket.ReceiveMessageAsync().ConfigureAwait(false);
        Assert.Equal("connection_ack", message.Type);

        // send query
        await webSocket.SendMessageAsync(new OperationMessage
        {
            Id = "1",
            Type = "subscribe",
            Payload = new GraphQLRequest
            {
                Query = query
            }
        });

        // wait for response
        foreach (var expected in expectedMessages)
        {
            message = await webSocket.ReceiveMessageAsync().WaitAsync(TimeSpan.FromSeconds(30)).ConfigureAwait(false);
            Assert.Equal("next", message.Type);
            Assert.Equal("1", message.Id);
            var payloadJson = JsonSerializer.Serialize(message.Payload);
            var expectedJson = JsonSerializer.Serialize(JsonSerializer.Deserialize<JsonElement>(expected));
            Assert.Equal(expectedJson, payloadJson);
        }
    }
}
