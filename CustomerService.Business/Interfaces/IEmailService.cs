namespace CustomerService.Business
{
    public interface IEmailService
    {
        void SendEmail(string email, string title, string body);
    }
}
