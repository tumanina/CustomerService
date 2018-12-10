using CustomerService.Repositories.DAL;
using CustomerService.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CustomerService.Business;
using Swashbuckle.AspNetCore.Swagger;
using NLog.Extensions.Logging;
using NLog.Web;
using System.Collections.Generic;
using CustomerService.Configuration;
using CustomerService.Business.MessageBroker;
using RabbitMQ.Client;

using System.IO;
using System;
using System.Reflection;

namespace CustomerService.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("CustomerDBConnectionString");

            var dbOptions = new DbContextOptionsBuilder<CustomerDBContext>();
            dbOptions.UseSqlServer(connectionString);
            services.AddSingleton<ICustomerDBContextFactory>(t => new CustomerDBContextFactory(dbOptions));
            services.AddSingleton<ICustomerDBContext, CustomerDBContext>();
            services.AddSingleton<ISenderProcessor, SenderProcessor>();
            services.AddSingleton<ISessionRepository, SessionRepository>();
            services.AddSingleton<IClientRepository, ClientRepository>();
            services.AddSingleton<ITokenRepository, TokenRepository>();
            services.AddSingleton<ISessionService, SessionService>();
            services.AddSingleton<IClientService, ClientService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IGoogleAuthService, GoogleAuthService>();
            services.AddSingleton<IEmailService, EmailService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Customer API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "Authorization header Example: \"Authorization: {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } } });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddMvc();

            var senders = Configuration.GetSection("Senders").Get<IEnumerable<SenderConfiguration>>();

            if (senders != null)
            {
                foreach (var sender in senders)
                {
                    services.AddSingleton<ISender>(t => new Sender(new ConnectionFactory
                    {
                        HostName = sender.Server.Host,
                        UserName = sender.Server.UserName,
                        Password = sender.Server.Password
                    },
                        sender.Type,
                        sender.QueueName,
                        sender.ExchangeName));
                }
            }

            var serviceProvider = services.BuildServiceProvider();
            services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Warning));
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            NLog.LogManager.LoadConfiguration("nlog.config");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer Service V1");
            });

            loggerFactory.AddNLog();
            env.ConfigureNLog("nlog.config");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{id}");
            });
        }
    }
}