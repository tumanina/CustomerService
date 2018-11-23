using System;

namespace CustomerService.Api.Areas.V1.Models
{
    public class Token
    {
        public Token(Business.Models.Token token)
        {
            Id = token.Id;
            ClientId = token.ClientId;
            Value = token.Value;
            AuthenticationMethod = token.AuthenticationMethod;
            IP = token.IP;
            IsActive = token.IsActive;
            CreatedDate = token.CreatedDate;
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