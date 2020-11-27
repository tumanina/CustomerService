using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CustomerService.Repositories.DAL;
using CustomerService.Repositories.Entities;
using CustomerService.Repositories.Interfaces;

namespace CustomerService.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ICustomerDBContext _dbContext;

        public TokenRepository(ICustomerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Token> GetTokens(Guid clientId, bool onlyActive = false)
        {
            return _dbContext.Token.AsNoTracking().Where(t => t.ClientId == clientId && (onlyActive == false || t.IsActive)).ToList();
        }

        public Token CreateToken(Guid clientId, string ip, string authMethod)
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

            var tokens = _dbContext.Set<Token>();
            tokens.Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public Token UpdateActive(Guid id, bool isActive)
        {
            var result = _dbContext.Token.SingleOrDefault(b => b.Id == id);
            if (result != null)
            {
                result.IsActive = isActive;
                _dbContext.SaveChanges();
            }
            else
            {
                return null;
            }

            return result;
        }
    }
}
