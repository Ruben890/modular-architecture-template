using Microsoft.EntityFrameworkCore;
using Module.User.Domain.Entity;
using Module.User.Domain.Interfaces.IRepository;
using Shared.Core;


namespace Module.User.Infrastrutucture
{
    public class RoleRepository : RepositoryBase<Role, UserContext>, IRoleRepository
    {
        public RoleRepository(UserContext context) : base(context){}

        public async Task<Role?> GetRoleByName(string roleName, int Id) =>
            await FindByCondition(x => x.Name == roleName || x.Id == Id, false)
            .FirstOrDefaultAsync();

    }
}
