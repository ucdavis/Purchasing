<%@ Page Language="C#" AutoEventWireup="true" CodeFile="register.aspx.cs" Inherits="business_purchasing_register" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Request Access</title>
    <style type="text/css">
        .radfdInnerSpan { color: #000000 !important; }
        .watermark { color: #999 !important; }
        table.request tr td { padding: 1px 0; }
        body { margin: 0px; padding: 0px; font-size: 11px; font-family: verdana, arial, helvetica, sans-serif !important; }
        .form { margin: 0px; padding: 10px 0 0 19px; }
        .title { height: 20px; font-size: 10px; vertical-align: middle; padding: 2px 10px; background-color: Gray; }
        .section { padding: 8px 0 10px 0; font-size: 12px; font-weight: bold; display: block; }
        a:link, #page_content a:visited { color: #002666; text-decoration: underline; }
        a:hover { text-decoration: none; color: #77160B; }
        a:active { color: #77160B; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="MasterRadScriptManager" runat="server">
    </telerik:RadScriptManager>
    <asp:MultiView ID="requestMultiView" runat="server">
        <asp:View ID="View_Request" runat="server">
            <telerik:RadFormDecorator ID="registerRadFormDecorator" DecoratedControls="All" runat="server" />
            <div class="form">
                <table cellpadding="0" cellspacing="0" class="request">
                    <tr>
                        <td colspan="2"><span class="section" style="padding-top: 0px;">Personal Information</span></td>
                    </tr>
                    <tr>
                        <td style="width: 110px">First Name: </td>
                        <td>
                            <asp:TextBox ID="fnameTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="fnameRequiredFieldValidator" Text="*" runat="server" ControlToValidate="fnameTextBox" ErrorMessage="Please enter your first name."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>Last Name: </td>
                        <td>
                            <asp:TextBox ID="lnameTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="lnameRequiredFieldValidator" Text="*" runat="server" ControlToValidate="lnameTextBox" ErrorMessage="Please enter your last name."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>UCD E-mail: </td>
                        <td>
                            <asp:TextBox ID="emailTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="emailRequiredFieldValidator" Text="*" runat="server" ControlToValidate="emailTextBox" ErrorMessage="Please enter your UC Davis e-mail address."></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="emailRegularExpressionValidator" Text="*" runat="server" ControlToValidate="emailTextBox" ErrorMessage="Please enter a valid e-mail address (EX: csmith@ucdavis.edu)" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>Contact Phone: </td>
                        <td>
                            <telerik:RadMaskedTextBox ID="phoneRadMaskedTextBox" BackColor="LemonChiffon" InvalidStyle-BackColor="Red" ClientEvents-OnFocus="OnFocus" runat="server" Mask="(###) ### - ####" ResetCaretOnFocus="true" Width="100px" Skin="">
                            </telerik:RadMaskedTextBox>
                            <asp:RequiredFieldValidator ID="phoneRequiredFieldValidator" Text="*" ControlToValidate="phoneRadMaskedTextBox" runat="server" ErrorMessage="Please enter a phone number"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
                <asp:MultiView ID="unitsMultiView" runat="server">
                    <asp:View ID="View_Unit_List" runat="server">
                        <table cellpadding="0" cellspacing="0" class="request">
                            <tr>
                                <td style="width: 110px">Unit: </td>
                                <td>
                                    <telerik:RadComboBox ID="unitsRadComboBox" Width="202px" CausesValidation="false" OnClientSelectedIndexChanging="Changing" CollapseAnimation-Type="None" AutoPostBack="true" AppendDataBoundItems="true" DataTextField="unit_abrv" DataValueField="id" runat="server" OnSelectedIndexChanged="unitsRadComboBox_SelectedIndexChanged">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                        </Items>
                                    </telerik:RadComboBox>
                                    <asp:CompareValidator ID="unitsCompareValidator" Text="*" runat="server" ControlToValidate="unitsRadComboBox" ValueToCompare="Select One..." Operator="NotEqual" ErrorMessage="Please select the unit where you work."></asp:CompareValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                    <asp:View ID="View_Unit_New" runat="server">
                        <table cellpadding="0" cellspacing="0" class="request">
                            <tr>
                                <td style="width: 110px">Unit Name: </td>
                                <td>
                                    <asp:TextBox ID="unitNameTextBox" BackColor="LemonChiffon" MaxLength="256" Width="200" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" Text="*" ControlToValidate="unitNameTextBox" runat="server" ErrorMessage="Please enter a unit name EX: Road Ecology Center"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>Unit Abbreviation: </td>
                                <td>
                                    <asp:TextBox ID="unitAbrvTextBox" BackColor="LemonChiffon" MaxLength="64" Width="200" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" Text="*" ControlToValidate="unitAbrvTextBox" runat="server" ErrorMessage="Please enter a unit abbreviation EX: TERC"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>FAX Number: </td>
                                <td>
                                    <telerik:RadMaskedTextBox ID="unitFaxRadMaskedTextBox" InvalidStyle-BackColor="Red" ClientEvents-OnFocus="OnFocus" runat="server" Mask="(###) ### - ####" ResetCaretOnFocus="true" Width="100px" Skin="">
                                    </telerik:RadMaskedTextBox>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                </asp:MultiView>
                <table>
                    <tr>
                        <td style="width: 98px; vertical-align: top;">Will you be?: </td>
                        <td>
                            <asp:RadioButton ID="purchasingRadioButton" Checked="true" Text="Requesting Orders" GroupName="role" runat="server" /><br />
                            <asp:RadioButton ID="approvingRadioButton" Text="Approving Orders" GroupName="role" runat="server" /><br />
                            <asp:RadioButton ID="adminsterRadioButton" Text="Administering Purchases" GroupName="role" runat="server" /><br />
                            <asp:RadioButton ID="managerRadioButton" Text="Viewing/Managing Purchases" GroupName="role" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2"><span class="section">Primary Shipping Location</span></td>
                    </tr>
                </table>
                <asp:MultiView ID="shiptoMultiView" runat="server">
                    <asp:View ID="View_ShipTo_List" runat="server">
                        <table cellpadding="0" cellspacing="0" class="request">
                            <tr>
                                <td colspan="2">
                                    <telerik:RadComboBox ID="shiptoRadComboBox" Width="400px" CausesValidation="false" OnClientSelectedIndexChanging="Changing" CollapseAnimation-Type="None" AutoPostBack="true" AppendDataBoundItems="true" DataTextField="address" DataValueField="id" runat="server" OnSelectedIndexChanged="shiptoRadComboBox_SelectedIndexChanged">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                        </Items>
                                    </telerik:RadComboBox>
                                    <asp:CompareValidator ID="CompareValidator1" Text="*" runat="server" ControlToValidate="shiptoRadComboBox" ValueToCompare="Select One..." Operator="NotEqual" ErrorMessage="Please select a primary shipping location."></asp:CompareValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                    <asp:View ID="View_ShipTo_New" runat="server">
                        <table cellpadding="0" cellspacing="0" class="request">
                            <tr>
                                <td style="width: 110px">Display Address: </td>
                                <td>
                                    <asp:TextBox ID="shiptoAddressTextBox" Width="200px" BackColor="LemonChiffon" MaxLength="256" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="addressRequiredFieldValidator" Text="*" runat="server" ControlToValidate="shiptoAddressTextBox" ErrorMessage="Please enter an address in the 'Display Address' field"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>Campus/Center: </td>
                                <td>
                                    <asp:TextBox ID="shiptoCampusTextBox" Width="200px" MaxLength="256" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>City: </td>
                                <td>
                                    <asp:TextBox ID="shiptoCityTextBox" Width="200px" BackColor="LemonChiffon" MaxLength="256" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" Text="*" ControlToValidate="shiptoCityTextBox" runat="server" ErrorMessage="Please enter a city"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>State: </td>
                                <td>
                                    <telerik:RadComboBox ID="shiptoStateRadComboBox" runat="server">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="Alabama" Value="AL" />
                                            <telerik:RadComboBoxItem Text="Alaska" Value="AK" />
                                            <telerik:RadComboBoxItem Text="Arizona" Value="AZ" />
                                            <telerik:RadComboBoxItem Text="Arkansas" Value="AR" />
                                            <telerik:RadComboBoxItem Text="California" Value="CA" />
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
                                <td>Street: </td>
                                <td>
                                    <asp:TextBox ID="shiptoStreetTextBox" BackColor="LemonChiffon" Width="200px" MaxLength="256" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="streetRequiredFieldValidator" ControlToValidate="shiptoStreetTextBox" Text="*" runat="server" ErrorMessage="Please enter a street"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>Building: </td>
                                <td>
                                    <asp:TextBox ID="shiptoBuildingTextBox" BackColor="LemonChiffon" Width="200px" MaxLength="64" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="shiptoBuildingRequiredFieldValidator" runat="server" Text="*" ControlToValidate="shiptoBuildingTextBox" ErrorMessage="Please enter a building"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>Room: </td>
                                <td>
                                    <asp:TextBox ID="shiptoRoomTextBox" BackColor="LemonChiffon" Width="200px" MaxLength="64" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="shiptoRoomRequiredFieldValidator" ControlToValidate="shiptoRoomTextBox" runat="server" Text="*" ErrorMessage="Please enter a room"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>Zip: </td>
                                <td>
                                    <telerik:RadNumericTextBox ID="shiptoZipRadNumericTextBox" BackColor="LemonChiffon" Width="40" Type="Number" NumberFormat-GroupSizes="5" Skin="" MaxLength="5" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" runat="server">
                                    </telerik:RadNumericTextBox>-
                                    <telerik:RadNumericTextBox ID="shiptoZipPlusRadNumericTextBox" BackColor="LemonChiffon" NumberFormat-GroupSizes="4" Width="30" Type="Number" Skin="" MaxLength="4" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" runat="server">
                                    </telerik:RadNumericTextBox>&nbsp;<a href="http://zip4.usps.com/zip4/welcome.jsp" style="vertical-align: middle; padding-bottom: 7px; font-size: 9px;" target="_blank">Lookup</a>
                                    <asp:RequiredFieldValidator ID="zipRequiredFieldValidator" Text="*" runat="server" ControlToValidate="shiptoZipRadNumericTextBox" ErrorMessage="Please enter the zip code of the address where you want shipments to be delivered."></asp:RequiredFieldValidator>
                                    <asp:RequiredFieldValidator ID="zipplusRequiredFieldValidator" Text="*" runat="server" ControlToValidate="shiptoZipPlusRadNumericTextBox" ErrorMessage="Please enter last four digits of the zip code. This information is important for calculating tax."></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td>Phone: </td>
                                <td>
                                    <telerik:RadMaskedTextBox ID="shiptoPhoneRadMaskedTextBox" BackColor="LemonChiffon" InvalidStyle-BackColor="Red" ClientEvents-OnFocus="OnFocus" runat="server" Mask="(###) ### - ####" ResetCaretOnFocus="true" Width="100px" Skin="">
                                    </telerik:RadMaskedTextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" Text="*" ControlToValidate="shiptoPhoneRadMaskedTextBox" runat="server" ErrorMessage="Please enter a phone number"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                </asp:MultiView>
                <table cellpadding="0" cellspacing="0" class="request">
                    <tr>
                        <td></td>
                        <td style="padding-top: 10px !important;">
                            <asp:Button ID="requestButton" runat="server" Text=" Submit " OnClick="requestButton_Click" />&nbsp;&nbsp; </td>
                    </tr>
                </table>
                <asp:ValidationSummary ID="registerValidationSummary" ShowMessageBox="true" ShowSummary="false" runat="server" />

                <script type="text/javascript" language="JavaScript">
                <!--
                    function OnFocus(sender, eventArgs) {
                        sender.set_caretPosition(1);
                    }

                    function Changing(sender, eventArgs) {
                        if (eventArgs.get_item().get_text() == "Other...")
                            eventArgs.set_cancel(false);
                        else {
                            sender.set_value(eventArgs.get_item().get_value());
                            sender.set_text(eventArgs.get_item().get_text());
                            eventArgs.set_cancel(true);
                        }
                    }
                    
                -->
                </script>

            </div>
        </asp:View>
        <asp:View ID="View_Confirm" runat="server">
            <div class="form">
                <asp:Label ID="confrimLabel" runat="server" />
                <br />
                <br />
                <asp:Button ID="backButton" runat="server" Text=" Back " Visible="false" OnClick="backButton_Click" />
                <telerik:RadFormDecorator ID="RadFormDecorator1" DecoratedControls="Buttons" runat="server" />
            </div>
        </asp:View>
        <asp:View ID="View_WaitingOnApproval" runat="server">
            <div class="form">
                <asp:Label ID="awaitingApprovalLabel" runat="server" />
            </div>
        </asp:View>
    </asp:MultiView>
    </form>
</body>
</html>
