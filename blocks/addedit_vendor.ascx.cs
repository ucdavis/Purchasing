using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class business_purchasing_blocks_addedit_vendor : System.Web.UI.UserControl
{
    /// <summary>
    /// Populates vendor form
    /// </summary>
    /// <param name="vendorID"></param>
    public void PopulateVendorForm(int vendorID)
    {
        //////////////////////////// Vendor ///////////////////////////////
        // Persist id (for updating & submitting later)
        vendorIDHiddenField.Value = vendorID.ToString();

        PurchasingTableAdapters.vendorsTableAdapter vendorAdapter = new PurchasingTableAdapters.vendorsTableAdapter();
        Purchasing.vendorsDataTable vendorTable = vendorAdapter.GetById(vendorID);
        foreach (Purchasing.vendorsRow vendorRow in vendorTable.Rows)
        {
            // Details view
            vendorNameLabel.Text = vendorRow.vendor_name;
            vendorAddressLabel.Text = "<strong>Address:</strong> " + vendorRow.vendor_address + ", " + vendorRow.vendor_city + ", " + vendorRow.vendor_state;
            vendorPhoneLabel.Text = "<strong>Phone:</strong> " + StringHelper.FormatPhone(vendorRow.vendor_phone, true);
            vendorFaxLabel.Text = "<strong>Fax:</strong> " + StringHelper.FormatPhone(vendorRow.vendor_fax, true);

            PurchasingTableAdapters.vendor_customer_numsTableAdapter customerNumsAdapter = new PurchasingTableAdapters.vendor_customer_numsTableAdapter();
            object customerNum = customerNumsAdapter.GetTop(HttpContext.Current.User.Identity.Name, vendorID);
            vendorCustomerTextBox.Text = customerNum != null ? customerNum.ToString() : String.Empty;

            // Add/Edit view
            vendorNameTextBox.Text = vendorRow.vendor_name;
            vendorAddressTextBox.Text = vendorRow.vendor_address;
            vendorCityTextBox.Text = vendorRow.vendor_city;
            vendorFaxRadMaskedTextBox.Text = vendorRow.vendor_fax;
            vendorPhoneRadMaskedTextBox.Text = vendorRow.vendor_phone;
            vendorCustomerAddTextBox.Text = customerNum != null ? customerNum.ToString() : String.Empty;
            if (!vendorRow.Isvendor_urlNull())
                vendorURLTextBox.Text = vendorRow.vendor_url;

            RadComboBoxItem li = vendorStateRadComboBox.Items.FindItemByValue(vendorRow.vendor_state);
            if (li != null)
                vendorStateRadComboBox.SelectedIndex = li.Index;

            vendorMultiView.SetActiveView(View_VendorDetail);
        }
    }

    /// <summary>
    /// Subroutine, clears vendor form
    /// </summary>
    protected void ClearVendorForm()
    {
        vendorCustomerAddTextBox.Text = String.Empty;
        vendorNameTextBox.Text = String.Empty;
        vendorAddressTextBox.Text = String.Empty;
        vendorCityTextBox.Text = String.Empty;
        vendorFaxRadMaskedTextBox.Text = String.Empty;
        vendorPhoneRadMaskedTextBox.Text = String.Empty;
        vendorStateRadComboBox.SelectedIndex = 5; // Default to CA
        vendorCustomerTextBox.Text = String.Empty;
        vendorURLTextBox.Text = String.Empty;
    }

    /// <summary>
    /// Subroutine, prepairs vendor form for new info
    /// </summary>
    protected void PrepVendorForm(string which)
    {
        if (which == "add")
        {
            addEditVendorLabel.Text = "New Vendor";
            backVendorLinkButton.Text = "&lArr; Back To Vendors List";
            backVendorLinkButton.CommandName = "Back";
            addEditVendorSubLabel.Text = "Enter the vendor's information below and continue completing the order form. This infomation will be used when placing the order.";
            vendorMultiView.SetActiveView(View_VendorAddEdit);
            vendorStateRadComboBox.SelectedIndex = 5; // Default to CA
            vendorURLTextBox.Text = "http://";
        }
        else if (which == "edit")
        {
            addEditVendorLabel.Text = "Edit Vendor";
            backVendorLinkButton.Text = "&lArr; Back To Vendor";
            backVendorLinkButton.CommandName = "Update";
            addEditVendorSubLabel.Text = "Edit the vendor's information below and continue completing the order form.";
            vendorMultiView.SetActiveView(View_VendorAddEdit);
        }
    }

    /// <summary>
    /// Subroutine, inserts or updates vendor customer number
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="vendorID"></param>
    /// <param name="customerNum"></param>
    protected void InsertUpdateCustomerNum(string userID, int vendorID, string customerNum)
    {
        PurchasingTableAdapters.vendor_customer_numsTableAdapter customerNumsAdapter = new PurchasingTableAdapters.vendor_customer_numsTableAdapter();

        // Does customer number relationship exist?
        if (PurchaseHelper.CustomerNumberExists(userID, vendorID))
            // Update customer number
            customerNumsAdapter.UpdateNumber(userID, vendorID, customerNum);
        else
        {
            // Insert customer number
            customerNumsAdapter.Insert(
                HttpContext.Current.User.Identity.Name,
                vendorID,
                customerNum
                );
        }
    }

    /// <summary>
    /// Subroutine, handles insert (or) update of vendor info
    /// </summary>
    public int HandleVendor(string authorUserID)
    {
        // Insert/update new vendor and customer number
        if (addEditVendorLabel.Text == "New Vendor")
        {
            int vendorID = InsertVendor();
            if (vendorCustomerAddTextBox.Text.Trim().Length > 0)
                InsertUpdateCustomerNum(authorUserID, vendorID, vendorCustomerAddTextBox.Text.Trim());
            return vendorID;
        }
        else if (addEditVendorLabel.Text == "Edit Vendor")
        {
            int vendorID = Convert.ToInt32(vendorIDHiddenField.Value);
            UpdateVendor(vendorID);
            if (vendorCustomerAddTextBox.Text.Trim().Length > 0)
                InsertUpdateCustomerNum(authorUserID, vendorID, vendorCustomerAddTextBox.Text.Trim());
            return vendorID;
        }
        else if (vendorCustomerTextBox.Text.Trim().Length > 0)
        {
            int vendorID = Convert.ToInt32(vendorIDHiddenField.Value);
            InsertUpdateCustomerNum(authorUserID, vendorID, vendorCustomerTextBox.Text.Trim());
            return vendorID;
        }
        else
            return !String.IsNullOrEmpty(vendorIDHiddenField.Value) ? Convert.ToInt32(vendorIDHiddenField.Value) : 0;
    }

    /// <summary>
    /// Subroutine, binds list of vendors
    /// </summary>
    public void BindVendorsList()
    {
        PurchasingTableAdapters.vendorsTableAdapter vendorAdapter = new PurchasingTableAdapters.vendorsTableAdapter();

        // Set view
        vendorMultiView.SetActiveView(View_VendorList);

        // Bind vendors by type
        vendorRadComboBox.Items.Clear();
        RadComboBoxItem newVendor = new RadComboBoxItem("Type vendor name here", "null");
        vendorRadComboBox.Items.Add(newVendor);

        vendorRadComboBox.DataSource = vendorAdapter.GetByType(GetVendorType());
        vendorRadComboBox.DataBind();

        // Add "New Vendor..." list item
        RadComboBoxItem li = new RadComboBoxItem("New Vendor...", "new");
        li.CssClass = "grn";
        vendorRadComboBox.Items.Add(li);
    }

    /// <summary>
    /// Returns vendor type
    /// </summary>
    /// <returns></returns>
    protected int GetVendorType()
    {
        PurchaseHelper.OrderType type = new PurchaseHelper.OrderType();
        if (Request.Url.ToString().Contains("dpo.aspx"))
            type = PurchaseHelper.OrderType.DPO;
        else if (Request.Url.ToString().Contains("ba.aspx"))
            type = PurchaseHelper.OrderType.Agreement;
        else
            type = PurchaseHelper.OrderType.DRO;
        return Convert.ToInt32(type);
    }

    /// <summary>
    /// Subroutine, binds vendor info to controls
    /// </summary>
    /// <param name="vendorID"></param>
    protected void BindVendor(int vendorID)
    {
        PurchasingTableAdapters.vendor_customer_numsTableAdapter customerNumsAdapter = new PurchasingTableAdapters.vendor_customer_numsTableAdapter();

        // Bind customer number
        object customerNum = customerNumsAdapter.GetTop(HttpContext.Current.User.Identity.Name, vendorID);
        vendorCustomerTextBox.Text =
            vendorCustomerAddTextBox.Text = customerNum != null ? customerNum.ToString() : String.Empty;

        // Bind vendor
        PurchasingTableAdapters.vendorsTableAdapter vendorAdapter = new PurchasingTableAdapters.vendorsTableAdapter();
        Purchasing.vendorsDataTable dt = vendorAdapter.GetById(vendorID);
        foreach (Purchasing.vendorsRow dr in dt.Rows)
        {
            // Persist id (for updating & submitting later)
            vendorIDHiddenField.Value = dr.id.ToString();

            // Details view
            vendorNameLabel.Text = dr.vendor_name;
            vendorAddressLabel.Text = "<strong>Address:</strong> " + dr.vendor_address + ", " + dr.vendor_city + ", " + dr.vendor_state;
            vendorPhoneLabel.Text = "<strong>Phone:</strong> " + StringHelper.FormatPhone(dr.vendor_phone, true);
            vendorFaxLabel.Text = "<strong>Fax:</strong> " + StringHelper.FormatPhone(dr.vendor_fax, true);

            // Edit view
            vendorNameTextBox.Text = dr.vendor_name;
            vendorAddressTextBox.Text = dr.vendor_address;
            vendorCityTextBox.Text = dr.vendor_city;
            vendorFaxRadMaskedTextBox.Text = dr.vendor_fax;
            vendorPhoneRadMaskedTextBox.Text = dr.vendor_phone;
            if (!dr.Isvendor_urlNull())
                vendorURLTextBox.Text = dr.vendor_url;

            RadComboBoxItem li = vendorStateRadComboBox.Items.FindItemByValue(dr.vendor_state);
            if (li != null)
                vendorStateRadComboBox.SelectedIndex = li.Index;
        }
    }

    /// <summary>
    /// Subroutine, adds vendor to db
    /// </summary>
    protected int InsertVendor()
    {
        // Insert vendor
        PurchasingTableAdapters.vendorsTableAdapter vendorAdapter = new PurchasingTableAdapters.vendorsTableAdapter();
        return Convert.ToInt32(vendorAdapter.InsertSelect(
             HttpContext.Current.User.Identity.Name,
             vendorNameTextBox.Text.Trim(),
             vendorAddressTextBox.Text.Trim(),
             vendorCityTextBox.Text.Trim(),
             vendorStateRadComboBox.SelectedValue,
             StringHelper.ZeroPad(vendorPhoneRadMaskedTextBox.Text.Trim(), 10),
             StringHelper.ZeroPad(vendorFaxRadMaskedTextBox.Text.Trim(), 10),
             vendorURLTextBox.Text != "http://" ? vendorURLTextBox.Text.Trim() : null,
             null,
             GetVendorType()
             ));
    }

    /// <summary>
    /// Subroutine, updates vendor
    /// </summary>
    /// <param name="vendorID"></param>
    protected void UpdateVendor(int vendorID)
    {
        // Update vendor
        PurchasingTableAdapters.vendorsTableAdapter vendorAdapter = new PurchasingTableAdapters.vendorsTableAdapter();
        Purchasing.vendorsDataTable dt = vendorAdapter.GetById(vendorID);
        foreach (Purchasing.vendorsRow dr in dt.Rows)
        {
            dr.vendor_name = vendorNameTextBox.Text.Trim();
            dr.vendor_address = vendorAddressTextBox.Text.Trim();
            dr.vendor_city = vendorCityTextBox.Text.Trim();
            dr.vendor_state = vendorStateRadComboBox.SelectedValue;
            dr.vendor_phone = StringHelper.ZeroPad(vendorPhoneRadMaskedTextBox.Text.Trim(), 10);
            dr.vendor_fax = StringHelper.ZeroPad(vendorFaxRadMaskedTextBox.Text.Trim(), 10);
            dr.vendor_url = vendorURLTextBox.Text != "http://" ? vendorURLTextBox.Text.Trim() : null;
            dr.modified = DateTime.Now;
            vendorAdapter.Update(dr);
        }
    }

    /// <summary>
    /// Subroutine, deletes vendor
    /// </summary>
    /// <param name="vendorID"></param>
    protected void DeleteVendor(int vendorID)
    {
        PurchasingTableAdapters.vendorsTableAdapter vendorAdapter = new PurchasingTableAdapters.vendorsTableAdapter();
        vendorAdapter.Delete(vendorID);
    }

    // Event, raised after vendorRadComboBox index changed.
    protected void vendorRadComboBox_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (vendorRadComboBox.SelectedValue == "new")
        {
            // The user wants to enter a new vendor, load the add vendor view
            ClearVendorForm();
            PrepVendorForm("add");
            vendorNameTextBox.Focus();
        }
        else if (vendorRadComboBox.SelectedValue != "null")
        {
            // The user made a selection, show the vendor details
            BindVendor(Convert.ToInt32(vendorRadComboBox.SelectedValue));
            vendorRadComboBox.SelectedIndex = 0;
            vendorMultiView.SetActiveView(View_VendorDetail);
        }
    }

    // Handles switching view to "add vendor" view
    protected void addVendorLinkButton_Click(object sender, EventArgs e)
    {
        ClearVendorForm();
        PrepVendorForm("add");
        vendorNameTextBox.Focus();
    }

    // Handles switching view to "edit vendor" view
    protected void editVendorLinkButton_Click(object sender, EventArgs e)
    {
        PrepVendorForm("edit");
    }

    // Handles switching view back to "list vendors" view
    protected void changeVendorLinkButton_Click(object sender, EventArgs e)
    {
        vendorRadComboBox.Text = String.Empty;
        vendorRadComboBox.SelectedIndex = 0;
        vendorMultiView.SetActiveView(View_VendorList);
    }

    // Handles switching view back to "list vendors" view
    protected void backVendorLinkButton_Click(object sender, EventArgs e)
    {
        LinkButton btn = sender as LinkButton;
        if (btn.CommandName == "Back")
        {
            vendorRadComboBox.Text = String.Empty;
            vendorRadComboBox.SelectedIndex = 0;
            vendorMultiView.SetActiveView(View_VendorList);
        }
        else if (btn.CommandName == "Update")
        {
            UpdateVendor(Convert.ToInt32(vendorIDHiddenField.Value));
            BindVendor(Convert.ToInt32(vendorIDHiddenField.Value));
            vendorMultiView.SetActiveView(View_VendorDetail);
        }
    }
}
