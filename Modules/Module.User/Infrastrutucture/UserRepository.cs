using Microsoft.EntityFrameworkCore;
using Module.User.Domain.Interfaces.IRepository;
using Shared.Core;
using UserEntity = Module.User.Domain.Entity.User;
using UserDto = Shared.DTO.Dtos.User;
using System.Threading.Tasks;

namespace Module.User.Infrastrutucture
{
    public class UserRepository :RepositoryBase<UserEntity, UserContext>, IUserRepository
    {
        private readonly UserContext _context;
        public UserRepository(UserContext context) : base(context)
        {
            _context = context;
        }

        public async Task<UserDto?> GetUserByEmailOrUserName(string email, string userName)
        {
            return await _context.Users
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    Email = x.Email,
                    UserName = x.UserName,
                    Name = x.Name,
                    LastName = x.LastName,
                    RoleName = x.Role.Name,
                }).FirstOrDefaultAsync(u => u.Email == email || u.UserName == userName);
        }

        public async Task AddUser(UserEntity user) => await Create(user);
        public void DeleteUser(UserEntity user) => Delete(user);
        public void UpdateUser(UserEntity user) => Update(user);
    }
}
