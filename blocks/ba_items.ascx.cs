using System;
using System.Configuration;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class business_purchasing_blocks_ba_items : System.Web.UI.UserControl
{
    // Page event, raised on page load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            HandleIE();

            // If this is a new order, set the inital row count to 5
            if (Request.QueryString.Count == 0)
                InitialRows(5);

            if (ConfigurationManager.AppSettings["purchaseTax"] != null)
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["purchaseTax"].ToString()))
                {
                    taxLabel.Text = ConfigurationManager.AppSettings["purchaseTax"].ToString();
                    taxRateHiddenField.Value = ConfigurationManager.AppSettings["purchaseTax"].ToString();
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
        {
            foreach (System.Web.UI.Control c in baItemsPanel.Controls)
            {
                foreach (System.Web.UI.Control c2 in c.Controls)
                {
                    foreach (System.Web.UI.Control c3 in c2.Controls)
                    {
                        if (c3.GetType().ToString().EndsWith(".RadNumericTextBox"))
                        {
                            RadNumericTextBox rntb = c3 as RadNumericTextBox;
                            rntb.EnableEmbeddedSkins = false;
                            rntb.Width = Unit.Percentage(80);
                            rntb.Style.Add("border", "solid 1px #888");
                            rntb.Style.Add("width", "80% !important");
                        }
                        if (c3.GetType().ToString().EndsWith(".TextBox"))
                        {
                            TextBox tb = c3 as TextBox;
                            tb.Style.Add("border", "solid 1px #888");
                            tb.Style.Add("width", "90% !important");
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Subroutine, sets the number of initially visible BA rows
    /// </summary>
    /// <param name="count"></param>
    protected void InitialRows(int count)
    {
        for (int r = 0; r <= 14; r++)
        {
            HtmlTableRow tr = baItemsPanel.FindControl("Tr" + r.ToString()) as HtmlTableRow;
            if (r < (count))
                tr.Style["display"] = String.Empty;
            else
                tr.Style["display"] = "none";
        }
    }

    /// <summary>
    /// Subroutine, populates items table with items
    /// </summary>
    /// <param name="orderID"></param>
    public void PopulateItems(int orderID)
    {
        PurchasingTableAdapters.ba_itemsTableAdapter baItemsAdapter = new PurchasingTableAdapters.ba_itemsTableAdapter();
        Purchasing.ba_itemsDataTable dt = baItemsAdapter.GetByOrderId(orderID);

        int r = 0;
        double rowsSubTotal = 0;

        foreach (Purchasing.ba_itemsRow dr in dt.Rows)
        {
            HtmlTableRow tr = baItemsPanel.FindControl("Tr" + r.ToString()) as HtmlTableRow;
            HiddenField hf = baItemsPanel.FindControl("rowHiddenField" + r.ToString()) as HiddenField;
            RadNumericTextBox qty = baItemsPanel.FindControl("qtyRadNumericTextBox" + r.ToString()) as RadNumericTextBox;
            TextBox unit = baItemsPanel.FindControl("unitTextBox" + r.ToString()) as TextBox;
            TextBox catalogNum = baItemsPanel.FindControl("serviceTextBox" + r.ToString()) as TextBox;
            TextBox desc = baItemsPanel.FindControl("descTextBox" + r.ToString()) as TextBox;
            RadNumericTextBox unitPrice = baItemsPanel.FindControl("costRadNumericTextBox" + r.ToString()) as RadNumericTextBox;

            tr.Style["display"] = String.Empty;
            qty.Text = dr.qty.ToString();
            unit.Text = dr.unit;
            catalogNum.Text = dr.service_item;
            desc.Text = dr.description;
            unitPrice.Text = dr.unit_price.ToString();

            double rowTotal = (dr.qty * Convert.ToDouble(dr.unit_price));
            hf.Value = rowTotal.ToString();
            rowsSubTotal += rowTotal;
            r++;
        }

        subTotalHiddenField.Value = rowsSubTotal.ToString();
        if (r > 0)
            InitialRows(r);
        else
            InitialRows(5);
    }

    /// <summary>
    /// Gets order subtotal
    /// </summary>
    /// <returns></returns>
    public Double GetOrderSubTotal()
    {
        double subTotal = 0;
        for (int r = 0; r <= 14; r++)
        {
            RadNumericTextBox qty = baItemsPanel.FindControl("qtyRadNumericTextBox" + r.ToString()) as RadNumericTextBox;
            TextBox unit = baItemsPanel.FindControl("unitTextBox" + r.ToString()) as TextBox;
            TextBox catalogNum = baItemsPanel.FindControl("serviceTextBox" + r.ToString()) as TextBox;
            TextBox desc = baItemsPanel.FindControl("descTextBox" + r.ToString()) as TextBox;
            RadNumericTextBox unitPrice = baItemsPanel.FindControl("costRadNumericTextBox" + r.ToString()) as RadNumericTextBox;

            if (!IsRowEmpty(qty, unit, catalogNum, desc, unitPrice))
            {
                subTotal += Convert.ToInt32(qty.Text) * Convert.ToDouble(unitPrice.Text);
            }
        }
        return subTotal;
    }

    /// <summary>
    /// Builds html table from items list
    /// </summary>
    /// <returns></returns>
    public string GetHTMLTable()
    {
        double subTotal = 0;
        string style = "border-bottom: 1px solid #000000; border-right: 1px solid #000000;";
        string emailTable = "<br /><br /><table cellspacing='0' cellpadding='2' width='100%' style='width:100%; font-size:11px; border-top: 1px solid #000000; border-left: 1px solid #000000;'>";

        emailTable += "<tr>";
        emailTable += "<td style='" + style + " width:50px'><strong>Quantity</strong></td>";
        emailTable += "<td style='" + style + " width:35px'><strong>Unit</strong></td>";
        emailTable += "<td style='" + style + " width:100px'><strong>Item/Service</strong></td>";
        emailTable += "<td style='" + style + "'><strong>Description</strong></td>";
        emailTable += "<td style='" + style + " width:55px'><strong>Price per Unit</strong></td>";
        emailTable += "</tr>";

        for (int r = 0; r <= 14; r++)
        {
            RadNumericTextBox qty = baItemsPanel.FindControl("qtyRadNumericTextBox" + r.ToString()) as RadNumericTextBox;
            TextBox unit = baItemsPanel.FindControl("unitTextBox" + r.ToString()) as TextBox;
            TextBox catalogNum = baItemsPanel.FindControl("serviceTextBox" + r.ToString()) as TextBox;
            TextBox desc = baItemsPanel.FindControl("descTextBox" + r.ToString()) as TextBox;
            RadNumericTextBox unitPrice = baItemsPanel.FindControl("costRadNumericTextBox" + r.ToString()) as RadNumericTextBox;

            if (!IsRowEmpty(qty, unit, catalogNum, desc, unitPrice))
            {
                emailTable += GetHTMLRow(qty.Text, unit.Text, catalogNum.Text, desc.Text, unitPrice.Text);
                subTotal += Convert.ToInt32(qty.Text) * Convert.ToDouble(unitPrice.Text);
            }
        }

        emailTable += @"<tr>
        <td colspan='4' style='text-align:right; border-bottom: 1px solid #000000; border-right: 1px solid #000000;'>
        <strong>Total (excluding tax and shipping charges): </strong></td>
        <td style='color:Green; border-bottom: 1px solid #000000; border-right: 1px solid #000000;'>
        <strong>" + subTotal.ToString("c") + "</strong></td></tr></table><br />";
        return emailTable;

    }

    /// <summary>
    /// Returns HTML formatted items row
    /// </summary>
    /// <param name="qty"></param>
    /// <param name="unit"></param>
    /// <param name="catalogNum"></param>
    /// <param name="desc"></param>
    /// <param name="unitPrice"></param>
    /// <returns></returns>
    protected string GetHTMLRow(string qty, string unit, string catalogNum, string desc, string unitPrice)
    {
        // Construct HTML table row
        string style = "style='border-bottom: 1px solid #000000; border-right: 1px solid #000000'";
        String row = "<tr>";
        row += "<td " + style + ">" + qty + "</td>";
        row += "<td " + style + ">" + unit + "</td>";
        row += "<td " + style + ">" + catalogNum + "</td>";
        row += "<td " + style + ">" + desc + "</td>";
        row += "<td " + style + ">" + Convert.ToDouble(unitPrice).ToString("c") + "</td>";
        row += "</tr>";

        return row;
    }

    /// <summary>
    /// Subroutine, inserts valid items into db for order, returns html table for e-mailing
    /// </summary>
    /// <param name="orderID"></param>
    public void InsertItems(int orderID)
    {
        double subTotal = 0;

        for (int r = 0; r <= 14; r++)
        {
            RadNumericTextBox qty = baItemsPanel.FindControl("qtyRadNumericTextBox" + r.ToString()) as RadNumericTextBox;
            TextBox unit = baItemsPanel.FindControl("unitTextBox" + r.ToString()) as TextBox;
            TextBox catalogNum = baItemsPanel.FindControl("serviceTextBox" + r.ToString()) as TextBox;
            TextBox desc = baItemsPanel.FindControl("descTextBox" + r.ToString()) as TextBox;
            RadNumericTextBox unitPrice = baItemsPanel.FindControl("costRadNumericTextBox" + r.ToString()) as RadNumericTextBox;

            if (!IsRowEmpty(qty, unit, catalogNum, desc, unitPrice))
            {
                InsertRow(orderID, qty.Text, unit.Text, catalogNum.Text, desc.Text, unitPrice.Text);
                subTotal += Convert.ToInt32(qty.Text) * Convert.ToDouble(unitPrice.Text);
            }
        }

        // Update the order total and flag it if it's over 5k
        double taxRate = 8.25;
        if (ConfigurationManager.AppSettings["purchaseTax"] != null)
        {
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["purchaseTax"].ToString()))
                taxRate = Convert.ToDouble(ConfigurationManager.AppSettings["purchaseTax"]);
        }

        PurchasingTableAdapters.ordersTableAdapter ordersAdapter = new PurchasingTableAdapters.ordersTableAdapter();
        ordersAdapter.UpdateLimit(orderID, ((subTotal * (taxRate / 100)) > 4999) ? true : false);
    }

    /// <summary>
    /// Inserts valid row into db
    /// </summary>
    /// <param name="orderID"></param>
    /// <param name="qty"></param>
    /// <param name="unit"></param>
    /// <param name="catalogNum"></param>
    /// <param name="desc"></param>
    /// <param name="unitPrice"></param>
    protected void InsertRow(int orderID, string qty, string unit, string catalogNum, string desc, string unitPrice)
    {
        PurchasingTableAdapters.ba_itemsTableAdapter baItemsAdapter = new PurchasingTableAdapters.ba_itemsTableAdapter();
        baItemsAdapter.Insert(
            orderID,
            Convert.ToInt32(qty),
            unit,
            catalogNum,
            desc,
            Convert.ToDecimal(unitPrice)
            );
    }

    /// <summary>
    /// Subroutine, checks is all controls are empty
    /// </summary>
    /// <param name="qty"></param>
    /// <param name="unit"></param>
    /// <param name="catalogNum"></param>
    /// <param name="desc"></param>
    /// <param name="unitPrice"></param>
    /// <returns></returns>
    protected bool IsRowEmpty(RadNumericTextBox qty, TextBox unit, TextBox catalogNum, TextBox desc, RadNumericTextBox unitPrice)
    {
        bool empty = false;

        if (String.IsNullOrEmpty(qty.Text.Trim()) &&
            (String.IsNullOrEmpty(unit.Text.Trim()) || unit.Text == "each") &&
            String.IsNullOrEmpty(catalogNum.Text.Trim()) &&
            String.IsNullOrEmpty(desc.Text.Trim()) &&
            String.IsNullOrEmpty(unitPrice.Text.Trim()))
            empty = true;

        return empty;
    }
}
