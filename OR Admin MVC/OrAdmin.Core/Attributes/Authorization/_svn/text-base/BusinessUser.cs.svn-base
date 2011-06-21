using System.Web.Mvc;
using OrAdmin.Core.Enums.App;

namespace OrAdmin.Core.Attributes.Authorization
{
    public class BusinessUser : AuthorizeAttribute
    {
        public BusinessUser()
            {
                var authorizedRoles = new[] {
                    RoleName.PurchaseAdmin,
                    RoleName.PurchaseApprover,
                    RoleName.PurchaseManager,
                    RoleName.PurchaseUser,
                    RoleName.MasterAdmin
                };

                Roles = string.Join(",", authorizedRoles);
            }
    }
}
