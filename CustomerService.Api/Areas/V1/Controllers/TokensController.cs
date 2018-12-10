using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CustomerService.Api.Areas.V1.Models;
using CustomerService.Business;
using CustomerService.Api.Authorization;

namespace CustomerService.Api.Areas.V1.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Methods for viewing and managing tokens in system.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ClientAuthorization]
    public class TokensController : BaseController
    {
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Initialization.
        /// </summary>
        public TokensController(ITokenService tokenService, ILogger<TokensController> logger) : base(logger)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// Returns list of client tokens in system.
        /// </summary>
        /// <returns>Execution status (ОК/500) and list of tokens/error information.</returns>
        [HttpGet]
        public IActionResult Get(bool onlyActive = false)
        {
            try
            {
                if (HttpContext.Items.TryGetValue("clientId", out var client))
                {
                    var clientId = (Guid)client;
                    var tokens = _tokenService.GetTokens(clientId, onlyActive);
                    return Ok(tokens.Select(t => new Token(t)).ToList());
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
        /// Method for creating new token.
        /// </summary>
        /// <param name="request">Token details</param>
        /// <returns>Execution status (ОК/500) and created token details.</returns>
        // POST api/tokens
        [HttpPost]
        public IActionResult Post([FromBody]CreateTokenRequest request)
        {
            try
            {
                if (HttpContext.Items.TryGetValue("clientId", out var client))
                {
                    var clientId = (Guid)client;
                    if (request == null)
                    {
                        return BadRequest("Request is empty or has invalid format");
                    }
                    if (!IsIPv4(request.IP))
                    {
                        return BadRequest("IP addres has invalid format.");
                    }
                    if (string.IsNullOrEmpty(request.AuthMethod))
                    {
                        return BadRequest("AuthMethod is empty.");
                    }

                    var result = _tokenService.CreateToken(clientId, request.IP, request.AuthMethod);

                    if (result == null)
                    {
                        return StatusCode(500, "Token didn't create.");
                    }

                    return Created(GetCreatedUrl(result.Id.ToString()), new Token(result));
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
        /// Method for deactivate token by specified id.
        /// </summary>
        /// <param name="id">Token identifier</param>
        /// <returns>Execution status (ОК/500).</returns>
        // PUT api/tokens/{id}/deactivate
        [HttpPut("{id}/deactivate", Name = "DeactivateToken")]
        public IActionResult DeactivateToken(Guid id)
        {
            try
            {
                var result = _tokenService.UpdateActive(id, false);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(new Token(result));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Method for actvate token by specified id.
        /// </summary>
        /// <param name="id">Token identifier</param>
        /// <returns>Execution status (ОК/500).</returns>
        // PUT api/tokens/{id}/deactivate
        [HttpPut("{id}/activate", Name = "ActivateToken")]
        public IActionResult ActivateToken(Guid id)
        {
            try
            {
                var result = _tokenService.UpdateActive(id, true);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(new Token(result));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
