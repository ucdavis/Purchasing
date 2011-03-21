<%@ Page Language="VB" MasterPageFile="~/ops.master" AutoEventWireup="false" CodeFile="edit_orders.aspx.vb" Inherits="edit_orders" title="" Trace="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:SqlDataSource ID="sdsAllOrders" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" SelectCommand="edit_orders" 
        SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:SessionParameter Name="emp_id" SessionField="emp_id" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsOrderRequest" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" DataSourceMode="DataReader" 
        SelectCommand="edit_order_info" SelectCommandType="StoredProcedure" 
        UpdateCommand="update_order" UpdateCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridView1" Name="order_id" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="vendor_id" Type="Int32" />
            <asp:Parameter Name="contact" Type="String" />
            <asp:Parameter Name="phone1" Type="String" />
            <asp:Parameter Name="routed_to" Type="Int32" />
            <asp:Parameter Name="account" Type="String" />
            <asp:Parameter Name="need_by" Type="String" />
            <asp:Parameter Name="ship_to" Type="String" />
            <asp:Parameter Name="radioactive" Type="Boolean" />
            <asp:Parameter Name="rua_num" Type="String" />
            <asp:Parameter Name="attachment" Type="Boolean" />
            <asp:Parameter Name="vendor_contact" Type="String" />
            <asp:Parameter Name="request_comments" Type="String" />
            <asp:Parameter Name="order_id" Type="Int32" />
            <asp:Parameter Name="controlled_substance" Type="Boolean" />
            <asp:Parameter Name="cs_class" Type="String" />
            <asp:Parameter Name="cs_use" Type="String" />
            <asp:Parameter Name="cs_store" Type="String" />
            <asp:Parameter Name="cs_custodian" Type="String" />
            <asp:Parameter Name="cs_user" Type="String" />
            <asp:Parameter Name="RETURN_VALUE" Type="Int32" Direction="ReturnValue" />
            <asp:SessionParameter Name="updated_by" SessionField="emp_id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsOrderDetails" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" DataSourceMode="DataReader" 
        DeleteCommand="delete_order_detail" DeleteCommandType="StoredProcedure" 
        InsertCommand="insert_details_from_edit" InsertCommandType="StoredProcedure" 
        SelectCommand="edit_order_info_details" SelectCommandType="StoredProcedure" 
        UpdateCommand="update_order_details" UpdateCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridView1" Name="order_id" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="quantity" Type="Int32" />
            <asp:Parameter Name="unit" Type="String" />
            <asp:Parameter Name="item" Type="String" />
            <asp:Parameter Name="description" Type="String" />
            <asp:Parameter Name="exceed" Type="Decimal" />
            <asp:Parameter Name="cost" Type="Decimal" />
            <asp:Parameter Name="detail_id" Type="Int32" />
            <asp:SessionParameter Name="receiver" SessionField="emp_id" Type="Int32" />
        </UpdateParameters>
        <DeleteParameters>
            <asp:Parameter Name="detail_id" Type="Int32" />
            <asp:SessionParameter Name="deletor" SessionField="emp_id" Type="Int32" />
        </DeleteParameters>
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
                <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
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
    &nbsp;&nbsp;<asp:ValidationSummary ID="ValidationSummary1" runat="server" />
    <asp:FormView ID="FormView1" runat="server" BorderColor="#91AF6F" 
        BorderStyle="Solid" BorderWidth="2px" DataKeyNames="order_id" 
        DataSourceID="sdsOrderRequest" DefaultMode="Edit" EnableViewState="False" 
        Width="95%">
        <EditItemTemplate>
            OrderOrder Request Submitted by
            Requestand Approved by
            Submitted by
            <asp:Label ID="submitted_byLabel" runat="server" Font-Bold="True" 
                Text='<%# Eval("submitted_by") %>'></asp:Label>
            and Approved by
            <asp:Label ID="approved_byLabel" runat="server" Font-Bold="True" 
                Text='<%# Eval("approved_by") %>'></asp:Label>
            <br />
            <table border="1" style="text-align: left" width="100%">
                <tr>
                    <td rowspan="11">
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
                        Vendor:
                        <asp:DropDownList ID="DropDownList1" runat="server" 
                            DataSourceID="sdsInitialVendors" DataTextField="vendor_name" 
                            DataValueField="vendor_id" SelectedValue='<%# Bind("vendor_id") %>'>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <strong>Authorizer:
                        <asp:DropDownList ID="DropDownList2" runat="server" 
                            DataSourceID="sdsAuthorizers" DataTextField="display_name" 
                            DataValueField="approver_id" SelectedValue='<%# Bind("routed_to") %>'>
                        </asp:DropDownList>
                        </strong>
                    </td>
                </tr>
                <tr>
                    <td>
                        <strong>ID#:<asp:TextBox ID="txtID" runat="server" 
                            Text='<%# Bind("account") %>' Width="300px"></asp:TextBox>
                        </strong>&nbsp;
                    </td>
                    <td>
                        <strong>Tag #:</strong><asp:Label ID="full_tagLabel" runat="server" 
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
                        <strong>Ship To:&nbsp;</strong>
                        <asp:TextBox ID="ship_toTextBox" runat="server" Text='<%# Bind("ship_to") %>'>
                        </asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <strong>Contact:&nbsp;</strong> &nbsp;<asp:TextBox ID="contactTextBox" 
                            runat="server" Text='<%# Bind("contact") %>'>
                        </asp:TextBox>
                        &nbsp;&nbsp;&nbsp;&nbsp; <strong>Phone:</strong>&nbsp;
                        <asp:TextBox ID="phone1TextBox" runat="server" Text='<%# Bind("phone1") %>'>
                        </asp:TextBox>
                        &nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <strong>Radioactive?: </strong>&nbsp;
                        <asp:CheckBox ID="radioactiveCheckBox" runat="server" 
                            Checked='<%# Bind("radioactive") %>' />
                        &nbsp;<strong>RUA #:&nbsp;</strong>
                        <asp:TextBox ID="rua_numTextBox" runat="server" Text='<%# Bind("rua_num") %>'>
                        </asp:TextBox>
                    </td>
                    <td>
                        <strong>Status:</strong>
                        <asp:Label ID="statusLabel" runat="server" Text='<%# Eval("status") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <strong>Controlled Substance?:&nbsp;</strong>
                        <asp:CheckBox ID="csCheckBox" runat="server" 
                            Checked='<%# Bind("controlled_substance") %>' />
                        &nbsp;<b>Class:</b>
                        <asp:TextBox ID="txtCSClass" runat="server" Text='<%# Bind("cs_class") %>' 
                            Width="50px"></asp:TextBox>
                        &nbsp;<b>Use:</b>
                        <asp:TextBox ID="txtCSUse" runat="server" 
                            Text='<%# Bind("cs_use") %>' Width="200px"></asp:TextBox>
                        <br />
                        <b>Storage Site:</b>
                        <asp:TextBox ID="txtCSStore" runat="server" Text='<%# Bind("cs_store") %>'></asp:TextBox>
                        &nbsp;<b>Custodian:</b>
                        <asp:TextBox ID="txtCSCustodian" runat="server" 
                            Text='<%# Bind("cs_custodian") %>'></asp:TextBox>
                        &nbsp;<b>End User(s):</b>
                        <asp:TextBox ID="txtCSUsers" runat="server" Text='<%# Bind("cs_user") %>'></asp:TextBox>
                    </td>
                    <td>
                        <strong>Submit Date: </strong>
                        <asp:Label ID="request_dateLabel" runat="server" 
                            Text='<%# Eval("request_date") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <strong>Quote is being sent to purchasing desk?:&nbsp;</strong>
                        <asp:CheckBox ID="attachmentCheckBox" runat="server" 
                            Checked='<%# Bind("attachment") %>' />
                        <asp:GridView ID="gvAttachments" runat="server" AutoGenerateColumns="False" 
                            DataSourceID="sdsFiles" EnableViewState="False" SkinID="notfull">
                            <Columns>
                                <asp:HyperLinkField DataNavigateUrlFields="order_id,attach_id" 
                                    DataNavigateUrlFormatString="filedownload.aspx?order_id={0}&amp;attach_id={1}" 
                                    DataTextField="attach_name" DataTextFormatString="{0}" 
                                    HeaderText="Attachments" />
                            </Columns>
                        </asp:GridView>
                        <br />
                        Upload new attachement:
                        <br />
                        <asp:FileUpload ID="fuQuote" runat="server" />
                        <asp:CustomValidator ID="cvalQuote" runat="server" 
                            ErrorMessage="CustomValidator" onservervalidate="cvalQuote_ServerValidate"></asp:CustomValidator>
                        <br />
                        <asp:Button ID="Button5" runat="server" onclick="Button5_Click" Text="Upload" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <strong>Vendor contact:&nbsp;</strong>
                        <asp:TextBox ID="vendor_contactTextBox" runat="server" 
                            Text='<%# Bind("vendor_contact") %>' Width="400px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="height: 30px">
                        <strong>Request comments:&nbsp;</strong>
                        <asp:TextBox ID="request_commentsTextBox" runat="server" 
                            Text='<%# Bind("request_comments") %>' Width="400px"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <br />
            <asp:FormView ID="FormView2" runat="server" BorderColor="Black" 
                BorderStyle="Solid" BorderWidth="2px" DataKeyNames="detail_id" 
                DataSourceID="sdsOrderDetails" DefaultMode="Insert" Width="100%">
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
                                    ErrorMessage="You must supply a description entry" ValidationGroup="insertitem">*</asp:RequiredFieldValidator>
                            </td>
                            <td>
                                <asp:TextBox ID="costTextBox" runat="server" Text='<%# Bind("cost") %>' 
                                    Width="100px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                    ControlToValidate="costTextBox" 
                                    ErrorMessage="You must entry a cost for this item" ValidationGroup="insertitem">*</asp:RequiredFieldValidator>
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
            <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" 
                DataKeyNames="detail_id" DataSourceID="sdsOrderDetails" Width="90%">
                <Columns>
                    <asp:CommandField ShowEditButton="True" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" OnClientClick="return confirm('Are you sure you wish to delete this item?');" CommandName="Delete">Delete</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="quantity" SortExpression="quantity">
                        <EditItemTemplate>
                            <asp:TextBox ID="EditQuantity" runat="server" Text='<%# Bind("quantity") %>' 
                                Width="50px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                                ControlToValidate="EditQuantity" ErrorMessage="Quantity 
                                    is required"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="CompareValidator3" runat="server" 
                                ControlToValidate="EditQuantity" 
                                ErrorMessage="Quantity must be a valid integer" Operator="DataTypeCheck" 
                                Type="Integer"></asp:CompareValidator>
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
                            <asp:TextBox ID="txtDescEdit" runat="server" Rows="5" 
                                Text='<%# Bind("description") %>' TextMode="MultiLine"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                                ControlToValidate="txtDescEdit" ErrorMessage="Description is a required field"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("description") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="exceed" SortExpression="exceed">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtExceedEdit" runat="server" 
                                Text='<%# Bind("exceed", "{0:n2}") %>' Width="50px"></asp:TextBox>
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
                    <asp:BoundField DataField="total" HeaderText="total" 
                        SortExpression="total" DataFormatString="{0:n2}" ReadOnly="True">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
            &nbsp;<br />
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
            <asp:Button ID="btnUpdate" runat="server" CommandName="Update" 
                Text="Update Order" />
            &nbsp;&nbsp;
            <asp:Button ID="btnCancel0" runat="server" CommandName="Cancel" 
                onclick="btnCancel0_Click" Text="Cancel Edit" />
        </EditItemTemplate>
        <InsertItemTemplate>
            submitted_by:
            <asp:TextBox ID="submitted_byTextBox" runat="server" 
                Text='<%# Bind("submitted_by") %>'>
            </asp:TextBox>
            <br />
            request_date:
            <asp:TextBox ID="request_dateTextBox" runat="server" 
                Text='<%# Bind("request_date") %>'>
            </asp:TextBox>
            <br />
            vendor_name:
            <asp:TextBox ID="vendor_nameTextBox" runat="server" 
                Text='<%# Bind("vendor_name") %>'>
            </asp:TextBox>
            <br />
            address:
            <asp:TextBox ID="addressTextBox" runat="server" Text='<%# Bind("address") %>'>
            </asp:TextBox>
            <br />
            csz:
            <asp:TextBox ID="cszTextBox" runat="server" Text='<%# Bind("csz") %>'>
            </asp:TextBox>
            <br />
            phone:
            <asp:TextBox ID="phoneTextBox" runat="server" Text='<%# Bind("phone") %>'>
            </asp:TextBox>
            <br />
            fax:
            <asp:TextBox ID="faxTextBox" runat="server" Text='<%# Bind("fax") %>'>
            </asp:TextBox>
            <br />
            website:
            <asp:TextBox ID="websiteTextBox" runat="server" Text='<%# Bind("website") %>'>
            </asp:TextBox>
            <br />
            contact:
            <asp:TextBox ID="contactTextBox0" runat="server" Text='<%# Bind("contact") %>'>
            </asp:TextBox>
            <br />
            phone1:
            <asp:TextBox ID="phone1TextBox0" runat="server" Text='<%# Bind("phone1") %>'>
            </asp:TextBox>
            <br />
            routed_to:
            <asp:TextBox ID="routed_toTextBox" runat="server" 
                Text='<%# Bind("routed_to") %>'>
            </asp:TextBox>
            <br />
            account:
            <asp:TextBox ID="accountTextBox" runat="server" Text='<%# Bind("account") %>'>
            </asp:TextBox>
            <br />
            full_tag:
            <asp:TextBox ID="full_tagTextBox" runat="server" Text='<%# Bind("full_tag") %>'>
            </asp:TextBox>
            <br />
            need_by:
            <asp:TextBox ID="need_byTextBox" runat="server" Text='<%# Bind("need_by") %>'>
            </asp:TextBox>
            <br />
            ship_to:
            <asp:TextBox ID="ship_toTextBox0" runat="server" Text='<%# Bind("ship_to") %>'>
            </asp:TextBox>
            <br />
            radioactive:
            <asp:CheckBox ID="radioactiveCheckBox0" runat="server" 
                Checked='<%# Bind("radioactive") %>' />
            <br />
            rua_num:
            <asp:TextBox ID="rua_numTextBox0" runat="server" Text='<%# Bind("rua_num") %>'>
            </asp:TextBox>
            <br />
            status:
            <asp:TextBox ID="statusTextBox" runat="server" Text='<%# Bind("status") %>'>
            </asp:TextBox>
            <br />
            msds:
            <asp:CheckBox ID="msdsCheckBox0" runat="server" Checked='<%# Bind("msds") %>' />
            <br />
            attachment:
            <asp:CheckBox ID="attachmentCheckBox0" runat="server" 
                Checked='<%# Bind("attachment") %>' />
            <br />
            vendor_contact:
            <asp:TextBox ID="vendor_contactTextBox0" runat="server" 
                Text='<%# Bind("vendor_contact") %>'>
            </asp:TextBox>
            <br />
            request_comments:
            <asp:TextBox ID="request_commentsTextBox0" runat="server" 
                Text='<%# Bind("request_comments") %>'>
            </asp:TextBox>
            <br />
            approver_comments:
            <asp:TextBox ID="approver_commentsTextBox" runat="server" 
                Text='<%# Bind("approver_comments") %>'>
            </asp:TextBox>
            <br />
            approved_by:
            <asp:TextBox ID="approved_byTextBox" runat="server" 
                Text='<%# Bind("approved_by") %>'>
            </asp:TextBox>
            <br />
            approve_date:
            <asp:TextBox ID="approve_dateTextBox" runat="server" 
                Text='<%# Bind("approve_date") %>'>
            </asp:TextBox>
            <br />
            purchasing_comments:
            <asp:TextBox ID="purchasing_commentsTextBox" runat="server" 
                Text='<%# Bind("purchasing_comments") %>'>
            </asp:TextBox>
            <br />
            purchaser:
            <asp:TextBox ID="purchaserTextBox" runat="server" 
                Text='<%# Bind("purchaser") %>'>
            </asp:TextBox>
            <br />
            order_date:
            <asp:TextBox ID="order_dateTextBox" runat="server" 
                Text='<%# Bind("order_date") %>'>
            </asp:TextBox>
            <br />
            <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" 
                CommandName="Insert" Text="Insert">
            </asp:LinkButton>
            <asp:LinkButton ID="InsertCancelButton" runat="server" CausesValidation="False" 
                CommandName="Cancel" Text="Cancel">
            </asp:LinkButton>
        </InsertItemTemplate>
    </asp:FormView>
    <br />
    <asp:GridView ID="GridView1" runat="server" AllowSorting="True" 
        AutoGenerateColumns="False" CssClass="noprint" DataKeyNames="order_id" 
        DataSourceID="sdsAllOrders">
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
            <asp:BoundField DataField="routed_to" HeaderText="Routed To" ReadOnly="True" 
                SortExpression="routed_to">
                <ItemStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status">
                <ItemStyle HorizontalAlign="Left" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
</asp:Content>

