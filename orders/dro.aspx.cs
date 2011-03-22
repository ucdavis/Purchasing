using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using aspNetEmail;
using Telerik.Web.UI;

public partial class business_purchasing_orders_dro : System.Web.UI.Page
{
    // Paths
    protected string savePath = ConfigurationManager.AppSettings["purchasingSavePath"].ToString();
    protected string filePath = ConfigurationManager.AppSettings["purchasingRoot"].ToString() + "admin/uploads/";

    // Event, raised on page load
    protected void Page_Load(object sender, EventArgs e)
    {
        HandleIE();

        if (!PurchaseHelper.UserIsValid(User.Identity.Name))
            Response.Redirect(ConfigurationManager.AppSettings["purchasingRoot"].ToString());

        if (!IsPostBack)
        {
            BindPIs();
            BindPurchasers();
            BindSiteAddresses();
            addedit_vendor1.BindVendorsList();
            SetDefaultShipTo();
            UCDMenu.BuildBar_3(Menu3); // Build "In this section" Menu
            Menu3.Items[0].Selected = true;

            SetDefaultSite();
            siteMultiView.SetActiveView(View_ChooseSite);
            droMultiView.SetActiveView(View_Order);
            submitMultiView.SetActiveView(View_Submit);

            if (Request.QueryString["order"] != null)
            {
                int orderID = Convert.ToInt32(Request.QueryString["order"]);

                if (PurchaseHelper.OrderExists(orderID))
                {
                    // An order was specified, populate form using order id
                    PopulateOrder(orderID);
                    dro_items1.PopulateItems(orderID);

                    // Handle form modes
                    if (Request.QueryString["mode"] != null)
                    {
                        PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
                        if (Convert.ToBoolean(ordersAdapter.IsLocked(orderID)))
                        {
                            // Order is locked, prompt user
                            Msg(
                                "<h1>Order locked</h1>This order is being processed and has been locked by the purchaser. Please contact " + PurchaseHelper.ProfileInfo(ordersAdapter.GetPurchaser(orderID).ToString(), false) + " if you would like to make changes to this order.",
                                "Edit or Approval of locked order by " + PurchaseHelper.ProfileInfo(User.Identity.Name, false),
                                true,
                                "myorders.aspx?order=" + orderID.ToString()
                                );
                        }
                        else
                            HandleMode(Request.QueryString["mode"].ToString());
                    }
                }
                else
                {
                    Msg(
                        "This order no longer exists.",
                        "User tried to access invalid url: " + Request.Url.ToString(),
                        true,
                        "type.aspx"
                        );
                }
            }
            else
            {
                // Pre populate shipto name, NOTE: this may be overwritten below
                siteNameTextBox.Text = Users.GetFullNameByUserName(User.Identity.Name, false, false);

                // Get last order
                PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
                Purchasing.ordersDataTable dt = ordersAdapter.GetTop(User.Identity.Name, Convert.ToInt32(PurchaseHelper.OrderType.DRO));
                foreach (Purchasing.ordersRow dr in dt.Rows)
                {
                    // A previous order exists, show/wire up "populate" button
                    subTitleMultiView.SetActiveView(View_LastOrder);
                    lastOrderImageButton.CommandArgument = dr.id.ToString();
                    lastOrderLinkButton.CommandArgument = dr.id.ToString();

                    // Pre-populate certain fields
                    siteNameTextBox.Text = dr.shipto_name;

                    // Select top 1 pi/approver username from last order
                    PurchasingTableAdapters.order_approvalsTableAdapter approvalAdapter = new PurchasingTableAdapters.order_approvalsTableAdapter();
                    object piObj = approvalAdapter.GetTop(dr.id);
                    if (piObj != null)
                    {
                        RadComboBoxItem piLi = piRadComboBox1.Items.FindItemByValue(piObj.ToString());
                        if (piLi != null)
                            piRadComboBox1.SelectedIndex = piLi.Index;
                    }

                    RadComboBoxItem adminLi = purchaserRadComboBox.Items.FindItemByValue(dr.admin_userid);
                    if (adminLi != null)
                        purchaserRadComboBox.SelectedIndex = adminLi.Index;

                    RadComboBoxItem shipTo = siteRadComboBox.Items.FindItemByValue(dr.shipto_id.ToString());
                    if (shipTo != null)
                        siteRadComboBox.SelectedIndex = shipTo.Index;
                }
            }
        }
    }

    /// <summary>
    /// Subroutine, handles IE browser
    /// </summary>
    protected void HandleIE()
    {
        System.Web.HttpBrowserCapabilities browser = Request.Browser;
        if (browser.Browser == "IE")
            RadFormDecorator1.DecoratedControls = FormDecoratorDecoratedControls.Buttons;
    }

    /// <summary>
    /// Subroutine, handles form modes
    /// </summary>
    /// <param name="mode"></param>
    protected void HandleMode(string mode)
    {
        PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
        int orderID = Convert.ToInt32(Request.QueryString["order"]);
        string orderAuthor = ordersAdapter.GetAuthor(orderID).ToString();

        if (mode == "edit")
        {
            // Validate purchaser
            string requester = User.Identity.Name;
            if (requester.Equals(orderAuthor))
            {
                // The PI is valid, show PI approve/deny view
                sectionTitleLabel.Text = "Now Editing: " + sectionTitleLabel.Text;
                subTitleMultiView.SetActiveView(View_Prompt);
                PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
                promptLabel.Text = "Submitted on " + Convert.ToDateTime(statusAdapter.GetDateByOrderStatus(orderID, Convert.ToInt32(PurchaseHelper.OrderStatus.New))).ToString("M/d/yy h:mm tt");
                subTitleMultiView.SetActiveView(View_Prompt);
                submitMultiView.SetActiveView(View_Update);
            }
        }
        else if (mode == "pi")
        {
            // Validate pi
            string pi = User.Identity.Name;
            PurchasingTableAdapters.order_approvalsTableAdapter approvalAdapter = new PurchasingTableAdapters.order_approvalsTableAdapter();
            if (Convert.ToInt32(approvalAdapter.IsUserApprover(orderID, pi)) > 0 && User.IsInRole("PurchaseApprover"))
            {
                // The pi is valid, show the approve/deny view
                piRadComboBox1.Enabled = false;
                piRadComboBox2.Enabled = false;
                piRadComboBox3.Enabled = false;
                piRadComboBox4.Enabled = false;
                piRadComboBox5.Enabled = false;
                purchaserRadComboBox.Enabled = false;

                string authorFullName = Users.GetFullNameByUserName(orderAuthor, false, false);
                string piFullName = Users.GetFullNameByUserName(User.Identity.Name, false, false);

                subTitleMultiView.SetActiveView(View_Prompt);
                PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
                promptLabel.Text = "Submitted by " + authorFullName + " on " + Convert.ToDateTime(statusAdapter.GetDateByOrderStatus(orderID, Convert.ToInt32(PurchaseHelper.OrderStatus.New))).ToString("M/d/yy h:mm tt") + ". To be reviewed by " + piFullName;
                subTitleMultiView.SetActiveView(View_Prompt);
                submitMultiView.SetActiveView(View_Submit_Blank);
                approvalMultiView.SetActiveView(View_Approve);
                accountRequiredFieldValidator.Visible = false;
                accountRequiredFieldValidator.Enabled = false;

                // Put red border around account cells
                HtmlTableCell td1 = View_Order.FindControl("accountCell1") as HtmlTableCell;
                td1.Style.Add("border", "solid 1px #D78E6E");
                td1.Style.Add("background-color", "#FBBC9A");
                td1.Style.Add("border-right", "0px none");
                HtmlTableCell td2 = View_Order.FindControl("accountCell2") as HtmlTableCell;
                td2.Style.Add("border", "solid 1px #D78E6E");
                td2.Style.Add("background-color", "#FBBC9A");
                td2.Style.Add("border-left", "0px none");
                attnLabel.Text = "ATTN: " + piFullName;
            }
            else
            {
                Msg("You are no longer listed as a PI/Approver for this order.",
                    "User: " + User.Identity.Name + " tried to approve at the following url: " + Request.Url.ToString(),
                    true,
                    "myorders.aspx?order=" + orderID
                    );
            }
        }
    }

    /// <summary>
    /// Subroutine, populates form with last order
    /// </summary>
    /// <param name="orderID"></param>
    protected void PopulateOrder(int orderID)
    {
        // Bind order
        PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
        Purchasing.ordersDataTable dt = ordersAdapter.GetById(orderID);
        foreach (Purchasing.ordersRow dr in dt.Rows)
        {
            //////////////////////////// Order preferences ///////////////////////////////
            sectionTitleLabel.Text = "Departmental Repair Order #" + dr.unique_id;

            // Populate ddls with past approvers
            PurchasingTableAdapters.order_approvalsTableAdapter approvalAdapter = new PurchasingTableAdapters.order_approvalsTableAdapter();
            Purchasing.order_approvalsDataTable approvalDt = approvalAdapter.GetByOrder(orderID);
            int i = 1;
            foreach (Purchasing.order_approvalsRow approvalDr in approvalDt.Rows)
            {
                // Loop through approvals, populating ddls as we go
                RadComboBox rcb = View_Order.FindControl("piRadComboBox" + i.ToString()) as RadComboBox;
                HtmlTableRow tr = View_Order.FindControl("Tr" + i.ToString()) as HtmlTableRow;

                // Populate combobox
                RadComboBoxItem pi = rcb.Items.FindItemByValue(approvalDr.pi_userid);
                if (pi != null)
                    rcb.SelectedIndex = pi.Index;

                // Show the table row (it has data)
                tr.Style["display"] = "table-row";
                i++;
            }

            // Hide remaining rows
            for (int h = i; h <= 5; h++)
            {
                if (h > 1)
                {
                    HtmlTableRow tr = View_Order.FindControl("Tr" + h.ToString()) as HtmlTableRow;
                    tr.Style["display"] = "none";
                }
            }

            RadComboBoxItem purchaser = purchaserRadComboBox.Items.FindItemByValue(dr.admin_userid);
            if (purchaser != null)
                purchaserRadComboBox.SelectedIndex = purchaser.Index;

            siteMultiView.SetActiveView(View_Site_List);
            RadComboBoxItem ship = shippingRadComboBox.Items.FindItemByText(dr.shipping);
            if (ship != null)
                shippingRadComboBox.SelectedIndex = ship.Index;

            neededTextBox.Text = dr.date_needed;

            if (!dr.IsaccountNull())
            {
                acctNameLabel.Text = dr.account;
                accountTextBox.Text = dr.account;
            }

            if (dr.backorder_ok)
            {
                backorderNoRadioButton.Checked = false;
                backorderOKRadioButton.Checked = true;
            }
            else
            {
                backorderNoRadioButton.Checked = true;
                backorderOKRadioButton.Checked = false;
            }

            // Show vendor details
            addedit_vendor1.PopulateVendorForm(dr.vendor_id);

            //////////////////////////// Ship To ///////////////////////////////
            if (dr.shipto_onsite)
            {
                onSiteRadioButton.Checked = true;
                offSiteRadioButton.Checked = false;
                siteMultiView.SetActiveView(View_Site_List);
            }
            else
            {
                offSiteRadioButton.Checked = true;
                onSiteRadioButton.Checked = false;
                siteMultiView.SetActiveView(View_ChooseSite);
            }

            RadComboBoxItem shipToLi = siteRadComboBox.Items.FindItemByValue(dr.shipto_id.ToString());
            if (shipToLi != null)
                siteRadComboBox.SelectedIndex = shipToLi.Index;
            siteNameTextBox.Text = dr.shipto_name;
            siteNameTextBox2.Text = dr.shipto_name;

            //////////////////////////// Notes ///////////////////////////////
            PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
            notesTextBox.Text = statusAdapter.GetSubmitNotesByOrder(orderID).ToString();
        }
    }

    /// <summary>
    /// Subroutine, sets default shipto address
    /// </summary>
    protected void SetDefaultShipTo()
    {
        // Bind default shipping addresses
        PurchasingTableAdapters.shipto_addressesTableAdapter shipToAdapter = new PurchasingTableAdapters.shipto_addressesTableAdapter();
        Purchasing.shipto_addressesDataTable dt = shipToAdapter.GetByUser(User.Identity.Name);
        foreach (Purchasing.shipto_addressesRow dr in dt.Rows)
        {
            RadComboBoxItem li = siteRadComboBox.Items.FindItemByValue(dr.id.ToString());
            if (li != null)
                siteRadComboBox.SelectedIndex = li.Index;
        }
    }

    /// <summary>
    /// Subroutine, binds list of purchase approvers (P.I.s)
    /// </summary>
    protected void BindPIs()
    {
        piRadComboBox1.DataSource =
          piRadComboBox2.DataSource =
          piRadComboBox3.DataSource =
          piRadComboBox4.DataSource =
          piRadComboBox5.DataSource =
          Users.GetUsersByRoleName("PurchaseApprover", false);

        piRadComboBox1.DataBind();
        piRadComboBox2.DataBind();
        piRadComboBox3.DataBind();
        piRadComboBox4.DataBind();
        piRadComboBox5.DataBind();
    }

    /// <summary>
    /// Subroutine, binds list of purchasers
    /// </summary>
    protected void BindPurchasers()
    {
        purchaserRadComboBox.DataSource = Users.GetUsersByRoleName("PurchaseAdmin", false);
        purchaserRadComboBox.DataBind();
    }

    /// <summary>
    /// Subroutine, sets default site address
    /// </summary>
    protected void SetDefaultSite()
    {
        // Bind default shipping addresses
        PurchasingTableAdapters.shipto_addressesTableAdapter shipToAdapter = new PurchasingTableAdapters.shipto_addressesTableAdapter();
        Purchasing.shipto_addressesDataTable dt = shipToAdapter.GetByUser(User.Identity.Name);
        foreach (Purchasing.shipto_addressesRow dr in dt.Rows)
        {
            RadComboBoxItem li = siteRadComboBox.Items.FindItemByValue(dr.id.ToString());
            if (li != null)
                siteRadComboBox.SelectedIndex = li.Index;
        }
    }

    /// <summary>
    /// Subroutine, binds list of addresses
    /// </summary>
    protected void BindSiteAddresses()
    {
        // Set view
        siteMultiView.SetActiveView(View_Site_List);

        // Bind addresses
        PurchasingTableAdapters.shipto_addressesTableAdapter shipToAdapter = new PurchasingTableAdapters.shipto_addressesTableAdapter();
        siteRadComboBox.DataSource = shipToAdapter.Get();
        siteRadComboBox.DataBind();

        // Add "New Location..." list item
        RadComboBoxItem li = new RadComboBoxItem("New Location...", "other");
        li.CssClass = "grn";
        siteRadComboBox.Items.Add(li);
    }

    /// <summary>
    /// Subroutine, returns id of site address (inserting address if nessisarry)
    /// </summary>
    /// <returns></returns>
    protected int InsertAddress()
    {
        int shipToID = 0;

        // Check if we are adding a new address
        if (siteMultiView.ActiveViewIndex == 2)
        {
            PurchasingTableAdapters.shipto_addressesTableAdapter shipToAdapter = new PurchasingTableAdapters.shipto_addressesTableAdapter();
            shipToID = Convert.ToInt32(shipToAdapter.InsertSelect(
                siteAddressTextBox.Text.Trim(),
                siteCampusTextBox.Text.Trim(),
                siteStreetTextBox.Text.Trim(),
                siteCityTextBox.Text.Trim(),
                siteStateRadComboBox.SelectedValue,
                StringHelper.ZeroPad(siteZipRadNumericTextBox.Text.Trim(), 5) + StringHelper.ZeroPad(siteZipPlusRadNumericTextBox.Text.Trim(), 4),
                siteBuildingTextBox.Text.Trim(),
                siteRoomTextBox.Text.Trim(),
                StringHelper.ZeroPad(sitePhoneRadMaskedTextBox.Text.Trim(), 10)
                ));
        }
        else
            // They selected an address from the list, use that id
            shipToID = Convert.ToInt32(siteRadComboBox.SelectedValue);

        return shipToID;
    }

    /// <summary>
    /// Returns list of pis
    /// </summary>
    /// <returns></returns>
    protected List<string> GetPIs()
    {
        List<string> piList = new List<string>();
        for (int i = 1; i <= 5; i++)
        {
            // Loop through approvals
            RadComboBox rcb = View_Order.FindControl("piRadComboBox" + i.ToString()) as RadComboBox;
            HtmlTableRow tr = View_Order.FindControl("Tr" + i.ToString()) as HtmlTableRow;

            if (rcb.SelectedValue != "Select One...")
            {
                // Add to pi list but prevent duplicates
                if (!piList.Contains(rcb.SelectedValue))
                    piList.Add(rcb.SelectedValue);
            }
        }
        return piList;
    }

    /// <summary>
    /// Clear the new site form
    /// </summary>
    protected void ClearSiteForm()
    {
        siteAddressTextBox.Text = String.Empty;
        siteCampusTextBox.Text = "University of California, Davis";
        siteCityTextBox.Text = "Davis";
        siteStateRadComboBox.SelectedIndex = 4; // Default to CA
        siteStreetTextBox.Text = String.Empty;
        siteBuildingTextBox.Text = String.Empty;
        siteRoomTextBox.Text = String.Empty;
        siteZipRadNumericTextBox.Text = String.Empty;
        siteZipPlusRadNumericTextBox.Text = String.Empty;
        sitePhoneRadMaskedTextBox.Text = String.Empty;
    }

    /// <summary>
    /// Subroutine, inserts order. returns order id (order id of 0 = failed)
    /// </summary>
    /// <returns></returns>
    protected int InsertOrder()
    {
        int orderID = 0;
        double subtotal = dro_items1.GetOrderSubTotal();
        string account = accountTextBox.Text.Trim();

        int vendorID = addedit_vendor1.HandleVendor(User.Identity.Name); // Insert (or) update vendor and customer number
        if (vendorID == 0)
        {
            Msg("An error occured while attempting to insert a new vendor.<br />Our webmaster has been notified.", "Vendor error", true, "dpo.aspx");
            return 0;
        }

        // Determine site (tech) contact
        string siteNameContact = String.Empty;
        if (siteMultiView.ActiveViewIndex == 1)
            siteNameContact = siteNameTextBox.Text.Trim();
        else if (siteMultiView.ActiveViewIndex == 2)
            siteNameContact = siteNameTextBox2.Text.Trim();

        // Insert order
        try
        {
            PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
            orderID = Convert.ToInt32(ordersAdapter.InsertOrder(
                User.Identity.Name,
                DateTime.Now.ToString("MMddyy-"),
                Convert.ToInt32(PurchaseHelper.OrderType.DRO),
                purchaserRadComboBox.SelectedValue,
                vendorID,
                siteNameContact,
                InsertAddress(),
                onSiteRadioButton.Checked ? true : false,
                shippingRadComboBox.SelectedItem.Text,
                neededTextBox.Text.Trim(),
                backorderNoRadioButton.Checked ? false : true,
                User.Identity.Name,
                notesTextBox.Text.Trim().Length > 0 ? notesTextBox.Text.Trim() : "N/A",
                account,
                Convert.ToDecimal(subtotal),
                subtotal > 4999.99 ? true : false,
                false,
                false,
                null,
                null,
                null,
                null,
                null,
                null,
                null
                ));
        }
        catch (Exception ex)
        {
            string error = "Oops! There was a problem submitting your order<br /><br />";
            error += "Our webmaster was notified and you will receive an explanation shortly.<br /><br />";
            error += "We apologize for the inconvenience.";
            Msg(error, ex.Message, true, "dpo.aspx");
            return 0;
        }

        // If insert was successful, continue processing
        if (orderID > 0)
        {
            // Get list of pis
            List<string> piList = GetPIs();
            foreach (string pi in piList)
            {
                PurchasingTableAdapters.order_approvalsTableAdapter approvalAdapter = new PurchasingTableAdapters.order_approvalsTableAdapter();
                approvalAdapter.Insert(
                    orderID,
                    pi,
                    false,
                    DateTime.Now
                    );
            }

            // Check if user is delegate for all the pis
            bool isDelegated = PurchaseHelper.isDelegated(piList, User.Identity.Name, subtotal, account, orderID);

            // Auto approve order if user is delegate
            if (isDelegated)
            {
                // Update status to approved
                PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
                statusAdapter.Insert(
                    orderID,
                    Convert.ToInt32(PurchaseHelper.OrderStatus.PIApproved),
                    "Request approved: " +
                    PurchaseHelper.DelegateText(piList, User.Identity.Name, subtotal, account),
                    DateTime.Now
                    );
            }

            // Insert items and get HTML table for subsequent e-mailings
            string orderTable = dro_items1.GetHTMLTable();
            dro_items1.InsertItems(orderID);

            // Upload files
            string mySavePath = Server.MapPath(savePath);
            string fileLinks = String.Empty;
            int count = 1;
            foreach (UploadedFile uf in droRadUpload.UploadedFiles)
            {
                string fileName = uf.GetName();
                fileName = StringHelper.SanitizeFileName(fileName, true);
                uf.SaveAs(mySavePath + fileName);
                fileLinks += "<a class='bdrLink' target='_blank' href='" + filePath + fileName + "'>File Attachment #" + count.ToString() + "</a><br />";
                count++;
            }
            if (droRadUpload.UploadedFiles.Count > 0)
                PurchaseHelper.UpdateOrderComments(orderID, "<span style='color:green'>File Attachments:</span><br />" + fileLinks, true);

            if (isDelegated)
            {
                // Send admin mail (ready to place)
                EmailMessage adminEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.PIApproved_Admin, orderID, String.Empty, String.Empty);
                if (notesTextBox.Text.Trim().Length > 0)
                    adminEmail.Body += "<br /><strong>Notes supplied by the requester:</strong><br />" + notesTextBox.Text.Trim();
                if (droRadUpload.UploadedFiles.Count > 0)
                    adminEmail.Body += "<br /><span style='color:green'>File Attachments:</span><br />" + fileLinks;
                MailHelper.SendToPickUp(adminEmail);
            }
            else
            {
                // Send all approvers an email except:
                // - Current PI (if they are one)
                // - PI's that have delegations
                PurchasingTableAdapters.order_approvalsTableAdapter approvalAdapter = new PurchasingTableAdapters.order_approvalsTableAdapter();
                Purchasing.order_approvalsDataTable approvalDt = approvalAdapter.GetByOrderAndApprovalStatus(orderID, false);
                foreach (Purchasing.order_approvalsRow approvalDr in approvalDt.Rows)
                {
                    // Send approver message
                    EmailMessage approverEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.New_Approver, orderID, orderTable, approvalDr.pi_userid);
                    if (notesTextBox.Text.Trim().Length > 0)
                        approverEmail.Body += "<br /><strong>Notes supplied by the requester:</strong><br />" + notesTextBox.Text.Trim();
                    if (droRadUpload.UploadedFiles.Count > 0)
                        approverEmail.Body += "<br /><span style='color:green'>File Attachments:</span><br />" + fileLinks;
                    MailHelper.SendToPickUp(approverEmail);
                }
            }
        }
        return orderID;
    }

    /// <summary>
    /// Subroutine, updates order
    /// </summary>
    /// <param name="orderID"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    protected void UpdateOrder(int orderID, PurchaseHelper.OrderStatus status, string message)
    {
        // Insert/update new vendor
        PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
        int vendorID = addedit_vendor1.HandleVendor(ordersAdapter.GetAuthor(orderID).ToString());

        if (vendorID > 0)
        {
            // Update order
            ordersAdapter.UpdateOrder(
                 orderID,
                 purchaserRadComboBox.SelectedValue,
                 vendorID,
                 siteMultiView.ActiveViewIndex == 0 ? siteNameTextBox.Text.Trim() : siteNameTextBox2.Text.Trim(),
                 InsertAddress(),
                 onSiteRadioButton.Checked ? true : false,
                 shippingRadComboBox.SelectedItem.Text,
                 neededTextBox.Text.Trim(),
                 backorderNoRadioButton.Checked ? false : true,
                 accountTextBox.Text.Trim(),
                 User.Identity.Name,
                 Convert.ToInt32(status),
                 message,
                 null,
                 null,
                 null,
                 null,
                 null,
                 null,
                 null
                 );

            // Notes
            PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
            statusAdapter.UpdateSubmitNotes(orderID, notesTextBox.Text.Trim());
        }
        else
            Msg("An error occured while attempting to insert a new vendor.<br />Our webmaster has been notified.", "Vendor error", true, Request.Url.ToString());
    }

    /// <summary>
    /// Handles over limit warning message
    /// </summary>
    protected void OverTotal()
    {
        sectionTitleLabel.Text = "Requisition Instructions";
        purchaserLabel.Text = PurchaseHelper.ProfileInfo(purchaserRadComboBox.SelectedValue, false);
        subTitleMultiView.Visible = false;
        droMultiView.SetActiveView(View_Requisition);
    }

    /// <summary>
    /// Prompt user with message
    /// </summary>
    /// <param name="msgText"></param>
    /// <param name="error"></param>
    protected void Msg(string msgText, string errorText, bool error, string redirectURL)
    {
        droMultiView.SetActiveView(View_Confirm);
        sectionTitleLabel.Text = String.Empty;
        subTitleMultiView.SetActiveView(View_Blank);
        confirmLabel.Text = msgText;
        confirmLabel.ForeColor = error ? Color.Red : Color.Green;
        if (redirectURL.Length > 0)
            Response.AppendHeader("refresh", "6;url=" + redirectURL);

        if (error)
        {
            MailHelper.SendMailMessage(
                "envnet@ucdavis.edu",
                ConfigurationManager.AppSettings["purchaseAdminEmail"].ToString(),
                String.Empty,
                String.Empty,
                "DRO Error: " + PurchaseHelper.ProfileInfo(User.Identity.Name, false),
                PurchaseHelper.ProfileInfo(User.Identity.Name, false) + " reached an error on the following page " + Request.Url.ToString() + ".<br /><br />Error text:" + errorText + "<br /><br />The user was prompted with the following message: <br />" + msgText);
        }
    }

    // Event, raised on site radio button checked changed
    protected void onSiteRadioButton_CheckedChanged(object sender, EventArgs e)
    {
        siteMultiView.SetActiveView(View_Site_List);
    }

    // Event, raised on siteRadComboBox selected index changed
    protected void siteRadComboBox_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (e.Value == "other")
        {
            siteMultiView.SetActiveView(View_Site_New);
            ClearSiteForm();
            siteNameTextBox2.Text = siteNameTextBox.Text;
        }
    }

    // Handles site back button clicks (back to list)
    protected void siteBackLinkButton_Click(object sender, EventArgs e)
    {
        siteMultiView.SetActiveView(View_Site_List);
        if (siteNameTextBox2.Text.Trim().Length > 0)
            siteNameTextBox.Text = siteNameTextBox2.Text;
        siteRadComboBox.SelectedIndex = 0;
    }

    // Handles site back button clicks (back to choices)
    protected void siteChooseLinkButton_Click(object sender, EventArgs e)
    {
        siteMultiView.SetActiveView(View_ChooseSite);
        onSiteRadioButton.Checked = false;
        offSiteRadioButton.Checked = true;
    }

    // Handles site new button clicks
    protected void siteNewLinkButton_Click(object sender, EventArgs e)
    {
        siteMultiView.SetActiveView(View_Site_New);
        ClearSiteForm();
        siteNameTextBox2.Text = siteNameTextBox.Text;
    }

    // Handles last order button clicks
    protected void lastOrder_Click(object sender, EventArgs e)
    {
        // Cast sender as both link & image button
        LinkButton lb = sender as LinkButton;
        ImageButton ib = sender as ImageButton;

        // Get the value from the correct sender
        int orderID = lb != null ? Convert.ToInt32(lb.CommandArgument) : Convert.ToInt32(ib.CommandArgument);

        // Bind order
        PopulateOrder(orderID);
        dro_items1.PopulateItems(orderID);
    }

    // Handles approve button clicks
    protected void approveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            approvalMultiView.Visible = false;
            int orderID = Convert.ToInt32(Request.QueryString["order"]);

            try
            {
                // Update approval for the current PI
                PurchasingTableAdapters.order_approvalsTableAdapter approvalAdapter = new PurchasingTableAdapters.order_approvalsTableAdapter();
                Purchasing.order_approvalsDataTable dt = approvalAdapter.GetByOrderAndPi(orderID, User.Identity.Name);
                foreach (Purchasing.order_approvalsRow dr in dt.Rows)
                {
                    dr.approval = true;
                    approvalAdapter.Update(dr);
                }

                // Insert approval order-status with notes (not fully approved just yet)
                // Update order
                UpdateOrder(
                    orderID,
                    PurchaseHelper.OrderStatus.PIApproving,
                    approveMessageTextBox.Text.Trim().Length > 0 ? PurchaseHelper.ProfileInfo(User.Identity.Name, true) + " says: " + approveMessageTextBox.Text.Trim() : "Order approved by " + PurchaseHelper.ProfileInfo(User.Identity.Name, true));

                // Re-insert items and get HTML table for subsequent email messages
                PurchasingTableAdapters.dro_itemsTableAdapter itemsAdapter = new PurchasingTableAdapters.dro_itemsTableAdapter();
                itemsAdapter.DeleteByOrder(orderID);
                string itemsTable = dro_items1.GetHTMLTable();
                dro_items1.InsertItems(orderID);

                // Order was successfully approved, confirm with user
                Msg("<h1>Order successfully approved!</h1><p>The order was forwarded to the purchaser to be placed with the vendor.</p>",
                    String.Empty,
                    false,
                    "myorders.aspx?order=" + orderID.ToString()
                    );

                // If all approvals have been collected, send ready to place message to purchaser
                if (approvalAdapter.GetByOrderAndApprovalStatus(orderID, false).Rows.Count == 0)
                {
                    // Insert approved status
                    PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
                    statusAdapter.Insert(orderID, Convert.ToInt32(PurchaseHelper.OrderStatus.PIApproved), String.Empty, DateTime.Now);

                    // Send mail to purchaser
                    EmailMessage adminEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.PIApproved_Admin, orderID, String.Empty, String.Empty);
                    MailHelper.SendToPickUp(adminEmail);
                }
            }
            catch (Exception ex)
            {
                string msg = "Oops! There was a problem approving this order<br /><br />";
                msg += "Our webmaster was notified and you will receive an explanation shortly.<br /><br />";
                msg += "We apologize for the inconvenience.";
                Msg(msg, ex.Message, true, "myorders.aspx?order=" + orderID.ToString());
            }
        }
    }

    // Handles update button clicks
    protected void updateButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            if ((dro_items1.GetOrderSubTotal() * 1.0775) < 4999.99)
            {
                int orderID = Convert.ToInt32(Request.QueryString["order"]);

                try
                {
                    // Update order and set status to "changed"
                    UpdateOrder(orderID, PurchaseHelper.OrderStatus.Changed, updateNotesTextBox.Text.Trim());
                    PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
                    ordersAdapter.UpdateTotal(orderID, Convert.ToDecimal(dro_items1.GetOrderSubTotal()));

                    // Re-insert items and get HTML table for subsequent email messages
                    PurchasingTableAdapters.dro_itemsTableAdapter itemsAdapter = new PurchasingTableAdapters.dro_itemsTableAdapter();
                    itemsAdapter.DeleteByOrder(orderID);
                    string itemsTable = dro_items1.GetHTMLTable();
                    dro_items1.InsertItems(orderID);
                    subTitleMultiView.SetActiveView(View_Blank);

                    // Delete existing approvals
                    PurchasingTableAdapters.order_approvalsTableAdapter approvalAdapter = new PurchasingTableAdapters.order_approvalsTableAdapter();
                    approvalAdapter.DeleteByOrder(orderID);
                    // Insert new approvals
                    List<string> piList = GetPIs();
                    foreach (string pi in piList)
                    {
                        approvalAdapter.Insert(
                            orderID,
                            pi,
                            false,
                            DateTime.Now
                            );
                    }

                    // Handle delegations
                    string author = ordersAdapter.GetAuthor(orderID).ToString();
                    string account = accountTextBox.Text.Trim();
                    double subtotal = dro_items1.GetOrderSubTotal() * 1.0775;
                    string orderTable = dro_items1.GetHTMLTable();

                    // If all go through, then insert approved status and send "ready to place" email to purchser
                    if (PurchaseHelper.isDelegated(piList, author, subtotal, account, orderID))
                    {
                        // Update order status to approved
                        PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
                        statusAdapter.Insert(
                            orderID,
                            Convert.ToInt32(PurchaseHelper.OrderStatus.PIApproved),
                            "Request approved: " +
                            PurchaseHelper.DelegateText(piList, User.Identity.Name, subtotal, account),
                            DateTime.Now
                            );

                        // Send admin mail (ready to place)
                        EmailMessage adminEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.PIApproved_Admin, orderID, itemsTable, String.Empty);
                        MailHelper.SendToPickUp(adminEmail);
                    }
                    else
                    {
                        // Send approval mail to all approvers except delegated and current pi (if they are one)
                        Purchasing.order_approvalsDataTable dt = approvalAdapter.GetByOrderAndApprovalStatus(orderID, false);
                        foreach (Purchasing.order_approvalsRow dr in dt.Rows)
                        {
                            EmailMessage approverEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.Changed_Approver, orderID, orderTable, dr.pi_userid);
                            approverEmail.Body += "<br /><br /><strong>Notes supplied by the requester regarding the changes:</strong><br />" + updateNotesTextBox.Text.Trim();
                            MailHelper.SendToPickUp(approverEmail);
                        }

                        // Send admin mail (warning)
                        EmailMessage adminEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.Changed_Admin, orderID, String.Empty, String.Empty);
                        adminEmail.Body += "<br /><br /><strong>Notes supplied by the requester regarding the changes:</strong><br />" + updateNotesTextBox.Text.Trim();
                        MailHelper.SendToPickUp(adminEmail);
                    }

                    // Confirm with user
                    Msg("<h1>Order successfully updated!</h1><br />You will be notified when the order has been placed with the vendor.",
                        String.Empty,
                        false,
                        "myorders.aspx?order=" + orderID.ToString());
                }
                catch (Exception ex)
                {
                    string msg = "Oops! There was a problem modifying this order<br /><br />";
                    msg += "Our webmaster was notified and you will receive an explanation shortly.<br /><br />";
                    msg += "We apologize for the inconvenience.";
                    Msg(msg, ex.Message, true, Request.Url.ToString());
                }
            }
            else
            {
                // Protect against requisitions
                sectionTitleLabel.ForeColor = Color.Red;
                sectionTitleLabel.Text = "Order updates cannot exceed $5,000.";
                promptLabel.ForeColor = Color.Red;
                promptLabel.Text = "Please submit a new request.";
            }
        }
    }

    // Handles reject button clicks
    protected void rejectButton_Click(object sender, EventArgs e)
    {
        approvalMultiView.Visible = false;
        int orderID = Convert.ToInt32(Request.QueryString["order"]);

        // Update status to "pi sendback"
        PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
        statusAdapter.Insert(
            orderID,
            Convert.ToInt32(PurchaseHelper.OrderStatus.PISendBack),
            PurchaseHelper.ProfileInfo(User.Identity.Name, true) + ": " + rejectMessageTextBox.Text.Trim(),
            DateTime.Now
            );

        // Confirm with user
        Msg(
            "<h1>Order not approved!</h1><p>An e-mail has been sent to the requester notifying him or her that their order may need revisions.</p>",
            String.Empty,
            false,
            "myorders.aspx?order=" + orderID.ToString()
            );

        // Send requester email
        EmailMessage requesterEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.PIRejected_Requester, orderID, String.Empty, String.Empty);
        requesterEmail.Body += "<br /><br /><strong>Notes supplied by the approver:</strong><br /><br />" + rejectMessageTextBox.Text.Trim();
        MailHelper.SendToPickUp(requesterEmail);
    }

    // Handles form submissions
    protected void submitLinkButton_Click(object sender, EventArgs e)
    {
        if ((dro_items1.GetOrderSubTotal() * 1.0775) < 4999.99)
        {
            int orderID = InsertOrder();
            if (orderID > 0)
            {
                // Order was successfully submitted
                Msg(
                "<h1>Order successfully submitted!</h1><p>You will be notified when the order has been placed with the vendor.</p>",
                String.Empty,
                false,
                "myorders.aspx?order=" + orderID.ToString()
                );
            }
            else
            {
                // Order was not submitted
                Msg(
                "<h1>Order could not be submitted!</h1><br />Your 45 minute secure session may have expired. Please click the BACK button on your browser and backup any information you entered.",
                "DRO error when submitting request: " + PurchaseHelper.ProfileInfo(User.Identity.Name, false) + " - Page valid? " + Page.IsValid.ToString(),
                true,
                String.Empty
                );
            }
        }
        else
            OverTotal();
    }

    // Handles back to order button clicks
    protected void reqBackButton_Click(object sender, EventArgs e)
    {
        droMultiView.SetActiveView(View_Order);
        sectionTitleLabel.Text = "Departmental Repair Order";
        subTitleMultiView.Visible = true;
        validationLabel.Text = String.Empty;
    }

    // Handles submitting requisition
    protected void submitReqButton_Click(object sender, EventArgs e)
    {
        // Process requisition
        int orderID = InsertOrder();
        subTitleMultiView.SetActiveView(View_Blank);

        if (orderID > 0)
        {
            PurchaseHelper.UpdateOrderComments(orderID, "<span style='color:red'>Purchase Requisition</span>", true);

            // Order was successfully submitted
            Msg(
                "<h1>Order successfully submitted!</h1><p>You will be notified when the order has been placed with the vendor.</p>",
                String.Empty,
                false,
                "myorders.aspx?order=" + orderID.ToString()
                );
        }
    }

    // Handles submitting requisition with supporting docs
    protected void submitReqWithDocsButton_Click(object sender, EventArgs e)
    {
        // Validate uploads
        string mySavePath = Server.MapPath(savePath);
        string fileLinks = String.Empty;
        bool valid = true;
        string errorMessage = "<script type='text/javascript'>";

        // Purchase specs
        if (specsRadUpload.UploadedFiles.Count == 0)
        {
            valid = false;
            errorMessage += " alert('- You must attach the purchase specifications document'); ";
        }
        else
        {
            foreach (UploadedFile uf in specsRadUpload.UploadedFiles)
            {
                string fileName = uf.GetName();
                fileName = StringHelper.SanitizeFileName(fileName, true);
                uf.SaveAs(mySavePath + fileName);
                fileLinks += "<a class='bdrLink' href='" + filePath + fileName + "'>Purchase Specifications</a><br />";
            }
        }

        // Sole source
        if (soleSourceRadUpload.UploadedFiles.Count > 0)
        {
            foreach (UploadedFile uf in soleSourceRadUpload.UploadedFiles)
            {
                string fileName = uf.GetName();
                fileName = StringHelper.SanitizeFileName(fileName, true);
                uf.SaveAs(mySavePath + fileName);
                fileLinks += "<a class='bdrLink' href='" + filePath + fileName + "'>Sole source justification</a><br />";
            }
        }

        // Vendor quotes
        if (quotesRadUpload.UploadedFiles.Count > 0)
        {
            foreach (UploadedFile uf in quotesRadUpload.UploadedFiles)
            {
                string fileName = uf.GetName();
                fileName = StringHelper.SanitizeFileName(fileName, true);
                uf.SaveAs(mySavePath + fileName);
                fileLinks += "<a class='bdrLink' href='" + filePath + fileName + "'>Vendor quote: " + fileName + "</a><br />";
            }
        }

        // Validate and submit
        if (!valid)
            validationLabel.Text = errorMessage + " </script>";
        else
        {
            // Process requisition
            int orderID = InsertOrder();
            if (orderID > 0)
            {
                PurchaseHelper.UpdateOrderComments(orderID, "<span style='color:red'>Requisition with supporting documents:</span><br />" + fileLinks, true);

                // Order was successfully submitted
                Msg("<h1>Order successfully submitted!</h1>", String.Empty, false, "myorders.aspx?order=" + orderID);
            }
        }
    }
}
