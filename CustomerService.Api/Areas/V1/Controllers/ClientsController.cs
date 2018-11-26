using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CustomerService.Business;
using CustomerService.Api.Areas.V1.Models;

namespace CustomerService.Api.Areas.V1.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Methods for viewing and managing cients in system.
    /// </summary>
    [Route("api/v1/[controller]")]
    public class ClientsController : BaseController
    {
        private readonly IClientService _clientService;

        /// <summary>
        /// Initialization.
        /// </summary>
        public ClientsController(IClientService clientService, ILogger<ClientsController> logger) : base(logger)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// Returns address details by Id.
        /// </summary>
        /// <returns>Execution status (ОК/500/404) and address details/error information.</returns>
        [HttpGet("{id}", Name = "GetClient")]
        public IActionResult GetById(Guid id)
        {
            try
            {

                var client = _clientService.GetClient(id);

                if (client == null)
                {
                    return NotFound();
                }

                return Ok(new Client(client));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Check by email/name is client registered in system.
        /// </summary>
        /// <returns>Execution status (ОК/400/500) and true if name/email available, false - in other case.</returns>
        [HttpGet("check", Name = "CheckAvailability")]
        public IActionResult CheckAvailability(string name = null, string email = null)
        {
            try
            {
                if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(email))
                {
                    return BadRequest();
                }

                var result = !string.IsNullOrEmpty(name) ? _clientService.CheckNameAvailability(name)
                    : (!string.IsNullOrEmpty(email) ? _clientService.CheckEmailAvailability(email) : false);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Method for creating new client.
        /// </summary>
        /// <param name="request">client details</param>
        /// <returns>Execution status (Created/500) and created client details.</returns>
        // POST api/clients
        [HttpPost]
        public IActionResult Post([FromBody]CreateClientRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Request is empty or has invalid format");
                }

                if (string.IsNullOrEmpty(request.Name) || request.Name.Length > 32)
                {
                    return BadRequest("Client name is empty or has length more than 32.");
                }

                if (string.IsNullOrEmpty(request.Password) || !ValidatePassword(request.Password))
                {
                    return BadRequest("Password is empty or has invalid format.");
                }

                if (string.IsNullOrEmpty(request.Email) || !IsEmailValid(request.Email))
                {
                    return BadRequest("Email is empty or has invalid format.");
                }

                var result = _clientService.CreateClient(request.Email, request.Name, request.Password);

                if (result == null)
                {
                    return StatusCode(500, "Client didn't create.");
                }

                return Created(GetCreatedUrl(result.Id.ToString()), new Client(result));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Method for actvate Client by specified activation code.
        /// </summary>
        /// <param name="activationCode">Activation Code</param>
        /// <returns>Execution status (ОК/404/500).</returns>
        // PUT api/clients/activate
        [HttpPut("activate", Name = "ActivateClient")]
        public IActionResult ActivateClient([FromBody]string activationCode)
        {
            try
            {
                var result = _clientService.ActivateClient(activationCode);

                if (result == false)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Method for creating google auth code.
        /// </summary>
        /// <param name="Id">Client id</param>
        /// <returns>Execution status (ОК/404/500).</returns>
        // PUT api/clients/{id}/googleauth
        [HttpPut("{id}/googleauth", Name = "CreateGoogleAuthCode")]
        public IActionResult CreateGoogleAuthCode(Guid id)
        {
            try
            {
                var result = _clientService.CreateGoogleAuthCode(id);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Method for activate client google auth code.
        /// </summary>
        /// <param name="Id">Client id</param>
        /// <param name="oneTimePassword">One Time Password (from GoogleAuth)</param>
        /// <returns>Execution status (ОК/404/500).</returns>
        // PUT api/clients/{id}/googleauth
        [HttpPut("{id}/googleauth/activate", Name = "ActivateGoogleAuthCode")]
        public IActionResult ActivateGoogleAuthCode(Guid id, [FromBody] string oneTimePassword)
        {
            try
            {
                var result = _clientService.SetGoogleAuthCode(id, oneTimePassword);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Method for deactivate client google auth code.
        /// </summary>
        /// <param name="Id">Client id</param>
        /// <param name="oneTimePassword">One Time Password (from GoogleAuth)</param>
        /// <returns>Execution status (ОК/404/500).</returns>
        // PUT api/clients/{id}/googleauth
        [HttpPut("{id}/googleauth/deactivate", Name = "DeactivateGoogleAuthCode")]
        public IActionResult DeactivateGoogleAuthCode(Guid id, [FromBody] string oneTimePassword)
        {
            try
            {
                var result = _clientService.DeactivateGoogleAuthCode(id, oneTimePassword);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private bool ValidatePassword(string password)
        {
            var rgx = new Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).{6,50}$");

            return rgx.IsMatch(password);
        }
    }
}
