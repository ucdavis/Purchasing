using System;
using System.Configuration;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class business_purchasing_admin_addresses : System.Web.UI.Page
{
    //Page event, raised on page load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PurchaseHelper.UserIsValid(User.Identity.Name))
            Response.Redirect(ConfigurationManager.AppSettings["purchasingRoot"].ToString());

        if (!IsPostBack)
        {
            UCDMenu.BuildBar_3(Menu3); // Build "In this section" Menu
            Menu3.Items[4].Selected = true;
            BindAddresses();
            sectionTitleLabel.Text = "Shipping Addresses";
            addressesMultiView.SetActiveView(View_List);
        }
    }

    /// <summary>
    /// Subroutine, binds address
    /// </summary>
    /// <param name="addressID"></param>
    protected void BindAddresses()
    {
        PurchasingTableAdapters.shipto_addressesTableAdapter shipToAdapter = new PurchasingTableAdapters.shipto_addressesTableAdapter();
        addressRadGrid.DataSource = shipToAdapter.Get();
        addressRadGrid.DataBind();
    }

    /// <summary>
    /// Subroutine, binds address
    /// </summary>
    /// <param name="addressID"></param>
    protected void BindAddress(int addressID)
    {
        PurchasingTableAdapters.shipto_addressesTableAdapter shipToAdapter = new PurchasingTableAdapters.shipto_addressesTableAdapter();
        Purchasing.shipto_addressesDataTable dt = shipToAdapter.GetById(addressID);
        foreach (Purchasing.shipto_addressesRow dr in dt.Rows)
        {
            if (!dr.IsaddressNull())
                addAddressTextBox.Text = dr.address;
            if (!dr.IscampusNull())
                addCampusTextBox.Text = dr.campus;
            addCityTextBox.Text = dr.city;
            ListItem li = addStateDropDownList.Items.FindByValue(dr.state);
            int index = addStateDropDownList.Items.IndexOf(li);
            addStateDropDownList.SelectedIndex = index;
            addStreetTextBox.Text = dr.street;
            addBuildingTextBox.Text = dr.building;
            addRoomTextBox.Text = dr.room;
            string zip = dr.zip;
            addZipRadNumericTextBox.Text = zip.Substring(0, 5);
            addZipPlusRadNumericTextBox.Text = zip.Substring(5, 4);
            addPhoneRadMaskedTextBox.Text = dr.phone;
            updateButton.CommandArgument = addressID.ToString();
        }
    }

    /// <summary>
    /// Subroutine, clears address form
    /// </summary>
    protected void ClearAddressForm()
    {
        addAddressTextBox.Text = String.Empty;
        addCampusTextBox.Text = String.Empty;
        addCityTextBox.Text = String.Empty;
        addStateDropDownList.SelectedIndex = 5;
        addStreetTextBox.Text = String.Empty;
        addBuildingTextBox.Text = String.Empty;
        addRoomTextBox.Text = String.Empty;
        addZipRadNumericTextBox.Text = String.Empty;
        addZipPlusRadNumericTextBox.Text = String.Empty;
        addPhoneRadMaskedTextBox.Text = String.Empty;
    }

    /// <summary>
    /// Subroutine, deletes shipto address by id
    /// </summary>
    /// <param name="addressID"></param>
    protected void DeleteAddress(int addressID)
    {
        PurchasingTableAdapters.shipto_addressesTableAdapter shipToAdapter = new PurchasingTableAdapters.shipto_addressesTableAdapter();
        shipToAdapter.Delete(addressID);
    }

    // Event, raised after grid item commands
    protected void addressRadGrid_ItemCommand(object sender, GridCommandEventArgs e)
    {
        int addressID = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "DeleteAddress")
        {
            try
            {
                DeleteAddress(addressID);
                Response.AppendHeader("refresh", "2;url=addresses.aspx");
                confirmLabel.ForeColor = Color.Green;
                confirmLabel.Text = "Address deleted.";
                addressesMultiView.SetActiveView(View_Confirm);
            }
            catch (Exception)
            {
                Response.AppendHeader("refresh", "2;url=addresses.aspx");
                confirmLabel.ForeColor = Color.Red;
                confirmLabel.Text = "Unable to delete address. It is linked to an order in the system.";
                addressesMultiView.SetActiveView(View_Confirm);
            }
        }
        else if (e.CommandName == "EditAddress")
        {
            BindAddress(addressID);
            sectionTitleLabel.Text = "Edit Address";
            commandMultiView.SetActiveView(View_Command_Update);
            addressesMultiView.SetActiveView(View_AddEdit);
        }
    }

    // Event, raised when grid needs a data source
    protected void addressRadGrid_NeedDataSource(object sender, EventArgs e)
    {
        PurchasingTableAdapters.shipto_addressesTableAdapter shipToAdapter = new PurchasingTableAdapters.shipto_addressesTableAdapter();
        addressRadGrid.DataSource = shipToAdapter.Get();
    }

    // Event, raised after gridview data bound
    protected void GridView_DataBound(object sender, EventArgs e)
    {
        GridView gv = sender as GridView;
        if (gv.Rows.Count == 0)
            gv.CssClass = "formtable bdr";
        else
            gv.CssClass = "formtable";
    }

    // Event, raised after gridview row data bound
    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        CheckBox cb = e.Row.FindControl("approvedCheckBox") as CheckBox;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (cb != null)
                cb.Checked = Convert.ToBoolean((DataBinder.Eval(e.Row.DataItem, "IsApproved")).ToString());

            // Row state styles
            if (e.Row.RowState == DataControlRowState.Normal)
                e.Row.Cells[0].CssClass = "spec";

            else if (e.Row.RowState == DataControlRowState.Alternate)
            {
                foreach (TableCell td in e.Row.Cells)
                    td.CssClass = "alt";
                e.Row.Cells[0].CssClass = "specalt alt";
            }
        }
    }

    // Handles add address linkbutton clicks
    protected void addAddressLinkButton_Click(object sender, EventArgs e)
    {
        ClearAddressForm();
        sectionTitleLabel.Text = "Add Address";
        commandMultiView.SetActiveView(View_Command_Add);
        addressesMultiView.SetActiveView(View_AddEdit);
    }

    // Handles cancel button clicks
    protected void cancelButton_Click(object sender, EventArgs e)
    {
        ClearAddressForm();
        sectionTitleLabel.Text = "Shipping Addresses";
        addressesMultiView.SetActiveView(View_List);
    }

    // Handles add button clicks
    protected void addButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            PurchasingTableAdapters.shipto_addressesTableAdapter shipToAdapter = new PurchasingTableAdapters.shipto_addressesTableAdapter();

            shipToAdapter.Insert(
                addAddressTextBox.Text.Trim(),
                addCampusTextBox.Text.Trim(),
                addStreetTextBox.Text.Trim(),
                addCityTextBox.Text.Trim(),
                addStateDropDownList.SelectedValue,
                StringHelper.ZeroPad(addZipRadNumericTextBox.Text, 5) + StringHelper.ZeroPad(addZipPlusRadNumericTextBox.Text, 4),
                addBuildingTextBox.Text.Trim(),
                addRoomTextBox.Text.Trim(),
                StringHelper.ZeroPad(addPhoneRadMaskedTextBox.Text.Trim(), 10),
                DateTime.Now
                );

            // Confirm
            confirmLabel.Text = "Address successfully added.";
            addressesMultiView.SetActiveView(View_Confirm);
            Response.AppendHeader("refresh", "2;url=addresses.aspx");
        }
    }

    // Handles update button clicks
    protected void updateButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            // Get address id from button's command argument
            Button btn = sender as Button;
            int addressID = Convert.ToInt32(btn.CommandArgument);

            // Update record
            PurchasingTableAdapters.shipto_addressesTableAdapter shipToAdapter = new PurchasingTableAdapters.shipto_addressesTableAdapter();

            shipToAdapter.Update(
                addAddressTextBox.Text.Trim(),
                addCampusTextBox.Text.Trim(),
                addStreetTextBox.Text.Trim(),
                addCityTextBox.Text.Trim(),
                addStateDropDownList.SelectedValue,
                StringHelper.ZeroPad(addZipRadNumericTextBox.Text, 5) + StringHelper.ZeroPad(addZipPlusRadNumericTextBox.Text, 4),
                addBuildingTextBox.Text.Trim(),
                addRoomTextBox.Text.Trim(),
                StringHelper.ZeroPad(addPhoneRadMaskedTextBox.Text.Trim(), 10),
                DateTime.Now,
                addressID
                );

            // Confirm
            confirmLabel.Text = "Address successfully updated";
            addressesMultiView.SetActiveView(View_Confirm);
            Response.AppendHeader("refresh", "2;url=addresses.aspx");
        }
    }
}
