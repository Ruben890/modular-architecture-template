using Microsoft.EntityFrameworkCore;
using Module.User.Domain.Entity;
using Module.User.Domain.Interfaces.IRepository;
using Shared.Core;
using Shared.Core.Interfaces;



namespace Module.User.Infrastrutucture
{
    public class UserRepository : RepositoryBase<UserEntity, UserContext>, IUserRepository
    {
        private readonly UserContext _context;
        public UserRepository(UserContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
        }

        public async Task<UserEntity?> GetUserByEmailOrUserName(string email, string userName)
        {
            return await _context.Users
                .Where(u => u.Email == email || u.UserName == userName)
                .AsNoTracking()
                .Select(x => new UserEntity
                {
                    Id = x.Id,
                    Email = x.Email,
                    UserName = x.UserName,
                    Name = x.Name,
                    LastName = x.LastName,
                    Role = x.Role
                }).FirstOrDefaultAsync();
        }

        public async Task<UserEntity?> GetUserById(Guid UserId)
        {
            return await _context.Users
                .Where(x => x.Id == UserId)
                .AsNoTracking()
                .Select(x => new UserEntity
                {
                    Id = x.Id,
                    Email = x.Email,
                    UserName = x.UserName,
                    Name = x.Name,
                    LastName = x.LastName,
                    Role = x.Role
                }).FirstOrDefaultAsync();
        }

        public async Task AddUser(UserEntity user) => await Create(user);
        public void DeleteUser(UserEntity user) => Delete(user);
        public void UpdateUser(UserEntity user) => Update(user);
    }
}
