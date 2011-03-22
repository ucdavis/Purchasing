using System;
using System.Configuration;

public partial class business_purchasing_orders_default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (PurchaseHelper.UserIsValid(User.Identity.Name))
            {
                // User is authenticated and has been assigned a role... route user to specific page
                string[] roles = new string[2] { "PurchaseAdmin", "MasterAdmin" };
                if (Users.UserIsInRoles(User.Identity.Name, roles))
                    Response.Redirect(ConfigurationManager.AppSettings["purchasingRoot"].ToString() + "admin/default.aspx");
                else if (User.IsInRole("PurchaseApprover"))
                    Response.Redirect(ConfigurationManager.AppSettings["purchasingRoot"].ToString() + "orders/type.aspx");
                else if (User.IsInRole("PurchaseUser"))
                    Response.Redirect(ConfigurationManager.AppSettings["purchasingRoot"].ToString() + "orders/type.aspx");
            }
            else
            {
                // User was authenticated, but lacks a role or profile
                Response.Redirect(ConfigurationManager.AppSettings["purchasingRoot"].ToString());
            }
        }
    }
}
