using GraphQL.Conventions;
using SubscriptionExample.Core;
using System;

namespace SubscriptionExample.GraphQl
{
    public class Subscription
    {
        public IObservable<Message> MessageUpdate([Inject] MessageService service) => service.ObservableMessages();
    }
}