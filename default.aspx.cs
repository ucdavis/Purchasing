using System;

public partial class tools_purchasing_default : System.Web.UI.Page
{
    // Page event, raised on page load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            UCDMenu.BuildBar_3(Menu3); // Build "In this section" Menu

            if (PurchaseHelper.UserIsValid(User.Identity.Name))
            {
                // User is authenticated and has been assigned a role... route user to a specific page
                string[] roles = new string[] { "PurchaseAdmin", "MasterAdmin", "PurchaseManager" };
                if (Users.UserIsInRoles(User.Identity.Name, roles))
                    Response.Redirect("admin/default.aspx");
                else if (User.IsInRole("PurchaseApprover"))
                    Response.Redirect("orders/type.aspx");
                else if (User.IsInRole("PurchaseUser"))
                    Response.Redirect("orders/type.aspx");
            }
            else
            {
                // User was authenticated, but lacks a role or profile
                requestRadWindow.VisibleOnPageLoad = true;
                if (!requestRadWindow.VisibleOnPageLoad)
                    Response.Redirect("default.aspx");
            }
        }
    }
}
