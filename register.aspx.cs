using System;
using System.Configuration;
using System.Web.Security;
using System.Web.UI;
using Telerik.Web.UI;

public partial class business_purchasing_register : System.Web.UI.Page
{
    // Page event, raised on page load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Users.UserHasRole(User.Identity.Name) && !Users.UserIsApproved(User.Identity.Name) && PurchaseHelper.UserProfileExists(User.Identity.Name))
            {
                awaitingApprovalLabel.Text = "<h2>Welcome back " + Users.GetFirstNameByUserName(User.Identity.Name) + "!</h2><br/ >";
                awaitingApprovalLabel.Text += "Your account is being reviewed and is awaiting activation.<br /><br />You will receive notification shortly.";
                requestMultiView.SetActiveView(View_WaitingOnApproval);
            }
            else
            {
                HandleExistingUsers();
                BindUnits();
                BindShipToAddresses();
                requestMultiView.SetActiveView(View_Request);
            }
        }
    }

    /// <summary>
    /// Subroutine, binds list of units to ddl
    /// </summary>
    protected void BindUnits()
    {
        // Set view
        unitsMultiView.SetActiveView(View_Unit_List);

        // Bind units
        PurchasingTableAdapters.unitsTableAdapter unitAdapter = new PurchasingTableAdapters.unitsTableAdapter();
        unitsRadComboBox.DataSource = unitAdapter.Get();
        unitsRadComboBox.DataBind();

        // Add "other" list item
        RadComboBoxItem li = new RadComboBoxItem("Other...", "other");
        unitsRadComboBox.Items.Add(li);
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

    /// <summary>
    /// Subroutine, returns id of unit (inserting unit if nessisarry)
    /// </summary>
    /// <returns></returns>
    protected int InsertUnit()
    {
        int unitID = 0;

        // If user choose to enter a new unit
        if (unitsMultiView.ActiveViewIndex == 1)
        {
            PurchasingTableAdapters.unitsTableAdapter unitAdapter = new PurchasingTableAdapters.unitsTableAdapter();
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
    /// Subroutine, populates controls with existing user info
    /// </summary>
    protected void HandleExistingUsers()
    {
        if (Users.UserExists(User.Identity.Name))
        {
            fnameTextBox.Text = Users.GetFirstNameByUserName(User.Identity.Name);
            lnameTextBox.Text = Users.GetLastNameByUserName(User.Identity.Name);
            emailTextBox.Text = Users.GetEmailByUserName(User.Identity.Name);
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
            PurchasingTableAdapters.user_profilesTableAdapter profilesAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
            profilesAdapter.Insert(
                false,
                User.Identity.Name,
                InsertUnit(),
                InsertAddress(),
                StringHelper.ZeroPad(phoneRadMaskedTextBox.Text.Trim(), 10),
                DateTime.Now
                );

            check = true;
        }

        return check;
    }

    // Event, raised on unitsRadComboBox selected index changed
    protected void unitsRadComboBox_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (e.Value == "other")
            unitsMultiView.SetActiveView(View_Unit_New);
    }

    // Event, raised on shiptoRadComboBox selected index changed
    protected void shiptoRadComboBox_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (e.Value == "other")
        {
            shiptoCampusTextBox.Text = "University of California, Davis";
            shiptoCityTextBox.Text = "Davis";
            shiptoStateRadComboBox.SelectedIndex = 4;
            shiptoMultiView.SetActiveView(View_ShipTo_New);
        }
    }

    // Handles clear button clicks
    protected void clearButton_Click(object sender, EventArgs e)
    {
        fnameTextBox.Text = String.Empty;
        lnameTextBox.Text = String.Empty;
        emailTextBox.Text = String.Empty;
        unitsRadComboBox.SelectedIndex = 0;
        unitsMultiView.SetActiveView(View_Unit_List);
        shiptoRadComboBox.SelectedIndex = 0;
        shiptoMultiView.SetActiveView(View_ShipTo_List);
    }

    // Handles request button clicks
    protected void requestButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Set a success trigger
                bool success = false;
                string err = String.Empty;

                // Create new user
                MembershipCreateStatus status = new MembershipCreateStatus();
                Membership.CreateUser(
                    User.Identity.Name,
                    "$ASHE@3476KJH_@5",
                    emailTextBox.Text.Trim(),
                    "question",
                    "answer",
                    false, // Set their account to "Un-Approved"
                    out status
                );

                // Confirmation
                requestMultiView.SetActiveView(View_Confirm);

                if (status.ToString() == "Success")
                {
                    // User successfully created, update name and insert profile
                    Users.UpdateNameByUserName(User.Identity.Name, fnameTextBox.Text.Trim(), lnameTextBox.Text.Trim());
                    success = InsertProfile();
                    if (!success)
                        err = "Unable to insert profile.";
                }
                else if (status.ToString() == "DuplicateUserName")
                {
                    // User already exists, check if they have a purchasing profle
                    if (PurchaseHelper.UserProfileExists(User.Identity.Name))
                    {
                        success = false;
                        err = "User & profile already exist.";
                    }
                    else
                    {
                        success = InsertProfile();
                        if (!success)
                            err = "User exists, but unable to insert profile.";
                    }
                }
                else
                {
                    // Error
                    success = false;
                    err = status.ToString();
                }

                // Determine role
                string choosenRole = String.Empty;
                if (purchasingRadioButton.Checked)
                    choosenRole = "PurchaseUser";
                else if (approvingRadioButton.Checked)
                    choosenRole = "PurchaseApprover";
                else if (adminsterRadioButton.Checked)
                    choosenRole = "PurchaseAdmin";
                else if (managerRadioButton.Checked)
                    choosenRole = "PurchaseManager";

                if (success)
                {
                    // Add user to role
                    string[] roles = new string[] { "PurchaseUser", "PurchaseApprover", "PurchaseAdmin", "PurchaseManager" };
                    if (!Users.UserIsInRoles(User.Identity.Name, roles))
                        Roles.AddUserToRole(User.Identity.Name, choosenRole);

                    // User was successfully created.
                    confrimLabel.Text = "<h2>Success!</h2><br />You will receive an e-mail notification when your account has been activated. You may now close your browser window or tab.<br /><br />Thank you.";

                    string body = "<h2>New JMIE " + choosenRole + "</h2>User: " + User.Identity.Name + " created a purchasing account. Please <a href='" + ConfigurationManager.AppSettings["purchasingRoot"].ToString() + "admin/users.aspx'>login here</a> and review this account.";
                    body += "<ul>";
                    body += "<li>Name: " + fnameTextBox.Text + " " + lnameTextBox.Text;
                    body += "</li><li>E-mail: " + Server.HtmlEncode(emailTextBox.Text);
                    body += "</li><li>Location: " + (unitsRadComboBox.SelectedValue != "other" ? unitsRadComboBox.SelectedItem.Text : unitNameTextBox.Text);
                    body += "</li><li>Role: " + choosenRole;
                    body += "</li><li>Date/Time: " + DateTime.Now.ToString();
                    body += "</li></ul>";

                    MailHelper.SendMailMessage(
                        emailTextBox.Text,
                        ConfigurationManager.AppSettings["purchaseAdminEmail"].ToString(),
                        ConfigurationManager.AppSettings["purchaseAdminBCCEmail"].ToString(),
                        ConfigurationManager.AppSettings["purchaseAdminCCEmail"].ToString() + "; " + ConfigurationManager.AppSettings["dept"].ToString(),
                        "New Purchasing User",
                        body
                        );
                }
                else
                {
                    // An error occured
                    confrimLabel.Text = "Oops! There was a problem creating your account. Our webmaster was notified and you will receive an explanation shortly.<br /><br />Thank you.";
                    string body = "Hello,<br /><br />User: " + User.Identity.Name + " attempted to create a purchasing account. Their request failed with the following error:<br /><br />" + err;

                    body += "<ul>";
                    body += "<li>Name: " + fnameTextBox.Text + " " + lnameTextBox.Text;
                    body += "</li><li>E-mail: " + emailTextBox.Text.Trim();
                    body += "</li><li>Location: " + (unitsRadComboBox.SelectedValue != "other" ? unitsRadComboBox.SelectedItem.Text : unitNameTextBox.Text);
                    body += "</li><li>Role: " + choosenRole;
                    body += "</li><li>Date/Time: " + DateTime.Now.ToString();
                    body += "</li></ul>";

                    MailHelper.SendMailMessage(
                        emailTextBox.Text,
                        ConfigurationManager.AppSettings["purchaseAdminEmail"].ToString(),
                        ConfigurationManager.AppSettings["purchaseAdminBCCEmail"].ToString(),
                        ConfigurationManager.AppSettings["purchaseAdminCCEmail"].ToString() + "; " + ConfigurationManager.AppSettings["dept"].ToString(),
                        "Purchasing Access Error",
                        body
                    );
                    backButton.Visible = true;
                }
            }
        }
    }

    // Handles back button clicks
    protected void backButton_Click(object sender, EventArgs e)
    {
        requestMultiView.SetActiveView(View_Request);
    }
}
