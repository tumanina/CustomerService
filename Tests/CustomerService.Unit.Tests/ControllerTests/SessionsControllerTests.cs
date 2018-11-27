using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CustomerService.Api.Areas.V1.Controllers;
using Session = CustomerService.Business.Models.Session;
using Microsoft.AspNetCore.Http;
using CustomerService.Api.Areas.V1.Models;
using System.Net.Http;
using CustomerService.Business;

namespace CustomerService.Unit.Tests.ControllerTests
{
    [TestClass]
    public class SessionsControllerTests
    {
        private static readonly Mock<ISessionService> SessionService = new Mock<ISessionService>();
        private static readonly Mock<ILogger<SessionsController>> Logger = new Mock<ILogger<SessionsController>>();

        [TestMethod]
        public void GetSessions_SessionsExisted_ReturnOk()
        {
            SessionService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();
            var key1 = Guid.NewGuid().ToString();
            var key2 = Guid.NewGuid().ToString();
            var key3 = Guid.NewGuid().ToString();
            var ip1 = "127.0.0.1";
            var ip2 = "127.0.0.2";
            var ip3 = "127.0.0.3";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate2 = DateTime.UtcNow.AddDays(-2);
            var createDate3 = DateTime.UtcNow.AddDays(-3);
            var expireDate1 = DateTime.UtcNow.AddDays(1);
            var expireDate2 = DateTime.UtcNow.AddDays(2);
            var expireDate3 = DateTime.UtcNow.AddDays(3);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);
            var updateDate3 = DateTime.UtcNow.AddMinutes(-9);

            SessionService.Setup(x => x.GetSessions(clientId, false)).Returns(new List<Session>
                {
                    new Session { Id = id1, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = true, Enabled = true },
                    new Session { Id = id2, ClientId = clientId, IP = ip2, SessionKey = key2, CreatedDate = createDate2, UpdatedDate = updateDate2, ExpiredDate = expireDate2, Confirmed = true, Enabled = true },
                    new Session { Id = id3, ClientId = clientId, IP = ip3, SessionKey = key3, CreatedDate = createDate3, UpdatedDate = updateDate3, ExpiredDate = expireDate3, Confirmed = true, Enabled = true }
                });

            var controller = new SessionsController(SessionService.Object, Logger.Object);

            var actionResult = controller.Get(clientId);
            var result = actionResult as OkObjectResult;
            var listResult = result.Value as IEnumerable<Api.Areas.V1.Models.Session>;

            SessionService.Verify(x => x.GetSessions(clientId, false), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual(listResult.Count(), 3);
            Assert.IsTrue(listResult.Any(t => t.Id == id1 && t.ClientId == clientId && t.SessionKey == key1 && t.CreatedDate == createDate1 && t.UpdatedDate == updateDate1 && t.ExpiredDate == expireDate1 && t.Confirmed == true && t.Enabled == true));
            Assert.IsTrue(listResult.Any(t => t.Id == id2 && t.ClientId == clientId && t.SessionKey == key2 && t.CreatedDate == createDate2 && t.UpdatedDate == updateDate2 && t.ExpiredDate == expireDate2 && t.Confirmed == true && t.Enabled == true));
            Assert.IsTrue(listResult.Any(t => t.Id == id3 && t.ClientId == clientId && t.SessionKey == key3 && t.CreatedDate == createDate3 && t.UpdatedDate == updateDate3 && t.ExpiredDate == expireDate3 && t.Confirmed == true && t.Enabled == true));
        }

        [TestMethod]
        public void GetSessions_SessionsNotExisted_ReturnCorrect()
        {
            SessionService.Invocations.Clear();

            var clientId = Guid.NewGuid();

            SessionService.Setup(x => x.GetSessions(clientId, false)).Returns(new List<Session>());

            var controller = new SessionsController(SessionService.Object, Logger.Object);

            var actionResult = controller.Get(clientId);
            var result = actionResult as OkObjectResult;
            var pagedResult = result.Value as IEnumerable<Api.Areas.V1.Models.Session>;

            SessionService.Verify(x => x.GetSessions(clientId, false), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual(pagedResult.Count(), 0);
        }

        [TestMethod]
        public void GetSession_SessionExisted_ReturnOk()
        {
            SessionService.Invocations.Clear();

            var id = Guid.NewGuid();
            var ip = "127.0.0.3";
            var clientId = Guid.NewGuid();
            var CreatedDate = DateTime.Now;

            SessionService.Setup(x => x.GetSession(clientId, id)).Returns(new Session { Id = id, ClientId = clientId, IP = ip, CreatedDate = CreatedDate, Confirmed = true, Enabled = true } );

            var controller = new SessionsController(SessionService.Object, Logger.Object);

            var actionResult = controller.GetById(clientId, id);
            var result = actionResult as OkObjectResult;
            var session = result.Value as Api.Areas.V1.Models.Session;

            SessionService.Verify(x => x.GetSession(clientId, id), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual(session.ClientId, clientId);
            Assert.AreEqual(session.CreatedDate, CreatedDate);
            Assert.AreEqual(session.IP, ip);
            Assert.AreEqual(session.Id, id);
            Assert.AreEqual(session.Confirmed, true);
            Assert.AreEqual(session.Enabled, true);
        }

        [TestMethod]
        public void GetSession_SessionNotExisted_ReturnNotFound()
        {
            SessionService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            SessionService.Setup(x => x.GetSession(clientId, id)).Returns((Session) null);

            var controller = new SessionsController(SessionService.Object, Logger.Object);

            var actionResult = controller.GetById(clientId, id);
            var result = actionResult as OkObjectResult;
            var result1 = actionResult as NotFoundResult;

            SessionService.Verify(x => x.GetSession(clientId, id), Times.Once);
            Assert.IsTrue(result == null);
            Assert.IsTrue(result1 != null);
        }

        [TestMethod]
        public void GetSession_ServiceReturnException_ReturnInternalServerError()
        {
            SessionService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var exceptionMessage = "some exception message";

            SessionService.Setup(x => x.GetSession(clientId, id)).Throws(new Exception(exceptionMessage));

            var controller = new SessionsController(SessionService.Object, Logger.Object);

            var actionResult = controller.GetById(clientId, id);
            var result = actionResult as OkObjectResult;
            var result1 = actionResult as ObjectResult;

            SessionService.Verify(x => x.GetSession(clientId, id), Times.Once);
            Assert.IsTrue(result == null);
            Assert.IsTrue(result1 != null);
            Assert.AreEqual(result1.StatusCode, 500);
            Assert.AreEqual(result1.Value, exceptionMessage);
        }

        [TestMethod]
        public void CreateSession_Success_ReturnCreatedAndCorrect()
        {
            SessionService.Invocations.Clear();

            var id = Guid.NewGuid();
            var name = "test";
            var password = "Ss123456";
            var ip = "127.0.0.3";
            var interval = 900;
            var sessionKey = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid();
            var CreatedDate = DateTime.Now;

            SessionService.Setup(x => x.CreateSession(name, password, ip, interval)).Returns(new Session { Id = id, SessionKey = sessionKey, ClientId = clientId, IP = ip, CreatedDate = CreatedDate, Confirmed = true, Enabled = true });

            var controller = new SessionsController(SessionService.Object, Logger.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/tokens";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(new CreateSessionKeyRequest
            {
                Name = name,
                Password = password,
                IP = ip
            });

            var result = actionResult as OkObjectResult;

            SessionService.Verify(x => x.CreateSession(name, password, ip, interval), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
        }

        [TestMethod]
        public void CreateSession_RequestIsEmpty_ReturnBadRequest()
        {
            SessionService.Invocations.Clear();

            var id = Guid.NewGuid();
            var name = "test";
            var password = "Ss123456";
            var ip = "127.0.0.3";
            var interval = 900;
            var sessionKey = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid();
            var CreatedDate = DateTime.Now;

            SessionService.Setup(x => x.CreateSession(name, password, ip, interval)).Returns(new Session { Id = id, SessionKey = sessionKey, ClientId = clientId, IP = ip, CreatedDate = CreatedDate, Confirmed = true, Enabled = true });

            var controller = new SessionsController(SessionService.Object, Logger.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/tokens";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(null);

            var result = actionResult as OkObjectResult;
            var result1 = actionResult as BadRequestObjectResult;

            SessionService.Verify(x => x.CreateSession(name, password, ip, interval), Times.Never);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 400);
        }

        [TestMethod]
        public void CreateSession_IPIsEmpty_ReturnBadRequest()
        {
            SessionService.Invocations.Clear();

            var id = Guid.NewGuid();
            var name = "test";
            var password = "Ss123456";
            var ip = "127.0.0.3";
            var interval = 900;
            var sessionKey = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid();
            var CreatedDate = DateTime.Now;

            SessionService.Setup(x => x.CreateSession(name, password, ip, interval)).Returns(new Session { Id = id, SessionKey = sessionKey, ClientId = clientId, IP = ip, CreatedDate = CreatedDate, Confirmed = true, Enabled = true });

            var controller = new SessionsController(SessionService.Object, Logger.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/tokens";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(new CreateSessionKeyRequest
            {
                Name = name,
                Password = password,
            });

            var result = actionResult as OkObjectResult;
            var result1 = actionResult as BadRequestObjectResult;

            SessionService.Verify(x => x.CreateSession(name, password, ip, interval), Times.Never);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 400);
        }

        [TestMethod]
        public void CreateSession_IPHasInvalidFormat_ReturnBadRequest()
        {
            SessionService.Invocations.Clear();

            var id = Guid.NewGuid();
            var name = "test";
            var password = "123456";
            var ip = "127.0";
            var interval = 900;
            var sessionKey = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid();
            var CreatedDate = DateTime.Now;

            SessionService.Setup(x => x.CreateSession(name, password, ip, interval)).Returns(new Session { Id = id, SessionKey = sessionKey, ClientId = clientId, IP = ip, CreatedDate = CreatedDate, Confirmed = true, Enabled = true });

            var controller = new SessionsController(SessionService.Object, Logger.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/tokens";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(new CreateSessionKeyRequest
            {
                Name = name,
                Password = password,
                IP = ip
            });

            var result = actionResult as OkObjectResult;
            var result1 = actionResult as BadRequestObjectResult;

            SessionService.Verify(x => x.CreateSession(name, password, ip, interval), Times.Never);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 400);
        }

        [TestMethod]
        public void IsSessionConfirmRequired_SessionExisted_ReturnCorrect()
        {
            SessionService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            SessionService.Setup(x => x.IsSessionConfirmRequired(clientId, id)).Returns(true);

            var controller = new SessionsController(SessionService.Object, Logger.Object);

            var actionResult = controller.IsSessionConfirmRequired(clientId, id);
            var result = actionResult as OkObjectResult;
 
            SessionService.Verify(x => x.IsSessionConfirmRequired(clientId, id), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual(result.Value, true);
        }

        [TestMethod]
        public void IsSessionConfirmRequired_SessionNotExisted_ReturnCorrect()
        {
            SessionService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            SessionService.Setup(x => x.IsSessionConfirmRequired(clientId, id)).Returns((bool?)null);

            var controller = new SessionsController(SessionService.Object, Logger.Object);

            var actionResult = controller.IsSessionConfirmRequired(clientId, id);
            var result = actionResult as NotFoundResult;

            SessionService.Verify(x => x.IsSessionConfirmRequired(clientId, id), Times.Once);
            Assert.AreEqual(result.StatusCode, 404);
        }

        [TestMethod]
        public void IsSessionConfirmRequired_ServiceReturnException_ReturnInternalServerError()
        {
            SessionService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            var exceptionMessage = "some exception message";

            SessionService.Setup(x => x.IsSessionConfirmRequired(clientId, id)).Throws(new Exception(exceptionMessage));

            var controller = new SessionsController(SessionService.Object, Logger.Object);

            var actionResult = controller.IsSessionConfirmRequired(clientId, id);

            var result = actionResult as OkObjectResult;
            var result1 = actionResult as ObjectResult;

            SessionService.Verify(x => x.IsSessionConfirmRequired(clientId, id), Times.Once);
            Assert.IsTrue(result == null);
            Assert.IsTrue(result1 != null);
            Assert.AreEqual(result1.StatusCode, 500);
            Assert.AreEqual(result1.Value, exceptionMessage);
        }

        [TestMethod]
        public void ConfirmSession_SessionExisted_ReturnCorrect()
        {
            SessionService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var oneTimePassword = "12345677"; 

            SessionService.Setup(x => x.ConfirmSession(clientId, id, oneTimePassword)).Returns(true);

            var controller = new SessionsController(SessionService.Object, Logger.Object);

            var actionResult = controller.ConfirmSession(clientId, id, oneTimePassword);

            var result = actionResult as OkResult;

            SessionService.Verify(x => x.ConfirmSession(clientId, id, oneTimePassword), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
        }

        [TestMethod]
        public void ConfirmSession_SessionNotExisted_ReturnCorrect()
        {
            SessionService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var oneTimePassword = "12345677";

            SessionService.Setup(x => x.ConfirmSession(clientId, id, oneTimePassword)).Returns(false);

            var controller = new SessionsController(SessionService.Object, Logger.Object);

            var actionResult = controller.ConfirmSession(clientId, id, oneTimePassword);

            var result = actionResult as NotFoundResult;

            SessionService.Verify(x => x.ConfirmSession(clientId, id, oneTimePassword), Times.Once);
            Assert.AreEqual(result.StatusCode, 404);
        }

        [TestMethod]
        public void ConfirmSession_ServiceReturnException_ReturnInternalServerError()
        {
            SessionService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var oneTimePassword = "12345677";

            var exceptionMessage = "some exception message";

            SessionService.Setup(x => x.ConfirmSession(clientId, id, oneTimePassword)).Throws(new Exception(exceptionMessage));

            var controller = new SessionsController(SessionService.Object, Logger.Object);

            var actionResult = controller.ConfirmSession(clientId, id, oneTimePassword);

            var result = actionResult as OkObjectResult;
            var result1 = actionResult as ObjectResult;

            SessionService.Verify(x => x.ConfirmSession(clientId, id, oneTimePassword), Times.Once);
            Assert.IsTrue(result == null);
            Assert.IsTrue(result1 != null);
            Assert.AreEqual(result1.StatusCode, 500);
            Assert.AreEqual(result1.Value, exceptionMessage);
        }

        [TestMethod]
        public void DisableSession_SessionExisted_ReturnCorrect()
        {
            SessionService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            SessionService.Setup(x => x.DisableSession(clientId, id)).Returns(true);

            var controller = new SessionsController(SessionService.Object, Logger.Object);

            var actionResult = controller.DisableSession(clientId, id);

            var result = actionResult as OkResult;

            SessionService.Verify(x => x.DisableSession(clientId, id), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
        }

        [TestMethod]
        public void DisableSession_SessionNotExisted_ReturnCorrect()
        {
            SessionService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            SessionService.Setup(x => x.DisableSession(clientId, id)).Returns(false);

            var controller = new SessionsController(SessionService.Object, Logger.Object);

            var actionResult = controller.DisableSession(clientId, id);

            var result = actionResult as NotFoundResult;

            SessionService.Verify(x => x.DisableSession(clientId, id), Times.Once);
            Assert.AreEqual(result.StatusCode, 404);
        }

        [TestMethod]
        public void DisableSession_ServiceReturnException_ReturnInternalServerError()
        {
            SessionService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            var exceptionMessage = "some exception message";

            SessionService.Setup(x => x.DisableSession(clientId, id)).Throws(new Exception(exceptionMessage));

            var controller = new SessionsController(SessionService.Object, Logger.Object);

            var actionResult = controller.DisableSession(clientId, id);

            var result = actionResult as OkObjectResult;
            var result1 = actionResult as ObjectResult;

            SessionService.Verify(x => x.DisableSession(clientId, id), Times.Once);
            Assert.IsTrue(result == null);
            Assert.IsTrue(result1 != null);
            Assert.AreEqual(result1.StatusCode, 500);
            Assert.AreEqual(result1.Value, exceptionMessage);
        }
    }
}
