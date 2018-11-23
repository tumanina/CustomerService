namespace CustomerService.Api.Areas.V1.Models
{
    public class CreateTokenRequest
    {
        public string IP { get; set; }
        public string AuthMethod { get; set; }
    }
}