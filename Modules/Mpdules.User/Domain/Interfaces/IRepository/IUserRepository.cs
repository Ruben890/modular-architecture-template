namespace Mpdules.User.Domain.Interfaces.IRepository
{
    public interface IUserRepository
    {
        public void AddUser(User user);
        public void DeleteUser(User user);
        public void UpdateUser(User user);
    }
}
