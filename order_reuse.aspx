<%@ Page Language="VB" MasterPageFile="~/ops_viewstate.master" AutoEventWireup="false"
    CodeFile="order_reuse.aspx.vb" Inherits="order_reuse" Trace="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:SqlDataSource ID="sdsPreOrderInfo" runat="server" ConnectionString="<%$ ConnectionStrings:OPSConn %>"
        SelectCommand="vendor_info_order" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:Parameter Name="vendor_id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsFiles" runat="server" ConnectionString="<%$ ConnectionStrings:OPSConn %>"
        SelectCommand="select_order_file" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:Parameter Name="order_id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsAuthorizers" runat="server" ConnectionString="<%$ ConnectionStrings:OPSConn %>"
        SelectCommand="select_current_approvers" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
            <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:XmlDataSource ID="xdsShipping" runat="server" DataFile="~/ddlElements.xml" XPath="root/shipping/value">
    </asp:XmlDataSource>
    <asp:SqlDataSource ID="sdsBldNames" runat="server" 
    ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
    SelectCommand="select_buildings" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
        </SelectParameters>
</asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsInsertOrderRequest" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        InsertCommand="insert_order_request_cs" InsertCommandType="StoredProcedure" 
        ProviderName="<%$ ConnectionStrings:OPSConn.ProviderName %>">
        <InsertParameters>
            <asp:Parameter Name="app_id" Type="Int32" DefaultValue="" />
            <asp:Parameter Name="fiscal_year" Type="String" />
            <asp:SessionParameter Name="submitted_by" SessionField="emp_id" 
                Type="String" />
            <asp:ControlParameter ControlID="hdVendorID" Name="vendor_id" 
                PropertyName="Value" Type="Int32" />
            <asp:ControlParameter ControlID="txtContact" Name="contact" PropertyName="Text" 
                Type="String" />
            <asp:ControlParameter ControlID="txtPhone" Name="phone" PropertyName="Text" 
                Type="String" />
            <asp:ControlParameter ControlID="ddlAuthorizer" Name="routed_to" 
                PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="txtAcct" Name="account" PropertyName="Text" 
                Type="String" />
            <asp:ControlParameter ControlID="ddlShipping" Name="need_by" 
                PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="ckbxRadioactive" Name="radioactive" 
                PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="txtRUA" Name="rua_num" PropertyName="Text" 
                Type="String" />
            <asp:ControlParameter ControlID="ckbxQuote" Name="attachment" 
                PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="txtVendor" Name="vendor_contact" 
                PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtComment" Name="request_comments" 
                PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="ddlShipStation" Name="ship_to" PropertyName="SelectedValue" 
                Type="String" />
            <asp:Parameter DefaultValue="0" Direction="InputOutput" Name="order_id" 
                Type="Int32" />
            <asp:Parameter DefaultValue="False" Direction="InputOutput" Name="approved" 
                Type="Boolean" />
            <asp:Parameter DefaultValue="0" Direction="InputOutput" Name="full_tag" 
                Size="10" Type="String" />
            <asp:Parameter DefaultValue="0" Direction="InputOutput" Name="email_name" 
                Size="200" Type="String" />
            <asp:Parameter DefaultValue="0" Direction="InputOutput" Name="email_address" 
                Size="100" Type="String" />
            <asp:Parameter DefaultValue="0" Direction="InputOutput" Name="pemail_name" 
                Size="200" Type="String" />
            <asp:Parameter DefaultValue="0" Direction="InputOutput" Name="pemail_address" 
                Size="100" Type="String" />
            <asp:Parameter DefaultValue="0" Direction="InputOutput" Name="remail_name" 
                Size="200" Type="String" />
            <asp:Parameter DefaultValue="0" Direction="Output" Name="remail_address" 
                Size="100" Type="String" />
            <asp:Parameter DefaultValue="False" Direction="InputOutput" Name="auto_budget" 
                Type="Boolean" />
            <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
            <asp:ControlParameter ControlID="ckbxControlledSubstance" Name="controlled_substance" 
                PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="txtCSClass" Name="cs_class" 
                PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtCSUse" Name="cs_use" 
                PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtCSStore" Name="cs_store" 
                PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtCSCustodian" Name="cs_custodian" 
                PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtCSUser" Name="cs_user" PropertyName="Text" 
                Type="String" />
        </InsertParameters>
        <UpdateParameters>
            <asp:Parameter Name="order_id" Type="Int32" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:Label ID="Label1" runat="server"></asp:Label>
    <br />
    <strong>Submit New Order<br />
    </strong>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
    <br />
    <asp:Panel ID="pnlOrder" runat="server" HorizontalAlign="Left" Width="100%">
        <p style="text-align: center">
            Fields marked with <span style="color: red">*Name:</span> are required.
        </p>
        <table border="1" width="100%">
            <tbody>
                <tr>
                    <td rowspan="10" style="width: 30%; height: 40px" valign="top">
                        <strong>Vendor Info<asp:HiddenField ID="hdVendorID" runat="server" />
                        <br />
                            <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" BorderStyle="Solid"
                                BorderWidth="0px" DataKeyNames="vendor_id" DataSourceID="sdsPreorderInfo" Font-Bold="False"
                                Height="50px" HorizontalAlign="Left" Width="100%">
                                <Fields>
                                    <asp:BoundField DataField="vendor_name" SortExpression="vendor_name"></asp:BoundField>
                                    <asp:BoundField DataField="address" HtmlEncode="False" ReadOnly="True" SortExpression="address">
                                    </asp:BoundField>
                                    <asp:BoundField DataField="csz" ReadOnly="True" SortExpression="csz"></asp:BoundField>
                                    <asp:TemplateField></asp:TemplateField>
                                    <asp:BoundField DataField="phone" DataFormatString="Phone: {0}" SortExpression="phone">
                                    </asp:BoundField>
                                    <asp:BoundField DataField="fax" DataFormatString="Fax: {0}" SortExpression="fax">
                                    </asp:BoundField>
                                    <asp:HyperLinkField DataNavigateUrlFields="website" DataNavigateUrlFormatString="http://{0}"
                                        DataTextField="vendor_name" Target="_blank"></asp:HyperLinkField>
                                </Fields>
                                <RowStyle BorderStyle="None" HorizontalAlign="Left" />
                            </asp:DetailsView>
                        </strong>
                    </td>
                    <td align="left">
                        <strong><span style="color: red">*Authorizer:</span></strong><asp:DropDownList ID="ddlAuthorizer"
                            runat="server" DataSourceID="sdsAuthorizers" DataTextField="display_name" DataValueField="approver_id">
                        </asp:DropDownList>
                        <cc1:listsearchextender id="ddlAuthorizer_ListSearchExtender" runat="server" enabled="True"
                            targetcontrolid="ddlAuthorizer">
                </cc1:listsearchextender>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="ddlAuthorizer"
                            ErrorMessage="You must select an approver" InitialValue="0">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <strong>ID#:</strong><asp:TextBox ID="txtAcct" runat="server" Width="400px" 
                            style="height: 22px"></asp:TextBox><asp:CustomValidator
                            ID="CustomValidator11" runat="server" ControlToValidate="txtContact" ErrorMessage="If submitter = authorizer, you must provide an account number">*</asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <strong>Shipping:</strong><asp:DropDownList ID="ddlShipping" runat="server" DataSourceID="xdsShipping"
                            DataTextField="label" DataValueField="label">
                        </asp:DropDownList>                        
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <strong><span style="color: #ff0000">*Contact:</span></strong> <asp:TextBox
                            ID="txtContact" runat="server"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator1"
                                runat="server" ControlToValidate="txtContact" ErrorMessage="Contact info can not be left blank">*</asp:RequiredFieldValidator>
                        <strong style="color: #ff0000">*Phone:</strong>
                        <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                            ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPhone" ErrorMessage="Phone can not be left blank">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Ship to:</b> <asp:DropDownList ID="ddlShipStation" runat="server" DataSourceID="sdsBldNames" DataTextField="bld_name" DataValueField="bld_name">
                            </asp:DropDownList>                       
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <strong>Radioactive?:</strong>
                        <asp:CheckBox ID="ckbxRadioactive" runat="server"></asp:CheckBox> <strong>RUA#:</strong>
                        <asp:TextBox ID="txtRUA" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <b>This order includes a controlled substance or precursor:</b>
                        <asp:CheckBox ID="ckbxControlledSubstance" runat="server" AutoPostBack="True" />
                        <asp:Panel ID="pnlCS" runat="server" Visible="False">
                            <b style="color: #ff0000">*Class/Schedule:</b>
                            <asp:TextBox ID="txtCSClass" runat="server" Width="50px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" 
                                ControlToValidate="txtCSClass" ErrorMessage="Class/Schedule is required"></asp:RequiredFieldValidator>
                            &nbsp;&nbsp; 
                            <br />
                            <b style="color: #ff0000">*What is it used for: </b>
                            <asp:TextBox ID="txtCSUse" runat="server" Width="400px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" 
                                ControlToValidate="txtCSUse" ErrorMessage="Use explaination is required"></asp:RequiredFieldValidator>
                            <br />
                            <b style="color: #ff0000">*Approved storage site on record: </b>
                            <asp:TextBox ID="txtCSStore" runat="server" Width="200px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" 
                                ControlToValidate="txtCSStore" ErrorMessage="Storage site is required"></asp:RequiredFieldValidator>
                            <br />
                            <b style="color: #ff0000">*Authorized Custodian: </b>
                            <asp:TextBox ID="txtCSCustodian" runat="server" Width="200px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" 
                                ControlToValidate="txtCSCustodian" ErrorMessage="Custodian is required"></asp:RequiredFieldValidator>
                            <br />
                            <b style="color: #ff0000">*Authorized End User(s): </b>
                            <asp:TextBox ID="txtCSUser" runat="server" Width="200px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" 
                                ControlToValidate="txtCSUser" ErrorMessage="Authorized user(s) is required"></asp:RequiredFieldValidator>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <strong>Check if quote is being sent to purchasing desk:</strong>
                        <asp:CheckBox ID="ckbxQuote" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <strong>Vendor contact:</strong>
                        <asp:TextBox ID="txtVendor" runat="server" Width="300px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <strong>Request comments: </strong>(including special deliver instruction)<br />
                        <asp:TextBox ID="txtComment" runat="server" Rows="3" TextMode="MultiLine" 
                            Width="100%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <strong>ITEMS<br />
                        </strong>(<span style="color: red">*note:</span> you must fill in either &quot;Not to 
                        Exceed amount&quot; or &quot;Unit Cost&quot; for each item)<br />
                        <table border="1" width="100%">
                            <tbody>
                                <tr>
                                    <td>
                                        <strong><span style="color: red">*Qty</span></strong>
                                    </td>
                                    <td>
                                        <strong><span style="color: red">*Unit</span></strong>
                                    </td>
                                    <td>
                                        <strong>Item/<br />
                                            Catalog<br />
                                            No.</strong>
                                    </td>
                                    <td>
                                        <strong><span style="color: red">*Description</span></strong>
                                    </td>
                                    <td>
                                        <strong>Not to<br />
                                            Exceed<br />
                                            amount</strong>
                                    </td>
                                    <td>
                                        <strong>Unit Cost</strong>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtQty1" runat="server" Width="25px"></asp:TextBox><asp:RequiredFieldValidator
                                            ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtQty1" ErrorMessage="You must provide a quantity for item #1">*</asp:RequiredFieldValidator><asp:CompareValidator
                                                ID="CompareValidator1" runat="server" ControlToValidate="txtQty1" ErrorMessage="Quantity for item #1 must be a number"
                                                Operator="DataTypeCheck" Type="Integer">*</asp:CompareValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtUnit1" runat="server" Width="75px"></asp:TextBox><asp:RequiredFieldValidator
                                            ID="RequiredFieldValidator7" runat="server" ControlToValidate="txtUnit1" ErrorMessage="You must provide a unit for item #1">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtItem1" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescription1" runat="server" Rows="3" TextMode="MultiLine" Width="300px"></asp:TextBox><asp:RequiredFieldValidator
                                            ID="RequiredFieldValidator8" runat="server" ControlToValidate="txtDescription1"
                                            ErrorMessage="You must provide a description for item #1">*</asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtExceed1" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator3" runat="server" ControlToValidate="txtExceed1" ErrorMessage="Not to exceed amount for item #1 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCost1" runat="server" Width="100px"></asp:TextBox><asp:CustomValidator
                                            ID="CustomValidator1" runat="server" ErrorMessage="You must provide a 'not to exceed' or 'unit cost' for item #1">*</asp:CustomValidator><asp:CompareValidator
                                                ID="CompareValidator2" runat="server" ControlToValidate="txtCost1" ErrorMessage="Unit cost for item #1 must be a valid dollar amount"
                                                Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                        <asp:CustomValidator ID="CustomValidator12" runat="server" 
                                            ErrorMessage="Quantity X Cost can not exceed $215,000." 
                                            ControlToValidate="txtCost1">*</asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtQty2" runat="server" Width="25px"></asp:TextBox><asp:CustomValidator
                                            ID="CustomValidator2" runat="server" ControlToValidate="txtContact" ErrorMessage="Item #2 has an error. Please ensure you have provided quantity (valid number), unit, description, and a 'not to exceed' or 'unit cost' (with valid currency format)">*</asp:CustomValidator><asp:CompareValidator
                                                ID="CompareValidator4" runat="server" ControlToValidate="txtQty2" ErrorMessage="Quantity for item #2 must be a number"
                                                Operator="DataTypeCheck" Type="Integer">*</asp:CompareValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtUnit2" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtItem2" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescription2" runat="server" Rows="3" TextMode="MultiLine" Width="300px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtExceed2" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator13" runat="server" ControlToValidate="txtExceed2" ErrorMessage="Not to exceed amount for item #2 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCost2" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator22" runat="server" ControlToValidate="txtCost2" ErrorMessage="Unit cost for item #2 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                        <asp:CustomValidator ID="CustomValidator13" runat="server" 
                                            ControlToValidate="txtCost2" 
                                            ErrorMessage="Quantity X Cost can not exceed $215,000.">*</asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtQty3" runat="server" Width="25px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator5" runat="server" ControlToValidate="txtQty3" ErrorMessage="Quantity for item #3 must be a number"
                                            Operator="DataTypeCheck" Type="Integer">*</asp:CompareValidator><asp:CustomValidator
                                                ID="CustomValidator3" runat="server" ControlToValidate="txtContact" ErrorMessage="Item #2 has an error. Please ensure you have provided quantity (valid number), unit, description, and a 'not to exceed' or 'unit cost' (with valid currency format)">*</asp:CustomValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtUnit3" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtItem3" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescription3" runat="server" Rows="3" TextMode="MultiLine" Width="300px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtExceed3" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator14" runat="server" ControlToValidate="txtExceed3" ErrorMessage="Not to exceed amount for item #3 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCost3" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator23" runat="server" ControlToValidate="txtCost3" ErrorMessage="Unit cost for item #3 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                        <asp:CustomValidator ID="CustomValidator14" runat="server" 
                                            ControlToValidate="txtCost3" 
                                            ErrorMessage="Quantity X Cost can not exceed $215,000.">*</asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtQty4" runat="server" Width="25px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator6" runat="server" ControlToValidate="txtQty4" ErrorMessage="Quantity for item #4 must be a number"
                                            Operator="DataTypeCheck" Type="Integer">*</asp:CompareValidator><asp:CustomValidator
                                                ID="CustomValidator4" runat="server" ControlToValidate="txtContact" ErrorMessage="Item #2 has an error. Please ensure you have provided quantity (valid number), unit, description, and a 'not to exceed' or 'unit cost' (with valid currency format)">*</asp:CustomValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtUnit4" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtItem4" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescription4" runat="server" Rows="3" TextMode="MultiLine" Width="300px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtExceed4" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator15" runat="server" ControlToValidate="txtExceed4" ErrorMessage="Not to exceed amount for item #4 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCost4" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator24" runat="server" ControlToValidate="txtCost4" ErrorMessage="Unit cost for item #4 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                        <asp:CustomValidator ID="CustomValidator15" runat="server" 
                                            ControlToValidate="txtCost4" 
                                            ErrorMessage="Quantity X Cost can not exceed $215,000.">*</asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtQty5" runat="server" Width="25px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator7" runat="server" ControlToValidate="txtQty5" ErrorMessage="Quantity for item #5 must be a number"
                                            Operator="DataTypeCheck" Type="Integer">*</asp:CompareValidator><asp:CustomValidator
                                                ID="CustomValidator5" runat="server" ControlToValidate="txtContact" ErrorMessage="Item #2 has an error. Please ensure you have provided quantity (valid number), unit, description, and a 'not to exceed' or 'unit cost' (with valid currency format)">*</asp:CustomValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtUnit5" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtItem5" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescription5" runat="server" Rows="3" TextMode="MultiLine" Width="300px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtExceed5" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator16" runat="server" ControlToValidate="txtExceed5" ErrorMessage="Not to exceed amount for item #5 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCost5" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator25" runat="server" ControlToValidate="txtCost5" ErrorMessage="Unit cost for item #5 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                        <asp:CustomValidator ID="CustomValidator16" runat="server" 
                                            ControlToValidate="txtCost5" 
                                            ErrorMessage="Quantity X Cost can not exceed $215,000.">*</asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtQty6" runat="server" Width="25px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator8" runat="server" ControlToValidate="txtQty6" ErrorMessage="Quantity for item #6 must be a number"
                                            Operator="DataTypeCheck" Type="Integer">*</asp:CompareValidator><asp:CustomValidator
                                                ID="CustomValidator6" runat="server" ControlToValidate="txtContact" ErrorMessage="Item #2 has an error. Please ensure you have provided quantity (valid number), unit, description, and a 'not to exceed' or 'unit cost' (with valid currency format)">*</asp:CustomValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtUnit6" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtItem6" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescription6" runat="server" Rows="3" TextMode="MultiLine" Width="300px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtExceed6" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator17" runat="server" ControlToValidate="txtExceed6" ErrorMessage="Not to exceed amount for item #6 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCost6" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator26" runat="server" ControlToValidate="txtCost6" ErrorMessage="Unit cost for item #6 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                        <asp:CustomValidator ID="CustomValidator17" runat="server" 
                                            ControlToValidate="txtCost6" 
                                            ErrorMessage="Quantity X Cost can not exceed $215,000.">*</asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtQty7" runat="server" Width="25px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator9" runat="server" ControlToValidate="txtQty7" ErrorMessage="Quantity for item #7 must be a number"
                                            Operator="DataTypeCheck" Type="Integer">*</asp:CompareValidator><asp:CustomValidator
                                                ID="CustomValidator7" runat="server" ControlToValidate="txtContact" ErrorMessage="Item #2 has an error. Please ensure you have provided quantity (valid number), unit, description, and a 'not to exceed' or 'unit cost' (with valid currency format)">*</asp:CustomValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtUnit7" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtItem7" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescription7" runat="server" Rows="3" TextMode="MultiLine" Width="300px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtExceed7" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator18" runat="server" ControlToValidate="txtExceed7" ErrorMessage="Not to exceed amount for item #7 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCost7" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator27" runat="server" ControlToValidate="txtCost7" ErrorMessage="Unit cost for item #7 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                        <asp:CustomValidator ID="CustomValidator18" runat="server" 
                                            ControlToValidate="txtCost7" 
                                            ErrorMessage="Quantity X Cost can not exceed $215,000.">*</asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtQty8" runat="server" Width="25px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator10" runat="server" ControlToValidate="txtQty8" ErrorMessage="Quantity for item #8 must be a number"
                                            Operator="DataTypeCheck" Type="Integer">*</asp:CompareValidator><asp:CustomValidator
                                                ID="CustomValidator8" runat="server" ControlToValidate="txtContact" ErrorMessage="Item #2 has an error. Please ensure you have provided quantity (valid number), unit, description, and a 'not to exceed' or 'unit cost' (with valid currency format)">*</asp:CustomValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtUnit8" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtItem8" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescription8" runat="server" Rows="3" TextMode="MultiLine" Width="300px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtExceed8" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator19" runat="server" ControlToValidate="txtExceed8" ErrorMessage="Not to exceed amount for item #8 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCost8" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator28" runat="server" ControlToValidate="txtCost8" ErrorMessage="Unit cost for item #8 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                        <asp:CustomValidator ID="CustomValidator19" runat="server" 
                                            ControlToValidate="txtCost8" 
                                            ErrorMessage="Quantity X Cost can not exceed $215,000.">*</asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtQty9" runat="server" Width="25px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator11" runat="server" ControlToValidate="txtQty9" ErrorMessage="Quantity for item #9 must be a number"
                                            Operator="DataTypeCheck" Type="Integer">*</asp:CompareValidator><asp:CustomValidator
                                                ID="CustomValidator9" runat="server" ControlToValidate="txtContact" ErrorMessage="Item #2 has an error. Please ensure you have provided quantity (valid number), unit, description, and a 'not to exceed' or 'unit cost' (with valid currency format)">*</asp:CustomValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtUnit9" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtItem9" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescription9" runat="server" Rows="3" TextMode="MultiLine" Width="300px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtExceed9" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator20" runat="server" ControlToValidate="txtExceed9" ErrorMessage="Not to exceed amount for item #9 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCost9" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator29" runat="server" ControlToValidate="txtCost9" ErrorMessage="Unit cost for item #9 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                        <asp:CustomValidator ID="CustomValidator20" runat="server" 
                                            ControlToValidate="txtCost9" 
                                            ErrorMessage="Quantity X Cost can not exceed $215,000.">*</asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtQty10" runat="server" Width="25px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator12" runat="server" ControlToValidate="txtQty10" ErrorMessage="Quantity for item #10 must be a number"
                                            Operator="DataTypeCheck" Type="Integer">*</asp:CompareValidator><asp:CustomValidator
                                                ID="CustomValidator10" runat="server" ControlToValidate="txtContact" ErrorMessage="Item #2 has an error. Please ensure you have provided quantity (valid number), unit, description, and a 'not to exceed' or 'unit cost' (with valid currency format)">*</asp:CustomValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtUnit10" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtItem10" runat="server" Width="75px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescription10" runat="server" Rows="3" TextMode="MultiLine" Width="300px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtExceed10" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator21" runat="server" ControlToValidate="txtExceed10" ErrorMessage="Not to exceed amount for item #10 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCost10" runat="server" Width="100px"></asp:TextBox><asp:CompareValidator
                                            ID="CompareValidator30" runat="server" ControlToValidate="txtCost10" ErrorMessage="Unit cost for item #10 must be a valid dollar amount"
                                            Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                                        <asp:CustomValidator ID="CustomValidator21" runat="server" 
                                            ControlToValidate="txtCost10" 
                                            ErrorMessage="Quantity X Cost can not exceed $215,000.">*</asp:CustomValidator>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <asp:Button ID="Button1" runat="server" Text="Preview Order/Calculate Totals"></asp:Button>
                    </td>
                </tr>
            </tbody>
        </table>
    </asp:Panel>
    <br />
<asp:Panel ID="pnlReview" runat="server" Visible="False" Width="95%">
    <TABLE border="1" width="100%">
        <tbody><tr><td rowspan="9" style="WIDTH: 30%; HEIGHT: 40px" valign="top"><strong>
            Vendor Info<br /><asp:DetailsView 
            ID="DetailsView2" runat="server" AutoGenerateRows="False" BorderStyle="Solid" 
            BorderWidth="0px" DataKeyNames="vendor_id" DataSourceID="sdsPreorderInfo" 
            Font-Bold="False" Height="50px" HorizontalAlign="Left" Width="100%"><Fields><asp:BoundField 
                    DataField="vendor_name" SortExpression="vendor_name"></asp:BoundField><asp:BoundField 
                    DataField="address" ReadOnly="True" SortExpression="address"></asp:BoundField><asp:BoundField 
                    DataField="csz" ReadOnly="True" SortExpression="csz"></asp:BoundField><asp:TemplateField></asp:TemplateField><asp:BoundField 
                    DataField="phone" DataFormatString="Phone: {0}" SortExpression="phone"></asp:BoundField><asp:BoundField 
                    DataField="fax" DataFormatString="Fax: {0}" SortExpression="fax"></asp:BoundField><asp:HyperLinkField 
                    DataNavigateUrlFields="website" DataTextField="vendor_name"></asp:HyperLinkField>
            </Fields>
            <RowStyle BorderStyle="None" HorizontalAlign="Left" />
        </asp:DetailsView></strong></td><td align="left"><strong>*Contact:</strong> <asp:Label 
            ID="lblContact" runat="server" Text="Label"></asp:Label><strong> *Phone: </strong> <asp:Label 
            ID="lblPhone" runat="server" Text="Label"></asp:Label></td></tr><tr><td 
            align="left"><strong>*Authorizer: </strong><asp:Label ID="lblAuthorizer" 
            runat="server" Text="Label"></asp:Label></td></tr><tr><td align="left"><strong>ID#:</strong> <asp:Label 
            ID="lblAcct" runat="server" Text="Label"></asp:Label></td></tr><tr><td 
            align="left"><strong>Shipping:</strong> <asp:Label ID="lblShipping" 
            runat="server" Text="Label"></asp:Label> <strong>Ship to:</strong> <asp:Label 
            ID="lblShipto" runat="server" Text="Label"></asp:Label></td></tr><tr><td 
            align="left"><strong>Radioactive?:</strong> <asp:Label ID="lblRadioactive" 
            runat="server" Text="Label"></asp:Label><strong> RUA#:</strong> <asp:Label 
            ID="lblRUA" runat="server" Text="Label"></asp:Label></td></tr>
            <tr>
                <td align="left">
                    <strong>Controlled Substance?:</strong>
                    <asp:Label ID="lblControlledSubstance" runat="server" Text="Label" />
                    <strong>Class/Schedule: </strong>
                    <asp:Label ID="lblCSClass" runat="server" Text="Label" />
                    <strong>Use: </strong>
                    <asp:Label ID="lblCSUse" runat="server" Text="Label" />
                    <strong>Storage site:</strong>
                    <asp:Label ID="lblCSStore" runat="server" Text="Label" />
                    <strong>Custodian: </strong>
                    <asp:Label ID="lblCSCustodian" runat="server" Text="Label" />
                    <strong>User(s): </strong>
                    <asp:Label ID="lblCSUsers" runat="server" Text="Label" />
                    &nbsp;</td>
            </tr>
            <tr><td 
            align="left"><strong>Check if quote is being sent to purchasing desk:</strong> <asp:Label 
            ID="lblQuote" runat="server" Text="Label"></asp:Label></td></tr><tr><td 
            align="left"><strong>Vendor contact:</strong> <asp:Label ID="lblVContact" 
            runat="server" Text="Label"></asp:Label></td></tr><tr><td align="left"><strong>
                Request comments:</strong> <asp:Label 
            ID="lblComments" runat="server" Text="Label"></asp:Label></td></tr><tr><td 
            align="center" colspan="2"><strong>ITEMS<br /></strong>(*note: you must fill in 
                either &quot;Not to Exceed amount&quot; or &quot;Unit Cost&quot; for each item)<br /><table 
            border="1" width="100%"><tbody><tr><td><strong>*Qty</strong></td><td><strong>*Unit</strong></td><td><strong>
                        Item/<br />Catalog<br />No.</strong></td><td><strong>*Description</strong></td><td><strong>
                        Not to<br />Exceed<br />amount</strong></td><td><strong>Unit Cost</strong></td><td 
                    style="WIDTH: 94px"><strong>Total</strong></td></tr><tr ID="tr1" 
                    runat="server"><td><asp:Label ID="lblQty1" runat="server" Text="Label"></asp:Label></td><td><asp:Label 
                        ID="lblUnit1" runat="server" Text="Label"></asp:Label></td><td><asp:Label 
                            ID="lblItem1" runat="server" Text="Label"></asp:Label>&nbsp;</td><td><asp:Label 
                            ID="lblDescription1" runat="server" Text="Label"></asp:Label></td><td><asp:Label 
                            ID="lblExceed1" runat="server" Text="Label"></asp:Label>&nbsp;</td><td><asp:Label 
                            ID="lblCost1" runat="server" Text="Label"></asp:Label>&nbsp;</td><td 
                        style="WIDTH: 94px"><asp:Label ID="lblTotal1" runat="server" Text="Label"></asp:Label></td></tr><tr 
                    ID="tr2" runat="server"><td><asp:Label ID="lblQty2" runat="server" Text="Label"></asp:Label></td><td><asp:Label 
                        ID="lblUnit2" runat="server" Text="Label"></asp:Label></td><td><asp:Label 
                            ID="lblItem2" runat="server" Text="Label"></asp:Label>&nbsp;</td><td><asp:Label 
                            ID="lblDescription2" runat="server" Text="Label"></asp:Label></td><td><asp:Label 
                            ID="lblExceed2" runat="server" Text="Label"></asp:Label>&nbsp;</td><td><asp:Label 
                            ID="lblCost2" runat="server" Text="Label"></asp:Label>&nbsp;</td><td 
                        style="WIDTH: 94px"><asp:Label ID="lblTotal2" runat="server" Text="Label"></asp:Label></td></tr><tr 
                    ID="tr3" runat="server"><td><asp:Label ID="lblQty3" runat="server" Text="Label"></asp:Label></td><td><asp:Label 
                        ID="lblUnit3" runat="server" Text="Label"></asp:Label></td><td><asp:Label 
                            ID="lblItem3" runat="server" Text="Label"></asp:Label>&nbsp;</td><td><asp:Label 
                            ID="lblDescription3" runat="server" Text="Label"></asp:Label></td><td><asp:Label 
                            ID="lblExceed3" runat="server" Text="Label"></asp:Label>&nbsp;</td><td><asp:Label 
                            ID="lblCost3" runat="server" Text="Label"></asp:Label>&nbsp;</td><td 
                        style="WIDTH: 94px"><asp:Label ID="lblTotal3" runat="server" Text="Label"></asp:Label></td></tr><tr 
                    ID="tr4" runat="server"><td><asp:Label ID="lblQty4" runat="server" Text="Label"></asp:Label></td><td><asp:Label 
                        ID="lblUnit4" runat="server" Text="Label"></asp:Label></td><td><asp:Label 
                            ID="lblItem4" runat="server" Text="Label"></asp:Label>&nbsp;</td><td><asp:Label 
                            ID="lblDescription4" runat="server" Text="Label"></asp:Label></td><td><asp:Label 
                            ID="lblExceed4" runat="server" Text="Label"></asp:Label>&nbsp;</td><td><asp:Label 
                            ID="lblCost4" runat="server" Text="Label"></asp:Label>&nbsp;</td><td 
                        style="WIDTH: 94px"><asp:Label ID="lblTotal4" runat="server" Text="Label"></asp:Label></td></tr><tr 
                    ID="tr5" runat="server"><td><asp:Label ID="lblQty5" runat="server" Text="Label"></asp:Label></td><td><asp:Label 
                        ID="lblUnit5" runat="server" Text="Label"></asp:Label></td><td><asp:Label 
                            ID="lblItem5" runat="server" Text="Label"></asp:Label>&nbsp;</td><td><asp:Label 
                            ID="lblDescription5" runat="server" Text="Label"></asp:Label></td><td><asp:Label 
                            ID="lblExceed5" runat="server" Text="Label"></asp:Label>&nbsp; </td><td><asp:Label 
                            ID="lblCost5" runat="server" Text="Label"></asp:Label> &nbsp;</td><td 
                        style="WIDTH: 94px"><asp:Label ID="lblTotal5" runat="server" Text="Label"></asp:Label> </td></tr><tr 
                    ID="tr6" runat="server"><td><asp:Label ID="lblQty6" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                        ID="lblUnit6" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblItem6" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblDescription6" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblExceed6" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblCost6" runat="server" Text="Label"></asp:Label> </td><td 
                        style="WIDTH: 94px"><asp:Label ID="lblTotal6" runat="server" Text="Label"></asp:Label> </td></tr><tr 
                    ID="tr7" runat="server"><td><asp:Label ID="lblQty7" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                        ID="lblUnit7" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblItem7" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblDescription7" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblExceed7" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblCost7" runat="server" Text="Label"></asp:Label> </td><td 
                        style="WIDTH: 94px"><asp:Label ID="lblTotal7" runat="server" Text="Label"></asp:Label> </td></tr><tr 
                    ID="tr8" runat="server"><td><asp:Label ID="lblQty8" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                        ID="lblUnit8" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblItem8" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblDescription8" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblExceed8" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblCost8" runat="server" Text="Label"></asp:Label> </td><td 
                        style="WIDTH: 94px"><asp:Label ID="lblTotal8" runat="server" Text="Label"></asp:Label> </td></tr><tr 
                    ID="tr9" runat="server"><td><asp:Label ID="lblQty9" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                        ID="lblUnit9" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblItem9" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblDescription9" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblExceed9" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblCost9" runat="server" Text="Label"></asp:Label> </td><td 
                        style="WIDTH: 94px"><asp:Label ID="lblTotal9" runat="server" Text="Label"></asp:Label> </td></tr><tr 
                    ID="tr10" runat="server"><td><asp:Label ID="lblQty10" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                        ID="lblUnit10" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblItem10" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblDescription10" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblExceed10" runat="server" Text="Label"></asp:Label> </td><td><asp:Label 
                            ID="lblCost10" runat="server" Text="Label"></asp:Label> </td><td 
                        style="WIDTH: 94px"><asp:Label ID="lblTotal10" runat="server" Text="Label"></asp:Label> </td></tr><tr 
                    ID="Tr11" runat="server"><td align="right" colspan="6">Subtotal:</td><td 
                        style="WIDTH: 94px"><asp:Label ID="lblSubTotal" runat="server" Text="Label"></asp:Label> </td></tr><tr 
                    ID="Tr12" runat="server"><td align="right" colspan="6">Tax:</td><td 
                        style="WIDTH: 94px"><asp:Label ID="lblTax" runat="server" Text="Label"></asp:Label> </td></tr><tr 
                    ID="Tr13" runat="server"><td align="right" colspan="6">Total:</td><td 
                        style="WIDTH: 94px"><asp:Label ID="lblTotal" runat="server" Text="Label"></asp:Label> </td></tr></tbody></table><br />
                I certify that I have read and understand the
                <asp:Label ID="lblDept" runat="server" Text="Label"></asp:Label>
                &nbsp;purchasing guidelines, and the University&#39;s Conflict of Interest Rules, and am 
                aware of the University’s purchasing policies (PPM 350).<p>Campus purchasing 
                    policies are available on-line at: <a 
            href="http://manuals.ucdavis.edu/ppm/contents.htm#350" target="_blank">
                    http://manuals.ucdavis.edu/ppm/contents.htm#350</a></p><p>
                    The University’s policy regarding conflict of interest in purchasing summary is 
                    available on-line at: <a 
            href="http://manuals.ucdavis.edu/directory/coi.htm" target="_blank">
                    http://manuals.ucdavis.edu/directory/coi.htm</a></p><p>By clicking &quot;Place 
                    Order&quot;, you agree to the above statements.<br /></p></td></tr><tr><td 
            align="center" colspan="2"><asp:Button ID="Button2" runat="server" 
            Text="Place Order"></asp:Button>&nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="Button3" runat="server" 
            Text="Edit Order"></asp:Button></td></tr></tbody>
    </TABLE>
    &nbsp; &nbsp; &nbsp;
</asp:Panel>
    <br />
    <asp:Panel ID="pnlTest" runat="server" Width="75%" Visible="False">
        &nbsp;Test:
        <asp:Label ID="lblTest" runat="server"></asp:Label><br />
        REmailName: &quot;<asp:Label ID="lblREmailName" runat="server"></asp:Label>&quot;<br />
        AuthName
        <asp:Label ID="lblAuthorizerName" runat="server"></asp:Label><br />
        Approved:
        <asp:Label ID="lblApproved" runat="server"></asp:Label><br />
        Email Addres:
        <asp:Label ID="lblEmailAddress" runat="server"></asp:Label><br />
        EmailName:
        <asp:Label ID="lblEmailName" runat="server"></asp:Label><br />
        REmail Address: &quot;<asp:Label ID="lblREmailAddress" runat="server"></asp:Label>&quot;
        <br />
        Order_id:
        <asp:Label ID="lblOrderID" runat="server" Text="Label"></asp:Label>
        <br />
    </asp:Panel>
<br />
<asp:Panel ID="pnlApproverConfirm" runat="server" HorizontalAlign="Left" 
    Visible="False" Width="95%">
    Because you are the approver for this request, your order has been processed and 
    approved. It will now be sent to the business office for processing.
    <br />
    <br />
    This order has been assigned the tag number &quot;<asp:Label ID="lblTag" 
        runat="server" Text="Label"></asp:Label>
    &quot;. Please reference this number with all attachments being sent to the 
    purchasing desk. If you wish to contact the purchasing desk, you may send an 
    email
    <asp:HyperLink ID="HyperLink2" runat="server">HyperLink</asp:HyperLink>
    now.
</asp:Panel>
<asp:Panel ID="pnlSubmitterConfirm" runat="server" HorizontalAlign="Left" 
    Visible="False" Width="95%">
    The Authorizer you selected has been sent an email indicating they have a new 
    order to review. You will be notified after action has been taken on this order.
    <br />
    <br />
    This order has been assigned the tag number &quot;<asp:Label ID="lblTag1" 
        runat="server" Text="Label"></asp:Label>
    &quot;. Please reference this number with all attachments being sent to the 
    purchasing desk. If you wish to contact the purchasing desk, you may send an 
    email
    <asp:HyperLink ID="HyperLink1" runat="server">HyperLink</asp:HyperLink>
    now.
</asp:Panel>
    <asp:Panel ID="pnlQuote" runat="server" Visible="False" Width="95%">
        <hr />
        <br />
        You indicated this order has a quote associated with it. If ready, please attach 
        to this order:<br />
        <asp:FileUpload ID="fuQuote" runat="server" />
        <asp:CustomValidator ID="cvalQuote" runat="server" 
            ErrorMessage="CustomValidator"></asp:CustomValidator>
        <br />
        <asp:Button ID="Button5" runat="server" Text="Upload" />
        
        <asp:GridView ID="gvAttachments" runat="server" AutoGenerateColumns="False" EnableViewState="False"
                DataSourceID="sdsFiles" SkinID="notfull">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFields="order_id,attach_id" DataNavigateUrlFormatString="filedownload.aspx?order_id={0}&amp;attach_id={1}"
                        DataTextField="attach_name" DataTextFormatString="{0}" 
                        HeaderText="Attachments" />
                </Columns>
            </asp:GridView>       
    </asp:Panel>
</asp:Content>
