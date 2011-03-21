<%@ Page Language="VB" MasterPageFile="~/ops.master" AutoEventWireup="false" CodeFile="budget_setup.aspx.vb" Inherits="budget_setup" title="" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:SqlDataSource ID="sdsAcctAsst" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        SelectCommand="acct_asst_employees" SelectCommandType="StoredProcedure" 
        UpdateCommand="update_budget_auto_approve" UpdateCommandType="StoredProcedure">
        <UpdateParameters>
            <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
            <asp:Parameter Name="emp_id" Type="Int32" />
            <asp:Parameter Name="auto_approve" Type="Boolean" />
        </UpdateParameters>
        <SelectParameters>
            <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        DataKeyNames="emp_id" DataSourceID="sdsAcctAsst">
                <Columns>
                    <asp:CommandField ShowEditButton="True" />
                    <asp:BoundField DataField="name" HeaderText="Name" ReadOnly="True" 
                SortExpression="name" />
                    <asp:CheckBoxField DataField="auto_approve" HeaderText="Auto Approve" 
                SortExpression="auto_approve" />
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

