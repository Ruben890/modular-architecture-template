using Module.User.Domain.Interfaces.IRepository;
using Module.User.Domain.Interfaces.IServices;
using Shared.Core.Interfaces;

namespace Module.User.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly ILoggerManager _logger;
        private readonly IUserRepository  _userRepository;


        public UserServices(ILoggerManager logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        

    }
}
