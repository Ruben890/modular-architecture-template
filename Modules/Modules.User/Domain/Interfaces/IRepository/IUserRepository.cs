
namespace Mpdules.User.Domain.Interfaces.IRepository
{
    public interface IUserRepository
    {
        public void AddUser(Entity.User user);
        public void DeleteUser(Entity.User user);
        Task<Entity.User?> GetUserByEmailOrUserName(string email, string userName);
        public void UpdateUser(Entity.User user);
    }
}
