using Microsoft.EntityFrameworkCore;
using Mpdules.User.Domain.Entity;
using Mpdules.User.Domain.Interfaces.IRepository;
using Shared.Core;
using User = Domain.Entity.User;

namespace Mpdules.User.Infrastrutucture
{
    public class UserRepository :RepositoryBase<Domain.Entity.User, UserContext>, IUserRepository
    {
        private readonly UserContext _context;
        public UserRepository(UserContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Domain.Entity.User?> GetUserByEmailOrUserName(string email, string userName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email || u.UserName == userName);
        }

        public void AddUser(Domain.Entity.User user) => Create(user);
        public void DeleteUser(Domain.Entity.User user) => Delete(user);
        public void UpdateUser(Domain.Entity.User user) => Update(user);
    }
}
