<%@ Page Title="" Language="C#" MasterPageFile="~/master/default.master" AutoEventWireup="true" CodeFile="myprofile.aspx.cs" Inherits="business_purchasing_orders_myprofile" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content0" runat="Server" ContentPlaceHolderID="HeadContentPlaceHolder">

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
    //-->
    </script>

</asp:Content>
<asp:Content ID="Content1" runat="Server" ContentPlaceHolderID="LeftSidebarContentPlaceHolder">
    <!-- ********** Left sidebar area of the page ********** -->
    <div id="left_sidebar">
        <div id="level2_nav">
            <a name="nav"></a>
            <h2>In this section</h2>
            <asp:Menu ID="Menu3" runat="server">
            </asp:Menu>
        </div>
    </div>
    <!-- ********** End left sidebar area of the page ********** -->
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="MainContentPlaceHolder">
    <!-- ********** Main content area of the page ********** -->
    <telerik:RadFormDecorator ID="RadFormDecorator1" DecoratedControls="All" runat="server" />
    <div id="formwrap">
        <h1>
            <asp:Label ID="sectionTitleLabel" runat="server" Text="My Profile"></asp:Label></h1>
        <asp:MultiView ID="profileMultiView" runat="server">
            <asp:View ID="View_Profile" runat="server">
                <table class="formtable bdr" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="width: 110px">Username:</td>
                        <td>
                            <asp:TextBox ID="useridTextBox" ReadOnly="true" BackColor="#EEEEEE" ForeColor="#777777" Width="200" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr class="alt">
                        <td>First Name: </td>
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
                    <tr class="alt">
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
                    <tr class="alt">
                        <td>Unit: </td>
                        <td>
                            <telerik:RadComboBox ID="unitsRadComboBox" Width="202px" CausesValidation="false" CollapseAnimation-Type="None" AppendDataBoundItems="true" DataTextField="unit_abrv" DataValueField="id" runat="server">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                </Items>
                            </telerik:RadComboBox>
                            <asp:CompareValidator ID="unitsCompareValidator" Text="*" runat="server" ControlToValidate="unitsRadComboBox" ValueToCompare="Select One..." Operator="NotEqual" ErrorMessage="Please select the unit where you work."></asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>Primary Shipping Location: </td>
                        <td>
                            <asp:MultiView ID="shiptoMultiView" runat="server">
                                <asp:View ID="View_ShipTo_List" runat="server">
                                    <telerik:RadComboBox ID="shiptoRadComboBox" Width="400px" CausesValidation="false" OnClientSelectedIndexChanging="Changing" CollapseAnimation-Type="None" AutoPostBack="true" AppendDataBoundItems="true" DataTextField="address" DataValueField="id" runat="server" OnSelectedIndexChanged="shiptoRadComboBox_SelectedIndexChanged">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                        </Items>
                                    </telerik:RadComboBox>
                                    <asp:CompareValidator ID="CompareValidator6" ControlToValidate="shiptoRadComboBox" Display="None" ValueToCompare="Select One..." Operator="NotEqual" runat="server" ErrorMessage="Please select a default shipping address"></asp:CompareValidator>
                                </asp:View>
                                <asp:View ID="View_ShipTo_New" runat="server">
                                    <table cellspacing="5">
                                        <tr>
                                            <td style="width: 110px">Display Address / Location Name: </td>
                                            <td>
                                                <asp:TextBox ID="shiptoAddressTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="addressRequiredFieldValidator" Text="*" runat="server" ControlToValidate="shiptoAddressTextBox" ErrorMessage="Please enter an address in the 'Display Address' field where you want shipments to be delivered."></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Campus: </td>
                                            <td>
                                                <asp:TextBox ID="shiptoCampusTextBox" Width="200px" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>City: </td>
                                            <td>
                                                <asp:TextBox ID="shiptoCityTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="cityRequiredFieldValidator" Text="*" ControlToValidate="shiptoCityTextBox" runat="server" ErrorMessage="Please enter a city"></asp:RequiredFieldValidator>
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
                                                <asp:TextBox ID="shiptoStreetTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="streetRequiredFieldValidator" Text="*" ControlToValidate="shiptoStreetTextBox" runat="server" ErrorMessage="Please enter a street"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Building: </td>
                                            <td>
                                                <asp:TextBox ID="shiptoBuildingTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="buildingRequiredFieldValidator" ControlToValidate="shiptoBuildingTextBox" Text="*" runat="server" ErrorMessage="Please enter a building"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Room: </td>
                                            <td>
                                                <asp:TextBox ID="shiptoRoomTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="roomRequiredFieldValidator" ControlToValidate="shiptoRoomTextBox" Text="*" runat="server" ErrorMessage="Please enter a room"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Zip: </td>
                                            <td>
                                                <telerik:RadNumericTextBox ID="shiptoZipRadNumericTextBox" BackColor="LemonChiffon" Width="40" Type="Number" NumberFormat-GroupSizes="5" Skin="" MaxLength="5" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" runat="server">
                                                </telerik:RadNumericTextBox>-
                                                <telerik:RadNumericTextBox ID="shiptoZipPlusRadNumericTextBox" BackColor="LemonChiffon" NumberFormat-GroupSizes="4" Width="30" Type="Number" Skin="" MaxLength="4" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" runat="server">
                                                </telerik:RadNumericTextBox>&nbsp;<a href="http://zip4.usps.com/zip4/welcome.jsp" style="vertical-align: middle; padding-bottom: 7px; font-size: 9px;" target="_blank">Lookup</a><asp:RequiredFieldValidator ID="zipRequiredFieldValidator" Text="*" runat="server" ControlToValidate="shiptoZipRadNumericTextBox" ErrorMessage="Please enter the zip code of the address where you want shipments to be delivered."></asp:RequiredFieldValidator>
                                                <asp:RequiredFieldValidator ID="zipplusRequiredFieldValidator" Text="*" runat="server" ControlToValidate="shiptoZipPlusRadNumericTextBox" ErrorMessage="Please enter last four digits of the zip code. This information is important for calculating tax."></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Phone: </td>
                                            <td>
                                                <telerik:RadMaskedTextBox ID="shiptoPhoneRadMaskedTextBox" Skin="" BackColor="LemonChiffon" InvalidStyle-BackColor="Red" ClientEvents-OnFocus="OnFocus" runat="server" Mask="(###) ### - ####" ResetCaretOnFocus="true" Width="100px">
                                                </telerik:RadMaskedTextBox>
                                                <asp:RequiredFieldValidator ID="shiptoPhoneRequiredFieldValidator" runat="server" Text="*" ControlToValidate="shiptoPhoneRadMaskedTextBox" ErrorMessage="Please enter a contact phone for this location"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:View>
                            </asp:MultiView>
                        </td>
                    </tr>
                </table>
                <div class="tableButtons ctrls">
                    <asp:LinkButton ID="updateUserButton" runat="server" OnClick="updateUserButton_Click">
                    <img src="../../../images/15/check.gif" alt="" />Update Profile
                    </asp:LinkButton>
                </div>
                <br class="clear" />
                <br />
                <asp:ValidationSummary ID="profileValidationSummary" ShowMessageBox="true" ShowSummary="false" runat="server" />
                <asp:Panel ID="backupPanel" runat="server">
                    <h2>Backup Users</h2>
                    <br />
                    <p>Backup users are copied with all of your notification e-mails while you are away.<br />
                        To add a backup user, select their name from the list and click "Add".</p>
                    <asp:UpdatePanel ID="backupUsersUpdatePanel" runat="server">
                        <ContentTemplate>
                            <telerik:RadComboBox ID="backupRadComboBox" Width="200px" Height="400px" AppendDataBoundItems="true" runat="server">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Select a user..." Value="Select a user..." />
                                </Items>
                            </telerik:RadComboBox>
                            <asp:Button ID="addBackupButton" CausesValidation="true" OnClick="addBackupButton_Click" ValidationGroup="backup" runat="server" Text="Add" />
                            <asp:CompareValidator ID="CompareValidator3" ValidationGroup="backup" ControlToValidate="backupRadComboBox" ValueToCompare="Select a user..." Operator="NotEqual" runat="server" ErrorMessage="&nbsp;&nbsp;Please select a backup user"></asp:CompareValidator>
                            <br />
                            <br />
                            <asp:Repeater ID="backupRepeater" runat="server">
                                <HeaderTemplate>
                                    <table class="formtable bdr">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td style="width: 20px; padding: 5px; text-align: center;">
                                            <asp:ImageButton CausesValidation="false" CommandArgument='<%# Eval("id") %>' ImageUrl="~/images/10/delete.gif" ToolTip="Remove this backup user" OnClick="deleteBackupImageButton_Click" ID="deleteBackupImageButton" runat="server" />
                                        </td>
                                        <td>
                                            <%# GetFullName(Eval("backup_profile_id")) %>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr class="alt">
                                        <td style="width: 20px; padding: 5px; text-align: center;">
                                            <asp:ImageButton CausesValidation="false" CommandArgument='<%# Eval("id") %>' ImageUrl="~/images/10/delete.gif" ToolTip="Remove this backup user" OnClick="deleteBackupImageButton_Click" ID="deleteBackupImageButton" runat="server" />
                                        </td>
                                        <td>
                                            <%# GetFullName(Eval("backup_profile_id")) %>
                                        </td>
                                    </tr>
                                </AlternatingItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
            </asp:View>
            <asp:View ID="View_Confirm" runat="server">
                <asp:Label ID="confirmLabel" ForeColor="#5e9734" runat="server" />
            </asp:View>
        </asp:MultiView>
    </div>
    <!-- ********** End main content area of the page ********** -->
</asp:Content>
