using Microsoft.AspNetCore.Mvc;

namespace Mpdules.User.Presentation.Controllers
{
    [ApiController]
    [Route("api/Users")]
    internal class UserController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetUsers()
        {
            return NotFound("no hay ususarios ");
        }
    }
}
