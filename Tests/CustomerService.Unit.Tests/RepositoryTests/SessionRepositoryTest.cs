using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CustomerService.Repositories;
using CustomerService.Repositories.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using CustomerService.Repositories.Entities;

namespace CustomerService.Unit.Tests.RepositoryTests
{
    [TestClass]
    public class SessionRepositoryTest
    {
        private static readonly Mock<ICustomerDBContext> CustomerDBContext = new Mock<ICustomerDBContext>();
        private static readonly Mock<ICustomerDBContextFactory> CustomerDBContextFactory = new Mock<ICustomerDBContextFactory>();

        [TestMethod]
        public void GettSessions_SessionsExisted_UseDbContextReturnCorrect()
        {
            CustomerDBContext.Invocations.Clear();

            var mockSet = new Mock<DbSet<Session>>();

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
            var expireDate3 = DateTime.UtcNow.AddDays(-3);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);
            var updateDate3 = DateTime.UtcNow.AddMinutes(-9);

            var data = new List<Session>
            {
                new Session { Id = id1, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = true, Enabled = true },
                new Session { Id = id2, ClientId = clientId, IP = ip2, SessionKey = key2, CreatedDate = createDate2, UpdatedDate = updateDate2, ExpiredDate = expireDate2, Confirmed = true, Enabled = false },
                new Session { Id = id3, ClientId = clientId, IP = ip3, SessionKey = key3, CreatedDate = createDate3, UpdatedDate = updateDate3, ExpiredDate = expireDate3, Confirmed = true, Enabled = true }
            }.AsQueryable();

            mockSet.As<IQueryable<Session>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Session>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Session>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Session>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            CustomerDBContext.Setup(x => x.Session).Returns(mockSet.Object);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new SessionRepository(CustomerDBContextFactory.Object);

            var result = repository.GetSessions(clientId, false);

            CustomerDBContext.Verify(x => x.Session, Times.Once);
            Assert.AreEqual(result.Count(), 3);
            Assert.IsTrue(result.Any(t => t.Id == id1 && t.ClientId == clientId && t.SessionKey == key1 && t.CreatedDate == createDate1 && t.UpdatedDate == updateDate1 && t.ExpiredDate == expireDate1 && t.Confirmed == true && t.Enabled == true));
            Assert.IsTrue(result.Any(t => t.Id == id2 && t.ClientId == clientId && t.SessionKey == key2 && t.CreatedDate == createDate2 && t.UpdatedDate == updateDate2 && t.ExpiredDate == expireDate2 && t.Confirmed == true && t.Enabled == false));
            Assert.IsTrue(result.Any(t => t.Id == id3 && t.ClientId == clientId && t.SessionKey == key3 && t.CreatedDate == createDate3 && t.UpdatedDate == updateDate3 && t.ExpiredDate == expireDate3 && t.Confirmed == true && t.Enabled == true));
        }

        [TestMethod]
        public void GettSessionsOnlyActive_SessionsExisted_UseDbContextReturnCorrect()
        {
            CustomerDBContext.Invocations.Clear();

            var mockSet = new Mock<DbSet<Session>>();

            var clientId = Guid.NewGuid();
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();
            var id4 = Guid.NewGuid();
            var key1 = Guid.NewGuid().ToString();
            var key2 = Guid.NewGuid().ToString();
            var key3 = Guid.NewGuid().ToString();
            var key4 = Guid.NewGuid().ToString();
            var ip1 = "127.0.0.1";
            var ip2 = "127.0.0.2";
            var ip3 = "127.0.0.3";
            var ip4 = "127.0.0.4";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate2 = DateTime.UtcNow.AddDays(-2);
            var createDate3 = DateTime.UtcNow.AddDays(-3);
            var createDate4 = DateTime.UtcNow.AddDays(-3);
            var expireDate1 = DateTime.UtcNow.AddDays(1);
            var expireDate2 = DateTime.UtcNow.AddDays(2);
            var expireDate3 = DateTime.UtcNow.AddDays(-3);
            var expireDate4 = DateTime.UtcNow.AddDays(3);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);
            var updateDate3 = DateTime.UtcNow.AddMinutes(-9);
            var updateDate4 = DateTime.UtcNow.AddMinutes(-5);

            var data = new List<Session>
            {
                new Session { Id = id1, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = true, Enabled = true },
                new Session { Id = id2, ClientId = clientId, IP = ip2, SessionKey = key2, CreatedDate = createDate2, UpdatedDate = updateDate2, ExpiredDate = expireDate2, Confirmed = true, Enabled = false },
                new Session { Id = id3, ClientId = clientId, IP = ip3, SessionKey = key3, CreatedDate = createDate3, UpdatedDate = updateDate3, ExpiredDate = expireDate3, Confirmed = true, Enabled = true },
                new Session { Id = id4, ClientId = clientId, IP = ip4, SessionKey = key4, CreatedDate = createDate4, UpdatedDate = updateDate4, ExpiredDate = expireDate4, Confirmed = true, Enabled = true }
            }.AsQueryable();

            mockSet.As<IQueryable<Session>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Session>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Session>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Session>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            CustomerDBContext.Setup(x => x.Session).Returns(mockSet.Object);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new SessionRepository(CustomerDBContextFactory.Object);

            var result = repository.GetSessions(clientId, true);

            CustomerDBContext.Verify(x => x.Session, Times.Once);
            Assert.AreEqual(result.Count(), 2);
            Assert.IsTrue(result.Any(t => t.Id == id1 && t.ClientId == clientId && t.SessionKey == key1 && t.CreatedDate == createDate1 && t.UpdatedDate == updateDate1 && t.ExpiredDate == expireDate1 && t.Confirmed == true && t.Enabled == true));
            Assert.IsTrue(result.Any(t => t.Id == id4 && t.ClientId == clientId && t.SessionKey == key4 && t.CreatedDate == createDate4 && t.UpdatedDate == updateDate4 && t.ExpiredDate == expireDate4 && t.Confirmed == true && t.Enabled == true));
        }

        [TestMethod]
        public void GetSessions_SessionsNotExisted_UseDbContextReturnCorrect()
        {
            CustomerDBContext.Invocations.Clear();

            var clientId = Guid.NewGuid();

            var mockSet = new Mock<DbSet<Session>>();

            var data = new List<Session>().AsQueryable();

            mockSet.As<IQueryable<Session>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Session>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Session>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Session>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            CustomerDBContext.Setup(x => x.Session).Returns(mockSet.Object);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new SessionRepository(CustomerDBContextFactory.Object);

            var result = repository.GetSessions(clientId);

            CustomerDBContext.Verify(x => x.Session, Times.Once);
            Assert.AreEqual(result.Count(), 0);
        }

        [TestMethod]
        public void GetSession_SessionExisted_UseDbContextReturnCorrect()
        {
            CustomerDBContext.Invocations.Clear();

            var mockSet = new Mock<DbSet<Session>>();

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
            var expireDate3 = DateTime.UtcNow.AddDays(-3);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);
            var updateDate3 = DateTime.UtcNow.AddMinutes(-9);

            var data = new List<Session>
            {
                new Session { Id = id1, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = true, Enabled = true },
                new Session { Id = id2, ClientId = clientId, IP = ip2, SessionKey = key2, CreatedDate = createDate2, UpdatedDate = updateDate2, ExpiredDate = expireDate2, Confirmed = true, Enabled = false },
                new Session { Id = id3, ClientId = clientId, IP = ip3, SessionKey = key3, CreatedDate = createDate3, UpdatedDate = updateDate3, ExpiredDate = expireDate3, Confirmed = true, Enabled = true }
            }.AsQueryable();

            mockSet.As<IQueryable<Session>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Session>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Session>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Session>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            CustomerDBContext.Setup(x => x.Session).Returns(mockSet.Object);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new SessionRepository(CustomerDBContextFactory.Object);

            var result = repository.GetSession(clientId, id2);

            CustomerDBContext.Verify(x => x.Session, Times.Once);
            Assert.AreEqual(result.ClientId, clientId);
            Assert.AreEqual(result.UpdatedDate, updateDate2);
            Assert.AreEqual(result.CreatedDate, createDate2);
            Assert.AreEqual(result.IP, ip2);
            Assert.AreEqual(result.SessionKey, key2);
            Assert.AreEqual(result.IP, ip2);
            Assert.AreEqual(result.IP, ip2);
            Assert.AreEqual(result.Id, id2);
        }

        [TestMethod]
        public void GetSession_SessionNotExisted_UseDbContextReturnNull()
        {
            CustomerDBContext.Invocations.Clear();

            var mockSet = new Mock<DbSet<Session>>();

            var clientId = Guid.NewGuid();
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();
            var key1 = Guid.NewGuid().ToString();
            var key3 = Guid.NewGuid().ToString();
            var ip1 = "127.0.0.1";
            var ip3 = "127.0.0.3";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate3 = DateTime.UtcNow.AddDays(-3);
            var expireDate1 = DateTime.UtcNow.AddDays(1);
            var expireDate3 = DateTime.UtcNow.AddDays(-3);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate3 = DateTime.UtcNow.AddMinutes(-9);

            var data = new List<Session>
            {
                new Session { Id = id1, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = true, Enabled = true },
                new Session { Id = id3, ClientId = clientId, IP = ip3, SessionKey = key3, CreatedDate = createDate3, UpdatedDate = updateDate3, ExpiredDate = expireDate3, Confirmed = true, Enabled = true }
            }.AsQueryable();

            mockSet.As<IQueryable<Session>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Session>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Session>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Session>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            CustomerDBContext.Setup(x => x.Session).Returns(mockSet.Object);

            var repository = new SessionRepository(CustomerDBContextFactory.Object);

            var result = repository.GetSession(clientId, id2);

            CustomerDBContext.Verify(x => x.Session, Times.Once);
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void GetSessionByKey_SessionExisted_UseDbContextReturnCorrect()
        {
            CustomerDBContext.Invocations.Clear();

            var mockSet = new Mock<DbSet<Session>>();

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
            var expireDate3 = DateTime.UtcNow.AddDays(-3);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);
            var updateDate3 = DateTime.UtcNow.AddMinutes(-9);

            var data = new List<Session>
            {
                new Session { Id = id1, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = true, Enabled = true },
                new Session { Id = id2, ClientId = clientId, IP = ip2, SessionKey = key2, CreatedDate = createDate2, UpdatedDate = updateDate2, ExpiredDate = expireDate2, Confirmed = true, Enabled = false },
                new Session { Id = id3, ClientId = clientId, IP = ip3, SessionKey = key3, CreatedDate = createDate3, UpdatedDate = updateDate3, ExpiredDate = expireDate3, Confirmed = true, Enabled = true }
            }.AsQueryable();

            mockSet.As<IQueryable<Session>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Session>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Session>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Session>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            CustomerDBContext.Setup(x => x.Session).Returns(mockSet.Object);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new SessionRepository(CustomerDBContextFactory.Object);

            var result = repository.GetSessionByKey(key2);

            CustomerDBContext.Verify(x => x.Session, Times.Once);
            Assert.AreEqual(result.ClientId, clientId);
            Assert.AreEqual(result.UpdatedDate, updateDate2);
            Assert.AreEqual(result.CreatedDate, createDate2);
            Assert.AreEqual(result.IP, ip2);
            Assert.AreEqual(result.SessionKey, key2);
            Assert.AreEqual(result.IP, ip2);
            Assert.AreEqual(result.IP, ip2);
            Assert.AreEqual(result.Id, id2);
        }

        [TestMethod]
        public void GetSessionByKey_SessionNotExisted_UseDbContextReturnNull()
        {
            CustomerDBContext.Invocations.Clear();

            var mockSet = new Mock<DbSet<Session>>();

            var clientId = Guid.NewGuid();
            var id1 = Guid.NewGuid();
            var id3 = Guid.NewGuid();
            var key1 = Guid.NewGuid().ToString();
            var key2 = Guid.NewGuid().ToString();
            var key3 = Guid.NewGuid().ToString();
            var ip1 = "127.0.0.1";
            var ip3 = "127.0.0.3";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate3 = DateTime.UtcNow.AddDays(-3);
            var expireDate1 = DateTime.UtcNow.AddDays(1);
            var expireDate3 = DateTime.UtcNow.AddDays(-3);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate3 = DateTime.UtcNow.AddMinutes(-9);

            var data = new List<Session>
            {
                new Session { Id = id1, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = true, Enabled = true },
                new Session { Id = id3, ClientId = clientId, IP = ip3, SessionKey = key3, CreatedDate = createDate3, UpdatedDate = updateDate3, ExpiredDate = expireDate3, Confirmed = true, Enabled = true }
            }.AsQueryable();

            mockSet.As<IQueryable<Session>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Session>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Session>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Session>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            CustomerDBContext.Setup(x => x.Session).Returns(mockSet.Object);

            var repository = new SessionRepository(CustomerDBContextFactory.Object);

            var result = repository.GetSessionByKey(key2);

            CustomerDBContext.Verify(x => x.Session, Times.Once);
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void ConfirmSession_SessionExisted_UseDbContextReturnCorrect()
        {
            CustomerDBContext.Invocations.Clear();

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
            var expireDate3 = DateTime.UtcNow.AddDays(-3);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);
            var updateDate3 = DateTime.UtcNow.AddMinutes(-9);

            var data = new List<Session>
            {
                new Session { Id = id1, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = true, Enabled = true },
                new Session { Id = id2, ClientId = clientId, IP = ip2, SessionKey = key2, CreatedDate = createDate2, UpdatedDate = updateDate2, ExpiredDate = expireDate2, Confirmed = true, Enabled = false },
                new Session { Id = id3, ClientId = clientId, IP = ip3, SessionKey = key3, CreatedDate = createDate3, UpdatedDate = updateDate3, ExpiredDate = expireDate3, Confirmed = true, Enabled = true }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Session>>();

            mockSet.As<IQueryable<Session>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Session>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Session>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Session>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Session>()));

            CustomerDBContext.Setup(x => x.Session).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.Set<Session>()).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.SaveChanges()).Returns(1);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new SessionRepository(CustomerDBContextFactory.Object);

            var result = repository.ConfirmSession(clientId, id1);

            Assert.IsTrue(result);
            CustomerDBContext.Verify(x => x.Session, Times.Exactly(1));
            CustomerDBContext.Verify(x => x.SaveChanges(), Times.Once);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void ConfirmSession_SessionNotExisted_UseDbContextReturnCorrect()
        {
            CustomerDBContext.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();
            var key1 = Guid.NewGuid().ToString();
            var key3 = Guid.NewGuid().ToString();
            var ip1 = "127.0.0.1";
            var ip3 = "127.0.0.3";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate3 = DateTime.UtcNow.AddDays(-3);
            var expireDate1 = DateTime.UtcNow.AddDays(1);
            var expireDate3 = DateTime.UtcNow.AddDays(-3);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate3 = DateTime.UtcNow.AddMinutes(-9);

            var data = new List<Session>
            {
                new Session { Id = id1, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = true, Enabled = true },
                new Session { Id = id3, ClientId = clientId, IP = ip3, SessionKey = key3, CreatedDate = createDate3, UpdatedDate = updateDate3, ExpiredDate = expireDate3, Confirmed = true, Enabled = true }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Session>>();

            mockSet.As<IQueryable<Session>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Session>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Session>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Session>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Session>()));

            CustomerDBContext.Setup(x => x.Session).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.Set<Session>()).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.SaveChanges()).Returns(1);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new SessionRepository(CustomerDBContextFactory.Object);

            var result = repository.ConfirmSession(clientId, id2);

            CustomerDBContext.Verify(x => x.Session, Times.Exactly(1));
            CustomerDBContext.Verify(x => x.SaveChanges(), Times.Never);
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void DisableSession_SessionExisted_UseDbContextReturnCorrect()
        {
            CustomerDBContext.Invocations.Clear();

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
            var expireDate3 = DateTime.UtcNow.AddDays(-3);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);
            var updateDate3 = DateTime.UtcNow.AddMinutes(-9);

            var data = new List<Session>
            {
                new Session { Id = id1, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = true, Enabled = true },
                new Session { Id = id2, ClientId = clientId, IP = ip2, SessionKey = key2, CreatedDate = createDate2, UpdatedDate = updateDate2, ExpiredDate = expireDate2, Confirmed = true, Enabled = false },
                new Session { Id = id3, ClientId = clientId, IP = ip3, SessionKey = key3, CreatedDate = createDate3, UpdatedDate = updateDate3, ExpiredDate = expireDate3, Confirmed = true, Enabled = true }
            }.AsQueryable();
            var mockSet = new Mock<DbSet<Session>>();

            mockSet.As<IQueryable<Session>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Session>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Session>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Session>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Session>()));

            CustomerDBContext.Setup(x => x.Session).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.Set<Session>()).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.SaveChanges()).Returns(1);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new SessionRepository(CustomerDBContextFactory.Object);

            var result = repository.DisableSession(clientId, id1);

            CustomerDBContext.Verify(x => x.Session, Times.Exactly(1));
            CustomerDBContext.Verify(x => x.SaveChanges(), Times.Once);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DisableSession_SessionNotExisted_UseDbContextReturnCorrect()
        {
            CustomerDBContext.Invocations.Clear();

            var clientId = Guid.NewGuid();
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();
            var key1 = Guid.NewGuid().ToString();
            var key3 = Guid.NewGuid().ToString();
            var ip1 = "127.0.0.1";
            var ip3 = "127.0.0.3";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate3 = DateTime.UtcNow.AddDays(-3);
            var expireDate1 = DateTime.UtcNow.AddDays(1);
            var expireDate3 = DateTime.UtcNow.AddDays(-3);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate3 = DateTime.UtcNow.AddMinutes(-9);

            var data = new List<Session>
            {
                new Session { Id = id1, ClientId = clientId, IP = ip1, SessionKey = key1, CreatedDate = createDate1, UpdatedDate = updateDate1, ExpiredDate = expireDate1, Confirmed = true, Enabled = true },
                new Session { Id = id3, ClientId = clientId, IP = ip3, SessionKey = key3, CreatedDate = createDate3, UpdatedDate = updateDate3, ExpiredDate = expireDate3, Confirmed = true, Enabled = true }
            }.AsQueryable();
            var mockSet = new Mock<DbSet<Session>>();

            mockSet.As<IQueryable<Session>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Session>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Session>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Session>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Session>()));

            CustomerDBContext.Setup(x => x.Session).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.Set<Session>()).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.SaveChanges()).Returns(1);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new SessionRepository(CustomerDBContextFactory.Object);

            var result = repository.DisableSession(clientId, id2);

            CustomerDBContext.Verify(x => x.Session, Times.Exactly(1));
            CustomerDBContext.Verify(x => x.SaveChanges(), Times.Never);
            Assert.AreEqual(result, false);
        }
    }
}
