using Marten;
using Shared.Messages.Querys;
using UserDto = Shared.DTO.Dtos.User;


namespace Module.User.Application.Handlers.Query
{
    public class GetUserByEmailOrUserNameHandler
    {
        private readonly IDocumentSession _session;

        public GetUserByEmailOrUserNameHandler(IDocumentSession session)
        {
            _session = session;
        }

        public Task<UserDto?> Handle(GetUserByEmailOrUserNameQuery query, CancellationToken ct)
        {
            return _session.Query<UserDto>()
                .FirstOrDefaultAsync(x => x.Email == query.Email || x.UserName == query.UserName, ct);
        }
    }
}
