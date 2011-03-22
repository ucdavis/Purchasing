<%@ Control Language="C#" AutoEventWireup="true" CodeFile="dro_items.ascx.cs" Inherits="business_purchasing_blocks_dro_items" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:UpdatePanel ID="droUpdatePanel" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="subTotalHiddenField" Value="0" runat="server" />
        <asp:HiddenField ID="taxRateHiddenField" Value="8.25" runat="server" />
        <table cellpadding="0" cellspacing="0" class="formtable bdrlft purchase">
            <caption class="purchase">
                <asp:LinkButton ID="addLinkButton" CssClass="bdrLink" runat="server" CausesValidation="false" OnClick="addLinkButton_Click">+ Add Another</asp:LinkButton>
                |
                <asp:LinkButton ID="removeLinkButton" CssClass="bdrLink" runat="server" CausesValidation="false" OnClick="removeLinkButton_Click">Remove</asp:LinkButton></caption>
            <tr>
                <th colspan="5">Items</th>
            </tr>
            <tr class="alt smaller">
                <td style="width: 25%">
                    Item Description
                </td>
                <td style="width: 20%">
                    Estimated Item Value
                </td>
                <td style="width: 40%">
                    Description of Repair
                </td>
                <td style="width: 15%">
                    Price of Repair
                </td>
            </tr>
            <tr id="Tr0" runat="server">
                <td>
                    <asp:TextBox ID="itemDescTextBox0" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="valueRadNumericTextBox0" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                    </telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:TextBox ID="repairDescTextBox0" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox0" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server" AutoPostBack="true" OnTextChanged="costRadNumericTextBox_TextChanged">
                    </telerik:RadNumericTextBox>
                    <asp:HiddenField Value="0" ID="rowHiddenField0" runat="server" />
                </td>
            </tr>
            <tr id="Tr1" runat="server" class="alt">
                <td>
                    <asp:TextBox ID="itemDescTextBox1" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="valueRadNumericTextBox1" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                    </telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:TextBox ID="repairDescTextBox1" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox1" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server" AutoPostBack="true" OnTextChanged="costRadNumericTextBox_TextChanged">
                    </telerik:RadNumericTextBox>
                    <asp:HiddenField Value="0" ID="rowHiddenField1" runat="server" />
                </td>
            </tr>
            <tr id="Tr2" runat="server">
                <td>
                    <asp:TextBox ID="itemDescTextBox2" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="valueRadNumericTextBox2" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                    </telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:TextBox ID="repairDescTextBox2" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox2" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server" AutoPostBack="true" OnTextChanged="costRadNumericTextBox_TextChanged">
                    </telerik:RadNumericTextBox>
                    <asp:HiddenField Value="0" ID="rowHiddenField2" runat="server" />
                </td>
            </tr>
            <tr id="Tr3" runat="server" class="alt">
                <td>
                    <asp:TextBox ID="itemDescTextBox3" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="valueRadNumericTextBox3" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                    </telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:TextBox ID="repairDescTextBox3" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox3" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server" AutoPostBack="true" OnTextChanged="costRadNumericTextBox_TextChanged">
                    </telerik:RadNumericTextBox>
                    <asp:HiddenField Value="0" ID="rowHiddenField3" runat="server" />
                </td>
            </tr>
            <tr id="Tr4" runat="server">
                <td>
                    <asp:TextBox ID="itemDescTextBox4" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="valueRadNumericTextBox4" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                    </telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:TextBox ID="repairDescTextBox4" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox4" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server" AutoPostBack="true" OnTextChanged="costRadNumericTextBox_TextChanged">
                    </telerik:RadNumericTextBox>
                    <asp:HiddenField Value="0" ID="rowHiddenField4" runat="server" />
                </td>
            </tr>
            <tr id="Tr5" runat="server" class="alt">
                <td>
                    <asp:TextBox ID="itemDescTextBox5" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="valueRadNumericTextBox5" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                    </telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:TextBox ID="repairDescTextBox5" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox5" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server" AutoPostBack="true" OnTextChanged="costRadNumericTextBox_TextChanged">
                    </telerik:RadNumericTextBox>
                    <asp:HiddenField Value="0" ID="rowHiddenField5" runat="server" />
                </td>
            </tr>
            <tr id="Tr6" runat="server">
                <td>
                    <asp:TextBox ID="itemDescTextBox6" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="valueRadNumericTextBox6" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                    </telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:TextBox ID="repairDescTextBox6" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox6" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server" AutoPostBack="true" OnTextChanged="costRadNumericTextBox_TextChanged">
                    </telerik:RadNumericTextBox>
                    <asp:HiddenField Value="0" ID="rowHiddenField6" runat="server" />
                </td>
            </tr>
            <tr id="Tr7" runat="server" class="alt">
                <td>
                    <asp:TextBox ID="itemDescTextBox7" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="valueRadNumericTextBox7" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                    </telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:TextBox ID="repairDescTextBox7" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox7" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server" AutoPostBack="true" OnTextChanged="costRadNumericTextBox_TextChanged">
                    </telerik:RadNumericTextBox>
                    <asp:HiddenField Value="0" ID="rowHiddenField7" runat="server" />
                </td>
            </tr>
            <tr id="Tr8" runat="server">
                <td>
                    <asp:TextBox ID="itemDescTextBox8" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="valueRadNumericTextBox8" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                    </telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:TextBox ID="repairDescTextBox8" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox8" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server" AutoPostBack="true" OnTextChanged="costRadNumericTextBox_TextChanged">
                    </telerik:RadNumericTextBox>
                    <asp:HiddenField Value="0" ID="rowHiddenField8" runat="server" />
                </td>
            </tr>
            <tr id="Tr9" runat="server" class="alt">
                <td>
                    <asp:TextBox ID="itemDescTextBox9" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="valueRadNumericTextBox9" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                    </telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:TextBox ID="repairDescTextBox9" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox9" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server" AutoPostBack="true" OnTextChanged="costRadNumericTextBox_TextChanged">
                    </telerik:RadNumericTextBox>
                    <asp:HiddenField Value="0" ID="rowHiddenField9" runat="server" />
                </td>
            </tr>
            <tr id="Tr10" runat="server">
                <td>
                    <asp:TextBox ID="itemDescTextBox10" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="valueRadNumericTextBox10" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server">
                    </telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:TextBox ID="repairDescTextBox10" Width="100%" runat="server"></asp:TextBox>
                </td>
                <td>
                    <telerik:RadNumericTextBox MinValue="0" ID="costRadNumericTextBox10" Type="Currency" Width="100%" InvalidStyle-BackColor="Red" Skin="" SelectionOnFocus="SelectAll" runat="server" AutoPostBack="true" OnTextChanged="costRadNumericTextBox_TextChanged">
                    </telerik:RadNumericTextBox>
                    <asp:HiddenField Value="0" ID="rowHiddenField10" runat="server" />
                </td>
            </tr>
            <tr class="alt smaller">
                <td colspan="3" style="text-align: right">
                    Estimated Total (including
                    <asp:Label ID="taxLabel" runat="server" Text="8.25" />% tax but not including shipping fees):
                </td>
                <td style="font-weight: bold;">
                    <asp:Label ID="droTotalLabel" runat="server" Text="$0.00" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
