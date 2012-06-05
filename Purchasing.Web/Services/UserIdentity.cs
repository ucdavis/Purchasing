using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace Purchasing.Web.Services
{
    public interface IUserIdentity
    {
        string Current { get; }
        IPrincipal CurrentPrincipal { get; }
        bool IsUserInRole(string userId, string roleId);
    }

    public class UserIdentity : IUserIdentity
    {
        public string Current { get { return HttpContext.Current.User.Identity.Name; } }
        public IPrincipal CurrentPrincipal { get { return HttpContext.Current.User; } }
        public bool IsUserInRole(string userId, string roleId)
        {
            return Roles.IsUserInRole(userId, roleId);
        }
    }
}