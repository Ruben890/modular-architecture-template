using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Module.User.Domain.Interfaces.IServices;
using Shared.DTO.Request.QueryParameters;

namespace Module.User.Presentation.Controllers
{
    [ApiVersion("0", Deprecated = true)]
    [ApiVersion("1")]
    [ApiController]
    [Route("api/[controller]")]
    internal class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [Authorize]
        [HttpGet("GetByUser")]
        public async Task<IActionResult> GetByUser([FromQuery] GenericParameters parameters) =>
         new ObjectResult(await _userServices.GetByUser(parameters, HttpContext));
    }
}
