using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CustomerService.Business;
using ClientEntity = CustomerService.Repositories.Entities.Client;
using CustomerService.Business.Models;
using CustomerService.Repositories.Interfaces;

namespace CustomerService.Unit.Tests.ServiceTests
{
    [TestClass]
    public class ClientServiceTest
    {
        private static readonly Mock<IClientRepository> _clientRepositoryMock = new Mock<IClientRepository>();
        private static readonly Mock<IGoogleAuthService> _googleAuthServiceMock = new Mock<IGoogleAuthService>();
        private static readonly Mock<IEmailService> _emailServiceMock = new Mock<IEmailService>();

        [TestCleanup]
        public void TestCleanUp()
        {
            _clientRepositoryMock.Invocations.Clear();
            _googleAuthServiceMock.Invocations.Clear();
            _emailServiceMock.Invocations.Clear();
        }

        [TestMethod]
        public void CheckNameAvailability_ClientWithNameExisted_ShouldReturnFalse()
        {
            var id = Guid.NewGuid();
            var email = "email2@mail.ru";
            var name = "name1";
            var passwordHashed = "542gythnfsli8";
            var createDate = DateTime.UtcNow.AddDays(-1);
            var updateDate = DateTime.UtcNow.AddMinutes(-3);

            var client = new ClientEntity { Id = id, Name = name, Email = email, PasswordHash = passwordHashed, CreatedDate = createDate, UpdatedDate = updateDate, IsActive = true };

            _clientRepositoryMock.Setup(x => x.GetClientByName(name)).Returns(client);

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);

            var result = service.CheckNameAvailability(name);

            _clientRepositoryMock.Verify(x => x.GetClientByName(name), Times.Once);
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void CheckNameAvailability_ClientWithNameNotExisted_ShouldReturnTrue()
        {
            var name = "name1";

            _clientRepositoryMock.Setup(x => x.GetClientByName(name)).Returns((ClientEntity) null);

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);

            var result = service.CheckNameAvailability(name);

            _clientRepositoryMock.Verify(x => x.GetClientByName(name), Times.Once);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void CheckEmailAvailability_ClientWithEmailExisted_ShouldReturnFalse()
        {
            var id = Guid.NewGuid();
            var email = "email2@mail.ru";
            var name = "name1";
            var passwordHashed = "542gythnfsli8";
            var createDate = DateTime.UtcNow.AddDays(-1);
            var updateDate = DateTime.UtcNow.AddMinutes(-3);

            var client = new ClientEntity { Id = id, Name = name, Email = email, PasswordHash = passwordHashed, CreatedDate = createDate, UpdatedDate = updateDate, IsActive = true };

            _clientRepositoryMock.Setup(x => x.GetClientByEmail(email)).Returns(client);

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);

            var result = service.CheckEmailAvailability(email);

            _clientRepositoryMock.Verify(x => x.GetClientByEmail(email), Times.Once);
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void CheckEmailAvailability_ClientWithEmailNotExisted_ShouldReturnTrue()
        {
            var email = "email2@mail.ru";

            _clientRepositoryMock.Setup(x => x.GetClientByEmail(email)).Returns((ClientEntity)null);

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);

            var result = service.CheckEmailAvailability(email);

            _clientRepositoryMock.Verify(x => x.GetClientByEmail(email), Times.Once);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void Authentification_ClientExistedValidPassword_ShouldReturnClient()
        {
            var clientId = Guid.NewGuid() ;
            var name = "test";
            var password = "123654";
            var isActive = true;
            var googleAuthCode = "sfdfgdgdfhd";
            var passwordHashed = "IqDoAwygKB2+M2hvzo/jCo192s+8POSDW8uO+QWhhiE=";

            _clientRepositoryMock.Setup(x => x.GetClientByName(name)).Returns(new ClientEntity { Id = clientId, Name = name, GoogleAuthCode = googleAuthCode, IsActive = isActive, PasswordHash = passwordHashed });

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);
            var result = service.Authentification(name, password);

            _clientRepositoryMock.Verify(x => x.GetClientByName(name), Times.Once);
            Assert.AreEqual(result.Id, clientId);
            Assert.AreEqual(result.Name, name);
            Assert.AreEqual(result.GoogleAuthCode, googleAuthCode);
            Assert.AreEqual(result.IsActive, isActive);
        }

        [TestMethod]
        public void Authentification_ClientNotExisted_ShouldReturnNull()
        {
            var name = "test";
            var password = "123654";

            _clientRepositoryMock.Setup(x => x.GetClientByName(name)).Returns((ClientEntity) null);

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);

            var result = service.Authentification(name, password);

            _clientRepositoryMock.Verify(x => x.GetClientByName(name), Times.Once);
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void Authentification_ClientExistedInvalidPassword_ShouldReturnNull()
        {
            var clientId = Guid.NewGuid();
            var name = "test";
            var password = "123654";
            var isActive = true;
            var googleAuthCode = "sfdfgdgdfhd";
            var passwordHashed = "IqDoAwygKB2+M2hvzo/";

            _clientRepositoryMock.Setup(x => x.GetClientByName(name)).Returns(new ClientEntity { Id = clientId, Name = name, GoogleAuthCode = googleAuthCode, IsActive = isActive, PasswordHash = passwordHashed });

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);

            var result = service.Authentification(name, password);

            _clientRepositoryMock.Verify(x => x.GetClientByName(name), Times.Once);
            Assert.AreEqual(result, null);
        }



        [TestMethod]
        public void SendActivationCode_ClientWithEmailExist_ShouldReturnTrue()
        {
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

            _clientRepositoryMock.Setup(x => x.GetClientByEmail(email)).Returns(client);

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);

            var result = service.SendActivationCode(email);

            _clientRepositoryMock.Verify(x => x.GetClientByEmail(email), Times.Once);
            _emailServiceMock.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void SendActivationCode_ClientWithEmailNotExist_ShouldReturnFalse()
        {
            var email = "email2@mail.ru";

            _clientRepositoryMock.Setup(x => x.GetClientByEmail(email)).Returns((ClientEntity)null);
            _emailServiceMock.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);

            var result = service.SendActivationCode(email);

            _clientRepositoryMock.Verify(x => x.GetClientByEmail(email), Times.Once);
            _emailServiceMock.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void ValidateClientByGoogleAuth_ClientExistedAuthCodeIsValid_ShouldReturnTrue()
        {
            var clientId = Guid.NewGuid();
            var name = "test";
            var oneTimePassword = "123654";
            var isActive = true;
            var googleAuthCode = "sfdfgdgdfhd";
            var passwordHashed = "IqDoAwygKB2+M2hvzo/";

            _clientRepositoryMock.Setup(x => x.GetClient(clientId)).Returns(new ClientEntity { Id = clientId, Name = name, GoogleAuthCode = googleAuthCode, GoogleAuthActive = true, IsActive = isActive, PasswordHash = passwordHashed });
            _googleAuthServiceMock.Setup(x => x.Validate(googleAuthCode, oneTimePassword)).Returns(true);

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);
            var result = service.ValidateClientByGoogleAuth(clientId, oneTimePassword);

            _clientRepositoryMock.Verify(x => x.GetClient(clientId), Times.Once);
            _googleAuthServiceMock.Verify(x => x.Validate(googleAuthCode, oneTimePassword), Times.Once);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateClientByGoogleAuth_ClientNotExisted_ShouldReturnNull()
        {
            var clientId = Guid.NewGuid();
            var oneTimePassword = "123654";
            var googleAuthCode = "sfdfgdgdfhd";

            _clientRepositoryMock.Setup(x => x.GetClient(clientId)).Returns((ClientEntity)null);
            _googleAuthServiceMock.Setup(x => x.Validate(googleAuthCode, oneTimePassword)).Returns(true);

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);
            var result = service.ValidateClientByGoogleAuth(clientId, oneTimePassword);

            _clientRepositoryMock.Verify(x => x.GetClient(clientId), Times.Once);
            _googleAuthServiceMock.Verify(x => x.Validate(googleAuthCode, oneTimePassword), Times.Never);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateClientByGoogleAuth_ClientExistedAuthCodeIsInvalid_ShouldReturnFalse()
        {
            var clientId = Guid.NewGuid();
            var name = "test";
            var oneTimePassword = "123654";
            var isActive = true;
            var googleAuthCode = "sfdfgdgdfhd";
            var passwordHashed = "IqDoAwygKB2+M2hvzo/";

            _clientRepositoryMock.Setup(x => x.GetClient(clientId)).Returns(new ClientEntity { Id = clientId, Name = name, GoogleAuthCode = googleAuthCode, GoogleAuthActive = true, IsActive = isActive, PasswordHash = passwordHashed });
            _googleAuthServiceMock.Setup(x => x.Validate(googleAuthCode, oneTimePassword)).Returns(false);

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);
            var result = service.ValidateClientByGoogleAuth(clientId, oneTimePassword);

            _clientRepositoryMock.Verify(x => x.GetClient(clientId), Times.Once);
            _googleAuthServiceMock.Verify(x => x.Validate(googleAuthCode, oneTimePassword), Times.Once);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateClientByGoogleAuth_ClientExistedAuthCodeIsNullGoogleAuthInactive_ShouldReturnTrueNotCheckValid()
        {
            var clientId = Guid.NewGuid();
            var name = "test";
            var oneTimePassword = "123654";
            var isActive = true;
            var passwordHashed = "IqDoAwygKB2+M2hvzo/";

            _clientRepositoryMock.Setup(x => x.GetClient(clientId)).Returns(new ClientEntity { Id = clientId, Name = name, GoogleAuthCode = null, GoogleAuthActive = false, IsActive = isActive, PasswordHash = passwordHashed });
            _googleAuthServiceMock.Setup(x => x.Validate(It.IsAny<string>(), oneTimePassword)).Returns(false);

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);
            var result = service.ValidateClientByGoogleAuth(clientId, oneTimePassword);

            _clientRepositoryMock.Verify(x => x.GetClient(clientId), Times.Once);
            _googleAuthServiceMock.Verify(x => x.Validate(It.IsAny<string>(), oneTimePassword), Times.Never);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateClientByGoogleAuth_ClientExistedAuthCodeIsNotNullAndInvalidGoogleAuthInactive_ShouldReturnFalse()
        {
            var clientId = Guid.NewGuid();
            var name = "test";
            var oneTimePassword = "123654";
            var isActive = true;
            var googleAuthCode = "sfdfgdgdfhd";
            var passwordHashed = "IqDoAwygKB2+M2hvzo/";

            _clientRepositoryMock.Setup(x => x.GetClient(clientId)).Returns(new ClientEntity { Id = clientId, Name = name, GoogleAuthCode = googleAuthCode, GoogleAuthActive = false, IsActive = isActive, PasswordHash = passwordHashed });
            _googleAuthServiceMock.Setup(x => x.Validate(googleAuthCode, oneTimePassword)).Returns(false);

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);
            var result = service.ValidateClientByGoogleAuth(clientId, oneTimePassword);

            _clientRepositoryMock.Verify(x => x.GetClient(clientId), Times.Once);
            _googleAuthServiceMock.Verify(x => x.Validate(googleAuthCode, oneTimePassword), Times.Once);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CreateClient_Success_ShouldCReateClientAndSendEmail()
        {
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

            _clientRepositoryMock.Setup(x => x.CreateClient(email, name, It.IsAny<string>(), It.IsAny<string>())).Returns(client);

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);

            var result = service.CreateClient(email, name, password);

            _clientRepositoryMock.Verify(x => x.CreateClient(email, name, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(result.Email, email);
            Assert.AreEqual(result.Name, name);
            Assert.AreEqual(result.IsActive, false);
        }

        [TestMethod]
        public void CreateClient_ServiceReturnNull_ShouldReturnNullAndShouldNotSendEmail()
        {
            var email = "email2@mail.ru";
            var Name = "name1";
            var password = "123654";

            _clientRepositoryMock.Setup(x => x.CreateClient(email, Name, It.IsAny<string>(), It.IsAny<string>())).Returns((ClientEntity)null);

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);

            var result = service.CreateClient(email, Name, password);

            _clientRepositoryMock.Verify(x => x.CreateClient(email, Name, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void ActivateClient_Success_ShouldReturnClientResult()
        {
            var activationCode = "12345qwerty78906yuiopsdfg";

            _clientRepositoryMock.Setup(x => x.ActivateClient(activationCode)).Returns(true);

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);

            var result = service.ActivateClient(activationCode);

            _clientRepositoryMock.Verify(x => x.ActivateClient(activationCode), Times.Once);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void CreateGoogleAuthCode_ClientExistedAndHaveNotGoogleCode_ShouldReturnGoogleAuthCode()
        {
            var clientId = Guid.NewGuid();
            var name = "test";
            var email = "test@mail.ru";
            var isActive = true;
            var qrCodeImageUrl = "someurl.com/test";
            var setupCode = "sfgsdg34rfs";

            _clientRepositoryMock.Setup(x => x.GetClient(clientId)).Returns(new ClientEntity { Id = clientId, Name = name, Email = email, IsActive = isActive });
            _clientRepositoryMock.Setup(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>())).Returns(true);
            _googleAuthServiceMock.Setup(x => x.Generate(email, It.IsAny<string>())).Returns(new GoogleAuthCode { QRCodeImageUrl = qrCodeImageUrl, SetupCode = setupCode });

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);

            var result = service.CreateGoogleAuthCode(clientId);

            _clientRepositoryMock.Verify(x => x.GetClient(clientId), Times.Once);
            _clientRepositoryMock.Verify(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>()), Times.Once);
            _googleAuthServiceMock.Verify(x => x.Generate(email, It.IsAny<string>()), Times.Once);
            Assert.AreEqual(result.QRCodeImageUrl, qrCodeImageUrl);
            Assert.AreEqual(result.SetupCode, setupCode);
        }

        [TestMethod]
        public void CreateGoogleAuthCode_ClientExistedAndHaveInactiveGoogleCode_ShouldNotGenerateCodeAndShouldReturnException()
        {
            var clientId = Guid.NewGuid();
            var name = "test";
            var email = "test@mail.ru";
            var isActive = true;
            var googleAuthCode = "sfdfgdgdfhd";
            var qrCodeImageUrl = "someurl.com/test";
            var setupCode = "sfgsdg34rfs";

            _clientRepositoryMock.Setup(x => x.GetClient(clientId)).Returns(new ClientEntity { Id = clientId, Name = name, Email = email, GoogleAuthCode = googleAuthCode, GoogleAuthActive = false, IsActive = isActive });
            _clientRepositoryMock.Setup(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>())).Returns(true);
            _googleAuthServiceMock.Setup(x => x.Generate(email, It.IsAny<string>())).Returns(new GoogleAuthCode { QRCodeImageUrl = qrCodeImageUrl, SetupCode = setupCode });

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);

            var result = service.CreateGoogleAuthCode(clientId);

            _clientRepositoryMock.Verify(x => x.GetClient(clientId), Times.Once);
            _googleAuthServiceMock.Verify(x => x.Generate(email, It.IsAny<string>()), Times.Once);
            _clientRepositoryMock.Verify(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>()), Times.Once);
            Assert.AreEqual(result.QRCodeImageUrl, qrCodeImageUrl);
            Assert.AreEqual(result.SetupCode, setupCode);
        }

        [TestMethod]
        public void CreateGoogleAuthCode_ClientExistedAndHaveActiveGoogleCode_ShouldNotGenerateCodeAndShouldReturnException()
        {
            var clientId = Guid.NewGuid();
            var name = "test";
            var email = "test@mail.ru";
            var isActive = true;
            var googleAuthCode = "sfdfgdgdfhd";
            var qrCodeImageUrl = "someurl.com/test";
            var setupCode = "sfgsdg34rfs";

            _clientRepositoryMock.Setup(x => x.GetClient(clientId)).Returns(new ClientEntity { Id = clientId, Name = name, Email = email, GoogleAuthCode = googleAuthCode, GoogleAuthActive = true, IsActive = isActive });
            _clientRepositoryMock.Setup(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>())).Returns(true);
            _googleAuthServiceMock.Setup(x => x.Generate(email, It.IsAny<string>())).Returns(new GoogleAuthCode { QRCodeImageUrl = qrCodeImageUrl, SetupCode = setupCode });

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);

            try
            {
                var result = service.CreateGoogleAuthCode(clientId);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Client already has active GoogleAuthCode.");
                _clientRepositoryMock.Verify(x => x.GetClient(clientId), Times.Once);
                _clientRepositoryMock.Verify(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>()), Times.Never);
                _googleAuthServiceMock.Verify(x => x.Generate(email, It.IsAny<string>()), Times.Never);
            }
        }

        [TestMethod]
        public void CreateGoogleAuthCode_ClientNotExisted_ShouldReturnNull()
        {
            var clientId = Guid.NewGuid();
            var email = "test@mail.ru";
            var qrCodeImageUrl = "someurl.com/test";
            var setupCode = "sfgsdg34rfs";

            _clientRepositoryMock.Setup(x => x.GetClient(clientId)).Returns((ClientEntity)null);
            _clientRepositoryMock.Setup(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>())).Returns(true);
            _googleAuthServiceMock.Setup(x => x.Generate(email, It.IsAny<string>())).Returns(new GoogleAuthCode { QRCodeImageUrl = qrCodeImageUrl, SetupCode = setupCode });

            var service = new ClientService(_clientRepositoryMock.Object, _emailServiceMock.Object, _googleAuthServiceMock.Object);

            var result = service.CreateGoogleAuthCode(clientId);

            _clientRepositoryMock.Verify(x => x.GetClient(clientId), Times.Once);
            _clientRepositoryMock.Verify(x => x.UpdateGoogleAuthCode(clientId, It.IsAny<string>()), Times.Never);
            _googleAuthServiceMock.Verify(x => x.Generate(email, It.IsAny<string>()), Times.Never);
            Assert.AreEqual(result, null);
        }
    }
}
