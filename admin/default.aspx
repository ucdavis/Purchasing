<%@ Page Title="" Language="C#" MasterPageFile="~/master/default.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="tools_purchasing_admin_default" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../blocks/order_details.ascx" TagName="order_details" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="LeftSidebarContentPlaceHolder" runat="Server">
    <!-- ********** Left sidebar area of the page ********** -->
    <div id="left_sidebar">
        <div id="level2_nav">
            <a name="nav"></a>
            <h2>In this section</h2>
            <asp:Menu ID="Menu3" runat="server">
            </asp:Menu>
        </div>
        <asp:Panel ID="adminPanel" runat="server">
            <div class="other_links">
                <h2>Manage</h2>
                <ul>
                    <li>
                        <asp:HyperLink ID="ordersHyperLink" CssClass="here" NavigateUrl="default.aspx" runat="server">Orders</asp:HyperLink></li></li>
                    <li>
                        <asp:HyperLink ID="usersHyperLink" NavigateUrl="users.aspx" runat="server">Users</asp:HyperLink></li>
                    <li>
                        <asp:HyperLink ID="vendorsHyperLink" NavigateUrl="vendors.aspx" runat="server">Vendors</asp:HyperLink></li>
                    <li>
                        <asp:HyperLink ID="addressesHyperLink" NavigateUrl="addresses.aspx" runat="server">Addresses</asp:HyperLink></li>
                </ul>
            </div>
        </asp:Panel>
        <asp:Panel ID="acrhivePanel" runat="server">
            <div class="other_links">
                <h2>Archived Orders</h2>
                <ul>
                    <li>
                        <asp:CheckBox ID="archiveCheckBox" AutoPostBack="true" OnCheckedChanged="archiveCheckBox_CheckedChanged" Style="padding: 0 1em;" Checked="false" Text=" Show Archived" runat="server" /></li>
                </ul>
            </div>
        </asp:Panel>
        <div class="other_links">
            <h2>View Orders By</h2>
            <telerik:RadPanelBar ID="ordersRadPanelBar" PersistStateInCookie="true" EnableEmbeddedSkins="false" EnableEmbeddedBaseStylesheet="false" runat="server">
            </telerik:RadPanelBar>
        </div>
    </div>
    <!-- ********** End left sidebar area of the page ********** -->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="Server">
    <!-- ********** Main content area of the page ********** -->
    <div id="formwrap">
        <asp:MultiView ID="commandsMultiView" runat="server">
            <asp:View ID="View_Commands_Detail" runat="server">
                <div class="flt_rt noprint">
                    <asp:Button ID="qtButton" runat="server" Text=" Manage " OnClientClick="return false;" ToolTip="Open quick tasks panel" />
                </div>
                <ajaxToolkit:PopupControlExtender ID="PopupControlExtender1" runat="server" TargetControlID="qtButton" PopupControlID="qtPanel" Position="Left" OffsetX="-157" OffsetY="22">
                </ajaxToolkit:PopupControlExtender>
                <asp:Panel ID="qtPanel" runat="server" CssClass="qtPanel noprint" Width="230px">
                    <h2><a href="#" title="Close" style="float: right; color: #FFF;" onclick="$find('<%= PopupControlExtender1.ClientID %>').hidePopup();">X</a>Quick Tasks</h2>
                    <ul>
                        <li>
                            <asp:LinkButton ID="qtPrintLinkButton" OnClientClick='javascript:AjaxControlToolkit.PopupControlBehavior.__VisiblePopup.hidePopup(); window.print(); return false;' runat="server">
                                <asp:Image ID="qtPrintImage" OnClientClick='javascript:window.print(); return false;' ImageAlign="AbsMiddle" ImageUrl="~/images/15/qtprint.gif" runat="server" />&nbsp;&nbsp;Print this page</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="qtPlacedLinkButton" runat="server" OnClick="qtPlacedLinkButton_Click">
                                <asp:Image ID="qtPlacedImage" ImageAlign="AbsMiddle" ImageUrl="~/images/15/qtcheck.gif" runat="server" />&nbsp;&nbsp;Mark as placed</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="qtDetailsLinkButton" runat="server">
                                <asp:Image ID="qtDetailsImage" ImageAlign="AbsMiddle" ImageUrl="~/images/15/qtview.gif" runat="server" />&nbsp;&nbsp;Update order details</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="qtStopLinkButton" runat="server">
                                <asp:Image ID="qtStoppedImage" ImageAlign="AbsMiddle" ImageUrl="~/images/15/qtdelete.gif" runat="server" />&nbsp;&nbsp;Stop this order</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="qtSendBackPopUpLinkButton" runat="server">
                                <asp:Image ID="qtSendBackImage" ImageAlign="AbsMiddle" ImageUrl="~/images/15/qtltos.gif" runat="server" />&nbsp;&nbsp;Send back to PI/Approver.</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="qtReceiptLinkButton" runat="server">
                                <asp:Image ID="qtReceiptImage" ImageAlign="AbsMiddle" ImageUrl="~/images/15/qtmail.gif" runat="server" />&nbsp;&nbsp;Ask for receipt</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="qtCommentsLinkButton" runat="server">
                                <asp:Image ID="qtCommentsImage" ImageAlign="AbsMiddle" ImageUrl="~/images/15/qtcomments.gif" runat="server" />&nbsp;&nbsp;Attach a comment or file</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="qtLockLinkButton" runat="server" OnClick="qtLockLinkButton_Click">
                                <asp:Image ID="qtLockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/15/qtcomments.gif" runat="server" />&nbsp;&nbsp;<asp:Label ID="lockLabel" runat="server" /></asp:LinkButton></li>
                    </ul>
                </asp:Panel>
                <!-- REJECT MODAL -->
                <ajaxToolkit:ModalPopupExtender PopupControlID="rejectPanel" CancelControlID="rejectCancelButton" BackgroundCssClass="modalBackground" TargetControlID="qtSendBackPopUpLinkButton" ID="rejectModalPopupExtender" runat="server">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="rejectPanel" runat="server" Width="350px" CssClass="modalPopup" Style="display: none">
                    <table class="formtable bdrlft" cellspacing="0">
                        <tr>
                            <th>Stop Order</th>
                        </tr>
                        <tr class="alt smaller">
                            <td style="height: 40px">Please use the space below to describe why you are rejecting this order. </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="rejectTextBox" Width="300" Rows="3" TextMode="MultiLine" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rejectRequiredFieldValidator" Display="None" ValidationGroup="reject" ControlToValidate="rejectTextBox" runat="server" ErrorMessage="You must enter a message"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr class="alt">
                            <td>
                                <asp:Button ID="rejectSubmitButton" ValidationGroup="reject" runat="server" Text="Submit" OnClick="rejectSubmitButton_Click" />
                                <asp:Button ID="rejectCancelButton" runat="server" Text="Close" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <!-- END: REJECT MODAL -->
                <!-- RECEIPT MODAL -->
                <ajaxToolkit:ModalPopupExtender PopupControlID="receiptPanel" CancelControlID="receiptCancelButton" BackgroundCssClass="modalBackground" TargetControlID="qtReceiptLinkButton" ID="receiptModalPopupExtender" runat="server">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="receiptPanel" runat="server" Width="350px" CssClass="modalPopup" Style="display: none">
                    <table class="formtable bdrlft" cellspacing="0">
                        <tr>
                            <th colspan="2">Ask for receipt</th>
                        </tr>
                        <tr class="alt smaller">
                            <td colspan="2" style="height: 40px">Use the form below to enter an optional message which will be attached to the receipt e-mail. </td>
                        </tr>
                        <tr>
                            <td>Message: </td>
                            <td>
                                <asp:TextBox ID="receiptMessageTextBox" Width="250" Rows="3" TextMode="MultiLine" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>Email: </td>
                            <td>
                                <asp:TextBox ID="receiptTextBox" Width="250" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="receiptRequiredFieldValidator" Display="None" ValidationGroup="receipt" ControlToValidate="receiptTextBox" runat="server" ErrorMessage="You must enter an e-mail address"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ControlToValidate="receiptTextBox" Display="None" ValidationGroup="receipt" ID="receiptRegularExpressionValidator" runat="server" ErrorMessage="You must enter a valid e-mail address."></asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr class="alt">
                            <td colspan="2">
                                <asp:Button ID="receiptSubmitButton" ValidationGroup="receipt" runat="server" Text="Submit" OnClick="receiptSubmitButton_Click" />
                                <asp:Button ID="receiptCancelButton" runat="server" Text="Close" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <!-- END: RECEIPT MODAL -->
                <!-- STOP MODAL -->
                <ajaxToolkit:ModalPopupExtender PopupControlID="stopPanel" CancelControlID="stopCancelButton" BackgroundCssClass="modalBackground" TargetControlID="qtStopLinkButton" ID="stopModalPopupExtender" runat="server">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="stopPanel" runat="server" Width="350px" CssClass="modalPopup" Style="display: none">
                    <table class="formtable bdrlft" cellspacing="0">
                        <tr>
                            <th>Stop Order</th>
                        </tr>
                        <tr class="alt smaller">
                            <td style="height: 40px">Please use the space below to describe why you are stopping this order. </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="stopTextBox" Width="300" Rows="3" TextMode="MultiLine" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="stopRequiredFieldValidator" Display="None" ValidationGroup="stop" ControlToValidate="stopTextBox" runat="server" ErrorMessage="You must enter a message"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr class="alt">
                            <td>
                                <asp:Button ID="stopSubmitButton" ValidationGroup="stop" runat="server" Text="Submit" OnClick="stopSubmitButton_Click" />
                                <asp:Button ID="stopCancelButton" runat="server" Text="Close" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <!-- END: STOP MODAL -->
                <!-- DETAILS MODAL -->
                <ajaxToolkit:ModalPopupExtender PopupControlID="detailsPanel" CancelControlID="detailsCancelButton" BackgroundCssClass="modalBackground" TargetControlID="qtDetailsLinkButton" ID="detailsModalPopupExtender" runat="server">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="detailsPanel" runat="server" Width="600px" CssClass="modalPopup" Style="display: none">
                    <table class="formtable bdrlft" cellspacing="0">
                        <tr>
                            <th colspan="2">Update Order Details</th>
                        </tr>
                        <tr>
                            <td style="width: 180px">Actual Total: </td>
                            <td>
                                <telerik:RadNumericTextBox ID="dtlTotalRadNumericTextBox" Width="150px" Type="Currency" EnableEmbeddedSkins="false" InvalidStyle-BackColor="Red" runat="server">
                                </telerik:RadNumericTextBox>&nbsp;(Include tax and shipping fees) </td>
                            <asp:RequiredFieldValidator ID="detailsRequiredFieldValidator" Display="None" ControlToValidate="dtlTotalRadNumericTextBox" ValidationGroup="details" runat="server" ErrorMessage="Please enter the actual order total with the vendor."></asp:RequiredFieldValidator>
                        </tr>
                        <tr class="alt">
                            <td>DaFIS Doc Number: </td>
                            <td>
                                <asp:TextBox ID="dtlDocTextBox" runat="server" Width="150px" MaxLength="64"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>DaFIS PO Number: </td>
                            <td>
                                <asp:TextBox ID="dtlPoTextBox" runat="server" Width="150px" MaxLength="64"></asp:TextBox>
                            </td>
                        </tr>
                        <tr class="alt">
                            <td>Vendor Confirmation Number: </td>
                            <td>
                                <asp:TextBox ID="dtlVendorConfirmationTextBox" runat="server" Width="150px" MaxLength="64"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Button ID="detailsSubmitButton" ValidationGroup="details" runat="server" Text="Submit" OnClick="detailsSubmitButton_Click" />
                                <asp:Button ID="detailsCancelButton" runat="server" Text="Close" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <!-- END: DETAILS MODAL -->
                <!-- COMMENTS MODAL -->
                <ajaxToolkit:ModalPopupExtender PopupControlID="commentsPanel" CancelControlID="commentsCancelButton" BackgroundCssClass="modalBackground" TargetControlID="qtCommentsLinkButton" ID="commentsModalPopupExtender" runat="server">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="commentsPanel" runat="server" Width="350px" CssClass="modalPopup" Style="display: none">
                    <table class="formtable bdrlft" cellspacing="0">
                        <tr>
                            <th>Attach comments & files</th>
                        </tr>
                        <tr class="alt smaller">
                            <td style="height: 25px">Please enter your comments below. </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="commentsTextBox" Width="300" Rows="3" TextMode="MultiLine" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="commentsRequiredFieldValidator" Display="None" ValidationGroup="comments" ControlToValidate="commentsTextBox" runat="server" ErrorMessage="You must enter a message"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <telerik:RadUpload ID="commentsRadUpload" Width="150px" ControlObjectsVisibility="None" runat="server">
                                </telerik:RadUpload>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBox ID="notifyRequesterCheckBox" Text=" Notify Requester" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBox ID="notifyApproverCheckBox" Text=" Notify Approver" runat="server" />
                            </td>
                        </tr>
                        <tr class="alt">
                            <td>
                                <asp:Button ID="commentsSubmitButton" ValidationGroup="comments" runat="server" Text="Submit" OnClick="commentsSubmitButton_Click" />
                                <asp:Button ID="commentsCancelButton" runat="server" Text="Close" /><br />
                                <asp:ValidationSummary ID="ValidationSummary" ValidationGroup="comments" runat="server" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <!-- END: COMMENTS MODAL -->
            </asp:View>
            <asp:View ID="View_Commands_Export" runat="server">
                <div class="flt_rt">
                    <asp:Button ID="archiveButton" runat="server" Text="Archive Selected" OnClick="archiveButton_Click" />
                    <asp:DropDownList ID="exportDropDownList" AutoPostBack="true" runat="server" OnSelectedIndexChanged="exportDropDownList_SelectedIndexChanged">
                        <asp:ListItem Text="Export To..." Value="Export" />
                        <asp:ListItem Text="Excel" Value="Excel" />
                        <asp:ListItem Text="PDF" Value="PDF" />
                        <asp:ListItem Text="Word" Value="Word" />
                    </asp:DropDownList>
                </div>
            </asp:View>
            <asp:View ID="View_Commands_Blank" runat="server">
            </asp:View>
        </asp:MultiView>
        <h1>
            <asp:Label ID="sectionTitleLabel" runat="server" /></h1>
        <asp:MultiView ID="adminMultiView" OnActiveViewChanged="adminMultiView_ActiveViewChanged" runat="server">
            <asp:View ID="View_Orders" runat="server">
                <telerik:RadGrid ID="ordersRadGrid" AllowMultiRowSelection="true" OnItemDataBound="ordersRadGrid_ItemDataBound" AllowPaging="true" PagerStyle-Mode="NextPrevNumericAndAdvanced" PageSize="30" OnInit="ordersRadGrid_Init" Skin="Default" AllowFilteringByColumn="True" ShowFooter="true" AllowSorting="True" AutoGenerateColumns="False" runat="server" OnNeedDataSource="ordersRadGrid_NeedDataSource" OnDataBound="ordersRadGrid_DataBound">
                    <ExportSettings ExportOnlyData="True" IgnorePaging="True" OpenInNewWindow="True">
                        <Excel Format="Html"></Excel>
                    </ExportSettings>
                    <MasterTableView GroupLoadMode="Client" DataKeyNames="id">
                        <RowIndicatorColumn>
                            <HeaderStyle Width="20px"></HeaderStyle>
                        </RowIndicatorColumn>
                        <ExpandCollapseColumn>
                            <HeaderStyle Width="20px"></HeaderStyle>
                        </ExpandCollapseColumn>
                        <Columns>
                            <telerik:GridTemplateColumn UniqueName="commands" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:HyperLink ID="selectHyperLink" Text="View" CssClass="bdrLink" NavigateUrl='<%# "default.aspx?order=" + Eval("id") %>' runat="server" />
                                    <asp:HiddenField ID="archivedHiddenField" Value='<%# Eval("archived")%>' EnableViewState="false" runat="server" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Requester">
                                <ItemTemplate>
                                    <asp:HyperLink ID="requesterHyperLink" NavigateUrl='<%# "?user=" + Eval("userid") + "&t=" + GetFullName(Eval("userid")) %>' Text='<% # GetFullName(Eval("userid")) %>' ToolTip='<%# ParseUser(Eval("userid")) %>' runat="server">HyperLink</asp:HyperLink>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="unique_id" HeaderText="Order ID" FilterControlWidth="80px">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn DataField="account" HeaderText="Account(s)" FilterControlWidth="80px">
                                <ItemTemplate>
                                    <%# TrimAccount(Eval("account")) %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridDateTimeColumn Visible="false" HeaderText="Order Date" AllowFiltering="True" FooterAggregateFormatString="Last: {0:M/d/yy}" Aggregate="Last" DataField="dt_stamp" DataFormatString="{0:MM/dd/yy}" DataType="System.DateTime" PickerType="None" SortExpression="dt_stamp" AllowSorting="true">
                            </telerik:GridDateTimeColumn>
                            <telerik:GridNumericColumn HeaderText="Order Total" FooterAggregateFormatString="{0:C}" Aggregate="Sum" DataField="order_total" NumericType="Currency" AllowFiltering="true" SortExpression="order_total" AllowSorting="true" FilterControlWidth="80px" />
                            <telerik:GridTemplateColumn UniqueName="approved" AllowFiltering="false">
                                <HeaderTemplate>
                                    <asp:Label runat="server" Text="O-A" ToolTip="Order Approved" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="approvedLabel" runat="server" Text='<%# ParseApproval(Eval("id")) %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="placed" AllowFiltering="false">
                                <HeaderTemplate>
                                    <asp:Label runat="server" Text="O-P" ToolTip="Order Placed" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="placedLabel" runat="server" Text='<%# ParseOrdered(Eval("id"))%>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="received" AllowFiltering="false">
                                <HeaderTemplate>
                                    <asp:Label runat="server" Text="O-R" ToolTip="Order Received" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="receivedLabel" runat="server" Text='<%# ParseReceived(Eval("id"))%>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings AllowExpandCollapse="False" EnableRowHoverStyle="true" Selecting-EnableDragToSelectRows="true" Selecting-AllowRowSelect="true" AllowGroupExpandCollapse="False">
                    </ClientSettings>
                    <FilterMenu EnableTheming="True">
                        <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                    </FilterMenu>
                </telerik:RadGrid>
            </asp:View>
            <asp:View ID="View_OrderDetails" runat="server">
                <uc1:order_details ID="order_details1" runat="server" />
            </asp:View>
            <asp:View ID="View_OrderPlace" runat="server">
                <table class="formtable bdr" cellspacing="0">
                    <tr>
                        <td style="width: 200px">Actual Total: </td>
                        <td>
                            <telerik:RadNumericTextBox ID="actualTotalRadNumericTextBox" Width="200px" Type="Currency" Skin="" InvalidStyle-BackColor="Red" runat="server">
                            </telerik:RadNumericTextBox>&nbsp;(Include tax and shipping fees) </td>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" Display="None" ControlToValidate="actualTotalRadNumericTextBox" ValidationGroup="place" runat="server" ErrorMessage="Please enter the actual order total with the vendor."></asp:RequiredFieldValidator>
                    </tr>
                    <tr class="alt">
                        <td>DaFIS Doc Number: </td>
                        <td>
                            <asp:TextBox ID="dafisDocTextBox" runat="server" Width="200px" MaxLength="64"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>DaFIS PO Number: </td>
                        <td>
                            <asp:TextBox ID="dafisPOTextBox" runat="server" Width="200px" MaxLength="64"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Vendor Confirmation Number: </td>
                        <td>
                            <asp:TextBox ID="confirmationTextBox" runat="server" Width="200px" MaxLength="64"></asp:TextBox>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td style="vertical-align: top">Message to the requester: </td>
                        <td>
                            <asp:TextBox ID="messageTextBox" TextMode="MultiLine" Rows="3" MaxLength="500" Width="300px" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Send Follow-Up In: </td>
                        <td>
                            <asp:TextBox ID="spanTextBox" Width="50px" runat="server"></asp:TextBox>&nbsp;<asp:DropDownList ID="spanUnitDropDownList" runat="server">
                                <asp:ListItem>Days</asp:ListItem>
                                <asp:ListItem>Weeks</asp:ListItem>
                                <asp:ListItem>Months</asp:ListItem>
                            </asp:DropDownList>
                            &nbsp;Enter zero '0' to send immediately.
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="spanTextBox" Display="None" ValidationGroup="place" runat="server" ErrorMessage="Please enter a time span for sending the follow-up e-mail"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr class="alt">
                        <td colspan="2">
                            <asp:CheckBox ID="placeNotifyCheckBox" Text=" Notify Requester that their order has been placed." Checked="true" runat="server" />
                        </td>
                    </tr>
                </table>
                <div class="tableButtons">
                    <asp:Button ID="placeButton" ValidationGroup="place" runat="server" Text=" Place " OnClick="placeButton_Click" />
                    <asp:Button ID="placeCancelButton" runat="server" Text=" Cancel " OnClick="placeCancelButton_Click" />
                </div>
                <asp:ValidationSummary ID="ValidationSummary2" ValidationGroup="place" ShowMessageBox="true" ShowSummary="false" runat="server" />
            </asp:View>
            <asp:View ID="View_ForceApproval" runat="server">
                <p>Please paste the signed approval text or attach a signed copy of an approval document using the form below.</p>
                <asp:Label ID="validationLabel" runat="server" />
                <table class="formtable bdrlft" cellspacing="0">
                    <tr>
                        <th colspan="2">Force Approval</th>
                    </tr>
                    <tr class="alt">
                        <td style="vertical-align: top; padding-top: 7px; width: 180px;">Signed approval text: </td>
                        <td>
                            <asp:TextBox ID="signedTextTextBox" Width="98%" Rows="6" TextMode="MultiLine" MaxLength="1024" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Signed approval document: </td>
                        <td>
                            <telerik:RadUpload ID="signedDocRadUpload" InitialFileInputsCount="1" ControlObjectsVisibility="None" runat="server">
                            </telerik:RadUpload>
                        </td>
                    </tr>
                </table>
                <div class="tableButtons">
                    <asp:Button ID="forceApprovalButton" runat="server" Text=" Submit Approval " OnClick="forceApprovalButton_Click" />
                    <asp:Button ID="forceBackButton" runat="server" Text=" Back to order " OnClick="forceBackButton_Click" />
                </div>
            </asp:View>
            <asp:View ID="View_OrderConfirm" runat="server">
                <asp:Label ID="confirmLabel" runat="server" />
            </asp:View>
        </asp:MultiView>
    </div>
    <!-- ********** End main content area of the page ********** -->
</asp:Content>
