using System.Web.Mvc;
using OrAdmin.Core.Enums.App;

namespace OrAdmin.Core.Attributes.Authorization
{
    public class PurchaseAdmin : AuthorizeAttribute
    {
        public PurchaseAdmin()
        {
            // var authorizedRoles = new[] {RoleNames.Admin, RoleNames.ProjectAdmin};
            // Roles = string.Join(",", authorizedRoles);

            Roles = RoleName.PurchaseAdmin.ToString();
        }
    }
}
