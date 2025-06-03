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
    public class GetUserByIdQueries
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueries(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> Handle(GetUserById query, CancellationToken ct)
        {
            var userEntity = await _userRepository.GetUserById(query.UserId);
            return userEntity!.Adapt<UserDto>();
        }
    }
}
