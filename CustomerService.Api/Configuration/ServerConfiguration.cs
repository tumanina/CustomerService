﻿namespace CustomerService.Configuration
{
    public class ServerConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int TimeToWait { get; set; }
    }
}