namespace CustomerService.Business.Models
{
    public enum AuthenticationMethod
    {
        None = 0,
        SessionKey = 1,
        HMACSHA256 = 2,
        RSASignature = 3,
        ECDSASignature = 4,
    }
}
