using CustomerService.Repositories.Entities;
using System;

namespace CustomerService.Repositories.Interfaces
{
    public interface IClientRepository
    {
        Client GetClient(Guid id);
        Client GetClientByName(string name);
        Client GetClientByEmail(string email);
        Client CreateClient(string email, string name, string passwordHash, string activateCode);
        Client UpdateClient(Guid id, string email, string name);
        bool ActivateClient(string activationCode);
        bool UpdateGoogleAuthCode(Guid id, string authCode);
        bool ActivateGoogleAuthCode(Guid id);
        bool DeactivateGoogleAuthCode(Guid id);
    }
}
