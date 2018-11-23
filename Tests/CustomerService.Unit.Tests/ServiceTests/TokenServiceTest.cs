using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CustomerService.Business;
using CustomerService.Repositories;
using TokenEntity = CustomerService.Repositories.Entities.Token;

namespace CustomerService.UnitTests.ServiceTests
{
    [TestClass]
    public class TokenServiceTest
    {
        private static readonly Mock<ITokenRepository> TokenRepository = new Mock<ITokenRepository>();
        private static readonly Mock<ISessionRepository> SessionRepository = new Mock<ISessionRepository>();

        [TestMethod]
        public void GetTokens_TokensExisted_ShouldReturnCorrect()
        {
            TokenRepository.Invocations.Clear();

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

            var data = new List<TokenEntity>
            {
                new TokenEntity { Id = id1, ClientId = clientId, Value = value1, IP = ip1, CreatedDate = createDate1, IsActive = true },
                new TokenEntity { Id = id2, ClientId = clientId, Value = value2, IP = ip2, CreatedDate = createDate2, IsActive = true },
                new TokenEntity { Id = id3, ClientId = clientId, Value = value3, IP = ip3, CreatedDate = createDate3, IsActive = true }
            };

            TokenRepository.Setup(x => x.GetTokens(clientId, It.IsAny<bool>())).Returns(data);

            var service = new TokenService(TokenRepository.Object);

            var result = service.GetTokens(clientId);

            TokenRepository.Verify(x => x.GetTokens(clientId, It.IsAny<bool>()), Times.Once);
            Assert.AreEqual(result.Count(), 3);
            Assert.IsTrue(result.Any(t => t.Id == id1 && t.ClientId == clientId && t.Value == value1 && t.IP == ip1 && t.CreatedDate == createDate1 && t.IsActive == true));
            Assert.IsTrue(result.Any(t => t.Id == id2 && t.ClientId == clientId && t.Value == value2 && t.IP == ip2 && t.CreatedDate == createDate2 && t.IsActive == true));
            Assert.IsTrue(result.Any(t => t.Id == id3 && t.ClientId == clientId && t.Value == value3 && t.IP == ip3 && t.CreatedDate == createDate3 && t.IsActive == true));
        }

        public void GetTokensOnlyActive_TokensExisted_ShouldReturnCorrect()
        {
            TokenRepository.Invocations.Clear();

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

            var data = new List<TokenEntity>
            {
                new TokenEntity { Id = id1, ClientId = clientId, Value = value1, IP = ip1, CreatedDate = createDate1, IsActive = true },
                new TokenEntity { Id = id2, ClientId = clientId, Value = value2, IP = ip2, CreatedDate = createDate2, IsActive = true },
                new TokenEntity { Id = id3, ClientId = clientId, Value = value3, IP = ip3, CreatedDate = createDate3, IsActive = true }
            };

            TokenRepository.Setup(x => x.GetTokens(clientId, It.IsAny<bool>())).Returns(data);

            var service = new TokenService(TokenRepository.Object);

            var result = service.GetTokens(clientId, true);

            TokenRepository.Verify(x => x.GetTokens(clientId, It.IsAny<bool>()), Times.Once);
            Assert.AreEqual(result.Count(), 3);
            Assert.IsTrue(result.Any(t => t.Id == id1 && t.ClientId == clientId && t.Value == value1 && t.IP == ip1 && t.CreatedDate == createDate1 && t.IsActive == true));
            Assert.IsTrue(result.Any(t => t.Id == id2 && t.ClientId == clientId && t.Value == value2 && t.IP == ip2 && t.CreatedDate == createDate2 && t.IsActive == true));
            Assert.IsTrue(result.Any(t => t.Id == id3 && t.ClientId == clientId && t.Value == value3 && t.IP == ip3 && t.CreatedDate == createDate3 && t.IsActive == true));
        }

        [TestMethod]
        public void GetTokens_TokensNotExisted_ShouldReturnCorrect()
        {
            TokenRepository.Invocations.Clear();

            var clientId = Guid.NewGuid();

            TokenRepository.Setup(x => x.GetTokens(clientId, It.IsAny<bool>())).Returns(new List<TokenEntity>());

            var service = new TokenService(TokenRepository.Object);

            var result = service.GetTokens(clientId);

            TokenRepository.Verify(x => x.GetTokens(clientId, It.IsAny<bool>()), Times.Once);
            Assert.AreEqual(result.Count(), 0);
        }

        [TestMethod]
        public void CreateToken_Success_ShouldReturnToken()
        {
            TokenRepository.Invocations.Clear();
            SessionRepository.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var value = "542gythnfsli8";
            var ip = "127.0.0.1";
            var createDate = DateTime.UtcNow;
            var authMethod = "token";

            var client = Guid.NewGuid();
            
            var token = new TokenEntity { Id = id, ClientId = clientId, Value = value, IP = ip, CreatedDate = createDate, IsActive = true };

            TokenRepository.Setup(x => x.CreateToken(clientId, It.IsAny<string>(), It.IsAny<string>())).Returns(token)
                .Callback((Guid clientParam, string ipParam, string methodParam) => { client = clientParam; });

            var service = new TokenService(TokenRepository.Object);

            var result = service.CreateToken(clientId, ip, authMethod);

            TokenRepository.Verify(x => x.CreateToken(clientId, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(result.ClientId, clientId);
            Assert.AreEqual(result.Value, value);
            Assert.AreEqual(client, clientId);
        }

        [TestMethod]
        public void CreateToken_ServiceReturnNull_ShouldReturnNull()
        {
            TokenRepository.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var ip = "127.0.0.1";
            var createDate = DateTime.UtcNow;
            var authMethod = "token";

            var client = Guid.NewGuid();

            TokenRepository.Setup(x => x.CreateToken(clientId, It.IsAny<string>(), It.IsAny<string>())).Returns((TokenEntity) null)
                .Callback((Guid clientParam, string ipParam, string methodParam) => { client = clientParam; });

            var service = new TokenService(TokenRepository.Object);

            var result = service.CreateToken(clientId, ip, authMethod);
            TokenRepository.Verify(x => x.CreateToken(clientId, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(result, null);
        }

        
        [TestMethod]
        public void UpdateActive_Success_ShouldReturnToken()
        {
            TokenRepository.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var value = "542gythnfsli8";
            var ip = "127.0.0.1";
            var createDate = DateTime.UtcNow;
            var isActive = true;

            var token = new TokenEntity { Id = id, ClientId = clientId, Value = value, IP = ip, CreatedDate = createDate, IsActive = true };

            TokenRepository.Setup(x => x.UpdateActive(id, isActive)).Returns(token);

            var service = new TokenService(TokenRepository.Object);

            var result = service.UpdateActive(id, isActive);

            TokenRepository.Verify(x => x.UpdateActive(id, isActive), Times.Once);
            Assert.AreEqual(result.Id, id);
        }

        [TestMethod]
        public void UpdateActive_ServiceReturnNull_ShouldReturnNull()
        {
            TokenRepository.Invocations.Clear();

            var id = Guid.NewGuid();
            var isActive = true;

            TokenRepository.Setup(x => x.UpdateActive(id, isActive)).Returns((TokenEntity) null);

            var service = new TokenService(TokenRepository.Object);

            var result = service.UpdateActive(id, isActive);

            TokenRepository.Verify(x => x.UpdateActive(id, isActive), Times.Once);
            Assert.AreEqual(result, null);
        }
    }
}
