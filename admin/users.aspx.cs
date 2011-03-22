using System;
using System.Configuration;
using System.Data;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class business_purchasing_admin_users : System.Web.UI.Page
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
            BindUnits();
            BindShipToAddresses();

            if (Request.QueryString["username"] != null)
            {
                string username = Request.QueryString["username"].ToString();
                BindUser(username);
                BindBackupUsers();
                string editMode = PurchaseHelper.UserProfileExists(username) ? "edit" : "update";
                AlterUserView(editMode, username);
            }
            else
            {
                string[] roleNames = new string[] { "PurchaseUser", "PurchaseApprover", "PurchaseAdmin", "PurchaseManager" };
                BindUsers(roleNames);
                sectionTitleLabel.Text = "Users";
                usersMultiView.SetActiveView(View_List);
            }
        }
    }

    /// <summary>
    /// Subroutine, binds purchase users to gridview
    /// </summary>
    protected void BindUsers(string[] roleNames)
    {
        usersRadGrid.MasterTableView.NoMasterRecordsText = "There are no users at this time.";
        usersRadGrid.DataSource = Users.GetUsersByRoleNames(roleNames, true);
        usersRadGrid.DataBind();
    }

    /// <summary>
    /// Subroutine, clears address form
    /// </summary>
    protected void ClearUserForm()
    {
        useridTextBox.Text = String.Empty;
        fnameTextBox.Text = String.Empty;
        lnameTextBox.Text = String.Empty;
        emailTextBox.Text = String.Empty;
        phoneRadMaskedTextBox.Text = String.Empty;
        unitsRadComboBox.SelectedIndex = 0;
        shiptoRadComboBox.SelectedIndex = 0;
        unitAbrvTextBox.Text = String.Empty;
        unitFaxRadMaskedTextBox.Text = String.Empty;
        unitNameTextBox.Text = String.Empty;
        shiptoDisplayAddressTextBox.Text = String.Empty;
        shiptoCampusTextBox.Text = String.Empty;
        shiptoCityTextBox.Text = String.Empty;
        shiptoStateDropDownList.SelectedIndex = 5; // Default to CA
        shiptoStreetTextBox.Text = String.Empty;
        shiptoBuildingTextBox.Text = String.Empty;
        shiptoRoomTextBox.Text = String.Empty;
        shiptoZipRadNumericTextBox.Text = String.Empty;
        shiptoZipPlusRadNumericTextBox.Text = String.Empty;
        shiptoPhoneRadMaskedTextBox.Text = String.Empty;
    }

    /// <summary>
    /// Subroutine, binds user to controls
    /// </summary>
    /// <param name="userid"></param>
    protected void BindUser(string userid)
    {
        PurchasingTableAdapters.user_profilesTableAdapter profilesAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();

        // Membership data
        useridTextBox.Text = userid;
        fnameTextBox.Text = Users.GetFirstNameByUserName(userid);
        lnameTextBox.Text = Users.GetLastNameByUserName(userid);
        emailTextBox.Text = Users.GetEmailByUserName(userid);

        purchaserRadioButton.Checked = Roles.IsUserInRole(userid, "PurchaseUser");
        approverRadioButton.Checked = Roles.IsUserInRole(userid, "PurchaseApprover");
        adminRadioButton.Checked = Roles.IsUserInRole(userid, "PurchaseAdmin");
        managerRadioButton.Checked = Roles.IsUserInRole(userid, "PurchaseManager");

        Purchasing.user_profilesDataTable dt = profilesAdapter.GetByUser(userid);
        foreach (Purchasing.user_profilesRow dr in dt.Rows)
        {
            // Profile data
            if (!dr.IsphoneNull())
                phoneRadMaskedTextBox.Text = dr.phone;

            awayCheckBox.Checked = dr.out_of_office;

            if (!dr.Isshipto_idNull())
            {
                RadComboBoxItem rcbi = shiptoRadComboBox.Items.FindItemByValue(dr.shipto_id.ToString());
                if (rcbi != null)
                    shiptoRadComboBox.SelectedIndex = rcbi.Index;
            }

            RadComboBoxItem li = unitsRadComboBox.Items.FindItemByValue(dr.unit_id.ToString());
            if (li != null)
                unitsRadComboBox.SelectedIndex = li.Index;
        }
    }

    /// <summary>
    /// Binds user's list of backup users
    /// </summary>
    /// <param name="userid"></param>
    protected void BindBackupUsers()
    {
        PurchasingTableAdapters.user_profilesTableAdapter profilesAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();

        string[] roles = new string[] { "PurchaseAdmin", "PurchaseUser", "PurchaseApprover", "PurchaseManager" };
        DataView view = Users.GetUsersByRoleNames(roles, true).Tables[0].DefaultView;
        view.Sort = "LastName";

        backupRadComboBox.DataSource = view;
        backupRadComboBox.DataTextField = "ListName";
        backupRadComboBox.DataValueField = "UserName";
        backupRadComboBox.DataBind();

        if (Request.QueryString["username"] != null)
        {
            string username = Request.QueryString["username"].ToString();
            int profileID = Convert.ToInt32(profilesAdapter.GetIdByUsername(username));

            PurchasingTableAdapters.user_profile_relTableAdapter profileRelAdapter = new PurchasingTableAdapters.user_profile_relTableAdapter();
            backupRepeater.DataSource = profileRelAdapter.GetByProfileId(profileID);
            backupRepeater.DataBind();
        }
    }

    /// <summary>
    /// Subroutine, deletes user (including orders, profile, customer nums etc...)
    /// </summary>
    /// <param name="userid"></param>
    protected void DeleteUser(string userid)
    {
        // Disapprove user
        MembershipUser usr = Membership.GetUser(userid);
        usr.IsApproved = false;
        Membership.UpdateUser(usr);

        // Remove user role(s)
        if (Roles.IsUserInRole(userid, "PurchaseUser"))
            Roles.RemoveUserFromRole(userid, "PurchaseUser");
        if (Roles.IsUserInRole(userid, "PurchaseApprover"))
            Roles.RemoveUserFromRole(userid, "PurchaseApprover");
        if (Roles.IsUserInRole(userid, "PurchaseAdmin"))
            Roles.RemoveUserFromRole(userid, "PurchaseAdmin");
        if (Roles.IsUserInRole(userid, "PurchaseManager"))
            Roles.RemoveUserFromRole(userid, "PurchaseManager");

        // Delete delegations (if any)
        PurchasingTableAdapters.user_profilesTableAdapter profilesAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
        PurchasingTableAdapters.delegatesTableAdapter delegatesAdapter = new PurchasingTableAdapters.delegatesTableAdapter();
        delegatesAdapter.DeleteByProfileId(Convert.ToInt32(profilesAdapter.GetIdByUsername(userid)));

        // Remove profile
        profilesAdapter.DeleteByUserId(userid);

        usersMultiView.SetActiveView(View_Confirm);
        confirmLabel.Text = "User successfully removed";
        Response.AppendHeader("refresh", "2;url=users.aspx");
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

        // Assign user role
        if (purchaserRadioButton.Checked && !Roles.IsUserInRole(userid, "PurchaseUser"))
        {
            if (Roles.IsUserInRole(userid, "PurchaseApprover"))
                Roles.RemoveUserFromRole(userid, "PurchaseApprover");
            if (Roles.IsUserInRole(userid, "PurchaseAdmin"))
                Roles.RemoveUserFromRole(userid, "PurchaseAdmin");
            if (Roles.IsUserInRole(userid, "PurchaseManager"))
                Roles.RemoveUserFromRole(userid, "PurchaseManager");
            Roles.AddUserToRole(userid, "PurchaseUser");
        }
        else if (approverRadioButton.Checked && !Roles.IsUserInRole(userid, "PurchaseApprover"))
        {
            if (Roles.IsUserInRole(userid, "PurchaseUser"))
                Roles.RemoveUserFromRole(userid, "PurchaseUser");
            if (Roles.IsUserInRole(userid, "PurchaseAdmin"))
                Roles.RemoveUserFromRole(userid, "PurchaseAdmin");
            if (Roles.IsUserInRole(userid, "PurchaseManager"))
                Roles.RemoveUserFromRole(userid, "PurchaseManager");
            Roles.AddUserToRole(userid, "PurchaseApprover");
        }
        else if (adminRadioButton.Checked && !Roles.IsUserInRole(userid, "PurchaseAdmin"))
        {
            if (Roles.IsUserInRole(userid, "PurchaseUser"))
                Roles.RemoveUserFromRole(userid, "PurchaseUser");
            if (Roles.IsUserInRole(userid, "PurchaseApprover"))
                Roles.RemoveUserFromRole(userid, "PurchaseApprover");
            if (Roles.IsUserInRole(userid, "PurchaseManager"))
                Roles.RemoveUserFromRole(userid, "PurchaseManager");
            Roles.AddUserToRole(userid, "PurchaseAdmin");
        }
        else if (managerRadioButton.Checked && !Roles.IsUserInRole(userid, "PurchaseManager"))
        {
            if (Roles.IsUserInRole(userid, "PurchaseUser"))
                Roles.RemoveUserFromRole(userid, "PurchaseUser");
            if (Roles.IsUserInRole(userid, "PurchaseApprover"))
                Roles.RemoveUserFromRole(userid, "PurchaseApprover");
            if (Roles.IsUserInRole(userid, "PurchaseAdmin"))
                Roles.RemoveUserFromRole(userid, "PurchaseAdmin");
            Roles.AddUserToRole(userid, "PurchaseManager");
        }

        // Update name
        Users.UpdateNameByUserName(userid, fnameTextBox.Text.Trim(), lnameTextBox.Text.Trim());
    }

    /// <summary>
    /// Subroutine, updates user profile
    /// </summary>
    /// <param name="userid"></param>
    protected void UpdateProfile(string userid)
    {
        PurchasingTableAdapters.user_profilesTableAdapter profilesAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();

        // Update profile
        Purchasing.user_profilesDataTable dt = profilesAdapter.GetByUser(userid);
        foreach (Purchasing.user_profilesRow dr in dt.Rows)
        {
            dr.unit_id = InsertUnit();
            dr.shipto_id = InsertAddress();
            dr.phone = StringHelper.ZeroPad(phoneRadMaskedTextBox.Text.Trim(), 10);
            dr.dt_stamp = DateTime.Now;
            profilesAdapter.Update(dr);
        }
    }

    /// <summary>
    /// Subroutine, inserts user profile
    /// </summary>
    protected void InsertProfile()
    {
        if (!PurchaseHelper.UserProfileExists(useridTextBox.Text.Trim()))
        {
            PurchasingTableAdapters.user_profilesTableAdapter profilesAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();

            profilesAdapter.Insert(
                false,
                useridTextBox.Text.Trim(),
                InsertUnit(),
                InsertAddress(),
                StringHelper.ZeroPad(phoneRadMaskedTextBox.Text.Trim(), 10),
                DateTime.Now
                );
        }
    }

    /// <summary>
    /// Subroutine, adds new user
    /// </summary>
    protected void AddUser()
    {
        // Create new user
        MembershipCreateStatus status = new MembershipCreateStatus();
        Membership.CreateUser(
            useridTextBox.Text.Trim(),
            "987%DF65_@IUH!IJ",
            emailTextBox.Text.Trim(),
            "question",
            "answer",
            true,
            out status
        );

        if (status.ToString() == "Success")
        {
            // User successfully created, update name and insert profile
            Users.UpdateNameByUserName(useridTextBox.Text.Trim(), fnameTextBox.Text.Trim(), lnameTextBox.Text.Trim());

            // Add user to role
            string[] roles = new string[] { "PurchaseUser", "PurchaseApprover", "PurchaseAdmin", "PurchaseManager" };

            if (!Users.UserIsInRoles(useridTextBox.Text.Trim(), roles))
            {
                if (purchaserRadioButton.Checked)
                    Roles.AddUserToRole(useridTextBox.Text.Trim(), "PurchaseUser");
                else if (approverRadioButton.Checked)
                    Roles.AddUserToRole(useridTextBox.Text.Trim(), "PurchaseApprover");
                else if (adminRadioButton.Checked)
                    Roles.AddUserToRole(useridTextBox.Text.Trim(), "PurchaseAdmin");
                else if (managerRadioButton.Checked)
                    Roles.AddUserToRole(useridTextBox.Text.Trim(), "PurchaseManager");
            }
        }
        else
        {
            // User exists in ASPNETDB, but lacks a profile... update membership then insert profile
            UpdateUser(useridTextBox.Text.Trim());
        }

        // Insert profile
        InsertProfile();
    }

    /// <summary>
    /// Subroutine, displays add/edit/update mode user view
    /// </summary>
    /// <param name="mode">add/edit/update</param>
    /// <param name="userID"></param>
    protected void AlterUserView(string mode, string userID)
    {
        switch (mode)
        {
            case "edit":
                useridTextBox.ReadOnly = true;
                backupPanel.Visible = true;
                useridTextBox.Attributes["disabled"] = "true";
                useridRequiredFieldValidator.Visible = false;
                sectionTitleLabel.Text = "Edit User";
                cancelUserButton.Visible = true;
                insertUpdateUserButton.Text = " Update ";
                insertUpdateUserButton.CommandName = "Update";
                useridRequiredFieldValidator.Visible = false;
                insertUpdateUserButton.CommandArgument = userID;
                usersMultiView.SetActiveView(View_AddEdit);
                customerNumbersPanel.Visible = true;
                BindCustomerNumbers(userID);
                break;
            case "update":
                useridTextBox.ReadOnly = true;
                backupPanel.Visible = true;
                useridTextBox.Attributes["disabled"] = "true";
                useridRequiredFieldValidator.Visible = false;
                sectionTitleLabel.Text = "Add Profile";
                insertUpdateUserButton.Text = " Add Profile ";
                insertUpdateUserButton.CommandName = "AddProfile";
                customerNumbersPanel.Visible = false;
                usersMultiView.SetActiveView(View_AddEdit);
                break;
            case "add":
                ClearUserForm();
                backupPanel.Visible = false;
                sectionTitleLabel.Text = "Add User";
                useridTextBox.ReadOnly = false;
                useridTextBox.Attributes.Remove("disabled");
                insertUpdateUserButton.Text = " Add User ";
                insertUpdateUserButton.CommandName = "Add";
                useridRequiredFieldValidator.Visible = true;
                customerNumbersPanel.Visible = false;
                usersMultiView.SetActiveView(View_AddEdit);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Subroutine, binds list of customer numbers
    /// </summary>
    /// <param name="userid"></param>
    protected void BindCustomerNumbers(string userid)
    {
        PurchasingTableAdapters.vendor_customer_numsTableAdapter customerNumsAdapter = new PurchasingTableAdapters.vendor_customer_numsTableAdapter();

        customerNumbersPanel.Visible = true;
        customerNumbersGridView.DataSource = customerNumsAdapter.GetByUser(userid);
        customerNumbersGridView.DataBind();
    }

    /// <summary>
    /// Subroutine, returns friendly role name
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected string ParseRole(object sender)
    {
        string user = sender.ToString();
        if (Roles.IsUserInRole(user, "PurchaseUser"))
            return "Requester";
        else if (Roles.IsUserInRole(user, "PurchaseApprover"))
            return "P.I.";
        else
            return "Purchaser";
    }

    /// <summary>
    /// Returns user's phone
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected string GetPhoneFromProfile(object sender)
    {
        PurchasingTableAdapters.user_profilesTableAdapter profilesAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();

        string userid = sender.ToString();
        string phone = String.Empty;
        Purchasing.user_profilesDataTable dt = profilesAdapter.GetByUser(userid);
        foreach (Purchasing.user_profilesRow dr in dt.Rows)
        {
            if (!dr.IsphoneNull())
                phone = StringHelper.FormatPhone(dr.phone, false);
        }
        return phone;
    }

    /// <summary>
    /// Subroutine, binds list of addresses
    /// </summary>
    protected void BindShipToAddresses()
    {
        PurchasingTableAdapters.shipto_addressesTableAdapter shipToAdapter = new PurchasingTableAdapters.shipto_addressesTableAdapter();

        // Set view
        shiptoMultiView.SetActiveView(View_ShipTo_List);

        // Bind addresses
        shiptoRadComboBox.DataSource = shipToAdapter.Get();
        shiptoRadComboBox.DataBind();

        // Add "other" list item
        RadComboBoxItem li = new RadComboBoxItem("Other...", "other");
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
                shiptoDisplayAddressTextBox.Text.Trim(),
                shiptoCampusTextBox.Text.Trim(),
                shiptoStreetTextBox.Text.Trim(),
                shiptoCityTextBox.Text.Trim(),
                shiptoStateDropDownList.SelectedValue,
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

    /// <summary>
    /// Subroutine, binds list of units to ddl
    /// </summary>
    protected void BindUnits()
    {
        PurchasingTableAdapters.unitsTableAdapter unitAdapter = new PurchasingTableAdapters.unitsTableAdapter();

        // Set view
        unitsMultiView.SetActiveView(View_Unit_List);

        // Bind units
        unitsRadComboBox.DataSource = unitAdapter.Get();
        unitsRadComboBox.DataBind();

        // Add "other" list item
        RadComboBoxItem li = new RadComboBoxItem("Other...", "other");
        unitsRadComboBox.Items.Add(li);
    }

    /// <summary>
    /// Subroutine, returns id of unit (inserting unit if nessisarry)
    /// </summary>
    /// <returns></returns>
    protected int InsertUnit()
    {
        PurchasingTableAdapters.unitsTableAdapter unitAdapter = new PurchasingTableAdapters.unitsTableAdapter();

        int unitID = 0;

        // If user choose to enter a new unit
        if (unitsMultiView.ActiveViewIndex == 1)
        {
            unitID = Convert.ToInt32(unitAdapter.InsertSelect(
                unitNameTextBox.Text.Trim(),
                unitAbrvTextBox.Text.Trim(),
                unitFaxRadMaskedTextBox.Text.Trim()
                ));
        }
        else
            unitID = Convert.ToInt32(unitsRadComboBox.SelectedValue);

        return unitID;
    }

    /// <summary>
    /// Returns backup user's full name
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    protected string GetFullName(object id)
    {
        PurchasingTableAdapters.user_profilesTableAdapter profilesAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
        string username = profilesAdapter.GetUsernameById(Convert.ToInt32(id)).ToString();
        return Users.GetFullNameByUserName(username, true, false);
    }

    // Event, raised on unitsRadComboBox selected index changed
    protected void unitsRadComboBox_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (e.Value == "other")
            unitsMultiView.SetActiveView(View_Unit_New);
    }

    // Event, raised on users rad grid init
    protected void usersRadGrid_Init(object sender, EventArgs e)
    {
        GridFilterMenu menu = usersRadGrid.FilterMenu;
        int i = 0;
        while (i < menu.Items.Count)
        {
            if (
                menu.Items[i].Text == "NoFilter" ||
                menu.Items[i].Text == "Contains" ||
                menu.Items[i].Text == "DoesNotContain" ||
                menu.Items[i].Text == "StartsWith" ||
                menu.Items[i].Text == "EndsWith" ||
                menu.Items[i].Text == "EqualTo" ||
                menu.Items[i].Text == "NotEqualTo"
                )
            {
                i++;
            }
            else
            {
                menu.Items.RemoveAt(i);
            }
        }
    }

    // Event, raised after row (item) commands
    protected void usersRadGrid_ItemCommand(object source, GridCommandEventArgs e)
    {
        if (e.CommandName == "DeleteUser")
            DeleteUser(e.CommandArgument.ToString());
    }

    // Event, raised when users grid needs a datasource
    protected void usersRadGrid_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        string[] roleNames = new string[] { "PurchaseUser", "PurchaseApprover", "PurchaseAdmin", "PurchaseManager" };
        usersRadGrid.DataSource = Users.GetUsersByRoleNames(roleNames, true);
    }

    // Event, raised after gridview row (item) data bound
    protected void usersRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        CheckBox cb = e.Item.FindControl("approvedCheckBox") as CheckBox;
        if (cb != null && (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem))
            cb.Checked = Convert.ToBoolean((DataBinder.Eval(e.Item.DataItem, "IsApproved")).ToString());

        HiddenField hf = e.Item.FindControl("userNameHiddenField") as HiddenField;
        CheckBox outOfOfficeCb = e.Item.FindControl("outOfOfficeCheckBox") as CheckBox;
        if (hf != null && outOfOfficeCb != null)
        {
            PurchasingTableAdapters.user_profilesTableAdapter profilesAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
            Purchasing.user_profilesDataTable dt = profilesAdapter.GetByUser(hf.Value);
            foreach (Purchasing.user_profilesRow dr in dt.Rows)
                outOfOfficeCb.Checked = dr.out_of_office;
        }
    }

    // Event, raised on shiptoRadComboBox selected index changed
    protected void shiptoRadComboBox_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (e.Value == "other")
        {
            shiptoMultiView.SetActiveView(View_ShipTo_New);
            shiptoStateDropDownList.SelectedIndex = 5;
        }
    }

    // Event, raised after approvedCheckBox check change
    protected void approvedCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        // Get username
        CheckBox cb = sender as CheckBox;
        HiddenField hf = cb.NamingContainer.FindControl("userNameHiddenField") as HiddenField;

        // Update user approval status
        MembershipUser user = Membership.GetUser(hf.Value);
        user.IsApproved = cb.Checked;
        Membership.UpdateUser(user);

        // Bind list of users
        string[] roleNames = new string[] { "PurchaseUser", "PurchaseApprover", "PurchaseAdmin", "PurchaseManager" };
        sectionTitleLabel.Text = "Users";
        BindUsers(roleNames);
    }

    // Event, raised after outOfOfficeCheckBox check change
    protected void outOfOfficeCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        // Get username
        CheckBox cb = sender as CheckBox;
        HiddenField hf = cb.NamingContainer.FindControl("userNameHiddenField2") as HiddenField;

        // Update out of office status
        PurchasingTableAdapters.user_profilesTableAdapter profilesAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
        Purchasing.user_profilesDataTable dt = profilesAdapter.GetByUser(hf.Value);
        foreach (Purchasing.user_profilesRow dr in dt.Rows)
        {
            dr.out_of_office = cb.Checked;
            profilesAdapter.Update(dr);
        }

        // Bind list of users
        string[] roleNames = new string[] { "PurchaseUser", "PurchaseApprover", "PurchaseAdmin", "PurchaseManager" };
        sectionTitleLabel.Text = "Users";
        BindUsers(roleNames);
    }

    // Event, raised after awayCheckBox check change
    protected void awayCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox cb = sender as CheckBox;
        if (Request.QueryString["username"] != null)
        {
            string username = Request.QueryString["username"].ToString();
            PurchasingTableAdapters.user_profilesTableAdapter profilesAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
            Purchasing.user_profilesDataTable dt = profilesAdapter.GetByUser(username);
            foreach (Purchasing.user_profilesRow dr in dt.Rows)
            {
                dr.out_of_office = cb.Checked;
                profilesAdapter.Update(dr);
            }

            BindUser(username);
        }
    }

    // Handles add user button clicks
    protected void insertUpdateUserButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Button btn = sender as Button;
            if (btn.CommandName == "Update")
            {
                // Update user
                UpdateUser(btn.CommandArgument);
                UpdateProfile(btn.CommandArgument);
                confirmLabel.Text = "User successfully updated";
            }
            else if (btn.CommandName == "AddProfile")
            {
                UpdateUser(btn.CommandArgument);
                confirmLabel.Text = "User profile successfully added"; ;
            }
            else if (btn.CommandName == "Add")
            {
                AddUser();
                confirmLabel.Text = "User successfully added";
            }
            usersMultiView.SetActiveView(View_Confirm);
            Response.AppendHeader("refresh", "2;url=users.aspx");
        }
    }

    // Handles switching view to user view
    protected void cancelUserButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("users.aspx");
    }

    // Handles show pending link button clicks
    protected void showPendingLinkButton_Click(object sender, EventArgs e)
    {
        sectionTitleLabel.Text = "Users (Pending Approval)";
        usersMultiView.SetActiveView(View_List);
        usersRadGrid.MasterTableView.NoMasterRecordsText = "There are pending users at this time.";
        string[] roleNames = new string[2] { "PurchaseUser", "PurchaseApprover" };
        usersRadGrid.DataSource = Users.GetPendingUsersInRoles(roleNames);
        usersRadGrid.DataBind();
    }

    // Handles add user link button clicks
    protected void addUserLinkButton_Click(object sender, EventArgs e)
    {
        AlterUserView("add", "null");
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
            PurchasingTableAdapters.user_profilesTableAdapter profilesAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();

            // Get current user's id
            string username = useridTextBox.Text.Trim();
            int profileID = Convert.ToInt32(profilesAdapter.GetIdByUsername(username));

            // Get backup user's id
            string backupUsername = backupRadComboBox.SelectedValue;
            int backupProfileID = Convert.ToInt32(profilesAdapter.GetIdByUsername(backupUsername));

            // If they aren't already in the db, add the relationship
            PurchasingTableAdapters.user_profile_relTableAdapter profileRelAdapter = new PurchasingTableAdapters.user_profile_relTableAdapter();
            Purchasing.user_profile_relDataTable dt = profileRelAdapter.GetByProfileIdAndBackupProfileId(profileID, backupProfileID);
            if (dt.Rows.Count == 0 && profileID != backupProfileID)
                profileRelAdapter.Insert(profileID, backupProfileID);

            BindBackupUsers();
        }
    }

}
