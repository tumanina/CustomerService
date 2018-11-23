using System;

namespace CustomerService.Api.Areas.V1.Models
{
    public class Session
    {
        public Session(Business.Models.Session entity)
        {
            Id = entity.Id;
            ClientId = entity.ClientId;
            SessionKey = entity.SessionKey;
            IP = entity.IP;
            Confirmed = entity.Confirmed;
            Enabled = entity.Enabled;
            CreatedDate = entity.CreatedDate;
            UpdatedDate = entity.UpdatedDate;
            ExpiredDate = entity.ExpiredDate;
        }

        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public string SessionKey { get; set; }
        public string IP { get; set; }
        public bool Confirmed { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
