
using Module.User.Domain.Entity;

namespace Module.User.Domain.Interfaces.IRepository
{
    public interface IUserRepository
    {
        public Task AddUser(UserEntity user);
        public void DeleteUser(UserEntity user);
        Task<UserEntity?> GetUserByEmailOrUserName(string email, string userName);
        public void UpdateUser(UserEntity user);
    }
}
