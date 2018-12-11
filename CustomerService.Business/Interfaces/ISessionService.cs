using System;
using CustomerService.Business.Models;
using CustomerService.Core;

namespace CustomerService.Business
{
    public interface ISessionService
    {
        PagedList<Session> GetSessions(Guid clientId, bool onlyActive, int pageNumber = 1, int pageSize = 20);
        Session GetSession(Guid clientId, Guid sessionId);
        Session GetSessionByKey(string sessionKey);
        bool? IsSessionConfirmRequired(Guid clientId, Guid sessionId);
        Session CreateSession(string name, string password, string ip, int interval);
        bool ConfirmSession(Guid clientId, Guid sessionId, string oneTimePassword);
        bool DisableSession(Guid clientId, Guid sessionId);
    }
}
