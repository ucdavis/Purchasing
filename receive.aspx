<%@ Page Title="" Language="VB" MasterPageFile="~/ops.master" AutoEventWireup="false" CodeFile="receive.aspx.vb" Inherits="receive" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:SqlDataSource ID="sdsOrders" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        FilterExpression="{1} LIKE '{0}'" SelectCommand="recd_orders_select" 
        SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="rblShowlast" Name="input" 
                PropertyName="SelectedValue" Type="String" />
            <asp:SessionParameter Name="emp_id" SessionField="emp_id" Type="Int32" />
        </SelectParameters>
        <FilterParameters>
            <asp:ControlParameter ControlID="txtSearch" Name="search_text" 
                PropertyName="Text" />
            <asp:ControlParameter ControlID="ddlsearchby" Name="search_what" 
                PropertyName="SelectedValue" />
        </FilterParameters>
    </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsOrderRequest" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" DataSourceMode="DataReader" 
            SelectCommand="recd_order_details" SelectCommandType="StoredProcedure" 
            UpdateCommand="update_process_order" 
        UpdateCommandType="StoredProcedure">
            <UpdateParameters>
                <asp:Parameter Name="RETURN_VALUE" Type="Int32" Direction="ReturnValue" />
                <asp:Parameter Name="vendor_id" Type="Int32" />
                <asp:Parameter Name="need_by" Type="String" />
                <asp:Parameter Name="order_id" Type="Int32" />
                <asp:Parameter Name="account" Type="String" />
            </UpdateParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="gvOrders" Name="order_id" 
                    PropertyName="SelectedValue" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsOrderDetails" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" DataSourceMode="DataReader" 
            SelectCommand="recd_info_details" SelectCommandType="StoredProcedure" 
            UpdateCommand="update_details_from_recieve" 
        UpdateCommandType="StoredProcedure">
            <UpdateParameters>
                <asp:Parameter Name="detail_id" Type="Int32" />
                <asp:Parameter Name="recd_quantity" Type="Int32" />
                <asp:Parameter Name="shipping_notes" Type="String" />
                <asp:SessionParameter Name="receiver" SessionField="emp_id" Type="Int32" />
            </UpdateParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="fvDetails" Name="order_id" 
                    PropertyName="SelectedValue" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsFiles" runat="server" ConnectionString="<%$ ConnectionStrings:OPSConn %>"
        SelectCommand="select_order_file" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="fvDetails" Name="order_id" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="sdsChanges" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        SelectCommand="select_changes" SelectCommandType="StoredProcedure">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="fvDetails" Name="order_id" 
                                PropertyName="SelectedValue" Type="Int32" />
                        </SelectParameters>
    </asp:SqlDataSource>
                    <asp:FormView ID="fvDetails" runat="server" DataSourceID="sdsOrderRequest" 
        EnableViewState="False" Width="95%" DataKeyNames="order_id">
                        <ItemTemplate>
                            Order Request Submitted by
                            <asp:Label ID="submitted_byLabel" runat="server" Font-Bold="True" 
                Text='<%# Eval("submitted_by") %>'></asp:Label>
                            and Approved by
                            <asp:Label ID="approved_byLabel" runat="server" Font-Bold="True" 
                Text='<%# Eval("approved_by") %>'></asp:Label>
                            <br />
                            <table border="1" style="text-align: left" width="100%">
                                <tr>
                                    <td rowspan="9">
                                        <strong>Vendor:</strong><br />
                                        <asp:Label ID="vendor_nameLabel" runat="server" 
                            Text='<%# Eval("vendor_name") %>'></asp:Label>
                                        <br />
                                        <asp:Label ID="addressLabel" runat="server" Text='<%# Eval("address") %>'></asp:Label>
                                        <br />
                                        <asp:Label ID="cszLabel" runat="server" Text='<%# Eval("csz") %>'></asp:Label>
                                        <br />
                                        <br />
                                        <strong>Phone:</strong>
                                        <asp:Label ID="phoneLabel" runat="server" Text='<%# Eval("phone") %>'></asp:Label>
                                        <br />
                                        <strong>Fax: </strong>
                                        <asp:Label ID="faxLabel" runat="server" Text='<%# Eval("fax") %>'></asp:Label>
                                        <br />
                                        <br />
                                        <strong>Web:</strong>
                                        <asp:HyperLink ID="HyperLink1" runat="server" 
                            NavigateUrl='<%# "http://" & Eval("website") %>' Target="_blank" 
                            Text='<%# Eval("vendor_name") %>'></asp:HyperLink>
                                    </td>
                                    <td colspan="2">
                                        <strong>Authorizer: </strong>
                                        <asp:Label ID="routed_toLabel" runat="server" 
                                    Text='<%# Eval("routed_to_name") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>ID#:&nbsp; </strong>
                                        <asp:Label ID="lblID" runat="server" Text='<%# Eval("account") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <strong>Tag #:</strong><asp:Label ID="full_tagLabel" runat="server" 
                            Text='<%# Eval("full_tag") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>Shipping Pref:</strong>
                                        <asp:Label ID="lblShip" runat="server" Text='<%# Eval("need_by") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <strong>Ship To: </strong>
                                        <asp:Label ID="ship_toLabel" runat="server" Text='<%# Eval("ship_to") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <strong>Contact: </strong>
                                        <asp:Label ID="contactLabel" runat="server" Text='<%# Eval("contact") %>'></asp:Label>
                                        &nbsp;&nbsp; <strong>Phone:</strong>
                                        <asp:Label ID="phone1Label" runat="server" Text='<%# Eval("phone1") %>'></asp:Label>
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>Radioactive?: </strong>&nbsp;<asp:CheckBox ID="radioactiveCheckBox" 
                            runat="server" Checked='<%# Eval("radioactive") %>' Enabled="false" />
                                        <strong>RUA #: </strong>
                                        <asp:Label ID="rua_numLabel" runat="server" Text='<%# Eval("rua_num") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <strong>Status:</strong>
                                        <asp:Label ID="statusLabel" runat="server" Text='<%# Eval("status") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>Cell phone?: </strong>
                                        <asp:CheckBox ID="cellCheckBox" runat="server" Checked='<%# Eval("cell_phone") %>' 
                            Enabled="False" />
                                        &nbsp;<asp:Label ID="lblCellOwner" runat="server" 
                            Text='<%# Eval("cell_owner", "<b>Owner:</b> {0}") %>'></asp:Label>
                                        &nbsp;<asp:Label ID="lblNumber" runat="server" 
                            Text='<%# Eval("cell_number", "<b>Number:</b> {0}") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <strong>Submit Date: </strong>
                                        <asp:Label ID="request_dateLabel" runat="server" 
                            Text='<%# Eval("request_date") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <strong>Quote is being sent to purchasing desk?: </strong>
                                        <asp:CheckBox ID="attachmentCheckBox" runat="server" 
                            Checked='<%# Eval("attachment") %>' Enabled="false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <strong>Vendor contact: </strong>
                                        <asp:Label ID="vendor_contactLabel" runat="server" 
                            Text='<%# Eval("vendor_contact") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <strong>Request comments: </strong>
                                        <asp:Label ID="request_commentsLabel" runat="server" 
                            Text='<%# Eval("request_comments") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <strong>Approved by:</strong>
                                        <asp:Label ID="lblApprovedBy" runat="server" Text='<%# Eval("approved_by") %>'></asp:Label>
                                        &nbsp;<asp:Label ID="lblApprovedDate" runat="server" 
                            Text='<%# Eval("approve_date","on {0}") %>'></asp:Label>
                                        &nbsp;
                                        <asp:Label ID="approver_commentsLabel" runat="server" 
                            Text='<%# Eval("approver_comments","<b>Comments:</b>{0}") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3" style="height: 23px">
                                        <strong>Budget:</strong>
                                        <asp:Label ID="lblBudgetName" runat="server" Text='<%# Eval("budget_by") %>'></asp:Label>
                                        &nbsp;<asp:Label ID="lblBudgetDate" runat="server" 
                            Text='<%# Eval("budget_date", "on {0}") %>'></asp:Label>
                                        &nbsp;<asp:Label ID="lblBudgetComment" runat="server" 
                            Text='<%# Eval("budget_comments", "<b>Comments:</b> {0}") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                            <td colspan="3">
                                <strong>Order by: </strong>
                                <asp:Label ID="purchaserLabel" runat="server" Text='<%# Eval("purchaser") %>'></asp:Label>
                                &nbsp;<asp:Label ID="order_dateLabel" runat="server" 
                        Text='<%# Eval("order_date","on {0}") %>'></asp:Label>
                                &nbsp;<asp:Label ID="purchasing_commentsLabel" runat="server" 
                        Text='<%# Eval("purchasing_comments", "<b>Comments:</b> {0}") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <strong>DaFIS PO:</strong>
                                <asp:Label ID="lblDaFISPO" runat="server" Text='<%# Eval("dafis_po_num") %>'></asp:Label>
                            </td>
                        </tr>
                            </table>
                            <br />
                            <%--<asp:Repeater ID="Repeater1" runat="server" DataSourceID="sdsOrderDetails" 
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
                                            <%# Eval("quantity") %>&nbsp;</td>
                                        <td>
                                            <%#Eval("unit")%>&nbsp;</td>
                                        <td>
                                            <%#Eval("item")%>&nbsp;</td>
                                        <td>
                                            <%# Eval("description") %>&nbsp;</td>
                                        <td>
                                            <%#Eval("exceed", "{0:c}")%>&nbsp;</td>
                                        <td>
                                            <%#Eval("cost","{0:c}")%>&nbsp;</td>
                                        <td>
                                            <%#Eval("total", "{0:c}")%>&nbsp;</td>
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
                            </asp:Repeater>--%>
                            <asp:GridView ID="gvItems" runat="server" 
                                AutoGenerateColumns="False" DataKeyNames="detail_id" 
                                DataSourceID="sdsOrderDetails">
                                <Columns>
                                    <asp:CommandField ShowEditButton="True" />
                                    <asp:TemplateField ShowHeader="False">
                                        <ItemTemplate>                                            
                                            <asp:Button ID="lnkRecieve" runat="server" CausesValidation="false" CommandArgument='<%# Bind("detail_id") %>'
                                                Text="Rec'd" onclick="LinkButton1_Click"></asp:Button>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="quantity" HeaderText="Qty" ReadOnly="True" 
                                        SortExpression="quantity" />
                                    <asp:BoundField DataField="recd_quantity" HeaderText="Rec'd" 
                                        SortExpression="recd_quantity" />
                                    <asp:BoundField DataField="unit" HeaderText="Unit" ReadOnly="True" SortExpression="unit" />
                                    <asp:BoundField DataField="item" HeaderText="Item No." ReadOnly="True"  SortExpression="item" />
                                    <asp:BoundField DataField="description" HeaderText="Description" ReadOnly="True" 
                                        SortExpression="description" />
                                    <asp:TemplateField HeaderText="Shipping Notes" SortExpression="shipping_notes">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBox1" runat="server" Rows="5" 
                                                Text='<%# Bind("shipping_notes") %>' TextMode="MultiLine" Width="300px"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("shipping_notes") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="exceed" HeaderText="Not to Exceed" ReadOnly="True" 
                                        SortExpression="exceed" DataFormatString="{0:c}" />
                                    <asp:BoundField DataField="cost" HeaderText="Unit Cost" ReadOnly="True" 
                                        SortExpression="cost" DataFormatString="{0:c}" />
                                    <asp:BoundField DataField="total" HeaderText="Total" ReadOnly="True" 
                                        SortExpression="total" DataFormatString="{0:c}" />
                                </Columns>
                            </asp:GridView>
                            <br />                            
                            <table width="100%" border="0">
                                <tr>                     
                                    <td rowspan="5"><asp:Button ID="btnRecieveAll" runat="server" CausesValidation="false" 
                                            CommandArgument='<%# Bind("order_id") %>' Text="Mark all items rec'd" 
                                            onclick="btnRecieveAll_Click" />  &nbsp;<br />
                                        Received Status:
                                        <asp:DropDownList ID="ddlStatus" runat="server" 
                                            SelectedValue='<%# Bind("recd_status") %>'>
                                            <asp:ListItem>Incomplete</asp:ListItem>
                                            <asp:ListItem>Partial</asp:ListItem>
                                            <asp:ListItem>Complete</asp:ListItem>
                                        </asp:DropDownList>
                                        Received Comments:
                                        <asp:TextBox ID="txtRecdComments" runat="server" Rows="5" TextMode="MultiLine" 
                                            Width="400px" Text='<%# Bind("recd_comments") %>'></asp:TextBox>
                                    </td>               
                                    <td align="right" style="width: 100%"><strong>Material:</strong></td>
                                    <td>&nbsp;</td>
                                    <td><asp:Label ID="materialLablel" runat="server" Text='<%# Eval("ship_mat", "{0:c}") %>'></asp:Label></td>
                                </tr>
                                <tr>               
                                    <td align="right" style="width: 100%"><strong>Subtotal:</strong></td>
                                    <td>&nbsp;</td>
                                    <td><asp:Label ID="subtotalLabel" runat="server" Text='<%# Eval("subtotal", "{0:c}") %>'></asp:Label></td>
                                </tr>
                                  <tr>             
                                    <td align="right" style="width: 100%"><strong>Tax:</strong></td>
                                    <td>&nbsp;</td>
                                    <td><asp:Label ID="Label7" runat="server" Text='<%# Eval("tax_total", "{0:c}") %>'></asp:Label></td>
                                </tr>
                                  <tr>             
                                    <td align="right" style="width: 100%"><strong>Shipping/Handling:</strong></td>
                                    <td>&nbsp;</td>
                                    <td><asp:Label ID="Label8" runat="server" Text='<%# Eval("ship_cost", "{0:c}") %>'></asp:Label></td>
                                </tr>
                                 <tr>                                    
                                    <td align="right" style="width: 100%"><strong>Total:</strong></td>
                                    <td>&nbsp;</td>
                                    <td><asp:Label ID="Label9" runat="server" Text='<%# Eval("order_total", "{0:c}") %>'></asp:Label></td>
                                </tr>
                            </table>
                            <br />
                            <br />
                            &nbsp;<asp:Button ID="btnEdit" runat="server"  
                        Text="Update Status" CausesValidation="False" onclick="btnEdit_Click" />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnCancel" runat="server" onclick="btnCancel_Click1" 
                        Text="Cancel Selection" CausesValidation="False" />
                            <asp:GridView ID="gvAttachments" runat="server" AutoGenerateColumns="False" 
                        DataSourceID="sdsFiles" EnableViewState="False" SkinID="notfull">
                                <Columns>
                                    <asp:HyperLinkField DataNavigateUrlFields="order_id,attach_id" 
                                DataNavigateUrlFormatString="filedownload.aspx?order_id={0}&amp;attach_id={1}" 
                                DataTextField="attach_name" DataTextFormatString="{0}" 
                                HeaderText="Attachments" />
                                </Columns>
                            </asp:GridView>
                            <asp:GridView ID="GridView3" runat="server" AllowSorting="True" 
                                AutoGenerateColumns="False" DataSourceID="sdsChanges" SkinID="noempty">
                                <Columns>
                                    <asp:BoundField DataField="column_change" HeaderText="Column Changed" 
                                        SortExpression="column_change" />
                                    <asp:BoundField DataField="old_value" HeaderText="Old Value" 
                                        SortExpression="old_value" />
                                    <asp:BoundField DataField="new_value" HeaderText="New Value" 
                                        SortExpression="new_value" />
                                    <asp:BoundField DataField="changed_by" HeaderText="By" ReadOnly="True" 
                                        SortExpression="changed_by" />
                                    <asp:BoundField DataField="date_change" HeaderText="Date" 
                                        SortExpression="date_change" />
                                </Columns>
                            </asp:GridView>
                            <br />
                            &nbsp;<br />
                        </ItemTemplate>
                        <EditItemTemplate>
                            Order Request Submitted by
                            <asp:Label ID="submitted_byLabel0" runat="server" Font-Bold="True" 
                Text='<%# Eval("submitted_by") %>'></asp:Label>
                            and Approved by
                            <asp:Label ID="approved_byLabel0" runat="server" Font-Bold="True" 
                Text='<%# Eval("approved_by") %>'></asp:Label>
                            <br />
                            <table border="1" style="text-align: left" width="100%">
                                <tr>
                                    <td rowspan="10">
                                        <strong>Vendor:</strong><br />
                                        <asp:Label ID="vendor_nameLabel0" runat="server" 
                            Text='<%# Eval("vendor_name") %>'></asp:Label>
                                        <br />
                                        <asp:Label ID="addressLabel0" runat="server" Text='<%# Eval("address") %>'></asp:Label>
                                        <br />
                                        <asp:Label ID="cszLabel0" runat="server" Text='<%# Eval("csz") %>'></asp:Label>
                                        <br />
                                        <br />
                                        <strong>Phone:</strong>
                                        <asp:Label ID="phoneLabel0" runat="server" Text='<%# Eval("phone") %>'></asp:Label>
                                        <br />
                                        <strong>Fax: </strong>
                                        <asp:Label ID="faxLabel0" runat="server" Text='<%# Eval("fax") %>'></asp:Label>
                                        <br />
                                        <br />
                                        <strong>Web:</strong>
                                        <asp:HyperLink ID="HyperLink2" runat="server" 
                            NavigateUrl='<%# "http://" & Eval("website") %>' Target="_blank" 
                            Text='<%# Eval("vendor_name") %>'></asp:HyperLink>
                                    </td>
                                    <td colspan="2">
                                        <b>Vendor:</b>
                                        <asp:DropDownList ID="DropDownList1" runat="server" 
                                    DataSourceID="sdsInitialVendors" DataTextField="vendor_name" 
                                    DataValueField="vendor_id" SelectedValue='<%# Bind("vendor_id") %>'>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <strong>Contact: </strong>
                                        <asp:Label ID="contactLabel0" runat="server" Text='<%# Eval("contact") %>'></asp:Label>
                                        &nbsp;&nbsp;&nbsp;<strong>Phone:</strong>
                                        <asp:Label ID="phone1Label0" runat="server" Text='<%# Eval("phone1") %>'></asp:Label>
                                        &nbsp;</td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <strong>Authorizer: </strong>
                                        <asp:Label ID="routed_toLabel0" runat="server" 
                                    Text='<%# Eval("routed_to_name") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>ID#: </strong>
                                        <asp:TextBox ID="txtAccount" runat="server" Text='<%# Bind("account") %>' 
                                    Width="200px"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" 
                                    ControlToValidate="txtAccount" ErrorMessage="RequiredFieldValidator"></asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <strong>Tag #:</strong><asp:Label ID="full_tagLabel0" runat="server" 
                            Text='<%# Eval("full_tag") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>Shipping Pref:</strong>
                                        <asp:DropDownList ID="ddlShip" runat="server" DataSourceID="xdsShipping" 
                            DataTextField="label" DataValueField="label" 
                            SelectedValue='<%# Bind("need_by") %>'>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <strong>Ship To: </strong>
                                        <asp:Label ID="ship_toLabel0" runat="server" Text='<%# Eval("status") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>Radioactive?: </strong>&nbsp;<asp:CheckBox ID="radioactiveCheckBox0" 
                            runat="server" Checked='<%# Eval("radioactive") %>' Enabled="False" />
                                        <strong>RUA #: </strong>
                                        <asp:Label ID="rua_numLabel0" runat="server" Text='<%# Eval("rua_num") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <strong>Status:</strong>
                                        <asp:Label ID="statusLabel0" runat="server" Text='<%# Eval("status") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>Cell phone?: </strong>
                                        <asp:CheckBox ID="cellCheckBox0" runat="server" Checked='<%# Eval("cell_phone") %>' 
                            Enabled="False" />
                                        &nbsp;<asp:Label ID="lblCellOwner0" runat="server" 
                            Text='<%# Eval("cell_owner", "<b>Owner:</b> {0}") %>'></asp:Label>
                                        &nbsp;<asp:Label ID="lblNumber0" runat="server" 
                            Text='<%# Eval("cell_number", "<b>Number:</b> {0}") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <strong>Submit Date: </strong>
                                        <asp:Label ID="request_dateLabel0" runat="server" 
                            Text='<%# Eval("request_date") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <strong>Quote is being sent to purchasing desk?: </strong>
                                        <asp:CheckBox ID="attachmentCheckBox0" runat="server" 
                            Checked='<%# Eval("attachment") %>' Enabled="False" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <strong>Vendor contact: </strong>
                                        <asp:Label ID="vendor_contactLabel0" runat="server" 
                            Text='<%# Eval("request_comments") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <strong>Request comments: </strong>
                                        <asp:Label ID="request_commentsLabel0" runat="server" 
                            Text='<%# Eval("approved_by") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <strong>Approved by:</strong>
                                        <asp:Label ID="lblApprovedBy0" runat="server" Text='<%# Eval("budget_by") %>'></asp:Label>
                                        &nbsp;<asp:Label ID="lblApprovedDate0" runat="server" 
                            Text='<%# Eval("budget_date", "on {0}") %>'></asp:Label>
                                        &nbsp;
                                        <asp:Label ID="approver_commentsLabel0" runat="server" 
                            Text='<%# Eval("budget_comments", "<b>Comments:</b> {0}") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3" style="height: 23px">
                                        <strong>Budget:</strong>
                                        <asp:Label ID="lblBudgetName0" runat="server" Text='<%# Eval("budget_by") %>'></asp:Label>
                                        &nbsp;<asp:Label ID="lblBudgetDate0" runat="server" 
                            Text='<%# Eval("budget_date", "on {0}") %>'></asp:Label>
                                        &nbsp;<asp:Label ID="lblBudgetComment0" runat="server" 
                            Text='<%# Eval("budget_comments", "<b>Comments:</b> {0}") %>'></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <br />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:FormView ID="FormView2" 
                        runat="server" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" 
                        DataKeyNames="detail_id" DataSourceID="sdsOrderDetails" DefaultMode="Insert" 
                        Width="100%">
                                <EditItemTemplate>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <table border="1" width="100%">
                                        <tr>
                                            <th>
                                                Quantity</th>
                                            <th>
                                                Units</th>
                                            <th>
                                                Item</th>
                                            <th>
                                                Description</th>
                                            <th>
                                                Cost</th>
                                        </tr>
                                        <tr>
                                            <td>
                                                &nbsp;<asp:TextBox ID="quantityTextBox" runat="server" 
                                            Text='<%# Bind("quantity") %>' Width="25px"></asp:TextBox>
                                                <asp:CompareValidator ID="CompareValidator2" runat="server" 
                                            ControlToValidate="quantityTextBox" 
                                            ErrorMessage="Quantity for item must be a number" Operator="DataTypeCheck" 
                                            Type="Integer" ValidationGroup="insertitem">*</asp:CompareValidator>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                            ControlToValidate="quantityTextBox" ErrorMessage="You must supply a quantity" 
                                            ValidationGroup="insertitem">*</asp:RequiredFieldValidator>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="unitTextBox" runat="server" Text='<%# Bind("unit") %>' 
                                            Width="75px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                            ControlToValidate="unitTextBox" ErrorMessage="You must supply a units value" 
                                            ValidationGroup="insertitem">*</asp:RequiredFieldValidator>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="itemTextBox" runat="server" Text='<%# Bind("item") %>' 
                                            Width="75px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="descriptionTextBox" runat="server" Rows="3" 
                                            Text='<%# Bind("description") %>' TextMode="MultiLine" Width="300px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                                            ControlToValidate="descriptionTextBox" 
                                            ErrorMessage="You must supply a description entry" 
                                            ValidationGroup="insertitem">*</asp:RequiredFieldValidator>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="costTextBox" runat="server" Text='<%# Bind("cost") %>' 
                                            Width="100px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                            ControlToValidate="costTextBox" 
                                            ErrorMessage="You must entry a cost for this item" 
                                            ValidationGroup="insertitem">*</asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="CompareValidator1" runat="server" 
                                            ControlToValidate="costTextBox" 
                                            ErrorMessage="Cost must be a valid dollar amount" Operator="DataTypeCheck" 
                                            Type="Currency" ValidationGroup="insertitem">*</asp:CompareValidator>
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                    <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" 
                                CommandName="Insert" Text="Insert" ValidationGroup="insertitem"></asp:LinkButton>
                                    &nbsp;
                                </InsertItemTemplate>
                                <ItemTemplate>
                                </ItemTemplate>
                                <HeaderTemplate>
                                    Add New Item to Order:
                                </HeaderTemplate>
                            </asp:FormView>
                            <br />
                            <br />
                            <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" 
                        DataKeyNames="detail_id" DataSourceID="sdsOrderDetails" Width="90%">
                                <Columns>
                                    <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
                                    <asp:TemplateField HeaderText="quantity" SortExpression="quantity">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="EditQuantity" runat="server" Text='<%# Bind("quantity") %>' 
                                        Width="50px"></asp:TextBox>
                                            <asp:RequiredFieldValidator
                                        ID="RequiredFieldValidator5" runat="server" 
                                        ErrorMessage="Quantity 
                                    is required" ControlToValidate="EditQuantity"></asp:RequiredFieldValidator>
                                            <asp:CompareValidator ID="CompareValidator3" runat="server" 
                                        ControlToValidate="EditQuantity" ErrorMessage="Quantity must be a valid integer" 
                                        Operator="DataTypeCheck" Type="Integer"></asp:CompareValidator>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("quantity") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="unit" SortExpression="unit">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtEditUnit" runat="server" Text='<%# Bind("unit") %>' 
                                        Width="75px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                                        ControlToValidate="txtEditUnit" ErrorMessage="Unit is a required field"></asp:RequiredFieldValidator>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("unit") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="item" SortExpression="item">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtItemEdit" runat="server" Text='<%# Bind("item") %>' 
                                        Width="75px"></asp:TextBox>
                                            <asp:CompareValidator ID="CompareValidator6" runat="server" 
                                        ControlToValidate="txtItemEdit" ErrorMessage="CompareValidator" 
                                        Operator="DataTypeCheck"></asp:CompareValidator>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("item") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="description" SortExpression="description">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtDescEdit" runat="server" Text='<%# Bind("description") %>' 
                                        Rows="5" TextMode="MultiLine"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                                        ControlToValidate="txtDescEdit" 
                                        ErrorMessage="Description is a required field"></asp:RequiredFieldValidator>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("description") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="exceed" SortExpression="exceed">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtExceedEdit" runat="server" Text='<%# Bind("exceed", "{0:n2}") %>' 
                                        Width="50px"></asp:TextBox>
                                            <asp:CompareValidator ID="CompareValidator5" runat="server" 
                                        ControlToValidate="txtExceedEdit" 
                                        ErrorMessage="Not to Exceed must be a valid currency amount" 
                                        Operator="DataTypeCheck" Type="Currency"></asp:CompareValidator>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label6" runat="server" Text='<%# Bind("exceed", "{0:N2}") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="cost" SortExpression="cost">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtCostEdit" runat="server" 
                                        Text='<%# Bind("cost", "{0:n2}") %>' Width="50px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                                        ControlToValidate="txtCostEdit" ErrorMessage="Cost is a required field"></asp:RequiredFieldValidator>
                                            <asp:CompareValidator ID="CompareValidator4" runat="server" 
                                        ControlToValidate="txtCostEdit" 
                                        ErrorMessage="Cost must be a valid currency amount" Operator="DataTypeCheck" 
                                        Type="Currency"></asp:CompareValidator>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("cost", "{0:n2}") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="total" DataFormatString="{0:n2}" HeaderText="total" 
                                ReadOnly="True" SortExpression="total">
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                            <br />
                            &nbsp;<asp:Button ID="btnUpdate" runat="server" CommandName="Update" 
                        Text="Update Order" />
                            &nbsp;&nbsp;
                            <asp:Button ID="btnCancel0" runat="server" CommandName="Cancel" 
                        Text="Cancel Edit" />
                            <br />
                        </EditItemTemplate>
                    </asp:FormView>
    <br />
            <asp:Panel ID="Panel1" runat="server" CssClass="noprint" 
        EnableViewState="False" Width="100%">
                Show Last:
                <asp:RadioButtonList ID="rblShowlast" runat="server" EnableViewState="False" 
            RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Selected="True" Value="2 weeks |">2 weeks |</asp:ListItem>
                    <asp:ListItem Value="Month |">Month |</asp:ListItem>
                    <asp:ListItem Value="2 Months |">2 Months |</asp:ListItem>
                    <asp:ListItem Value="4 Months |">4 Months |</asp:ListItem>
                    <asp:ListItem Value="6 Months |">6 Months |</asp:ListItem>
                    <asp:ListItem Value="Year |">Year |</asp:ListItem>
                    <asp:ListItem Value="All">All</asp:ListItem>
                </asp:RadioButtonList>
                <br />
                Search By:&nbsp;<asp:DropDownList ID="ddlsearchby" runat="server" 
            EnableViewState="False">
                    <asp:ListItem Value="full_tag">Tag #</asp:ListItem>
                    <asp:ListItem>Description</asp:ListItem>
                    <asp:ListItem Value="order_date">Order Date</asp:ListItem>
                    <asp:ListItem Value="Vendor_name">Vendor</asp:ListItem>
                    <asp:ListItem Value="lab">lab</asp:ListItem>
                    <asp:ListItem Value="item">Catalog number</asp:ListItem>
                </asp:DropDownList>
                Search Text:
                <asp:TextBox ID="txtSearch" runat="server" EnableViewState="False"></asp:TextBox>
                (use % for wildcard)
                <asp:Button ID="Button1" runat="server" EnableViewState="False" 
            Text="Update Search" />
            </asp:Panel>
            <asp:GridView ID="gvOrders" runat="server" AllowSorting="True" 
        AutoGenerateColumns="False" DataKeyNames="order_id" 
        DataSourceID="sdsOrders">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="tag" HeaderText="Tag" ReadOnly="True" 
                        SortExpression="tag" />
                    <asp:BoundField DataField="name" HeaderText="Ordered By" ReadOnly="True" 
                        SortExpression="name" />
                    <asp:BoundField DataField="vendor_name" HeaderText="Vendor" 
                        SortExpression="vendor_name" />
                    <asp:BoundField DataField="order_date" HeaderText="Ordered" ReadOnly="True" 
                        SortExpression="sort_date" />
                    <asp:BoundField DataField="dafis_po_num" HeaderText="Conf #" 
                        SortExpression="dafis_po_num" />
                    <asp:BoundField DataField="recd_status" HeaderText="Recd. Status" 
                        ReadOnly="True" SortExpression="recd_status" />
                </Columns>
    </asp:GridView>
</asp:Content>

