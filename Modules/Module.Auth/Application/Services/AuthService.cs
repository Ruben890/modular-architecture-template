using Module.Auth.Domain.Interfaces;
using Wolverine;

namespace Module.Auth.Application.Services
{
    public class AuthService : IAuthService
    {

        private readonly IMessageBus _bus;

        public AuthService(IMessageBus bus)
        {
            _bus = bus;
        }



    }
}
