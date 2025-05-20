

namespace Mpdules.User.Domain.Interfaces.IRepository
{
    public interface IRoleRepository
    {
        Task<Role?> GetRoleByName(string roleName, int Id);
    }
}
