namespace CustomerService.Api.Areas.V1.Models
{
    public class CreateSessionKeyRequest
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string IP { get; set; }
    }
}
