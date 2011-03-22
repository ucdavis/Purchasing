<%@ Page Title="" Language="C#" MasterPageFile="~/master/default.master" AutoEventWireup="true" CodeFile="vendors.aspx.cs" Inherits="business_purchasing_admin_vendors" %>

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
                    <asp:HyperLink ID="vendorsHyperLink" CssClass="here" NavigateUrl="vendors.aspx" runat="server">Vendors</asp:HyperLink>
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
                    <asp:LinkButton ID="addLinkButton" CausesValidation="false" runat="server" OnClick="addLinkButton_Click">Add Vendor</asp:LinkButton>
                </li>
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
        <asp:MultiView ID="vendorMultiView" runat="server">
            <asp:View ID="View_List" runat="server">
                <telerik:RadGrid ID="vendorRadGrid" AutoGenerateColumns="false" AllowSorting="true" OnNeedDataSource="vendorRadGrid_NeedDataSource" OnItemCommand="vendorRadGrid_ItemCommand" runat="server">
                    <MasterTableView NoMasterRecordsText="No vendors to display" GroupLoadMode="Client">
                        <Columns>
                            <telerik:GridTemplateColumn>
                                <ItemTemplate>
                                    <asp:ImageButton ID="deleteImageButton" CommandName="DeleteVendor" CommandArgument='<%# Eval("id") %>' ToolTip="Delete this vendor" OnClientClick='return confirm("Are you sure you want to delete this vendor?");' ImageUrl="~/images/10/delete.gif" runat="server" />
                                    <asp:ImageButton ID="editImageButton" CommandName="EditVendor" CommandArgument='<%# Eval("id") %>' ToolTip="Edit this vendor" ImageUrl="~/images/10/edit.gif" runat="server" />
                                </ItemTemplate>
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="vendor_name" HeaderText="Vendor Name" SortExpression="vendor_name">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn DataField="vendor_address" HeaderText="Address" SortExpression="vendor_address">
                                <ItemTemplate>
                                    <%#TrimAddress(Eval("vendor_address"), Eval("vendor_city"), Eval("vendor_state"))%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="vendor_phone" HeaderText="Phone" SortExpression="vendor_phone">
                                <ItemTemplate>
                                    <%# TrimPhone(Eval("vendor_phone")) %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridDateTimeColumn DataField="dt_stamp" HeaderText="Date Added" DataFormatString="{0:MM/dd/yyyy hh:mm tt}" HtmlEncode="false">
                            </telerik:GridDateTimeColumn>
                            <telerik:GridTemplateColumn HeaderText="Type" DataField="type" SortExpression="type">
                                <ItemTemplate>
                                    <%# ParseType(Eval("type")) %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <GroupByExpressions>
                            <telerik:GridGroupByExpression>
                                <GroupByFields>
                                    <telerik:GridGroupByField FieldName="type" />
                                </GroupByFields>
                                <SelectFields>
                                    <telerik:GridGroupByField FieldName="type" HeaderText=" " HeaderValueSeparator=" " FormatString="<span style='display: none'>{0}</span>" />
                                </SelectFields>
                            </telerik:GridGroupByExpression>
                        </GroupByExpressions>
                    </MasterTableView>
                </telerik:RadGrid>
            </asp:View>
            <asp:View ID="View_AddEdit" runat="server">
                <table class="formtable bdr" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="width: 120px">Vendor Type:</td>
                        <td>
                            <asp:DropDownList ID="vendorTypeDropDownList" runat="server">
                                <asp:ListItem Text="DPO Vendor" Value="0" />
                                <asp:ListItem Text="Service Request Vendor" Value="1" />
                                <asp:ListItem Text="DRO Vendor" Value="2" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>Vendor Name:</td>
                        <td>
                            <asp:TextBox ID="venderNameTextBox" Width="200" MaxLength="256" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" Text="*" runat="server" ControlToValidate="venderNameTextBox" ErrorMessage="Please enter a vendor name."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>Vendor Address:</td>
                        <td>
                            <asp:TextBox ID="vendorAddressTextBox" Width="200" MaxLength="256" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" Text="*" runat="server" ControlToValidate="vendorAddressTextBox" ErrorMessage="Please enter a vendor address."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>Vendor City:</td>
                        <td>
                            <asp:TextBox ID="vendorCityTextBox" Width="200" MaxLength="64" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" Text="*" runat="server" ControlToValidate="vendorCityTextBox" ErrorMessage="Please enter a vendor city."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>Vendor State:</td>
                        <td>
                            <asp:DropDownList ID="vendorStateDropDownList" runat="server">
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
                            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="vendorStateDropDownList" ErrorMessage="Please select a state." Text="*" ValueToCompare="Select One..." Operator="NotEqual"></asp:CompareValidator>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>Vendor Phone:</td>
                        <td>
                            <telerik:RadMaskedTextBox ID="vendorPhoneRadMaskedTextBox" ClientEvents-OnFocus="OnFocus" InvalidStyle-BackColor="Red" Width="200" EnableEmbeddedSkins="false" Mask="(###) ### - ####" runat="server">
                            </telerik:RadMaskedTextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" Text="*" ControlToValidate="vendorPhoneRadMaskedTextBox" runat="server" ErrorMessage="Please enter a vendor phone number"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>Vendor FAX:</td>
                        <td>
                            <telerik:RadMaskedTextBox ID="vendorFaxRadMaskedTextBox" ClientEvents-OnFocus="OnFocus" InvalidStyle-BackColor="Red" Width="200" EnableEmbeddedSkins="false" Mask="(###) ### - ####" runat="server">
                            </telerik:RadMaskedTextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" Text="*" ControlToValidate="vendorFaxRadMaskedTextBox" runat="server" ErrorMessage="Please enter a vendor fax number"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td>Vendor URL:</td>
                        <td>
                            <asp:TextBox ID="vendorURLTextBox" MaxLength="256" Width="200" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top;">Vendor Notes:</td>
                        <td>
                            <asp:TextBox ID="vendorNotesTextBox" Width="500" Rows="8" TextMode="MultiLine" runat="server"></asp:TextBox></td>
                    </tr>
                </table>
                <div class="tableButtons">
                    <asp:Button ID="submitButton" CommandName="Add" runat="server" Text=" Add " OnClick="submitButton_Click" />
                    <asp:Button ID="clearButton" CausesValidation="false" runat="server" CommandName="Clear" Text=" Clear " OnClick="clearButton_Click" />
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
