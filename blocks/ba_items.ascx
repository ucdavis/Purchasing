<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ba_items.ascx.cs" Inherits="business_purchasing_blocks_ba_items" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Panel ID="baItemsPanel" runat="server">
    <asp:HiddenField ID="subTotalHiddenField" Value="0" runat="server" />
    <asp:HiddenField ID="taxRateHiddenField" Value="8.25" runat="server" />
    <table cellpadding="0" cellspacing="0" class="formtable bdrlft purchase">
        <caption class="purchase"><a href="javascript:AddItem();" class="bdrLink">+ Add Another</a> | <a href="javascript:RemoveItem()" class="bdrLink">Remove</a></caption>
        <tr>
            <th colspan="5">Items</th>
        </tr>
        <tr class="alt smaller">
            <td style="width: 10%">Quantity</td>
            <td style="width: 10%">Unit</td>
            <td style="width: 15%">Service / Item</td>
            <td style="width: 50%">Complete Description</td>
            <td style="width: 15%">Price per Unit</td>
        </tr>
        <tr id="Tr0" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox0" row="0" NumberFormat-AllowRounding="true" MinValue="0" ClientEvents-OnValueChanged="QuanitiyChanged" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox0" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="serviceTextBox0" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox0" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox0" row="0" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField0" runat="server" />
            </td>
        </tr>
        <tr id="Tr1" runat="server" class="alt">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox1" row="1" MinValue="0" ClientEvents-OnValueChanged="QuanitiyChanged" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox1" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="serviceTextBox1" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox1" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox1" row="1" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField1" runat="server" />
            </td>
        </tr>
        <tr id="Tr2" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox2" row="2" MinValue="0" ClientEvents-OnValueChanged="QuanitiyChanged" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox2" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="serviceTextBox2" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox2" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox2" row="2" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField2" runat="server" />
            </td>
        </tr>
        <tr id="Tr3" runat="server" class="alt">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox3" row="3" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox3" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="serviceTextBox3" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox3" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox3" row="3" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField3" runat="server" />
            </td>
        </tr>
        <tr id="Tr4" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox4" row="4" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox4" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="serviceTextBox4" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox4" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox4" row="4" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField4" runat="server" />
            </td>
        </tr>
        <tr id="Tr5" runat="server" class="alt">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox5" row="5" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox5" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="serviceTextBox5" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox5" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox5" row="5" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField5" runat="server" />
            </td>
        </tr>
        <tr id="Tr6" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox6" row="6" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox6" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="serviceTextBox6" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox6" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox6" row="6" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField6" runat="server" />
            </td>
        </tr>
        <tr id="Tr7" runat="server" class="alt">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox7" row="7" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox7" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="serviceTextBox7" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox7" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox7" row="7" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField7" runat="server" />
            </td>
        </tr>
        <tr id="Tr8" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox8" row="8" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox8" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="serviceTextBox8" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox8" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox8" row="8" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField8" runat="server" />
            </td>
        </tr>
        <tr id="Tr9" runat="server" class="alt">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox9" row="9" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox9" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="serviceTextBox9" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox9" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox9" row="9" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField9" runat="server" />
            </td>
        </tr>
        <tr id="Tr10" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox10" row="10" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox10" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="serviceTextBox10" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox10" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox10" row="10" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField10" runat="server" />
            </td>
        </tr>
        <tr id="Tr11" runat="server" class="alt">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox11" row="11" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox11" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="serviceTextBox11" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox11" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox11" row="11" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField11" runat="server" />
            </td>
        </tr>
        <tr id="Tr12" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox12" row="12" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox12" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="serviceTextBox12" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox12" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox12" row="12" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField12" runat="server" />
            </td>
        </tr>
        <tr id="Tr13" runat="server" class="alt">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox13" row="13" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox13" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="serviceTextBox13" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox13" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox13" row="13" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField13" runat="server" />
            </td>
        </tr>
        <tr id="Tr14" runat="server">
            <td>
                <telerik:RadNumericTextBox ID="qtyRadNumericTextBox14" row="14" ClientEvents-OnValueChanged="QuanitiyChanged" MinValue="0" Width="100%" Type="Number" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
            </td>
            <td>
                <asp:TextBox ID="unitTextBox14" Width="100%" Text="each" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="serviceTextBox14" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="descTextBox14" Width="100%" runat="server"></asp:TextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox14" ClientEvents-OnLoad="InitTotals" row="14" ClientEvents-OnValueChanged="ValueChanged" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" EnableEmbeddedSkins="false" SelectionOnFocus="SelectAll" runat="server">
                </telerik:RadNumericTextBox>
                <asp:HiddenField Value="0" ID="rowHiddenField14" runat="server" />
            </td>
        </tr>
        <tr class="alt smaller">
            <td colspan="4" style="text-align: right">Estimated Total (including
                <asp:Label ID="taxLabel" runat="server" Text="8.25" />% tax but not including shipping fees): </td>
            <td style="font-weight: bold;">
                <asp:Label ID="baTotalLabel" runat="server" Text="" />
            </td>
        </tr>
    </table>
</asp:Panel>

<script type="text/javascript">
    <!--
    var rowTotals = new Array(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    var runningTotal = 0;
    var formattedNumber = 0;
    var hasInnerText = (document.getElementsByTagName("body")[0].innerText != undefined) ? true : false;

    function InitTotals() {
        for (var r = 0; r <= 14; r = r + 1)
            rowTotals[r] = parseFloat(document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_rowHiddenField" + r).value);
        runningTotal = (document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_subTotalHiddenField").value);
        OutputTotal(AddRowTotals());
    }

    // Handles quanitiy changes
    function QuanitiyChanged(sender, eventArgs) {
        var row = GetRowAtrb(sender) // Determine the row in which the quanitiy was changed
        var unitPrice = document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_costRadNumericTextBox" + row).value; // Get the unit price for this row
        if (unitPrice == "")
            unitPrice = 0;

        rowTotals[row] = Math.round(eventArgs.get_newValue()) * unitPrice; // Insert the row's total (int quanitiy * unitPrice) into the row position in the array
        OutputTotal(AddRowTotals());

        if (eventArgs.get_newValue() == "")
            eventArgs.set_cancel(true);
    }

    // Handles value (unit price) changes
    function ValueChanged(sender, eventArgs) {
        var row = GetRowAtrb(sender) // Determine the row in which the unit price was changed

        var qty = document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_qtyRadNumericTextBox" + row + "_text").value; // Get the quanitiy for the unit price
        if (qty == "")
            qty = 0;

        rowTotals[row] = eventArgs.get_newValue() * qty; // Insert the row's total (unit price * quanitiy) into the row position in the array
        OutputTotal(AddRowTotals());

        if (eventArgs.get_newValue() == "")
            eventArgs.set_cancel(true);
    }

    // Outputs total to label element
    function OutputTotal(runningTotal) {
        var lbl = document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_baTotalLabel");
        var chargeLbl = document.getElementById("ctl00_MainContentPlaceHolder_chargeLabel");

        formattedNumber = formatNumber(runningTotal, 2); // Add tax and decimal places

        if (formattedNumber > 4999) {
            lbl.style.color = "Red";
            if (chargeLbl)
                chargeLbl.style.color = "Red";
        }
        else if (formattedNumber == 0.00) {
            lbl.style.color = "";
            if (chargeLbl)
                chargeLbl.style.color = "";
        }
        else {
            lbl.style.color = "Green";
            if (chargeLbl)
                chargeLbl.style.color = "Green";
        }

        if (!hasInnerText) {
            // FireFox doesn't like innerText
            lbl.textContent = "$" + formattedNumber;
            if (chargeLbl)
                chargeLbl.textContent = "$" + formattedNumber;
        }
        else {
            lbl.innerText = "$" + formattedNumber;
            if (chargeLbl)
                chargeLbl.innerText = "$" + formattedNumber;
        }
    }

    // Adds up the row totals
    function AddRowTotals() {
        var total = 0;
        for (var i = 0; i <= 14; i = i + 1)
            total = total + rowTotals[i]; // Add up all the row totals
        return total;
    }

    // Returns which row the sender is from
    function GetRowAtrb(sender) {
        var target = document.getElementById(sender.get_element().id + '_text');
        if (target.getAttribute('row'))
            return target.getAttribute('row');
        if (target.row)
            return target.row;
    }

    // Returns taxed and padded number
    function formatNumber(myNum, numOfDec) {
        var decimal = 1;
        for (i = 1; i <= numOfDec; i++)
            decimal = decimal * 10;

        var taxRate = (document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_taxRateHiddenField").value);

        var myFormattedNum = ((Math.round(myNum * decimal) / decimal) * ((taxRate / 100) + 1)).toFixed(numOfDec);
        return myFormattedNum;
    }

    // Resets caret to start on focus (phone and fax masked texboxes)
    function OnFocus(sender, eventArgs) {
        sender.set_caretPosition(1);
    }

    // Adds BA row
    function AddItem() {
        for (var r = 1; r <= 14; ++r) {
            var row = document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_Tr" + r);
            if (row.style.display == "none") {
                row.style.display = "";
                return;
            }
        }
        // none available?
    }

    // Removes BA row
    function RemoveItem() {
        for (var r = 14; r > 0; --r) {
            var row = document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_Tr" + r);
            if (row.style.display == "") {
                row.style.display = "none";
                ClearTB(r);
                rowTotals[r] = 0;
                OutputTotal(AddRowTotals());
                return;
            }
        }
    }

    function ClearTB(which) {
        switch (which) {
            case 0:
                var cost = $find("<%= costRadNumericTextBox0.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox0.ClientID %>"); qty.clear();
                break;
            case 1:
                var cost = $find("<%= costRadNumericTextBox1.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox1.ClientID %>"); qty.clear();
                break;
            case 2:
                var cost = $find("<%= costRadNumericTextBox2.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox2.ClientID %>"); qty.clear();
                break;
            case 3:
                var cost = $find("<%= costRadNumericTextBox3.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox3.ClientID %>"); qty.clear();
                break;
            case 4:
                var cost = $find("<%= costRadNumericTextBox4.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox4.ClientID %>"); qty.clear();
                break;
            case 5:
                var cost = $find("<%= costRadNumericTextBox5.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox5.ClientID %>"); qty.clear();
                break;
            case 6:
                var cost = $find("<%= costRadNumericTextBox6.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox6.ClientID %>"); qty.clear();
                break;
            case 7:
                var cost = $find("<%= costRadNumericTextBox7.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox7.ClientID %>"); qty.clear();
                break;
            case 8:
                var cost = $find("<%= costRadNumericTextBox8.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox8.ClientID %>"); qty.clear();
                break;
            case 9:
                var cost = $find("<%= costRadNumericTextBox9.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox9.ClientID %>"); qty.clear();
                break;
            case 10:
                var cost = $find("<%= costRadNumericTextBox10.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox10.ClientID %>"); qty.clear();
                break;
            case 11:
                var cost = $find("<%= costRadNumericTextBox11.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox11.ClientID %>"); qty.clear();
                break;
            case 12:
                var cost = $find("<%= costRadNumericTextBox12.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox12.ClientID %>"); qty.clear();
                break;
            case 13:
                var cost = $find("<%= costRadNumericTextBox13.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox13.ClientID %>"); qty.clear();
                break;
            case 14:
                var cost = $find("<%= costRadNumericTextBox14.ClientID %>"); cost.clear();
                var qty = $find("<%= qtyRadNumericTextBox14.ClientID %>"); qty.clear();
                break;
            default:
                break;
        }

        document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_unitTextBox" + which).value = 'each';
        document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_serviceTextBox" + which).value = '';
        document.getElementById("ctl00_MainContentPlaceHolder_ba_items1_descTextBox" + which).value = '';
    }
    //-->
</script>

