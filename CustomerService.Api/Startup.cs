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
using Microsoft.OpenApi.Models;
using CustomerService.Repositories.Interfaces;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
            services.AddDbContext<CustomerDBContext>(options => options.UseSqlServer(connectionString));

            services.AddTransient<ICustomerDBContext, CustomerDBContext>();
            services.AddTransient<ISenderProcessor, SenderProcessor>();
            services.AddTransient<ISessionRepository, SessionRepository>();
            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<ISessionService, SessionService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddSingleton<IGoogleAuthService, GoogleAuthService>();
            services.AddSingleton<IEmailService, EmailService>();

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });
            services.AddSwaggerGenNewtonsoftSupport();

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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IreckonU Self-Service Reports", Version = "v1" });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }
                        },
                        new List<string>()
                    }
                });

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header Token. Enter : \"Bearer YourTokenHere\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
            });

            /*NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            NLog.LogManager.LoadConfiguration("nlog.config");
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddNLog(Configuration);
            });*/
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer Service V1");
                c.DocExpansion(DocExpansion.None);
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}