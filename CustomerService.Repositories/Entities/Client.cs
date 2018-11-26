using System;

namespace CustomerService.Repositories.Entities
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public string ActivationCode { get; set; }
        public bool IsActive { get; set; }
        public string GoogleAuthCode { get; set; }
        public bool? GoogleAuthActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}