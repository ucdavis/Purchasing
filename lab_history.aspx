<%@ Page Title="" Language="VB" MasterPageFile="~/ops.master" AutoEventWireup="false" CodeFile="lab_history.aspx.vb" Inherits="lab_history" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:SqlDataSource ID="sdsMSDSAll" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        FilterExpression="{1} LIKE '{0}'" SelectCommand="lab_history_select" 
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
            <asp:SessionParameter DefaultValue="" Name="emp_id" SessionField="emp_id" 
                Type="String" />
            <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
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
    <asp:SqlDataSource ID="sdsChanges" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        SelectCommand="select_changes" SelectCommandType="StoredProcedure">
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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:FormView ID="FormView1" runat="server" DataSourceID="sdsOrderRequest" 
        EnableViewState="False" Width="95%">
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
                            <td rowspan="10">
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
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <strong>Authorizer: </strong>
                                <asp:Label ID="routed_toLabel" runat="server" Text='<%# Bind("routed_to") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>ID#: </strong>&nbsp;
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("account") %>'></asp:Label>
                            </td>
                            <td>
                                <strong>Tag #:</strong><asp:Label 
                            ID="full_tagLabel" runat="server" 
                            Text='<%# Bind("full_tag") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>Shipping Pref:</strong><asp:Label ID="Label4" runat="server" Text='<%# Bind("need_by") %>'></asp:Label>
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
                                &nbsp;&nbsp;&nbsp;&nbsp; <strong>Phone:</strong>
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
                                <asp:GridView ID="gvAttachments" runat="server" AutoGenerateColumns="False" 
                                    DataSourceID="sdsFiles" EnableViewState="False" SkinID="notfull">
                                    <Columns>
                                        <asp:HyperLinkField DataNavigateUrlFields="order_id,attach_id" 
                                            DataNavigateUrlFormatString="filedownload.aspx?order_id={0}&amp;attach_id={1}" 
                                            DataTextField="attach_name" DataTextFormatString="{0}" 
                                            HeaderText="Attachments" />
                                    </Columns>
                                </asp:GridView>
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
                            <td colspan="2" style="height: 23px">
                                <strong>Request comments: </strong>
                                <asp:Label ID="request_commentsLabel" runat="server" 
                            Text='<%# Bind("request_comments") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <strong>Approved by:</strong><asp:Label ID="Label1" runat="server" Text='<%# Bind("approved_by") %>'></asp:Label>
                                on
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("approve_date") %>'></asp:Label>
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
                                <strong>Order by: </strong>
                                <asp:Label ID="purchaserLabel" runat="server" Text='<%# Bind("purchaser") %>'></asp:Label>
                                on
                                <asp:Label ID="order_dateLabel" runat="server" Text='<%# Bind("order_date") %>'></asp:Label>
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
                                        <b>Shipping Status:</b>
                                        <asp:Label ID="Label12" runat="server" Text='<%# Eval("recd_status") %>'></asp:Label>
                                        &nbsp;<b>Shipping Comments: </b>
                                        <asp:Label ID="Label13" runat="server" Text='<%# Eval("recd_comments") %>'></asp:Label>
                                    </td>
                        </tr>
                    </table>
                    <br />
                   <asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False" 
                                DataKeyNames="detail_id" DataSourceID="sdsOrderDetails">
                                <Columns>                                   
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
                    &nbsp;&nbsp;&nbsp;&nbsp;<br />
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
                    &nbsp;&nbsp;<asp:Button ID="Button2" runat="server" CausesValidation="False" 
                PostBackUrl="~/order_reuse.aspx" Text="Submit new order based on this one" />
                    &nbsp;&nbsp;<br />
                    &nbsp;<br />
                </ItemTemplate>
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
            <asp:GridView ID="GridView1" runat="server" AllowSorting="True" 
        AutoGenerateColumns="False" CssClass="noprint" DataKeyNames="order_id" 
        DataSourceID="sdsMSDSAll">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="full_tag" HeaderText="full_tag" ReadOnly="True" 
                SortExpression="full_tag" />
                    <asp:BoundField DataField="description" HeaderText="description" 
                SortExpression="description" />
                    <asp:BoundField DataField="vendor_name" HeaderText="vendor_name" 
                SortExpression="vendor_name" />
                    <asp:BoundField DataField="lab" HeaderText="lab" ReadOnly="True" 
                SortExpression="lab" />
                    <asp:BoundField DataField="order_date" HeaderText="order_date" ReadOnly="True" 
                SortExpression="order_date" DataFormatString="{0:d}" />
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

