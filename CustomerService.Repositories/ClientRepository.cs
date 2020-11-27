using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CustomerService.Repositories.Entities;
using CustomerService.Repositories.DAL;
using CustomerService.Repositories.Interfaces;

namespace CustomerService.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ICustomerDBContext _dbContext;

        public ClientRepository(ICustomerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Client GetClient(Guid id)
        {
            return _dbContext.Client.AsNoTracking().FirstOrDefault(t => t.Id == id);
        }

        public Client GetClientByName(string name)
        {
            return _dbContext.Client.AsNoTracking().FirstOrDefault(t => t.Name == name);
        }

        public Client GetClientByEmail(string email)
        {
            return _dbContext.Client.AsNoTracking().FirstOrDefault(t => t.Email == email);
        }

        public Client CreateClient(string email, string name, string passwordHash, string activationCode)
        {
            var clients = _dbContext.Set<Client>();

            var client = new Client
            {
                Name = name,
                Email = email,
                PasswordHash = passwordHash,
                ActivationCode = activationCode,
                IsActive = false,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            clients.Add(client);
            _dbContext.SaveChanges();

            return client;
        }

        public Client UpdateClient(Guid id, string email, string name)
        {
            var result = _dbContext.Client.SingleOrDefault(b => b.Id == id);
            if (result != null)
            {
                result.Name = name;
                result.Email = email;
                result.UpdatedDate = DateTime.UtcNow;
                _dbContext.SaveChanges();
            }
            else
            {
                return null;
            }

            return result;
        }

        public bool ActivateClient(string activateCode)
        {
            var result = _dbContext.Client.SingleOrDefault(b => b.ActivationCode == activateCode);
            if (result != null)
            {
                result.IsActive = true;
                result.UpdatedDate = DateTime.UtcNow;
                _dbContext.SaveChanges();
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool UpdateGoogleAuthCode(Guid id, string authCode)
        {
            var result = _dbContext.Client.SingleOrDefault(b => b.Id == id);
            if (result != null)
            {
                result.GoogleAuthCode = authCode;
                result.GoogleAuthActive = false;
                result.UpdatedDate = DateTime.UtcNow;
                _dbContext.SaveChanges();
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool ActivateGoogleAuthCode(Guid id)
        {
            var result = _dbContext.Client.SingleOrDefault(b => b.Id == id);
            if (result != null)
            {
                result.GoogleAuthActive = true;
                result.UpdatedDate = DateTime.UtcNow;
                _dbContext.SaveChanges();
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool DeactivateGoogleAuthCode(Guid id)
        {
            var result = _dbContext.Client.SingleOrDefault(b => b.Id == id);
            if (result != null)
            {
                result.GoogleAuthActive = false;
                result.UpdatedDate = DateTime.UtcNow;
                _dbContext.SaveChanges();
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
