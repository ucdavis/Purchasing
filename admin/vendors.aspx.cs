using System;
using System.Configuration;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class business_purchasing_admin_vendors : System.Web.UI.Page
{
    // Page event, raised on page load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PurchaseHelper.UserIsValid(User.Identity.Name))
            Response.Redirect(ConfigurationManager.AppSettings["purchasingRoot"].ToString());

        if (!IsPostBack)
        {
            UCDMenu.BuildBar_3(Menu3); // Build "In this section" Menu
            Menu3.Items[4].Selected = true;
            BindVendors();
            sectionTitleLabel.Text = "Vendors";
            vendorMultiView.SetActiveView(View_List);
        }
    }

    /// <summary>
    /// Subroutine, binds vendors to gridview
    /// </summary>
    protected void BindVendors()
    {
        PurchasingTableAdapters.vendorsTableAdapter vendorAdapter = new PurchasingTableAdapters.vendorsTableAdapter();
        vendorRadGrid.DataSource = vendorAdapter.Get();
        vendorRadGrid.DataBind();
    }

    /// <summary>
    /// Subroutine, binds vendor info to controls
    /// </summary>
    /// <param name="id"></param>
    protected void BindVendor(int vendorID)
    {
        PurchasingTableAdapters.vendorsTableAdapter vendorAdapter = new PurchasingTableAdapters.vendorsTableAdapter();

        // Bind vendor
        Purchasing.vendorsDataTable dt = vendorAdapter.GetById(vendorID);
        foreach (Purchasing.vendorsRow dr in dt.Rows)
        {
            ListItem type = vendorTypeDropDownList.Items.FindByValue(dr.type.ToString());
            if (type != null)
            {
                int typeIndex = vendorTypeDropDownList.Items.IndexOf(type);
                vendorTypeDropDownList.SelectedIndex = typeIndex;
            }
            venderNameTextBox.Text = dr.vendor_name;
            vendorAddressTextBox.Text = dr.vendor_address;
            vendorCityTextBox.Text = dr.vendor_city;
            vendorFaxRadMaskedTextBox.Text = dr.vendor_fax;
            vendorPhoneRadMaskedTextBox.Text = dr.vendor_phone;

            ListItem li = vendorStateDropDownList.Items.FindByValue(dr.vendor_state);
            if (li != null)
            {
                int index = vendorStateDropDownList.Items.IndexOf(li);
                vendorStateDropDownList.SelectedIndex = index;
            }

            if (!dr.Isvendor_urlNull())
                vendorURLTextBox.Text = dr.vendor_url;
            if (!dr.Isvendor_notesNull())
                vendorNotesTextBox.Text = dr.vendor_notes;
        }
    }

    /// <summary>
    /// Subroutine, adds vendor to db
    /// </summary>
    protected bool AddVendor()
    {
        try
        {
            PurchasingTableAdapters.vendorsTableAdapter vendorAdapter = new PurchasingTableAdapters.vendorsTableAdapter();

            // Insert vendor
            vendorAdapter.Insert(
                User.Identity.Name,
                Convert.ToInt32(vendorTypeDropDownList.SelectedValue),
                venderNameTextBox.Text.Trim(),
                vendorAddressTextBox.Text.Trim(),
                vendorCityTextBox.Text.Trim(),
                vendorStateDropDownList.SelectedValue,
                StringHelper.ZeroPad(vendorPhoneRadMaskedTextBox.Text.Trim(), 10),
                StringHelper.ZeroPad(vendorFaxRadMaskedTextBox.Text.Trim(), 10),
                vendorURLTextBox.Text != "http://" ? vendorURLTextBox.Text.Trim() : null,
                vendorNotesTextBox.Text.Trim(),
                DateTime.Now,
                DateTime.Now
                );
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Subroutine, updates vendor
    /// </summary>
    protected bool UpdateVendor(int vendorID)
    {
        try
        {
            PurchasingTableAdapters.vendorsTableAdapter vendorAdapter = new PurchasingTableAdapters.vendorsTableAdapter();

            // Update vendor
            vendorAdapter.Update(
                User.Identity.Name,
                Convert.ToInt32(vendorTypeDropDownList.SelectedValue),
                venderNameTextBox.Text.Trim(),
                vendorAddressTextBox.Text.Trim(),
                vendorCityTextBox.Text.Trim(),
                vendorStateDropDownList.SelectedValue,
                StringHelper.ZeroPad(vendorPhoneRadMaskedTextBox.Text.Trim(), 10),
                StringHelper.ZeroPad(vendorFaxRadMaskedTextBox.Text.Trim(), 10),
                vendorURLTextBox.Text != "http://" ? vendorURLTextBox.Text.Trim() : null,
                vendorNotesTextBox.Text.Trim(),
                DateTime.Now,
                DateTime.Now,
                vendorID
                );
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Subroutine, deletes vendors
    /// </summary>
    /// <param name="id"></param>
    protected void DeleteVendor(int vendorID)
    {
        PurchasingTableAdapters.vendorsTableAdapter vendorAdapter = new PurchasingTableAdapters.vendorsTableAdapter();
        vendorAdapter.Delete(vendorID);
    }

    /// <summary>
    /// Subroutine, clears vendor form
    /// </summary>
    protected void ClearVendorForm()
    {
        vendorTypeDropDownList.SelectedIndex = 0;
        venderNameTextBox.Text = String.Empty;
        vendorAddressTextBox.Text = String.Empty;
        vendorCityTextBox.Text = String.Empty;
        vendorFaxRadMaskedTextBox.Text = String.Empty;
        vendorPhoneRadMaskedTextBox.Text = String.Empty;
        vendorStateDropDownList.SelectedIndex = 5;
        vendorURLTextBox.Text = String.Empty;
        vendorNotesTextBox.Text = String.Empty;
    }

    /// <summary>
    /// Subroutine, trims address string.
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected string TrimAddress(object address, object city, object state)
    {
        string addy = address.ToString() + ", " + city.ToString() + ", " + state.ToString();
        return addy.Length > 30 ? addy.Substring(0, 27) + "..." : addy;
    }

    /// <summary>
    /// Subroutine, returns friendly phone format
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected string TrimPhone(object sender)
    {
        return StringHelper.FormatPhone(sender.ToString(), false);
    }

    /// <summary>
    /// Returns friendly type text
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected string ParseType(object sender)
    {
        PurchaseHelper.OrderType type = (PurchaseHelper.OrderType)Convert.ToInt32(sender);
        if (type == PurchaseHelper.OrderType.DPO)
            return "DPO";
        else if (type == PurchaseHelper.OrderType.Agreement)
            return "Business Agreement";
        else
            return "DRO";
    }

    /// <summary>
    /// Handles error messages
    /// </summary>
    /// <param name="msgText"></param>
    /// <param name="error"></param>
    protected void Msg(string msgText, bool error)
    {
        confirmLabel.Text = msgText;
        confirmLabel.ForeColor = error ? Color.Red : Color.Green;
        vendorMultiView.SetActiveView(View_Confirm);
        Response.AppendHeader("refresh", "3;url=vendors.aspx");
    }

    // Event, raised when grid needs a datasource
    protected void vendorRadGrid_NeedDataSource(object sender, EventArgs e)
    {
        PurchasingTableAdapters.vendorsTableAdapter vendorAdapter = new PurchasingTableAdapters.vendorsTableAdapter();
        vendorRadGrid.DataSource = vendorAdapter.Get();
    }

    // Event, raised on vendorGridView row commands
    protected void vendorRadGrid_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandName == "EditVendor")
        {
            vendorMultiView.SetActiveView(View_AddEdit);
            BindVendor(Convert.ToInt32(e.CommandArgument));
            sectionTitleLabel.Text = "Edit Vendor";
            submitButton.CommandArgument = e.CommandArgument.ToString();
            submitButton.CommandName = "Update";
            submitButton.Text = " Update ";
            clearButton.CommandName = "Cancel";
            clearButton.Text = " Cancel ";
        }
        else if (e.CommandName == "DeleteVendor")
        {
            try
            {
                DeleteVendor(Convert.ToInt32(e.CommandArgument));
                Msg("Vendor successfully deleted.", false);
            }
            catch (Exception)
            {
                Msg("Unable to delete vendor. It is linked to an order in the system.", true);
            }
        }
    }

    // Handles submit button clicks
    protected void submitButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Button btn = sender as Button;
            if (btn.CommandName == "Add")
            {
                if (AddVendor())
                    Msg("Vendor successfully added.", false);
                else
                    Msg("Unable to add vendor.", true);
                BindVendors();

            }
            else if (btn.CommandName == "Update")
            {
                if (UpdateVendor(Convert.ToInt32(btn.CommandArgument)))
                    Msg("Vendor successfully updated.", false);
                else
                    Msg("Unable to update vendor.", true);
                BindVendors();
            }
        }
    }

    // Handles clear button clicks
    protected void clearButton_Click(object sender, EventArgs e)
    {
        Button btn = sender as Button;

        if (btn.CommandName == "Clear")
            ClearVendorForm();
        else if (btn.CommandName == "Cancel")
        {
            sectionTitleLabel.Text = "Vendors";
            vendorMultiView.SetActiveView(View_List);
        }
    }

    // Handles add vendor button clicks
    protected void addLinkButton_Click(object sender, EventArgs e)
    {
        ClearVendorForm();
        submitButton.Text = " Add ";
        submitButton.CommandName = "Add";
        sectionTitleLabel.Text = "Add Vendor";
        vendorMultiView.SetActiveView(View_AddEdit);
        vendorURLTextBox.Text = "http://";
    }
}
