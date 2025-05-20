using Microsoft.AspNetCore.Mvc;
using Module.User.Domain.Interfaces.IRepository;
using Shared.Core.Interfaces;

namespace Module.User.Presentation.Controllers
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
            _logger.LogInfo("funciona");
            return NotFound("no hay ususarios ");

        }
    }
}
