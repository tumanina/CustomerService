using System;
using System.Linq;
using CustomerService.Core;
using CustomerService.Repositories.DAL;
using CustomerService.Repositories.Entities;

namespace CustomerService.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ICustomerDBContextFactory _factory;

        public SessionRepository(ICustomerDBContextFactory factory)
        {
            _factory = factory;
        }

        public PagedList<Session> GetSessions(Guid clientId, bool onlyActive = false, int pageNumber = 1, int pageSize = 20)
        {
            using (var context = _factory.CreateDBContext())
            {
                return context.Session.Where(t => t.ClientId == clientId && (!onlyActive || (t.Enabled && t.ExpiredDate > DateTime.UtcNow))).ToPagedList(pageNumber, pageSize);
            }
        }

        public Session GetSession(Guid clientId, Guid id)
        {
            using(var context = _factory.CreateDBContext())
            {
                return context.Session.FirstOrDefault(t => t.ClientId == clientId && t.Id == id);
            }
        }

        public Session GetSessionByKey(string key)
        {
            using (var context = _factory.CreateDBContext())
            {
                return context.Session.FirstOrDefault(t => t.SessionKey == key);
            }
        }

        public Session CreateSession(Guid clientId, string ip, int interval, bool confirmed)
        {
            using (var context = _factory.CreateDBContext())
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

                var sessions = context.Set<Session>();
                sessions.Add(entity);
                context.SaveChanges();

                return GetSession(clientId, entity.Id);
            }
        }

        public bool ConfirmSession(Guid clientId, Guid id)
        {
            using (var context = _factory.CreateDBContext())
            {
                var result = context.Session.SingleOrDefault(b => b.ClientId == clientId && b.Id == id);

                if (result == null)
                {
                    return false;
                }

                result.Confirmed = true;
                result.UpdatedDate = DateTime.UtcNow;
                context.SaveChanges();

                return true;
            }
        }

        public bool DisableSession(Guid clientId, Guid id)
        {
            using (var context = _factory.CreateDBContext())
            {
                var result = context.Session.SingleOrDefault(b => b.ClientId == clientId && b.Id == id);

                if (result == null)
                {
                    return false;
                }

                result.Enabled = false;
                result.UpdatedDate = DateTime.UtcNow;
                context.SaveChanges();

                return true;
            }
        }
    }
}
