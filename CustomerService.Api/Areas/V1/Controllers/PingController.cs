using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Api.Areas.V1.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Methods for checking if system is alive.
    /// </summary>
    public class PingController : Controller
    {
        /// <summary>
        /// Return response (means that system is alive).
        /// </summary>
        /// <returns>Response</returns>
        [Route("api/v1/ping")]
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok("response");
        }
    }
}
