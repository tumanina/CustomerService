using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CustomerService.Repositories.Entities;
using CustomerService.Repositories.DAL;

namespace CustomerService.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ICustomerDBContextFactory _factory;

        public ClientRepository(ICustomerDBContextFactory factory)
        {
            _factory = factory;
        }

        public Client GetClient(Guid id)
        {
            using (var context = _factory.CreateDBContext())
            {
                return context.Client.AsNoTracking().FirstOrDefault(t => t.Id == id);
            }
        }

        public Client GetClientByName(string name)
        {
            using (var context = _factory.CreateDBContext())
            {
                return context.Client.AsNoTracking().FirstOrDefault(t => t.Name == name);
            }
        }

        public Client GetClientByEmail(string email)
        {
            using (var context = _factory.CreateDBContext())
            {
                return context.Client.AsNoTracking().FirstOrDefault(t => t.Email == email);
            }
        }

        public Client CreateClient(string email, string name, string passwordHash, string activationCode)
        {
            using (var context = _factory.CreateDBContext())
            {
                var clients = context.Set<Client>();

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
                context.SaveChanges();

                return client;
            }
        }

        public Client UpdateClient(Guid id, string email, string name)
        {
            using (var context = _factory.CreateDBContext())
            {
                var result = context.Client.SingleOrDefault(b => b.Id == id);
                if (result != null)
                {
                    result.Name = name;
                    result.Email = email;
                    result.UpdatedDate = DateTime.UtcNow;
                    context.SaveChanges();
                }
                else
                {
                    return null;
                }

                return result;
            }
        }

        public bool ActivateClient(string activateCode)
        {
            using (var context = _factory.CreateDBContext())
            {
                var result = context.Client.SingleOrDefault(b => b.ActivationCode == activateCode);
                if (result != null)
                {
                    result.IsActive = true;
                    result.UpdatedDate = DateTime.UtcNow;
                    context.SaveChanges();
                }
                else
                {
                    return false;
                }

                return true;
            }
        }

        public bool UpdateGoogleAuthCode(Guid id, string authCode)
        {
            using (var context = _factory.CreateDBContext())
            {
                var result = context.Client.SingleOrDefault(b => b.Id == id);
                if (result != null)
                {
                    result.GoogleAuthCode = authCode;
                    result.GoogleAuthActive = false;
                    result.UpdatedDate = DateTime.UtcNow;
                    context.SaveChanges();
                }
                else
                {
                    return false;
                }

                return true;
            }
        }

        public bool ActivateGoogleAuthCode(Guid id)
        {
            using (var context = _factory.CreateDBContext())
            {
                var result = context.Client.SingleOrDefault(b => b.Id == id);
                if (result != null)
                {
                    result.GoogleAuthActive = true;
                    result.UpdatedDate = DateTime.UtcNow;
                    context.SaveChanges();
                }
                else
                {
                    return false;
                }

                return true;
            }
        }

        public bool DeactivateGoogleAuthCode(Guid id)
        {
            using (var context = _factory.CreateDBContext())
            {
                var result = context.Client.SingleOrDefault(b => b.Id == id);
                if (result != null)
                {
                    result.GoogleAuthActive = false;
                    result.UpdatedDate = DateTime.UtcNow;
                    context.SaveChanges();
                }
                else
                {
                    return false;
                }

                return true;
            }
        }
    }
}
