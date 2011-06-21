using System.Security.Principal;
using OrAdmin.Entities.App;
using OrAdmin.Repositories.App;

namespace OrAdmin.Core.Extensions
{
    public static class PrincipleExtensions
    {
        public static Profile Profile(this IPrincipal user)
        {
            ProfileRepository repo = new ProfileRepository();
            return repo.GetProfile(user.Identity.Name);
        }

        public static Profile Profile(this IPrincipal user, string userName)
        {
            ProfileRepository repo = new ProfileRepository();
            return repo.GetProfile(userName);
        }

        public static bool HasProfile(this IPrincipal user)
        {
            ProfileRepository repo = new ProfileRepository();
            return repo.HasProfile(user.Identity.Name);
        }
    }
}
