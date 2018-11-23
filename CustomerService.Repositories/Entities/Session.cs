using System;

namespace CustomerService.Repositories.Entities
{
    public class Session
    {
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
