using System;

namespace CustomerService.Business.Models
{
    public class Token
    {
        public Token()
        {
        }

        public Token(Repositories.Entities.Token entity)
        {
            Id = entity.Id;
            ClientId = entity.ClientId;
            Value = entity.Value;
            AuthenticationMethod = entity.AuthenticationMethod;
            IP = entity.IP;
            IsActive = entity.IsActive;
            CreatedDate = entity.CreatedDate;
        }

        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public string Value { get; set; }
        public string AuthenticationMethod { get; set; }
        public string IP { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}