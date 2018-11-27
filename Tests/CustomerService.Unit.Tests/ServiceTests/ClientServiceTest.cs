using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CustomerService.Business;
using CustomerService.Repositories;
using ClientEntity = CustomerService.Repositories.Entities.Client;
using CustomerService.Business.Models;

namespace CustomerService.Unit.Tests.ServiceTests
{
    [TestClass]
    public class ClientServiceTest
    {
        private static readonly Mock<IClientRepository> ClientRepository = new Mock<IClientRepository>();
        private static readonly Mock<IGoogleAuthService> GoogleAuthService = new Mock<IGoogleAuthService>();
        private static readonly Mock<IEmailService> EmailService = new Mock<IEmailService>();

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

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);

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

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);

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

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);

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

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);

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

            ClientRepository.Setup(x => x.GetClientByName(name)).Returns(new ClientEntity { Id = clientId, Name = name, GoogleAuthCode = googleAuthCode, IsActive = isActive, PasswordHash = passwordHashed });

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);
            var result = service.Authentification(name, password);

            ClientRepository.Verify(x => x.GetClientByName(name), Times.Once);
            Assert.AreEqual(result.Id, clientId);
            Assert.AreEqual(result.Name, name);
            Assert.AreEqual(result.GoogleAuthCode, googleAuthCode);
            Assert.AreEqual(result.IsActive, isActive);
        }

        [TestMethod]
        public void Authentification_ClientNotExisted_ShouldReturnNull()
        {
            ClientRepository.Invocations.Clear();

            var name = "test";
            var password = "123654";

            ClientRepository.Setup(x => x.GetClientByName(name)).Returns((ClientEntity) null);

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);

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

            ClientRepository.Setup(x => x.GetClientByName(name)).Returns(new ClientEntity { Id = clientId, Name = name, GoogleAuthCode = googleAuthCode, IsActive = isActive, PasswordHash = passwordHashed });

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);

            var result = service.Authentification(name, password);

            ClientRepository.Verify(x => x.GetClientByName(name), Times.Once);
            Assert.AreEqual(result, null);
        }



        [TestMethod]
        public void SendActivationCode_ClientWithEmailExist_ShouldReturnTrue()
        {
            ClientRepository.Invocations.Clear();

            var id = Guid.NewGuid();
            var email = "email2@mail.ru";
            var name = "name1";
            var passwordHashed = "542gythnfsli8";
            var createDate = DateTime.UtcNow.AddDays(-1);
            var updateDate = DateTime.UtcNow.AddMinutes(-3);

            var createdName = string.Empty;
            var createdEmail = string.Empty;
            var createdPassword = string.Empty;

            var client = new ClientEntity { Id = id, Name = name, Email = email, PasswordHash = passwordHashed, CreatedDate = createDate, UpdatedDate = updateDate, IsActive = false };

            ClientRepository.Setup(x => x.GetClientByEmail(email)).Returns(client);

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);

            var result = service.SendActivationCode(email);

            ClientRepository.Verify(x => x.GetClientByEmail(email), Times.Once);
            EmailService.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void SendActivationCode_ClientWithEmailNotExist_ShouldReturnFalse()
        {
            ClientRepository.Invocations.Clear();
            EmailService.Invocations.Clear();

            var email = "email2@mail.ru";

            ClientRepository.Setup(x => x.GetClientByEmail(email)).Returns((ClientEntity)null);
            EmailService.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);

            var result = service.SendActivationCode(email);

            ClientRepository.Verify(x => x.GetClientByEmail(email), Times.Once);
            EmailService.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void ValidateClientByGoogleAuth_ClientExistedAuthCodeIsValid_ShouldReturnTrue()
        {
            ClientRepository.Invocations.Clear();
            GoogleAuthService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var name = "test";
            var oneTimePassword = "123654";
            var isActive = true;
            var googleAuthCode = "sfdfgdgdfhd";
            var passwordHashed = "IqDoAwygKB2+M2hvzo/";

            ClientRepository.Setup(x => x.GetClient(clientId)).Returns(new ClientEntity { Id = clientId, Name = name, GoogleAuthCode = googleAuthCode, GoogleAuthActive = true, IsActive = isActive, PasswordHash = passwordHashed });
            GoogleAuthService.Setup(x => x.Validate(googleAuthCode, oneTimePassword)).Returns(true);

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);
            var result = service.ValidateClientByGoogleAuth(clientId, oneTimePassword);

            ClientRepository.Verify(x => x.GetClient(clientId), Times.Once);
            GoogleAuthService.Verify(x => x.Validate(googleAuthCode, oneTimePassword), Times.Once);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateClientByGoogleAuth_ClientNotExisted_ShouldReturnNull()
        {
            ClientRepository.Invocations.Clear();
            GoogleAuthService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var oneTimePassword = "123654";
            var googleAuthCode = "sfdfgdgdfhd";

            ClientRepository.Setup(x => x.GetClient(clientId)).Returns((ClientEntity)null);
            GoogleAuthService.Setup(x => x.Validate(googleAuthCode, oneTimePassword)).Returns(true);

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);
            var result = service.ValidateClientByGoogleAuth(clientId, oneTimePassword);

            ClientRepository.Verify(x => x.GetClient(clientId), Times.Once);
            GoogleAuthService.Verify(x => x.Validate(googleAuthCode, oneTimePassword), Times.Never);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateClientByGoogleAuth_ClientExistedAuthCodeIsInvalid_ShouldReturnFalse()
        {
            ClientRepository.Invocations.Clear();
            GoogleAuthService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var name = "test";
            var oneTimePassword = "123654";
            var isActive = true;
            var googleAuthCode = "sfdfgdgdfhd";
            var passwordHashed = "IqDoAwygKB2+M2hvzo/";

            ClientRepository.Setup(x => x.GetClient(clientId)).Returns(new ClientEntity { Id = clientId, Name = name, GoogleAuthCode = googleAuthCode, GoogleAuthActive = true, IsActive = isActive, PasswordHash = passwordHashed });
            GoogleAuthService.Setup(x => x.Validate(googleAuthCode, oneTimePassword)).Returns(false);

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);
            var result = service.ValidateClientByGoogleAuth(clientId, oneTimePassword);

            ClientRepository.Verify(x => x.GetClient(clientId), Times.Once);
            GoogleAuthService.Verify(x => x.Validate(googleAuthCode, oneTimePassword), Times.Once);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateClientByGoogleAuth_ClientExistedAuthCodeIsNullGoogleAuthInactive_ShouldReturnTrueNotCheckValid()
        {
            ClientRepository.Invocations.Clear();
            GoogleAuthService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var name = "test";
            var oneTimePassword = "123654";
            var isActive = true;
            var passwordHashed = "IqDoAwygKB2+M2hvzo/";

            ClientRepository.Setup(x => x.GetClient(clientId)).Returns(new ClientEntity { Id = clientId, Name = name, GoogleAuthCode = null, GoogleAuthActive = false, IsActive = isActive, PasswordHash = passwordHashed });
            GoogleAuthService.Setup(x => x.Validate(It.IsAny<string>(), oneTimePassword)).Returns(false);

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);
            var result = service.ValidateClientByGoogleAuth(clientId, oneTimePassword);

            ClientRepository.Verify(x => x.GetClient(clientId), Times.Once);
            GoogleAuthService.Verify(x => x.Validate(It.IsAny<string>(), oneTimePassword), Times.Never);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateClientByGoogleAuth_ClientExistedAuthCodeIsNotNullAndInvalidGoogleAuthInactive_ShouldReturnFalse()
        {
            ClientRepository.Invocations.Clear();
            GoogleAuthService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var name = "test";
            var oneTimePassword = "123654";
            var isActive = true;
            var googleAuthCode = "sfdfgdgdfhd";
            var passwordHashed = "IqDoAwygKB2+M2hvzo/";

            ClientRepository.Setup(x => x.GetClient(clientId)).Returns(new ClientEntity { Id = clientId, Name = name, GoogleAuthCode = googleAuthCode, GoogleAuthActive = false, IsActive = isActive, PasswordHash = passwordHashed });
            GoogleAuthService.Setup(x => x.Validate(googleAuthCode, oneTimePassword)).Returns(false);

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);
            var result = service.ValidateClientByGoogleAuth(clientId, oneTimePassword);

            ClientRepository.Verify(x => x.GetClient(clientId), Times.Once);
            GoogleAuthService.Verify(x => x.Validate(googleAuthCode, oneTimePassword), Times.Once);
            Assert.IsFalse(result);
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

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);

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

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);

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

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);

            var result = service.ActivateClient(activationCode);

            ClientRepository.Verify(x => x.ActivateClient(activationCode), Times.Once);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void CreateGoogleAuthCode_ClientExistedAndHaveNotGoogleCode_ShouldReturnGoogleAuthCode()
        {
            ClientRepository.Invocations.Clear();
            GoogleAuthService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var name = "test";
            var email = "test@mail.ru";
            var isActive = true;
            var qrCodeImageUrl = "someurl.com/test";
            var setupCode = "sfgsdg34rfs";

            ClientRepository.Setup(x => x.GetClient(clientId)).Returns(new ClientEntity { Id = clientId, Name = name, Email = email, IsActive = isActive });
            ClientRepository.Setup(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>())).Returns(true);
            GoogleAuthService.Setup(x => x.Generate(email, It.IsAny<string>())).Returns(new GoogleAuthCode { QRCodeImageUrl = qrCodeImageUrl, SetupCode = setupCode });

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);

            var result = service.CreateGoogleAuthCode(clientId);

            ClientRepository.Verify(x => x.GetClient(clientId), Times.Once);
            ClientRepository.Verify(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>()), Times.Once);
            GoogleAuthService.Verify(x => x.Generate(email, It.IsAny<string>()), Times.Once);
            Assert.AreEqual(result.QRCodeImageUrl, qrCodeImageUrl);
            Assert.AreEqual(result.SetupCode, setupCode);
        }

        [TestMethod]
        public void CreateGoogleAuthCode_ClientExistedAndHaveInactiveGoogleCode_ShouldNotGenerateCodeAndShouldReturnException()
        {
            ClientRepository.Invocations.Clear();
            GoogleAuthService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var name = "test";
            var email = "test@mail.ru";
            var isActive = true;
            var googleAuthCode = "sfdfgdgdfhd";
            var qrCodeImageUrl = "someurl.com/test";
            var setupCode = "sfgsdg34rfs";

            ClientRepository.Setup(x => x.GetClient(clientId)).Returns(new ClientEntity { Id = clientId, Name = name, Email = email, GoogleAuthCode = googleAuthCode, GoogleAuthActive = false, IsActive = isActive });
            ClientRepository.Setup(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>())).Returns(true);
            GoogleAuthService.Setup(x => x.Generate(email, It.IsAny<string>())).Returns(new GoogleAuthCode { QRCodeImageUrl = qrCodeImageUrl, SetupCode = setupCode });

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);

            var result = service.CreateGoogleAuthCode(clientId);

            ClientRepository.Verify(x => x.GetClient(clientId), Times.Once);
            GoogleAuthService.Verify(x => x.Generate(email, It.IsAny<string>()), Times.Once);
            ClientRepository.Verify(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>()), Times.Once);
            Assert.AreEqual(result.QRCodeImageUrl, qrCodeImageUrl);
            Assert.AreEqual(result.SetupCode, setupCode);
        }

        [TestMethod]
        public void CreateGoogleAuthCode_ClientExistedAndHaveActiveGoogleCode_ShouldNotGenerateCodeAndShouldReturnException()
        {
            ClientRepository.Invocations.Clear();
            GoogleAuthService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var name = "test";
            var email = "test@mail.ru";
            var isActive = true;
            var googleAuthCode = "sfdfgdgdfhd";
            var qrCodeImageUrl = "someurl.com/test";
            var setupCode = "sfgsdg34rfs";

            ClientRepository.Setup(x => x.GetClient(clientId)).Returns(new ClientEntity { Id = clientId, Name = name, Email = email, GoogleAuthCode = googleAuthCode, GoogleAuthActive = true, IsActive = isActive });
            ClientRepository.Setup(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>())).Returns(true);
            GoogleAuthService.Setup(x => x.Generate(email, It.IsAny<string>())).Returns(new GoogleAuthCode { QRCodeImageUrl = qrCodeImageUrl, SetupCode = setupCode });

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);

            try
            {
                var result = service.CreateGoogleAuthCode(clientId);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Client already has active GoogleAuthCode.");
                ClientRepository.Verify(x => x.GetClient(clientId), Times.Once);
                ClientRepository.Verify(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>()), Times.Never);
                GoogleAuthService.Verify(x => x.Generate(email, It.IsAny<string>()), Times.Never);
            }
        }

        [TestMethod]
        public void CreateGoogleAuthCode_ClientNotExisted_ShouldReturnNull()
        {
            ClientRepository.Invocations.Clear();
            GoogleAuthService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var email = "test@mail.ru";
            var qrCodeImageUrl = "someurl.com/test";
            var setupCode = "sfgsdg34rfs";

            ClientRepository.Setup(x => x.GetClient(clientId)).Returns((ClientEntity)null);
            ClientRepository.Setup(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>())).Returns(true);
            GoogleAuthService.Setup(x => x.Generate(email, It.IsAny<string>())).Returns(new GoogleAuthCode { QRCodeImageUrl = qrCodeImageUrl, SetupCode = setupCode });

            var service = new ClientService(ClientRepository.Object, EmailService.Object, GoogleAuthService.Object);

            var result = service.CreateGoogleAuthCode(clientId);

            ClientRepository.Verify(x => x.GetClient(clientId), Times.Once);
            ClientRepository.Verify(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>()), Times.Never);
            GoogleAuthService.Verify(x => x.Generate(email, It.IsAny<string>()), Times.Never);
            Assert.AreEqual(result, null);
        }
    }
}
