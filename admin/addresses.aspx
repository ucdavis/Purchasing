<%@ Page Title="" Language="C#" MasterPageFile="~/master/default.master" AutoEventWireup="true" CodeFile="addresses.aspx.cs" Inherits="business_purchasing_admin_addresses" %>

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
                    <asp:HyperLink ID="usersHyperLink" NavigateUrl="users.aspx" runat="server">Users</asp:HyperLink>
                </li>
                <li>
                    <asp:HyperLink ID="vendorsHyperLink" NavigateUrl="vendors.aspx" runat="server">Vendors</asp:HyperLink>
                </li>
                <li>
                    <asp:HyperLink ID="addressesHyperLink" CssClass="here" NavigateUrl="addresses.aspx" runat="server">Addresses</asp:HyperLink>
                </li>
            </ul>
        </div>
        <div class="other_links">
            <h2>Tasks</h2>
            <ul>
                <li>
                    <asp:LinkButton ID="addAddressLinkButton" runat="server" CausesValidation="false" OnClick="addAddressLinkButton_Click">Add Address</asp:LinkButton></li>
            </ul>
        </div>
    </div>
    <!-- ********** End left sidebar area of the page ********** -->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="Server">
    <!-- ********** Main content area of the page ********** -->

    <script language="javascript" type="text/javascript">
    <!--
        function OnFocus(sender, eventArgs) {
            sender.set_caretPosition(1);
        }    
    //-->
    </script>

    <div id="formwrap">
        <h1>
            <asp:Label ID="sectionTitleLabel" runat="server" /></h1>
        <div class="fixfc">
            <!-- this is necessary to fix a problem with multiple right floats in IE 6 -->
        </div>
        <asp:MultiView ID="addressesMultiView" runat="server">
            <asp:View ID="View_List" runat="server">
                <telerik:RadGrid ID="addressRadGrid" AutoGenerateColumns="false" OnNeedDataSource="addressRadGrid_NeedDataSource" OnItemCommand="addressRadGrid_ItemCommand" runat="server">
                    <MasterTableView NoMasterRecordsText="There are no addresses at this time.">
                        <Columns>
                            <telerik:GridTemplateColumn>
                                <ItemTemplate>
                                    <asp:ImageButton ID="addressDeleteImageButton" CommandName="DeleteAddress" CommandArgument='<%#Eval("id") %>' ToolTip="Delete this vendor" OnClientClick='return confirm("Are you sure you want to delete this address?");' ImageUrl="~/images/10/delete.gif" runat="server" />
                                    <asp:ImageButton ID="addressEditImageButton" CommandName="EditAddress" CommandArgument='<%#Eval("id") %>' ToolTip="Edit this address" ImageUrl="~/images/10/edit.gif" runat="server" />
                                </ItemTemplate>
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="address" HeaderText="Address" SortExpression="address">
                            </telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </asp:View>
            <asp:View ID="View_AddEdit" runat="server">
                <table cellpadding="0" cellspacing="0" class="formtable bdr">
                    <tr>
                        <td style="width: 120px">Display Address: </td>
                        <td>
                            <asp:TextBox ID="addAddressTextBox" Width="200px" BackColor="LemonChiffon" MaxLength="256" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" Text="*" runat="server" ControlToValidate="addAddressTextBox" ErrorMessage="Please enter an address in the 'Display Address' field"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>Campus/Center: </td>
                        <td>
                            <asp:TextBox ID="addCampusTextBox" Width="200px" MaxLength="256" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>City: </td>
                        <td>
                            <asp:TextBox ID="addCityTextBox" Width="200px" BackColor="LemonChiffon" MaxLength="256" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="addCityTextBox" Text="*" runat="server" ErrorMessage="Please enter a city"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>State: </td>
                        <td>
                            <asp:DropDownList ID="addStateDropDownList" BackColor="LemonChiffon" runat="server">
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
                            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="addStateDropDownList" ErrorMessage="Please select a state." Text="*" ValueToCompare="Select One..." Operator="NotEqual"></asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>Street: </td>
                        <td>
                            <asp:TextBox ID="addStreetTextBox" BackColor="LemonChiffon" Width="200px" MaxLength="256" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="streetRequiredFieldValidator" Text="*" ControlToValidate="addStreetTextBox" runat="server" ErrorMessage="Please enter a street"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>Building: </td>
                        <td>
                            <asp:TextBox ID="addBuildingTextBox" BackColor="LemonChiffon" Width="200px" MaxLength="64" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="buildingRequiredFieldValidator" Text="*" ControlToValidate="addBuildingTextBox" runat="server" ErrorMessage="Please enter a building"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>Room: </td>
                        <td>
                            <asp:TextBox ID="addRoomTextBox" BackColor="LemonChiffon" Width="200px" MaxLength="64" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="roomRequiredFieldValidator" Text="*" ControlToValidate="addRoomTextBox" runat="server" ErrorMessage="Please enter a room"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>Zip: </td>
                        <td>
                            <telerik:RadNumericTextBox ID="addZipRadNumericTextBox" BackColor="LemonChiffon" Width="40" Type="Number" NumberFormat-GroupSizes="5" EnableEmbeddedSkins="false" MaxLength="5" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" runat="server">
                            </telerik:RadNumericTextBox>-
                            <telerik:RadNumericTextBox ID="addZipPlusRadNumericTextBox" BackColor="LemonChiffon" NumberFormat-GroupSizes="4" Width="30" Type="Number" EnableEmbeddedSkins="false" MaxLength="4" NumberFormat-DecimalDigits="0" InvalidStyle-BackColor="Red" runat="server">
                            </telerik:RadNumericTextBox>&nbsp;<a href="http://zip4.usps.com/zip4/welcome.jsp" style="vertical-align: middle; padding-bottom: 7px; font-size: 9px;" target="_blank">Lookup</a>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" Text="*" runat="server" ControlToValidate="addZipRadNumericTextBox" ErrorMessage="Please enter the zip code of the address where you want shipments to be delivered."></asp:RequiredFieldValidator>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator9" Text="*" runat="server" ControlToValidate="addZipPlusRadNumericTextBox" ErrorMessage="Please enter last four digits of the zip code. This information is important for calculating tax."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>Phone: </td>
                        <td>
                            <telerik:RadMaskedTextBox ID="addPhoneRadMaskedTextBox" BackColor="LemonChiffon" InvalidStyle-BackColor="Red" ClientEvents-OnFocus="OnFocus" runat="server" Mask="(###) ### - ####" ResetCaretOnFocus="true" Width="100px" EnableEmbeddedSkins="false">
                            </telerik:RadMaskedTextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="addPhoneRadMaskedTextBox" runat="server" Text="*" ErrorMessage="Please enter a phone number."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
                <div class="tableButtons">
                    <asp:MultiView ID="commandMultiView" runat="server">
                        <asp:View ID="View_Command_Add" runat="server">
                            <asp:Button ID="addButton" runat="server" Text="Add" OnClick="addButton_Click" />
                        </asp:View>
                        <asp:View ID="View_Command_Update" runat="server">
                            <asp:Button ID="updateButton" runat="server" Text="Update" OnClick="updateButton_Click" />
                        </asp:View>
                    </asp:MultiView>
                    &nbsp;<asp:Button ID="cancelButton" CausesValidation="false" runat="server" Text="Cancel" OnClick="cancelButton_Click" />
                </div>
                <asp:ValidationSummary ID="ValidationSummary1" ShowMessageBox="true" ShowSummary="false" runat="server" />
            </asp:View>
            <asp:View ID="View_Confirm" runat="server">
                <asp:Label ID="confirmLabel" ForeColor="Green" runat="server" />
            </asp:View>
        </asp:MultiView>
    </div>
    <!-- ********** End main content area of the page ********** -->
</asp:Content>
