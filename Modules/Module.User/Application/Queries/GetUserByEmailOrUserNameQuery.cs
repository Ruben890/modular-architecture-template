using Mapster;
using Module.User.Domain.Interfaces.IRepository;
using Shared.Core.Attributes;
using Shared.Messages.Queries;
using Wolverine.Attributes;
using UserDto = Shared.DTO.Dtos.User;


namespace Module.User.Application.Queries
{
    [WolverineHandler]
    [WModuleHandler]
    public class GetUserByEmailOrUserNameQuery
    {
        private readonly IUserRepository _userRepository;

        public GetUserByEmailOrUserNameQuery(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> Handle(GetUserByEmailOrUserName query, CancellationToken ct)
        {
            var userEntity = await _userRepository.GetUserByEmailOrUserName(query.Email, query.UserName);

            return userEntity.Adapt<UserDto>();
        }
    }
}
