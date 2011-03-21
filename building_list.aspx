<%@ Page Language="VB" MasterPageFile="~/ops.master" AutoEventWireup="false" CodeFile="building_list.aspx.vb" Inherits="building_list" title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        DeleteCommand="delete_building" DeleteCommandType="StoredProcedure" 
        InsertCommand="insert_building" InsertCommandType="StoredProcedure" 
        SelectCommand="select_buildings_admin" SelectCommandType="StoredProcedure" 
        UpdateCommand="update_building" UpdateCommandType="StoredProcedure">
        <SelectParameters>
            <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
        </SelectParameters>
        <DeleteParameters>
            <asp:Parameter Name="bld_id" Type="Int32" />
        </DeleteParameters>
        <UpdateParameters>
            <asp:Parameter Name="bld_id" Type="Int32" />
            <asp:Parameter Name="bld_name" Type="String" />
            <asp:Parameter Name="bld_order" Type="Byte" />
        </UpdateParameters>
        <InsertParameters>
            <asp:SessionParameter Name="app_id" SessionField="app_id" Type="Int32" />
            <asp:Parameter Name="bld_name" Type="String" />
            <asp:Parameter Name="bld_order" Type="Byte" />
        </InsertParameters>
    </asp:SqlDataSource>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:FormView ID="FormView1" runat="server" 
    DataKeyNames="bld_id" DataSourceID="SqlDataSource1" DefaultMode="Insert" 
    Visible="False">
                <EditItemTemplate>
                    bld_id:
                    <asp:Label ID="bld_idLabel1" runat="server" 
            Text='<%# Eval("bld_id") %>' />
                    <br />
                    bld_name:
                    <asp:TextBox ID="bld_nameTextBox" runat="server" 
            Text='<%# Bind("bld_name") %>' />
                    <br />
                    bld_order:
                    <asp:TextBox ID="bld_orderTextBox" runat="server" 
            Text='<%# Bind("bld_order") %>' />
                    <br />
                    <asp:LinkButton ID="UpdateButton" runat="server" 
            CausesValidation="True" CommandName="Update" Text="Update" />
                    &nbsp;<asp:LinkButton ID="UpdateCancelButton" runat="server" 
            CausesValidation="False" CommandName="Cancel" Text="Cancel" />
                </EditItemTemplate>
                <InsertItemTemplate>
                    Name:
                    <asp:TextBox ID="bld_nameTextBox" runat="server" 
            Text='<%# Bind("bld_name") %>' />
                    <br />
                    Order:
                    <asp:TextBox ID="bld_orderTextBox" runat="server" 
            Text='<%# Bind("bld_order") %>' />
                    <br />
                    &nbsp;<asp:Button ID="Button2" runat="server" 
            CommandName="Insert" Text="Add Building" />
                    &nbsp;<asp:Button ID="Button3" runat="server" 
            CommandName="Cancel" Text="Cancel" />
                </InsertItemTemplate>
                <ItemTemplate>
                    bld_id:
                    <asp:Label ID="bld_idLabel" runat="server" 
            Text='<%# Eval("bld_id") %>' />
                    <br />
                    bld_name:
                    <asp:Label ID="bld_nameLabel" runat="server" 
            Text='<%# Bind("bld_name") %>' />
                    <br />
                    bld_order:
                    <asp:Label ID="bld_orderLabel" runat="server" 
            Text='<%# Bind("bld_order") %>' />
                    <br />
                    <asp:LinkButton ID="EditButton" runat="server" 
            CausesValidation="False" CommandName="Edit" Text="Edit" />
                    &nbsp;<asp:LinkButton ID="DeleteButton" runat="server" 
            CausesValidation="False" CommandName="Delete" Text="Delete" />
                    &nbsp;<asp:LinkButton ID="NewButton" runat="server" 
            CausesValidation="False" CommandName="New" Text="New" />
                </ItemTemplate>
            </asp:FormView>
            <br />
            <asp:Button ID="Button1" runat="server" Text="Add New Building" />
            <asp:GridView ID="GridView1" runat="server" AllowSorting="True" 
                AutoGenerateColumns="False" DataKeyNames="bld_id" DataSourceID="SqlDataSource1">
                <Columns>
                    <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
                    <asp:BoundField DataField="bld_name" HeaderText="Building Name" 
                        SortExpression="bld_name" />
                    <asp:BoundField DataField="bld_order" HeaderText="Order" 
                        SortExpression="bld_order" />
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
</asp:Content>

