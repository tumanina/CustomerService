using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CustomerService.Business.MessageBroker;

namespace CustomerService.Unit.Tests.MessageBrockerTests
{
    [TestClass]
    public class SenderProcessorTest
    {
        [TestMethod]
        public void SendMessage_SenderExist_ShouldUseCorrectSender()
        {
            var message = "{ 'currency' : 'BTC' }";
            var type = "MakeTransfer";

            var sendingMessage = "";

            var createAddressSender = new Mock<ISender>();
            createAddressSender.Setup(x => x.Type).Returns("CreateAddress");
            createAddressSender.Setup(x => x.SendMessage(message)).Callback((string messageParam) => { sendingMessage = messageParam; });

            var makeTransferSender = new Mock<ISender>();
            makeTransferSender.Setup(x => x.Type).Returns("MakeTransfer");
            makeTransferSender.Setup(x => x.SendMessage(message)).Callback((string messageParam) => { sendingMessage = messageParam; });

            var checkTransferSender = new Mock<ISender>();
            checkTransferSender.Setup(x => x.Type).Returns("CheckTransfer");
            checkTransferSender.Setup(x => x.SendMessage(message)).Callback((string messageParam) => { sendingMessage = messageParam; });

            var processor = new SenderProcessor(new List<ISender> { createAddressSender.Object, makeTransferSender.Object, checkTransferSender.Object });

            processor.SendMessage(type, message);

            createAddressSender.Verify(x => x.SendMessage(message), Times.Never);
            makeTransferSender.Verify(x => x.SendMessage(message), Times.Once);
            checkTransferSender.Verify(x => x.SendMessage(message), Times.Never);
            createAddressSender.Verify(x => x.Type, Times.Once);
            makeTransferSender.Verify(x => x.Type, Times.Once);
            checkTransferSender.Verify(x => x.Type, Times.Never);
            Assert.AreEqual(sendingMessage, message);
        }

        [TestMethod]
        public void SendMessage_SenderNotExisted_ThrowException()
        {
            var message = "{ 'currency' : 'BTC' }";
            var type = "CheckDeposit";

            var sendingMessage = "";

            var createAddressSender = new Mock<ISender>();
            createAddressSender.Setup(x => x.Type).Returns("CreateAddress");
            createAddressSender.Setup(x => x.SendMessage(message)).Callback((string messageParam) => { sendingMessage = messageParam; });

            var makeTransferSender = new Mock<ISender>();
            makeTransferSender.Setup(x => x.Type).Returns("MakeTransfer");
            makeTransferSender.Setup(x => x.SendMessage(message)).Callback((string messageParam) => { sendingMessage = messageParam; });

            var checkTransferSender = new Mock<ISender>();
            checkTransferSender.Setup(x => x.Type).Returns("CheckTransfer");
            checkTransferSender.Setup(x => x.SendMessage(message)).Callback((string messageParam) => { sendingMessage = messageParam; });

            var processor = new SenderProcessor(new List<ISender> { createAddressSender.Object, makeTransferSender.Object, checkTransferSender.Object });

            try
            {
                processor.SendMessage(type, message);

                Assert.Fail();
            }
            catch (Exception ex)
            {
                createAddressSender.Verify(x => x.SendMessage(message), Times.Never);
                makeTransferSender.Verify(x => x.SendMessage(message), Times.Never);
                checkTransferSender.Verify(x => x.SendMessage(message), Times.Never);
                createAddressSender.Verify(x => x.Type, Times.Once);
                makeTransferSender.Verify(x => x.Type, Times.Once);
                checkTransferSender.Verify(x => x.Type, Times.Once);
                Assert.AreEqual(sendingMessage, string.Empty);
                Assert.AreEqual(ex.Message, $"Sender for type '{type}' not found.");
            }
        }
    }
}
