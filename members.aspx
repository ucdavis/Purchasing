<%@ Page Language="VB" MasterPageFile="~/ops.master" AutoEventWireup="false" CodeFile="members.aspx.vb" Inherits="members" title="" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h1>
        List of Members </h1><asp:SqlDataSource ID="sdsEmployees" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
            SelectCommand="select_employees" SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsEmployeesDetails" runat="server" 
            ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
            SelectCommand="employees_details" SelectCommandType="StoredProcedure" 
        InsertCommand="insert_employees" InsertCommandType="StoredProcedure" 
        UpdateCommand="update_employees" UpdateCommandType="StoredProcedure">
            <SelectParameters>
                <asp:ControlParameter ControlID="GridView1" Name="emp_id" 
                    PropertyName="SelectedValue" Type="Int32" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                <asp:Parameter Name="emp_id" Type="Int32" />
                <asp:Parameter Name="first_name" Type="String" />
                <asp:Parameter Name="last_name" Type="String" />
                <asp:Parameter Name="campus_phone" Type="String" />
                <asp:Parameter Name="kerberos_id" Type="String" />
                <asp:Parameter Name="current" Type="Boolean" />
                <asp:Parameter Name="admin_role" Type="String" />
            </UpdateParameters>
            <InsertParameters>
                <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
                <asp:Parameter Name="first_name" Type="String" />
                <asp:Parameter Name="last_name" Type="String" />
                <asp:Parameter Name="campus_phone" Type="String" />
                <asp:Parameter Name="kerberos_id" Type="String" />
                <asp:Parameter Name="admin_role" Type="String" />
            </InsertParameters>
        </asp:SqlDataSource>
   
    <asp:SqlDataSource ID="sdsLabs" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        SelectCommand="current_primary_approvers" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
   
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Button ID="btnNew" runat="server" Text="Add New Employee" />
            <asp:FormView ID="FormView1" runat="server" DataKeyNames="emp_id" 
                DataSourceID="sdsEmployeesDetails" DefaultMode="Edit">
                <EditItemTemplate>
                    <table style="width:100%;">
                        <tr>
                            <td>
                                First Name:</td>
                            <td>
                                &nbsp;</td>
                            <td>
                                <asp:TextBox ID="first_nameTextBox" runat="server" 
                                    Text='<%# Bind("first_name") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Last Name:</td>
                            <td>
                                &nbsp;</td>
                            <td>
                                <asp:TextBox ID="last_nameTextBox" runat="server" 
                                    Text='<%# Bind("last_name") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Campus Phone:</td>
                            <td>
                                &nbsp;</td>
                            <td>
                                <asp:TextBox ID="campus_phoneTextBox" runat="server" 
                                    Text='<%# Bind("campus_phone") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Kerberos ID</td>
                            <td>
                                &nbsp;</td>
                            <td style="margin-left: 40px">
                                <asp:TextBox ID="kerberos_idTextBox" runat="server" 
                                    Text='<%# Bind("kerberos_id") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Current:</td>
                            <td>
                                &nbsp;</td>
                            <td style="margin-left: 40px">
                                <asp:CheckBox ID="currentCheckBox" runat="server" 
                                    Checked='<%# Bind("current") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Lab:</td>
                            <td>
                                &nbsp;</td>
                            <td style="margin-left: 40px">
                                <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="sdsLabs" 
                                    DataTextField="name" DataValueField="emp_id" SelectedValue='<%# Bind("lab") %>'>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Admin Role:</td>
                            <td>
                                &nbsp;</td>
                            <td style="margin-left: 40px">
                                <asp:DropDownList ID="ddlRole" runat="server" 
                                    SelectedValue='<%# Bind("admin_role") %>'>
                                    <asp:ListItem Value="none">None</asp:ListItem>
                                    <asp:ListItem Value="dept_auth">Department Authorizer</asp:ListItem>
                                    <asp:ListItem Value="acct_asst">Accounting Assistant</asp:ListItem>
                                    <asp:ListItem Value="purch_asst">Purchasing Assistant</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:Button ID="Button1" runat="server" CommandName="Update" Text="Update" />
                    &nbsp;
                    <asp:Button ID="btnCancel" runat="server" CommandName="Cancel" 
                        onclick="Button2_Click" Text="Cancel Selection" />
                </EditItemTemplate>
                <InsertItemTemplate>
                    <table style="width:100%;">
                        <tr>
                            <td>
                                First Name:</td>
                            <td>
                                &nbsp;</td>
                            <td>
                                <asp:TextBox ID="first_nameTextBox" runat="server" 
                                    Text='<%# Bind("first_name") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Last Name:</td>
                            <td>
                                &nbsp;</td>
                            <td>
                                <asp:TextBox ID="last_nameTextBox" runat="server" 
                                    Text='<%# Bind("last_name") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Campus Phone:</td>
                            <td>
                                &nbsp;</td>
                            <td>
                                <asp:TextBox ID="campus_phoneTextBox" runat="server" 
                                    Text='<%# Bind("campus_phone") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Kerberos ID</td>
                            <td>
                                &nbsp;</td>
                            <td style="margin-left: 40px">
                                <asp:TextBox ID="kerberos_idTextBox" runat="server" 
                                    Text='<%# Bind("kerberos_id") %>' />
                            </td>
                        </tr>
                         <tr>
                            <td>
                                Lab:</td>
                            <td>
                                &nbsp;</td>
                            <td style="margin-left: 40px">
                                <asp:DropDownList ID="DropDownList1" runat="server" DataSourceID="sdsLabs" 
                                    DataTextField="name" DataValueField="emp_id" SelectedValue='<%# Bind("lab") %>'>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Admin Role:</td>
                            <td>
                                &nbsp;</td>
                            <td style="margin-left: 40px">
                                <asp:DropDownList ID="ddlRole" runat="server" 
                                    SelectedValue='<%# Bind("admin_role") %>'>
                                    <asp:ListItem Value="none">None</asp:ListItem>
                                    <asp:ListItem Value="dept_auth">Department Authorizer</asp:ListItem>
                                    <asp:ListItem Value="acct_asst">Accounting Assistant</asp:ListItem>
                                    <asp:ListItem Value="purch_asst">Purchasing Assistant</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:Button ID="Button1" runat="server" CommandName="Insert" 
                        Text="Add Member" />
                    &nbsp;
                    <asp:Button ID="btnCancel" runat="server" CommandName="Cancel" 
                        onclick="Button2_Click" Text="Cancel" />
                </InsertItemTemplate>
                <ItemTemplate>
                    emp_id:
                    <asp:Label ID="emp_idLabel" runat="server" Text='<%# Eval("emp_id") %>' />
                    <br />
                    first_name:
                    <asp:Label ID="first_nameLabel" runat="server" 
                        Text='<%# Bind("first_name") %>' />
                    <br />
                    middle_name:
                    <asp:Label ID="middle_nameLabel" runat="server" 
                        Text='<%# Bind("middle_name") %>' />
                    <br />
                    last_name:
                    <asp:Label ID="last_nameLabel" runat="server" Text='<%# Bind("last_name") %>' />
                    <br />
                    ucd_mailid:
                    <asp:Label ID="ucd_mailidLabel" runat="server" 
                        Text='<%# Bind("ucd_mailid") %>' />
                    <br />
                    campus_room:
                    <asp:Label ID="campus_roomLabel" runat="server" 
                        Text='<%# Bind("campus_room") %>' />
                    <br />
                    campus_bldg:
                    <asp:Label ID="campus_bldgLabel" runat="server" 
                        Text='<%# Bind("campus_bldg") %>' />
                    <br />
                    campus_phone:
                    <asp:Label ID="campus_phoneLabel" runat="server" 
                        Text='<%# Bind("campus_phone") %>' />
                    <br />
                    kerberos_id:
                    <asp:Label ID="kerberos_idLabel" runat="server" 
                        Text='<%# Bind("kerberos_id") %>' />
                    <br />
                    current:
                    <asp:CheckBox ID="currentCheckBox" runat="server" 
                        Checked='<%# Bind("current") %>' Enabled="false" />
                    <br />
                    admin_role:
                    <asp:Label ID="admin_roleLabel" runat="server" 
                        Text='<%# Bind("admin_role") %>' />
                    <br />
                </ItemTemplate>
            </asp:FormView>
            <asp:GridView ID="GridView1" runat="server" AllowPaging="True" 
                AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="emp_id" 
                DataSourceID="sdsEmployees" PageSize="100">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="first_name" HeaderText="First Name" 
                        SortExpression="first_name" />
                    <asp:BoundField DataField="last_name" HeaderText="Last Name" 
                        SortExpression="last_name" />
                    <asp:BoundField DataField="kerberos_id" HeaderText="Kerberos ID" 
                        SortExpression="kerberos_id" />
                    <asp:BoundField DataField="admin_role" HeaderText="Admin Role" 
                        SortExpression="admin_role" />
                    <asp:CheckBoxField DataField="current" HeaderText="Current" 
                        SortExpression="current" />
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
   
    <p>
        &nbsp;</p>
</asp:Content>

