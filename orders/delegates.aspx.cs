using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Data;

public partial class business_purchasing_orders_delegates : System.Web.UI.Page
{
    // Page event, raised on page load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            UCDMenu.BuildBar_3(Menu3); // Build "In this section" Menu
            BindPIs();

            if (Request.QueryString["act"] != null)
            {
                // Switch to add view
                string action = Request.QueryString["act"].ToString();
                delegatesMultiView.SetActiveView(View_AddEdit);
                editUpdateButton.CommandName = "Add";
                editUpdateButton.Text = " Add ";
                sectionTitleLabel.Text = "Add Delegate";

                HandleAdmin();
            }
            else if (Request.QueryString["edit"] != null)
            {
                // Switch to edit view
                delegatesMultiView.SetActiveView(View_AddEdit);

                // Bind delegation
                int delegateID = Convert.ToInt32(Request.QueryString["edit"]);
                BindDelegate(delegateID);
                LoadDelegateText();
            }
            else
            {
                // No queries were present, bind the full list of delegates
                delegatesMultiView.SetActiveView(View_List);
                sectionTitleLabel.Text = "Delegates";
                BindDelegates();
            }
        }
    }

    /// <summary>
    /// Hide/show admin panel
    /// </summary>
    protected void HandleAdmin()
    {
        adminPanel.Visible = IsUserAdmin() ? true : false;
    }

    /// <summary>
    /// Subroutine, checks if user is in admin role
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    protected bool IsUserAdmin()
    {
        string[] roles = new string[] { "MasterAdmin", "MajorAdmin", "AssistantAdmin", "PurchaseAdmin", "PurchaseManager" };
        return Users.UserIsInRoles(User.Identity.Name, roles);
    }

    /// <summary>
    /// Subroutine, binds list of delegates
    /// </summary>
    protected void BindDelegates()
    {
        PurchasingTableAdapters.delegatesTableAdapter delegateAdapter = new PurchasingTableAdapters.delegatesTableAdapter();

        if (IsUserAdmin())
        {
            delegatesRadGrid.DataSource = delegateAdapter.Get();
            delegatesRadGrid.DataBind();
            delegatesRadGrid.Columns[1].Visible = true;
        }
        else
        {
            PurchasingTableAdapters.user_profilesTableAdapter profileAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
            delegatesRadGrid.DataSource = delegateAdapter.GetByUser(Convert.ToInt32(profileAdapter.GetIdByUsername(User.Identity.Name)));
            delegatesRadGrid.DataBind();
            delegatesRadGrid.Columns[1].Visible = false;
        }
    }

    /// <summary>
    /// Binds delegate to form controls
    /// </summary>
    /// <param name="delegateID"></param>
    protected void BindDelegate(int delegateID)
    {
        PurchasingTableAdapters.delegatesTableAdapter delegateAdapter = new PurchasingTableAdapters.delegatesTableAdapter();
        Purchasing.delegatesDataTable dt = delegateAdapter.GetById(delegateID);
        foreach (Purchasing.delegatesRow dr in dt.Rows)
        {
            PurchasingTableAdapters.user_profilesTableAdapter profileAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();

            RadComboBoxItem pi = piRadComboBox.FindItemByValue(profileAdapter.GetUsernameById(dr.pi_profile_id).ToString());
            if (pi != null)
                piRadComboBox.SelectedIndex = pi.Index;

            RadComboBoxItem targetUser = targetUserRadComboBox.FindItemByValue(profileAdapter.GetUsernameById(dr.target_profile_id).ToString());
            if (targetUser != null)
                targetUserRadComboBox.SelectedIndex = targetUser.Index;

            if (!dr.IsunitNull())
            {
                RadComboBoxItem unit = unitRadComboBox.FindItemByValue(dr.unit);
                if (unit != null)
                    unitRadComboBox.SelectedIndex = unit.Index;
            }

            if (dr.andor_1 == "AND")
            {
                andor1AndRadioButton.Checked = true;
                andor1OrRadioButton.Checked = false;
            }
            else
            {
                andor1AndRadioButton.Checked = false;
                andor1OrRadioButton.Checked = true;
            }

            if (dr.andor_2 == "AND")
            {
                andor2AndRadioButton.Checked = true;
                andor2OrRadioButton.Checked = false;
            }
            else
            {
                andor2AndRadioButton.Checked = false;
                andor2OrRadioButton.Checked = true;
            }

            if (!dr.IsamountNull())
                amountRadNumericTextBox.Text = dr.amount.ToString();
            if (!dr.Isaccount_nameNull())
                accountTextBox.Text = dr.account_name;
            if (!dr.Iscontains_nameNull())
                containRadComboBox.SelectedIndex = containRadComboBox.FindItemByValue(dr.contains_name.ToString()).Index;

            if (!dr.Isstart_dtNull())
            {
                if (!DateTime.Equals(dr.start_dt, new DateTime(1753, 1, 1)))
                    fromTextBox.Text = dr.start_dt.ToString("MM/dd/yyyy");
            }
            if (!dr.Isend_dtNull())
            {
                if (!DateTime.Equals(dr.end_dt, new DateTime(1753, 1, 1)))
                    toTextBox.Text = dr.end_dt.ToString("MM/dd/yyyy");
            }

            editUpdateButton.CommandArgument = dr.id.ToString();
            editUpdateButton.CommandName = "Update";
            editUpdateButton.Text = " Update ";
            sectionTitleLabel.Text = "Edit Delegate";

            HandleAdmin();
        }
    }

    /// <summary>
    /// Subroutine, binds list of purchase approvers (P.I.s)
    /// </summary>
    protected void BindPIs()
    {
        string[] roles = new string[3] { "PurchaseApprover", "PurchaseUser", "PurchaseAdmin" };
        DataView dv = Users.GetUsersByRoleNames(roles, false).Tables[0].DefaultView;
        dv.Sort = "LastName";
        targetUserRadComboBox.DataSource = dv;
        targetUserRadComboBox.DataBind();

        piRadComboBox.DataSource = Users.GetUsersByRoleName("PurchaseApprover", false);
        piRadComboBox.DataBind();
    }

    /// <summary>
    /// Returns user's name
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected string ParsePI(object sender)
    {
        PurchasingTableAdapters.user_profilesTableAdapter profileAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
        object username = profileAdapter.GetUsernameById(Convert.ToInt32(sender));
        return username != null ? Users.GetFullNameByUserName(username.ToString(), true, false) : String.Empty;
    }

    /// <summary>
    /// Retunrs friendly account name
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected string ParseAccounts(object sender)
    {
        return String.IsNullOrEmpty(sender.ToString()) ? "All" : sender.ToString();
    }

    /// <summary>
    /// Returns friendly unit
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected string ParseUnit(object sender)
    {
        return sender.ToString() == "N/A" ? "All" : sender.ToString();
    }

    /// <summary>
    /// Returns friendly amount
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected string ParseAmount(object sender)
    {
        return Convert.ToDecimal(sender) > 0 ? Convert.ToDecimal(sender).ToString("c") : "All";
    }

    /// <summary>
    /// Subroutine, returns friendly effective date
    /// </summary>
    /// <param name="start_dt"></param>
    /// <param name="end_dt"></param>
    /// <returns></returns>
    protected string ParseEffectiveDate(object start_dt, object end_dt)
    {
        if (start_dt != DBNull.Value && end_dt != DBNull.Value)
        {
            DateTime start = Convert.ToDateTime(start_dt);
            DateTime end = Convert.ToDateTime(end_dt);
            if (!DateTime.Equals(start, new DateTime(1753, 1, 1)) || !DateTime.Equals(end, new DateTime(1753, 1, 1)))
                return "From: " + start.ToString("MM/dd/yy") + " To: " + end.ToString("MM/dd/yy");
            else
                return "All";
        }
        else
            return "All";
    }

    /// <summary>
    /// 
    /// </summary>
    protected void LoadDelegateText()
    {
        if (targetUserRadComboBox.SelectedIndex == 0)
        {
            samplePanel.Visible = false;
            return;
        }
        else
            samplePanel.Visible = true;

        string txt = String.Empty;
        // userid's request was automatically approved by pi because it met the following critera: 
        txt += "<span style='color:#Green'>";
        txt += Users.GetFullNameByUserName(targetUserRadComboBox.SelectedValue, false, false);
        txt += "'s request was automatically approved by ";
        if (IsUserAdmin())
            txt += Users.GetFullNameByUserName(piRadComboBox.SelectedValue, false, false);
        else
            txt += Users.GetFullNameByUserName(User.Identity.Name, false, false);
        txt += " because it met the following criteria: ";
        txt += "Approved ";

        // Unit (gt/lt/eq to amount)
        try
        {
            double amount = Convert.ToDouble(amountRadNumericTextBox.Text.Trim());
            if (amount > 0 && unitRadComboBox.SelectedValue != "N/A")
            {
                if (unitRadComboBox.SelectedValue == ">")
                    txt += "over ";
                else if (unitRadComboBox.SelectedValue == "<")
                    txt += "up to ";
                else if (unitRadComboBox.SelectedValue == "=")
                    txt += "when amount is ";

                txt += string.Format("{0:C}", amount) + " ";
            }
            else
                txt += "for all amounts ";
        }
        catch (Exception)
        {
            txt += "for all amounts ";
        }

        // Account (contains/does not contain)
        if (!String.IsNullOrEmpty(accountTextBox.Text.Trim()))
        {
            if (Convert.ToBoolean(containRadComboBox.SelectedValue))
                txt += "on " + accountTextBox.Text.Trim() + " ";
            else
                txt += "on all accounts except " + accountTextBox.Text.Trim() + " ";
        }
        else
            txt += "on all accounts ";

        // Date
        try
        {
            if (fromTextBox.Text.Trim().Length > 0 && toTextBox.Text.Trim().Length > 0)
            {
                txt += "from " + fromTextBox.Text.Trim() + " to " + toTextBox.Text.Trim() + ".";
            }
            else
                txt += "from " + DateTime.Now.ToString("MM/dd/yy") + " on.";
        }
        catch (Exception)
        {
            txt += "from " + DateTime.Now.ToString("MM/dd/yy") + " on.";
        }

        txt += "</span>";
        sampleLabel.Text = txt;
    }

    // Event, raised on grid item command
    protected void delegatesRadGrid_ItemCommand(object source, GridCommandEventArgs e)
    {
        if (e.CommandName == "DeleteDelegate")
        {
            PurchasingTableAdapters.delegatesTableAdapter delegateAdapter = new PurchasingTableAdapters.delegatesTableAdapter();
            delegateAdapter.Delete(Convert.ToInt32(e.CommandArgument));
            BindDelegates();
        }
        else if (e.CommandName == "EditDelegate")
        {
            Response.Redirect("delegates.aspx?edit=" + e.CommandArgument.ToString());
        }
    }

    // Event, raised on unit ddl change
    protected void unitRadComboBox_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        LoadDelegateText();
    }

    // Event, raised on target user ddl change
    protected void targetUserRadComboBox_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        LoadDelegateText();
    }

    // Event, raised on contains name ddl change
    protected void containRadComboBox_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        LoadDelegateText();
    }

    // Event, raised on ammount change
    protected void amountRadNumericTextBox_TextChanged(object sender, EventArgs e)
    {
        LoadDelegateText();
    }

    // Event, raised on account change
    protected void accountTextBox_TextChanged(object sender, EventArgs e)
    {
        LoadDelegateText();
    }

    // Event, raised on from date change
    protected void fromTextBox_TextChanged(object sender, EventArgs e)
    {
        LoadDelegateText();
    }

    // Event, raised on to date change
    protected void toTextBox_TextChanged(object sender, EventArgs e)
    {
        LoadDelegateText();
    }

    // Event, raised on andor1 change
    protected void andor1OrRadioButton_CheckedChanged(object sender, EventArgs e)
    {
        LoadDelegateText();
    }

    // Event, raised on andor2 change
    protected void andor2OrRadioButton_CheckedChanged(object sender, EventArgs e)
    {
        LoadDelegateText();
    }

    // Handles edit/update button clicks
    protected void editUpdateButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Button btn = sender as Button;
            if (btn.CommandName == "Update")
            {
                try
                {
                    // Update delegate
                    PurchasingTableAdapters.delegatesTableAdapter delegateAdapter = new PurchasingTableAdapters.delegatesTableAdapter();
                    PurchasingTableAdapters.user_profilesTableAdapter profileAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
                    delegateAdapter.Update(
                        IsUserAdmin() ? Convert.ToInt32(profileAdapter.GetIdByUsername(piRadComboBox.SelectedValue)) : Convert.ToInt32(profileAdapter.GetIdByUsername(User.Identity.Name)),
                        Convert.ToInt32(profileAdapter.GetIdByUsername(targetUserRadComboBox.SelectedValue)),
                        amountRadNumericTextBox.Text.Trim().Length > 0 ? Convert.ToDecimal(amountRadNumericTextBox.Text) : 0,
                        unitRadComboBox.SelectedValue,
                        andor1AndRadioButton.Checked ? "AND" : "OR",
                        accountTextBox.Text.Trim(),
                        Convert.ToBoolean(containRadComboBox.SelectedValue),
                        andor2AndRadioButton.Checked ? "AND" : "OR",
                        fromTextBox.Text.Trim().Length > 0 ? Convert.ToDateTime(fromTextBox.Text.Trim()) : new DateTime(1753, 1, 1),
                        toTextBox.Text.Trim().Length > 0 ? Convert.ToDateTime(toTextBox.Text.Trim()) : new DateTime(1753, 1, 1),
                        DateTime.Now,
                        Convert.ToInt32(btn.CommandArgument)
                        );

                    // Confirm
                    delegatesMultiView.SetActiveView(View_Confirm);
                    confirmLabel.Text = "Delegate successfully updated.";
                    Response.AppendHeader("refresh", "2;url=delegates.aspx");
                }
                catch (Exception ex)
                {
                    delegatesMultiView.SetActiveView(View_Confirm);
                    confirmLabel.ForeColor = Color.Red;
                    confirmLabel.Text = ex.Message;
                    Response.AppendHeader("refresh", "2;url=delegates.aspx");
                }
            }
            else if (btn.CommandName == "Add")
            {
                try
                {
                    // Insert delegate
                    PurchasingTableAdapters.delegatesTableAdapter delegateAdapter = new PurchasingTableAdapters.delegatesTableAdapter();
                    PurchasingTableAdapters.user_profilesTableAdapter profileAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
                    delegateAdapter.Insert(
                        IsUserAdmin() ? Convert.ToInt32(profileAdapter.GetIdByUsername(piRadComboBox.SelectedValue)) : Convert.ToInt32(profileAdapter.GetIdByUsername(User.Identity.Name)),
                        Convert.ToInt32(profileAdapter.GetIdByUsername(targetUserRadComboBox.SelectedValue)),
                        amountRadNumericTextBox.Text.Trim().Length > 0 ? Convert.ToDecimal(amountRadNumericTextBox.Text) : 0,
                        unitRadComboBox.SelectedValue,
                        andor1AndRadioButton.Checked ? "AND" : "OR",
                        accountTextBox.Text.Trim(),
                        Convert.ToBoolean(containRadComboBox.SelectedValue),
                        andor2AndRadioButton.Checked ? "AND" : "OR",
                        fromTextBox.Text.Trim().Length > 0 ? Convert.ToDateTime(fromTextBox.Text.Trim()) : new DateTime(1753, 1, 1),
                        toTextBox.Text.Trim().Length > 0 ? Convert.ToDateTime(toTextBox.Text.Trim()) : new DateTime(1753, 1, 1),
                        DateTime.Now
                        );

                    // Confirm
                    delegatesMultiView.SetActiveView(View_Confirm);
                    confirmLabel.Text = "Delegate successfully added.";
                    Response.AppendHeader("refresh", "2;url=delegates.aspx");
                }
                catch (Exception ex)
                {
                    delegatesMultiView.SetActiveView(View_Confirm);
                    confirmLabel.ForeColor = Color.Red;
                    confirmLabel.Text = ex.Message;
                    Response.AppendHeader("refresh", "2;url=delegates.aspx");
                }
            }
        }
    }

    // Handles edit cancel button clicks
    protected void editCancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("delegates.aspx");
    }

    // Handles switching to add view
    protected void addLinkButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("delegates.aspx?act=add");
    }
}
