

using Module.User.Domain.Entity;

namespace Module.User.Domain.Interfaces.IRepository
{
    public interface IRoleRepository
    {
        Task<Role?> GetRoleByName(string roleName, int Id);
    }
}
