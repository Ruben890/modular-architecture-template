using Mpdules.User.Domain;
using Mpdules.User.Domain.Interfaces.IRepository;
using Shareds.Core;

namespace Mpdules.User.Infrastrutucture
{
    public class RoleRepository : RepositoryBase<Role, UserContext>, IRoleRepository
    {
        private readonly UserContext _contex;
        public RoleRepository(UserContext context) : base(context)
        {
            _contex = context;
        }


    }
}
