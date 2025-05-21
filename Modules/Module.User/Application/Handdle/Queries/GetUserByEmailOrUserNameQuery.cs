using Module.User.Domain.Interfaces.IRepository;
using Wolverine.Attributes;
using UserDto = Shared.DTO.Dtos.User;


namespace Module.User.Application.Handlers.Queries
{
    [WolverineHandler]
    public class GetUserByEmailOrUserNameHandler
    {
        private readonly IUserRepository _userRepository;

        public GetUserByEmailOrUserNameHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<UserDto?> Handle(Shared.Messages.Queries.GetUserByEmailOrUserNameHandler query, CancellationToken ct)
        {
            return _userRepository.GetUserByEmailOrUserName(query.Email, query.UserName);
        }
    }
}
