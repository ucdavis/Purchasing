using System.Linq;
using System.Security.Principal;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Services
{
    public interface IHassAccessService
    {
        bool DaAccessToWorkgroup(IRepositoryWithTypedId<User, string> userRepository, Workgroup workgroup, IPrincipal currentUser);
    }

    public class HassAccessService : IHassAccessService
    {
        public bool DaAccessToWorkgroup(IRepositoryWithTypedId<User, string> userRepository, Workgroup workgroup, IPrincipal currentUser)
        {
            var user = userRepository.GetNullableById(currentUser.Identity.Name);
            Check.Require(user != null);
            Check.Require(workgroup != null);

            if(!currentUser.IsInRole(Role.Codes.DepartmentalAdmin))
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