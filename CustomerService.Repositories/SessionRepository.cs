using System;
using System.Linq;
using CustomerService.Core;
using CustomerService.Repositories.DAL;
using CustomerService.Repositories.Entities;
using CustomerService.Repositories.Interfaces;

namespace CustomerService.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ICustomerDBContext _dbContext;

        public SessionRepository(ICustomerDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public PagedList<Session> GetSessions(Guid clientId, bool onlyActive = false, int pageNumber = 1, int pageSize = 20)
        {
            return _dbContext.Session.Where(t => t.ClientId == clientId && (!onlyActive || (t.Enabled && t.ExpiredDate > DateTime.UtcNow))).ToPagedList(pageNumber, pageSize);
        }

        public Session GetSession(Guid clientId, Guid id)
        {
            return _dbContext.Session.FirstOrDefault(t => t.ClientId == clientId && t.Id == id);
        }

        public Session GetSessionByKey(string key)
        {
            return _dbContext.Session.FirstOrDefault(t => t.SessionKey == key);
        }

        public Session CreateSession(Guid clientId, string ip, int interval, bool confirmed)
        {
            var currentDate = DateTime.UtcNow;

            var entity = new Session
            {
                ClientId = clientId,
                IP = ip,
                SessionKey = Guid.NewGuid().ToString(),
                Confirmed = confirmed,
                Enabled = true,
                CreatedDate = currentDate,
                ExpiredDate = currentDate.AddSeconds(interval),
                UpdatedDate = currentDate
            };

            var sessions = _dbContext.Set<Session>();
            sessions.Add(entity);
            _dbContext.SaveChanges();

            return GetSession(clientId, entity.Id);
        }

        public bool ConfirmSession(Guid clientId, Guid id)
        {
            var result = _dbContext.Session.SingleOrDefault(b => b.ClientId == clientId && b.Id == id);

            if (result == null)
            {
                return false;
            }

            result.Confirmed = true;
            result.UpdatedDate = DateTime.UtcNow;
            _dbContext.SaveChanges();

            return true;
        }

        public bool DisableSession(Guid clientId, Guid id)
        {
            var result = _dbContext.Session.SingleOrDefault(b => b.ClientId == clientId && b.Id == id);

            if (result == null)
            {
                return false;
            }

            result.Enabled = false;
            result.UpdatedDate = DateTime.UtcNow;
            _dbContext.SaveChanges();

            return true;
        }
    }
}
