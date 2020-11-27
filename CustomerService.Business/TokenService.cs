using System;
using System.Collections.Generic;
using System.Linq;
using CustomerService.Business.Models;
using CustomerService.Repositories.Interfaces;

namespace CustomerService.Business
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _tokenRepository;

        public TokenService(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public Token CreateToken(Guid clientId, string ip, string authMethod)
        {
            var createdToken = _tokenRepository.CreateToken(clientId, ip, authMethod);

            return createdToken == null ? null : new Token(createdToken);
        }

        public IEnumerable<Token> GetTokens(Guid clientId, bool onlyActive = false)
        {
            var tokens = _tokenRepository.GetTokens(clientId, onlyActive);

            return !tokens.Any() ? new List<Token>() : tokens.Select(t => new Token(t));
        }

        public Token UpdateActive(Guid id, bool isActive)
        {
            var updatedToken = _tokenRepository.UpdateActive(id, isActive);

            return updatedToken == null ? null : new Token(updatedToken);
        }
    }
}