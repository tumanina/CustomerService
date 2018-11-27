namespace CustomerService.Configuration
{
    public class SenderConfiguration
    {
        public string Type { get; set; }
        public ServerConfiguration Server { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
    }
}
