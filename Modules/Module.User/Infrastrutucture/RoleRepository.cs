using Microsoft.EntityFrameworkCore;
using Module.User.Domain.Entity;
using Module.User.Domain.Interfaces.IRepository;
using Shared.Core;
using Shared.Core.Interfaces;


namespace Module.User.Infrastrutucture
{
    public class RoleRepository : RepositoryBase<Role, UserContext>, IRoleRepository
    {
        public RoleRepository(UserContext context,  IUnitOfWork unitOfWork) : base(context, unitOfWork) {}

        public async Task<Role?> GetRoleByName(string roleName, int Id) =>
            await FindByCondition(x => x.Name == roleName || x.Id == Id, false)
            .FirstOrDefaultAsync();

    }
}
