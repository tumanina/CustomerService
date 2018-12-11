using System;
using CustomerService.Business.Models;
using CustomerService.Core;
using CustomerService.Repositories;

namespace CustomerService.Business
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IClientService _clientService;

        public SessionService(ISessionRepository sessionRepository, IClientService clientService)
        {
            _sessionRepository = sessionRepository;
            _clientService = clientService;
        }

        public PagedList<Session> GetSessions(Guid clientId, bool onlyActive = false, int pageNumber = 1, int pageSize = 20)
        {
            var sessions = _sessionRepository.GetSessions(clientId, onlyActive, pageNumber, pageSize);

            return sessions.Convert(t => new Session(t));
        }

        public Session GetSession(Guid clientId, Guid sessionId)
        {
            var session = _sessionRepository.GetSession(clientId, sessionId);

            return session == null ? null : new Session(session);
        }

        public Session GetSessionByKey(string sessionKey)
        {
            var session = _sessionRepository.GetSessionByKey(sessionKey);

            return session == null ? null : new Session(session);
        }

        public bool? IsSessionConfirmRequired(Guid clientId, Guid sessionId)
        {
            var session = _sessionRepository.GetSession(clientId, sessionId);

            return (session == null) ? (bool?)null : !session.Confirmed;
        }

        public Session CreateSession(string name, string password, string ip, int interval)
        {
            var client = _clientService.Authentification(name, password);

            if (client == null)
            {
                return null;
            }

            var confirmed = (client.GoogleAuthCode == null);

            var createdSession = _sessionRepository.CreateSession(client.Id, ip, interval, confirmed);

            return createdSession == null ? null : new Session(createdSession);
        }

        public bool ConfirmSession(Guid clientId, Guid sessionId, string oneTimePassword)
        {
            var isCorrectPIN = _clientService.ValidateClientByGoogleAuth(clientId, oneTimePassword);

            if (isCorrectPIN)
            {
                return _sessionRepository.ConfirmSession(clientId, sessionId);
            }

            return false;
        }

        public bool DisableSession(Guid clientId, Guid sessionId)
        {
            return _sessionRepository.DisableSession(clientId, sessionId);
        }
    }
}
