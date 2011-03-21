<%@ Page Language="VB" AutoEventWireup="false" CodeFile="receipt.aspx.vb" Inherits="receipt" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">    
       <style type="text/css" media="print">
.noprint { display: none}
.vertical {	writing-mode: tb-rl;
	font-size: x-large;
	font-weight: bold;	
}
</style>
<style type="text/css" media="screen">
.noprint { display: visible}
.vertical {	writing-mode: tb-rl;
	font-size: x-large;
	font-weight: bold;	
}
</style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:SqlDataSource ID="sdsOrderRequest" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" DataSourceMode="DataReader" 
            SelectCommand="receipt_info" SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:QueryStringParameter Name="order_id" QueryStringField="order_id" 
                    Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsOrderDetails" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" DataSourceMode="DataReader" 
            SelectCommand="order_info_details" SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:QueryStringParameter Name="order_id" QueryStringField="order_id" 
                    Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <a class="noprint" href="process.aspx">Process more Orders</a>&nbsp;
        <asp:FormView ID="FormView1" runat="server" DataSourceID="sdsOrderRequest" 
            EnableViewState="False" Width="95%" SkinID="nofill">
            <ItemTemplate>
                &nbsp;
                <br />
                <table border="1" style="text-align: left" width="100%">
                    <tr>
                        <td rowspan="9">
                            <strong>Vendor:</strong><br />
                            <asp:Label ID="vendor_nameLabel" runat="server" 
                                Text='<%# Bind("vendor_name") %>'></asp:Label>
                            <br />
                            <asp:Label ID="addressLabel" runat="server" Text='<%# Bind("address") %>'></asp:Label>
                            <br />
                            <asp:Label ID="cszLabel" runat="server" Text='<%# Bind("csz") %>'></asp:Label>
                            <br />
                            <br />
                            <strong>Phone:</strong>
                            <asp:Label ID="phoneLabel" runat="server" Text='<%# Bind("phone") %>'></asp:Label>
                            <br />
                            <strong>Fax: </strong>
                            <asp:Label ID="faxLabel" runat="server" Text='<%# Bind("fax") %>'></asp:Label>
                            <br />
                            <br />
                            <strong>Web:</strong>
                            <asp:HyperLink ID="HyperLink1" runat="server" 
                                NavigateUrl='<%# "http://" & Eval("website") %>' Target="_blank" 
                                Text='<%# Eval("vendor_name") %>'></asp:HyperLink>
                        </td>
                        <td colspan="2">
                            <strong>Contact: </strong>
                            <asp:Label ID="contactLabel" runat="server" Text='<%# Bind("contact") %>'></asp:Label>
                            &nbsp;&nbsp; <strong>Phone:</strong>
                            <asp:Label ID="phone1Label" runat="server" Text='<%# Bind("phone1") %>'></asp:Label>
                            &nbsp;</td>
                        <td class="vertical" rowspan="10">
                            Tag#:
                            <asp:Label ID="Label4" runat="server" Text='<%# Eval("full_tag") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <strong>Submitted by:</strong> &nbsp;<asp:Label ID="submitted_byLabel" 
                                runat="server" Font-Bold="False" Text='<%# Bind("submitted_by") %>'></asp:Label>
                            on
                            <asp:Label ID="request_dateLabel" runat="server" 
                                Text='<%# Bind("request_date") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <strong>Routed to: </strong>
                            <asp:Label ID="routed_toLabel" runat="server" Text='<%# Bind("routed_to") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <strong>Approved by:</strong>
                            <asp:Label ID="approved_byLabel" runat="server" Font-Bold="False" 
                                Text='<%# Bind("approved_by") %>'></asp:Label>
                            on
                            <asp:Label ID="Label2" runat="server" Text='<%# Eval("approve_date") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <strong>Ordered by: </strong>
                            <asp:Label ID="purchaserLabel" runat="server" Text='<%# Bind("purchaser") %>'></asp:Label>
                            on
                            <asp:Label ID="order_dateLabel" runat="server" Text='<%# Bind("order_date") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <strong>ID#:</strong>
                            <asp:Label ID="accountLabel" runat="server" Text='<%# Bind("account") %>'></asp:Label>
                        </td>
                        <td>
                            <strong>Status:</strong>
                            <asp:Label ID="statusLabel" runat="server" Text='<%# Bind("status") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <strong>Shipping Pref:</strong><asp:Label ID="need_byLabel" runat="server" 
                                Text='<%# Bind("need_by") %>'></asp:Label>
                        </td>
                        <td>
                            <strong>Ship To: </strong>
                            <asp:Label ID="ship_toLabel" runat="server" Text='<%# Bind("ship_to") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <strong>Confirmation #:</strong>
                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("dafis_po_num") %>'></asp:Label>
                        </td>
                        <td>
                            <strong>Tag #:</strong><asp:Label ID="full_tagLabel" runat="server" 
                                Text='<%# Bind("full_tag") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <strong>Vendor contact: </strong>
                            <asp:Label ID="vendor_contactLabel" runat="server" 
                                Text='<%# Bind("vendor_contact") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="width: 100%">
                            <strong>Request comments: </strong>
                            <asp:Label ID="request_commentsLabel" runat="server" 
                                Text='<%# Bind("request_comments") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="height: 23px">
                            <strong>Approval Comments:</strong>
                            <asp:Label ID="approver_commentsLabel" runat="server" 
                                Text='<%# Bind("approver_comments") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <strong>Purchasing Comments:</strong>
                            <asp:Label ID="purchasing_commentsLabel" runat="server" 
                                Text='<%# Bind("purchasing_comments") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <table border="1" width="100%">
                                <tr>
                                    <th>
                                        Date</th>
                                    <th>
                                        Debit</th>
                                    <th>
                                        Credit</th>
                                    <th>
                                        Balance</th>
                                    <th>
                                        FPD Doc#/<br />
                                        Invoice #</th>
                                    <th>
                                        Invoice Date</th>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;<asp:Label ID="Label3" runat="server" 
                                            Text='<%# Eval("order_date", "{0:d}") %>'></asp:Label>
                                    </td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;<asp:Label ID="lblBalance" runat="server" Text="Label"></asp:Label>
                                    </td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        &nbsp;</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Repeater ID="Repeater1" runat="server" DataSourceID="sdsOrderDetails" 
                    OnItemDataBound="Repeater1_ItemDataBound">
                    <HeaderTemplate>
                        <strong>Items</strong>
                        <table border="1" style="text-align: left" width="100%">
                            <tr>
                                <th>
                                    Qty</th>
                                <th>
                                    Unit</th>
                                <th>
                                    Item/Catalog<br />
                                    No.</th>
                                <th>
                                    Description</th>
                                <th>
                                    Not to<br />
                                    Exceed<br />
                                    Amount</th>
                                <th>
                                    Unit
                                    <br />
                                    Cost</th>
                                <th>
                                    Total</th>
                            </tr>                        
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                            <%# Eval("quantity") %>
                                &nbsp;</td>
                            <td>
                            <%#Eval("unit")%>
                                &nbsp;</td>
                            <td>
                            <%#Eval("item")%>
                                &nbsp;</td>
                            <td>
                            <%# Eval("description") %>
                                &nbsp;</td>
                            <td>
                            <%#Eval("exceed", "{0:c}")%>
                                &nbsp;</td>
                            <td>
                            <%#Eval("cost","{0:c}")%>
                                &nbsp;</td>
                            <td>
                            <%#Eval("total", "{0:c}")%>
                                &nbsp;</td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr>
                            <td align="right" colspan="6">
                                <strong>Material:</strong></td>
                            <td>
                                <asp:Label ID="lblShMat" runat="server" Text=""></asp:Label>
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td align="right" colspan="6">
                                <strong>Subtotal:</strong></td>
                            <td>
                                <asp:Label ID="lblsum" runat="server" Text=""></asp:Label>
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td align="right" colspan="6">
                                <strong>Tax:</strong></td>
                            <td>
                                <asp:Label ID="lblTax" runat="server" Text=""></asp:Label>
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td align="right" colspan="6">
                                <strong>Shipping/Handling:</strong></td>
                            <td>
                                <asp:Label ID="lblSandH" runat="server" Text=""></asp:Label>
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td align="right" colspan="6">
                                <strong>Total:</strong></td>
                            <td>
                                <asp:Label ID="lblTotal" runat="server" Text=""></asp:Label>
                                &nbsp;</td>
                        </tr>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                &nbsp; &nbsp;&nbsp;<br />
                &nbsp;<br />
            </ItemTemplate>
        </asp:FormView>
        <a class="noprint" href="process.aspx">Process more Orders</a></div>
    </form>
</body>
</html>
