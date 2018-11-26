using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CustomerService.Business;
using CustomerService.Business.Models;
using CustomerService.Repositories;
using SessionEntity = CustomerService.Repositories.Entities.Session;

namespace CustomerService.UnitTests.ServiceTests
{
    [TestClass]
    public class SessionServiceTest
    {
        private static readonly Mock<ISessionRepository> SessionRepository = new Mock<ISessionRepository>();
        private static readonly Mock<IClientService> ClientService = new Mock<IClientService>();

        [TestMethod]
        public void GetSessions_SessionsExisted_ShouldReturnCorrect()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

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

            var data = new List<SessionEntity>
                {
                    new SessionEntity { Id = id1, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = true, Enabled = true },
                    new SessionEntity { Id = id2, ClientId = clientId, IP = ip2, SessionKey = key2, CreatedDate = createDate2, UpdatedDate = updateDate2, ExpiredDate = expireDate2, Confirmed = true, Enabled = true },
                    new SessionEntity { Id = id3, ClientId = clientId, IP = ip3, SessionKey = key3, CreatedDate = createDate3, UpdatedDate = updateDate3, ExpiredDate = expireDate3, Confirmed = true, Enabled = true }
                };

            SessionRepository.Setup(x => x.GetSessions(clientId, false)).Returns(data);

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            var result = service.GetSessions(clientId);

            SessionRepository.Verify(x => x.GetSessions(clientId, false), Times.Once);
            Assert.AreEqual(result.Count(), 3);
            Assert.IsTrue(result.Any(t => t.Id == id1 && t.ClientId == clientId && t.SessionKey == key1 && t.CreatedDate == createDate1 && t.UpdatedDate == updateDate1 && t.ExpiredDate == expireDate1 && t.Confirmed == true && t.Enabled == true));
            Assert.IsTrue(result.Any(t => t.Id == id2 && t.ClientId == clientId && t.SessionKey == key2 && t.CreatedDate == createDate2 && t.UpdatedDate == updateDate2 && t.ExpiredDate == expireDate2 && t.Confirmed == true && t.Enabled == true));
            Assert.IsTrue(result.Any(t => t.Id == id3 && t.ClientId == clientId && t.SessionKey == key3 && t.CreatedDate == createDate3 && t.UpdatedDate == updateDate3 && t.ExpiredDate == expireDate3 && t.Confirmed == true && t.Enabled == true));
        }

        [TestMethod]
        public void GetSessions_SessionsNotExisted_ShouldReturnCorrect()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

            var clientId = Guid.NewGuid();

            var data = new List<SessionEntity>();

            SessionRepository.Setup(x => x.GetSessions(clientId, false)).Returns(data);

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            var result = service.GetSessions(clientId);

            SessionRepository.Verify(x => x.GetSessions(clientId, false), Times.Once);
            Assert.AreEqual(result.Count(), 0);
        }

        [TestMethod]
        public void GetSession_SessionExisted_ShouldReturnCorrect()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var key1 = Guid.NewGuid().ToString();
            var ip1 = "127.0.0.1";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var expireDate1 = DateTime.UtcNow.AddDays(1);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);

            var session = new SessionEntity { Id = id, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = true, Enabled = true };

            SessionRepository.Setup(x => x.GetSession(clientId, id)).Returns(session);

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            var result = service.GetSession(clientId, id);

            SessionRepository.Verify(x => x.GetSession(clientId, id), Times.Once);
            Assert.AreEqual(result.ClientId, clientId);
            Assert.AreEqual(result.UpdatedDate, updateDate1);
            Assert.AreEqual(result.CreatedDate, createDate1);
            Assert.AreEqual(result.IP, ip1);
            Assert.AreEqual(result.SessionKey, key1);
            Assert.AreEqual(result.Id, id);
        }

        [TestMethod]
        public void GetSession_SessionNotExisted_ShouldReturnNull()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            SessionRepository.Setup(x => x.GetSession(clientId, id)).Returns((SessionEntity)null);

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            var result = service.GetSession(clientId, id);

            SessionRepository.Verify(x => x.GetSession(clientId, id), Times.Once);
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void GetSessionByKey_SessionExisted_ShouldReturnCorrect()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var key1 = Guid.NewGuid().ToString();
            var ip1 = "127.0.0.1";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var expireDate1 = DateTime.UtcNow.AddDays(1);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);

            var session = new SessionEntity { Id = id, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = true, Enabled = true };

            SessionRepository.Setup(x => x.GetSessionByKey(key1)).Returns(session);

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            var result = service.GetSessionByKey(key1);

            SessionRepository.Verify(x => x.GetSessionByKey(key1), Times.Once);
            Assert.AreEqual(result.ClientId, clientId);
            Assert.AreEqual(result.UpdatedDate, updateDate1);
            Assert.AreEqual(result.CreatedDate, createDate1);
            Assert.AreEqual(result.IP, ip1);
            Assert.AreEqual(result.SessionKey, key1);
            Assert.AreEqual(result.Id, id);
        }

        [TestMethod]
        public void GetSessionByKey_SessionNotExisted_ShouldReturnNull()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

            var key1 = Guid.NewGuid().ToString();

            SessionRepository.Setup(x => x.GetSessionByKey(key1)).Returns((SessionEntity)null);

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            var result = service.GetSessionByKey(key1);

            SessionRepository.Verify(x => x.GetSessionByKey(key1), Times.Once);
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void CreateSession_Success_ShouldReturnSession()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

            var name = "name1";
            var password = "password1";
            var clientId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var key1 = Guid.NewGuid().ToString();
            var ip1 = "127.0.0.1";
            var interval = 900;
            var createDate1 = DateTime.UtcNow;
            var expireDate1 = DateTime.UtcNow.AddSeconds(900);
            var updateDate1 = DateTime.UtcNow;

            var entity = new SessionEntity { Id = id, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = false, Enabled = false };

            ClientService.Setup(x => x.Authentification(name, password)).Returns(new Client { Id = clientId, Name = name });
            SessionRepository.Setup(x => x.CreateSession(clientId, ip1, interval, true)).Returns(entity);

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            var result = service.CreateSession(name, password, ip1, interval);

            SessionRepository.Verify(x => x.CreateSession(clientId, ip1, interval, true), Times.Once);
            Assert.AreEqual(result.ClientId, clientId);
            Assert.AreEqual(result.CreatedDate, createDate1);
            Assert.AreEqual(result.UpdatedDate, updateDate1);
            Assert.AreEqual(result.ExpiredDate, expireDate1);
            Assert.AreEqual(result.IP, ip1);
            Assert.AreEqual(result.Id, id);
            Assert.AreEqual(result.Confirmed, false);
            Assert.AreEqual(result.Enabled, false);
        }

        [TestMethod]
        public void CreateSession_ServiceReturnNull_ShouldReturnNull()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

            var name = "name1";
            var password = "password1";
            var clientId = Guid.NewGuid();
            var key1 = Guid.NewGuid().ToString();
            var ip1 = "127.0.0.1";
            var interval = 200;
            var createDate1 = DateTime.UtcNow;
            var expireDate1 = DateTime.UtcNow.AddSeconds(200);
            var updateDate1 = DateTime.UtcNow;

            SessionRepository.Setup(x => x.CreateSession(clientId, ip1, interval, true)).Returns((SessionEntity)null);
            ClientService.Setup(x => x.Authentification(name, password)).Returns(new Client { Id = clientId, Name = name });

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            var result = service.CreateSession(name, password, ip1, interval);

            SessionRepository.Verify(x => x.CreateSession(clientId, ip1, interval, true), Times.Once);
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void ConfirmSession_Success_ShouldReturnTrue()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var oneTimePasword = "12345678";

            SessionRepository.Setup(x => x.ConfirmSession(clientId, id)).Returns(true);
            ClientService.Setup(x => x.ValidateClientByGoogleAuth(clientId, oneTimePasword)).Returns(true);

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            var result = service.ConfirmSession(clientId, id, oneTimePasword);

            SessionRepository.Verify(x => x.ConfirmSession(clientId, id), Times.Once);
            ClientService.Verify(x => x.ValidateClientByGoogleAuth(clientId, oneTimePasword), Times.Once);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void ConfirmSession_GoogleAuthReturnsFalse_ShouldThrowException()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var oneTimePasword = "12345678";

            SessionRepository.Setup(x => x.ConfirmSession(clientId, id)).Returns(true);
            ClientService.Setup(x => x.ValidateClientByGoogleAuth(clientId, oneTimePasword)).Returns(false);

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            try
            {
                var result = service.ConfirmSession(clientId, id, oneTimePasword);
            }
            catch (Exception)
            {
                SessionRepository.Verify(x => x.ConfirmSession(clientId, id), Times.Never);
                ClientService.Verify(x => x.ValidateClientByGoogleAuth(clientId, oneTimePasword), Times.Once);
            }
        }

        [TestMethod]
        public void ConfirmSession_False_ShouldReturnFalse()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var oneTimePasword = "12345678";

            SessionRepository.Setup(x => x.ConfirmSession(clientId, id)).Returns(false);
            ClientService.Setup(x => x.ValidateClientByGoogleAuth(clientId, oneTimePasword)).Returns(true);

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            var result = service.ConfirmSession(clientId, id, oneTimePasword);

            SessionRepository.Verify(x => x.ConfirmSession(clientId, id), Times.Once);
            ClientService.Verify(x => x.ValidateClientByGoogleAuth(clientId, oneTimePasword), Times.Once);
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void IsSessionConfirmRequired_SessionExistAndConfirm_ShouldReturnFalse()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var key1 = Guid.NewGuid().ToString();
            var ip1 = "127.0.0.1";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var expireDate1 = DateTime.UtcNow.AddDays(1);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);

            var session = new SessionEntity { Id = id, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = true, Enabled = true };

            SessionRepository.Setup(x => x.GetSession(clientId, id)).Returns(session);

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            var result = service.IsSessionConfirmRequired(clientId, id);

            SessionRepository.Verify(x => x.GetSession(clientId, id), Times.Once);
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void IsSessionConfirmRequired_SessionExistAndNotConfirm_ShouldReturnTrue()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var key1 = Guid.NewGuid().ToString();
            var ip1 = "127.0.0.1";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var expireDate1 = DateTime.UtcNow.AddDays(1);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);

            var session = new SessionEntity { Id = id, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = false, Enabled = true };

            SessionRepository.Setup(x => x.GetSession(clientId, id)).Returns(session);

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            var result = service.IsSessionConfirmRequired(clientId, id);

            SessionRepository.Verify(x => x.GetSession(clientId, id), Times.Once);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void IsSessionConfirmRequired_SessionNotExist_ShouldReturnFalse()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            SessionRepository.Setup(x => x.GetSession(clientId, id)).Returns((SessionEntity)null);

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            var result = service.IsSessionConfirmRequired(clientId, id);

            SessionRepository.Verify(x => x.GetSession(clientId, id), Times.Once);
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void DisableSession_Success_ShouldReturnTrue()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            SessionRepository.Setup(x => x.DisableSession(clientId, id)).Returns(true);

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            var result = service.DisableSession(clientId, id);

            SessionRepository.Verify(x => x.DisableSession(clientId, id), Times.Once);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DisableSession_False_ShouldReturnFalse()
        {
            SessionRepository.Invocations.Clear();
            ClientService.Invocations.Clear();

            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            SessionRepository.Setup(x => x.DisableSession(clientId, id)).Returns(false);

            var service = new SessionService(SessionRepository.Object, ClientService.Object);

            var result = service.DisableSession(clientId, id);

            SessionRepository.Verify(x => x.DisableSession(clientId, id), Times.Once);
            Assert.AreEqual(result, false);
        }
    }
}
