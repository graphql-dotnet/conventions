using GraphQL;
using GraphQL.SystemTextJson;
using GraphQL.Transport;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Xunit;

namespace SubscriptionExample.Tests;

public static class WebSocketExtensions
{
    private static readonly IGraphQLTextSerializer _serializer = new GraphQLSerializer();

    public static Task SendMessageAsync(this WebSocket socket, OperationMessage message)
        => SendStringAsync(socket, _serializer.Serialize(message));

    public static async Task SendStringAsync(this WebSocket socket, string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        await socket.SendAsync(new Memory<byte>(bytes), WebSocketMessageType.Text, true, default);
    }

    public static async Task<OperationMessage> ReceiveMessageAsync(this WebSocket socket)
    {
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(5000);
        var mem = new MemoryStream();
        ValueWebSocketReceiveResult response;
        do
        {
            var buffer = new byte[1024];
            response = await socket.ReceiveAsync(new Memory<byte>(buffer), cts.Token);
            mem.Write(buffer, 0, response.Count);
        } while (!response.EndOfMessage);
        Assert.Equal(WebSocketMessageType.Text, response.MessageType);
        mem.Position = 0;
        var message = await _serializer.ReadAsync<OperationMessage>(mem);
        return message;
    }

    public static async Task ReceiveCloseMessageAsync(this WebSocket socket)
    {
        using var cts = new CancellationTokenSource(5000);
        var drainBuffer = new byte[1024];
        ValueWebSocketReceiveResult response;
        do
        {
            response = await socket.ReceiveAsync(new Memory<byte>(drainBuffer), cts.Token);
        } while (!response.EndOfMessage);
        Assert.Equal(WebSocketMessageType.Close, response.MessageType);
    }
}
