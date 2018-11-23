using System;
using System.Collections.Generic;
using CustomerService.Business.Models;

namespace CustomerService.Business
{
    public interface ISessionService
    {
        IEnumerable<Session> GetSessions(Guid clientId, bool onlyActive);
        Session GetSession(Guid clientId, Guid sessionId);
        Session GetSessionByKey(string sessionKey);
        bool? IsSessionConfirmRequired(Guid clientId, Guid sessionId);
        Session CreateSession(string name, string password, string ip, int interval);
        bool ConfirmSession(Guid clientId, Guid sessionId, string oneTimePassword);
        bool DisableSession(Guid clientId, Guid sessionId);
    }
}
