using System;
using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;

namespace CustomerService.Business.MessageBroker
{
    public class Sender : ISender
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly string _queueName;
        private readonly string _exchangeName;

        public string Type { get; }

        public Sender(IConnectionFactory connectionFactory, string type, string queueName, string exchangeName)
        {
            Type = type;
            _queueName = queueName;
            _exchangeName = exchangeName;
            _connectionFactory = connectionFactory;
        }

        public void SendMessage(string message)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var model = connection.CreateModel())
                {
                    model.ExchangeDeclare(_exchangeName, ExchangeType.Direct, true);
                    model.QueueDeclare(_queueName, true, false, false, new ConcurrentDictionary<string, object>());
                    model.QueueBind(_queueName, _exchangeName, _queueName, null);

                    byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
                    model.BasicPublish(_exchangeName, _queueName, null, messageBodyBytes);
                }
            }
        }
    }
}
