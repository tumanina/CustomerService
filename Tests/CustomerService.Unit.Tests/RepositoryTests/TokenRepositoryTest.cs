using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CustomerService.Repositories;
using CustomerService.Repositories.DAL;
using CustomerService.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomerService.Unit.Tests.RepositoryTests
{
    [TestClass]
    public class TokenRepositoryTest
    {
        private static readonly Mock<ICustomerDBContext> _customerDBContextMock = new Mock<ICustomerDBContext>();

        [TestCleanup]
        public void TestCleanUp()
        {
            _customerDBContextMock.Invocations.Clear();
        }

        [TestMethod]
        public void GetTokens_TokensExisted_UseDbContextReturnCorrect()
        {
            var mockSet = new Mock<DbSet<Token>>();

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
            }.AsQueryable();

            mockSet.As<IQueryable<Token>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Token>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Token>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Token>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            _customerDBContextMock.Setup(x => x.Token).Returns(mockSet.Object);

            var repository = new TokenRepository(_customerDBContextMock.Object);

            var result = repository.GetTokens(clientId);

            _customerDBContextMock.Verify(x => x.Token, Times.Once);
            Assert.AreEqual(result.Count(), 3);
            Assert.IsTrue(result.Any(t => t.Id == id1 && t.ClientId == clientId && t.Value == value1 && t.IP == ip1 && t.CreatedDate == createDate1 && t.IsActive == true));
            Assert.IsTrue(result.Any(t => t.Id == id2 && t.ClientId == clientId && t.Value == value2 && t.IP == ip2 && t.CreatedDate == createDate2 && t.IsActive == true));
            Assert.IsTrue(result.Any(t => t.Id == id3 && t.ClientId == clientId && t.Value == value3 && t.IP == ip3 && t.CreatedDate == createDate3 && t.IsActive == true));
        }

        [TestMethod]
        public void GetTokensOnlyActive_TokensExisted_UseDbContextReturnCorrect()
        {
            var mockSet = new Mock<DbSet<Token>>();

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
                new Token { Id = id1, ClientId = clientId, Value = value1, IP = ip1, CreatedDate = createDate1, IsActive = false },
                new Token { Id = id2, ClientId = clientId, Value = value2, IP = ip2, CreatedDate = createDate2, IsActive = true },
                new Token { Id = id3, ClientId = clientId, Value = value3, IP = ip3, CreatedDate = createDate3, IsActive = true }
            }.AsQueryable();

            mockSet.As<IQueryable<Token>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Token>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Token>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Token>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            _customerDBContextMock.Setup(x => x.Token).Returns(mockSet.Object);

            var repository = new TokenRepository(_customerDBContextMock.Object);

            var result = repository.GetTokens(clientId, true);

            _customerDBContextMock.Verify(x => x.Token, Times.Once);
            Assert.AreEqual(result.Count(), 2);
            Assert.IsTrue(result.Any(t => t.Id == id2 && t.ClientId == clientId && t.Value == value2 && t.IP == ip2 && t.CreatedDate == createDate2 && t.IsActive == true));
            Assert.IsTrue(result.Any(t => t.Id == id3 && t.ClientId == clientId && t.Value == value3 && t.IP == ip3 && t.CreatedDate == createDate3 && t.IsActive == true));
        }

        [TestMethod]
        public void GetTokens_TokensNotExisted_UseDbContextReturnCorrect()
        {
            var clientId = Guid.NewGuid();

            var mockSet = new Mock<DbSet<Token>>();

            var data = new List<Token>().AsQueryable();

            mockSet.As<IQueryable<Token>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Token>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Token>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Token>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            _customerDBContextMock.Setup(x => x.Token).Returns(mockSet.Object);

            var repository = new TokenRepository(_customerDBContextMock.Object);

            var result = repository.GetTokens(clientId);

            _customerDBContextMock.Verify(x => x.Token, Times.Once);
            Assert.AreEqual(result.Count(), 0);
        }

        [TestMethod]
        public void CreateToken_Correct_UseDbContextReturnCorrect()
        {
            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var value = "542gythnfsli8";
            var ip = "127.0.0.1";
            var authMethod = "token";
            var createDate = DateTime.UtcNow;
            var updateDate = DateTime.UtcNow;

            var token = new Token();

            var mockSet0 = new Mock<DbSet<Token>>();

            var data0 = new List<Token>().AsQueryable();

            mockSet0.As<IQueryable<Token>>().Setup(m => m.Provider).Returns(data0.Provider);
            mockSet0.As<IQueryable<Token>>().Setup(m => m.Expression).Returns(data0.Expression);
            mockSet0.As<IQueryable<Token>>().Setup(m => m.ElementType).Returns(data0.ElementType);
            mockSet0.As<IQueryable<Token>>().Setup(m => m.GetEnumerator()).Returns(() => data0.GetEnumerator());
            mockSet0.Setup(m => m.Add(It.IsAny<Token>()))
                .Callback((Token TokenParam) => { token = TokenParam; });

            var data = new List<Token>
            {
                new Token { Id = id, ClientId = clientId, Value = value, IP = ip, CreatedDate = createDate, AuthenticationMethod = authMethod, IsActive = true }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Token>>();

            mockSet.As<IQueryable<Token>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Token>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Token>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Token>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Token>()));

            _customerDBContextMock.Setup(x => x.Token).Returns(mockSet.Object);
            _customerDBContextMock.Setup(x => x.Set<Token>()).Returns(mockSet0.Object);
            _customerDBContextMock.Setup(x => x.SaveChanges()).Returns(1);

            var repository = new TokenRepository(_customerDBContextMock.Object);

            var result = repository.CreateToken(clientId, ip, authMethod);

            _customerDBContextMock.Verify(x => x.Set<Token>(), Times.Once);
            _customerDBContextMock.Verify(x => x.SaveChanges(), Times.Once);
            mockSet0.Verify(x => x.Add(It.IsAny<Token>()), Times.Once);
            Assert.AreEqual(token.ClientId, clientId);
            Assert.AreEqual(token.IP, ip);
            Assert.AreEqual(token.AuthenticationMethod, authMethod);
        }

        [TestMethod]
        public void UpdateActive_TokenExisted_UseDbContextReturnCorrect()
        {
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
                new Token { Id = id2, ClientId = clientId, Value = value2, IP = ip2, CreatedDate = createDate2, IsActive = false },
                new Token { Id = id3, ClientId = clientId, Value = value3, IP = ip3, CreatedDate = createDate3, IsActive = true }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Token>>();

            mockSet.As<IQueryable<Token>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Token>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Token>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Token>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Token>()));

            _customerDBContextMock.Setup(x => x.Token).Returns(mockSet.Object);
            _customerDBContextMock.Setup(x => x.Set<Token>()).Returns(mockSet.Object);
            _customerDBContextMock.Setup(x => x.SaveChanges()).Returns(1);

            var repository = new TokenRepository(_customerDBContextMock.Object);

            var result = repository.UpdateActive(id2, true);

            Assert.IsTrue(result.IsActive);
            Assert.AreEqual(result.ClientId, clientId);
            Assert.AreEqual(result.Value, value2);
            Assert.AreEqual(result.IP, ip2);
            _customerDBContextMock.Verify(x => x.Token, Times.Exactly(1));
            _customerDBContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void UpdateActive_TokenNotExisted_UseDbContextReturnNull()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var value1 = "542gythnfsli8";
            var value3 = "333gythnfsli8";
            var ip1 = "127.0.0.1";
            var ip3 = "127.0.0.3";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate3 = DateTime.UtcNow.AddDays(-3);

            var data = new List<Token>
            {
                new Token { Id = id1, ClientId = clientId, Value = value1, IP = ip1, CreatedDate = createDate1, IsActive = true },
                new Token { Id = id3, ClientId = clientId, Value = value3, IP = ip3, CreatedDate = createDate3, IsActive = true }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Token>>();

            mockSet.As<IQueryable<Token>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Token>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Token>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Token>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Token>()));

            _customerDBContextMock.Setup(x => x.Token).Returns(mockSet.Object);
            _customerDBContextMock.Setup(x => x.Set<Token>()).Returns(mockSet.Object);
            _customerDBContextMock.Setup(x => x.SaveChanges()).Returns(1);

            var repository = new TokenRepository(_customerDBContextMock.Object);

            var result = repository.UpdateActive(id2, false);

            _customerDBContextMock.Verify(x => x.Token, Times.Exactly(1));
            _customerDBContextMock.Verify(x => x.SaveChanges(), Times.Never);
            Assert.AreEqual(result, null);
        }
    }
}
