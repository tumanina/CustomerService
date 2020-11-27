using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CustomerService.Business.MessageBroker;
using RabbitMQ.Client;
using System;

namespace CustomerService.Unit.Tests.MessageBrockerTests
{
    [TestClass]
    public class SenderTest
    {
        private static readonly Mock<IConnectionFactory> _connectionFactoryMock = new Mock<IConnectionFactory>();
        private static readonly Mock<IConnection> _connectionMock = new Mock<IConnection>();
        private static readonly Mock<IModel> _modelMock = new Mock<IModel>();

        [TestMethod]
        public void SendMessage_UseConnectionFactoryAndModel()
        {
            _connectionFactoryMock.Invocations.Clear();

            var message = "{ 'currency' : 'BTC' }";
            var queueName = "126_queue";
            var exchangeName = "exchange";
            var queueName1 = string.Empty;
            bool? durable = null;
            bool? exclusive = null;
            bool? autoDelete = null;
            var exchange = string.Empty;
            var routing = string.Empty;
            var body = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(""));

            _modelMock.Setup(x => x.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>()))
                .Callback<string, bool, bool, bool, IDictionary<string, object>>((queueParam, durableParam, exclusiveParam, autoDeleteParam, param) =>
                {
                    queueName1 = queueParam;
                    durable = durableParam;
                    exclusive = exclusiveParam;
                    autoDelete = autoDeleteParam;
                });

            _modelMock.Setup(x => x.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null));
            _modelMock.Setup(x => x.QueueBind(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null));
            _modelMock.Setup(x => x.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IBasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>()))
                .Callback<string, string, bool, IBasicProperties, ReadOnlyMemory<byte>>((exchangeParam, routingParam, mandatoryParam, propertyParam, bodyParam) =>
                {
                    exchange = exchangeParam;
                    routing = routingParam;
                    body = bodyParam;
                });

            _connectionMock.Setup(x => x.CreateModel()).Returns(_modelMock.Object);
            _connectionFactoryMock.Setup(x => x.CreateConnection()).Returns(_connectionMock.Object);
            
            var sender = new Sender(_connectionFactoryMock.Object, "MakeTransfer", queueName, exchangeName);

            sender.SendMessage(message);

            _connectionFactoryMock.Verify(x => x.CreateConnection(), Times.Once);
            _connectionMock.Verify(x => x.CreateModel(), Times.Once);
            _modelMock.Verify(x => x.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null), Times.Once);
            _modelMock.Verify(x => x.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(),It.IsAny<IDictionary<string, object>>()), Times.Once);
            _modelMock.Verify(x => x.QueueBind(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null), Times.Once);
            _modelMock.Verify(x => x.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), null, It.IsAny<ReadOnlyMemory<byte>>()), Times.Once);

            Assert.AreEqual(queueName1, queueName);
            Assert.AreEqual(durable, true);
            Assert.AreEqual(exclusive, false);
            Assert.AreEqual(autoDelete, false);
            Assert.AreEqual(exchange, exchangeName);
            Assert.AreEqual(routing, queueName);
            Assert.AreEqual(Encoding.UTF8.GetString(body.ToArray()), message);
        }
    }
}
