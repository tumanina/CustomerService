using CustomerService.Core;
using CustomerService.Repositories.Entities;
using System;

namespace CustomerService.Repositories
{
    public interface ISessionRepository
    {
        PagedList<Session> GetSessions(Guid clientId, bool onlyActive, int pageNumber = 1, int pageSize = 20);
        Session GetSession(Guid clientId, Guid id);
        Session GetSessionByKey(string key);
        Session CreateSession(Guid clientId, string ip, int interval, bool confirmed);
        bool ConfirmSession(Guid clientId, Guid id);
        bool DisableSession(Guid clientId, Guid id);
    }
}
