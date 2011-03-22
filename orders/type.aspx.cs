using System;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.UI;

public partial class business_purchasing_orders_type : System.Web.UI.Page
{
    //Page event, raised on page load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PurchaseHelper.UserIsValid(User.Identity.Name))
            Response.Redirect(ConfigurationManager.AppSettings["purchasingRoot"].ToString());

        if (!IsPostBack)
        {
            UCDMenu.BuildBar_3(Menu3); // Build "In this section" Menu
            sectionTitleLabel.Text = "Place An Order";
        }
    }

    // Hadles imagebutton clicks
    protected void ImageButton_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect(((ImageButton)sender).CommandArgument);
    }
}
