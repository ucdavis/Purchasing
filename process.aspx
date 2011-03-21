<%@ Page Language="VB" MasterPageFile="~/ops_viewstate.master" AutoEventWireup="false" CodeFile="process.aspx.vb" Inherits="process" title="" Trace="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
            <asp:SqlDataSource ID="sdsLogProcess" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
            FilterExpression="purch_asst = '{0}'" SelectCommand="process_select" 
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
            FilterExpression="purch_asst &lt;&gt; '{0}'" SelectCommand="process_select" 
            SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
            </SelectParameters>
            <FilterParameters>
                <asp:SessionParameter Name="processor" SessionField="name" />
            </FilterParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsUrgentProcess" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
            FilterExpression="need_by &lt;&gt;''" SelectCommand="process_select" 
            SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
            </SelectParameters>
            <FilterParameters>
                <asp:SessionParameter Name="processor" SessionField="name" />
            </FilterParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsOrderRequest" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" DataSourceMode="DataReader" 
            SelectCommand="process_order_info" SelectCommandType="StoredProcedure" 
            UpdateCommand="update_process_order" UpdateCommandType="StoredProcedure">
            <UpdateParameters>
                <asp:Parameter Name="vendor_id" Type="Int32" />
                <asp:Parameter Name="need_by" Type="String" />
                <asp:Parameter Name="order_id" Type="Int32" />
                <asp:Parameter Name="account" Type="String" />
                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                <asp:SessionParameter Name="updated_by" SessionField="emp_id" Type="Int32" />
            </UpdateParameters>
            <SelectParameters>
                <asp:Parameter Name="order_id" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsOrderDetails" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" DataSourceMode="DataReader" 
            DeleteCommand="delete_order_detail" DeleteCommandType="StoredProcedure" 
            InsertCommand="insert_details_from_edit" InsertCommandType="StoredProcedure" 
            SelectCommand="order_info_details" SelectCommandType="StoredProcedure" 
            UpdateCommand="update_order_details_shipping" 
                UpdateCommandType="StoredProcedure">
            <DeleteParameters>
                <asp:Parameter Name="detail_id" Type="Int32" />
                <asp:SessionParameter Name="deletor" SessionField="emp_id" Type="Int32" />
            </DeleteParameters>
            <UpdateParameters>
                <asp:Parameter Name="quantity" Type="Int32" />
                <asp:Parameter Name="unit" Type="String" />
                <asp:Parameter Name="item" Type="String" />
                <asp:Parameter Name="description" Type="String" />
                <asp:Parameter Name="exceed" Type="Decimal" />
                <asp:Parameter Name="cost" Type="Decimal" />
                <asp:Parameter Name="detail_id" Type="Int32" />
                <asp:Parameter Name="shipping_notes" Type="String" />
                <asp:SessionParameter Name="receiver" SessionField="emp_id" Type="Int32" />
            </UpdateParameters>
            <SelectParameters>
                <asp:ControlParameter ControlID="FormView1" Name="order_id" 
                    PropertyName="SelectedValue" Type="Int32" />
            </SelectParameters>
            <InsertParameters>
                <asp:ControlParameter ControlID="FormView1" Name="order_id" 
                    PropertyName="SelectedValue" Type="Int32" />
                <asp:Parameter Name="quantity" Type="Int32" />
                <asp:Parameter Name="unit" Type="String" />
                <asp:Parameter Name="item" Type="String" />
                <asp:Parameter Name="description" Type="String" />
                <asp:Parameter Name="exceed" Type="Decimal" />
                <asp:Parameter Name="cost" Type="Decimal" />
                <asp:SessionParameter Name="insertor" SessionField="emp_id" Type="Int32" />
            </InsertParameters>
        </asp:SqlDataSource>
        <asp:XmlDataSource ID="xdsShipping" runat="server" DataFile="~/ddlElements.xml" 
            XPath="root/shipping/value"></asp:XmlDataSource>
        <asp:SqlDataSource ID="sdsInitialVendors" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
            SelectCommand="select_vendor" SelectCommandType="StoredProcedure">
        </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsFiles" runat="server" ConnectionString="<%$ ConnectionStrings:OPSConn %>"
        SelectCommand="select_order_file" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="FormView1" Name="order_id" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsAuthorizers" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
            SelectCommand="select_current_approvers" SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:SessionParameter Name="dept" SessionField="dept" Type="String" />
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
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                    <asp:ValidationSummary ID="ValidationSummary2" runat="server" 
                        ValidationGroup="FinalProcess" />
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
                                        <asp:Label ID="routed_toLabel" runat="server" 
                                    Text='<%# Bind("routed_to_name") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>ID#:&nbsp; </strong>
                                        <asp:Label ID="lblID" runat="server" Text='<%# Eval("account") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <strong>Tag #:</strong><asp:Label ID="full_tagLabel" runat="server" 
                            Text='<%# Bind("full_tag") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>Shipping Pref:</strong>
                                        <asp:Label ID="lblShip" runat="server" Text='<%# Eval("need_by") %>'></asp:Label>
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
                                        <asp:CheckBox ID="csCheckBox" runat="server" Checked='<%# Eval("controlled_substance") %>' 
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
                            </table>
                            <br />
                             <asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False" 
                                DataKeyNames="detail_id" DataSourceID="sdsOrderDetails">
                                <Columns>
                                    <asp:CommandField ShowEditButton="True" />
                                    <asp:BoundField DataField="quantity" HeaderText="Qty" SortExpression="quantity" />
                                    <asp:BoundField DataField="unit" HeaderText="Unit" SortExpression="unit" />
                                    <asp:BoundField DataField="item" HeaderText="Item No." SortExpression="item" />                                    
                                    <asp:TemplateField HeaderText="Description" SortExpression="description" >
                                        <EditItemTemplate>
                                            <asp:TextBox ID="textboxDescription" runat="server" Rows="5" Text='<%# Bind("description") %>' TextMode="MultiLine" Width="300px"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="LabelDescription" runat="server" Text='<%# Bind("description") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Shipping Notes" SortExpression="shipping_notes">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBox2" runat="server" Rows="5" 
                                                Text='<%# Bind("shipping_notes") %>' TextMode="MultiLine" Width="300px"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label8" runat="server" Text='<%# Bind("shipping_notes") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="exceed" DataFormatString="{0:c}" HeaderText="Not to Exceed" SortExpression="exceed" />
                                    <asp:BoundField DataField="cost" DataFormatString="{0:c}" HeaderText="Unit Cost" SortExpression="cost" />
                                    <asp:BoundField DataField="total" DataFormatString="{0:c}" HeaderText="Total" ReadOnly="True" SortExpression="total" />
                                </Columns>
                            </asp:GridView>
                            <table border="0" width="100%">
                                <tr>
                                    <td align="right" style="width: 100%">
                                        <strong>Material:</strong></td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        <asp:Label ID="materialLablel" runat="server" 
                                            Text='<%# Eval("ship_mat", "{0:c}") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" style="width: 100%">
                                        <strong>Subtotal:</strong></td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        <asp:Label ID="subtotalLabel" runat="server" 
                                            Text='<%# Eval("subtotal", "{0:c}") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" style="width: 100%">
                                        <strong>Tax:</strong></td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        <asp:Label ID="Label10" runat="server" Text='<%# Eval("tax_total", "{0:c}") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" style="width: 100%">
                                        <strong>Shipping/Handling:</strong></td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        <asp:Label ID="Label11" runat="server" Text='<%# Eval("ship_cost", "{0:c}") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" style="width: 100%">
                                        <strong>Total:</strong></td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        <asp:Label ID="Label9" runat="server" 
                                            Text='<%# Eval("order_total", "{0:c}") %>'></asp:Label>
                                    </td>
                                </tr>
                            </table>
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
                            <br />
                            &nbsp;<asp:Button ID="btnEdit" runat="server"  
                        Text="Edit Order" CommandName="Edit" 
                        CausesValidation="False" />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnCancel" runat="server" onclick="btnCancel_Click1" 
                        Text="Cancel Selection" CausesValidation="False" />
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
                                        <strong>Controlled Substance?: </strong>
                                        <asp:CheckBox ID="csCheckBox" runat="server" Checked='<%# Eval("controlled_substance") %>' 
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
                            <asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False" 
                                DataKeyNames="detail_id" DataSourceID="sdsOrderDetails">
                                <Columns>
                                    <asp:CommandField ShowEditButton="True" />
                                    <asp:BoundField DataField="quantity" HeaderText="Qty" SortExpression="quantity" />
                                    <asp:BoundField DataField="unit" HeaderText="Unit" SortExpression="unit" />
                                    <asp:BoundField DataField="item" HeaderText="Item No." SortExpression="item" />
                                     <asp:TemplateField HeaderText="Description" SortExpression="description" >
                                        <EditItemTemplate>
                                            <asp:TextBox ID="textboxDescription" runat="server" Rows="5" Text='<%# Bind("description") %>' TextMode="MultiLine" Width="300px"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="LabelDescription" runat="server" Text='<%# Bind("description") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Shipping Notes" SortExpression="shipping_notes">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBox2" runat="server" Rows="5" 
                                                Text='<%# Bind("shipping_notes") %>' TextMode="MultiLine" Width="300px"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label8" runat="server" Text='<%# Bind("shipping_notes") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="exceed" DataFormatString="{0:c}" HeaderText="Not to Exceed" SortExpression="exceed" />
                                    <asp:BoundField DataField="cost" DataFormatString="{0:c}" HeaderText="Unit Cost" SortExpression="cost" />
                                    <asp:BoundField DataField="total" DataFormatString="{0:c}" HeaderText="Total" ReadOnly="True" SortExpression="total" />
                                </Columns>
                            </asp:GridView>
                            <table border="0" width="100%">
                                <tr>
                                    <td align="right" style="width: 100%">
                                        <strong>Material:</strong></td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        <asp:Label ID="materialLablel" runat="server" 
                                            Text='<%# Eval("ship_mat", "{0:c}") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" style="width: 100%">
                                        <strong>Subtotal:</strong></td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        <asp:Label ID="subtotalLabel" runat="server" 
                                            Text='<%# Eval("subtotal", "{0:c}") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" style="width: 100%">
                                        <strong>Tax:</strong></td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        <asp:Label ID="Label10" runat="server" Text='<%# Eval("tax_total", "{0:c}") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" style="width: 100%">
                                        <strong>Shipping/Handling:</strong></td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        <asp:Label ID="Label11" runat="server" Text='<%# Eval("ship_cost", "{0:c}") %>'></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" style="width: 100%">
                                        <strong>Total:</strong></td>
                                    <td>
                                        &nbsp;</td>
                                    <td>
                                        <asp:Label ID="Label9" runat="server" 
                                            Text='<%# Eval("order_total", "{0:c}") %>'></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            &nbsp;<asp:Button ID="btnUpdate" runat="server" CommandName="Update" 
                        Text="Update Order" />
                            &nbsp;&nbsp;
                            <asp:Button ID="btnCancel0" runat="server" CommandName="Cancel" 
                        Text="Cancel Edit" />
                            <br />
                        </EditItemTemplate>
                    </asp:FormView>
                    <asp:Panel ID="pnlProcess" runat="server" HorizontalAlign="Left" 
                Visible="False" Width="75%">
                        <strong>Is order Taxable:</strong>
                        <asp:RadioButtonList ID="rblTax" runat="server" RepeatDirection="Horizontal" 
                    RepeatLayout="Flow">
                            <asp:ListItem Selected="True" Value="1">Yes |</asp:ListItem>
                            <asp:ListItem Value="0">No</asp:ListItem>
                        </asp:RadioButtonList>
                        <br />
                        <strong>Shipping:</strong>
                        <asp:TextBox ID="txtShip" runat="server" Width="75px">0</asp:TextBox>
                        &nbsp; <strong>Shipping Materials (taxable):</strong>
                        <asp:TextBox ID="txtShipMat" runat="server" Width="75px">0</asp:TextBox>
                        <br />
                        <strong>Payment Type:</strong>
                        <asp:DropDownList ID="ddlPaymentType" runat="server">
                            <asp:ListItem></asp:ListItem>
                            <asp:ListItem>Charge</asp:ListItem>
                            <asp:ListItem>DPO</asp:ListItem>
                            <asp:ListItem>DRO</asp:ListItem>
                            <asp:ListItem>PR</asp:ListItem>
                        </asp:DropDownList>
                        <strong>Order Status: </strong>
                        <asp:DropDownList ID="ddlStatus" runat="server">
                            <asp:ListItem>Order Placed</asp:ListItem>
                            <asp:ListItem>Submitted to Purchasing</asp:ListItem>
                            <asp:ListItem>Denied by Purchasing</asp:ListItem>
                        </asp:DropDownList>
                        <br />
                        <strong>PO/Confirmation Number:</strong>
                        <asp:TextBox ID="txtPO" runat="server" Width="300px"></asp:TextBox>
                        <asp:CustomValidator ID="CustomValidator2" runat="server" 
                            ControlToValidate="ddlStatus" 
                            ErrorMessage="PO/Confirmation Number is a required field" 
                            ValidationGroup="FinalProcess"></asp:CustomValidator>
                        <br />
                        <strong>Order ETA/Delivery/Purchasing Comments:</strong> (will be included in confirmation email)&nbsp;<br />
                        <asp:TextBox ID="txtComment" runat="server" Rows="3" TextMode="MultiLine" 
                    Width="500px"></asp:TextBox>
                        <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="txtComment" 
                    ErrorMessage="Comments are required for denied orders" ValidateEmptyText="True" 
                    ValidationGroup="FinalProcess"></asp:CustomValidator>
                        <br />
                        <asp:Button ID="Button1" runat="server" Text="Submit" 
                    ValidationGroup="FinalProcess" />
                        <asp:Button ID="Button2" runat="server" CausesValidation="False" 
                    Text="Cancel Processing" />
                        <br />
                        <hr />
                        <br />
                    </asp:Panel>
                    <br />
                    <asp:Panel ID="Panel1" runat="server" CssClass="noprint" Width="100%">
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
                                <asp:BoundField DataField="vendor_name" HeaderText="Vendor" 
                    SortExpression="vendor_name">
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="approved_by" HeaderText="Approver" ReadOnly="True" 
                    SortExpression="approved_by">
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="purch_asst" HeaderText="Purch. Asst." 
                    ReadOnly="True" SortExpression="purch_asst">
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>
                        <br />
                        <br />
                        <strong>Your Accounts:</strong><asp:GridView 
            ID="gvYour" runat="server" 
            AllowSorting="True" AutoGenerateColumns="False" CssClass="noprint" 
            DataKeyNames="order_id" DataSourceID="sdsLogProcess" Width="95%" EnableViewState="False">
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
                                <asp:BoundField DataField="vendor_name" HeaderText="Vendor" 
                    SortExpression="vendor_name">
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="approved_by" HeaderText="Approver" ReadOnly="True" 
                    SortExpression="approved_by">
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="purch_asst" HeaderText="Purch. Asst." 
                    ReadOnly="True" SortExpression="purch_asst">
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
                                <asp:BoundField DataField="vendor_name" HeaderText="Vendor" 
                    SortExpression="vendor_name">
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="approved_by" HeaderText="Approver" ReadOnly="True" 
                    SortExpression="approved_by">
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                                <asp:BoundField DataField="purch_asst" HeaderText="Purch. Asst." 
                    ReadOnly="True" SortExpression="purch_asst">
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
               </ContentTemplate>
            </asp:UpdatePanel>
            <br />
</asp:Content>

