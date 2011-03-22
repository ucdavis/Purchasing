using System;
using System.Configuration;
using System.Data;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class business_purchasing_orders_myprofile : System.Web.UI.Page
{
    //Page event, raised on page load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PurchaseHelper.UserIsValid(User.Identity.Name))
            Response.Redirect(ConfigurationManager.AppSettings["purchasingRoot"].ToString());

        if (!IsPostBack)
        {
            UCDMenu.BuildBar_3(Menu3); // Build "In this section" Menu
            HandleIE();
            BindUnits();
            BindShipToAddresses();
            BindUser(User.Identity.Name);
            BindBackupUsers();

            profileMultiView.SetActiveView(View_Profile);

            // Disable username textbox
            useridTextBox.Attributes.Add("disabled", "true");
        }
    }

    /// <summary>
    /// Subroutine, handles IE browser
    /// </summary>
    protected void HandleIE()
    {
        System.Web.HttpBrowserCapabilities browser = Request.Browser;
        if (browser.Browser == "IE")
            RadFormDecorator1.Visible = false;
    }

    /// <summary>
    /// Subroutine, binds list of units to ddl
    /// </summary>
    protected void BindUnits()
    {
        PurchasingTableAdapters.unitsTableAdapter unitsAdapter = new PurchasingTableAdapters.unitsTableAdapter();
        unitsRadComboBox.DataSource = unitsAdapter.Get();
        unitsRadComboBox.DataBind();
    }

    /// <summary>
    /// Binds user's list of backup users
    /// </summary>
    /// <param name="userid"></param>
    protected void BindBackupUsers()
    {
        string[] roles = new string[] { "PurchaseAdmin", "PurchaseUser", "PurchaseApprover", "PurchaseManager" };
        DataView view = Users.GetUsersByRoleNames(roles, false).Tables[0].DefaultView;
        view.Sort = "LastName";

        backupRadComboBox.DataSource = view;
        backupRadComboBox.DataTextField = "ListName";
        backupRadComboBox.DataValueField = "UserName";
        backupRadComboBox.DataBind();

        PurchasingTableAdapters.user_profilesTableAdapter profileAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
        int profileID = Convert.ToInt32(profileAdapter.GetIdByUsername(User.Identity.Name));
        PurchasingTableAdapters.user_profile_relTableAdapter profileRelAdapter = new PurchasingTableAdapters.user_profile_relTableAdapter();
        backupRepeater.DataSource = profileRelAdapter.GetByProfileId(profileID);
        backupRepeater.DataBind();

    }

    /// <summary>
    /// Returns backup user's full name
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    protected string GetFullName(object id)
    {
        string FullName = String.Empty;

        if (id != null)
        {
            int userID = Convert.ToInt32(id);
            if (userID != 0)
            {
                PurchasingTableAdapters.user_profilesTableAdapter profileAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
                string username = profileAdapter.GetUsernameById(userID).ToString();
                FullName = Users.GetFullNameByUserName(username, true, false);
            }
        }

        return FullName;
    }

    /// <summary>
    /// Subroutine, binds user to controls, returns success
    /// </summary>
    /// <param name="userid"></param>
    /// <returns></returns>
    protected void BindUser(string userid)
    {
        useridTextBox.Text = User.Identity.Name;

        // Bind membership data
        fnameTextBox.Text = Users.GetFirstNameByUserName(userid);
        lnameTextBox.Text = Users.GetLastNameByUserName(userid);
        emailTextBox.Text = Users.GetEmailByUserName(userid);

        // Bind profile data
        PurchasingTableAdapters.user_profilesTableAdapter profileAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
        Purchasing.user_profilesDataTable dt = profileAdapter.GetByUser(userid);
        foreach (Purchasing.user_profilesRow dr in dt.Rows)
        {
            useridTextBox.Text = dr.userid;
            if (!dr.IsphoneNull())
                phoneRadMaskedTextBox.Text = dr.phone;

            RadComboBoxItem li = unitsRadComboBox.Items.FindItemByValue(dr.unit_id.ToString());
            if (li != null)
                unitsRadComboBox.SelectedIndex = li.Index;

            RadComboBoxItem li2 = shiptoRadComboBox.Items.FindItemByValue(dr.shipto_id.ToString());
            if (li2 != null)
                shiptoRadComboBox.SelectedIndex = li2.Index;
        }
    }

    /// <summary>
    /// Subroutine, updates user profile
    /// </summary>
    /// <param name="userid"></param>
    protected void UpdateUser(string userid)
    {
        // Update e-mail address and role
        MembershipUser usr = Membership.GetUser(userid);
        usr.Email = emailTextBox.Text.Trim();
        Membership.UpdateUser(usr);

        // Update name
        Users.UpdateNameByUserName(userid, fnameTextBox.Text.Trim(), lnameTextBox.Text.Trim());

        // Update profile
        PurchasingTableAdapters.user_profilesTableAdapter profileAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
        Purchasing.user_profilesDataTable dt = profileAdapter.GetByUser(userid);
        foreach (Purchasing.user_profilesRow dr in dt.Rows)
        {
            dr.userid = userid;
            dr.unit_id = Convert.ToInt32(unitsRadComboBox.SelectedValue);
            dr.shipto_id = InsertAddress();
            dr.phone = phoneRadMaskedTextBox.Text.Trim().Length > 0 ? StringHelper.ZeroPad(phoneRadMaskedTextBox.Text.Trim(), 10) : String.Empty;
            profileAdapter.Update(dr);
        }
    }

    /// <summary>
    /// Subroutine, inserts user profile, returns bool
    /// </summary>
    /// <returns></returns>
    protected bool InsertProfile()
    {
        bool check = false;

        if (!PurchaseHelper.UserProfileExists(User.Identity.Name))
        {
            PurchasingTableAdapters.user_profilesTableAdapter profileAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
            profileAdapter.Insert(
                false,
                User.Identity.Name,
                Convert.ToInt32(unitsRadComboBox.SelectedValue),
                InsertAddress(),
                StringHelper.ZeroPad(phoneRadMaskedTextBox.Text.Trim(), 10),
                DateTime.Now
                );

            check = true;
        }

        return check;
    }

    /// <summary>
    /// Subroutine, binds list of addresses
    /// </summary>
    protected void BindShipToAddresses()
    {
        // Set view
        shiptoMultiView.SetActiveView(View_ShipTo_List);

        // Bind addresses
        PurchasingTableAdapters.shipto_addressesTableAdapter shipToAdapter = new PurchasingTableAdapters.shipto_addressesTableAdapter();
        shiptoRadComboBox.DataSource = shipToAdapter.Get();
        shiptoRadComboBox.DataBind();

        // Add "Other..." list item
        RadComboBoxItem li = new RadComboBoxItem("Other...", "other");
        li.CssClass = "grn";
        shiptoRadComboBox.Items.Add(li);
    }

    /// <summary>
    /// Subroutine, returns id of shipto address (inserting address if nessisarry)
    /// </summary>
    /// <returns></returns>
    protected int InsertAddress()
    {
        int shipToID = 0;

        // Check if we are adding a new address
        if (shiptoMultiView.ActiveViewIndex == 1)
        {
            PurchasingTableAdapters.shipto_addressesTableAdapter shipToAdapter = new PurchasingTableAdapters.shipto_addressesTableAdapter();
            shipToID = Convert.ToInt32(shipToAdapter.InsertSelect(
                shiptoAddressTextBox.Text.Trim(),
                shiptoCampusTextBox.Text.Trim(),
                shiptoStreetTextBox.Text.Trim(),
                shiptoCityTextBox.Text.Trim(),
                shiptoStateRadComboBox.SelectedValue,
                StringHelper.ZeroPad(shiptoZipRadNumericTextBox.Text.Trim(), 5) + StringHelper.ZeroPad(shiptoZipPlusRadNumericTextBox.Text.Trim(), 4),
                shiptoBuildingTextBox.Text.Trim(),
                shiptoRoomTextBox.Text.Trim(),
                StringHelper.ZeroPad(shiptoPhoneRadMaskedTextBox.Text.Trim(), 10)
                ));
        }
        else
            // They selected an address from the list, use that id
            shipToID = Convert.ToInt32(shiptoRadComboBox.SelectedValue);

        return shipToID;
    }

    // Event, raised on shiptoRadComboBox selected index changed
    protected void shiptoRadComboBox_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (e.Value == "other")
        {
            shiptoStateRadComboBox.SelectedIndex = 4;
            shiptoCampusTextBox.Text = "University of California, Davis";
            shiptoCityTextBox.Text = "Davis";
            shiptoMultiView.SetActiveView(View_ShipTo_New);
        }
    }

    // Handles add user button clicks
    protected void updateUserButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            UpdateUser(User.Identity.Name);
            profileMultiView.SetActiveView(View_Confirm);
            confirmLabel.Text = "Profile successfully updated.";
            Response.AppendHeader("refresh", "3;url=myprofile.aspx");
        }
    }

    // Handles deleting backup user
    protected void deleteBackupImageButton_Click(object sender, EventArgs e)
    {
        ImageButton ib = sender as ImageButton;
        PurchasingTableAdapters.user_profile_relTableAdapter profileRelAdapter = new PurchasingTableAdapters.user_profile_relTableAdapter();
        profileRelAdapter.Delete(Convert.ToInt32(ib.CommandArgument));
        BindBackupUsers();
    }

    // Handles adding backup user
    protected void addBackupButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            // Get current user's id
            string username = useridTextBox.Text.Trim();
            PurchasingTableAdapters.user_profilesTableAdapter profileAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
            int profileID = Convert.ToInt32(profileAdapter.GetIdByUsername(username));

            // Get backup user's id
            string backupUsername = backupRadComboBox.SelectedValue;
            int backupProfileID = Convert.ToInt32(profileAdapter.GetIdByUsername(backupUsername));

            // If they aren't already in the db, add the relationship
            PurchasingTableAdapters.user_profile_relTableAdapter profileRelAdapter = new PurchasingTableAdapters.user_profile_relTableAdapter();
            Purchasing.user_profile_relDataTable dt = profileRelAdapter.GetByProfileIdAndBackupProfileId(profileID, backupProfileID);
            if (dt.Rows.Count == 0 && profileID != backupProfileID && backupProfileID != 0)
                profileRelAdapter.Insert(profileID, backupProfileID);

            BindBackupUsers();
        }
    }
}
