using Microsoft.AspNetCore.Mvc;
using Shareds.Core.Interfaces;

namespace Mpdules.User.Presentation.Controllers
{
    [ApiController]
    [Route("api/Users")]
    internal class UserController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        public UserController(ILoggerManager logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            _logger.LogInfo("holas funciona ");
            return Ok(new[] { "User1", "User2" });
        }
    }
}
