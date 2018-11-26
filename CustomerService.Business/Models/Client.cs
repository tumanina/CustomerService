using System;

namespace CustomerService.Business.Models
{
    public class Client
    {
        public Client()
        {

        }

        public Client(Repositories.Entities.Client entity)
        {
            Id = entity.Id;
            IsActive = entity.IsActive;
            Email = entity.Email;
            Name = entity.Name;
            GoogleAuthCode = entity.GoogleAuthCode;
            GoogleAuthActive = entity.GoogleAuthActive;
            PasswordHash = entity.PasswordHash;
            ActivationCode = entity.ActivationCode;
            CreatedDate = entity.CreatedDate;
            UpdatedDate = entity.UpdatedDate;
        }

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
