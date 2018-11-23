using CustomerService.Repositories.Entities;
using System;
using System.Collections.Generic;

namespace CustomerService.Repositories
{
    public interface ISessionRepository
    {
        IEnumerable<Session> GetSessions(Guid clientId, bool onlyActive);
        Session GetSession(Guid clientId, Guid id);
        Session GetSessionByKey(string key);
        Session CreateSession(Guid clientId, string ip, int interval, bool confirmed);
        bool ConfirmSession(Guid clientId, Guid id);
        bool DisableSession(Guid clientId, Guid id);
    }
}
