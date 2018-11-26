using CustomerService.Business.Models;
using Google.Authenticator;

namespace CustomerService.Business
{
    public class GoogleAuthService : IGoogleAuthService
    {
        public GoogleAuthCode Generate(string email, string authCode)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var setupInfo = tfa.GenerateSetupCode("CustomerService", email, authCode, 300, 300);

            var qrCodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            var manualEntrySetupCode = setupInfo.ManualEntryKey;

            return new GoogleAuthCode
            {
                QRCodeImageUrl = qrCodeImageUrl,
                SetupCode = manualEntrySetupCode
            };
        }

        public bool Validate(string secretKey, string code)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            return tfa.ValidateTwoFactorPIN(secretKey, code);
        }
    }
}
