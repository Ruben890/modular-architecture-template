using Mapster;
using Module.User.Domain.Entity;
using Module.User.Domain.Interfaces.IRepository;
using Shared.Core.Attributes;
using Shared.Messages.Commads;
using Wolverine.Attributes;

namespace Module.User.Application.Commands.User
{
    [WolverineHandler]
    [WModuleHandler]
    public class UpdateUserCommand
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserCommand(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Handle(UpdateUser command)
        {
            _userRepository.UpdateUser(command.User.Adapt<UserEntity>());
        }
    }
}
