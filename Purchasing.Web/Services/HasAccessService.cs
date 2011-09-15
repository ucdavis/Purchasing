using System.Linq;
using System.Security.Principal;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Services
{
    public interface IHasAccessService
    {
        bool DaAccessToWorkgroup(Workgroup workgroup);
    }

    public class HasAccessService : IHasAccessService
    {
        private readonly IUserIdentity _userIdentity;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;

        public HasAccessService(IUserIdentity userIdentity, IRepositoryWithTypedId<User, string> userRepository)
        {
            _userIdentity = userIdentity;
            _userRepository = userRepository;
        }

        public bool DaAccessToWorkgroup(Workgroup workgroup)
        {
            var user = _userRepository.GetNullableById(_userIdentity.Current);
            Check.Require(user != null);
            Check.Require(workgroup != null);

            if(!_userIdentity.CurrentPrincipal.IsInRole(Role.Codes.DepartmentalAdmin))
            {
                return false;
            }

            var usersOrgs = user.Organizations.Select(a => a.Id).ToList();
            if(workgroup.Organizations.Where(a => usersOrgs.Contains(a.Id)).Any())
            {
                return true;
            }

            return false;
        }
        
    }


}