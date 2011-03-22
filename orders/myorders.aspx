<%@ Page Title="" Language="C#" MasterPageFile="~/master/default.master" AutoEventWireup="true" CodeFile="myorders.aspx.cs" Inherits="tools_purchasing_orders_myorders" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../blocks/order_details.ascx" TagName="order_details" TagPrefix="uc1" %>
<asp:Content ID="Content0" runat="server" ContentPlaceHolderID="HeadContentPlaceHolder">
    <style type="text/css">
        ul.itemsList { margin: 0; padding: 0 0 0 1.5em; font-size: 9px; }
    </style>
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
    <div id="formwrap">
        <asp:MultiView ID="ordersMultiView" runat="server">
            <asp:View ID="View_Orders" runat="server">
                <div class="flt_rt">
                    <telerik:RadComboBox ID="exportRadComboBox" AutoPostBack="true" runat="server" OnSelectedIndexChanged="exportRadComboBox_SelectedIndexChanged">
                        <Items>
                            <telerik:RadComboBoxItem Text="Export To..." Value="Export" />
                            <telerik:RadComboBoxItem Text="Excel" Value="Excel" />
                            <telerik:RadComboBoxItem Text="PDF" Value="PDF" />
                            <telerik:RadComboBoxItem Text="Word" Value="Word" />
                        </Items>
                    </telerik:RadComboBox>
                </div>
                <h1>
                    <asp:Label ID="sectionTitleLabel" runat="server" Text="My Orders"></asp:Label></h1>
                <telerik:RadGrid ID="ordersRadGrid" OnNeedDataSource="ordersRadGrid_NeedDataSource" ShowFooter="true" AllowFilteringByColumn="False" AllowSorting="True" AutoGenerateColumns="False" runat="server">
                    <ExportSettings ExportOnlyData="True" IgnorePaging="True" OpenInNewWindow="True">
                        <Excel Format="Html"></Excel>
                    </ExportSettings>
                    <MasterTableView AllowPaging="true" PageSize="30" PagerStyle-Mode="NextPrevNumericAndAdvanced">
                        <RowIndicatorColumn>
                            <HeaderStyle Width="20px"></HeaderStyle>
                        </RowIndicatorColumn>
                        <ExpandCollapseColumn>
                            <HeaderStyle Width="20px"></HeaderStyle>
                        </ExpandCollapseColumn>
                        <Columns>
                            <telerik:GridTemplateColumn AllowFiltering="false" ItemStyle-Width="40px">
                                <ItemTemplate>
                                    <asp:HyperLink ID="selectHyperLink" Text="View" Style="margin-left: 8px;" CssClass="bdrLink" NavigateUrl='<%# ParseViewLink(Eval("id"), Eval("type")) %>' runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="unique_id" AllowSorting="false" HeaderText="Order ID">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn HeaderText="Vendor">
                                <ItemTemplate>
                                    <%# TrimVendorName(Eval("vendor_name")) %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Items">
                                <ItemTemplate>
                                    <%# GetItems(Eval("id"), Eval("type")) %></ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridNumericColumn HeaderText="Total" FooterAggregateFormatString="Totals: {0:C}" Aggregate="Sum" DataField="order_total" NumericType="Currency" AllowFiltering="false" SortExpression="order_total" AllowSorting="true" />
                            <telerik:GridDateTimeColumn HeaderText="Requested On" FooterAggregateFormatString="Last Order: {0:M/d/yy}" Aggregate="Last" ItemStyle-Width="150px" DataField="dt_stamp" DataFormatString="{0:MM/dd/yy h:mm tt}" DataType="System.DateTime" PickerType="DatePicker" AllowFiltering="true" SortExpression="dt_stamp" AllowSorting="true">
                            </telerik:GridDateTimeColumn>
                            <telerik:GridTemplateColumn HeaderText="Approval" AllowFiltering="false">
                                <ItemTemplate>
                                    <%# ParseApproval(Eval("id")) %>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Placed" AllowFiltering="false">
                                <ItemTemplate>
                                    <%# ParseOrdered(Eval("id")) %>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Status" AllowFiltering="false">
                                <ItemTemplate>
                                    <%# ParseReceived(Eval("id")) %>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings AllowExpandCollapse="False" AllowGroupExpandCollapse="False">
                    </ClientSettings>
                    <FilterMenu EnableTheming="True">
                        <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                    </FilterMenu>
                </telerik:RadGrid>
            </asp:View>
            <asp:View ID="View_Detail" runat="server">
                <telerik:RadFormDecorator ID="RadFormDecorator1" DecoratedControls="All" runat="server" />
                <div class="flt_rt" style="padding-right: 3px;">
                    <div style="padding-left: 5px; float: right;">
                        <asp:Button ID="printButton" OnClientClick='javascript:window.print(); return false;' runat="server" Text=" Print This Page " />
                    </div>
                    <div style="padding-left: 5px; float: right;">
                        <asp:Button ID="duplicateButton" OnClick="duplicateButton_Click" runat="server" Text="Order Similar" />
                    </div>
                    <div style="padding-left: 5px; float: right;">
                        <asp:Button ID="editButton" OnClick="editButton_Click" runat="server" Text="Edit Order" />
                    </div>
                    <div style="padding-left: 5px; float: right;">
                        <div class="red">
                            <asp:Button ID="stopButton" runat="server" Text="Stop This Order" /></div>
                        <!-- STOP MODAL -->
                        <ajaxToolkit:ModalPopupExtender PopupControlID="stopPanel" CancelControlID="stopCancelButton" BackgroundCssClass="modalBackground" TargetControlID="stopButton" ID="stopModalPopupExtender" runat="server">
                        </ajaxToolkit:ModalPopupExtender>
                        <asp:Panel ID="stopPanel" runat="server" Width="350px" CssClass="modalPopup" Style="display: none">
                            <table class="formtable bdrlft" cellspacing="0">
                                <tr>
                                    <th>Stop Order</th>
                                </tr>
                                <tr class="alt smaller">
                                    <td style="padding: 8px 10px !important;">Please use the space below to describe why you would like this order to be stopped.</td>
                                </tr>
                                <tr>
                                    <td style="padding: 10px !important;">
                                        <asp:TextBox ID="stopTextBox" Width="325" Rows="3" TextMode="MultiLine" runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" Display="None" ValidationGroup="stop" ControlToValidate="stopTextBox" runat="server" ErrorMessage="You must enter a message"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr class="alt">
                                    <td style="padding: 8px 10px !important;">
                                        <asp:Button ID="stopSubmitButton" ValidationGroup="stop" runat="server" Text=" Submit stop request " OnClick="stopSubmitButton_Click" />
                                        <asp:Button ID="stopCancelButton" runat="server" Text=" Cancel " />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <!-- END: STOP MODAL -->
                    </div>
                </div>
                <h1>
                    <asp:Label ID="detailsTitleLabel" runat="server"></asp:Label></h1>
                <uc1:order_details ID="order_details1" runat="server" />
            </asp:View>
            <asp:View ID="View_Confirm" runat="server">
                <h1>
                    <asp:Label ID="confirmTitleLabel" runat="server"></asp:Label></h1>
                <asp:Label ID="confirmLabel" ForeColor="Green" runat="server" />
            </asp:View>
        </asp:MultiView>
    </div>
    <!-- ********** End main content area of the page ********** -->
</asp:Content>
