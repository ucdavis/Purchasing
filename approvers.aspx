<%@ Page Language="VB" MasterPageFile="~/ops.master" AutoEventWireup="false" CodeFile="approvers.aspx.vb" Inherits="approvers" title="" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:SqlDataSource ID="sdsAllApprovers" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        SelectCommand="approvers_all" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsApproverWork" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        InsertCommand="approver_insert" InsertCommandType="StoredProcedure" 
        SelectCommand="approver_individual" SelectCommandType="StoredProcedure" 
        UpdateCommand="approver_update" UpdateCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridView1" Name="approver_id" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="emp_id" Type="Int32" />
            <asp:Parameter Name="on_behalf_of" Type="Int32" />
            <asp:Parameter Name="start_date" Type="DateTime" />
            <asp:Parameter Name="end_date" Type="DateTime" />
            <asp:Parameter Name="up_to_amount" Type="Decimal" />
            <asp:Parameter Name="budget_id" Type="Int32" />
            <asp:Parameter Name="purch_id" Type="Int32" />
            <asp:Parameter Name="approver_comments" Type="String" />
            <asp:Parameter Name="current" Type="Boolean" />
            <asp:Parameter Name="approver_id" Type="Int32" />
        </UpdateParameters>
        <InsertParameters>
            <asp:Parameter Name="RETURN_VALUE" Type="Int32" Direction="ReturnValue" />
            <asp:Parameter Name="emp_id" Type="Int32" />
            <asp:Parameter Name="on_behalf_of" Type="Int32" />
            <asp:Parameter Name="start_date" Type="DateTime" />
            <asp:Parameter Name="end_date" Type="DateTime" />
            <asp:Parameter Name="up_to_amount" Type="Decimal" />
            <asp:Parameter Name="budget_id" Type="Int32" />
            <asp:Parameter Name="purch_id" Type="Int32" />
            <asp:Parameter Name="approver_comments" Type="String" />
            <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
        </InsertParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsEmployees" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        SelectCommand="all_employees" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsAdmins" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" SelectCommand="admin_names" 
        SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsCurrentApprovers" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        SelectCommand="current_primary_approvers" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <br />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            <br />
            <asp:FormView ID="FormView1" runat="server" BorderColor="#91AF6F" 
                BorderStyle="None" BorderWidth="2px" DataKeyNames="approver_id" 
                DataSourceID="sdsApproverWork" DefaultMode="Edit" Width="75%">
                <EditItemTemplate>
                    ApproverApprover Name: Name:
                    <asp:DropDownList ID="ddlApprover" runat="server" DataSourceID="sdsEmployees" 
                        DataTextField="name" DataValueField="emp_id" Enabled="False" 
                        SelectedValue='<%# Bind("emp_id") %>'>
                    </asp:DropDownList>
                    <br />
                    <br />
                    <hr />
                    <br />
                    &nbsp;<table width="75%">
                        <tr>
                            <td>
                                <strong>On Behalf of: </strong>
                            </td>
                            <td align="left">
                                <asp:DropDownList ID="ddlOnBehalfOf" runat="server" 
                                    DataSourceID="sdsCurrentApprovers" DataTextField="name" DataValueField="emp_id" 
                                    Enabled="False" SelectedValue='<%# Bind("on_behalf_of") %>'>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>Start Date:</strong></td>
                            <td align="left">
                                <asp:TextBox ID="start_dateTextBox" runat="server" 
                                    Text='<%# Bind("start_date") %>'></asp:TextBox>
                                <asp:CompareValidator ID="CompareValidator4" runat="server" 
                                    ControlToValidate="start_dateTextBox" 
                                    ErrorMessage="Start date must be a valid date" Operator="DataTypeCheck" 
                                    Type="Date">*</asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>End Date:</strong></td>
                            <td align="left">
                                <asp:TextBox ID="end_dateTextBox" runat="server" Text='<%# Bind("end_date") %>'></asp:TextBox>
                                <asp:CompareValidator ID="CompareValidator5" runat="server" 
                                    ControlToValidate="end_dateTextBox" 
                                    ErrorMessage="End date must be a valid date" Operator="DataTypeCheck" 
                                    Type="Date">*</asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>Up to Amount:</strong></td>
                            <td align="left">
                                <asp:TextBox ID="up_to_amountTextBox" runat="server" 
                                    Text='<%# Bind("up_to_amount", "{0:N}") %>'></asp:TextBox>
                                <asp:CompareValidator ID="CompareValidator6" runat="server" 
                                    ControlToValidate="up_to_amountTextBox" 
                                    ErrorMessage="Up to amount must be a valid dollar amount" 
                                    Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>Budget Asst:</strong></td>
                            <td align="left">
                                <asp:DropDownList ID="ddlBudget" runat="server" DataSourceID="sdsAdmins" 
                                    DataTextField="name" DataValueField="emp_id" 
                                    SelectedValue='<%# Bind("budget_id") %>'>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>Purchasing Asst:</strong></td>
                            <td align="left">
                                <asp:DropDownList ID="ddlPurchasing" runat="server" DataSourceID="sdsAdmins" 
                                    DataTextField="name" DataValueField="emp_id" 
                                    SelectedValue='<%# Bind("purch_id") %>'>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>HR Asst:</b></td>
                            <td align="left">
                                <asp:DropDownList ID="ddlHR" runat="server" DataSourceID="sdsAdmins" 
                                    DataTextField="name" DataValueField="emp_id" 
                                    SelectedValue='<%# Bind("hr_id") %>'>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                <strong>Comments:</strong></td>
                            <td align="left">
                                <asp:TextBox ID="approver_commentsTextBox" runat="server" Rows="3" 
                                    Text='<%# Bind("approver_comments") %>' TextMode="MultiLine"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                <strong>Current:</strong></td>
                            <td align="left">
                                <asp:CheckBox ID="currentCheckBox" runat="server" 
                                    Checked='<%# Bind("current") %>' />
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:Button ID="btnUpdate" runat="server" CommandName="Update" Text="Update" />
                    &nbsp;
                    <asp:Button ID="btnCancel" runat="server" CommandName="Cancel" 
                        Text="Cancel Selection" />
                    <br />
                    <br />
                </EditItemTemplate>
                <InsertItemTemplate>
                    Approver Name:
                    <asp:DropDownList ID="ddlApprover" runat="server" DataSourceID="sdsEmployees" 
                        DataTextField="name" DataValueField="emp_id" 
                        SelectedValue='<%# Bind("emp_id") %>'>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                        ControlToValidate="ddlApprover" ErrorMessage="Must select an approver">*</asp:RequiredFieldValidator>
                    <br />
                    <br />
                    <hr />
                    <br />
                    &nbsp;<table width="75%">
                        <tr>
                            <td>
                                <strong>On Behalf of: </strong>
                            </td>
                            <td align="left">
                                <asp:DropDownList ID="ddlOnBehalfOf" runat="server" 
                                    DataSourceID="sdsCurrentApprovers" DataTextField="name" DataValueField="emp_id" 
                                    SelectedValue='<%# Bind("on_behalf_of") %>'>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>Start Date:</strong></td>
                            <td align="left">
                                <asp:TextBox ID="start_dateTextBox" runat="server" 
                                    Text='<%# Bind("start_date") %>'></asp:TextBox>
                                <asp:CompareValidator ID="CompareValidator1" runat="server" 
                                    ControlToValidate="start_dateTextBox" 
                                    ErrorMessage="Start date must be a valid date" Operator="DataTypeCheck" 
                                    Type="Date">*</asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>End Date:</strong></td>
                            <td align="left">
                                <asp:TextBox ID="end_dateTextBox" runat="server" 
                                    Text='<%# Bind("end_date") %>'></asp:TextBox>
                                <asp:CompareValidator ID="CompareValidator2" runat="server" 
                                    ControlToValidate="end_dateTextBox" 
                                    ErrorMessage="End date must be a valid date" Operator="DataTypeCheck" 
                                    Type="Date">*</asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>Up to Amount:</strong></td>
                            <td align="left">
                                <asp:TextBox ID="up_to_amountTextBox" runat="server" 
                                    Text='<%# Bind("up_to_amount") %>'></asp:TextBox>
                                <asp:CompareValidator ID="CompareValidator3" runat="server" 
                                    ControlToValidate="up_to_amountTextBox" 
                                    ErrorMessage="Up to amount must be a valid money amount" 
                                    Operator="DataTypeCheck" Type="Currency">*</asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>Budget Asst:</strong></td>
                            <td align="left">
                                <asp:DropDownList ID="ddlBudget" runat="server" DataSourceID="sdsAdmins" 
                                    DataTextField="name" DataValueField="emp_id" 
                                    SelectedValue='<%# Bind("budget_id") %>'>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                    ControlToValidate="ddlBudget" ErrorMessage="You must select a budget assistant"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <strong>Purchasing Asst:</strong></td>
                            <td align="left">
                                <asp:DropDownList ID="ddlPurchID" runat="server" DataSourceID="sdsAdmins" 
                                    DataTextField="name" DataValueField="emp_id" 
                                    SelectedValue='<%# Bind("purch_id") %>'>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                                    ControlToValidate="ddlPurchID" 
                                    ErrorMessage="You must select a purchasing assistant"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>HR Asst:</b></td>
                            <td align="left">
                                <asp:DropDownList ID="ddlHR" runat="server" DataSourceID="sdsAdmins" 
                                    DataTextField="name" DataValueField="emp_id" 
                                    SelectedValue='<%# Bind("hr_id") %>'>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                    ControlToValidate="ddlHR" ErrorMessage="You must select a HR assistant"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                <strong>Comments:</strong></td>
                            <td align="left">
                                <asp:TextBox ID="approver_commentsTextBox" runat="server" Rows="5" 
                                    Text='<%# Bind("approver_comments") %>' TextMode="MultiLine" Width="300px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <asp:Button ID="btnUpdate" runat="server" CommandName="Insert" Text="Insert" />
                    &nbsp;
                    <asp:Button ID="btnCancel" runat="server" CommandName="Cancel" Text="Cancel" />
                    <br />
                    <br />
                </InsertItemTemplate>
                <ItemTemplate>
                    approver_id:
                    <asp:Label ID="approver_idLabel" runat="server" 
                        Text='<%# Eval("approver_id") %>'></asp:Label>
                    <br />
                    employee_id:
                    <asp:Label ID="employee_idLabel" runat="server" 
                        Text='<%# Bind("employee_id") %>'></asp:Label>
                    <br />
                    on_behalf_of:
                    <asp:Label ID="on_behalf_ofLabel" runat="server" 
                        Text='<%# Bind("on_behalf_of") %>'></asp:Label>
                    <br />
                    start_date:
                    <asp:Label ID="start_dateLabel" runat="server" Text='<%# Bind("start_date") %>'></asp:Label>
                    <br />
                    end_date:
                    <asp:Label ID="end_dateLabel" runat="server" Text='<%# Bind("end_date") %>'></asp:Label>
                    <br />
                    up_to_amount:
                    <asp:Label ID="up_to_amountLabel" runat="server" 
                        Text='<%# Bind("up_to_amount") %>'></asp:Label>
                    <br />
                    purch_asst:
                    <asp:Label ID="purch_asstLabel" runat="server" Text='<%# Bind("purch_asst") %>'></asp:Label>
                    <br />
                    approver_comments:
                    <asp:Label ID="approver_commentsLabel" runat="server" 
                        Text='<%# Bind("approver_comments") %>'></asp:Label>
                    <br />
                    current:
                    <asp:CheckBox ID="currentCheckBox0" runat="server" 
                        Checked='<%# Bind("current") %>' Enabled="false" />
                    <br />
                    <asp:LinkButton ID="EditButton" runat="server" CausesValidation="False" 
                        CommandName="Edit" Text="Edit"></asp:LinkButton>
                    <asp:LinkButton ID="NewButton" runat="server" CausesValidation="False" 
                        CommandName="New" Text="New"></asp:LinkButton>
                    <br />
                </ItemTemplate>
            </asp:FormView>
            <br />
            <asp:Button ID="btnNew" runat="server" Text="Add New Approver" />
            <br />
            <asp:GridView ID="GridView1" runat="server" AllowSorting="True" 
                AutoGenerateColumns="False" DataKeyNames="approver_id" 
                DataSourceID="sdsAllApprovers">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="name" HeaderText="Name" ReadOnly="True" 
                        SortExpression="last_name" />
                    <asp:BoundField DataField="on_behalf_of" HeaderText="On Behalf Of" 
                        ReadOnly="True" SortExpression="on_behalf_of" />
                    <asp:BoundField DataField="special_account" HeaderText="Budget Asst" 
                        SortExpression="special_account" />
                    <asp:BoundField DataField="purch_asst" HeaderText="Purch. Asst." 
                        ReadOnly="True" SortExpression="purch_asst" />
                    <asp:BoundField DataField="hr_name" HeaderText="HR Asst." 
                        SortExpression="hr_name" />
                    <asp:CheckBoxField DataField="current" HeaderText="Current?" 
                        SortExpression="current" />
                </Columns>
            </asp:GridView>
            <br />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

