<%@ Page Language="VB" MasterPageFile="~/ops.master" AutoEventWireup="false" CodeFile="add_vendor.aspx.vb" Inherits="add_vendor" title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table align="center" border="0" cellpadding="0" cellspacing="0" swidth="100%" 
        width="690">
        <tr>
            <td align="middle" colspan="2">
                <h2 align="center">
                    Add New Vendor</h2>
            </td>
        </tr>
        <tr>
            <td align="middle" colspan="2">
                Fields marked with <b class="req"><span style="COLOR: red">*Field:</span></b> 
                are required.<br />
                <b>NOTE:</b> You must enter at least EITHER a website OR street address. 
                Entering both is fine. If you enter a street address, you must also enter city, 
                state, zip, and phone.
            </td>
        </tr>
    </table>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
    <br />
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        DataSourceID="sdsVendorConfirm" SkinID="noempty">
        <Columns>
            <asp:BoundField DataField="vendor_name" HeaderText="Vendor Name" 
                SortExpression="vendor_name" />
            <asp:BoundField DataField="address1" HeaderText="Address" 
                SortExpression="address1" />
            <asp:BoundField DataField="city" HeaderText="City" SortExpression="city" />
            <asp:BoundField DataField="state" HeaderText="State" SortExpression="state" />
            <asp:BoundField DataField="zip" HeaderText="Zip" SortExpression="zip" />
            <asp:BoundField DataField="phone" HeaderText="Phone" SortExpression="phone" />
            <asp:BoundField DataField="website" HeaderText="Web" SortExpression="website" />
        </Columns>
    </asp:GridView>
    <br />
    <asp:FormView ID="FormView1" runat="server" DataKeyNames="vendor_id" 
        DataSourceID="sdsVendor" DefaultMode="Insert">
        <InsertItemTemplate>
            <table border="1" width="100%">
                <tr>
                    <td style="color: red">
                        *Name:</td>
                    <td style="width: 165px">
                        <asp:TextBox ID="vendor_nameTextBox" runat="server" AutoPostBack="True" 
                            ontextchanged="vendor_nameTextBox_TextChanged" 
                            Text='<%# Bind("vendor_name") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                            ControlToValidate="vendor_nameTextBox" 
                            ErrorMessage="You must provide a name for this vendor">*</asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="CustomValidator3" runat="server" 
                            ControlToValidate="vendor_nameTextBox" 
                            ErrorMessage="Please verify that this vendor is not in the list below. If not, you may continue with adding this vendor." 
                            onservervalidate="CustomValidator3_ServerValidate"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td style="color: red">
                        *Address:</td>
                    <td style="width: 165px">
                        <asp:TextBox ID="address1TextBox" runat="server" Text='<%# Bind("address1") %>'>
                        </asp:TextBox>
                        <asp:CustomValidator ID="CustomValidator1" runat="server" 
                            ControlToValidate="vendor_nameTextBox" 
                            ErrorMessage="Must supply either website OR street address, city, state, zip, and phone" 
                            OnServerValidate="CustomValidator1_ServerValidate">*</asp:CustomValidator>
                        <asp:TextBox ID="address2TextBox" runat="server" Text='<%# Bind("address2") %>'>
                        </asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        City:</td>
                    <td style="width: 165px">
                        <asp:TextBox ID="cityTextBox" runat="server" Text='<%# Bind("city") %>'>
                        </asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        State:</td>
                    <td style="width: 165px">
                        <asp:TextBox ID="stateTextBox" runat="server" Text='<%# Bind("state") %>'>
                        </asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Zip/Country:</td>
                    <td style="width: 165px">
                        <asp:TextBox ID="zipTextBox" runat="server" Text='<%# Bind("zip") %>'>
                        </asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Phone:</td>
                    <td style="width: 165px">
                        <asp:TextBox ID="phoneTextBox" runat="server" Text='<%# Bind("phone") %>'>
                        </asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        Fax:</td>
                    <td style="width: 165px">
                        <asp:TextBox ID="faxTextBox" runat="server" Text='<%# Bind("fax") %>'>
                        </asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="color: red">
                        *Website:<br />
                        (Omit(Omit &quot;http://&quot;)
                        &quot;http://&quot;)
                    </td>
                    <td style="width: 165px">
                        <asp:TextBox ID="websiteTextBox" runat="server" Text='<%# Bind("website") %>'>
                        </asp:TextBox>
                        <asp:CustomValidator ID="CustomValidator2" runat="server" 
                            ControlToValidate="websiteTextBox" 
                            ErrorMessage="Website URL can not contail &quot;http://&quot; or &quot;https://&quot;" 
                            OnServerValidate="CustomValidator2_ServerValidate">*</asp:CustomValidator>
                    </td>
                </tr>
            </table>
            <br />
            <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" 
                CommandName="Insert" onclick="InsertButton_Click" Text="Insert">
            </asp:LinkButton>
            &nbsp;<asp:LinkButton ID="InsertCancelButton" runat="server" 
                CausesValidation="False" CommandName="Cancel" Text="Cancel">
            </asp:LinkButton>
            &nbsp;<asp:LinkButton ID="InsertButtonConfirm" runat="server" 
                CausesValidation="False" CommandName="Insert" onclick="InsertButton_Click" 
                Text="I confirm this is a new vendor" Visible="False"></asp:LinkButton>
        </InsertItemTemplate>
        <ItemTemplate>
        </ItemTemplate>
    </asp:FormView>
    <asp:SqlDataSource ID="sdsVendor" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        InsertCommand="insert_vendor" InsertCommandType="StoredProcedure" 
        ProviderName="<%$ ConnectionStrings:OPSConn.ProviderName %>">
        <InsertParameters>
            <asp:Parameter Name="vendor_name" Type="String" />
            <asp:Parameter Name="address1" Type="String" />
            <asp:Parameter Name="address2" Type="String" />
            <asp:Parameter Name="city" Type="String" />
            <asp:Parameter Name="state" Type="String" />
            <asp:Parameter Name="zip" Type="String" />
            <asp:Parameter Name="phone" Type="String" />
            <asp:Parameter Name="fax" Type="String" />
            <asp:Parameter Name="website" Type="String" />
        </InsertParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsVendorConfirm" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        SelectCommand="check_vendor_name" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="FormView1" Name="vendor_name" 
                PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
</asp:Content>

