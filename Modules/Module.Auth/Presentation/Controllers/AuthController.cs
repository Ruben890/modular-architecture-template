using Microsoft.AspNetCore.Mvc;
using Module.Auth.Domain.Interfaces;

namespace Module.Auth.Presentation.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    internal class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;


        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }



    }
}
