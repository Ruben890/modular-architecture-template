using Microsoft.EntityFrameworkCore;
using Mpdules.User.Domain;
using Mpdules.User.Domain.Interfaces.IRepository;
using Shared.Core;


namespace Mpdules.User.Infrastrutucture
{
    public class RoleRepository : RepositoryBase<Role, UserContext>, IRoleRepository
    {
        private readonly UserContext _contex;
        public RoleRepository(UserContext context) : base(context)
        {
            _contex = context;
        }

        public async Task<Role?> GetRoleByName(string roleName, int Id) =>
            await FindByCondition(x => x.Name == roleName || x.Id == Id, false)
            .FirstOrDefaultAsync();

    }
}
