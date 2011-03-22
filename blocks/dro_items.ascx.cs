using System;
using System.Configuration;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Drawing;

public partial class business_purchasing_blocks_dro_items : System.Web.UI.UserControl
{
    // Paths
    protected string savePath = ConfigurationManager.AppSettings["purchasingSavePath"].ToString();
    protected string filePath = ConfigurationManager.AppSettings["purchasingRoot"].ToString() + "admin/uploads/";

    // Page event, raised on page load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            HandleIE();

            // If this is a new order, set the inital row count to 5
            if (Request.QueryString.Count == 0)
                InitialDroRows(3);

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
            foreach (System.Web.UI.Control c in droUpdatePanel.ContentTemplateContainer.Controls)
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
    /// Subroutine, sets the number of initially visible DPO rows
    /// </summary>
    /// <param name="count"></param>
    protected void InitialDroRows(int count)
    {
        for (int r = 0; r <= 10; r++)
        {
            HtmlTableRow tr = droUpdatePanel.FindControl("Tr" + r.ToString()) as HtmlTableRow;
            tr.Style["display"] = r < count ? String.Empty : "none";
        }
    }

    /// <summary>
    /// Subroutine, populates items table with items
    /// </summary>
    /// <param name="orderID"></param>
    public void PopulateItems(int orderID)
    {
        PurchasingTableAdapters.dro_itemsTableAdapter itemsAdapter = new PurchasingTableAdapters.dro_itemsTableAdapter();
        Purchasing.dro_itemsDataTable dt = itemsAdapter.GetByOrderId(orderID);

        int r = 0;
        double rowsSubTotal = 0;

        foreach (Purchasing.dro_itemsRow dr in dt.Rows)
        {
            HtmlTableRow tr = droUpdatePanel.FindControl("Tr" + r.ToString()) as HtmlTableRow;
            HiddenField hf = droUpdatePanel.FindControl("rowHiddenField" + r.ToString()) as HiddenField;
            RadNumericTextBox unitPrice = droUpdatePanel.FindControl("costRadNumericTextBox" + r.ToString()) as RadNumericTextBox;
            RadNumericTextBox itemValue = droUpdatePanel.FindControl("valueRadNumericTextBox" + r.ToString()) as RadNumericTextBox;
            TextBox itemDesc = droUpdatePanel.FindControl("itemDescTextBox" + r.ToString()) as TextBox;
            TextBox repairDesc = droUpdatePanel.FindControl("repairDescTextBox" + r.ToString()) as TextBox;

            tr.Style["display"] = String.Empty;
            itemValue.Text = dr.item_value.ToString();
            unitPrice.Text = dr.repair_cost.ToString();
            itemDesc.Text = dr.item_desc;
            repairDesc.Text = dr.repair_desc;

            double rowTotal = (1 * Convert.ToDouble(dr.repair_cost));
            hf.Value = rowTotal.ToString();
            rowsSubTotal += rowTotal;
            r++;
        }

        subTotalHiddenField.Value = rowsSubTotal.ToString();
        if (r > 0)
            InitialDroRows(r);
        else
            InitialDroRows(5);
        UpdateTotal();
    }

    /// <summary>
    /// Gets order subtotal
    /// </summary>
    /// <returns></returns>
    public Double GetOrderSubTotal()
    {
        double subTotal = 0;
        for (int r = 0; r <= 10; r++)
        {
            RadNumericTextBox unitPrice = droUpdatePanel.FindControl("costRadNumericTextBox" + r.ToString()) as RadNumericTextBox;
            RadNumericTextBox itemValue = droUpdatePanel.FindControl("valueRadNumericTextBox" + r.ToString()) as RadNumericTextBox;
            TextBox itemDesc = droUpdatePanel.FindControl("itemDescTextBox" + r.ToString()) as TextBox;
            TextBox repairDesc = droUpdatePanel.FindControl("repairDescTextBox" + r.ToString()) as TextBox;

            if (!IsRowEmpty(unitPrice, itemValue, itemDesc, repairDesc))
                subTotal += 1 * Convert.ToDouble(unitPrice.Text);
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
        emailTable += "<td style='" + style + " width:200px'><strong>Item Description</strong></td>";
        emailTable += "<td style='" + style + " width:120px'><strong>Estimated Item Value</strong></td>";
        emailTable += "<td style='" + style + " width:200px'><strong>Description of Repair</strong></td>";
        emailTable += "<td style='" + style + " width:100px'><strong>Price of Repair</strong></td>";
        emailTable += "</tr>";

        for (int r = 0; r <= 10; r++)
        {
            RadNumericTextBox unitPrice = droUpdatePanel.FindControl("costRadNumericTextBox" + r.ToString()) as RadNumericTextBox;
            RadNumericTextBox itemValue = droUpdatePanel.FindControl("valueRadNumericTextBox" + r.ToString()) as RadNumericTextBox;
            TextBox itemDesc = droUpdatePanel.FindControl("itemDescTextBox" + r.ToString()) as TextBox;
            TextBox repairDesc = droUpdatePanel.FindControl("repairDescTextBox" + r.ToString()) as TextBox;

            if (!IsRowEmpty(unitPrice, itemValue, itemDesc, repairDesc))
            {
                emailTable += GetHTMLRow(itemDesc.Text, repairDesc.Text, itemValue.Text, unitPrice.Text);
                subTotal += 1 * Convert.ToDouble(unitPrice.Text);
            }
        }

        emailTable += @"<tr>
        <td colspan='3' style='text-align:right; border-bottom: 1px solid #000000; border-right: 1px solid #000000;'>
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
    protected string GetHTMLRow(string itemDesc, string repairDesc, string itemValue, string repairCost)
    {
        // Construct HTML table row
        string style = "style='border-bottom: 1px solid #000000; border-right: 1px solid #000000'";
        String row = "<tr>";
        row += "<td " + style + ">" + itemDesc + "</td>";
        row += "<td " + style + ">" + Convert.ToDouble(itemValue).ToString("c") + "</td>";
        row += "<td " + style + ">" + repairDesc + "</td>";
        row += "<td " + style + ">" + Convert.ToDouble(repairCost).ToString("c") + "</td>";
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

        for (int r = 0; r <= 10; r++)
        {
            RadNumericTextBox unitPrice = droUpdatePanel.FindControl("costRadNumericTextBox" + r.ToString()) as RadNumericTextBox;
            RadNumericTextBox itemValue = droUpdatePanel.FindControl("valueRadNumericTextBox" + r.ToString()) as RadNumericTextBox;
            TextBox itemDesc = droUpdatePanel.FindControl("itemDescTextBox" + r.ToString()) as TextBox;
            TextBox repairDesc = droUpdatePanel.FindControl("repairDescTextBox" + r.ToString()) as TextBox;

            if (!IsRowEmpty(unitPrice, itemValue, itemDesc, repairDesc))
            {
                InsertRow(orderID, itemDesc.Text, repairDesc.Text, itemValue.Text, unitPrice.Text);
                subTotal += 1 * Convert.ToDouble(unitPrice.Text);
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
    protected void InsertRow(int orderID, string itemDesc, string repairDesc, string itemValue, string repairCost)
    {
        PurchasingTableAdapters.dro_itemsTableAdapter itemsAdapter = new PurchasingTableAdapters.dro_itemsTableAdapter();
        itemsAdapter.Insert(
            orderID,
            itemDesc,
            repairDesc,
            Convert.ToDecimal(repairCost),
            Convert.ToDecimal(itemValue)
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
    protected bool IsRowEmpty(RadNumericTextBox unitPrice, RadNumericTextBox itemValue, TextBox itemDesc, TextBox repairDesc)
    {
        bool empty = false;

        if (String.IsNullOrEmpty(unitPrice.Text.Trim()) &&
            String.IsNullOrEmpty(itemValue.Text.Trim()) &&
            String.IsNullOrEmpty(itemDesc.Text.Trim()) &&
            String.IsNullOrEmpty(repairDesc.Text.Trim())
            )
            empty = true;

        return empty;
    }

    /// <summary>
    /// Updates item total
    /// </summary>
    private void UpdateTotal()
    {
        double runningTotal = 0;

        // Add up all rows
        for (int i = 0; i <= 10; i++)
        {
            RadNumericTextBox rntb = droUpdatePanel.FindControl("costRadNumericTextBox" + i.ToString()) as RadNumericTextBox;
            if (rntb.Value != null)
                runningTotal += (Double)rntb.Value;
        }

        // Add tax
        double taxRate = 8.25;
        if (ConfigurationManager.AppSettings["purchaseTax"] != null)
        {
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["purchaseTax"].ToString()))
                taxRate = Convert.ToDouble(ConfigurationManager.AppSettings["purchaseTax"]);
        }
        runningTotal += runningTotal * (taxRate / 100);

        // Set total and color
        droTotalLabel.Text = runningTotal.ToString("c");
        
        if (runningTotal == 0)
            droTotalLabel.ForeColor = Color.Empty;
        else if (runningTotal > 5000)
            droTotalLabel.ForeColor = Color.Red;
        else
            droTotalLabel.ForeColor = Color.Green;
    }

    // Event, raised on cost textbox text changed
    protected void costRadNumericTextBox_TextChanged(object sender, EventArgs e)
    {
        UpdateTotal();
    }

    // Handles adding row
    protected void addLinkButton_Click(object sender, EventArgs e)
    {
        for (int i = 0; i <= 10; i++)
        {
            HtmlTableRow tr = droUpdatePanel.FindControl("Tr" + i.ToString()) as HtmlTableRow;
            if (tr.Style["display"] == "none")
            {
                tr.Style["display"] = String.Empty;
                return;
            }
        }
    }

    // Handles clearing and hiding row
    protected void removeLinkButton_Click(object sender, EventArgs e)
    {
        for (int i = 10; i > 0; i--)
        {
            HtmlTableRow tr = droUpdatePanel.FindControl("Tr" + i.ToString()) as HtmlTableRow;
            if (tr.Style["display"] != "none")
            {
                TextBox itemDesc = droUpdatePanel.FindControl("itemDescTextBox" + i.ToString()) as TextBox;
                TextBox repairDesc = droUpdatePanel.FindControl("repairDescTextBox" + i.ToString()) as TextBox;
                RadNumericTextBox valueTb = droUpdatePanel.FindControl("valueRadNumericTextBox" + i.ToString()) as RadNumericTextBox;
                RadNumericTextBox costTb = droUpdatePanel.FindControl("costRadNumericTextBox" + i.ToString()) as RadNumericTextBox;

                itemDesc.Text = String.Empty;
                repairDesc.Text = String.Empty;
                valueTb.Text = String.Empty;
                costTb.Text = String.Empty;

                tr.Style["display"] = "none";

                // Update total
                UpdateTotal();
                return;
            }
        }
    }
}
