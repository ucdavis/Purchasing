using System.Security.Principal;
using System.Web;

namespace Purchasing.Web.Services
{
    public interface IUserIdentity
    {
        string Current { get; }
        IPrincipal CurrentPrincipal { get; }
    }

    public class UserIdentity : IUserIdentity
    {
        public string Current { get { return HttpContext.Current.User.Identity.Name; } }
        public IPrincipal CurrentPrincipal { get { return HttpContext.Current.User; } }
    }
}