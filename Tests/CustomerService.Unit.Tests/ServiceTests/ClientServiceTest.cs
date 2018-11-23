using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CustomerService.Business;
using CustomerService.Repositories;
using ClientEntity = CustomerService.Repositories.Entities.Client;

namespace CustomerService.UnitTests.ServiceTests
{
    [TestClass]
    public class ClientServiceTest
    {
        private static readonly Mock<IClientRepository> ClientRepository = new Mock<IClientRepository>();

        [TestMethod]
        public void CheckNameAvailability_ClientWithNameExisted_ShouldReturnFalse()
        {
            ClientRepository.Invocations.Clear();

            var id = Guid.NewGuid();
            var email = "email2@mail.ru";
            var name = "name1";
            var passwordHashed = "542gythnfsli8";
            var createDate = DateTime.UtcNow.AddDays(-1);
            var updateDate = DateTime.UtcNow.AddMinutes(-3);

            var client = new ClientEntity { Id = id, Name = name, Email = email, PasswordHash = passwordHashed, CreatedDate = createDate, UpdatedDate = updateDate, IsActive = true };

            ClientRepository.Setup(x => x.GetClientByName(name)).Returns(client);

            var service = new ClientService(ClientRepository.Object);

            var result = service.CheckNameAvailability(name);

            ClientRepository.Verify(x => x.GetClientByName(name), Times.Once);
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void CheckNameAvailability_ClientWithNameNotExisted_ShouldReturnTrue()
        {
            ClientRepository.Invocations.Clear();

            var name = "name1";

            ClientRepository.Setup(x => x.GetClientByName(name)).Returns((ClientEntity) null);

            var service = new ClientService(ClientRepository.Object);

            var result = service.CheckNameAvailability(name);

            ClientRepository.Verify(x => x.GetClientByName(name), Times.Once);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void CheckEmailAvailability_ClientWithEmailExisted_ShouldReturnFalse()
        {
            ClientRepository.Invocations.Clear();

            var id = Guid.NewGuid();
            var email = "email2@mail.ru";
            var name = "name1";
            var passwordHashed = "542gythnfsli8";
            var createDate = DateTime.UtcNow.AddDays(-1);
            var updateDate = DateTime.UtcNow.AddMinutes(-3);

            var client = new ClientEntity { Id = id, Name = name, Email = email, PasswordHash = passwordHashed, CreatedDate = createDate, UpdatedDate = updateDate, IsActive = true };

            ClientRepository.Setup(x => x.GetClientByEmail(email)).Returns(client);

            var service = new ClientService(ClientRepository.Object);

            var result = service.CheckEmailAvailability(email);

            ClientRepository.Verify(x => x.GetClientByEmail(email), Times.Once);
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void CheckEmailAvailability_ClientWithEmailNotExisted_ShouldReturnTrue()
        {
            ClientRepository.Invocations.Clear();

            var email = "email2@mail.ru";

            ClientRepository.Setup(x => x.GetClientByEmail(email)).Returns((ClientEntity)null);

            var service = new ClientService(ClientRepository.Object);

            var result = service.CheckEmailAvailability(email);

            ClientRepository.Verify(x => x.GetClientByEmail(email), Times.Once);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void Authentification_ClientExistedValidPassword_ShouldReturnClient()
        {
            ClientRepository.Invocations.Clear();

            var clientId = Guid.NewGuid() ;
            var name = "test";
            var password = "123654";
            var isActive = true;
            var googleAuthCode = "sfdfgdgdfhd";
            var passwordHashed = "IqDoAwygKB2+M2hvzo/jCo192s+8POSDW8uO+QWhhiE=";

            ClientRepository.Setup(x => x.GetClientByName(name)).Returns(new ClientEntity { Id = clientId, Name = name, IsActive = isActive, PasswordHash = passwordHashed } );

            var service = new ClientService(ClientRepository.Object);

            var result = service.Authentification(name, password);

            ClientRepository.Verify(x => x.GetClientByName(name), Times.Once);
            Assert.AreEqual(result.Id, clientId);
            Assert.AreEqual(result.Name, name);
            Assert.AreEqual(result.IsActive, isActive);
        }

        [TestMethod]
        public void Authentification_ClientNotExisted_ShouldReturnNull()
        {
            ClientRepository.Invocations.Clear();

            var name = "test";
            var password = "123654";

            ClientRepository.Setup(x => x.GetClientByName(name)).Returns((ClientEntity) null);

            var service = new ClientService(ClientRepository.Object);

            var result = service.Authentification(name, password);

            ClientRepository.Verify(x => x.GetClientByName(name), Times.Once);
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void Authentification_ClientExistedInvalidPassword_ShouldReturnNull()
        {
            ClientRepository.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var name = "test";
            var password = "123654";
            var isActive = true;
            var googleAuthCode = "sfdfgdgdfhd";
            var passwordHashed = "IqDoAwygKB2+M2hvzo/";

            ClientRepository.Setup(x => x.GetClientByName(name)).Returns(new ClientEntity { Id = clientId, Name = name, IsActive = isActive, PasswordHash = passwordHashed });

            var service = new ClientService(ClientRepository.Object);

            var result = service.Authentification(name, password);

            ClientRepository.Verify(x => x.GetClientByName(name), Times.Once);
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void CreateClient_Success_ShouldCReateClientAndSendEmail()
        {
            ClientRepository.Invocations.Clear();

            var id = Guid.NewGuid();
            var email = "email2@mail.ru";
            var name = "name1";
            var password = "123654";
            var passwordHashed = "542gythnfsli8";
            var createDate = DateTime.UtcNow.AddDays(-1);
            var updateDate = DateTime.UtcNow.AddMinutes(-3);

            var createdName = string.Empty;
            var createdEmail = string.Empty;
            var createdPassword = string.Empty;

            var client = new ClientEntity { Id = id, Name = name, Email = email, PasswordHash = passwordHashed, CreatedDate = createDate, UpdatedDate = updateDate, IsActive = false };

            ClientRepository.Setup(x => x.CreateClient(email, name, It.IsAny<string>(), It.IsAny<string>())).Returns(client);

            var service = new ClientService(ClientRepository.Object);

            var result = service.CreateClient(email, name, password);

            ClientRepository.Verify(x => x.CreateClient(email, name, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(result.Email, email);
            Assert.AreEqual(result.Name, name);
            Assert.AreEqual(result.IsActive, false);
        }

        [TestMethod]
        public void CreateClient_ServiceReturnNull_ShouldReturnNullAndShouldNotSendEmail()
        {
            ClientRepository.Invocations.Clear();

            var email = "email2@mail.ru";
            var Name = "name1";
            var password = "123654";

            ClientRepository.Setup(x => x.CreateClient(email, Name, It.IsAny<string>(), It.IsAny<string>())).Returns((ClientEntity)null);

            var service = new ClientService(ClientRepository.Object);

            var result = service.CreateClient(email, Name, password);

            ClientRepository.Verify(x => x.CreateClient(email, Name, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void ActivateClient_Success_ShouldReturnClientResult()
        {
            ClientRepository.Invocations.Clear();

            var activationCode = "12345qwerty78906yuiopsdfg";

            ClientRepository.Setup(x => x.ActivateClient(activationCode)).Returns(true);

            var service = new ClientService(ClientRepository.Object);

            var result = service.ActivateClient(activationCode);

            ClientRepository.Verify(x => x.ActivateClient(activationCode), Times.Once);
            Assert.AreEqual(result, true);
        }
    }
}
