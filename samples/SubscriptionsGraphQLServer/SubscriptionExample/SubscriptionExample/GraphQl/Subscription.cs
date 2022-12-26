using System;
using GraphQL.Conventions;
using SubscriptionExample.Core;

namespace SubscriptionExample.GraphQl;

public class Subscription
{
    public IObservable<Message> MessageUpdate([Inject] MessageService service) => service.ObservableMessages();
}
