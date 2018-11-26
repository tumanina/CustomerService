using CustomerService.Business.Models;
using System;

namespace CustomerService.Business
{
    public interface IClientService
    {
        bool CheckNameAvailability (string name);
        bool CheckEmailAvailability(string email);
        Client GetClient(Guid id);
        bool ValidateClientByGoogleAuth(Guid id, string oneTimePassword);
        Client Authentification(string name, string password);
        Client CreateClient(string email, string name, string password);
        Client UpdateClient(Guid id, string email, string name);
        bool ActivateClient(string activationCode);
        GoogleAuthCode CreateGoogleAuthCode(Guid id);
        bool? SetGoogleAuthCode(Guid id, string oneTimePassword);
        bool? DeactivateGoogleAuthCode(Guid id, string oneTimePassword);
    }
}
