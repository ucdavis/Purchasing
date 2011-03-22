<%@ Control Language="C#" AutoEventWireup="true" CodeFile="order_details.ascx.cs" Inherits="business_purchasing_blocks_order_details" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:MultiView ID="receiptMultiView" runat="server">
    <asp:View ID="View_Receipt_Blank" runat="server">
    </asp:View>
    <asp:View ID="View_Receipt" runat="server">
        <div class="roundedBoxRed">
            <div class="roundedBoxTopRed">
                <span></span>
            </div>
            <div class="roundedBoxContentRed" style="padding-top: 3px;">
                <!-- CONTENT BEGIN -->
                <h1>Order Receipt</h1>
                <table cellspacing="0" class="standard_tbl" style="width: 100%; font-size: 11px;">
                    <tr class="alt">
                        <td colspan="3" style="padding: 10px;">UC Davis policy on the physical receipt of purchased items: The person physically receiving and verifying purchased items shall be someone other than the person who placed or approved the order. <a href="http://manuals.ucdavis.edu/ppm/330/330-11.htm" target="_blank" class="bdrLink">Chapter 330, Financial Management and Services</a> </td>
                    </tr>
                    <tr>
                        <td style="text-align: center; padding-top: 10px; width: 33.3%; border-bottom: 0 none;">
                            <asp:ImageButton ID="receivedImageButton" ToolTip="Item(s) Received" ImageUrl="~/images/common/received.gif" OnClick="receivedImageButton_Click" runat="server" />
                        </td>
                        <td style="text-align: center; padding-top: 10px; width: 33.3%; border-bottom: 0 none;">
                            <asp:ImageButton ID="notReceivedImageButton" ToolTip="Item(s) Not Received" ImageUrl="~/images/common/not_received.gif" runat="server" />
                        </td>
                        <td style="text-align: center; padding-top: 10px; width: 33.3%; border-bottom: 0 none;">
                            <asp:ImageButton ID="conflictImageButton" ToolTip="Item Issues" ImageUrl="~/images/common/issues.gif" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="padding: 3px 10px 15px 10px; vertical-align: top; text-align: center;">I certify that the item(s) or service(s) ordered were received in full and will be used for official University business only.</td>
                        <td style="padding: 3px 10px 15px 10px; vertical-align: top; text-align: center;">I have not yet received the item(s) or service(s).</td>
                        <td style="padding: 3px 10px 15px 10px; vertical-align: top; text-align: center;">I received the order, however there are problems that need to be resolved before paying the invoice.</td>
                    </tr>
                </table>
                <!-- CONTENT END -->
            </div>
            <div class="roundedBoxBottomRed">
                <span></span>
            </div>
        </div>
        <!-- NOT RECEIVED MODAL -->
        <ajaxToolkit:ModalPopupExtender PopupControlID="notreceivedPanel" CancelControlID="notreceivedCancelButton" BackgroundCssClass="modalBackground" TargetControlID="notReceivedImageButton" ID="commentModalPopupExtender" runat="server">
        </ajaxToolkit:ModalPopupExtender>
        <asp:Panel ID="notreceivedPanel" runat="server" Width="350px" CssClass="modalPopup" Style="display: none">
            <table class="formtable bdrlft" cellspacing="0">
                <tr>
                    <th>Order Not Received</th>
                </tr>
                <tr class="alt smaller">
                    <td style="height: 50px">Please enter any comments below and click 'submit' to notify your purchaser that you have not received your order. </td>
                </tr>
                <tr>
                    <td style="padding: 8px;">
                        <asp:TextBox ID="notreceivedTextBox" Width="98%" Rows="3" TextMode="MultiLine" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr class="alt">
                    <td>
                        <asp:Button ID="notreceivedSubmitButton" ValidationGroup="notreceived" runat="server" Text="Submit" OnClick="notreceivedSubmitButton_Click" />
                        <asp:Button ID="notreceivedCancelButton" runat="server" Text="Close" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- END: NOT RECEIVED MODAL -->
        <!-- CONFLICT MODAL -->
        <ajaxToolkit:ModalPopupExtender PopupControlID="conflictPanel" CancelControlID="conflictCloseButton" BackgroundCssClass="modalBackground" TargetControlID="conflictImageButton" ID="commentModalPopupExtender2" runat="server">
        </ajaxToolkit:ModalPopupExtender>
        <asp:Panel ID="conflictPanel" runat="server" Width="350px" CssClass="modalPopup" Style="display: none">
            <table class="formtable bdrlft" cellspacing="0">
                <tr>
                    <th>Order Conflict</th>
                </tr>
                <tr class="alt smaller">
                    <td style="height: 55px">Please describe the problems with your order in the textbox below. Be sure to include the catalog number of the items you are referring to. </td>
                </tr>
                <tr>
                    <td style="padding: 8px;">
                        <asp:TextBox ID="conflictTextBox" Width="98%" Rows="3" TextMode="MultiLine" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="conflict" Display="None" ControlToValidate="conflictTextBox" runat="server" ErrorMessage="You must enter a comment."></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr class="alt">
                    <td>
                        <asp:Button ID="conflictSubmitButton" ValidationGroup="comment" runat="server" Text="Submit" OnClick="conflictSubmitButton_Click" />
                        <asp:Button ID="conflictCloseButton" runat="server" Text="Close" />
                        <asp:ValidationSummary ID="ValidationSummary1" ValidationGroup="comment" ShowMessageBox="true" ShowSummary="false" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- END: CONFLICT MODAL -->
        <br />
    </asp:View>
    <asp:View ID="View_Receipt_Confirm" runat="server">
        <div class="roundedBoxRed">
            <div class="roundedBoxTopRed">
                <span></span>
            </div>
            <div class="roundedBoxContentRed" style="padding-top: 3px; min-height: 80px;">
                <!-- CONTENT BEGIN -->
                <h1>Thank You</h1>
                <asp:Label ID="confirmReceiptLabel" runat="server" />
                <!-- CONTENT END -->
            </div>
            <div class="roundedBoxBottomRed">
                <span></span>
            </div>
        </div>
        <br />
    </asp:View>
</asp:MultiView>
<div class="roundedBox">
    <div class="roundedBoxTop">
        <span></span>
    </div>
    <div class="roundedBoxContent">
        <!-- CONTENT BEGIN -->
        <h2 class="flt_rt" style="width: 300px; padding: 0px; text-align: right;">
            <asp:Label ID="dtlNeededLabel" runat="server" />
        </h2>
        <h1 style="margin: 0px; padding: 0px;">
            <asp:Label ID="dtlVendorLabel" runat="server" />&nbsp;<asp:ImageButton ID="vendorImageButton" Style="vertical-align: inherit;" CausesValidation="false" ImageUrl="~/images/15/document.gif" ToolTip="Vendor Notes" runat="server" />
        </h1>
        <p>
            <asp:Label ID="vendorAddress" runat="server" /><br />
            Phone:&nbsp;<asp:Label ID="vendorPhone" runat="server" />&nbsp;&nbsp;FAX:&nbsp;<asp:Label ID="vendorFAX" runat="server" /><br />
            <asp:HyperLink ID="vendorHyperLink" CssClass="bdrLink" Target="_blank" runat="server" />
        </p>
        <asp:Panel ID="agreementPanel" Visible="false" runat="server">
            <table style="width: 100%;" cellspacing="0">
                <tr>
                    <td style="font-size: 10px; border: 1px solid #DDD; padding: 8px; vertical-align: top;"><strong>Agreement Details</strong><br />
                        <asp:Label ID="baTypeLabel" runat="server" /><br />
                        <asp:Label ID="baAgreementContactLabel" runat="server" /><br />
                        <asp:Label ID="baUCDContactLabel" runat="server" /><br />
                        <asp:Label ID="baDatesLabel" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- VENDOR NOTES MODAL -->
        <ajaxToolkit:ModalPopupExtender PopupControlID="vendorNotesPanel" CancelControlID="vendorCancelButton" BackgroundCssClass="modalBackground" TargetControlID="vendorImageButton" ID="vendorModalPopupExtender" runat="server">
        </ajaxToolkit:ModalPopupExtender>
        <asp:Panel ID="vendorNotesPanel" runat="server" Width="545px" CssClass="modalPopup" Style="display: none">
            <table class="formtable bdrlft" cellspacing="0">
                <tr>
                    <th colspan="2">Vendor Notes</th>
                </tr>
                <tr class="alt smaller">
                    <td colspan="2" style="height: 25px">Please enter your notes below and click "submit". </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="vendorNotesTextBox" Width="525" Rows="12" TextMode="MultiLine" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr class="alt">
                    <td colspan="2">
                        <asp:Button ID="vendorSubmitButton" ValidationGroup="notes" runat="server" Text="Submit" OnClick="vendorSubmitButton_Click" />
                        <asp:Button ID="vendorCancelButton" runat="server" Text="Close" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- END: VENDOR NOTES MODAL -->
        <asp:MultiView ID="itemsMultiView" runat="server">
            <asp:View ID="View_Items_DPO" runat="server">
                <br />
                <asp:Repeater ID="dpoItemsRepeater" runat="server" OnItemCreated="ItemsRepeater_ItemCreated">
                    <HeaderTemplate>
                        <table style="width: 100%;" class="formtable bdrlft" cellspacing="0">
                            <tr class="smaller">
                                <th style="width: 10%">Quantity</th>
                                <th style="width: 10%">Unit</th>
                                <th style="width: 15%">Catalog #</th>
                                <th style="width: 50%">Description</th>
                                <th style="width: 10%">Unit Price</th>
                                <th style="width: 5%">Notes</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="smaller">
                            <td>
                                <asp:Label ID="qtyLabel" runat="server" Text='<%#Eval("qty") %>' />
                            </td>
                            <td>
                                <%#Eval("unit") %>
                            </td>
                            <td>
                                <%#Eval("catalog_num") %>
                            </td>
                            <td>
                                <%#Eval("description") %>
                            </td>
                            <td>
                                <asp:HiddenField ID="unitPriceHiddenField" Value='<%# Eval("unit_price") %>' runat="server" />
                                <asp:Label ID="unitPriceLabel" runat="server" Text='<%# ConvertToCurrency(Eval("unit_price")) %>' />
                            </td>
                            <td style="text-align: center">
                                <asp:Image ID="notesImage" ToolTip="Attach notes to this item" Visible='<%# (Eval("notes").ToString().Length > 0 ? true : false) %>' Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
                                <ajaxToolkit:ModalPopupExtender PopupControlID="notesPanel" CancelControlID="notesButton" BackgroundCssClass="modalBackground" TargetControlID="notesImage" ID="notesModalPopupExtender" runat="server">
                                </ajaxToolkit:ModalPopupExtender>
                                <asp:Panel ID="notesPanel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
                                    <table class="formtable bdrlft" cellspacing="0">
                                        <tr>
                                            <th>Item Notes</th>
                                        </tr>
                                        <tr class="alt smaller">
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td style="padding: 8px !important;">
                                                <%# Eval("notes").ToString().Length > 0 ? Eval("notes").ToString() : "&nbsp;"  %>
                                            </td>
                                        </tr>
                                        <tr class="alt">
                                            <td style="padding: 8px !important;">
                                                <asp:Button ID="notesButton" CausesValidation="false" runat="server" Text=" Close " />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="alt smaller">
                            <td>
                                <asp:Label ID="qtyLabel" runat="server" Text='<%#Eval("qty") %>' />
                            </td>
                            <td>
                                <%#Eval("unit") %>
                            </td>
                            <td>
                                <%#Eval("catalog_num") %>
                            </td>
                            <td>
                                <%#Eval("description") %>
                            </td>
                            <td>
                                <asp:HiddenField ID="unitPriceHiddenField" Value='<%#Eval("unit_price") %>' runat="server" />
                                <asp:Label ID="unitPriceLabel" runat="server" Text='<%#ConvertToCurrency(Eval("unit_price")) %>' />
                            </td>
                            <td style="text-align: center">
                                <asp:Image ID="notesImage" ToolTip="Attach notes to this item" Visible='<%# (Eval("notes").ToString().Length > 0 ? true : false) %>' Style="cursor: pointer;" ImageUrl="~/images/15/note_add.gif" runat="server" />
                                <ajaxToolkit:ModalPopupExtender PopupControlID="notesPanel" CancelControlID="notesButton" BackgroundCssClass="modalBackground" TargetControlID="notesImage" ID="notesModalPopupExtender" runat="server">
                                </ajaxToolkit:ModalPopupExtender>
                                <asp:Panel ID="notesPanel" runat="server" Width="380px" CssClass="modalPopup" Style="display: none">
                                    <table class="formtable bdrlft" cellspacing="0">
                                        <tr>
                                            <th>Item Notes</th>
                                        </tr>
                                        <tr class="alt smaller">
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td style="padding: 8px !important; background-color: #FFF;">
                                                <%# Eval("notes") %>
                                            </td>
                                        </tr>
                                        <tr class="alt">
                                            <td style="padding: 8px !important;">
                                                <asp:Button ID="notesButton" CausesValidation="false" runat="server" Text=" Close " />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        <tr class="smaller" id="subtotalRow" runat="server">
                            <td colspan="4" style="text-align: right">Subtotal: </td>
                            <td colspan="2">
                                <asp:Label ID="dtlSubtotalLabel" Font-Bold="true" Text='<%#OrderSubTotal() %>' runat="server" />
                            </td>
                        </tr>
                        <tr class="smaller">
                            <td colspan="4" style="text-align: right">
                                <%#ActualTotalText() %>
                            </td>
                            <td colspan="2">
                                <asp:Label ID="Label1" Font-Bold="true" Text='<%#ActualTotal() %>' runat="server" />
                            </td>
                        </tr>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:View>
            <asp:View ID="View_Items_DRO" runat="server">
                <br />
                <asp:Repeater ID="droItemsRepeater" runat="server" OnItemCreated="ItemsRepeater_ItemCreated">
                    <HeaderTemplate>
                        <table style="width: 100%;" class="formtable bdrlft" cellspacing="0">
                            <tr class="smaller">
                                <th style="width: 25%">Item Description</th>
                                <th style="width: 20%">Estimated Item Value</th>
                                <th style="width: 40%">Description of Repair</th>
                                <th style="width: 15%">Price of Repair</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="smaller">
                            <td>
                                <%#Eval("item_desc") %>
                            </td>
                            <td>
                                <%#ConvertToCurrency(Eval("item_value")) %>
                            </td>
                            <td>
                                <%#Eval("repair_desc") %>
                            </td>
                            <td>
                                <%#ConvertToCurrency(Eval("repair_cost")) %>
                                <asp:HiddenField ID="repairCostHiddenField" Value='<%#Eval("repair_cost") %>' runat="server" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="alt smaller">
                            <td>
                                <%#Eval("item_desc") %>
                            </td>
                            <td>
                                <%#ConvertToCurrency(Eval("item_value")) %>
                            </td>
                            <td>
                                <%#Eval("repair_desc") %>
                            </td>
                            <td>
                                <%#ConvertToCurrency(Eval("repair_cost")) %>
                                <asp:HiddenField ID="repairCostHiddenField" Value='<%#Eval("repair_cost") %>' runat="server" />
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        <tr class="smaller" id="subtotalRow" runat="server">
                            <td colspan="3" style="text-align: right">Subtotal: </td>
                            <td>
                                <asp:Label ID="dtlSubtotalLabel" Font-Bold="true" Text='<%#OrderSubTotal() %>' runat="server" />
                            </td>
                        </tr>
                        <tr class="smaller">
                            <td colspan="3" style="text-align: right">
                                <%#ActualTotalText() %>
                            </td>
                            <td>
                                <asp:Label ID="Label1" Font-Bold="true" Text='<%#ActualTotal() %>' runat="server" />
                            </td>
                        </tr>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:View>
            <asp:View ID="View_Items_Agreement" runat="server">
                <br />
                <asp:Repeater ID="baItemsRepeater" runat="server" OnItemCreated="ItemsRepeater_ItemCreated">
                    <HeaderTemplate>
                        <table style="width: 100%;" class="formtable bdrlft" cellspacing="0">
                            <tr class="smaller">
                                <th style="width: 10%">Quantity</th>
                                <th style="width: 10%">Unit</th>
                                <th style="width: 15%">Service / Item</th>
                                <th style="width: 50%">Description</th>
                                <th style="width: 15%">Price per Unit</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="smaller">
                            <td>
                                <asp:Label ID="qtyLabel" runat="server" Text='<%#Eval("qty") %>' />
                            </td>
                            <td>
                                <%#Eval("unit") %>
                            </td>
                            <td>
                                <%#Eval("service_item") %>
                            </td>
                            <td>
                                <%#Eval("description") %>
                            </td>
                            <td>
                                <asp:HiddenField ID="unitPriceHiddenField" Value='<%#Eval("unit_price") %>' runat="server" />
                                <asp:Label ID="unitPriceLabel" runat="server" Text='<%# ConvertToCurrency(Eval("unit_price")) %>' />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="alt smaller">
                            <td>
                                <asp:Label ID="qtyLabel" runat="server" Text='<%#Eval("qty") %>' />
                            </td>
                            <td>
                                <%#Eval("unit") %>
                            </td>
                            <td>
                                <%#Eval("service_item")%>
                            </td>
                            <td>
                                <%#Eval("description") %>
                            </td>
                            <td>
                                <asp:HiddenField ID="unitPriceHiddenField" Value='<%#Eval("unit_price") %>' runat="server" />
                                <asp:Label ID="unitPriceLabel" runat="server" Text='<%#ConvertToCurrency(Eval("unit_price")) %>' />
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        <tr class="smaller" id="subtotalRow" runat="server">
                            <td colspan="4" style="text-align: right">Subtotal: </td>
                            <td>
                                <asp:Label ID="dtlSubtotalLabel" Font-Bold="true" Text='<%#OrderSubTotal() %>' runat="server" />
                            </td>
                        </tr>
                        <tr class="smaller">
                            <td colspan="4" style="text-align: right">
                                <%#ActualTotalText() %>
                            </td>
                            <td>
                                <asp:Label ID="Label1" Font-Bold="true" Text='<%#ActualTotal() %>' runat="server" />
                            </td>
                        </tr>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:View>
            <asp:View ID="View_Items_Blank" runat="server">
            </asp:View>
        </asp:MultiView>
        <br />
        <table style="width: 100%;" cellspacing="0">
            <tr>
                <td style="font-size: 10px; border: 1px solid #DDD; border-right: 0px none; padding: 8px; vertical-align: top;">
                    <asp:MultiView ID="shiptoMultiView" runat="server">
                        <asp:View ID="View_OnSite" runat="server">
                            <strong>
                                <asp:Label ID="shiptoLabel" runat="server" /></strong><br />
                            <asp:Label ID="dtlShiptoNameLabel" runat="server" />
                            <asp:Label ID="dtlShiptoAddressLabel" runat="server" />
                            <asp:Label ID="dtlShiptoCampusLabel" runat="server" />
                            <asp:Label ID="dtlShiptoBuildingLabel" runat="server" /><asp:Label ID="dtlShiptoRoomLabel" runat="server" />
                            <asp:Label ID="dtlShiptoStreetLabel" runat="server" />
                            <asp:Label ID="dtlShiptoCityLabel" runat="server" />,&nbsp;<asp:Label ID="dtlShiptoStateLabel" runat="server" />&nbsp;<asp:Label ID="dtlShiptoZipLabel" runat="server" />
                        </asp:View>
                        <asp:View ID="View_OffSite" runat="server">
                            <strong>Service/Repair Site</strong><br />
                            Off site at vendor's location<br />
                        </asp:View>
                        <asp:View ID="View_NA" runat="server">
                            <strong>Service/Delivery Site</strong><br />
                            N/A
                        </asp:View>
                    </asp:MultiView>
                </td>
                <td style="font-size: 10px; border: 1px solid #DDD; padding: 8px; vertical-align: top;"><strong>Order Details</strong><br />
                    -&nbsp;<asp:Label ID="dtlShippingLabel" runat="server" /><br />
                    -&nbsp;<asp:Label ID="dtlBackorderLabel" runat="server" /><br />
                    -&nbsp;Account:&nbsp;<asp:Label ID="dtlAccountLabel" runat="server" />
                    <asp:Label ID="dtlDafisDocLabel" runat="server" />
                    <asp:Label ID="dtlDafisPOLabel" runat="server" />
                    <asp:Label ID="dtlConfirmLabel" runat="server" />
                    <asp:Label ID="dtlInvoiceLabel" runat="server" />
                    <asp:Label ID="dtlPurchaser" runat="server" />
                    <br />
                    -&nbsp;PI/Approvers:<br />
                    <asp:Repeater ID="piRepeater" runat="server">
                        <HeaderTemplate>
                            <ul class="piList">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class='<%# Eval("approval").ToString() %>'>
                                <%# Users.GetFullNameByUserName(Eval("pi_userid").ToString(), false, false) %></li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </td>
            </tr>
        </table>
        <br />
        <table cellspacing="0" style="width: 100%; font-size: 10px; background-color: #EAEDF0; border: 1px solid #DDD;">
            <tr>
                <td style="padding: 8px"><strong>Order History</strong><br />
                    <br />
                    <asp:Repeater ID="statusRepeater" OnItemDataBound="statusRepeater_ItemDataBound" runat="server">
                        <HeaderTemplate>
                            <table cellspacing="0" class="standard_tbl" style="width: 100%; border-color: #DDD; font-size: 10px;">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td style="background-color: #FFF" id="statusCell" runat="server">
                                    <asp:HiddenField ID="statusHiddenField" Value='<%# Eval("status") %>' runat="server" />
                                    <asp:Image ID="statusImage" runat="server" ImageAlign="AbsMiddle" />&nbsp;
                                    <asp:Label ID="statusDateLabel" Text='<%#ShortenDate(Eval("dt_stamp")) %>' runat="server" />:&nbsp;
                                    <asp:Label ID="statusMsgLabel" runat="server" />
                                </td>
                                <td style="background-color: #FFF">
                                    <asp:Label ID="statusNotesLabel" Text='<%# ParseNotes(Eval("notes")) %>' runat="server" />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </td>
            </tr>
        </table>
        <asp:Panel ID="notesPanel" runat="server">
            <br />
            <table cellspacing="0" style="width: 100%; font-size: 10px; background-color: #EAEDF0; border: 1px solid #DDD;">
                <tr>
                    <td style="padding: 8px">
                        <div style="float: right; text-align: right; width: 100px;">
                            <asp:HyperLink ID="addNotesHyperLink" NavigateUrl="#" CssClass="bdrLink" runat="server" Text="Add Notes" /></div>
                        <strong>Order Notes</strong><br />
                        <br />
                        <table cellspacing="0" class="standard_tbl" style="width: 100%; border-color: #DDD; font-size: 10px;">
                            <tr>
                                <td style="background-color: #FFF">
                                    <asp:Label ID="notesLabel" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <!-- ORDER NOTES MODAL -->
            <ajaxToolkit:ModalPopupExtender PopupControlID="notesModalPanel" CancelControlID="notesCancelButton" BackgroundCssClass="modalBackground" TargetControlID="addNotesHyperLink" ID="notesModalPopupExtender" runat="server">
            </ajaxToolkit:ModalPopupExtender>
            <asp:Panel ID="notesModalPanel" runat="server" Width="545px" CssClass="modalPopup" Style="display: none">
                <table class="formtable bdrlft" cellspacing="0">
                    <tr>
                        <th colspan="2">Add Notes</th>
                    </tr>
                    <tr class="alt smaller">
                        <td colspan="2" style="padding: 8px 10px !important;">Please enter your notes below and click "submit". </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding: 10px !important;">
                            <asp:TextBox ID="notesTextBox" Width="530" Rows="6" TextMode="MultiLine" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Attachments: </td>
                        <td>
                            <telerik:RadUpload ID="dpoRadUpload" runat="server" ControlObjectsVisibility="AddButton, RemoveButtons" InitialFileInputsCount="0">
                            </telerik:RadUpload>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:CheckBox ID="purchaserCheckBox" Checked="true" runat="server" Text=" Notify purchaser of new notes.<br />" />
                            <asp:CheckBox ID="requesterCheckBox" runat="server" Text=" Notify requester of new notes.<br />" />
                            <asp:CheckBox ID="approverCheckBox" runat="server" Text=" Notify approver of new notes." />
                        </td>
                    </tr>
                    <tr class="alt">
                        <td colspan="2">
                            <asp:Button ID="notesSubmitButton" ValidationGroup="notes" runat="server" Text="Submit" OnClick="notesSubmitButton_Click" />
                            <asp:Button ID="notesCancelButton" runat="server" Text="Close" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <!-- END: ORDER NOTES MODAL -->
        </asp:Panel>
        <!-- CONTENT END -->
    </div>
    <div class="roundedBoxBottom">
        <span></span>
    </div>
</div>
