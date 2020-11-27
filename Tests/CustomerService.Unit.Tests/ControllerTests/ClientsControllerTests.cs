using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CustomerService.Api.Areas.V1.Controllers;
using CustomerService.Api.Areas.V1.Models;
using CustomerService.Business;
using Client = CustomerService.Business.Models.Client;

namespace CustomerService.Unit.Tests.ControllerTests
{
    [TestClass]
    public class ClientsControllerTests
    {
        private static readonly Mock<IClientService> _clientServiceMock = new Mock<IClientService>();
        private static readonly Mock<ILogger<ClientsController>> _loggerMock = new Mock<ILogger<ClientsController>>();

        [TestCleanup]
        public void TestCleanUp()
        {
            _clientServiceMock.Invocations.Clear();
        }

        [TestMethod]
        public void CheckNameAvailability_NameAvailable_ReturnOkWithTrue()
        {
            var name = "ivanov";

            _clientServiceMock.Setup(x => x.CheckNameAvailability(name)).Returns(true);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object);

            var actionResult = controller.CheckAvailability(name);
            var result = actionResult as OkObjectResult;

            _clientServiceMock.Verify(x => x.CheckNameAvailability(name), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual(result.Value, true);
        }

        [TestMethod]
        public void CheckNameAvailability_NameNotAvailable_ReturnOkWithFalse()
        {
            var name = "ivanov";

            _clientServiceMock.Setup(x => x.CheckNameAvailability(name)).Returns(false);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object);

            var actionResult = controller.CheckAvailability(name);
            var result = actionResult as OkObjectResult;

            _clientServiceMock.Verify(x => x.CheckNameAvailability(name), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual(result.Value, false);
        }

        [TestMethod]
        public void CheckNameAvailability_RequestIsEmpty_ReturnBadRequest()
        {
            var name = "ivanov";

            _clientServiceMock.Setup(x => x.CheckNameAvailability(name)).Returns(true);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object);

            var actionResult = controller.CheckAvailability(string.Empty);
            var result = actionResult as OkObjectResult;
            var errorResult = actionResult as BadRequestResult;

            _clientServiceMock.Verify(x => x.CheckNameAvailability(name), Times.Never);
            Assert.AreEqual(result, null);
            Assert.IsTrue(errorResult != null);
        }

        [TestMethod]
        public void CheckNameAvailability_ServiceReturnException_ReturnServerError()
        {
            var name = "ivanov";

            var exceptionMessage = "some exception message";
            _clientServiceMock.Setup(x => x.CheckNameAvailability(name)).Throws(new Exception(exceptionMessage));

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object);

            var actionResult = controller.CheckAvailability(name);
            var result = actionResult as OkObjectResult;
            var result1 = actionResult as ObjectResult;

            _clientServiceMock.Verify(x => x.CheckNameAvailability(name), Times.Once);
            Assert.IsTrue(result == null);
            Assert.IsTrue(result1 != null);
            Assert.AreEqual(result1.StatusCode, 500);
            Assert.AreEqual(result1.Value, exceptionMessage);
        }

        [TestMethod]
        public void CheckEmailAvailability_EmailAvailable_ReturnOkWithTrue()
        {
            var email = "ivanov@mail.ru";

            _clientServiceMock.Setup(x => x.CheckEmailAvailability(email)).Returns(true);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object);

            var actionResult = controller.CheckAvailability(string.Empty, email);
            var result = actionResult as OkObjectResult;

            _clientServiceMock.Verify(x => x.CheckEmailAvailability(email), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
        }

        [TestMethod]
        public void CheckEmailAvailability_EmailNotAvailable_ReturnOkWithFalse()
        {
            var email = "ivanov@mail.ru";

            _clientServiceMock.Setup(x => x.CheckEmailAvailability(email)).Returns(true);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object);

            var actionResult = controller.CheckAvailability(string.Empty, email);
            var result = actionResult as OkObjectResult;

            _clientServiceMock.Verify(x => x.CheckEmailAvailability(email), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
        }

        [TestMethod]
        public void CheckEmailAvailability_RequestIsEmpty_ReturnBadRequest()
        {
            var email = "ivanov@mail.ru";

            _clientServiceMock.Setup(x => x.CheckEmailAvailability(email)).Returns(true);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object);

            var actionResult = controller.CheckAvailability(string.Empty);
            var result = actionResult as OkObjectResult;
            var errorResult = actionResult as BadRequestResult;

            _clientServiceMock.Verify(x => x.CheckEmailAvailability(email), Times.Never);
            Assert.AreEqual(result, null);
            Assert.IsTrue(errorResult != null);
        }

        [TestMethod]
        public void CheckEmailAvailability_ServiceReturnException_ReturnServerError()
        {
            var email = "ivanov@mail.ru";

            var exceptionMessage = "some exception message";
            _clientServiceMock.Setup(x => x.CheckEmailAvailability(email)).Throws(new Exception(exceptionMessage));

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object);

            var actionResult = controller.CheckAvailability(string.Empty, email);
            var result = actionResult as OkObjectResult;
            var result1 = actionResult as ObjectResult;

            _clientServiceMock.Verify(x => x.CheckEmailAvailability(email), Times.Once);
            Assert.IsTrue(result == null);
            Assert.IsTrue(result1 != null);
            Assert.AreEqual(result1.StatusCode, 500);
            Assert.AreEqual(result1.Value, exceptionMessage);
        }

        [TestMethod]
        public void CreateClient_Success_ReturnCreatedAndCorrect()
        {
            var id = Guid.NewGuid();
            var email = "email2@mail.ru";
            var name = "name1";
            var password = "Sshfgjk123654";
            var passwordHashed = "542gythnfsli8";
            var createDate = DateTime.UtcNow.AddDays(-1);
            var updateDate = DateTime.UtcNow.AddMinutes(-3);

            var createdName = string.Empty;
            var createdEmail = string.Empty;
            var createdPassword = string.Empty;

            var Client = new Client { Id = id, Name = name, Email = email, PasswordHash = passwordHashed, CreatedDate = createDate, UpdatedDate = updateDate, IsActive = false };

            _clientServiceMock.Setup(x => x.CreateClient(email, name, password)).Returns(Client)
            .Callback((string emailParam, string nameParam, string passwordParam) => { createdEmail = emailParam; createdName = nameParam; createdPassword = passwordParam; });

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/clients";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(new CreateClientRequest
            {
                Email = email,
                Name = name,
                Password = password
            });

            var result = actionResult as CreatedResult;

            _clientServiceMock.Verify(x => x.CreateClient(email, name, password), Times.Once);
            Assert.AreEqual(result.StatusCode, 201);
            Assert.AreEqual(createdEmail, email);
            Assert.AreEqual(createdName, name);
            Assert.AreEqual(createdPassword, password);
        }

        [TestMethod]
        public void CreateClient_RequestIsEmpty_ReturnBadRequest()
        {
            var id = Guid.NewGuid();
            var email = "email2@mail.ru";
            var name = "name1";
            var password = "Sshfgjk123654";
            var passwordHashed = "542gythnfsli8";
            var createDate = DateTime.UtcNow.AddDays(-1);
            var updateDate = DateTime.UtcNow.AddMinutes(-3);

            var createdName = string.Empty;
            var createdEmail = string.Empty;
            var createdPassword = string.Empty;

            var Client = new Client { Id = id, Name = name, Email = email, PasswordHash = passwordHashed, CreatedDate = createDate, UpdatedDate = updateDate, IsActive = false };

            _clientServiceMock.Setup(x => x.CreateClient(email, name, password)).Returns(Client)
            .Callback((string emailParam, string nameParam, string passwordParam) => { createdEmail = emailParam; createdName = nameParam; createdPassword = passwordParam; });

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/Clients";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(null);

            var result = actionResult as CreatedResult;
            var result1 = actionResult as BadRequestObjectResult;

            _clientServiceMock.Verify(x => x.CreateClient(email, name, password), Times.Never);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 400);
        }

        public void CreateClient_ClientNameIsEmpty_ReturnBadRequest()
        {
            var id = Guid.NewGuid();
            var email = "email2@mail.ru";
            var name = "name1";
            var password = "Sshfgjk123654";
            var passwordHashed = "542gythnfsli8";
            var createDate = DateTime.UtcNow.AddDays(-1);
            var updateDate = DateTime.UtcNow.AddMinutes(-3);

            var createdName = string.Empty;
            var createdEmail = string.Empty;
            var createdPassword = string.Empty;

            var Client = new Client { Id = id, Name = name, Email = email, PasswordHash = passwordHashed, CreatedDate = createDate, UpdatedDate = updateDate, IsActive = false };

            _clientServiceMock.Setup(x => x.CreateClient(email, name, password)).Returns(Client);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/clients";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(new CreateClientRequest
            {
                Email = email,
                Password = password
            });

            var result = actionResult as CreatedResult;
            var result1 = actionResult as BadRequestObjectResult;

            _clientServiceMock.Verify(x => x.CreateClient(email, name, password), Times.Never);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 400);
        }

        public void CreateClient_ClientNameTooLong_ReturnBadRequest()
        {
            var id = Guid.NewGuid();
            var email = "email2@mail.ru";
            var name = "name1";
            var password = "Sshfgjk123654";
            var passwordHashed = "542gythnfsli8";
            var createDate = DateTime.UtcNow.AddDays(-1);
            var updateDate = DateTime.UtcNow.AddMinutes(-3);

            var createdName = string.Empty;
            var createdEmail = string.Empty;
            var createdPassword = string.Empty;

            var Client = new Client { Id = id, Name = name, Email = email, PasswordHash = passwordHashed, CreatedDate = createDate, UpdatedDate = updateDate, IsActive = false };

            _clientServiceMock.Setup(x => x.CreateClient(email, name, password)).Returns(Client);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/clients";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(new CreateClientRequest
            {
                Email = email,
                Name = name,
                Password = password
            });

            var result = actionResult as CreatedResult;
            var result1 = actionResult as BadRequestObjectResult;

            _clientServiceMock.Verify(x => x.CreateClient(email, name, password), Times.Never);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 400);
        }

        public void CreateClient_PasswordIsEmpty_ReturnBadRequest()
        {
            var id = Guid.NewGuid();
            var email = "email2@mail.ru";
            var name = "name1";
            var password = "Sshfgjk123654";
            var passwordHashed = "542gythnfsli8";
            var createDate = DateTime.UtcNow.AddDays(-1);
            var updateDate = DateTime.UtcNow.AddMinutes(-3);

            var createdName = string.Empty;
            var createdEmail = string.Empty;
            var createdPassword = string.Empty;

            var Client = new Client { Id = id, Name = name, Email = email, PasswordHash = passwordHashed, CreatedDate = createDate, UpdatedDate = updateDate, IsActive = false };

            _clientServiceMock.Setup(x => x.CreateClient(email, name, password)).Returns(Client);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/clients";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(new CreateClientRequest
            {
                Email = email,
                Name = name
            });

            var result = actionResult as CreatedResult;
            var result1 = actionResult as BadRequestObjectResult;

            _clientServiceMock.Verify(x => x.CreateClient(email, name, password), Times.Never);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 400);
        }

        public void CreateClient_EmailIsEmpty_ReturnBadRequest()
        {
            var id = Guid.NewGuid();
            var email = "email2@mail.ru";
            var name = "name1";
            var password = "Sshfgjk123654";
            var passwordHashed = "542gythnfsli8";
            var createDate = DateTime.UtcNow.AddDays(-1);
            var updateDate = DateTime.UtcNow.AddMinutes(-3);

            var createdName = string.Empty;
            var createdEmail = string.Empty;
            var createdPassword = string.Empty;

            var Client = new Client { Id = id, Name = name, Email = email, PasswordHash = passwordHashed, CreatedDate = createDate, UpdatedDate = updateDate, IsActive = false };

            _clientServiceMock.Setup(x => x.CreateClient(email, name, password)).Returns(Client);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/clients";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(new CreateClientRequest
            {
                Name = name,
                Password = password
            });

            var result = actionResult as CreatedResult;
            var result1 = actionResult as BadRequestObjectResult;

            _clientServiceMock.Verify(x => x.CreateClient(email, name, password), Times.Never);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 400);
        }

        public void CreateClient_EmailHasInvalidFormat_ReturnBadRequest()
        {
            var id = Guid.NewGuid();
            var email = "email2";
            var name = "name1";
            var password = "Sshfgjk123654";
            var passwordHashed = "542gythnfsli8";
            var createDate = DateTime.UtcNow.AddDays(-1);
            var updateDate = DateTime.UtcNow.AddMinutes(-3);

            var createdName = string.Empty;
            var createdEmail = string.Empty;
            var createdPassword = string.Empty;

            var Client = new Client { Id = id, Name = name, Email = email, PasswordHash = passwordHashed, CreatedDate = createDate, UpdatedDate = updateDate, IsActive = false };

            _clientServiceMock.Setup(x => x.CreateClient(email, name, password)).Returns(Client);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/clients";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(new CreateClientRequest
            {
                Name = name,
                Email = email,
                Password = password
            });

            var result = actionResult as CreatedResult;
            var result1 = actionResult as BadRequestObjectResult;

            _clientServiceMock.Verify(x => x.CreateClient(email, name, password), Times.Never);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 400);
        }

        [TestMethod]
        public void CreateClient_ServiceReturnNull_ReturnInternalServerError()
        {
            var email = "email2@mail.ru";
            var name = "name1";
            var password = "Sshfgjk123654";

            _clientServiceMock.Setup(x => x.CreateClient(email, name, password)).Returns((Client)null);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/Clients";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(new CreateClientRequest
            {
                Email = email,
                Name = name,
                Password = password
            });

            var result = actionResult as CreatedResult;
            var result1 = actionResult as ObjectResult;

            _clientServiceMock.Verify(x => x.CreateClient(email, name, password), Times.Once);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 500);
        }

        [TestMethod]
        public void CreateClient_ServiceReturnException_ReturnInternalServerError()
        {
            var email = "email2@mail.ru";
            var name = "name1";
            var password = "Sshfgjk123654";

            var exceptionMessage = "any exception message";

            _clientServiceMock.Setup(x => x.CreateClient(email, name, password)).Throws(new Exception(exceptionMessage));

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/Clients";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(new CreateClientRequest
            {
                Email = email,
                Name = name,
                Password = password
            });

            var result = actionResult as CreatedResult;
            var result1 = actionResult as ObjectResult;

            _clientServiceMock.Verify(x => x.CreateClient(email, name, password), Times.Once);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 500);
        }

        [TestMethod]
        public void ActivateClient_ClientExisted_ReturnOk()
        {
            var activationCode = "12345qwerty78906yuiopsdfg";

            _clientServiceMock.Setup(x => x.ActivateClient(activationCode)).Returns(true);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object);

            var actionResult = controller.ActivateClient(activationCode);
            var result = actionResult as OkResult;

            _clientServiceMock.Verify(x => x.ActivateClient(activationCode), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
        }

        [TestMethod]
        public void ActivateClient_ClientNotExisted_ReturnNotFound()
        {
            var activationCode = "12345qwerty78906yuiopsdfg";

            _clientServiceMock.Setup(x => x.ActivateClient(activationCode)).Returns(false);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object);

            var actionResult = controller.ActivateClient(activationCode);
            var result = actionResult as OkObjectResult;
            var result1 = actionResult as NotFoundResult;


            _clientServiceMock.Verify(x => x.ActivateClient(activationCode), Times.Once);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 404);
        }

        [TestMethod]
        public void ActivateClient_ServiceReturnException_ReturnInternalServerError()
        {
            var activationCode = "12345qwerty78906yuiopsdfg";
            var exceptionMessage = "any exception message";

            _clientServiceMock.Setup(x => x.ActivateClient(activationCode)).Throws(new Exception(exceptionMessage));

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object);

            var actionResult = controller.ActivateClient(activationCode);
            var result = actionResult as OkObjectResult;
            var result1 = actionResult as ObjectResult;

            _clientServiceMock.Verify(x => x.ActivateClient(activationCode), Times.Once);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 500);
        }

        [TestMethod]
        public void SendActivationCode_ClientExisted_ReturnOk()
        {
            var email = "email2@mail.ru";

            _clientServiceMock.Setup(x => x.SendActivationCode(email)).Returns(true);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object);

            var actionResult = controller.SendActivationCode(email);
            var result = actionResult as OkResult;

            _clientServiceMock.Verify(x => x.SendActivationCode(email), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
        }

        [TestMethod]
        public void SendActivationCode_ClientNotExisted_ReturnNotFound()
        {
            var email = "email2@mail.ru";

            _clientServiceMock.Setup(x => x.SendActivationCode(email)).Returns(false);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object);

            var actionResult = controller.SendActivationCode(email);
            var result = actionResult as OkObjectResult;
            var result1 = actionResult as NotFoundResult;


            _clientServiceMock.Verify(x => x.SendActivationCode(email), Times.Once);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 404);
        }

        [TestMethod]
        public void SendActivationCode_EmailHasInvalidFormat_ReturnOk()
        {
            var email = "email2";

            _clientServiceMock.Setup(x => x.SendActivationCode(email)).Returns(true);

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object);

            var actionResult = controller.SendActivationCode(email);
            var result = actionResult as OkResult;
            var result1 = actionResult as BadRequestObjectResult;

            _clientServiceMock.Verify(x => x.SendActivationCode(email), Times.Never);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 400);
        }

        [TestMethod]
        public void SendActivationCode_ServiceReturnException_ReturnInternalServerError()
        {
            var email = "email2@mail.ru";
            var exceptionMessage = "any exception message";

            _clientServiceMock.Setup(x => x.SendActivationCode(email)).Throws(new Exception(exceptionMessage));

            var controller = new ClientsController(_clientServiceMock.Object, _loggerMock.Object);

            var actionResult = controller.SendActivationCode(email);
            var result = actionResult as OkObjectResult;
            var result1 = actionResult as ObjectResult;

            _clientServiceMock.Verify(x => x.SendActivationCode(email), Times.Once);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 500);
        }
    }
}