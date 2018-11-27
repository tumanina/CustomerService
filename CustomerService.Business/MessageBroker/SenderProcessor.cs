using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomerService.Business.MessageBroker
{
    public class SenderProcessor: ISenderProcessor
    {
        private readonly IEnumerable<ISender> _senders;

        public SenderProcessor(IEnumerable<ISender> senders)
        {
            _senders = senders;
        }
        
        public void SendMessage(string type, string message)
        {
            var sender = _senders.FirstOrDefault(t => t.Type == type);

            if (sender == null)
            {
                throw new Exception($"Sender for type '{type}' not found.");
            }

            sender.SendMessage(message);
        }
    }
}
