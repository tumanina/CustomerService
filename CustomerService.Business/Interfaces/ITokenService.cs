using CustomerService.Business.Models;
using System;
using System.Collections.Generic;

namespace CustomerService.Business
{
    public interface ITokenService
    {
        IEnumerable<Token> GetTokens(Guid clientId, bool onlyActive);
        Token CreateToken(Guid clientId, string ip, string authMethod);
        Token UpdateActive(Guid id, bool isActive);
    }
}
