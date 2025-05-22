
using UserDto = Shared.DTO.Dtos.User;
namespace Module.User.Domain.Interfaces.IRepository
{
    public interface IUserRepository
    {
        public Task AddUser(Entity.User user);
        public void DeleteUser(Entity.User user);
        Task<UserDto?> GetUserByEmailOrUserName(string email, string userName);
        public void UpdateUser(Entity.User user);
    }
}
