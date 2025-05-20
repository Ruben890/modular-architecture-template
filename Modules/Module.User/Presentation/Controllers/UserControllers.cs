using Microsoft.AspNetCore.Mvc;
using Module.User.Domain.Interfaces.IRepository;
using Shared.Core.Interfaces;

namespace Module.User.Presentation.Controllers
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
