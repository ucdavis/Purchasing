using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using aspNetEmail;
using Telerik.Web.UI;

public partial class tools_purchasing_orders_myorders : System.Web.UI.Page
{
    //Page event, raised on page load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!PurchaseHelper.UserIsValid(User.Identity.Name))
            Response.Redirect(ConfigurationManager.AppSettings["purchasingRoot"].ToString());

        if (!IsPostBack)
        {
            UCDMenu.BuildBar_3(Menu3); // Build "In this section" Menu

            // Bind all orders for user
            PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
            ordersRadGrid.DataSource = ordersAdapter.GetByUser(User.Identity.Name, true);
            ordersRadGrid.DataBind();

            if (Request.QueryString["order"] != null)
            {
                if (PurchaseHelper.OrderExists(Convert.ToInt32(Request.QueryString["order"])))
                {
                    // Set view
                    ordersMultiView.SetActiveView(View_Detail);

                    int orderID = Convert.ToInt32(Request.QueryString["order"]);

                    PurchaseHelper.OrderType ot = PurchaseHelper.GetOrderType(orderID);
                    if (ot == PurchaseHelper.OrderType.DPO)
                        detailsTitleLabel.Text = "JMIE Purchase Order #";
                    else if (ot == PurchaseHelper.OrderType.DRO)
                        detailsTitleLabel.Text = "JMIE Repair Order #";
                    else if (ot == PurchaseHelper.OrderType.Agreement)
                        detailsTitleLabel.Text = "JMIE Business Agreement #";
                    detailsTitleLabel.Text += ordersAdapter.GetUniqueId(orderID).ToString();

                    PurchaseHelper.OrderStatus orderStatus = PurchaseHelper.GetOrderStatusByOrderId(orderID);
                    if ((orderStatus >= PurchaseHelper.OrderStatus.AdminApproved && orderStatus != PurchaseHelper.OrderStatus.Unlocked) || User.Identity.Name != ordersAdapter.GetAuthor(orderID).ToString())
                        // The order has already been placed (or) user is not the author... hide the edit button
                        editButton.Visible = false;
                    if (orderStatus >= PurchaseHelper.OrderStatus.ShippingVendorConflict && orderStatus != PurchaseHelper.OrderStatus.Unlocked)
                        // A stop has already been requested or initiated, hide the stop button
                        stopButton.Visible = false;
                }
                else
                {
                    ordersMultiView.SetActiveView(View_Confirm);
                    confirmTitleLabel.Text = "Oops!";
                    confirmLabel.Text = "This order no longer exists.";
                }
            }
            else
                ordersMultiView.SetActiveView(View_Orders);
        }
    }

    /// <summary>
    /// Returns order page based on order type
    /// </summary>
    /// <param name="orderID"></param>
    /// <returns></returns>
    private string GetOrderPage(object orderType)
    {
        PurchaseHelper.OrderType ot = (PurchaseHelper.OrderType)Convert.ToInt32(orderType);

        switch (ot)
        {
            case PurchaseHelper.OrderType.DPO:
                return "dpo.aspx";
            case PurchaseHelper.OrderType.Agreement:
                return "ba.aspx";
            case PurchaseHelper.OrderType.DRO:
                return "dro.aspx";
            default: break;
        }

        return String.Empty;
    }

    /// <summary>
    /// Returns list of items
    /// </summary>
    /// <param name="orderID"></param>
    /// <param name="orderType"></param>
    /// <returns></returns>
    protected string GetItems(object orderID, object orderType)
    {
        PurchaseHelper.OrderType ot = (PurchaseHelper.OrderType)Convert.ToInt32(orderType);
        int id = Convert.ToInt32(orderID);
        string itemsList = "<ul class=\"itemsList\">";
        string listFormat = "<li>{0}</li>";

        switch (ot)
        {
            case PurchaseHelper.OrderType.DPO:

                PurchasingTableAdapters.dpo_itemsTableAdapter dpoAdapter = new PurchasingTableAdapters.dpo_itemsTableAdapter();
                Purchasing.dpo_itemsDataTable dpoDt = dpoAdapter.GetByOrderId(id);
                foreach (Purchasing.dpo_itemsRow dpoDr in dpoDt)
                    itemsList += String.Format(listFormat, dpoDr.description.Length > 20 ? dpoDr.description.Substring(0, 17) + "..." : dpoDr.description);
                break;
            case PurchaseHelper.OrderType.Agreement:
                PurchasingTableAdapters.ba_itemsTableAdapter baAdapter = new PurchasingTableAdapters.ba_itemsTableAdapter();
                Purchasing.ba_itemsDataTable baDt = baAdapter.GetByOrderId(id);
                foreach (Purchasing.ba_itemsRow baDr in baDt)
                    itemsList += String.Format(listFormat, baDr.description.Length > 20 ? baDr.description.Substring(0, 17) + "..." : baDr.description);
                break;
            case PurchaseHelper.OrderType.DRO:
                PurchasingTableAdapters.dro_itemsTableAdapter droAdapter = new PurchasingTableAdapters.dro_itemsTableAdapter();
                Purchasing.dro_itemsDataTable droDt = droAdapter.GetByOrderId(id);
                foreach (Purchasing.dro_itemsRow droDr in droDt)
                    itemsList += String.Format(listFormat, droDr.item_desc.Length > 20 ? droDr.item_desc.Substring(0, 17) + "..." : droDr.item_desc);
                break;
            default: break;
        }

        itemsList += "</ul>";
        return itemsList;
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
    /// Route user to specific view based on order's approval and user's role
    /// </summary>
    /// <param name="orderID"></param>
    /// <returns></returns>
    protected string ParseViewLink(object orderID, object orderType)
    {
        PurchasingTableAdapters.order_approvalsTableAdapter approvalAdapter = new PurchasingTableAdapters.order_approvalsTableAdapter();
        PurchasingTableAdapters.ordersTableAdapter orderAdapter = new PurchasingTableAdapters.ordersTableAdapter();

        int id = Convert.ToInt32(orderID);
        PurchaseHelper.OrderStatus status = PurchaseHelper.GetOrderStatusByOrderId(id);

        if (User.IsInRole("PurchaseAdmin") && orderAdapter.GetAuthor(id).ToString() != User.Identity.Name)
            return "../admin/default.aspx?order=" + id.ToString();
        else if (Convert.ToInt32(approvalAdapter.IsUserApprover(id, User.Identity.Name)) > 0 && status < PurchaseHelper.OrderStatus.PIApproved)
            return GetOrderPage(orderType) + "?order=" + id.ToString() + "&mode=pi";
        else
            return "myorders.aspx?order=" + id.ToString();
    }

    /// <summary>
    /// Returns trimmed vendor name
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected string TrimVendorName(object sender)
    {
        string vendorName = sender.ToString();
        return vendorName.Length > 25 ? "<span title='" + vendorName + "'>" + vendorName.Substring(0, 24).Trim() + "...</span>" : vendorName;
    }

    /// <summary>
    /// Event, raised on export radcombobox index change
    /// </summary>
    /// <param name="o"></param>
    /// <param name="e"></param>
    protected void exportRadComboBox_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        ordersRadGrid.ExportSettings.FileName = Users.GetFullNameByUserName(User.Identity.Name, true, true) + "_Orders_" + DateTime.Now.ToString("M_d_yy");
        ordersRadGrid.ExportSettings.IgnorePaging = true;
        ordersRadGrid.ExportSettings.ExportOnlyData = true;
        ordersRadGrid.ExportSettings.ExportOnlyData = true;

        switch (exportRadComboBox.SelectedValue)
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

    /// <summary>
    /// Re-binds orders radgrid on sort
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ordersRadGrid_NeedDataSource(object sender, EventArgs e)
    {
        PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
        ordersRadGrid.DataSource = ordersAdapter.GetByUser(User.Identity.Name, true);
    }

    // Handles redirect to edit page
    protected void editButton_Click(object sender, EventArgs e)
    {
        int orderID = Convert.ToInt32(Request.QueryString["order"]);
        int orderType = Convert.ToInt32(PurchaseHelper.GetOrderType(orderID));
        Response.Redirect(GetOrderPage(orderType) + "?order=" + orderID.ToString() + "&mode=edit");
    }

    // Handles redirect to order page
    protected void duplicateButton_Click(object sender, EventArgs e)
    {
        int orderID = Convert.ToInt32(Request.QueryString["order"]);
        int orderType = Convert.ToInt32(PurchaseHelper.GetOrderType(orderID));
        Response.Redirect(GetOrderPage(orderType) + "?order=" + orderID.ToString());
    }

    // Handles stop order button clicks
    protected void stopSubmitButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid && stopTextBox.Text.Trim().Length > 0)
        {
            int orderID = Convert.ToInt32(Request.QueryString["order"]);

            // Update order status to "stop requested"
            PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
            statusAdapter.Insert(
                orderID,
                Convert.ToInt32(PurchaseHelper.OrderStatus.StopRequested),
                Users.GetFullNameByUserName(User.Identity.Name, false, false) + ": " + stopTextBox.Text.Trim(),
                DateTime.Now
                );

            // Confirm with user
            confirmTitleLabel.Text = "Stop Requested!";
            confirmLabel.Text = "A stop was requested for this order.<br /><br />You will be notified when your purchase admin has stopped the order completely.";
            ordersMultiView.SetActiveView(View_Confirm);
            Response.AppendHeader("refresh", "4;url=myorders.aspx");

            // Send admin mail
            EmailMessage adminEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.StopRequested_Admin, orderID, String.Empty, String.Empty);
            MailHelper.SendToPickUp(adminEmail);
        }
    }
}
