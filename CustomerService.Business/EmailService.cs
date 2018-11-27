using CustomerService.Business.MessageBroker;
using Newtonsoft.Json;

namespace CustomerService.Business
{
    public class EmailService : IEmailService
    {
        private readonly ISenderProcessor _senderProcessor;

        public EmailService(ISenderProcessor senderProcessor)
        {
            _senderProcessor = senderProcessor;
        }

        public void SendEmail(string email, string title, string body)
        {
            var message = new EmailMessage { Title = title, Body = body, Email = email };
            _senderProcessor.SendMessage("Email", JsonConvert.SerializeObject(message));
        }

        private class EmailMessage
        {
            public string Title { get; set; }
            public string Body { get; set; }
            public string Email { get; set; }
        }
    }
}
