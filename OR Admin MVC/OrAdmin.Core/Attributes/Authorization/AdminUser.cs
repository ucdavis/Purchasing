using System.Web.Mvc;
using OrAdmin.Core.Enums.App;

namespace OrAdmin.Core.Attributes.Authorization
{
    public class AdminUser : AuthorizeAttribute
    {
        public AdminUser()
        {
            var authorizedRoles = new[] {
                    RoleName.PurchaseAdmin,
                    RoleName.PurchaseManager,
                    RoleName.MasterAdmin
                };

            Roles = string.Join(",", authorizedRoles);
        }
    }
}
