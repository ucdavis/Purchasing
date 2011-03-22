using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using aspNetEmail;
using Telerik.Web.UI;

public partial class business_purchasing_orders_ba : System.Web.UI.Page
{
    // Paths
    protected string savePath = ConfigurationManager.AppSettings["purchasingSavePath"].ToString();
    protected string filePath = ConfigurationManager.AppSettings["purchasingRoot"].ToString() + "admin/uploads/";

    //Page event, raised on page load
    protected void Page_Load(object sender, EventArgs e)
    {
        HandleIE();

        if (!PurchaseHelper.UserIsValid(User.Identity.Name))
            Response.Redirect(ConfigurationManager.AppSettings["purchasingRoot"].ToString());

        if (!IsPostBack)
        {            
            BindPIs();
            BindPurchasers();
            BindShipToAddresses();
            addedit_vendor1.BindVendorsList();
            SetDefaultShipTo();
            UCDMenu.BuildBar_3(Menu3); // Build "In this section" Menu
            Menu3.Items[0].Selected = true;
            sectionTitleLabel.Text = "Business Agreement";
            baMultiView.SetActiveView(View_Order);
            submitMultiView.SetActiveView(View_Submit);

            if (Request.QueryString["order"] != null)
            {
                int orderID = Convert.ToInt32(Request.QueryString["order"]);

                if (PurchaseHelper.OrderExists(orderID))
                {
                    // An order was specified, populate form using order id
                    PopulateOrder(orderID);
                    ba_items1.PopulateItems(orderID);

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
                    Msg("This order no longer exists.", "User tried to navigate to " + Request.Url.ToString(),
                        true, "type.aspx");
                }
            }
            else
            {
                // Pre populate certain fields, NOTE: these may be overwritten below
                string fullName = Users.GetFullNameByUserName(User.Identity.Name, false, false);
                shiptoNameTextBox.Text = fullName;
                baContactTextBox.Text = fullName;
                PurchasingTableAdapters.user_profilesTableAdapter profileAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
                Purchasing.user_profilesDataTable userDt = profileAdapter.GetByUser(User.Identity.Name);
                foreach (Purchasing.user_profilesRow userDr in userDt.Rows)
                {
                    if (!userDr.IsphoneNull())
                        baContactPhoneRadMaskedTextBox.Text = userDr.phone;
                }
                baFromTextBox.Text = DateTime.Now.ToString("MM/dd/yy");
                HandleAgreementView("No-cost Agreement");

                // Get last order
                PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
                Purchasing.ordersDataTable dt = ordersAdapter.GetTop(User.Identity.Name, Convert.ToInt32(PurchaseHelper.OrderType.Agreement));
                foreach (Purchasing.ordersRow dr in dt.Rows)
                {
                    // A previous order exists, show/wire up "populate" button
                    subTitleMultiView.SetActiveView(View_LastOrder);
                    lastOrderImageButton.CommandArgument = dr.id.ToString();
                    lastOrderLinkButton.CommandArgument = dr.id.ToString();

                    // Pre-populate certain fields
                    if (!dr.Isba_ucd_contactNull())
                        baContactTextBox.Text = dr.ba_ucd_contact;
                    if (!dr.Isba_ucd_contact_phoneNull())
                        baContactPhoneRadMaskedTextBox.Text = dr.ba_ucd_contact_phone;
                    if (!dr.Isba_fromNull())
                        baFromTextBox.Text = dr.ba_from;

                    shiptoNameTextBox.Text = dr.shipto_name;

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

                    RadComboBoxItem shipTo = shiptoRadComboBox.Items.FindItemByValue(dr.shipto_id.ToString());
                    if (shipTo != null)
                        shiptoRadComboBox.SelectedIndex = shipTo.Index;
                }
            }
        }
    }

    /// <summary>
    /// Subroutine, handles form modes
    /// </summary>
    /// <param name="mode"></param>
    protected void HandleMode(string mode)
    {
        int orderID = Convert.ToInt32(Request.QueryString["order"]);
        PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
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

                // The pi is valid, show the approve/deny view
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
            //////////////////////////// Agreement ///////////////////////////////
            if (!dr.Isba_typeNull())
            {
                RadComboBoxItem rcbi = baTypeRadComboBox.FindItemByValue(dr.ba_type);
                if (rcbi != null)
                    baTypeRadComboBox.SelectedIndex = rcbi.Index;

                HandleAgreementView(dr.ba_type);
            }

            if (!dr.Isba_contactNull())
                baAgreementContactTextBox.Text = dr.ba_contact;
            if (!dr.Isba_contact_phoneNull())
                baAgreementContactRadMaskedTextBox.Text = dr.ba_contact_phone;
            if (!dr.Isba_ucd_contactNull())
                baContactTextBox.Text = dr.ba_ucd_contact;
            if (!dr.Isba_ucd_contact_phoneNull())
                baContactPhoneRadMaskedTextBox.Text = dr.ba_ucd_contact_phone;
            if (!dr.Isba_fromNull())
                baFromTextBox.Text = dr.ba_from;
            if (!dr.Isba_toNull())
                baToTextBox.Text = dr.ba_to;

            //////////////////////////// Order preferences ///////////////////////////////
            sectionTitleLabel.Text = "Business Agreement #" + dr.unique_id;

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

            shiptoMultiView.SetActiveView(View_ShipTo_List);
            RadComboBoxItem ship = shippingRadComboBox.Items.FindItemByText(dr.shipping);
            if (ship != null)
                shippingRadComboBox.SelectedIndex = ship.Index;

            neededTextBox.Text = dr.date_needed;

            if (!dr.IsaccountNull())
            {
                accountTextBox.Text = dr.account;
                acctNameLabel.Text = dr.account;
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

            addedit_vendor1.PopulateVendorForm(dr.vendor_id);

            //////////////////////////// Ship To ///////////////////////////////
            RadComboBoxItem shipToLi = shiptoRadComboBox.Items.FindItemByValue(dr.shipto_id.ToString());
            if (shipToLi != null)
                shiptoRadComboBox.SelectedIndex = shipToLi.Index;
            shiptoNameTextBox.Text = dr.shipto_name;
            shiptoNameTextBox2.Text = dr.shipto_name;

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
            RadComboBoxItem li = shiptoRadComboBox.Items.FindItemByValue(dr.id.ToString());
            if (li != null)
                shiptoRadComboBox.SelectedIndex = li.Index;
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

        // Add "New Location..." list item
        RadComboBoxItem li = new RadComboBoxItem("New Location...", "other");
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

    /// <summary>
    /// Clear the new shipto form
    /// </summary>
    protected void ClearShipToForm()
    {
        shiptoAddressTextBox.Text = String.Empty;
        shiptoCampusTextBox.Text = "University of California, Davis";
        shiptoCityTextBox.Text = "Davis";
        shiptoStateRadComboBox.SelectedIndex = 4; // Default to CA
        shiptoStreetTextBox.Text = String.Empty;
        shiptoBuildingTextBox.Text = String.Empty;
        shiptoRoomTextBox.Text = String.Empty;
        shiptoZipRadNumericTextBox.Text = String.Empty;
        shiptoZipPlusRadNumericTextBox.Text = String.Empty;
        shiptoPhoneRadMaskedTextBox.Text = String.Empty;
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
    /// Subroutine, inserts order. returns order id (order id of 0 = failed)
    /// </summary>
    /// <returns></returns>
    protected int InsertOrder()
    {
        int orderID = 0;
        double subtotal = ba_items1.GetOrderSubTotal();
        string account = accountTextBox.Text.Trim();

        int vendorID = addedit_vendor1.HandleVendor(User.Identity.Name); // Insert (or) update vendor and customer number
        if (vendorID == 0)
        {
            Msg("An error occured while attempting to insert a new vendor.<br />Our webmaster has been notified.", "Vendor error", true, "ba.aspx");
            return 0;
        }

        // Insert order
        try
        {
            PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
            orderID = Convert.ToInt32(ordersAdapter.InsertOrder(
                User.Identity.Name,
                DateTime.Now.ToString("MMddyy-"),
                Convert.ToInt32(PurchaseHelper.OrderType.Agreement),
                purchaserRadComboBox.SelectedValue,
                vendorID,
                shiptoMultiView.ActiveViewIndex == 0 ? shiptoNameTextBox.Text.Trim() : shiptoNameTextBox2.Text.Trim(),
                InsertAddress(),
                true,
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
                baTypeRadComboBox.SelectedValue,
                baAgreementContactTextBox.Text.Trim(),
                baAgreementContactRadMaskedTextBox.Text.Trim(),
                baContactTextBox.Text.Trim(),
                baContactPhoneRadMaskedTextBox.Text.Trim(),
                baFromTextBox.Text.Trim(),
                baToTextBox.Text.Trim()
                ));
        }
        catch (Exception ex)
        {
            string error = "Oops! There was a problem submitting your order<br /><br />";
            error += "Our webmaster was notified and you will receive an explanation shortly.<br /><br />";
            error += "We apologize for the inconvenience.";
            Msg(error, ex.Message, true, "ba.aspx");
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
            string style = "border-bottom: 1px solid #000000; border-right: 1px solid #000000;";
            string orderTable = "<br /><br /><table cellspacing='0' cellpadding='2' width='100%' style='width:100%; font-size:11px; border-top: 1px solid #000000; border-left: 1px solid #000000;'>";
            orderTable += "<tr><td style='" + style + "' colspan='2'><strong>Agreement Details</strong></td>";
            orderTable += "<tr><td style='" + style + "'>Agreement Type</td><td style='" + style + "'>" + baTypeRadComboBox.SelectedValue + "</td></tr>";
            orderTable += "<tr><td style='" + style + "'>Agreement Contact</td><td style='" + style + "'>" + baAgreementContactTextBox.Text + " " + StringHelper.FormatPhone(baAgreementContactRadMaskedTextBox.Text, false) + "</td></tr>";
            orderTable += "<tr><td style='" + style + "'>UCD Contact</td><td style='" + style + "'>" + baContactTextBox.Text + " " + StringHelper.FormatPhone(baContactPhoneRadMaskedTextBox.Text, false) + "</td></tr>";
            orderTable += "<tr><td style='" + style + "'>Effective</td><td style='" + style + "'>From: " + baFromTextBox.Text + " To: " + baToTextBox.Text + "</td></tr>";
            orderTable += "</table><br />";

            if (baTypeRadComboBox.SelectedValue != "No-cost Agreement")
            {
                ba_items1.InsertItems(orderID);
                orderTable += ba_items1.GetHTMLTable();
            }

            // Upload files
            string mySavePath = Server.MapPath(savePath);
            string fileLinks = String.Empty;
            int count = 1;
            foreach (UploadedFile uf in baRadUpload.UploadedFiles)
            {
                string fileName = uf.GetName();
                fileName = StringHelper.SanitizeFileName(fileName, true);
                uf.SaveAs(mySavePath + fileName);
                fileLinks += "<a class='bdrLink' target='_blank' href='" + filePath + fileName + "'>File Attachment #" + count.ToString() + "</a><br />";
                count++;
            }
            if (baRadUpload.UploadedFiles.Count > 0)
                PurchaseHelper.UpdateOrderComments(orderID, "<span style='color:green'>File Attachments:</span><br />" + fileLinks, true);

            if (isDelegated)
            {
                // Send admin mail (ready to place)
                EmailMessage adminEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.PIApproved_Admin, orderID, String.Empty, String.Empty);
                if (notesTextBox.Text.Trim().Length > 0)
                    adminEmail.Body += "<br /><strong>Notes supplied by the requester:</strong><br />" + notesTextBox.Text.Trim();
                if (baRadUpload.UploadedFiles.Count > 0)
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
                    if (baRadUpload.UploadedFiles.Count > 0)
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
                 shiptoMultiView.ActiveViewIndex == 0 ? shiptoNameTextBox.Text.Trim() : shiptoNameTextBox2.Text.Trim(),
                 InsertAddress(),
                 true,
                 shippingRadComboBox.SelectedItem.Text,
                 neededTextBox.Text.Trim(),
                 backorderNoRadioButton.Checked ? false : true,
                 accountTextBox.Text.Trim(),
                 User.Identity.Name,
                 Convert.ToInt32(status),
                 message,
                 baTypeRadComboBox.SelectedValue,
                 baAgreementContactTextBox.Text.Trim(),
                 baAgreementContactRadMaskedTextBox.Text.Trim(),
                 baContactTextBox.Text.Trim(),
                 baContactPhoneRadMaskedTextBox.Text.Trim(),
                 baFromTextBox.Text.Trim(),
                 baToTextBox.Text.Trim()
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
        baMultiView.SetActiveView(View_Requisition);
    }

    /// <summary>
    /// Determines agreement view
    /// </summary>
    /// <param name="agreement"></param>
    protected void HandleAgreementView(string agreement)
    {
        if (agreement == "No-cost Agreement" || agreement == "Select One...")
            agreementMultiView.SetActiveView(View_NoCost);
        else
            agreementMultiView.SetActiveView(View_Blanket);
    }

    /// <summary>
    /// Prompt user with message
    /// </summary>
    /// <param name="msgText"></param>
    /// <param name="error"></param>
    protected void Msg(string msgText, string errorText, bool error, string redirectURL)
    {
        baMultiView.SetActiveView(View_Confirm);
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
                "BA Error: " + PurchaseHelper.ProfileInfo(User.Identity.Name, false),
                PurchaseHelper.ProfileInfo(User.Identity.Name, false) + " reached an error on the following page " + Request.Url.ToString() + ".<br /><br />Error text:" + errorText + "<br /><br />The user was prompted with the following message: <br />" + msgText);
        }
    }

    // Event, raised on shiptoRadComboBox selected index changed
    protected void shiptoRadComboBox_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (e.Value == "other")
        {
            shiptoMultiView.SetActiveView(View_ShipTo_New);
            ClearShipToForm();
            shiptoNameTextBox2.Text = shiptoNameTextBox.Text;
        }
    }

    // Event, raised on ba type selected index changed
    protected void baTypeRadComboBox_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        HandleAgreementView(e.Value);
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
                PurchasingTableAdapters.ba_itemsTableAdapter itemsAdapter = new PurchasingTableAdapters.ba_itemsTableAdapter();
                itemsAdapter.DeleteByOrder(orderID);
                string itemsTable = ba_items1.GetHTMLTable();
                ba_items1.InsertItems(orderID);

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
            if ((ba_items1.GetOrderSubTotal() * 1.0775) < 4999.99)
            {
                int orderID = Convert.ToInt32(Request.QueryString["order"]);

                try
                {
                    // Update order and set status to "changed"
                    UpdateOrder(orderID, PurchaseHelper.OrderStatus.Changed, updateNotesTextBox.Text.Trim());
                    PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
                    ordersAdapter.UpdateTotal(orderID, Convert.ToDecimal(ba_items1.GetOrderSubTotal()));

                    // Re-insert items and get HTML table for subsequent email messages
                    PurchasingTableAdapters.ba_itemsTableAdapter itemsAdapter = new PurchasingTableAdapters.ba_itemsTableAdapter();
                    itemsAdapter.DeleteByOrder(orderID);
                    string itemsTable = ba_items1.GetHTMLTable();
                    ba_items1.InsertItems(orderID);
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
                    double subtotal = ba_items1.GetOrderSubTotal() * 1.0775;
                    string orderTable = ba_items1.GetHTMLTable();

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
            "<h1>Order not approved!</h1><p>An e-mail has been sent to the requester notifying him or her that their order may need revisions.<p>",
            String.Empty,
            false,
            "myorders.aspx?order=" + orderID.ToString()
            );

        // Send requester email
        EmailMessage requesterEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.PIRejected_Requester, orderID, String.Empty, String.Empty);
        requesterEmail.Body += "<br /><br /><strong>Notes supplied by the approver:</strong><br /><br />" + rejectMessageTextBox.Text.Trim();
        MailHelper.SendToPickUp(requesterEmail);
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
        ba_items1.PopulateItems(orderID);
    }

    // Handles form submissions
    protected void submitLinkButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            if ((ba_items1.GetOrderSubTotal() * 1.0775) < 4999.99 || baTypeRadComboBox.SelectedValue == "No-cost Agreement")
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
                    "BA error when submitting request: " + PurchaseHelper.ProfileInfo(User.Identity.Name, false) + " - Page valid? " + Page.IsValid.ToString(),
                    true,
                    String.Empty
                    );
                }
            }
            else
                OverTotal();
        }
    }

    // Handles shipto back button clicks
    protected void shiptoBackLinkButton_Click(object sender, EventArgs e)
    {
        shiptoMultiView.SetActiveView(View_ShipTo_List);
        if (shiptoNameTextBox2.Text.Trim().Length > 0)
            shiptoNameTextBox.Text = shiptoNameTextBox2.Text;
        shiptoRadComboBox.SelectedIndex = 0;
    }

    // Handles shipto new button clicks
    protected void shiptoNewLinkButton_Click(object sender, EventArgs e)
    {
        shiptoMultiView.SetActiveView(View_ShipTo_New);
        ClearShipToForm();
        shiptoNameTextBox2.Text = shiptoNameTextBox.Text;
    }

    // Handles back to order button clicks
    protected void reqBackButton_Click(object sender, EventArgs e)
    {
        baMultiView.SetActiveView(View_Order);
        sectionTitleLabel.Text = "Business Agreement";
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
                Msg("<h1>Order successfully submitted!</h1><p>You will be notified when the order has been placed with the vendor.</p>", String.Empty, false, "myorders.aspx?order=" + orderID);
            }
        }
    }

}