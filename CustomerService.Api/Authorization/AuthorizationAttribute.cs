using System;
using System.Collections.Generic;
using CustomerService.Api.Areas.V1.Controllers;
using CustomerService.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CustomerService.Api.Authorization
{
    public class ClientAuthorizationAttribute : TypeFilterAttribute
    {
        public ClientAuthorizationAttribute() : base(typeof(ClientAuthorizationFilter))
        {
        }
    }

    public class ClientAuthorizationFilter : IAuthorizationFilter
    {
        private readonly ILogger<ClientAuthorizationFilter> _logger;
        private readonly ISessionService _sessionService;

        public ClientAuthorizationFilter(ISessionService sessionService, ILogger<ClientAuthorizationFilter> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var message = string.Empty;
            var statusCode = 200;

            try
            {
                if (context.HttpContext.Items.TryGetValue("clientId", out var clientId))
                {
                    return;
                }

                var authorization = context.HttpContext.Request.Headers["Authorization"].ToString();

                if (string.IsNullOrEmpty(authorization))
                {
                    statusCode = 401;
                    context.Result = new UnauthorizedResult();
                    message = "Authorize header is empty.";
                }
                else
                {
                    var sessionKey = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                        ? authorization.Substring("Bearer ".Length).Trim()
                        : authorization;

                    var session = _sessionService.GetSessionByKey(sessionKey);

                    if (session == null)
                    {
                        statusCode = 401;
                    }
                    else
                    {
                        if (session.ExpiredDate < DateTime.UtcNow)
                        {
                            statusCode = 401;
                            message = "Session expired.";
                        }

                        if (session.Confirmed == false || session.Enabled == false)
                        {
                            statusCode = 401;
                            message = "Session mst be confirmed and enabled.";
                        }
                    }

                    if (statusCode == 200)
                    {
                        context.HttpContext.Items.Add("clientId", session.ClientId);
                    }
                }
            }
            catch (Exception ex)
            {
                statusCode = 500;
                message = ex.InnerMessage();
                _logger.LogInformation(ex, message);
            }
            finally
            {
                if (statusCode != 200)
                {
                    context.Result = statusCode == 401 ? new UnauthorizedResult() : new StatusCodeResult(statusCode);
                }
                
                context.HttpContext.Response.StatusCode = statusCode;
                if (!string.IsNullOrEmpty(message))
                {
                    context.HttpContext.Response.WriteAsync(message).ConfigureAwait(true);
                }
            }
        }
    }
}