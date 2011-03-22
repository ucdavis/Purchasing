<%@ Page Title="" Language="C#" MasterPageFile="~/master/default.master" AutoEventWireup="true" CodeFile="users.aspx.cs" Inherits="business_purchasing_admin_users" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content0" ContentPlaceHolderID="HeadContentPlaceHolder" runat="server">

    <script language="javascript" type="text/javascript">
    <!--
        function Changing(sender, eventArgs) {
            if (eventArgs.get_item().get_text() == "Other...")
                eventArgs.set_cancel(false);
            else {
                sender.set_value(eventArgs.get_item().get_value());
                sender.set_text(eventArgs.get_item().get_text());
                eventArgs.set_cancel(true);
            }
        }

        function OnFocus(sender, eventArgs) {
            sender.set_caretPosition(1);
        }
               
    //-->
    </script>

    <style type="text/css">
        table.white tr td { background-color: White !important; }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="LeftSidebarContentPlaceHolder" runat="Server">
    <!-- ********** Left sidebar area of the page ********** -->
    <div id="left_sidebar">
        <div id="level2_nav">
            <a name="nav"></a>
            <h2>In this section</h2>
            <asp:Menu ID="Menu3" runat="server">
            </asp:Menu>
        </div>
        <div class="other_links">
            <h2>Manage</h2>
            <ul>
                <li>
                    <asp:HyperLink ID="ordersHyperLink" NavigateUrl="default.aspx" runat="server">Orders</asp:HyperLink>
                </li>
                <li>
                    <asp:HyperLink ID="usersHyperLink" CssClass="here" NavigateUrl="users.aspx" runat="server">Users</asp:HyperLink>
                </li>
                <li>
                    <asp:HyperLink ID="vendorsHyperLink" NavigateUrl="vendors.aspx" runat="server">Vendors</asp:HyperLink>
                </li>
                <li>
                    <asp:HyperLink ID="addressesHyperLink" NavigateUrl="addresses.aspx" runat="server">Addresses</asp:HyperLink>
                </li>
            </ul>
        </div>
        <div class="other_links">
            <h2>Tasks</h2>
            <ul>
                <li>
                    <asp:LinkButton ID="showPendingLinkButton" CausesValidation="false" runat="server" OnClick="showPendingLinkButton_Click">Show Pending</asp:LinkButton>
                </li>
                <li>
                    <asp:LinkButton ID="addUserLinkButton" CausesValidation="false" runat="server" OnClick="addUserLinkButton_Click">Add User</asp:LinkButton>
                </li>
            </ul>
        </div>
    </div>
    <!-- ********** End left sidebar area of the page ********** -->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="Server">
    <!-- ********** Main content area of the page ********** -->
    <div id="formwrap">
        <h1>
            <asp:Label ID="sectionTitleLabel" runat="server" /></h1>
        <asp:MultiView ID="usersMultiView" runat="server">
            <asp:View ID="View_List" runat="server">
                <telerik:RadGrid ID="usersRadGrid" OnInit="usersRadGrid_Init" AllowFilteringByColumn="true" AutoGenerateColumns="false" runat="server" OnItemCommand="usersRadGrid_ItemCommand" OnItemDataBound="usersRadGrid_ItemDataBound" OnNeedDataSource="usersRadGrid_NeedDataSource">
                    <MasterTableView GroupLoadMode="Client" AllowSorting="true">
                        <Columns>
                            <telerik:GridTemplateColumn AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:ImageButton ID="deleteImageButton" CommandName="DeleteUser" CommandArgument='<% #Eval("UserName") %>' ToolTip="Delete this user" OnClientClick='return confirm("Are you sure you want to delete this user? WARNING: This will only remove the user from the purchasing roles, to delete the user from envnet completely, see System->Users");' ImageUrl="~/images/10/delete.gif" runat="server" />
                                    &nbsp;<asp:HyperLink ID="editHyperLink" ImageUrl="~/images/10/view.gif" NavigateUrl='<%# "users.aspx?username=" + Eval("UserName") %>' ToolTip="Edit this user" runat="server"></asp:HyperLink>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Access" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:HiddenField ID="userNameHiddenField" Value='<%#Eval("UserName") %>' runat="server" />
                                    <asp:CheckBox ID="approvedCheckBox" AutoPostBack="true" OnCheckedChanged="approvedCheckBox_CheckedChanged" runat="server" />
                                </ItemTemplate>
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Name" DataField="FullName" FilterControlWidth="150px">
                                <ItemTemplate>
                                    <%#Eval("LastName") %>,
                                    <%#Eval("FirstName") %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Phone" AllowFiltering="false">
                                <ItemTemplate>
                                    <%#GetPhoneFromProfile(Eval("UserName")) %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="E-mail" AllowFiltering="false">
                                <ItemTemplate>
                                    <a href="mailto:<%#Eval("Email") %>">
                                        <%#Eval("Email") %></a>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="CreateDate" HtmlEncode="false" DataFormatString="{0:M/d/yy h:mm tt}" HeaderText="Created" />
                            <telerik:GridTemplateColumn HeaderText="Out of Office" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:HiddenField ID="userNameHiddenField2" Value='<%#Eval("UserName") %>' runat="server" />
                                    <asp:CheckBox ID="outOfOfficeCheckBox" AutoPostBack="true" OnCheckedChanged="outOfOfficeCheckBox_CheckedChanged" runat="server" />
                                </ItemTemplate>
                                <ItemStyle Width="85px" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <GroupByExpressions>
                            <telerik:GridGroupByExpression>
                                <SelectFields>
                                    <telerik:GridGroupByField FieldName="RoleName" HeaderText=" " HeaderValueSeparator="" />
                                </SelectFields>
                                <GroupByFields>
                                    <telerik:GridGroupByField FieldName="RoleName" HeaderText=" " HeaderValueSeparator="" />
                                </GroupByFields>
                            </telerik:GridGroupByExpression>
                        </GroupByExpressions>
                    </MasterTableView>
                </telerik:RadGrid>
            </asp:View>
            <asp:View ID="View_AddEdit" runat="server">
                <h2>User Information</h2>
                <br />
                <table class="formtable bdr" cellpadding="0" cellspacing="0">
                    <tr class="alt">
                        <td style="width: 115px">Out of office?</td>
                        <td>
                            <asp:CheckBox ID="awayCheckBox" AutoPostBack="true" OnCheckedChanged="awayCheckBox_CheckedChanged" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>Username:</td>
                        <td>
                            <asp:TextBox ID="useridTextBox" BackColor="LemonChiffon" Width="200" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="useridRequiredFieldValidator" Text="*" runat="server" ControlToValidate="useridTextBox" ErrorMessage="Please enter a kerberos login ID"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>First name: </td>
                        <td>
                            <asp:TextBox ID="fnameTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="fnameRequiredFieldValidator" Text="*" runat="server" ControlToValidate="fnameTextBox" ErrorMessage="Please enter the user's first name."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>Last name: </td>
                        <td>
                            <asp:TextBox ID="lnameTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="lnameRequiredFieldValidator" Text="*" runat="server" ControlToValidate="lnameTextBox" ErrorMessage="Please enter the user's last name."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>UCD E-mail: </td>
                        <td>
                            <asp:TextBox ID="emailTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="emailRequiredFieldValidator" Text="*" runat="server" ControlToValidate="emailTextBox" ErrorMessage="Please enter the user's UC Davis e-mail address."></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="emailRegularExpressionValidator" Text="*" runat="server" ControlToValidate="emailTextBox" ErrorMessage="Please enter a valid e-mail address (EX: csmith@ucdavis.edu)" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>Contact phone: </td>
                        <td>
                            <telerik:RadMaskedTextBox ID="phoneRadMaskedTextBox" BackColor="LemonChiffon" InvalidStyle-BackColor="Red" ClientEvents-OnFocus="OnFocus" runat="server" Mask="(###) ### - ####" ResetCaretOnFocus="true" Width="100px" EnableEmbeddedSkins="false">
                            </telerik:RadMaskedTextBox>
                            <asp:RequiredFieldValidator ID="phoneRequiredFieldValidator" Text="*" ControlToValidate="phoneRadMaskedTextBox" runat="server" ErrorMessage="Please enter a phone number"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>Unit: </td>
                        <td>
                            <asp:MultiView ID="unitsMultiView" runat="server">
                                <asp:View ID="View_Unit_List" runat="server">
                                    <telerik:RadComboBox ID="unitsRadComboBox" Width="202px" CausesValidation="false" OnClientSelectedIndexChanging="Changing" CollapseAnimation-Type="None" AutoPostBack="true" AppendDataBoundItems="true" DataTextField="unit_abrv" DataValueField="id" runat="server" OnSelectedIndexChanged="unitsRadComboBox_SelectedIndexChanged">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                        </Items>
                                    </telerik:RadComboBox>
                                    <asp:CompareValidator ID="CompareValidator2" Text="*" runat="server" ControlToValidate="unitsRadComboBox" ValueToCompare="Select One..." Operator="NotEqual" ErrorMessage="Please select the unit where you work."></asp:CompareValidator>
                                </asp:View>
                                <asp:View ID="View_Unit_New" runat="server">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td style="width: 110px">Unit Name: </td>
                                            <td>
                                                <asp:TextBox ID="unitNameTextBox" BackColor="LemonChiffon" MaxLength="256" Width="200" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" Text="*" ControlToValidate="unitNameTextBox" runat="server" ErrorMessage="Please enter a unit name EX: Road Ecology Center"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Unit Abbreviation: </td>
                                            <td>
                                                <asp:TextBox ID="unitAbrvTextBox" BackColor="LemonChiffon" MaxLength="64" Width="200" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" Text="*" ControlToValidate="unitAbrvTextBox" runat="server" ErrorMessage="Please enter a unit abbreviation EX: TERC"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>FAX Number: </td>
                                            <td>
                                                <telerik:RadMaskedTextBox ID="unitFaxRadMaskedTextBox" InvalidStyle-BackColor="Red" ClientEvents-OnFocus="OnFocus" runat="server" Mask="(###) ### - ####" ResetCaretOnFocus="true" Width="100px" EnableEmbeddedSkins="false">
                                                </telerik:RadMaskedTextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:View>
                            </asp:MultiView>
                        </td>
                    </tr>
                    <tr>
                        <td>Role: </td>
                        <td>
                            <asp:RadioButton ID="purchaserRadioButton" Checked="true" Text="Requester" GroupName="role" runat="server" />
                            <asp:RadioButton ID="approverRadioButton" Text="Approver" GroupName="role" runat="server" />
                            <asp:RadioButton ID="adminRadioButton" Text="Purchaser" GroupName="role" runat="server" />
                            <asp:RadioButton ID="managerRadioButton" Text="Manager" GroupName="role" runat="server" />
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>Default shipping address: </td>
                        <td>
                            <asp:MultiView ID="shiptoMultiView" runat="server">
                                <asp:View ID="View_ShipTo_List" runat="server">
                                    <telerik:RadComboBox ID="shiptoRadComboBox" Width="400px" CausesValidation="false" OnClientSelectedIndexChanging="Changing" CollapseAnimation-Type="None" AutoPostBack="true" AppendDataBoundItems="true" DataTextField="address" DataValueField="id" runat="server" OnSelectedIndexChanged="shiptoRadComboBox_SelectedIndexChanged">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                        </Items>
                                    </telerik:RadComboBox>
                                    <asp:CompareValidator ID="CompareValidator1" ControlToValidate="shiptoRadComboBox" Text="*" ValueToCompare="Select One..." Operator="NotEqual" runat="server" ErrorMessage="Please select a default shipping address"></asp:CompareValidator>
                                </asp:View>
                                <asp:View ID="View_ShipTo_New" runat="server">
                                    <table cellspacing="5">
                                        <tr>
                                            <td style="width: 115px">Display Address: </td>
                                            <td>
                                                <asp:TextBox ID="shiptoDisplayAddressTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="addressRequiredFieldValidator" Text="*" runat="server" ControlToValidate="shiptoDisplayAddressTextBox" ErrorMessage="Please enter a full address in the 'Display Address' field."></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr class="alt">
                                            <td style="width: 100px">Campus/Center: </td>
                                            <td>
                                                <asp:TextBox ID="shiptoCampusTextBox" Width="200px" runat="server"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px">City: </td>
                                            <td>
                                                <asp:TextBox ID="shiptoCityTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" Text="*" ControlToValidate="shiptoCityTextBox" runat="server" ErrorMessage="Please enter a city"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr class="alt">
                                            <td style="width: 100px">State: </td>
                                            <td>
                                                <asp:DropDownList ID="shiptoStateDropDownList" runat="server">
                                                    <asp:ListItem Text="Select One..." Value="Select One..." />
                                                    <asp:ListItem Text="Alabama" Value="AL" />
                                                    <asp:ListItem Text="Alaska" Value="AK" />
                                                    <asp:ListItem Text="Arizona" Value="AZ" />
                                                    <asp:ListItem Text="Arkansas" Value="AR" />
                                                    <asp:ListItem Text="California" Value="CA" />
                                                    <asp:ListItem Text="Colorado" Value="CO" />
                                                    <asp:ListItem Text="Connecticut" Value="CT" />
                                                    <asp:ListItem Text="Delaware" Value="DE" />
                                                    <asp:ListItem Text="Florida" Value="FL" />
                                                    <asp:ListItem Text="Georgia" Value="GA" />
                                                    <asp:ListItem Text="Hawaii" Value="HI" />
                                                    <asp:ListItem Text="Idaho" Value="ID" />
                                                    <asp:ListItem Text="Illinois" Value="IL" />
                                                    <asp:ListItem Text="Indiana" Value="IN" />
                                                    <asp:ListItem Text="Iowa" Value="IA" />
                                                    <asp:ListItem Text="Kansas" Value="KS" />
                                                    <asp:ListItem Text="Kentucky" Value="KY" />
                                                    <asp:ListItem Text="Louisiana" Value="LA" />
                                                    <asp:ListItem Text="Maine" Value="ME" />
                                                    <asp:ListItem Text="Maryland" Value="MD" />
                                                    <asp:ListItem Text="Massachusetts" Value="MA" />
                                                    <asp:ListItem Text="Michigan" Value="MI" />
                                                    <asp:ListItem Text="Minnesota" Value="MN" />
                                                    <asp:ListItem Text="Mississippi" Value="MS" />
                                                    <asp:ListItem Text="Missouri" Value="MO" />
                                                    <asp:ListItem Text="Montana" Value="MT" />
                                                    <asp:ListItem Text="Nebraska" Value="NE" />
                                                    <asp:ListItem Text="Nevada" Value="NV" />
                                                    <asp:ListItem Text="New Hampshire" Value="NH" />
                                                    <asp:ListItem Text="New Jersey" Value="NJ" />
                                                    <asp:ListItem Text="New Mexico" Value="NM" />
                                                    <asp:ListItem Text="New York" Value="NY" />
                                                    <asp:ListItem Text="North Carolina" Value="NC" />
                                                    <asp:ListItem Text="North Dakota" Value="ND" />
                                                    <asp:ListItem Text="Ohio" Value="OH" />
                                                    <asp:ListItem Text="Oklahoma" Value="OK" />
                                                    <asp:ListItem Text="Oregon" Value="OR" />
                                                    <asp:ListItem Text="Pennsylvania" Value="PA" />
                                                    <asp:ListItem Text="Rhode Island" Value="RI" />
                                                    <asp:ListItem Text="South Carolina" Value="SC" />
                                                    <asp:ListItem Text="South Dakota" Value="SD" />
                                                    <asp:ListItem Text="Tennessee" Value="TN" />
                                                    <asp:ListItem Text="Texas" Value="TX" />
                                                    <asp:ListItem Text="Utah" Value="UT" />
                                                    <asp:ListItem Text="Vermont" Value="VT" />
                                                    <asp:ListItem Text="Virginia" Value="VA" />
                                                    <asp:ListItem Text="Washington" Value="WA" />
                                                    <asp:ListItem Text="West Virginia" Value="WV" />
                                                    <asp:ListItem Text="Wisconsin" Value="WI" />
                                                    <asp:ListItem Text="Wyoming" Value="WY" />
                                                </asp:DropDownList>
                                                <asp:CompareValidator ID="stateCompareValidator" ControlToValidate="shiptoStateDropDownList" Operator="NotEqual" ValueToCompare="Select One..." Text="*" runat="server" ErrorMessage="Please enter a state"></asp:CompareValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px">Street: </td>
                                            <td>
                                                <asp:TextBox ID="shiptoStreetTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="streetRequiredFieldValidator" Text="*" ControlToValidate="shiptoStreetTextBox" runat="server" ErrorMessage="Please enter a street"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr class="alt">
                                            <td>Building: </td>
                                            <td>
                                                <asp:TextBox ID="shiptoBuildingTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="buildingRequiredFieldValidator" Text="*" ControlToValidate="shiptoBuildingTextBox" runat="server" ErrorMessage="Please enter a building"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Room: </td>
                                            <td>
                                                <asp:TextBox ID="shiptoRoomTextBox" BackColor="LemonChiffon" Width="200px" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="roomRequiredFieldValidator" Text="*" ControlToValidate="shiptoRoomTextBox" runat="server" ErrorMessage="Please enter a room number"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr class="alt">
                                            <td>Zip: </td>
                                            <td>
                                                <telerik:RadNumericTextBox ID="shiptoZipRadNumericTextBox" BackColor="LemonChiffon" Width="40" Type="Number" NumberFormat-GroupSizes="5" EnableEmbeddedSkins="false" MaxLength="5" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" runat="server">
                                                </telerik:RadNumericTextBox>
                                                -
                                                <telerik:RadNumericTextBox ID="shiptoZipPlusRadNumericTextBox" BackColor="LemonChiffon" NumberFormat-GroupSizes="4" Width="30" Type="Number" EnableEmbeddedSkins="false" MaxLength="4" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" runat="server">
                                                </telerik:RadNumericTextBox>
                                                &nbsp;<a href="http://zip4.usps.com/zip4/welcome.jsp" style="vertical-align: middle; padding-bottom: 7px; font-size: 9px;" target="_blank">Lookup</a>
                                                <asp:RequiredFieldValidator ID="zipRequiredFieldValidator" Text="*" runat="server" ControlToValidate="shiptoZipRadNumericTextBox" ErrorMessage="Please enter the zip code of the address where the user wants shipments to be delivered."></asp:RequiredFieldValidator>
                                                <asp:RequiredFieldValidator ID="zipplusRequiredFieldValidator" Text="*" runat="server" ControlToValidate="shiptoZipPlusRadNumericTextBox" ErrorMessage="Please enter last four digits of the zip code. This information is important for calculating tax."></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Phone: </td>
                                            <td>
                                                <telerik:RadMaskedTextBox ID="shiptoPhoneRadMaskedTextBox" BackColor="LemonChiffon" InvalidStyle-BackColor="Red" ClientEvents-OnFocus="OnFocus" runat="server" Mask="(###) ### - ####" ResetCaretOnFocus="true" Width="100px" EnableEmbeddedSkins="false">
                                                </telerik:RadMaskedTextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" Text="*" ControlToValidate="shiptoPhoneRadMaskedTextBox" runat="server" ErrorMessage="Please enter a phone number"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:View>
                            </asp:MultiView>
                        </td>
                    </tr>
                </table>
                <div class="tableButtons">
                    <asp:Button ID="insertUpdateUserButton" runat="server" Text=" Submit " OnClick="insertUpdateUserButton_Click" />
                    <asp:Button ID="cancelUserButton" runat="server" CausesValidation="false" OnClick="cancelUserButton_Click" Visible="false" Text=" Cancel " />
                </div>
                <asp:ValidationSummary ID="registerValidationSummary" ShowMessageBox="true" ShowSummary="false" runat="server" />
                <asp:Panel ID="customerNumbersPanel" runat="server">
                    <h2>Customer Numbers</h2>
                    <br />
                    <asp:GridView ID="customerNumbersGridView" AutoGenerateColumns="false" ShowHeader="false" CssClass="formtable bdr" GridLines="None" EmptyDataText="There are no customer numbers on file." AlternatingRowStyle-CssClass="alt" runat="server">
                        <Columns>
                            <asp:BoundField DataField="vendor_name" />
                            <asp:BoundField DataField="customer_num" />
                        </Columns>
                    </asp:GridView>
                </asp:Panel>
                <asp:Panel ID="backupPanel" runat="server">
                    <h2>Backup Users</h2>
                    <br />
                    <p>A backup user will be copied with all of your notification e-mails while you are away.<br />
                        To add a backup user, select their name from the list and click "Add".</p>
                    <asp:UpdatePanel ID="backupUsersUpdatePanel" runat="server">
                        <ContentTemplate>
                            <telerik:RadComboBox ID="backupRadComboBox" Width="200px" Height="400px" AppendDataBoundItems="true" runat="server">
                                <Items>
                                    <telerik:RadComboBoxItem Text="Select a user..." Value="Select a user..." />
                                </Items>
                            </telerik:RadComboBox>
                            <asp:Button ID="addBackupButton" CausesValidation="true" OnClick="addBackupButton_Click" ValidationGroup="backup" runat="server" Text=" Add " />
                            <asp:CompareValidator ID="CompareValidator3" ValidationGroup="backup" ControlToValidate="backupRadComboBox" ValueToCompare="Select a user..." Operator="NotEqual" runat="server" ErrorMessage="Please select a backup user"></asp:CompareValidator>
                            <br />
                            <br />
                            <asp:Repeater ID="backupRepeater" runat="server">
                                <HeaderTemplate>
                                    <table class="formtable bdr">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td style="width: 20px; text-align: center;">
                                            <asp:ImageButton CausesValidation="false" CommandArgument='<%# Eval("id") %>' ImageUrl="~/images/10/delete.gif" ToolTip="Remove this backup user" OnClick="deleteBackupImageButton_Click" ID="deleteBackupImageButton" runat="server" />
                                        </td>
                                        <td>
                                            <%# GetFullName(Eval("backup_profile_id")) %>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr class="alt">
                                        <td style="width: 20px; text-align: center;">
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
                <asp:Label ID="confirmLabel" ForeColor="Green" runat="server" />
            </asp:View>
        </asp:MultiView>
    </div>
    <!-- ********** End main content area of the page ********** -->
</asp:Content>
