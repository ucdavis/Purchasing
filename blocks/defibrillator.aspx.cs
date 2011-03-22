using System;

public partial class business_purchasing_blocks_defibrillator : System.Web.UI.Page
{
    private void Page_Load(object sender, System.EventArgs e)
    {
        Response.AddHeader("Refresh", Convert.ToString((Session.Timeout * 60) - 60));
    }
}
