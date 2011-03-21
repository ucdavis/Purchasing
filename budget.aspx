<%@ Page Language="VB" MasterPageFile="~/ops_viewstate.master" AutoEventWireup="false" CodeFile="budget.aspx.vb" Inherits="budget" title="" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <p>
        <asp:SqlDataSource ID="sdsUrgentProcess" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
            FilterExpression="need_by &lt;&gt;''" SelectCommand="budget_select" 
            SelectCommandType="StoredProcedure">
            <FilterParameters>
                <asp:SessionParameter Name="processor" SessionField="name" />
            </FilterParameters>
            <SelectParameters>
                <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsLogProcess" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
            FilterExpression="budget_asst = '{0}'" SelectCommand="budget_select" 
            SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
            </SelectParameters>
            <FilterParameters>
                <asp:SessionParameter Name="processor" SessionField="name" />
            </FilterParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsNotLogProcess" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
            FilterExpression="budget_asst &lt;&gt; '{0}'" SelectCommand="budget_select" 
            SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
            </SelectParameters>
            <FilterParameters>
                <asp:SessionParameter Name="processor" SessionField="name" />
            </FilterParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsOrderRequest" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" DataSourceMode="DataReader" 
            SelectCommand="order_info" SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:ControlParameter ControlID="gvYour" Name="order_id" 
                    PropertyName="SelectedValue" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsOrderDetails" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" DataSourceMode="DataReader" 
            SelectCommand="order_info_details" SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:ControlParameter ControlID="FormView1" Name="order_id" 
                    PropertyName="SelectedValue" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsFiles" runat="server" ConnectionString="<%$ ConnectionStrings:OPSConn %>"
        SelectCommand="select_order_file" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="FormView1" Name="order_id" 
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
    </p>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
        ValidationGroup="Approve" />
            <asp:ValidationSummary ID="ValidationSummary2" runat="server" 
        ValidationGroup="Deny" />
            <p>
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
                                    <asp:Label ID="need_byLabel" runat="server" Text='<%# Eval("need_by") %>'></asp:Label>
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
                            <strong>Budget Comments:</strong>
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
                        <asp:Button ID="btnCancel" runat="server" 
                CausesValidation="False" onclick="btnCancel_Click" Text="Cancel Selection" />
                        <br />
                        &nbsp;<br />
                    </ItemTemplate>
                </asp:FormView>
                <br />
                <br />
                <br />
                <strong>&quot;Urgent&quot; Orders:</strong>
                <asp:GridView ID="gvUrgent" runat="server" AllowSorting="True" 
            AutoGenerateColumns="False" CssClass="noprint" DataKeyNames="order_id" 
            DataSourceID="sdsUrgentProcess" Width="95%" EnableViewState="False">
                    <Columns>
                        <asp:CommandField ShowSelectButton="True" />
                        <asp:BoundField DataField="full_tag" HeaderText="Tag" ReadOnly="True" 
                    SortExpression="full_tag">
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="submitted_by" HeaderText="Submitted By" 
                    ReadOnly="True" SortExpression="submitted_by">
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="submit_date" HeaderText="Date" ReadOnly="True" 
                    SortExpression="submit_date" DataFormatString="{0:d}">
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="approved_by" HeaderText="Approver" ReadOnly="True" 
                    SortExpression="approved_by">
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="vendor_name" HeaderText="Vendor" 
                            SortExpression="vendor_name" />
                        <asp:BoundField DataField="budget_asst" HeaderText="Budget Asst." 
                    ReadOnly="True" SortExpression="budget_asst">
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
                <br />
                <br />
                <strong>Your Accounts:</strong><br />
                <asp:GridView ID="gvYour" runat="server" AllowSorting="True" 
            AutoGenerateColumns="False" CssClass="noprint" DataKeyNames="order_id" 
            DataSourceID="sdsLogProcess" Width="95%" EnableViewState="False">
                    <Columns>
                        <asp:CommandField ShowSelectButton="True" />
                        <asp:BoundField DataField="full_tag" HeaderText="Tag" ReadOnly="True" 
                    SortExpression="full_tag">
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="submitted_by" HeaderText="Submitted By" 
                    ReadOnly="True" SortExpression="submitted_by">
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="submit_date" HeaderText="Date" ReadOnly="True" 
                    SortExpression="submit_date" DataFormatString="{0:d}">
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="approved_by" HeaderText="Approver" ReadOnly="True" 
                    SortExpression="approved_by">
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="vendor_name" HeaderText="Vendor" 
                            SortExpression="vendor_name" />
                        <asp:BoundField DataField="budget_asst" HeaderText="Budget Asst." 
                    ReadOnly="True" SortExpression="budget_asst">
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
                <br />
                <strong>Other Accounts:</strong><br />
                <asp:GridView ID="gvOther" runat="server" AllowSorting="True" 
            AutoGenerateColumns="False" CssClass="noprint" DataKeyNames="order_id" 
            DataSourceID="sdsNotLogProcess" Width="95%" EnableViewState="False">
                    <Columns>
                        <asp:CommandField ShowSelectButton="True" />
                        <asp:BoundField DataField="full_tag" HeaderText="Tag" ReadOnly="True" 
                    SortExpression="full_tag">
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="submitted_by" HeaderText="Submitted By" 
                    ReadOnly="True" SortExpression="submitted_by">
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="submit_date" HeaderText="Date" ReadOnly="True" 
                    SortExpression="submit_date" DataFormatString="{0:d}">
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="approved_by" HeaderText="Approver" ReadOnly="True" 
                    SortExpression="approved_by">
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="vendor_name" HeaderText="Vendor" 
                            SortExpression="vendor_name" />
                        <asp:BoundField DataField="budget_asst" HeaderText="Budget Asst." 
                    ReadOnly="True" SortExpression="budget_asst">
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
            </p>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

