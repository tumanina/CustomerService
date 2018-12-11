using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CustomerService.Business;
using CustomerService.Api.Areas.V1.Models;
using CustomerService.Api.Authorization;
using Session = CustomerService.Api.Areas.V1.Models.Session;
using CustomerService.Core;

namespace CustomerService.Api.Areas.V1.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Methods for viewing and managing user sessions.
    /// </summary>
    [Route("api/v1/[controller]")]
    public class SessionsController : BaseController
    {
        private readonly ISessionService _sessionService;

        /// <summary>
        /// Initialisation.
        /// </summary>
        public SessionsController(ISessionService sessionService, ILogger<SessionsController> logger) : base(logger)
        {
            _sessionService = sessionService;
        }

        /// <summary>
        /// Returns list of client sessions.
        /// </summary>
        /// <param name="onlyActive">Show only active session</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Status of request response (OK/500) and list of client sessions in case of success execution.</returns>
        [HttpGet]
        [ClientAuthorization]
        public IActionResult Get(bool onlyActive = false, int? pageNumber = 1, int? pageSize = 20)
        {
            try
            {
                if (HttpContext.Items.TryGetValue("clientId", out var client))
                {
                    var clientId = (Guid)client;
                    var sessions = _sessionService.GetSessions(clientId, onlyActive, pageNumber.Value, pageSize.Value);

                    return Ok(sessions.Convert(t => new Session(t)));
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Returns session information by id.
        /// </summary>
        /// <param name="id">Session identifier</param>
        /// <returns>Status of request response (OK/404/500) and session details in case of success execute.</returns>
        [HttpGet("{id}", Name = "GetSession")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                if (HttpContext.Items.TryGetValue("clientId", out var client))
                {
                    var clientId = (Guid)client;
                    var session = _sessionService.GetSession(clientId, id);
                    if (session == null)
                    {
                        return NotFound();
                    }

                    return Ok(new Session(session));
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Method for create new session key
        /// </summary>
        /// <param name="request">Session key</param>
        /// <returns>Execution status (ОК/500) and session key.</returns>
        // POST api/sessions/key
        [HttpPost]
        public IActionResult Post([FromBody]CreateSessionKeyRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Request is empty or has invalid format.");
                }

                if (string.IsNullOrEmpty(request.IP) || !IsIPv4(request.IP))
                {
                    return BadRequest("IP address is empty or has invalid format.");
                }

                var result = _sessionService.CreateSession(request.Name, request.Password, request.IP, 900);
                if (result == null)
                {
                    return StatusCode(500, "Session didn't create.");
                }

                return Ok(result.SessionKey);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Method for getting information is session confirm required.
        /// </summary>
        /// <param name="id">Session identifier</param>
        /// <param name="clientId">Client identifier</param>
        /// <returns>Status of request execution (ОК/404/500).</returns>
        // GET api/sessions/{id}/confirmrequired
        [HttpGet("{id}/confirm/required", Name = "IsSessionConfirmRequired")]
        public IActionResult IsSessionConfirmRequired(Guid id)
        {
            try
            {
                if (HttpContext.Items.TryGetValue("clientId", out var client))
                {
                    var clientId = (Guid)client;
                    var result = _sessionService.IsSessionConfirmRequired(clientId, id);
                    if (result == null)
                    {
                        return NotFound();
                    }

                    return Ok(result.Value);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Method for confirmation session by specified id.
        /// </summary>
        /// <param name="id">Session identifier</param>
        /// /// <param name="clientId">Client identifier</param>
        /// <returns>Status of request execution (ОК/404/500).</returns>
        // PUT api/sessions/{id}/confirm
        [HttpPut("{id}/confirm", Name = "ConfirmSession")]
        public IActionResult ConfirmSession(Guid id, [FromBody] string oneTimePassword)
        {
            try
            {
                if (HttpContext.Items.TryGetValue("clientId", out var client))
                {
                    var clientId = (Guid)client;
                    var result = _sessionService.ConfirmSession(clientId, id, oneTimePassword);
                    if (result == false)
                    {
                        return NotFound();
                    }

                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Method for disable session by specified id.
        /// </summary>
        /// <param name="id">Session identifier</param>
        /// <returns>Status of request execution (ОК/404/500).</returns>
        // PUT api/sessions/{id}/disable
        [HttpPut("{id}/disable", Name = "DisableSession")]
        public IActionResult DisableSession(Guid id)
        {
            try
            {
                if (HttpContext.Items.TryGetValue("clientId", out var client))
                {
                    var clientId = (Guid)client;
                    var result = _sessionService.DisableSession(clientId, id);

                    if (result == false)
                    {
                        return NotFound();
                    }

                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
