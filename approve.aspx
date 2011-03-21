<%@ Page Language="VB" MasterPageFile="~/ops.master" AutoEventWireup="false" CodeFile="approve.aspx.vb" Inherits="approve" title="" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:SqlDataSource ID="sdsAllOrders" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        FilterExpression="{1} LIKE '{0}'" SelectCommand="nonadmin_approve" 
        SelectCommandType="StoredProcedure">
        <FilterParameters>
            <asp:ControlParameter ControlID="txtSearch" Name="searchtext" 
                PropertyName="Text" />
            <asp:ControlParameter ControlID="ddlsearchby" Name="searchwhat" 
                PropertyName="SelectedValue" />
        </FilterParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="rblShowlast" DefaultValue="" Name="input" 
                PropertyName="SelectedValue" Type="String" />
            <asp:SessionParameter Name="emp_id" SessionField="emp_id" Type="Int32" />
            <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsOrderRequest" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" DataSourceMode="DataReader" 
        SelectCommand="order_info" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridView1" Name="order_id" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsOrderDetails" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" DataSourceMode="DataReader" 
        SelectCommand="order_info_details" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridView1" Name="order_id" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:XmlDataSource ID="xdsShipping" runat="server" DataFile="~/ddlElements.xml" 
        XPath="root/shipping/value"></asp:XmlDataSource>
    <asp:SqlDataSource ID="sdsFiles" runat="server" ConnectionString="<%$ ConnectionStrings:OPSConn %>"
        SelectCommand="select_order_file" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridView1" Name="order_id" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsChanges" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        SelectCommand="select_changes" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="FormView1" Name="order_id" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <br />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
        ValidationGroup="Deny" />
            <asp:ValidationSummary ID="ValidationSummary2" runat="server" 
        ValidationGroup="Approve" />
            <br />
            <asp:FormView ID="FormView1" runat="server" DataSourceID="sdsOrderRequest" 
        EnableViewState="False" Width="95%" DataKeyNames="order_id">
                <ItemTemplate>
                    Order Request Submitted by
                    <asp:Label ID="submitted_byLabel" runat="server" Font-Bold="True" 
                Text='<%# Bind("submitted_by") %>'></asp:Label>
                    and Approved by
                    <asp:Label ID="approved_byLabel" runat="server" Font-Bold="True" 
                Text='<%# Bind("approved_by") %>'></asp:Label>
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
                                <strong>Authorizer: </strong>
                                <asp:Label ID="routed_toLabel" runat="server" Text='<%# Bind("routed_to") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>ID#:<asp:TextBox ID="txtID" runat="server" 
                            Text='<%# Bind("account") %>' Width="300px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                            ControlToValidate="txtID" ErrorMessage="Account ID must be supplied." 
                            ValidationGroup="Approve">*</asp:RequiredFieldValidator>
                                </strong>&nbsp;
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                            ControlToValidate="txtID" ErrorMessage="Account ID must be supplied." 
                            ValidationGroup="Deny">*</asp:RequiredFieldValidator>
                            </td>
                            <td>
                                <strong>Tag #:</strong><asp:Label ID="full_tagLabel" runat="server" 
                            Text='<%# Bind("full_tag") %>'></asp:Label>
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
                                <asp:Label ID="ship_toLabel" runat="server" Text='<%# Bind("ship_to") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <strong>Contact: </strong>
                                <asp:Label ID="contactLabel" runat="server" Text='<%# Bind("contact") %>'></asp:Label>
                                &nbsp;&nbsp; <strong>Phone:</strong>
                                <asp:Label ID="phone1Label" runat="server" Text='<%# Bind("phone1") %>'></asp:Label>
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td>
                                <strong>Radioactive?: </strong>&nbsp;<asp:CheckBox ID="radioactiveCheckBox" 
                            runat="server" Checked='<%# Bind("radioactive") %>' Enabled="false" />
                                <strong>RUA #: </strong>
                                <asp:Label ID="rua_numLabel" runat="server" Text='<%# Bind("rua_num") %>'></asp:Label>
                            </td>
                            <td>
                                <strong>Status:</strong>
                                <asp:Label ID="statusLabel" runat="server" Text='<%# Bind("status") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>Controlled Substance?: </strong>
                                <asp:CheckBox ID="csCheckBox" runat="server" Checked='<%# Bind("controlled_substance") %>' 
                            Enabled="False" />
                                &nbsp;<asp:Label ID="lblCSClass" runat="server" 
                            Text='<%# Eval("cs_class", "<b>Class:</b> {0}") %>'></asp:Label>
                                &nbsp;<asp:Label ID="lblCSUse" runat="server" 
                            Text='<%# Eval("cs_use", "<b>Use:</b> {0}") %>'></asp:Label>
                                &nbsp;<asp:Label ID="lblCSStore" runat="server" 
                                    Text='<%# Eval("cs_store", "<b>Storage:</b> {0}") %>'></asp:Label>
                                &nbsp;<asp:Label ID="lblCSCustodian" runat="server" 
                                    Text='<%# Eval("cs_custodian", "<b>Custodian:</b> {0}") %>'></asp:Label>
                                &nbsp;<asp:Label ID="lblCSUsers" runat="server" 
                                    Text='<%# Eval("cs_user", "<b>User(s):</b> {0}") %>'></asp:Label>
                            </td>
                            <td>
                                <strong>Submit Date: </strong>
                                <asp:Label ID="request_dateLabel" runat="server" 
                            Text='<%# Bind("request_date") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <strong>Quote is being sent to purchasing desk?: </strong>
                                <asp:CheckBox ID="attachmentCheckBox" runat="server" 
                            Checked='<%# Bind("attachment") %>' Enabled="false" />
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
                            <td colspan="2">
                                <strong>Request comments: </strong>
                                <asp:Label ID="request_commentsLabel" runat="server" 
                            Text='<%# Bind("request_comments") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <strong>Approved by:</strong>
                                <asp:Label ID="lblApprovedBy" runat="server" Text='<%# Bind("approved_by") %>'></asp:Label>
                                &nbsp;<asp:Label ID="lblApprovedDate" runat="server" 
                            Text='<%# Eval("approve_date","on {0}") %>'></asp:Label>
                                &nbsp;
                                <asp:Label ID="approver_commentsLabel" runat="server" 
                            Text='<%# Bind("approver_comments","<b>Comments:</b>{0}") %>'></asp:Label>
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
                                <asp:Label ID="purchaserLabel" runat="server" Text='<%# Bind("purchaser") %>'></asp:Label>
                                &nbsp;<asp:Label ID="order_dateLabel" runat="server" 
                            Text='<%# Bind("order_date","on {0}") %>'></asp:Label>
                                &nbsp;<asp:Label ID="purchasing_commentsLabel" runat="server" 
                            Text='<%# Bind("purchasing_comments", "<b>Comments:</b> {0}") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <strong>DaFIS PO:</strong>&nbsp;<asp:Label 
                            ID="lblDaFISPO" runat="server" 
                            Text='<%# Eval("dafis_po_num") %>'></asp:Label>
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
                    </asp:Repeater>
                    <asp:GridView ID="gvAttachments" runat="server" AutoGenerateColumns="False" 
                        DataSourceID="sdsFiles" EnableViewState="False" SkinID="notfull">
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="order_id,attach_id" 
                                DataNavigateUrlFormatString="filedownload.aspx?order_id={0}&amp;attach_id={1}" 
                                DataTextField="attach_name" DataTextFormatString="{0}" 
                                HeaderText="Attachments" />
                        </Columns>
                    </asp:GridView>
                    <asp:GridView ID="GridView2" runat="server" AllowSorting="True" 
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
                    <asp:Panel ID="pnlActive" runat="server">
                        <strong>Approver Comments:</strong>
                        <asp:TextBox ID="txtComments" runat="server" Width="500px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                    ControlToValidate="txtComments" 
                    ErrorMessage="Must provide comments when an order is denied" 
                    ValidationGroup="Deny">*</asp:RequiredFieldValidator>
                        <br />
                        <br />
                        <asp:Button ID="btnApprove" runat="server" 
                    Text="Approve order" ValidationGroup="Approve" 
                    onclick="btnApprove_Click" />
                        &nbsp; &nbsp;&nbsp; &nbsp;<asp:Button ID="btnDeny" runat="server"  
                    Text="Deny this order" ValidationGroup="Deny" 
                    onclick="btnDeny_Click" />
                    </asp:Panel>
                    <br />
                    <br />
                    <asp:Button ID="btnReuse" runat="server" CausesValidation="False" 
                Text="Submit new order based on this one" PostBackUrl="~/order_reuse.aspx" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnCancel" runat="server" 
                CausesValidation="False" onclick="btnCancel_Click" Text="Cancel Selection" />
                    <br />
                    &nbsp;<br />
                </ItemTemplate>
            </asp:FormView>
            <br />
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
                    <asp:ListItem Value="Submit_date">Submit Date</asp:ListItem>
                    <asp:ListItem Selected="True" Value="submitted_by">Submitted By</asp:ListItem>
                    <asp:ListItem Value="Vendor_name">Vendor</asp:ListItem>
                    <asp:ListItem Value="Routed_To">Routed To</asp:ListItem>
                    <asp:ListItem>Status</asp:ListItem>
                </asp:DropDownList>
                Search Text:
                <asp:TextBox ID="txtSearch" runat="server" EnableViewState="False"></asp:TextBox>
                (use % for wildcard)
                <asp:Button ID="Button1" runat="server" EnableViewState="False" 
            Text="Update Search" />
            </asp:Panel>
            <asp:GridView ID="GridView1" runat="server" AllowPaging="True" 
        AllowSorting="True" AutoGenerateColumns="False" CellPadding="3" 
        CssClass="noprint" DataKeyNames="order_id" DataSourceID="sdsAllOrders" 
        EnableViewState="False" PageSize="30">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="full_tag" HeaderText="full_tag" ReadOnly="True" 
                SortExpression="full_tag">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="submit_date" HeaderText="submit_date" 
                ReadOnly="True" SortExpression="submit_date" DataFormatString="{0:d}">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="submitted_by" HeaderText="submitted_by" 
                ReadOnly="True" SortExpression="submitted_by">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="vendor_name" HeaderText="vendor_name" 
                SortExpression="vendor_name">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="routed_to" HeaderText="routed_to" ReadOnly="True" 
                SortExpression="routed_to">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="status" HeaderText="status" SortExpression="status">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
</asp:Content>

