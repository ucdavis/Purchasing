using System;
using System.Configuration;
using System.Drawing;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using aspNetEmail;
using Telerik.Web.UI;

public partial class business_purchasing_blocks_order_details : System.Web.UI.UserControl
{
    // Paths
    protected string savePath = ConfigurationManager.AppSettings["purchasingSavePath"].ToString();
    protected string filePath = ConfigurationManager.AppSettings["purchasingRoot"].ToString() + "admin/uploads/";

    // Event raised on user control load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["order"] != null)
            {
                int orderID = Convert.ToInt32(Request.QueryString["order"]);
                PurchaseHelper.OrderStatus orderStatus = PurchaseHelper.GetOrderStatusByOrderId(orderID);

                // Determine order type
                PurchaseHelper.OrderType ot = PurchaseHelper.GetOrderType(orderID);
                if (ot == PurchaseHelper.OrderType.DPO)
                {
                    shiptoLabel.Text = "Deliver To";
                    itemsMultiView.SetActiveView(View_Items_DPO);
                }
                else if (ot == PurchaseHelper.OrderType.DRO)
                {
                    shiptoLabel.Text = "Repair Site";
                    itemsMultiView.SetActiveView(View_Items_DRO);
                }
                else
                {
                    agreementPanel.Visible = true;
                    shiptoLabel.Text = "Service/Delivery Site";
                    itemsMultiView.SetActiveView(View_Items_Agreement);
                }

                // Show the receipt table if the order has been placed
                if (
                    (orderStatus == PurchaseHelper.OrderStatus.AdminApproved ||
                    orderStatus == PurchaseHelper.OrderStatus.NotReceived ||
                    orderStatus == PurchaseHelper.OrderStatus.ShippingVendorConflict) &&
                    !Request.Url.ToString().Contains("admin/default.aspx"))
                    receiptMultiView.SetActiveView(View_Receipt);
                else
                    receiptMultiView.SetActiveView(View_Receipt_Blank);

                BuildDetailsView(orderID);
                BindItems(orderID);
                BindStatus(orderID);

                if (HttpContext.Current.User.IsInRole("PurchaseAdmin"))
                    addNotesHyperLink.Visible = false;
                else if (HttpContext.Current.User.IsInRole("PurchaseUser"))
                {
                    requesterCheckBox.Visible = false;
                    approverCheckBox.Checked = true;
                }
                else if (HttpContext.Current.User.IsInRole("PurchaseApprover"))
                {
                    approverCheckBox.Visible = false;
                    requesterCheckBox.Checked = true;
                }
                else if (HttpContext.Current.User.IsInRole("PurchaseManager") || HttpContext.Current.User.IsInRole("MasterAdmin"))
                {
                    approverCheckBox.Checked = true;
                    requesterCheckBox.Checked = true;
                }
            }
        }
    }

    /// <summary>
    /// Subroutine, populates list with order status
    /// </summary>
    /// <param name="orderID"></param>
    protected void BindStatus(int orderID)
    {
        PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
        statusRepeater.DataSource = statusAdapter.GetByOrder(orderID);
        statusRepeater.DataBind();
    }

    /// <summary>
    /// Subroutine, populates controls with order details
    /// </summary>
    /// <param name="orderID"></param>
    protected void BuildDetailsView(int orderID)
    {
        PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
        Purchasing.ordersDataTable dt = ordersAdapter.GetById(orderID);
        foreach (Purchasing.ordersRow dr in dt.Rows)
        {
            // Order details
            if (!dr.Isadmin_useridNull())
                dtlPurchaser.Text = "<br/>-&nbsp;Purchase Admin: " + PurchaseHelper.ProfileInfo(dr.admin_userid, true);
            if (!dr.Isdafis_docNull())
                dtlDafisDocLabel.Text = "<br />-&nbsp;DaFIS Doc Number: " + dr.dafis_doc;
            if (!dr.Isdafis_poNull())
                dtlDafisPOLabel.Text = "<br />-&nbsp;DaFIS PO Number: " + dr.dafis_po;
            if (!dr.Isconfirmation_numNull())
                dtlConfirmLabel.Text = "<br />-&nbsp;Vendor Confirmation Number: " + dr.confirmation_num;
            if (!dr.Isinvoice_totalNull())
                dtlInvoiceLabel.Text = "<br />-&nbsp;Final Invoice Total: $" + dr.invoice_total.ToString();

            if (!dr.IsaccountNull())
                dtlAccountLabel.Text = dr.account;
            else
                dtlAccountLabel.Text = "None specified";

            PurchasingTableAdapters.order_approvalsTableAdapter approvalAdapter = new PurchasingTableAdapters.order_approvalsTableAdapter();
            piRepeater.DataSource = approvalAdapter.GetByOrder(orderID);
            piRepeater.DataBind();

            dtlShippingLabel.Text = dr.shipping + " Shipping";
            dtlNeededLabel.Text = "Items needed by: ";
            if (dr.date_needed.Length > 0)
                dtlNeededLabel.Text += dr.date_needed;
            else
                dtlNeededLabel.Text += "N/A";
            dtlBackorderLabel.Text = dr.backorder_ok ? "OK to backorder" : "Do not backorder";

            // Agreement
            if (!dr.Isba_typeNull())
                baTypeLabel.Text = "Agreement Type: " + dr.ba_type;
            if (!dr.Isba_contactNull())
                baAgreementContactLabel.Text = "Agreement Contact: " + dr.ba_contact;
            if (!dr.Isba_contact_phoneNull())
                baAgreementContactLabel.Text += " " + StringHelper.FormatPhone(dr.ba_contact_phone, false);
            if (!dr.Isba_ucd_contactNull())
                baUCDContactLabel.Text = "UCD Contact: " + dr.ba_ucd_contact;
            if (!dr.Isba_ucd_contact_phoneNull())
                baUCDContactLabel.Text += " " + StringHelper.FormatPhone(dr.ba_ucd_contact_phone, false);
            if (!dr.Isba_fromNull() && !dr.Isba_toNull())
                baDatesLabel.Text = "Effective from " + dr.ba_from + " to " + dr.ba_to;

            // Order notes
            if (!dr.Isadmin_commentsNull())
                notesLabel.Text = dr.admin_comments;
            else if (HttpContext.Current.User.IsInRole("PurchaseAdmin"))
                notesLabel.Text = "Click \"Manage\" -> \"Attach comment or file\" to add comments.";
            else
                notesLabel.Text = "Click 'Add Notes' to append notes and/or attachments to this order.";

            if (!HttpContext.Current.User.IsInRole("PurchaseAdmin") && !HttpContext.Current.User.IsInRole("MasterAdmin"))
                // Hide the vendor notes pop-up
                vendorImageButton.Visible = false;

            // Vendor
            PurchasingTableAdapters.vendorsTableAdapter vendorAdapter = new PurchasingTableAdapters.vendorsTableAdapter();
            Purchasing.vendorsDataTable vendorTable = vendorAdapter.GetById(dr.vendor_id);
            foreach (Purchasing.vendorsRow vendorRow in vendorTable.Rows)
            {
                dtlVendorLabel.Text = vendorRow.vendor_name;
                vendorAddress.Text = vendorRow.vendor_address + ", " + vendorRow.vendor_city + ", " + vendorRow.vendor_state;
                vendorPhone.Text = StringHelper.FormatPhone(vendorRow.vendor_phone, true);
                vendorFAX.Text = StringHelper.FormatPhone(vendorRow.vendor_fax, true);

                if (!vendorRow.Isvendor_urlNull())
                    vendorHyperLink.Text = vendorHyperLink.NavigateUrl = vendorRow.vendor_url;
                else
                    vendorHyperLink.Visible = false;

                if (!vendorRow.Isvendor_notesNull())
                    vendorNotesTextBox.Text = vendorRow.vendor_notes;

                vendorSubmitButton.CommandArgument = vendorRow.id.ToString();
            }

            // Populate shipto address
            shiptoMultiView.SetActiveView(dr.shipto_onsite ? View_OnSite : View_OffSite);

            PurchasingTableAdapters.shipto_addressesTableAdapter shipToAdapter = new PurchasingTableAdapters.shipto_addressesTableAdapter();
            Purchasing.shipto_addressesDataTable shipToTable = shipToAdapter.GetById(dr.shipto_id);
            foreach (Purchasing.shipto_addressesRow shipToRow in shipToTable.Rows)
            {
                dtlShiptoNameLabel.Text = dr.shipto_name;
                dtlShiptoBuildingLabel.Text = shipToRow.building + " ";
                dtlShiptoRoomLabel.Text = shipToRow.room + "<br />";
                if (!shipToRow.IsaddressNull())
                    dtlShiptoAddressLabel.Text = shipToRow.address + "<br />";
                if (!shipToRow.IscampusNull())
                    dtlShiptoCampusLabel.Text = shipToRow.campus + "<br />";
                dtlShiptoCityLabel.Text = shipToRow.city;
                dtlShiptoStateLabel.Text = shipToRow.state;
                dtlShiptoStreetLabel.Text = shipToRow.street;
                dtlShiptoZipLabel.Text = shipToRow.zip.Substring(0, 5) + "-";
                dtlShiptoZipLabel.Text += shipToRow.zip.Substring(5, 4);
            }

            // Overide shipto if order is no-cost agreement
            if (!dr.Isba_typeNull())
                if (dr.ba_type == "No-cost Agreement")
                {
                    itemsMultiView.SetActiveView(View_Items_Blank);
                    shiptoMultiView.SetActiveView(View_NA);
                }

            // Contact Phone
            PurchasingTableAdapters.user_profilesTableAdapter profileAdapter = new PurchasingTableAdapters.user_profilesTableAdapter();
            Purchasing.user_profilesDataTable profileTable = profileAdapter.GetByUser(dr.userid);
            foreach (Purchasing.user_profilesRow profileRow in profileTable.Rows)
            {
                if (!profileRow.IsphoneNull())
                {
                    dtlShiptoNameLabel.Text += " - " + StringHelper.FormatPhone(profileRow.phone, false) + "<br />";
                }
            }
        }
    }

    /// <summary>
    /// Subroutine, binds list of order items
    /// </summary>
    /// <param name="orderID"></param>
    protected void BindItems(int orderID)
    {
        PurchaseHelper.OrderType ot = PurchaseHelper.GetOrderType(orderID);
        if (ot == PurchaseHelper.OrderType.DPO)
        {
            PurchasingTableAdapters.dpo_itemsTableAdapter dpoItemsAdapter = new PurchasingTableAdapters.dpo_itemsTableAdapter();
            dpoItemsRepeater.DataSource = dpoItemsAdapter.GetByOrderId(orderID);
            dpoItemsRepeater.DataBind();
        }
        else if (ot == PurchaseHelper.OrderType.DRO)
        {
            PurchasingTableAdapters.dro_itemsTableAdapter droItemsAdapter = new PurchasingTableAdapters.dro_itemsTableAdapter();
            droItemsRepeater.DataSource = droItemsAdapter.GetByOrderId(orderID);
            droItemsRepeater.DataBind();
        }
        else
        {
            PurchasingTableAdapters.ba_itemsTableAdapter baItemsAdapter = new PurchasingTableAdapters.ba_itemsTableAdapter();
            baItemsRepeater.DataSource = baItemsAdapter.GetByOrderId(orderID);
            baItemsRepeater.DataBind();
        }
    }

    /// <summary>
    /// Converts string to currency
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected string ConvertToCurrency(object sender)
    {
        double price = Convert.ToDouble(sender);
        return price.ToString("c");
    }

    /// <summary>
    /// Subroutine, returns order subtotal
    /// </summary>
    /// <returns></returns>
    protected string OrderSubTotal()
    {
        PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
        int orderID = Convert.ToInt32(Request.QueryString["order"]);
        return Convert.ToDecimal(ordersAdapter.GetOrderTotal(orderID)).ToString("c");
    }

    /// <summary>
    /// Subroutine, returns actual total or est total
    /// </summary>
    /// <returns></returns>
    protected string ActualTotal()
    {
        int orderID = Convert.ToInt32(Request.QueryString["order"]);
        PurchaseHelper.OrderStatus orderStatus = PurchaseHelper.GetOrderStatusByOrderId(orderID);
        PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
        double total = Convert.ToDouble(ordersAdapter.GetOrderTotal(orderID));

        if (orderStatus > PurchaseHelper.OrderStatus.AdminSendBack)
            return total.ToString("c");
        else
        {
            string taxRate = "8.25";
            if (ConfigurationManager.AppSettings["purchaseTax"] != null)
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["purchaseTax"].ToString()))
                    taxRate = ConfigurationManager.AppSettings["purchaseTax"].ToString();
            double estTotal = total + (total * (Convert.ToDouble(taxRate) / 100));
            return estTotal.ToString("c");
        }
    }

    /// <summary>
    /// Subroutine, returns appropriate total text
    /// </summary>
    /// <returns></returns>
    protected string ActualTotalText()
    {
        int orderID = Convert.ToInt32(Request.QueryString["order"]);
        PurchaseHelper.OrderStatus orderStatus = PurchaseHelper.GetOrderStatusByOrderId(orderID);

        string taxRate = "8.25";
        if (ConfigurationManager.AppSettings["purchaseTax"] != null)
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["purchaseTax"].ToString()))
                taxRate = ConfigurationManager.AppSettings["purchaseTax"].ToString();

        if (orderStatus > PurchaseHelper.OrderStatus.AdminSendBack)
            return "Actual Total (including shipping):";
        else
            return "Estimated Total (including " + taxRate + "% tax but not including shipping fees):";
    }

    /// <summary>
    /// Subroutine, returns shortened date
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected string ShortenDate(object sender)
    {
        return Convert.ToDateTime(sender).ToString("M-d-yy h:mm tt");
    }

    /// <summary>
    /// Appends "Notes:" if notes are present
    /// </summary>
    /// <param name="notes"></param>
    /// <returns></returns>
    protected string ParseNotes(object notes)
    {
        string notesStr = notes.ToString();
        if (notesStr.Trim().Length > 0)
            return "<strong>Notes:</strong>&nbsp;" + notesStr.Trim();
        else
            return String.Empty;
    }

    // Event raised on items repeater item created
    protected void ItemsRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        int orderID = Convert.ToInt32(Request.QueryString["order"]);
        PurchaseHelper.OrderStatus orderStatus = PurchaseHelper.GetOrderStatusByOrderId(orderID);

        // Hide subtotal if order has been placed
        if (e.Item.ItemType == ListItemType.Footer)
        {
            HtmlTableRow tr = e.Item.FindControl("subtotalRow") as HtmlTableRow;
            tr.Style["display"] = orderStatus > PurchaseHelper.OrderStatus.AdminSendBack ? "none" : String.Empty;
        }
    }

    // Event, raised on status repeater item databound
    protected void statusRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();

            HiddenField hf = e.Item.FindControl("statusHiddenField") as HiddenField;
            System.Web.UI.WebControls.Image img = e.Item.FindControl("statusImage") as System.Web.UI.WebControls.Image;
            Label lbl = e.Item.FindControl("statusMsgLabel") as Label;
            HtmlTableCell td = e.Item.FindControl("statusCell") as HtmlTableCell;

            int orderID = Convert.ToInt32(Request.QueryString["order"]);
            int status = Convert.ToInt32(hf.Value);

            string msg = String.Empty;
            string msgColor = "#4C4C4C";
            string imageName = String.Empty;
            string bg = "#FFFFFF";

            PurchaseHelper.OrderStatus orderStatus = (PurchaseHelper.OrderStatus)status;

            switch (orderStatus)
            {
                case PurchaseHelper.OrderStatus.New:
                    msg = "Order submitted by " + PurchaseHelper.ProfileInfo(ordersAdapter.GetAuthor(orderID).ToString(), true) + "."; ;
                    imageName = "document.gif";
                    break;
                case PurchaseHelper.OrderStatus.Changed:
                    msg = "Modified by " + PurchaseHelper.ProfileInfo(ordersAdapter.GetAuthor(orderID).ToString(), true) + ".";
                    imageName = "warning.gif";
                    break;
                case PurchaseHelper.OrderStatus.PISendBack:
                    msg = "Order not approved";
                    msgColor = "#FF0000";
                    imageName = "thumbs_down.gif";
                    break;
                case PurchaseHelper.OrderStatus.PIApproving:
                    msg = "Order approved";
                    imageName = "thumbs_up.gif";
                    break;
                case PurchaseHelper.OrderStatus.PIApproved:
                    msg = "All approvals collected";
                    imageName = "accomp_yes.gif";
                    break;
                case PurchaseHelper.OrderStatus.AdminSendBack:
                    msg = "Order rejected";
                    msgColor = "#FF0000";
                    imageName = "accomp_no.gif";
                    break;
                case PurchaseHelper.OrderStatus.AdminApproved:
                    msg = "Order placed";
                    imageName = "check.gif";
                    break;
                case PurchaseHelper.OrderStatus.ShippingVendorConflict:
                    msg = "Order conflict";
                    msgColor = "#FF0000";
                    imageName = "box_returned.gif";
                    break;
                case PurchaseHelper.OrderStatus.Delivered:
                    msg = "Order received";
                    imageName = "box_delivered.gif";
                    break;
                case PurchaseHelper.OrderStatus.StopRequested:
                    msg = "Stop requested";
                    imageName = "accomp_no.gif";
                    break;
                case PurchaseHelper.OrderStatus.Stopped:
                    msg = "Order stopped";
                    imageName = "delete.gif";
                    break;
                case PurchaseHelper.OrderStatus.NotReceived:
                    msg = "Order not received";
                    imageName = "box_pending.gif";
                    break;
                case PurchaseHelper.OrderStatus.Locked:
                    msg = "Order locked";
                    imageName = "lock2.gif";
                    break;
                case PurchaseHelper.OrderStatus.Unlocked:
                    msg = "Order un-locked";
                    imageName = "lock_open.gif";
                    break;
                default: break;
            }

            lbl.Text = msg;
            lbl.ForeColor = Color.FromName(msgColor);
            img.ToolTip = msg.Replace("<span title='", String.Empty).Replace("'>", " ").Replace("</span>", String.Empty);
            img.ImageUrl = ResolveClientUrl("~/images/15/" + imageName);
            td.Style["background-color"] = bg;
        }
    }

    // Handles receieved receipts
    protected void receivedImageButton_Click(object sender, EventArgs e)
    {
        int orderID = Convert.ToInt32(Request.QueryString["order"]);

        // Update order status to "delevered"
        PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
        statusAdapter.Insert(
            orderID,
            Convert.ToInt32(PurchaseHelper.OrderStatus.Delivered),
            "Marked as received by " + PurchaseHelper.ProfileInfo(HttpContext.Current.User.Identity.Name, true),
            DateTime.Now
            );

        // Clear pending receipts
        PurchasingTableAdapters.order_receiptsTableAdapter receiptAdapter = new PurchasingTableAdapters.order_receiptsTableAdapter();
        receiptAdapter.DeleteByOrder(orderID);

        // Send requester mail
        EmailMessage requesterEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.Delevered_Requester, orderID, String.Empty, String.Empty);
        MailHelper.SendToPickUp(requesterEmail);

        // Send admin mail
        EmailMessage adminEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.Delevered_Admin, orderID, String.Empty, String.Empty);
        MailHelper.SendToPickUp(adminEmail);

        receiptMultiView.SetActiveView(View_Receipt_Confirm);
        confirmReceiptLabel.Text = "Order marked as received. No further action is required for this order.";
        BindStatus(orderID);
    }

    // Handles notreceived button clicks
    protected void notreceivedSubmitButton_Click(object sender, EventArgs e)
    {
        int orderID = Convert.ToInt32(Request.QueryString["order"]);

        // Update order status to "not received"
        PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
        statusAdapter.Insert(
            orderID,
            Convert.ToInt32(PurchaseHelper.OrderStatus.NotReceived),
            notreceivedTextBox.Text.Trim().Length > 0 ? PurchaseHelper.ProfileInfo(HttpContext.Current.User.Identity.Name, true) + ": " + notreceivedTextBox.Text.Trim() : "Marked as not received by " + PurchaseHelper.ProfileInfo(HttpContext.Current.User.Identity.Name, true),
            DateTime.Now
            );

        // Send admin mail
        EmailMessage adminEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.NotReceived_Admin, orderID, String.Empty, String.Empty);
        MailHelper.SendToPickUp(adminEmail);

        receiptMultiView.SetActiveView(View_Receipt_Confirm);
        confirmReceiptLabel.Text = "Order marked as not received. If/when you receive this order, please return to this page and complete the receipt section again.";
        BindStatus(orderID);
    }

    // Handles conflict button clicks
    protected void conflictSubmitButton_Click(object sender, EventArgs e)
    {
        int orderID = Convert.ToInt32(Request.QueryString["order"]);

        // Update order status to "shipping/vendor conflict"
        PurchasingTableAdapters.order_status_relTableAdapter statusAdapter = new PurchasingTableAdapters.order_status_relTableAdapter();
        statusAdapter.Insert(
            orderID,
            Convert.ToInt32(PurchaseHelper.OrderStatus.ShippingVendorConflict),
            conflictTextBox.Text.Trim().Length > 0 ? PurchaseHelper.ProfileInfo(HttpContext.Current.User.Identity.Name, true) + ": " + conflictTextBox.Text.Trim() : "Marked as conflict by " + PurchaseHelper.ProfileInfo(HttpContext.Current.User.Identity.Name, true),
            DateTime.Now
            );

        // Clear pending receipts
        PurchasingTableAdapters.order_receiptsTableAdapter receiptAdapter = new PurchasingTableAdapters.order_receiptsTableAdapter();
        receiptAdapter.DeleteByOrder(orderID);

        // Send requester mail
        EmailMessage requesterMessage = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.Conflict_Requester, orderID, String.Empty, String.Empty);
        MailHelper.SendToPickUp(requesterMessage);

        // Send admin mail
        EmailMessage adminMessage = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.Conflict_Admin, orderID, String.Empty, String.Empty);
        MailHelper.SendToPickUp(adminMessage);

        receiptMultiView.SetActiveView(View_Receipt_Confirm);
        confirmReceiptLabel.Text = "Your purchaser has been notified that there is a problem with your order. If/when the problem is resolved, please return to this page and complete the receipt section again.";
        BindStatus(orderID);
    }

    // Handles add notes button clicks
    protected void notesSubmitButton_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["order"] != null)
        {
            int orderID = Convert.ToInt32(Request.QueryString["order"]);

            // Upload files
            savePath = Server.MapPath(savePath);
            string fileLinks = String.Empty;
            int count = 1;
            foreach (UploadedFile uf in dpoRadUpload.UploadedFiles)
            {
                string fileName = uf.GetName();
                fileName = StringHelper.SanitizeFileName(fileName, true);
                uf.SaveAs(savePath + fileName);
                fileLinks += "<a class='bdrLink' href='" + filePath + fileName + "'>File Attachment #" + count.ToString() + "</a><br />";
                count++;
            }

            string currentUser = PurchaseHelper.ProfileInfo(HttpContext.Current.User.Identity.Name, true);

            PurchaseHelper.UpdateOrderComments(
                orderID,
                currentUser + " says: " + notesTextBox.Text.Trim() + "<br />" + fileLinks,
                true
                );

            //------------ Send e-mails ------------//


            if (purchaserCheckBox.Checked)
            {
                // Send admin mail (notes updated)
                EmailMessage adminEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.Comments_Admin, orderID, String.Empty, String.Empty);
                adminEmail.Body += "<br /><br />NOTES SUPPLIED BY " + currentUser + ":<br />" + notesTextBox.Text.Trim() + "<br />" + fileLinks;
                MailHelper.SendToPickUp(adminEmail);
            }

            if (requesterCheckBox.Checked)
            {
                // Send requester mail (notes updated)
                EmailMessage requesterEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.Comments_Requester, orderID, String.Empty, String.Empty);
                requesterEmail.Body += "<br /><br />NOTES SUPPLIED BY " + currentUser + ":<br />" + notesTextBox.Text.Trim() + "<br />" + fileLinks;
                MailHelper.SendToPickUp(requesterEmail);
            }

            if (approverCheckBox.Checked)
            {
                // Send approvers mail (notes updated)
                PurchasingTableAdapters.order_approvalsTableAdapter approvalAdapter = new PurchasingTableAdapters.order_approvalsTableAdapter();
                Purchasing.order_approvalsDataTable dt = approvalAdapter.GetByOrder(orderID);
                foreach (Purchasing.order_approvalsRow dr in dt.Rows)
                {
                    EmailMessage approverEmail = PurchaseHelper.MergeMessage(PurchaseHelper.PurchaseMessage.Comments_Approver, orderID, String.Empty, dr.pi_userid);
                    approverEmail.Body += "<br /><br />NOTES SUPPLIED BY " + currentUser + ":<br />" + notesTextBox.Text.Trim() + "<br />" + fileLinks;
                    MailHelper.SendToPickUp(approverEmail);
                }
            }

            Response.Redirect(Request.Url.ToString());
        }
    }

    // Handles vendor notes submit button clicks
    protected void vendorSubmitButton_Click(object sender, EventArgs e)
    {
        Button btn = sender as Button;
        PurchasingTableAdapters.vendorsTableAdapter vendorAdapter = new PurchasingTableAdapters.vendorsTableAdapter();
        vendorAdapter.UpdateNotes(
            Convert.ToInt32(btn.CommandArgument),
            vendorNotesTextBox.Text.Trim()
            );
        Response.Redirect(Request.Url.ToString());
    }
}
