using GraphQL.Conventions;
using SubscriptionExample.Core;

namespace SubscriptionExample.GraphQl;

public class Mutation
{
    public Message AddMessage([Inject] MessageService service, MessageInput message) => service.AddMessage(new Message
    {
        Author = message.Author,
        Text = message.Text
    });
}

[InputType]
public class MessageInput
{
    public string Author { get; set; }
    public string Text { get; set; }
}
