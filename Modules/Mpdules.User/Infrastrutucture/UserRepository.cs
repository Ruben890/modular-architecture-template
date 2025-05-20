using Mpdules.User.Domain;
using Shareds.Core;

namespace Mpdules.User.Infrastrutucture
{
    public class UserRepository :RepositoryBase<Domain.User, UserContext>, IUserRepository
    {
        private readonly UserContext _context;
        public UserRepository(UserContext context) : base(context)
        {
            _context = context;
        }


    }
}
