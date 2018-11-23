using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CustomerService.Repositories.DAL;
using CustomerService.Repositories.Entities;

namespace CustomerService.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ICustomerDBContextFactory _factory;

        public TokenRepository(ICustomerDBContextFactory factory)
        {
            _factory = factory;
        }

        public IEnumerable<Token> GetTokens(Guid clientId, bool onlyActive = false)
        {
            using (var context = _factory.CreateDBContext())
            {
                return context.Token.AsNoTracking().Where(t => t.ClientId == clientId && (onlyActive == false || t.IsActive)).ToList();
            }
        }

        public Token CreateToken(Guid clientId, string ip, string authMethod)
        {
            using (var context = _factory.CreateDBContext())
            {
                var currentDate = DateTime.UtcNow;

                var entity = new Token
                {
                    ClientId = clientId,
                    Value = Guid.NewGuid().ToString(),
                    CreatedDate = currentDate,
                    AuthenticationMethod = authMethod,
                    IP = ip,
                    IsActive = false
                };

                var tokens = context.Set<Token>();
                tokens.Add(entity);
                context.SaveChanges();

                return entity;
            }
        }

        public Token UpdateActive(Guid id, bool isActive)
        {
            using (var context = _factory.CreateDBContext())
            {
                var result = context.Token.SingleOrDefault(b => b.Id == id);
                if (result != null)
                {
                    result.IsActive = isActive;
                    context.SaveChanges();
                }
                else
                {
                    return null;
                }

                return result;
            }
        }
    }
}
