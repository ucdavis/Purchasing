<%@ Control Language="C#" AutoEventWireup="true" CodeFile="addedit_vendor.ascx.cs" Inherits="business_purchasing_blocks_addedit_vendor" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:MultiView ID="vendorMultiView" runat="server">
    <asp:View ID="View_VendorList" runat="server">
        <table cellpadding="0" cellspacing="0" class="formtable bdrlft">
            <caption class="purchase">
                <asp:LinkButton ID="addVendorLinkButton" CausesValidation="false" CssClass="bdrLink" runat="server" OnClick="addVendorLinkButton_Click">New Vendor</asp:LinkButton></caption>
            <tr>
                <th colspan="2">Vendor</th>
            </tr>
            <tr class="alt smaller">
                <td>Select a vendor. If your vendor is not listed, select "New Vendor" </td>
            </tr>
            <tr>
                <td style="padding: 8px;">
                    <telerik:RadComboBox ID="vendorRadComboBox" CausesValidation="false" OnClientSelectedIndexChanging="VendorChanging" AutoCompleteSeparator="|" MarkFirstMatch="true" CollapseAnimation-Type="None" Width="300px" AppendDataBoundItems="true" AutoPostBack="True" DataTextField="vendor_name" DataValueField="id" runat="server" OnSelectedIndexChanged="vendorRadComboBox_SelectedIndexChanged">
                    </telerik:RadComboBox>
                    <asp:CompareValidator ID="vendorCompareValidator" runat="server" Text="*" ControlToValidate="vendorRadComboBox" ValueToCompare="Type vendor name here" Operator="NotEqual" ErrorMessage="Please select a vendor"></asp:CompareValidator>
                </td>
            </tr>
        </table>
    </asp:View>
    <asp:View ID="View_VendorDetail" runat="server">
        <table cellpadding="0" cellspacing="0" class="formtable bdrlft">
            <caption class="purchase">
                <asp:LinkButton ID="changeVendorLinkButton" CausesValidation="false" CssClass="bdrLink" runat="server" OnClick="changeVendorLinkButton_Click">&lArr; Back To Vendors List</asp:LinkButton>
                |
                <asp:LinkButton ID="editVendorLinkButton" CausesValidation="false" CssClass="bdrLink" runat="server" OnClick="editVendorLinkButton_Click">Edit This Vendor</asp:LinkButton></caption>
            <tr>
                <th colspan="2">Vendor<asp:Label ID="vendorTitleLabel" runat="server" /></th>
            </tr>
            <tr class="alt smaller">
                <td>Vendor Details </td>
            </tr>
            <tr>
                <td style="padding: 8px;">
                    <table style="border: 1px solid #DDD; width: 100%">
                        <tr>
                            <td style="border: 0px none !important; padding: 5px; font-size: 10px; vertical-align: top;">
                                <asp:Label ID="vendorNameLabel" Font-Bold="true" Font-Size="1.7em" runat="server" /><br />
                                <asp:Label ID="vendorAddressLabel" Font-Size="10px" runat="server" /><br />
                                <asp:Label ID="vendorPhoneLabel" Font-Size="10px" runat="server" /><br />
                                <asp:Label ID="vendorFaxLabel" Font-Size="10px" runat="server" /><br />
                                <!-- Vendor customer number is hidden until finished (need to add to order details as well as a field for promotional code) -->
                                <span style="font-size: 15px; display: none;"><strong>Customer #:</strong></span>
                                <asp:TextBox ID="vendorCustomerTextBox" Visible="false" MaxLength="64" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:View>
    <asp:View ID="View_VendorAddEdit" runat="server">
        <asp:HiddenField ID="vendorIDHiddenField" runat="server" />
        <table cellpadding="0" cellspacing="0" class="formtable bdrlft">
            <caption class="purchase">
                <asp:LinkButton ID="backVendorLinkButton" CausesValidation="false" CssClass="bdrLink" runat="server" OnClick="backVendorLinkButton_Click" /></caption>
            <tr>
                <th colspan="2">
                    <asp:Label ID="addEditVendorLabel" runat="server" /></th>
            </tr>
            <tr class="alt smaller">
                <td colspan="2">
                    <asp:Label ID="addEditVendorSubLabel" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="width: 110px;">Vendor Name: </td>
                <td>
                    <asp:TextBox ID="vendorNameTextBox" Width="200" BackColor="LemonChiffon" MaxLength="256" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="vendorNameRequiredFieldValidator" runat="server" ControlToValidate="vendorNameTextBox" ErrorMessage="Please enter a vendor name." Text="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr class="alt">
                <td>Vendor Address: </td>
                <td>
                    <asp:TextBox ID="vendorAddressTextBox" Width="200" BackColor="LemonChiffon" MaxLength="256" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="vendorAddressRequiredFieldValidator" runat="server" ControlToValidate="vendorAddressTextBox" ErrorMessage="Please enter a vendor address." Text="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>Vendor City: </td>
                <td>
                    <asp:TextBox ID="vendorCityTextBox" Width="200" BackColor="LemonChiffon" MaxLength="64" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="vendorCityRequiredFieldValidator" runat="server" ControlToValidate="vendorCityTextBox" ErrorMessage="Please enter a vendor city." Text="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr class="alt">
                <td>State/Province: </td>
                <td>
                    <telerik:RadComboBox ID="vendorStateRadComboBox" CollapseAnimation-Type="None" Width="202px" runat="server">
                        <Items>
                            <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                            <telerik:RadComboBoxItem Text="Alabama" Value="AL" />
                            <telerik:RadComboBoxItem Text="Alaska" Value="AK" />
                            <telerik:RadComboBoxItem Text="Arizona" Value="AZ" />
                            <telerik:RadComboBoxItem Text="Arkansas" Value="AR" />
                            <telerik:RadComboBoxItem Text="California" Value="CA" />
                            <telerik:RadComboBoxItem Text="Canada" Value="CANADA" />
                            <telerik:RadComboBoxItem Text="Colorado" Value="CO" />
                            <telerik:RadComboBoxItem Text="Connecticut" Value="CT" />
                            <telerik:RadComboBoxItem Text="Delaware" Value="DE" />
                            <telerik:RadComboBoxItem Text="Florida" Value="FL" />
                            <telerik:RadComboBoxItem Text="Georgia" Value="GA" />
                            <telerik:RadComboBoxItem Text="Hawaii" Value="HI" />
                            <telerik:RadComboBoxItem Text="Idaho" Value="ID" />
                            <telerik:RadComboBoxItem Text="Illinois" Value="IL" />
                            <telerik:RadComboBoxItem Text="Indiana" Value="IN" />
                            <telerik:RadComboBoxItem Text="Iowa" Value="IA" />
                            <telerik:RadComboBoxItem Text="Kansas" Value="KS" />
                            <telerik:RadComboBoxItem Text="Kentucky" Value="KY" />
                            <telerik:RadComboBoxItem Text="Louisiana" Value="LA" />
                            <telerik:RadComboBoxItem Text="Maine" Value="ME" />
                            <telerik:RadComboBoxItem Text="Maryland" Value="MD" />
                            <telerik:RadComboBoxItem Text="Massachusetts" Value="MA" />
                            <telerik:RadComboBoxItem Text="Michigan" Value="MI" />
                            <telerik:RadComboBoxItem Text="Minnesota" Value="MN" />
                            <telerik:RadComboBoxItem Text="Mississippi" Value="MS" />
                            <telerik:RadComboBoxItem Text="Missouri" Value="MO" />
                            <telerik:RadComboBoxItem Text="Montana" Value="MT" />
                            <telerik:RadComboBoxItem Text="Nebraska" Value="NE" />
                            <telerik:RadComboBoxItem Text="Nevada" Value="NV" />
                            <telerik:RadComboBoxItem Text="New Hampshire" Value="NH" />
                            <telerik:RadComboBoxItem Text="New Jersey" Value="NJ" />
                            <telerik:RadComboBoxItem Text="New Mexico" Value="NM" />
                            <telerik:RadComboBoxItem Text="New York" Value="NY" />
                            <telerik:RadComboBoxItem Text="North Carolina" Value="NC" />
                            <telerik:RadComboBoxItem Text="North Dakota" Value="ND" />
                            <telerik:RadComboBoxItem Text="Ohio" Value="OH" />
                            <telerik:RadComboBoxItem Text="Oklahoma" Value="OK" />
                            <telerik:RadComboBoxItem Text="Oregon" Value="OR" />
                            <telerik:RadComboBoxItem Text="Pennsylvania" Value="PA" />
                            <telerik:RadComboBoxItem Text="Rhode Island" Value="RI" />
                            <telerik:RadComboBoxItem Text="South Carolina" Value="SC" />
                            <telerik:RadComboBoxItem Text="South Dakota" Value="SD" />
                            <telerik:RadComboBoxItem Text="Tennessee" Value="TN" />
                            <telerik:RadComboBoxItem Text="Texas" Value="TX" />
                            <telerik:RadComboBoxItem Text="Utah" Value="UT" />
                            <telerik:RadComboBoxItem Text="Vermont" Value="VT" />
                            <telerik:RadComboBoxItem Text="Virginia" Value="VA" />
                            <telerik:RadComboBoxItem Text="Washington" Value="WA" />
                            <telerik:RadComboBoxItem Text="West Virginia" Value="WV" />
                            <telerik:RadComboBoxItem Text="Wisconsin" Value="WI" />
                            <telerik:RadComboBoxItem Text="Wyoming" Value="WY" />
                        </Items>
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td>Vendor Phone: </td>
                <td>
                    <telerik:RadMaskedTextBox ID="vendorPhoneRadMaskedTextBox" BackColor="LemonChiffon" InvalidStyle-BackColor="Red" ClientEvents-OnFocus="OnFocus" Width="200" Skin="" Mask="(###) ### - ####" runat="server">
                    </telerik:RadMaskedTextBox><asp:RequiredFieldValidator ID="vendorPhoneRequiredFieldValidator" runat="server" ControlToValidate="vendorPhoneRadMaskedTextBox" Text="*" ErrorMessage="Please enter a vendor phone."></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr class="alt">
                <td>Vendor FAX: </td>
                <td>
                    <telerik:RadMaskedTextBox ID="vendorFaxRadMaskedTextBox" BackColor="LemonChiffon" InvalidStyle-BackColor="Red" ClientEvents-OnFocus="OnFocus" Width="200" Skin="" Mask="(###) ### - ####" runat="server">
                    </telerik:RadMaskedTextBox><asp:RequiredFieldValidator ID="vendorFaxRequiredFieldValidator" runat="server" ControlToValidate="vendorFaxRadMaskedTextBox" Text="*" ErrorMessage="Please enter a vendor fax number."></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>Vendor URL: </td>
                <td>
                    <asp:TextBox ID="vendorURLTextBox" MaxLength="256" Width="200" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr class="alt">
                <td>Customer #: </td>
                <td>
                    <asp:TextBox ID="vendorCustomerAddTextBox" MaxLength="64" Width="200" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>
    </asp:View>
</asp:MultiView>

<script type="text/javascript">
<!--
    // Handles vendor combo box changes
    function VendorChanging(sender, eventArgs) {
        if (eventArgs.get_item().get_text() == "Type vendor name here")
            eventArgs.set_cancel(true);
        else {
            eventArgs.set_cancel(false);
        }
    }
//-->
</script>

