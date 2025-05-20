using Mpdules.User.Domain.Interfaces.IRepository;
using Shared.Core;

namespace Mpdules.User.Infrastrutucture
{
    public class UserRepository :RepositoryBase<Domain.User, UserContext>, IUserRepository
    {
        private readonly UserContext _context;
        public UserRepository(UserContext context) : base(context)
        {
            _context = context;
        }

        public void AddUser(Domain.User user) => Create(user);
        public void DeleteUser(Domain.User user) => Delete(user);
        public void UpdateUser(Domain.User user) => Update(user);
    }
}
