using Module.User.Domain.Interfaces.IRepository;
using Shared.Core.Attributes;
using Shared.Messages.Queries;
using Wolverine.Attributes;
using UserDto = Shared.DTO.Dtos.User;


namespace Module.User.Application.Handlers.Queries
{
    [WolverineHandler]
    [ModuleHandler]
    public class GetUserByEmailOrUserNameQueries 
    {
        private readonly IUserRepository _userRepository;

        public GetUserByEmailOrUserNameQueries(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<UserDto?> Handle(GetUserByEmailOrUserName query, CancellationToken ct)
        {
            return _userRepository.GetUserByEmailOrUserName(query.Email, query.UserName);
        }
    }
}
