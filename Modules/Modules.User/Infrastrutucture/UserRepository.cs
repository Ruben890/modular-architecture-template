using Microsoft.EntityFrameworkCore;
using Mpdules.User.Domain.Interfaces.IRepository;
using Shared.Core;
using UserEntity = Mpdules.User.Domain.Entity.User;

namespace Mpdules.User.Infrastrutucture
{
    public class UserRepository :RepositoryBase<UserEntity, UserContext>, IUserRepository
    {
        private readonly UserContext _context;
        public UserRepository(UserContext context) : base(context)
        {
            _context = context;
        }

        public async Task<UserEntity?> GetUserByEmailOrUserName(string email, string userName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email || u.UserName == userName);
        }

        public void AddUser(UserEntity user) => Create(user);
        public void DeleteUser(UserEntity user) => Delete(user);
        public void UpdateUser(UserEntity user) => Update(user);
    }
}
