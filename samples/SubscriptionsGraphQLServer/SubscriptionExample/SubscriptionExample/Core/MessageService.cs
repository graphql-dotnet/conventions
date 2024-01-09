using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace SubscriptionExample.Core
{
    public class MessageService
    {
        private readonly List<Message> _messages;
        private readonly ISubject<Message> _messageStream = new ReplaySubject<Message>(1);

        public MessageService()
        {
            _messages = new List<Message>();
        }

        public Message AddMessage(Message message)
        {
            _messages.Add(message);
            _messageStream.OnNext(message);
            return message;
        }

        public IEnumerable<Message> Messages => _messages;

        public IObservable<Message> ObservableMessages()
        {
            return _messageStream.AsObservable();
        }
    }
}
