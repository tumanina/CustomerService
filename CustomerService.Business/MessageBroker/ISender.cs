namespace CustomerService.Business.MessageBroker
{
    public interface ISender
    {
        string Type { get; }

        void SendMessage(string message);
    }
}
