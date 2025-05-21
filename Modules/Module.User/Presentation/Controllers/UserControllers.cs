using Microsoft.AspNetCore.Mvc;

namespace Module.User.Presentation.Controllers
{
    [ApiVersion("0", Deprecated = true)]
    [ApiVersion("1")]
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
