using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using aspNetEmail;
using Telerik.Web.UI;

public partial class tools_purchasing_admin_default : System.Web.UI.Page
{
    // Paths
    protected string savePath = ConfigurationManager.AppSettings["purchasingSavePath"].ToString();
    protected string filePath = ConfigurationManager.AppSettings["purchasingRoot"].ToString() + "admin/uploads/";
    protected static bool temporaryLock;

    //Page event, raised on page load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PurchaseHelper.UserIsValid(User.Identity.Name))
            Response.Redirect(ConfigurationManager.AppSettings["purchasingRoot"].ToString());
        if (!IsPostBack)
        {
            // Redirect loop until unit is present
            if (Request.QueryString.Count == 0)
            {
                PurchasingTableAdapters.unitsTableAdapter unitsAdapter = new PurchasingTableAdapters.unitsTableAdapter();
                Purchasing.unitsDataTable dt = unitsAdapter.GetByUser(User.Identity.Name);
                foreach (Purchasing.unitsRow dr in dt.Rows)
                    Response.Redirect("default.aspx?unit=" + dr.id + "&t=" + Server.UrlEncode(dr.unit_abrv));
            }

            UCDMenu.BuildBar_3(Menu3); // Build "In this section" Menu

            BindPanelBar(); // Build "View Orders By" panel in left_sidebar

            // Hide/show the admin "Manage" and archive panels
            adminPanel.Visible = IsUserAdmin();
            acrhivePanel.Visible = IsUserAdmin();
            archiveButton.Visible = IsUserAdmin();

            if (Request.QueryString["order"] != null)
            {
                int orderID = Convert.ToInt32(Request.QueryString["order"]);
                if (PurchaseHelper.OrderExists(orderID))
                {
                    // Auto-complete e-mail field in receipt dialouge, then switch view
                    PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
                    receiptTextBox.Text = Users.GetEmailByUserName(ordersAdapter.GetAuthor(orderID).ToString());
                    adminMultiView.SetActiveView(View_OrderDetails);
                }
                else
                {
                    confirmLabel.Text = "This order no longer exists.";
                    adminMultiView.SetActiveView(View_OrderConfirm);
                }
            }
            else
                adminMultiView.SetActiveView(View_Orders);

            // Handle datasource
            HandleDataSource();
            ordersRadGrid.DataBind();
        }
    }

    /// <summary>
    /// Checks if current user is in an administrative role
    /// </summary>
    /// <returns></returns>
    protected bool IsUserAdmin()
    {
        string[] roles = new string[] { "MasterAdmin", "MajorAdmin", "AssistantAdmin", "PurchaseAdmin" };
        return Users.UserIsInRoles(User.Identity.Name, roles);
    }

    /// <summary>
    /// Determines the grid's datasource based on the available query strings
    /// </summary>
    protected void HandleDataSource()
    {
        PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();

        // Determine if we need to show archived orders
        bool archived = archiveCheckBox.Checked;

        if (Request.QueryString["unit"] != null && Request.QueryString["t"] != null)
        {
            // Bind orders by unit if "unit" query is present
            int unitID = Convert.ToInt32(Request.QueryString["unit"]);
            sectionTitleLabel.Text = String.Format("{0} Orders", Server.UrlDecode(Request.QueryString["t"]));
            ordersRadGrid.DataSource = ordersAdapter.GetByUnit(unitID, archived);

            if (archived)
                sectionTitleLabel.Text += " (Archived)";
        }
        else if (Request.QueryString["user"] != null && Request.QueryString["t"] != null)
        {
            // Bind orders by user if "user" query is present
            string userID = Request.QueryString["user"].ToString();
            sectionTitleLabel.Text = String.Format("{0}'s Orders", Server.UrlDecode(Request.QueryString["t"]));
            ordersRadGrid.DataSource = ordersAdapter.GetByUser(userID, archived);

            if (archived)
                sectionTitleLabel.Text += " (Archived)";
        }
        else if (Request.QueryString["vendor"] != null && Request.QueryString["t"] != null)
        {
            // Bind orders by user if "vendor" query is present
            int vendorID = Convert.ToInt32(Request.QueryString["vendor"]);
            sectionTitleLabel.Text = String.Format("{0} Orders", Server.UrlDecode(Request.QueryString["t"]));
            ordersRadGrid.DataSource = ordersAdapter.GetByVendor(vendorID, archived);

            if (archived)
                sectionTitleLabel.Text += " (Archived)";
        }
        else if (Request.QueryString["status"] != null)
        {
            // Bind orders by status if "status" query is present
            string title = String.Empty;
            switch (Request.QueryString["status"].ToString())
            {
                case "new":
                    title = "New";
                    ordersRadGrid.DataSource = ordersAdapter.GetByStatus(
                        Convert.ToInt32(PurchaseHelper.OrderStatus.Changed).ToString(),
                        "<=",
                        archived ? "1" : "0"
                        );
                    break;
                case "pi":
                    title = "P.I. Approved";
                    ordersRadGrid.DataSource = ordersAdapter.GetByStatus(
                        Convert.ToUInt32(PurchaseHelper.OrderStatus.PIApproved).ToString(),
                        "=",
                        archived ? "1" : "0"
                        );
                    break;
                case "pireg":
                    title = "P.I. Rejected";
                    ordersRadGrid.DataSource = ordersAdapter.GetByStatus(
                        Convert.ToUInt32(PurchaseHelper.OrderStatus.PISendBack).ToString(),
                        "=",
                        archived ? "1" : "0"
                        );
                    break;
                case "placed":
                    title = "Placed";
                    ordersRadGrid.DataSource = ordersAdapter.GetByStatus(
                        Convert.ToUInt32(PurchaseHelper.OrderStatus.AdminApproved).ToString(),
                        "=",
                        archived ? "1" : "0"
                        );
                    break;
                case "received":
                    title = "Received";
                    ordersRadGrid.DataSource = ordersAdapter.GetByStatus(
                        Convert.ToUInt32(PurchaseHelper.OrderStatus.Delivered).ToString(),
                        "=",
                        archived ? "1" : "0"
                        );
                    break;
                case "notreceived":
                    title = "Not Received";
                    ordersRadGrid.DataSource = ordersAdapter.GetByStatus(
                        Convert.ToUInt32(PurchaseHelper.OrderStatus.NotReceived).ToString(),
                        "=",
                        archived ? "1" : "0"
                        );
                    break;
                case "conflict":
                    title = "Shipping/Vendor Conflict";
                    ordersRadGrid.DataSource = ordersAdapter.GetByStatus(
                        Convert.ToUInt32(PurchaseHelper.OrderStatus.ShippingVendorConflict).ToString(),
                        "=",
                        archived ? "1" : "0"
                        );
                    break;
                case "req":
                    title = "Requisitions";
                    ordersRadGrid.DataSource = ordersAdapter.GetByLimit(true, archived);
                    break;
                case "all":
                    title = "All Orders";
                    ordersRadGrid.DataSource = ordersAdapter.GetAll(archived);
                    break;
                default:
                    break;
            }
            sectionTitleLabel.Text = String.Format("Orders ({0})", title);

            if (archived)
                sectionTitleLabel.Text += " (Archived)";
        }
    }

    /// <summary>
    /// Subroutine, returns approval status
    /// </summary>
    /// <param name="orderID"></param>
    /// <returns></returns>
    protected string ParseApproval(object orderID)
    {
        PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
        int id = Convert.ToInt32(orderID);
        PurchaseHelper.OrderStatus orderStatus = PurchaseHelper.GetOrderStatusByOrderId(id);
        DateTime approvalDate = Convert.ToDateTime(statusAdapter.GetDateByOrderStatus(id, Convert.ToInt32(PurchaseHelper.OrderStatus.PIApproved)));
        DateTime beginningOfTime = new DateTime();

        if (approvalDate != beginningOfTime)
        {
            if (orderStatus == PurchaseHelper.OrderStatus.PISendBack)
                return "<a href='default.aspx?order=" + id.ToString() + "' class='bdrLink'><img src='" + ResolveClientUrl("~/images/15/warning.gif") + "' alt='' title='Revisions needed' /></a>";
            else if (orderStatus == PurchaseHelper.OrderStatus.StopRequested)
                return String.Empty;
            else if (orderStatus == PurchaseHelper.OrderStatus.Stopped)
                return String.Empty;
            else if (orderStatus > PurchaseHelper.OrderStatus.PISendBack && orderStatus != PurchaseHelper.OrderStatus.AdminSendBack)
                return "<img src='" + ResolveClientUrl("~/images/15/check.gif") + "' alt='' title='Order approved on " + approvalDate.ToString("M/d/yy h:mm tt") + "'/>";
            else
                return String.Empty;
        }
        else
            return String.Empty;
    }

    /// <summary>
    /// Subroutine, returns ordered status
    /// </summary>
    /// <param name="orderID"></param>
    /// <returns></returns>
    protected string ParseOrdered(object orderID)
    {
        PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
        int id = Convert.ToInt32(orderID);
        PurchaseHelper.OrderStatus orderStatus = PurchaseHelper.GetOrderStatusByOrderId(id);
        DateTime dt = Convert.ToDateTime(statusAdapter.GetDateByOrderStatus(id, Convert.ToInt32(PurchaseHelper.OrderStatus.AdminApproved)));

        if (orderStatus == PurchaseHelper.OrderStatus.AdminSendBack)
            return "<a href='default.aspx?order=" + id.ToString() + "' class='bdrLink'><img src='" + ResolveClientUrl("~/images/15/warning.gif") + "' alt='' title='Revisions needed' /></a>";
        else if (orderStatus == PurchaseHelper.OrderStatus.AdminApproved ||
            orderStatus == PurchaseHelper.OrderStatus.ShippingVendorConflict ||
            orderStatus == PurchaseHelper.OrderStatus.Delivered ||
            orderStatus == PurchaseHelper.OrderStatus.NotReceived)
            return "<img src='" + ResolveClientUrl("~/images/15/check.gif") + "' alt='' title='Order placed on " + dt.ToString("M/d/yy h:mm tt") + "'/>";
        else if (orderStatus == PurchaseHelper.OrderStatus.StopRequested)
            return "<img src='" + ResolveClientUrl("~/images/15/stop.gif") + "' alt='' title='Stop Requested'/>";
        else if (orderStatus == PurchaseHelper.OrderStatus.Stopped)
            return "<img src='" + ResolveClientUrl("~/images/15/stop.gif") + "' alt='' title='Order Stopped'/>";
        else
            return String.Empty;
    }

    /// <summary>
    /// Subroutine, returns received status
    /// </summary>
    /// <param name="orderID"></param>
    /// <returns></returns>
    protected string ParseReceived(object orderID)
    {
        int id = Convert.ToInt32(orderID);
        PurchaseHelper.OrderStatus orderStatus = PurchaseHelper.GetOrderStatusByOrderId(id);

        if (orderStatus == PurchaseHelper.OrderStatus.ShippingVendorConflict)
            return "<img src='" + ResolveClientUrl("~/images/15/box_returned.gif") + "' alt='' title='Shipping/Vendor Conflict'/>";
        else if (orderStatus == PurchaseHelper.OrderStatus.Delivered)
            return "<img src='" + ResolveClientUrl("~/images/15/box_delivered.gif") + "' alt='' title='Order Received'/>";
        else if (orderStatus == PurchaseHelper.OrderStatus.NotReceived)
            return "<img src='" + ResolveClientUrl("~/images/15/box_pending.gif") + "' alt='' title='Order Not Received'/>";
        else
            return String.Empty;
    }

    /// <summary>
    /// Subroutine, returns user's full name
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected string ParseUser(object sender)
    {
        string userid = sender.ToString();
        try
        {
            return PurchaseHelper.ProfileInfo(userid, false);
        }
        catch (Exception)
        {
            return userid;
        }
    }

    /// <summary>
    /// Returns user's full name
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected string GetFullName(object sender)
    {
        return Users.GetFullNameByUserName(sender.ToString(), false, false);
    }

    /// <summary>
    /// Returns trimmed account
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected string TrimAccount(object sender)
    {
        string account = sender.ToString();
        return account.Length > 20 ? "<span title='" + account + "'>" + account.Substring(0, 19).Trim() + "...</span>" : account;
    }

    /// <summary>
    /// Subroutine, binds orders panel bar
    /// </summary>
    protected void BindPanelBar()
    {
        PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
        PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();

        ////////// Archived ///////////
        RadPanelItem archiveLi = new RadPanelItem("All Orders", "?status=all");
        ordersRadPanelBar.Items.Add(archiveLi);

        ////////// Bind units ///////////
        PurchasingTableAdapters.unitsTableAdapter unitsAdapter = new PurchasingTableAdapters.unitsTableAdapter();
        RadPanelItem unitsLi = new RadPanelItem("Unit");
        unitsLi.Expanded = false;
        Purchasing.unitsDataTable unitsTable = unitsAdapter.Get();
        foreach (Purchasing.unitsRow unitsRow in unitsTable.Rows)
        {
            RadPanelItem rpi = new RadPanelItem();
            rpi.Text = unitsRow.unit_abrv;

            // Add count of orders
            int count = Convert.ToInt32(ordersAdapter.CountByUnit(unitsRow.id));
            if (count > 0)
                rpi.Text += " (" + count.ToString() + ")";

            rpi.NavigateUrl = "?unit=" + unitsRow.id.ToString() + "&t=" + Server.UrlEncode(unitsRow.unit_abrv);
            unitsLi.Items.Add(rpi);
        }
        ordersRadPanelBar.Items.Add(unitsLi);

        ////////// Bind Vendors ////////////
        PurchasingTableAdapters.vendorsTableAdapter vendorAdapter = new PurchasingTableAdapters.vendorsTableAdapter();
        RadPanelItem vendorLi = new RadPanelItem("Vendor");
        vendorLi.Expanded = false;
        Purchasing.vendorsDataTable vendorTable = vendorAdapter.Get();
        foreach (Purchasing.vendorsRow vendorRow in vendorTable.Rows)
        {
            RadPanelItem vendorItem = new RadPanelItem();

            vendorItem.Text = vendorRow.vendor_name;
            int count = Convert.ToInt32(ordersAdapter.CountByVendor(vendorRow.id));
            if (count > 0)
                vendorItem.Text += " (" + count.ToString() + ")";
            vendorItem.NavigateUrl = "?vendor=" + vendorRow.id.ToString() + "&t=" + Server.UrlEncode(vendorRow.vendor_name);
            vendorLi.Items.Add(vendorItem);
        }
        ordersRadPanelBar.Items.Add(vendorLi);

        ////////// Bind PIs ////////////
        RadPanelItem piLi = new RadPanelItem("PI/Approver");
        piLi.Expanded = false;
        DataSet ds = Users.GetUsersByRoleName("PurchaseApprover", false);
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            RadPanelItem rpi = new RadPanelItem();
            string piUserid = dr["UserName"].ToString();
            rpi.Text = dr["FullName"].ToString();
            int count = Convert.ToInt32(ordersAdapter.CountByUser(piUserid));
            if (count > 0)
                rpi.Text += " (" + count.ToString() + ")";
            rpi.NavigateUrl = String.Format("?user={0}&t={1}", piUserid, Server.UrlEncode(dr["FullName"].ToString()));
            piLi.Items.Add(rpi);
        }
        ordersRadPanelBar.Items.Add(piLi);

        ////////// Bind Requesters ////////////
        RadPanelItem requesterLi = new RadPanelItem("Requester");
        requesterLi.Expanded = false;
        DataSet dsRequester = Users.GetUsersByRoleName("PurchaseUser", false);
        foreach (DataRow drRequester in dsRequester.Tables[0].Rows)
        {
            RadPanelItem rpi = new RadPanelItem();
            string requesterUserid = drRequester["UserName"].ToString();

            rpi.Text = drRequester["FullName"].ToString();
            int count = Convert.ToInt32(ordersAdapter.CountByUser(requesterUserid));
            if (count > 0)
                rpi.Text += " (" + count.ToString() + ")";

            rpi.NavigateUrl = String.Format("?user={0}&t={1}", requesterUserid, Server.UrlEncode(drRequester["FullName"].ToString()));
            requesterLi.Items.Add(rpi);
        }
        ordersRadPanelBar.Items.Add(requesterLi);

        ////////// Bind Purchasers ////////////
        RadPanelItem adminLi = new RadPanelItem("Purchase Admin");
        adminLi.Expanded = false;
        DataSet dsAdmin = Users.GetUsersByRoleName("PurchaseAdmin", false);
        foreach (DataRow drAdmin in dsAdmin.Tables[0].Rows)
        {
            RadPanelItem rpi = new RadPanelItem();
            string adminUserid = drAdmin["UserName"].ToString();

            rpi.Text = drAdmin["FullName"].ToString();
            int count = Convert.ToInt32(ordersAdapter.CountByUser(adminUserid));
            if (count > 0)
                rpi.Text += " (" + count.ToString() + ")";

            rpi.NavigateUrl = String.Format("?user={0}&t={1}", adminUserid, Server.UrlEncode(drAdmin["FullName"].ToString()));
            adminLi.Items.Add(rpi);
        }
        ordersRadPanelBar.Items.Add(adminLi);

        ////////// Order Status ////////////
        RadPanelItem statusLi = new RadPanelItem("Status");
        RadPanelItem piApprovedLi = new RadPanelItem("P.I. Approved (" + statusAdapter.CountByStatus(Convert.ToInt32(PurchaseHelper.OrderStatus.PIApproved).ToString(), "=") + ")", "?status=pi");
        RadPanelItem piRejectedLi = new RadPanelItem("P.I. Rejected (" + statusAdapter.CountByStatus(Convert.ToInt32(PurchaseHelper.OrderStatus.PISendBack).ToString(), "=") + ")", "?status=pireg");
        RadPanelItem placedLi = new RadPanelItem("Placed (" + statusAdapter.CountByStatus(Convert.ToInt32(PurchaseHelper.OrderStatus.AdminApproved).ToString(), "=") + ")", "?status=placed");
        RadPanelItem receivedLi = new RadPanelItem("Received (" + statusAdapter.CountByStatus(Convert.ToInt32(PurchaseHelper.OrderStatus.Delivered).ToString(), "=") + ")", "?status=received");
        RadPanelItem notReceivedLi = new RadPanelItem("Not Received (" + statusAdapter.CountByStatus(Convert.ToInt32(PurchaseHelper.OrderStatus.NotReceived).ToString(), "=") + ")", "?status=notreceived");
        RadPanelItem conflictLi = new RadPanelItem("Conflict (" + statusAdapter.CountByStatus(Convert.ToInt32(PurchaseHelper.OrderStatus.ShippingVendorConflict).ToString(), "=") + ")", "?status=conflict");
        RadPanelItem newLi = new RadPanelItem("New (" + statusAdapter.CountByStatus(Convert.ToInt32(PurchaseHelper.OrderStatus.Changed).ToString(), "<=") + ")", "?status=new");
        RadPanelItem requisitionLi = new RadPanelItem("Requisition (" + ordersAdapter.CountRequisitions().ToString() + ")", "?status=req");

        statusLi.Items.Add(newLi);
        statusLi.Items.Add(piApprovedLi);
        statusLi.Items.Add(piRejectedLi);
        statusLi.Items.Add(placedLi);
        statusLi.Items.Add(receivedLi);
        statusLi.Items.Add(notReceivedLi);
        statusLi.Items.Add(conflictLi);
        statusLi.Items.Add(requisitionLi);

        ordersRadPanelBar.Items.Add(statusLi);

        ordersRadPanelBar.CollapseAllItems();
    }

    /// <summary>
    /// Subroutine, handles IE browser
    /// </summary>
    protected bool isIE()
    {
        System.Web.HttpBrowserCapabilities browser = Request.Browser;
        if (browser.Browser == "IE")
            return true;
        else
            return false;
    }

    /// <summary>
    /// Prompt user with message
    /// </summary>
    /// <param name="msgText"></param>
    /// <param name="error"></param>
    protected void Msg(string msgText, string errorText, bool error, string redirectURL)
    {
        // Confirm with user
        adminMultiView.SetActiveView(View_OrderConfirm);
        if (msgText.Length > 0)
        {
            sectionTitleLabel.Text = String.Empty;
            confirmLabel.Text = msgText;
        }
        confirmLabel.ForeColor = error ? Color.Red : Color.Green;
        Response.AppendHeader("refresh", "2;url=" + redirectURL);

        if (error)
        {
            MailHelper.SendMailMessage(
                "envnet@ucdavis.edu",
                ConfigurationManager.AppSettings["purchaseAdminEmail"].ToString(),
                String.Empty,
                String.Empty,
                "Admin Error: " + PurchaseHelper.ProfileInfo(User.Identity.Name, false),
                PurchaseHelper.ProfileInfo(User.Identity.Name, false) + " reached an error on the following page: " + Request.Url.ToString() + ".<br /><br />Error text:" + errorText + "<br /><br />The user was prompted with the following message: <br />" + msgText);
        }
    }

    // Event, raised on export ddl index change
    protected void exportDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        ordersRadGrid.ExportSettings.FileName = Users.GetFullNameByUserName(User.Identity.Name, true, true) + "_Orders_" + DateTime.Now.ToString("M_d_yy");
        ordersRadGrid.MasterTableView.GetColumn("commands").Visible = false;
        ordersRadGrid.MasterTableView.GetColumn("approved").Visible = false;
        ordersRadGrid.MasterTableView.GetColumn("placed").Visible = false;
        ordersRadGrid.MasterTableView.GetColumn("received").Visible = false;
        ordersRadGrid.Rebind();

        switch (exportDropDownList.SelectedValue)
        {
            case "Excel":
                ordersRadGrid.MasterTableView.ExportToExcel();
                break;
            case "Word":
                ordersRadGrid.MasterTableView.ExportToWord();
                break;
            case "PDF":
                ordersRadGrid.MasterTableView.ExportToPdf();
                break;
            default: break;
        }
    }

    // Event, raised after admin multiview index changed
    protected void adminMultiView_ActiveViewChanged(object sender, EventArgs e)
    {

        if (adminMultiView.ActiveViewIndex == 0)
        {
            commandsMultiView.SetActiveView(View_Commands_Export);
            acrhivePanel.Visible = IsUserAdmin();
        }
        else if (adminMultiView.ActiveViewIndex == 1)
        {
            PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
            int orderID = Convert.ToInt32(Request.QueryString["order"]);

            Purchasing.ordersDataTable dt = ordersAdapter.GetById(orderID);
            foreach (Purchasing.ordersRow dr in dt.Rows)
            {
                // Order type
                PurchaseHelper.OrderType ot = (PurchaseHelper.OrderType)dr.type;
                if (ot == PurchaseHelper.OrderType.DPO)
                    sectionTitleLabel.Text = "JMIE Purchase Order #";
                else if (ot == PurchaseHelper.OrderType.DRO)
                    sectionTitleLabel.Text = "JMIE Repair Order #";
                else if (ot == PurchaseHelper.OrderType.Agreement)
                    sectionTitleLabel.Text = "JMIE Business Agreement #";

                // Unique ID
                sectionTitleLabel.Text += dr.unique_id;
                sectionTitleLabel.Text = dr.locked ?
                    "<img src='" + ResolveClientUrl("~/images/32/") + "lock2.gif' style='display: block; float: left; vertical-align: middle;' title='Order locked'/>" + sectionTitleLabel.Text : sectionTitleLabel.Text;

                // Order details
                if (!dr.Isorder_totalNull())
                    dtlTotalRadNumericTextBox.Text = dr.order_total.ToString();
                if (!dr.Isdafis_docNull())
                    dtlDocTextBox.Text = dr.dafis_doc;
                if (!dr.Isdafis_poNull())
                    dtlPoTextBox.Text = dr.dafis_po;
                if (!dr.Isconfirmation_numNull())
                    dtlVendorConfirmationTextBox.Text = dr.confirmation_num;

                // Locked status
                lockLabel.Text = dr.locked ? "Unlock this order" : "Lock this order";
                qtLockImage.ImageUrl = dr.locked ? ResolveClientUrl("~/images/15/") + "qtlock_open.gif" : ResolveClientUrl("~/images/15/") + "qtlock.gif";
                qtLockLinkButton.CommandName = dr.locked ? "Unlock" : "Lock";
                qtLockLinkButton.OnClientClick = dr.locked ? "return confirm('Are you sure you want to un-lock this order?')" : "return confirm('Are you sure you want to lock this order?');";
            }

            // Switch to order details view
            commandsMultiView.SetActiveView(View_Commands_Detail);
            acrhivePanel.Visible = false;

            // Hide/show admin "Manage" button
            qtButton.Visible = IsUserAdmin();
        }
        else if (adminMultiView.ActiveViewIndex == 2)
        {
            sectionTitleLabel.Text = "Place Order";
            commandsMultiView.SetActiveView(View_Commands_Blank);
            actualTotalRadNumericTextBox.Text = String.Empty;
            messageTextBox.Text = "I received your approved order.  Please submit original receipts to the Business Office.";
            spanTextBox.Text = "5";
            acrhivePanel.Visible = false;
        }
        else if (adminMultiView.ActiveViewIndex == 3)
        {
            sectionTitleLabel.Text = "Approval Required";
            commandsMultiView.SetActiveView(View_Commands_Blank);
            acrhivePanel.Visible = false;
        }
        else
            commandsMultiView.SetActiveView(View_Commands_Blank);
    }

    // Event, raised on rad grid init
    protected void ordersRadGrid_Init(object sender, System.EventArgs e)
    {
        GridFilterMenu menu = ordersRadGrid.FilterMenu;
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

    // Event, raised when the rad grid needs a datasource
    protected void ordersRadGrid_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        HandleDataSource();
    }

    // Event, raised after grid data bound
    protected void ordersRadGrid_DataBound(object sender, EventArgs e)
    {
        if (archiveCheckBox.Checked)
        {
            archiveButton.Text = "Unarchive Selected";
            archiveButton.CommandName = "Unarchive";
        }
        else
        {
            archiveButton.Text = "Archive Selected";
            archiveButton.CommandName = "Archive";
        }
    }

    // Event, raised on archive checkbox checked changed
    protected void archiveCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        // Handle datasource
        adminMultiView.SetActiveView(View_Orders);
        HandleDataSource();
        ordersRadGrid.DataBind();
    }

    // Event, raised on grid item data bound
    protected void ordersRadGrid_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        // Is it a GridDataItem?
        if (e.Item is GridDataItem)
        {
            // Get the instance of the right type
            GridDataItem dataBoundItem = e.Item as GridDataItem;

            // If the order is archived, set a different color
            HiddenField hf = dataBoundItem.FindControl("archivedHiddenField") as HiddenField;
            if (hf != null)
                if (Convert.ToBoolean(hf.Value) == true)
                    dataBoundItem.CssClass = "disabled";
        }
    }

    // Handles switching view to place view
    protected void qtPlacedLinkButton_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["order"] != null)
        {
            PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
            int orderID = Convert.ToInt32(Request.QueryString["order"]);

            // Temporarily lock order (if necessary) while purchaser is on "place order" page.
            if (!Convert.ToBoolean(ordersAdapter.IsLocked(orderID)))
            {
                ordersAdapter.UpdateLock(orderID, true);
                temporaryLock = true;
            }

            // If order has not been approved, present "force approval" view
            PurchaseHelper.OrderStatus orderStatus = PurchaseHelper.GetOrderStatusByOrderId(orderID);
            if (orderStatus < PurchaseHelper.OrderStatus.PIApproved || orderStatus == PurchaseHelper.OrderStatus.AdminSendBack)
                adminMultiView.SetActiveView(View_ForceApproval);
            else
                adminMultiView.SetActiveView(View_OrderPlace);
        }
    }

    // Handles sending order receipt to requester
    protected void receiptSubmitButton_Click(object sender, EventArgs e)
    {
        int orderID = Convert.ToInt32(Request.QueryString["order"]);

        // Send email to requester
        EmailMessage followUp = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.FollowUp, orderID, String.Empty, String.Empty);
        followUp.To = receiptTextBox.Text.Trim(); // Re-assign recipient
        if (receiptMessageTextBox.Text.Trim().Length > 0) // Append message, if present
            followUp.Body += "<b>Message from your purchaser:</b><br />" + receiptMessageTextBox.Text.Trim();
        MailHelper.SendToPickUp(followUp);

        // Confirm with admin
        Msg(
            "Order receipt successfully sent to " + receiptTextBox.Text.Trim() + ".",
            String.Empty,
            false,
            Request.Url.ToString()
            );
    }

    // Handles stop button clicks
    protected void stopSubmitButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
            int orderID = Convert.ToInt32(Request.QueryString["order"]);

            statusAdapter.Insert(
                orderID,
                Convert.ToInt32(PurchaseHelper.OrderStatus.Stopped),
                PurchaseHelper.ProfileInfo(User.Identity.Name, true) + ": " + stopTextBox.Text.Trim(),
                DateTime.Now
                );

            // Send mail to requester
            EmailMessage requesterEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.Stopped_Requester, orderID, String.Empty, String.Empty);
            requesterEmail.Body += "<br /><br />Notes regarding the stoppage:<br /><br />" + stopTextBox.Text.Trim();
            MailHelper.SendToPickUp(requesterEmail);

            // Send mail to pi(s)
            PurchasingTableAdapters.order_approvalsTableAdapter approvalAdapter = new PurchasingTableAdapters.order_approvalsTableAdapter();
            Purchasing.order_approvalsDataTable dt = approvalAdapter.GetByOrder(orderID);
            foreach (Purchasing.order_approvalsRow dr in dt.Rows)
            {
                EmailMessage approverEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.Stopped_Approver, orderID, String.Empty, dr.pi_userid);
                approverEmail.Body += "<br /><br />Notes regarding the stoppage:<br /><br />" + stopTextBox.Text.Trim();
                MailHelper.SendToPickUp(approverEmail);
            }

            // Confirm with admin
            Msg(
                "<h1>Order Stopped!</h1>All parties have been notified.",
                String.Empty,
                false,
                "default.aspx?order=" + Request.QueryString["order"].ToString()
                );
        }
    }

    // Handles insertion of comments and attachments
    protected void commentsSubmitButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            int orderID = Convert.ToInt32(Request.QueryString["order"]);

            // Build comment, attaching file if present
            string comment = "<div class='comment'>";
            comment += "<h4>" + DateTime.Now.ToString("MM-dd-yy h:mm tt") + "</h4>";
            if (commentsRadUpload.UploadedFiles.Count > 0)
            {
                savePath = Server.MapPath(savePath);
                string fileName = StringHelper.SanitizeFileName(commentsRadUpload.UploadedFiles[0].GetName(), true);
                commentsRadUpload.UploadedFiles[0].SaveAs(savePath + fileName);
                comment += "<div class='comment_file'><a class='bdrLink' href='" + filePath + fileName + "'>Attachment</a></div>";
            }
            comment += "<p>" + PurchaseHelper.ProfileInfo(User.Identity.Name, true) + " says: " + commentsTextBox.Text.Trim() + "</p>";
            comment += "</div>";

            // Update comments
            PurchaseHelper.UpdateOrderComments(orderID, comment, false);

            // Handle e-mail notifications
            if (notifyRequesterCheckBox.Checked)
            {
                // Send mail to requester
                EmailMessage requesterEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.Comments_Requester, orderID, String.Empty, String.Empty);
                requesterEmail.Body += "<br /><br /><b>Comments:</b><br /><br />" + commentsTextBox.Text.Trim();
                if (commentsRadUpload.UploadedFiles.Count > 0)
                {
                    string fileName = StringHelper.SanitizeFileName(commentsRadUpload.UploadedFiles[0].GetName(), true);
                    requesterEmail.Body += "<br /><br /><b><a class='bdrLink' href='" + filePath + fileName + "'>View Attachment</a></b>";
                }
                MailHelper.SendToPickUp(requesterEmail);
            }

            if (notifyApproverCheckBox.Checked)
            {
                // Send mail to approver
                PurchasingTableAdapters.order_approvalsTableAdapter approvalAdapter = new PurchasingTableAdapters.order_approvalsTableAdapter();
                Purchasing.order_approvalsDataTable dt = approvalAdapter.GetByOrder(orderID);
                foreach (Purchasing.order_approvalsRow dr in dt.Rows)
                {
                    EmailMessage approverEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.Comments_Approver, orderID, String.Empty, dr.pi_userid);
                    approverEmail.Body += "<br /><br /><b>Comments:</b><br /><br />" + commentsTextBox.Text.Trim();
                    if (commentsRadUpload.UploadedFiles.Count > 0)
                    {
                        string fileName = StringHelper.SanitizeFileName(commentsRadUpload.UploadedFiles[0].GetName(), true);
                        approverEmail.Body += "<br /><br /><b><a class='bdrLink' href='" + filePath + fileName + "'>View Attachment</a></b>";
                    }
                    MailHelper.SendToPickUp(approverEmail);
                }
            }

            // Confirm with admin
            Msg(
                "<h1>Your comments have been added!</h1>",
                String.Empty,
                false,
                "default.aspx?order=" + Request.QueryString["order"].ToString()
                );
        }
    }

    // Handles send back button clicks
    protected void rejectSubmitButton_Click(object sender, EventArgs e)
    {
        PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
        int orderID = Convert.ToInt32(Request.QueryString["order"]);

        statusAdapter.Insert(
            orderID,
            Convert.ToInt32(PurchaseHelper.OrderStatus.AdminSendBack),
            PurchaseHelper.ProfileInfo(User.Identity.Name, true) + ": " + rejectTextBox.Text.Trim(),
            DateTime.Now
            );

        // Confirm with admin
        Msg(
            "<h1>Order sent back!</h1>The PI/Approvers have been notified.",
            String.Empty,
            false,
            "default.aspx?order=" + Request.QueryString["order"].ToString()
            );

        // Reset all approvals to (false)
        PurchasingTableAdapters.order_approvalsTableAdapter approvalAdapter = new PurchasingTableAdapters.order_approvalsTableAdapter();
        approvalAdapter.ResetByOrder(orderID);

        // Send mail to approver(s)
        Purchasing.order_approvalsDataTable dt = approvalAdapter.GetByOrder(orderID);
        foreach (Purchasing.order_approvalsRow dr in dt.Rows)
        {
            EmailMessage approverEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.AdminRejected_Approver, orderID, String.Empty, dr.pi_userid);
            approverEmail.Body += "<br /><br /><strong>Notes regarding this action:</string><br /><br />" + rejectTextBox.Text.Trim();
            MailHelper.SendToPickUp(approverEmail);
        }
    }

    // Handles place cancel button clicks
    protected void placeCancelButton_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["order"] != null)
        {
            int orderID = Convert.ToInt32(Request.QueryString["order"]);

            // If order was locked temporarily, unlock it
            if (temporaryLock)
            {
                PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
                ordersAdapter.UpdateLock(orderID, false);
                temporaryLock = false;
            }

            Response.Redirect("default.aspx?order=" + orderID);
        }
    }

    // Handles place button clicks
    protected void placeButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid && Request.QueryString["order"] != null)
        {
            PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
            PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
            int orderID = Convert.ToInt32(Request.QueryString["order"]);

            // Place order
            statusAdapter.Insert(
                orderID,
                Convert.ToInt32(PurchaseHelper.OrderStatus.AdminApproved),
                PurchaseHelper.ProfileInfo(User.Identity.Name, true) + ": " + messageTextBox.Text.Trim(),
                DateTime.Now
                );

            // Update order total
            ordersAdapter.UpdateTotal(orderID, Convert.ToDecimal(actualTotalRadNumericTextBox.Text));

            // Update order's DaFIS order number
            ordersAdapter.UpdateOrderNums(
                orderID,
                dafisDocTextBox.Text.Trim().Length > 0 ? dafisDocTextBox.Text.Trim() : "n/a",
                dafisPOTextBox.Text.Trim().Length > 0 ? dafisPOTextBox.Text.Trim() : "n/a",
                confirmationTextBox.Text.Trim().Length > 0 ? confirmationTextBox.Text.Trim() : "n/a"
                );

            confirmLabel.Text = "<h1>Order placed!</h1>";

            if (spanTextBox.Text.Trim() == "0" || spanTextBox.Text.Trim().ToLower() == "zero")
            {
                confirmLabel.Text += Users.GetFirstNameByUserName(ordersAdapter.GetAuthor(orderID).ToString());
                if (placeNotifyCheckBox.Checked)
                    confirmLabel.Text += " has been notified and";
                confirmLabel.Text += " will receive the follow up e-mail immediately.";

                // Send email to requester
                EmailMessage followUp = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.FollowUp, orderID, String.Empty, String.Empty);
                MailHelper.SendToPickUp(followUp);
            }
            else
            {
                confirmLabel.Text += Users.GetFirstNameByUserName(ordersAdapter.GetAuthor(orderID).ToString());
                if (placeNotifyCheckBox.Checked)
                    confirmLabel.Text += " has been notified and";
                confirmLabel.Text += " will receive the follow up e-mail in " + spanTextBox.Text + " " + spanUnitDropDownList.SelectedItem.Text + ".";

                // Prepare prompt date
                DateTime promptDT = DateTime.Now;
                if (spanUnitDropDownList.SelectedValue == "Days")
                    promptDT = promptDT.AddDays(Convert.ToDouble(spanTextBox.Text.Trim()));
                else if (spanUnitDropDownList.SelectedValue == "Weeks")
                    promptDT = promptDT.AddDays(Convert.ToDouble(spanTextBox.Text.Trim()) * 7);
                else if (spanUnitDropDownList.SelectedValue == "Months")
                    promptDT = promptDT.AddMonths(Convert.ToInt32(spanTextBox.Text.Trim()));

                // Delete existing order prompts
                PurchasingTableAdapters.order_receiptsTableAdapter receiptAdapter = new PurchasingTableAdapters.order_receiptsTableAdapter();
                receiptAdapter.DeleteByOrder(orderID);

                // Insert new order prompt
                string requesterEmail = Users.GetEmailByUserName(ordersAdapter.GetAuthor(orderID).ToString());

                receiptAdapter.Insert(
                    promptDT,
                    orderID,
                    requesterEmail,
                    ordersAdapter.GetUniqueId(orderID).ToString()
                    );
            }

            // Send email to requester
            if (placeNotifyCheckBox.Checked)
            {
                EmailMessage requesterMessage = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.AdminApproved_Requester, orderID, String.Empty, String.Empty);
                MailHelper.SendToPickUp(requesterMessage);
            }

            // Confirm with user
            Msg(
                String.Empty,
                String.Empty,
                false,
                "default.aspx?order=" + orderID.ToString()
                );
        }
    }

    // Handles force approval button clicks
    protected void forceApprovalButton_Click(object sender, EventArgs e)
    {
        if ((signedTextTextBox.Text.Trim().Length > 0 || signedDocRadUpload.UploadedFiles.Count > 0) && Request.QueryString["order"] != null)
        {
            validationLabel.Text = String.Empty;
            int orderID = Convert.ToInt32(Request.QueryString["order"]);

            // Build approval comment
            string approvalText = String.Empty;
            if (signedTextTextBox.Text.Trim().Length > 0)
                // Attach approval text
                approvalText += "<strong>Approval text:</strong> " + signedTextTextBox.Text + "<br />";
            if (signedDocRadUpload.UploadedFiles.Count > 0)
            {
                // Upload document and attach comment
                string savePath = Server.MapPath(ConfigurationManager.AppSettings["purchasingSavePath"].ToString());
                string filePath = ConfigurationManager.AppSettings["purchasingRoot"].ToString() + "admin/uploads/";
                string fileLink = String.Empty;

                foreach (UploadedFile uf in signedDocRadUpload.UploadedFiles)
                {
                    string fileName = uf.GetName();
                    fileName = StringHelper.SanitizeFileName(fileName, true);
                    uf.SaveAs(savePath + fileName);
                    fileLink += "<a class='bdrLink' href='" + filePath + fileName + "'>" + fileName + "</a>";
                    approvalText += "<strong>Approval document:</strong> " + fileLink;
                }
            }

            // Insert approval
            PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
            statusAdapter.Insert(
                orderID,
                Convert.ToInt32(PurchaseHelper.OrderStatus.PIApproved),
                "<strong>Force approval by:</strong> " + PurchaseHelper.ProfileInfo(User.Identity.Name, true),
                DateTime.Now
                );

            // Insert approval text/doc as comments
            PurchaseHelper.UpdateOrderComments(orderID, approvalText, true);

            // Redirect to place order view
            adminMultiView.SetActiveView(View_OrderPlace);
        }
        else
            validationLabel.Text = "<script type='text/javascript'> alert('You must attach at least one form of approval.'); </script>";
    }

    // Handles force approval back button clicks
    protected void forceBackButton_Click(object sender, EventArgs e)
    {
        int orderID = Convert.ToInt32(Request.QueryString["order"]);

        // If order was locked temporarily, unlock it
        if (temporaryLock)
        {
            PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
            ordersAdapter.UpdateLock(orderID, false);
            temporaryLock = false;
        }

        Response.Redirect("default.aspx?order=" + orderID);
    }

    // Handles archive selected button clicks
    protected void archiveButton_Click(object sender, EventArgs e)
    {
        // Get selected rows
        foreach (GridDataItem gdi in ordersRadGrid.SelectedItems)
        {
            if (gdi.ItemType == GridItemType.Item || gdi.ItemType == GridItemType.AlternatingItem)
            {
                PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();

                if (archiveButton.CommandName == "Archive")
                    ordersAdapter.UpdateArchived(Convert.ToInt32(gdi.GetDataKeyValue("id")), true);
                else
                    ordersAdapter.UpdateArchived(Convert.ToInt32(gdi.GetDataKeyValue("id")), false);
            }
        }

        // Rebind datagrid
        ordersRadGrid.Rebind();
    }

    // Handles updating order details
    protected void detailsSubmitButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid && Request.QueryString["order"] != null)
        {
            PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
            int orderID = Convert.ToInt32(Request.QueryString["order"]);

            // Update order total
            ordersAdapter.UpdateTotal(orderID, Convert.ToDecimal(dtlTotalRadNumericTextBox.Text));
            ordersAdapter.UpdateInvoiceTotal(orderID, Convert.ToDecimal(dtlTotalRadNumericTextBox.Text));

            // Update order's DaFIS order number
            ordersAdapter.UpdateOrderNums(
                orderID,
                dtlDocTextBox.Text.Trim().Length > 0 ? dtlDocTextBox.Text.Trim() : "n/a",
                dtlPoTextBox.Text.Trim().Length > 0 ? dtlPoTextBox.Text.Trim() : "n/a",
                dtlVendorConfirmationTextBox.Text.Trim().Length > 0 ? dtlVendorConfirmationTextBox.Text.Trim() : "n/a"
                );

            // Confirm with user
            Msg(
                "<h1>Order details updated!</h1>",
                String.Empty,
                false,
                "default.aspx?order=" + Request.QueryString["order"].ToString()
                );
        }
    }

    // Handles toggling order's locked status
    protected void qtLockLinkButton_Click(object sender, EventArgs e)
    {
        LinkButton lb = sender as LinkButton;
        if (lb != null && Request.QueryString["order"] != null)
        {
            PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
            int orderID = Convert.ToInt32(Request.QueryString["order"]);

            if (lb.CommandName == "Unlock")
            {
                ordersAdapter.UpdateLock(orderID, false);
                PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
                statusAdapter.Insert(
                    orderID,
                    Convert.ToInt32(PurchaseHelper.OrderStatus.Unlocked),
                    "Order un-locked by " + PurchaseHelper.ProfileInfo(User.Identity.Name, true),
                    DateTime.Now);
                Msg(
                    "<h1>Order successfully un-locked!</h1>",
                    String.Empty,
                    false,
                    Request.Url.ToString()
                    );
            }
            else if (lb.CommandName == "Lock")
            {
                ordersAdapter.UpdateLock(orderID, true);
                PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
                statusAdapter.Insert(
                    orderID,
                    Convert.ToInt32(PurchaseHelper.OrderStatus.Locked),
                    "Order locked by " + PurchaseHelper.ProfileInfo(User.Identity.Name, true),
                    DateTime.Now);
                Msg(
                    "<h1>Order successfully locked!</h1>",
                    String.Empty,
                    false,
                    Request.Url.ToString()
                    );
            }
        }
    }
}
