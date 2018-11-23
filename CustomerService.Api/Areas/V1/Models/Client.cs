using System;

namespace CustomerService.Api.Areas.V1.Models
{
    public class Client
    {
        public Client(Business.Models.Client entity)
        {
            Id = entity.Id;
            IsActive = entity.IsActive;
            Email = entity.Email;
            Name = entity.Name;
            CreatedDate = entity.CreatedDate;
            UpdatedDate = entity.UpdatedDate;
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
