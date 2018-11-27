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
    public class ClientRepositoryTest
    {
        private static readonly Mock<ICustomerDBContext> CustomerDBContext = new Mock<ICustomerDBContext>();
        private static readonly Mock<ICustomerDBContextFactory> CustomerDBContextFactory = new Mock<ICustomerDBContextFactory>();

        [TestMethod]
        public void GetClientByName_ClientExisted_UseDbContextReturnCorrect()
        {
            CustomerDBContext.Invocations.Clear();

            var mockSet = new Mock<DbSet<Client>>();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var email1 = "email1@mail.ru";
            var email2 = "email2@mail.ru";
            var name1 = "name1";
            var name2 = "name2";
            var passwordHashed1 = "542gythnfsli8";
            var passwordHashed2 = "111gythnfsli8";
            var activationCode1 = "12345qwerty78906yuiopsdfg";
            var activationCode2 = "56789qwerty78906yuiopsdfg";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate2 = DateTime.UtcNow.AddDays(-2);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);

            var data = new List<Client>
            {
                new Client { Id = id1, Name = name1, Email = email1, PasswordHash = passwordHashed1, ActivationCode = activationCode1, CreatedDate = createDate1, UpdatedDate = updateDate1, IsActive = true },
                new Client { Id = id2, Name = name2, Email = email2, PasswordHash = passwordHashed2, ActivationCode = activationCode2, CreatedDate = createDate2, UpdatedDate = updateDate2, IsActive = true }
            }.AsQueryable();

            mockSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            CustomerDBContext.Setup(x => x.Client).Returns(mockSet.Object);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new ClientRepository(CustomerDBContextFactory.Object);

            var result = repository.GetClientByName(name2);

            CustomerDBContext.Verify(x => x.Client, Times.Exactly(1));
            Assert.AreEqual(result.Name, name2);
            Assert.AreEqual(result.Email, email2);
            Assert.AreEqual(result.PasswordHash, passwordHashed2);
            Assert.AreEqual(result.ActivationCode, activationCode2);
            Assert.AreEqual(result.CreatedDate, createDate2);
            Assert.AreEqual(result.UpdatedDate, updateDate2);
            Assert.AreEqual(result.IsActive, true);
            Assert.AreEqual(result.Id, id2);
        }

        [TestMethod]
        public void GetClientsByName_ClientNotExisted_UseDbContextReturnCorrect()
        {
            CustomerDBContext.Invocations.Clear();

            var mockSet = new Mock<DbSet<Client>>();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var email1 = "email1@mail.ru";
            var email2 = "email2@mail.ru";
            var name1 = "name1";
            var name2 = "name2";
            var name3 = "name3";
            var passwordHashed1 = "542gythnfsli8";
            var passwordHashed2 = "111gythnfsli8";
            var activationCode1 = "12345qwerty78906yuiopsdfg";
            var activationCode2 = "56789qwerty78906yuiopsdfg";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate2 = DateTime.UtcNow.AddDays(-2);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);

            var data = new List<Client>
            {
                new Client { Id = id1, Name = name1, Email = email1, PasswordHash = passwordHashed1, ActivationCode = activationCode1, CreatedDate = createDate1, UpdatedDate = updateDate1, IsActive = true },
                new Client { Id = id2, Name = name2, Email = email2, PasswordHash = passwordHashed2, ActivationCode = activationCode2, CreatedDate = createDate2, UpdatedDate = updateDate2, IsActive = true }
            }.AsQueryable();

            mockSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            CustomerDBContext.Setup(x => x.Client).Returns(mockSet.Object);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new ClientRepository(CustomerDBContextFactory.Object);

            var result = repository.GetClientByName(name3);

            CustomerDBContext.Verify(x => x.Client, Times.Exactly(1));
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void GetClientByEmail_ClientExisted_UseDbContextReturnCorrect()
        {
            CustomerDBContext.Invocations.Clear();

            var mockSet = new Mock<DbSet<Client>>();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var email1 = "email1@mail.ru";
            var email2 = "email2@mail.ru";
            var name1 = "name1";
            var name2 = "name2";
            var passwordHashed1 = "542gythnfsli8";
            var passwordHashed2 = "111gythnfsli8";
            var activationCode1 = "12345qwerty78906yuiopsdfg";
            var activationCode2 = "56789qwerty78906yuiopsdfg";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate2 = DateTime.UtcNow.AddDays(-2);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);

            var data = new List<Client>
            {
                new Client { Id = id1, Name = name1, Email = email1, PasswordHash = passwordHashed1, ActivationCode = activationCode1, CreatedDate = createDate1, UpdatedDate = updateDate1, IsActive = true },
                new Client { Id = id2, Name = name2, Email = email2, PasswordHash = passwordHashed2, ActivationCode = activationCode2, CreatedDate = createDate2, UpdatedDate = updateDate2, IsActive = true }
            }.AsQueryable();

            mockSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            CustomerDBContext.Setup(x => x.Client).Returns(mockSet.Object);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new ClientRepository(CustomerDBContextFactory.Object);

            var result = repository.GetClientByEmail(email2);

            CustomerDBContext.Verify(x => x.Client, Times.Exactly(1));
            Assert.AreEqual(result.Name, name2);
            Assert.AreEqual(result.Email, email2);
            Assert.AreEqual(result.PasswordHash, passwordHashed2);
            Assert.AreEqual(result.ActivationCode, activationCode2);
            Assert.AreEqual(result.CreatedDate, createDate2);
            Assert.AreEqual(result.UpdatedDate, updateDate2);
            Assert.AreEqual(result.IsActive, true);
            Assert.AreEqual(result.Id, id2);
        }

        [TestMethod]
        public void GetClientsByEmail_ClientNotExisted_UseDbContextReturnCorrect()
        {
            CustomerDBContext.Invocations.Clear();

            var mockSet = new Mock<DbSet<Client>>();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var email1 = "email1@mail.ru";
            var email2 = "email2@mail.ru";
            var email3 = "email3@mail.ru";
            var name1 = "name1";
            var name2 = "name2";
            var passwordHashed1 = "542gythnfsli8";
            var passwordHashed2 = "111gythnfsli8";
            var activationCode1 = "12345qwerty78906yuiopsdfg";
            var activationCode2 = "56789qwerty78906yuiopsdfg";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate2 = DateTime.UtcNow.AddDays(-2);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);

            var data = new List<Client>
            {
                new Client { Id = id1, Name = name1, Email = email1, PasswordHash = passwordHashed1, ActivationCode = activationCode1, CreatedDate = createDate1, UpdatedDate = updateDate1, IsActive = true },
                new Client { Id = id2, Name = name2, Email = email2, PasswordHash = passwordHashed2, ActivationCode = activationCode2, CreatedDate = createDate2, UpdatedDate = updateDate2, IsActive = true }
            }.AsQueryable();

            mockSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            CustomerDBContext.Setup(x => x.Client).Returns(mockSet.Object);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new ClientRepository(CustomerDBContextFactory.Object);

            var result = repository.GetClientByEmail(email3);

            CustomerDBContext.Verify(x => x.Client, Times.Exactly(1));
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void CreateClient_Correct_UseDbContextReturnCorrect()
        {
            CustomerDBContext.Invocations.Clear();

            var id = Guid.NewGuid();
            var email = "email2@mail.ru";
            var name = "name1";
            var password = "123654";
            var passwordHashed = "542gythnfsli8";
            var activationCode = "12345qwerty78906yuiopsdfg";
            var createDate = DateTime.UtcNow.AddDays(-1);
            var updateDate = DateTime.UtcNow.AddMinutes(-3);

            var createdName = string.Empty;
            var createdEmail = string.Empty;
            var createdPassword = string.Empty;

            var client = new Client();

            var mockSet0 = new Mock<DbSet<Client>>();

            var data0 = new List<Client>().AsQueryable();

            mockSet0.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(data0.Provider);
            mockSet0.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(data0.Expression);
            mockSet0.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(data0.ElementType);
            mockSet0.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(() => data0.GetEnumerator());
            mockSet0.Setup(m => m.Add(It.IsAny<Client>()))
                .Callback((Client clientParam) => { client = clientParam; });

            var data = new List<Client>
            {
                new Client { Id = id, Name = name, Email = email, PasswordHash = passwordHashed, ActivationCode = activationCode, CreatedDate = createDate, UpdatedDate = updateDate, IsActive = false  }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Client>>();

            mockSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Client>()));

            CustomerDBContext.Setup(x => x.Client).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.Set<Client>()).Returns(mockSet0.Object);
            CustomerDBContext.Setup(x => x.SaveChanges()).Returns(1);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new ClientRepository(CustomerDBContextFactory.Object);

            var result = repository.CreateClient(email, name, password, activationCode);

            CustomerDBContext.Verify(x => x.Set<Client>(), Times.Once);
            CustomerDBContext.Verify(x => x.SaveChanges(), Times.Once);
            mockSet0.Verify(x => x.Add(It.IsAny<Client>()), Times.Once);
            Assert.AreEqual(client.Email, email);
            Assert.AreEqual(client.Name, name);
            Assert.AreEqual(client.ActivationCode, activationCode);
        }
        
        [TestMethod]
        public void ActivateClient_ClientExisted_UseDbContextSaveReturnTrue()
        {
            CustomerDBContext.Invocations.Clear();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var email1 = "email1@mail.ru";
            var email2 = "email2@mail.ru";
            var name1 = "name1";
            var name2 = "name2";
            var passwordHashed1 = "542gythnfsli8";
            var passwordHashed2 = "111gythnfsli8";
            var activationCode1 = "12345qwerty78906yuiopsdfg";
            var activationCode2 = "56789qwerty78906yuiopsdfg";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate2 = DateTime.UtcNow.AddDays(-2);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);

            var data = new List<Client>
            {
                new Client { Id = id1, Name = name1, Email = email1, PasswordHash = passwordHashed1, ActivationCode = activationCode1, CreatedDate = createDate1, UpdatedDate = updateDate1, IsActive = true },
                new Client { Id = id2, Name = name2, Email = email2, PasswordHash = passwordHashed2, ActivationCode = activationCode2, CreatedDate = createDate2, UpdatedDate = updateDate2, IsActive = true }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Client>>();

            mockSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Client>()));

            CustomerDBContext.Setup(x => x.Client).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.Set<Client>()).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.SaveChanges()).Returns(1);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new ClientRepository(CustomerDBContextFactory.Object);

            var result = repository.ActivateClient(activationCode1);

            Assert.IsTrue(result);
            CustomerDBContext.Verify(x => x.Client, Times.Exactly(1));
            CustomerDBContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void ActivateClient_ClientNotExisted_UseDbContextReturnFalse()
        {
            CustomerDBContext.Invocations.Clear();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var email1 = "email1@mail.ru";
            var email2 = "email2@mail.ru";
            var name1 = "name1";
            var name2 = "name2";
            var passwordHashed1 = "542gythnfsli8";
            var passwordHashed2 = "111gythnfsli8";
            var activationCode1 = "12345qwerty78906yuiopsdfg";
            var activationCode2 = "56789qwerty78906yuiopsdfg";
            var activationCode3 = "56789qwerty12345yuiopsdfg";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate2 = DateTime.UtcNow.AddDays(-2);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);

            var data = new List<Client>
            {
                new Client { Id = id1, Name = name1, Email = email1, PasswordHash = passwordHashed1, ActivationCode = activationCode1, CreatedDate = createDate1, UpdatedDate = updateDate1, IsActive = true },
                new Client { Id = id2, Name = name2, Email = email2, PasswordHash = passwordHashed2, ActivationCode = activationCode2, CreatedDate = createDate2, UpdatedDate = updateDate2, IsActive = true }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Client>>();

            mockSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Client>()));

            CustomerDBContext.Setup(x => x.Client).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.Set<Client>()).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.SaveChanges()).Returns(1);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new ClientRepository(CustomerDBContextFactory.Object);

            var result = repository.ActivateClient(activationCode3);

            Assert.IsTrue(result == false);
            CustomerDBContext.Verify(x => x.Client, Times.Exactly(1));
            CustomerDBContext.Verify(x => x.SaveChanges(), Times.Never);
        }


        [TestMethod]
        public void UpdateGoogleAuthCode_ClientExisted_UseDbContextSaveReturnTrue()
        {
            CustomerDBContext.Invocations.Clear();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var email1 = "email1@mail.ru";
            var email2 = "email2@mail.ru";
            var name1 = "name1";
            var name2 = "name2";
            var passwordHashed1 = "542gythnfsli8";
            var passwordHashed2 = "111gythnfsli8";
            var activationCode1 = "12345qwerty78906yuiopsdfg";
            var activationCode2 = "56789qwerty78906yuiopsdfg";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate2 = DateTime.UtcNow.AddDays(-2);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);
            var authCode1 = Guid.NewGuid().ToString();
            var authCode2 = Guid.NewGuid().ToString();
            var authCode3 = Guid.NewGuid().ToString();

            var data = new List<Client>
            {
                new Client { Id = id1, Name = name1, Email = email1, PasswordHash = passwordHashed1, ActivationCode = activationCode1, CreatedDate = createDate1, UpdatedDate = updateDate1, IsActive = true, GoogleAuthCode = authCode1 },
                new Client { Id = id2, Name = name2, Email = email2, PasswordHash = passwordHashed2, ActivationCode = activationCode2, CreatedDate = createDate2, UpdatedDate = updateDate2, IsActive = true, GoogleAuthCode = authCode2 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Client>>();

            mockSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Client>()));

            CustomerDBContext.Setup(x => x.Client).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.Set<Client>()).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.SaveChanges()).Returns(1);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new ClientRepository(CustomerDBContextFactory.Object);

            var result = repository.UpdateGoogleAuthCode(id2, authCode3);

            Assert.IsTrue(result);
            CustomerDBContext.Verify(x => x.Client, Times.Exactly(1));
            CustomerDBContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void UpdateGoogleAuthCode_ClientNotExisted_UseDbContextReturnFalse()
        {
            CustomerDBContext.Invocations.Clear();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var email1 = "email1@mail.ru";
            var email2 = "email2@mail.ru";
            var name1 = "name1";
            var name2 = "name2";
            var passwordHashed1 = "542gythnfsli8";
            var passwordHashed2 = "111gythnfsli8";
            var activationCode1 = "12345qwerty78906yuiopsdfg";
            var activationCode2 = "56789qwerty78906yuiopsdfg";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate2 = DateTime.UtcNow.AddDays(-2);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);
            var authCode1 = Guid.NewGuid().ToString();
            var authCode2 = Guid.NewGuid().ToString();
            var authCode3 = Guid.NewGuid().ToString();

            var data = new List<Client>
            {
                new Client { Id = id1, Name = name1, Email = email1, PasswordHash = passwordHashed1, ActivationCode = activationCode1, CreatedDate = createDate1, UpdatedDate = updateDate1, IsActive = true, GoogleAuthCode = authCode1 },
                new Client { Id = id2, Name = name2, Email = email2, PasswordHash = passwordHashed2, ActivationCode = activationCode2, CreatedDate = createDate2, UpdatedDate = updateDate2, IsActive = true, GoogleAuthCode = authCode2 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Client>>();

            mockSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Client>()));

            CustomerDBContext.Setup(x => x.Client).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.Set<Client>()).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.SaveChanges()).Returns(1);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new ClientRepository(CustomerDBContextFactory.Object);

            var result = repository.UpdateGoogleAuthCode(Guid.NewGuid(), authCode3);

            Assert.IsTrue(result == false);
            CustomerDBContext.Verify(x => x.Client, Times.Exactly(1));
            CustomerDBContext.Verify(x => x.SaveChanges(), Times.Never);
        }

        [TestMethod]
        public void ActivateGoogleAuthCode_ClientExisted_UseDbContextSaveReturnTrue()
        {
            CustomerDBContext.Invocations.Clear();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var email1 = "email1@mail.ru";
            var email2 = "email2@mail.ru";
            var name1 = "name1";
            var name2 = "name2";
            var passwordHashed1 = "542gythnfsli8";
            var passwordHashed2 = "111gythnfsli8";
            var activationCode1 = "12345qwerty78906yuiopsdfg";
            var activationCode2 = "56789qwerty78906yuiopsdfg";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate2 = DateTime.UtcNow.AddDays(-2);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);

            var data = new List<Client>
            {
                new Client { Id = id1, Name = name1, Email = email1, PasswordHash = passwordHashed1, ActivationCode = activationCode1, CreatedDate = createDate1, UpdatedDate = updateDate1, IsActive = true, GoogleAuthActive = false },
                new Client { Id = id2, Name = name2, Email = email2, PasswordHash = passwordHashed2, ActivationCode = activationCode2, CreatedDate = createDate2, UpdatedDate = updateDate2, IsActive = true, GoogleAuthActive = false }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Client>>();

            mockSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Client>()));

            CustomerDBContext.Setup(x => x.Client).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.Set<Client>()).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.SaveChanges()).Returns(1);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new ClientRepository(CustomerDBContextFactory.Object);

            var result = repository.ActivateGoogleAuthCode(id2);

            Assert.IsTrue(result);
            CustomerDBContext.Verify(x => x.Client, Times.Exactly(1));
            CustomerDBContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void ActivateGoogleAuthCode_ClientNotExisted_UseDbContextReturnFalse()
        {
            CustomerDBContext.Invocations.Clear();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var email1 = "email1@mail.ru";
            var email2 = "email2@mail.ru";
            var name1 = "name1";
            var name2 = "name2";
            var passwordHashed1 = "542gythnfsli8";
            var passwordHashed2 = "111gythnfsli8";
            var activationCode1 = "12345qwerty78906yuiopsdfg";
            var activationCode2 = "56789qwerty78906yuiopsdfg";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate2 = DateTime.UtcNow.AddDays(-2);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);

            var data = new List<Client>
            {
                new Client { Id = id1, Name = name1, Email = email1, PasswordHash = passwordHashed1, ActivationCode = activationCode1, CreatedDate = createDate1, UpdatedDate = updateDate1, IsActive = true, GoogleAuthActive = false },
                new Client { Id = id2, Name = name2, Email = email2, PasswordHash = passwordHashed2, ActivationCode = activationCode2, CreatedDate = createDate2, UpdatedDate = updateDate2, IsActive = true, GoogleAuthActive = false }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Client>>();

            mockSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Client>()));

            CustomerDBContext.Setup(x => x.Client).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.Set<Client>()).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.SaveChanges()).Returns(1);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new ClientRepository(CustomerDBContextFactory.Object);

            var result = repository.ActivateGoogleAuthCode(Guid.NewGuid());

            Assert.IsTrue(result == false);
            CustomerDBContext.Verify(x => x.Client, Times.Exactly(1));
            CustomerDBContext.Verify(x => x.SaveChanges(), Times.Never);
        }

        [TestMethod]
        public void DeactivateGoogleAuthCode_ClientExisted_UseDbContextSaveReturnTrue()
        {
            CustomerDBContext.Invocations.Clear();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var email1 = "email1@mail.ru";
            var email2 = "email2@mail.ru";
            var name1 = "name1";
            var name2 = "name2";
            var passwordHashed1 = "542gythnfsli8";
            var passwordHashed2 = "111gythnfsli8";
            var activationCode1 = "12345qwerty78906yuiopsdfg";
            var activationCode2 = "56789qwerty78906yuiopsdfg";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate2 = DateTime.UtcNow.AddDays(-2);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);

            var data = new List<Client>
            {
                new Client { Id = id1, Name = name1, Email = email1, PasswordHash = passwordHashed1, ActivationCode = activationCode1, CreatedDate = createDate1, UpdatedDate = updateDate1, IsActive = true, GoogleAuthActive = false },
                new Client { Id = id2, Name = name2, Email = email2, PasswordHash = passwordHashed2, ActivationCode = activationCode2, CreatedDate = createDate2, UpdatedDate = updateDate2, IsActive = true, GoogleAuthActive = false }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Client>>();

            mockSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Client>()));

            CustomerDBContext.Setup(x => x.Client).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.Set<Client>()).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.SaveChanges()).Returns(1);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new ClientRepository(CustomerDBContextFactory.Object);

            var result = repository.DeactivateGoogleAuthCode(id2);

            Assert.IsTrue(result);
            CustomerDBContext.Verify(x => x.Client, Times.Exactly(1));
            CustomerDBContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void DeactivateGoogleAuthCode_ClientNotExisted_UseDbContextReturnFalse()
        {
            CustomerDBContext.Invocations.Clear();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var email1 = "email1@mail.ru";
            var email2 = "email2@mail.ru";
            var name1 = "name1";
            var name2 = "name2";
            var passwordHashed1 = "542gythnfsli8";
            var passwordHashed2 = "111gythnfsli8";
            var activationCode1 = "12345qwerty78906yuiopsdfg";
            var activationCode2 = "56789qwerty78906yuiopsdfg";
            var createDate1 = DateTime.UtcNow.AddDays(-1);
            var createDate2 = DateTime.UtcNow.AddDays(-2);
            var updateDate1 = DateTime.UtcNow.AddMinutes(-3);
            var updateDate2 = DateTime.UtcNow.AddMinutes(-6);

            var data = new List<Client>
            {
                new Client { Id = id1, Name = name1, Email = email1, PasswordHash = passwordHashed1, ActivationCode = activationCode1, CreatedDate = createDate1, UpdatedDate = updateDate1, IsActive = true, GoogleAuthActive = false },
                new Client { Id = id2, Name = name2, Email = email2, PasswordHash = passwordHashed2, ActivationCode = activationCode2, CreatedDate = createDate2, UpdatedDate = updateDate2, IsActive = true, GoogleAuthActive = false }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Client>>();

            mockSet.As<IQueryable<Client>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Client>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Client>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Client>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<Client>()));

            CustomerDBContext.Setup(x => x.Client).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.Set<Client>()).Returns(mockSet.Object);
            CustomerDBContext.Setup(x => x.SaveChanges()).Returns(1);
            CustomerDBContextFactory.Setup(x => x.CreateDBContext()).Returns(CustomerDBContext.Object);

            var repository = new ClientRepository(CustomerDBContextFactory.Object);

            var result = repository.DeactivateGoogleAuthCode(Guid.NewGuid());

            Assert.IsTrue(result == false);
            CustomerDBContext.Verify(x => x.Client, Times.Exactly(1));
            CustomerDBContext.Verify(x => x.SaveChanges(), Times.Never);
        }
    }
}
