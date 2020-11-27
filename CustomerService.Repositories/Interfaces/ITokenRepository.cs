using System;
using System.Collections.Generic;
using CustomerService.Repositories.Entities;

namespace CustomerService.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        IEnumerable<Token> GetTokens(Guid clientId, bool onlyActive);
        Token CreateToken(Guid clientId, string ip, string authMethod);
        Token UpdateActive(Guid id, bool isActive);
    }
}
