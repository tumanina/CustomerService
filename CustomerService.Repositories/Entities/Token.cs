using System;

namespace CustomerService.Repositories.Entities
{
    public class Token
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public string Value { get; set; }
        public string AuthenticationMethod { get; set; }
        public string IP { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}