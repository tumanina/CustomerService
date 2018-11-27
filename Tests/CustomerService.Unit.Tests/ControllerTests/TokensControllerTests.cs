using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CustomerService.Api.Areas.V1.Controllers;
using CustomerService.Api.Areas.V1.Models;
using Token = CustomerService.Business.Models.Token;
using CustomerService.Business;

namespace CustomerService.Unit.Tests.ControllerTests
{
    [TestClass]
    public class TokensControllerTests
    {
        private static readonly Mock<ITokenService> TokenService = new Mock<ITokenService>();
        private static readonly Mock<ILogger<TokensController>> Logger = new Mock<ILogger<TokensController>>();

        [TestMethod]
        public void GetTokens_TokensExisted_ReturnOk()
        {
            TokenService.Invocations.Clear();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var value1 = "542gythnfsli8";
            var value2 = "111gythnfsli8";
            var value3 = "333gythnfsli8";
            var ip1 = "127.0.0.1";
            var ip2 = "127.0.0.2";
            var ip3 = "127.0.0.3";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate2 = DateTime.UtcNow.AddDays(-2);
            var createDate3 = DateTime.UtcNow.AddDays(-3);

            var data = new List<Token>
            {
                new Token { Id = id1, ClientId = clientId, Value = value1, IP = ip1, CreatedDate = createDate1, IsActive = true },
                new Token { Id = id2, ClientId = clientId, Value = value2, IP = ip2, CreatedDate = createDate2, IsActive = true },
                new Token { Id = id3, ClientId = clientId, Value = value3, IP = ip3, CreatedDate = createDate3, IsActive = true }
            };

            TokenService.Setup(x => x.GetTokens(clientId, It.IsAny<bool>())).Returns(data);

            var controller = new TokensController(TokenService.Object, Logger.Object);

            var actionResult = controller.Get(clientId);
            var result = actionResult as OkObjectResult;
            var tokens = result.Value as List<Api.Areas.V1.Models.Token>;

            TokenService.Verify(x => x.GetTokens(clientId, It.IsAny<bool>()), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual(tokens.Count(), 3);
            Assert.IsTrue(tokens.Any(t => t.Id == id1 && t.ClientId == clientId && t.Value == value1 && t.IP == ip1 && t.CreatedDate == createDate1 && t.IsActive == true));
            Assert.IsTrue(tokens.Any(t => t.Id == id2 && t.ClientId == clientId && t.Value == value2 && t.IP == ip2 && t.CreatedDate == createDate2 && t.IsActive == true));
            Assert.IsTrue(tokens.Any(t => t.Id == id3 && t.ClientId == clientId && t.Value == value3 && t.IP == ip3 && t.CreatedDate == createDate3 && t.IsActive == true));
        }

        [TestMethod]
        public void GetTokens_TokensNotExisted_ReturnEmptyList()
        {
            TokenService.Invocations.Clear();

            var clientId = Guid.NewGuid();

            TokenService.Setup(x => x.GetTokens(clientId, It.IsAny<bool>())).Returns(new List<Token>());

            var controller = new TokensController(TokenService.Object, Logger.Object);

            var actionResult = controller.Get(clientId);
            var result = actionResult as OkObjectResult;
            var tokens = result.Value as List<Api.Areas.V1.Models.Token>;

            TokenService.Verify(x => x.GetTokens(clientId, It.IsAny<bool>()), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.AreEqual(tokens.Count(), 0);
        }

        [TestMethod]
        public void GetTokens_ServiceReturnException_ReturnInternalServerError()
        {
            TokenService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var exceptionMessage = "some exception message";

            TokenService.Setup(x => x.GetTokens(clientId, It.IsAny<bool>())).Throws(new Exception(exceptionMessage));

            var controller = new TokensController(TokenService.Object, Logger.Object);

            var actionResult = controller.Get(clientId);
            var result = actionResult as OkObjectResult;
            var result1 = actionResult as ObjectResult;

            TokenService.Verify(x => x.GetTokens(clientId, It.IsAny<bool>()), Times.Once);
            Assert.IsTrue(result == null);
            Assert.IsTrue(result1 != null);
            Assert.AreEqual(result1.StatusCode, 500);
            Assert.AreEqual(result1.Value, exceptionMessage);
        }

        
        [TestMethod]
        public void CreateToken_Success_ReturnCreatedAndCorrect()
        {
            TokenService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var value = "542gythnfsli8";
            var ip = "127.0.0.1";
            var createDate = DateTime.UtcNow;
            var authMethod = "token";

            var token = new Token { Id = id, ClientId = clientId, Value = value, IP = ip, AuthenticationMethod = authMethod, CreatedDate = createDate, IsActive = true };

            TokenService.Setup(x => x.CreateToken(clientId, ip, authMethod)).Returns(token);

            var controller = new TokensController(TokenService.Object, Logger.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/tokens";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(clientId, new CreateTokenRequest
            {
                IP = ip,
                AuthMethod = authMethod
            });

            var result = actionResult as CreatedResult;

            TokenService.Verify(x => x.CreateToken(clientId, ip, authMethod), Times.Once);
            Assert.AreEqual(result.StatusCode, 201);
        }

        [TestMethod]
        public void CreateToken_RequestIsEmpty_ReturnBadRequest()
        {
            TokenService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var value = "542gythnfsli8";
            var ip = "127.0.0.1";
            var createDate = DateTime.UtcNow;
            var authMethod = "token";

            var token = new Token { Id = id, ClientId = clientId, Value = value, IP = ip, AuthenticationMethod = authMethod, CreatedDate = createDate, IsActive = true };

            TokenService.Setup(x => x.CreateToken(clientId, ip, authMethod)).Returns(token);

            var controller = new TokensController(TokenService.Object, Logger.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/tokens";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(clientId, null);

            var result = actionResult as CreatedResult;
            var result1 = actionResult as BadRequestObjectResult;

            TokenService.Verify(x => x.CreateToken(clientId, ip, authMethod), Times.Never);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 400);
        }

        public void CreateToken_IpIsEmpty_ReturnBadRequest()
        {
            TokenService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var value = "542gythnfsli8";
            var ip = "127.0.0.1";
            var createDate = DateTime.UtcNow;
            var authMethod = "token";

            var token = new Token { Id = id, ClientId = clientId, Value = value, IP = ip, AuthenticationMethod = authMethod, CreatedDate = createDate, IsActive = true };

            TokenService.Setup(x => x.CreateToken(clientId, ip, authMethod)).Returns(token);

            var controller = new TokensController(TokenService.Object, Logger.Object);

            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/tokens";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(clientId, new CreateTokenRequest
            {
                AuthMethod = authMethod
            });

            var result = actionResult as CreatedResult;
            var result1 = actionResult as BadRequestObjectResult;

            TokenService.Verify(x => x.CreateToken(clientId, ip, authMethod), Times.Never);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 400);
        }

        
        public void CreateToken_IPHasInvalidFormat_ReturnBadRequest()
        {
            TokenService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var value = "542gythnfsli8";
            var ip = "11";
            var createDate = DateTime.UtcNow;
            var authMethod = "token";

            var token = new Token { Id = id, ClientId = clientId, Value = value, IP = ip, AuthenticationMethod = authMethod, CreatedDate = createDate, IsActive = true };

            TokenService.Setup(x => x.CreateToken(clientId, ip, authMethod)).Returns(token);

            var controller = new TokensController(TokenService.Object, Logger.Object);

            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/tokens";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(clientId, new CreateTokenRequest
            {
                IP = ip,
                AuthMethod = authMethod
            });

            var result = actionResult as CreatedResult;
            var result1 = actionResult as BadRequestObjectResult;

            TokenService.Verify(x => x.CreateToken(clientId, ip, authMethod), Times.Never);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 400);
        }

        [TestMethod]
        public void CreateToken_ServiceReturnNull_ReturnInternalServerError()
        {
            TokenService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var ip = "127.0.0.1";
            var authMethod = "token";

            TokenService.Setup(x => x.CreateToken(clientId, ip, authMethod)).Returns((Token) null);

            var controller = new TokensController(TokenService.Object, Logger.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/tokens";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(clientId, new CreateTokenRequest
            {
                IP = ip,
                AuthMethod = authMethod
            });

            var result = actionResult as CreatedResult;
            var result1 = actionResult as ObjectResult;

            TokenService.Verify(x => x.CreateToken(clientId, ip, authMethod), Times.Once);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 500);
        }

        [TestMethod]
        public void CreateToken_ServiceReturnException_ReturnInternalServerError()
        {
            TokenService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var ip = "127.0.0.1";
            var authMethod = "token";

            var exceptionMessage = "any exception message";
            TokenService.Setup(x => x.CreateToken(clientId, ip, authMethod)).Throws(new Exception(exceptionMessage));

            var controller = new TokensController(TokenService.Object, Logger.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            controller.ControllerContext.HttpContext.Request.Scheme = "http";
            controller.ControllerContext.HttpContext.Request.Host = new HostString("someurl.com", 72001);
            controller.ControllerContext.HttpContext.Request.Path = "/api/v1/Tokens";
            controller.ControllerContext.HttpContext.Request.Method = HttpMethod.Post.ToString();

            var actionResult = controller.Post(clientId, new CreateTokenRequest
            {
                IP = ip,
                AuthMethod = authMethod
            });

            var result = actionResult as CreatedResult;
            var result1 = actionResult as ObjectResult;

            TokenService.Verify(x => x.CreateToken(clientId, ip, authMethod), Times.Once);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 500);
        }

        [TestMethod]
        public void ActivateToken_Success_ReturnCorrect()
        {
            TokenService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var value = "542gythnfsli8";
            var ip = "11";
            var createDate = DateTime.UtcNow;
            var authMethod = "token";

            var token = new Token { Id = id, ClientId = clientId, Value = value, IP = ip, AuthenticationMethod = authMethod, CreatedDate = createDate, IsActive = true };
            TokenService.Setup(x => x.UpdateActive(id, true)).Returns(token);

            var controller = new TokensController(TokenService.Object, Logger.Object);
            var actionResult = controller.ActivateToken(id);
            var result = actionResult as OkObjectResult;

            TokenService.Verify(x => x.UpdateActive(id, true), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
        }

        [TestMethod]
        public void ActivateToken_ServiceReturnNull_ReturnNotFound()
        {
            TokenService.Invocations.Clear();

            var id = Guid.NewGuid();

            TokenService.Setup(x => x.UpdateActive(id, true)).Returns((Token)null);

            var controller = new TokensController(TokenService.Object, Logger.Object);

            var actionResult = controller.ActivateToken(id);
            var result = actionResult as OkObjectResult;
            var result1 = actionResult as NotFoundResult;

            TokenService.Verify(x => x.UpdateActive(id, true), Times.Once);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 404);
        }

        [TestMethod]
        public void ActivateToken_ServiceReturnException_ReturnInternalServerError()
        {
            TokenService.Invocations.Clear();

            var id = Guid.NewGuid();

            var exceptionMessage = "any exception message";

            TokenService.Setup(x => x.UpdateActive(id, true)).Throws(new Exception(exceptionMessage));

            var controller = new TokensController(TokenService.Object, Logger.Object);

            var actionResult = controller.ActivateToken(id);
            var result = actionResult as OkObjectResult;
            var result1 = actionResult as ObjectResult;

            TokenService.Verify(x => x.UpdateActive(id, true), Times.Once);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 500);
        }

        [TestMethod]
        public void DeactivateToken_Success_ReturnCorrect()
        {
            TokenService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var value = "542gythnfsli8";
            var ip = "11";
            var createDate = DateTime.UtcNow;
            var authMethod = "token";

            var token = new Token { Id = id, ClientId = clientId, Value = value, IP = ip, AuthenticationMethod = authMethod, CreatedDate = createDate, IsActive = true };
            TokenService.Setup(x => x.UpdateActive(id, false)).Returns(token);

            var controller = new TokensController(TokenService.Object, Logger.Object);

            var actionResult = controller.DeactivateToken(id);

            var result = actionResult as OkObjectResult;

            TokenService.Verify(x => x.UpdateActive(id, false), Times.Once);
            Assert.AreEqual(result.StatusCode, 200);
        }

        [TestMethod]
        public void DeactivateToken_ServiceReturnNull_ReturnNotFound()
        {
            TokenService.Invocations.Clear();

            var id = Guid.NewGuid();

            TokenService.Setup(x => x.UpdateActive(id, false)).Returns((Token)null);

            var controller = new TokensController(TokenService.Object, Logger.Object);

            var actionResult = controller.DeactivateToken(id);

            var result = actionResult as OkObjectResult;
            var result1 = actionResult as NotFoundResult;

            TokenService.Verify(x => x.UpdateActive(id, false), Times.Once);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 404);
        }

        [TestMethod]
        public void DeactivateToken_ServiceReturnException_ReturnInternalServerError()
        {
            TokenService.Invocations.Clear();

            var id = Guid.NewGuid();

            var exceptionMessage = "any exception message";

            TokenService.Setup(x => x.UpdateActive(id, false)).Throws(new Exception(exceptionMessage));

            var controller = new TokensController(TokenService.Object, Logger.Object);

            var actionResult = controller.DeactivateToken(id);

            var result = actionResult as OkObjectResult;
            var result1 = actionResult as ObjectResult;

            TokenService.Verify(x => x.UpdateActive(id, false), Times.Once);
            Assert.AreEqual(result, null);
            Assert.AreEqual(result1.StatusCode, 500);
        }
    }
}