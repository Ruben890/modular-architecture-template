namespace Mpdules.User.Domain.Interfaces
{
    public interface IUserRepository
    {
        public void AddUser(Domain.User user);
        public void DeleteUser(Domain.User user);
        public void UpdateUser(Domain.User user);
    }
}
