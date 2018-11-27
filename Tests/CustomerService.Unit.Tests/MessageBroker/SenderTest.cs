using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CustomerService.Business.MessageBroker;
using RabbitMQ.Client;

namespace CustomerService.Unit.Tests.MessageBrockerTests
{
    [TestClass]
    public class SenderTest
    {
        private static readonly Mock<IConnectionFactory> ConnectionFactory = new Mock<IConnectionFactory>();
        private static readonly Mock<IConnection> Connection = new Mock<IConnection>();
        private static readonly Mock<IModel> Model = new Mock<IModel>();

        [TestMethod]
        public void SendMessage_UseConnectionFactoryAndModel()
        {
            ConnectionFactory.ResetCalls();

            var message = "{ 'currency' : 'BTC' }";
            var queueName = "126_queue";
            var exchangeName = "exchange";
            var queueName1 = string.Empty;
            bool? durable = null;
            bool? exclusive = null;
            bool? autoDelete = null;
            var exchange = string.Empty;
            var routing = string.Empty;
            var body = Encoding.UTF8.GetBytes("");

            Model.Setup(x => x.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>()))
                .Callback<string, bool, bool, bool, IDictionary<string, object>>((queueParam, durableParam, exclusiveParam, autoDeleteParam, param) =>
                {
                    queueName1 = queueParam;
                    durable = durableParam;
                    exclusive = exclusiveParam;
                    autoDelete = autoDeleteParam;
                });

            Model.Setup(x => x.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null));
            Model.Setup(x => x.QueueBind(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null));
            Model.Setup(x => x.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IBasicProperties>(), It.IsAny<byte[]>()))
                .Callback<string, string, bool, IBasicProperties, byte[]>((exchangeParam, routingParam, mandatoryParam, propertyParam, bodyParam) =>
                {
                    exchange = exchangeParam;
                    routing = routingParam;
                    body = bodyParam;
                });

            Connection.Setup(x => x.CreateModel()).Returns(Model.Object);
            ConnectionFactory.Setup(x => x.CreateConnection()).Returns(Connection.Object);
            
            var sender = new Sender(ConnectionFactory.Object, "MakeTransfer", queueName, exchangeName);

            sender.SendMessage(message);

            ConnectionFactory.Verify(x => x.CreateConnection(), Times.Once);
            Connection.Verify(x => x.CreateModel(), Times.Once);
            Model.Verify(x => x.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null), Times.Once);
            Model.Verify(x => x.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(),It.IsAny<IDictionary<string, object>>()), Times.Once);
            Model.Verify(x => x.QueueBind(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Once);
            Model.Verify(x => x.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), null, It.IsAny<byte[]>()), Times.Once);

            Assert.AreEqual(queueName1, queueName);
            Assert.AreEqual(durable, true);
            Assert.AreEqual(exclusive, false);
            Assert.AreEqual(autoDelete, false);
            Assert.AreEqual(exchange, exchangeName);
            Assert.AreEqual(routing, queueName);
            Assert.AreEqual(Encoding.UTF8.GetString(body), message);
        }
    }
}
