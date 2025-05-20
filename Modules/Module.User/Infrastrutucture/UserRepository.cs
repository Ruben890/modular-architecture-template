using Microsoft.EntityFrameworkCore;
using Module.User.Domain.Interfaces.IRepository;
using Shared.Core;
using UserEntity = Module.User.Domain.Entity.User;

namespace Module.User.Infrastrutucture
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
