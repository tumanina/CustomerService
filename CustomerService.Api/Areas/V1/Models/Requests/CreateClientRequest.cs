namespace CustomerService.Api.Areas.V1.Models
{
    public class CreateClientRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
