using CustomerService.Business.Models;

namespace CustomerService.Business
{
    public interface IGoogleAuthService
    {
        GoogleAuthCode Generate(string email, string authCode);
        bool Validate(string secretKey, string code);
    }
}
