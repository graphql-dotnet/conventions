using GraphQL.Conventions;
using SubscriptionExample.Core;

namespace SubscriptionExample.GraphQl;

public class Query
{
    public IEnumerable<Message> Messages([Inject] MessageService service) => service.Messages;
}
