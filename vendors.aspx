<%@ Page Language="VB" MasterPageFile="~/ops.master" AutoEventWireup="false" CodeFile="vendors.aspx.vb" Inherits="vendors" title="" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:SqlDataSource ID="sdsVendors" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        SelectCommand="select_vendors" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlAlpha" Name="search" 
                PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="sdsEditVendor" runat="server" 
        ConnectionString="<%$ ConnectionStrings:OPSConn %>" 
        SelectCommand="select_one_vendor" SelectCommandType="StoredProcedure" 
        UpdateCommand="update_vendor" UpdateCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter ControlID="GridView1" Name="vendor_id" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="vendor_id" Type="Int32" />
            <asp:Parameter Name="vendor_name" Type="String" />
            <asp:Parameter Name="address1" Type="String" />
            <asp:Parameter Name="address2" Type="String" />
            <asp:Parameter Name="city" Type="String" />
            <asp:Parameter Name="state" Type="String" />
            <asp:Parameter Name="zip" Type="String" />
            <asp:Parameter Name="phone" Type="String" />
            <asp:Parameter Name="fax" Type="String" />
            <asp:Parameter Name="website" Type="String" />
            <asp:Parameter Name="vendor_comments" Type="String" />
            <asp:Parameter Name="cust_id" Type="String" />
            <asp:Parameter Name="current" Type="Boolean" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <asp:XmlDataSource ID="xmlAlpha" runat="server" DataFile="~/ddlElements.xml" 
        XPath="root/alpha/value"></asp:XmlDataSource>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:FormView ID="FormView1" runat="server" BorderColor="#91AF6F" 
        BorderStyle="None" BorderWidth="2px" DataKeyNames="vendor_id" 
        DataSourceID="sdsEditVendor" DefaultMode="Edit" Width="50%">
                <EditItemTemplate>
                    <table>
                        <tr>
                            <td style="width: 3px; height: 26px">
                                Name:
                            </td>
                            <td style="height: 26px">
                                <asp:TextBox ID="vendor_nameTextBox" runat="server" 
                            Text='<%# Bind("vendor_name") %>'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 3px">
                                Address:</td>
                            <td>
                                <asp:TextBox ID="address1TextBox" runat="server" Text='<%# Bind("address1") %>'></asp:TextBox>
                                <br />
                                <asp:TextBox ID="address2TextBox" runat="server" Text='<%# Bind("address2") %>'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 3px">
                                City:</td>
                            <td>
                                <asp:TextBox ID="cityTextBox" runat="server" Text='<%# Bind("city") %>'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 3px">
                                State:</td>
                            <td>
                                <asp:TextBox ID="stateTextBox" runat="server" Text='<%# Bind("state") %>'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 3px">
                                Zip:</td>
                            <td>
                                <asp:TextBox ID="zipTextBox" runat="server" Text='<%# Bind("zip") %>'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 3px">
                                Phone</td>
                            <td>
                                <asp:TextBox ID="phoneTextBox" runat="server" Text='<%# Bind("phone") %>'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 3px">
                                Fax</td>
                            <td>
                                <asp:TextBox ID="faxTextBox" runat="server" Text='<%# Bind("fax") %>'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 3px">
                                Website</td>
                            <td>
                                <asp:TextBox ID="websiteTextBox" runat="server" Text='<%# Bind("website") %>'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 3px">
                                Comments:
                            </td>
                            <td>
                                <asp:TextBox ID="vendor_commentsTextBox" runat="server" 
                            Text='<%# Bind("vendor_comments") %>'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Customer id:</td>
                            <td>
                                <asp:TextBox ID="cust_idTextBox" runat="server" Text='<%# Bind("cust_id") %>'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 3px">
                                Current?:
                            </td>
                            <td>
                                <asp:CheckBox ID="currentCheckBox" runat="server" 
                            Checked='<%# Bind("current") %>' />
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    &nbsp;<asp:Button ID="btnUpdate" runat="server" CommandName="Update" Text="Update" />
                    &nbsp;
                    <asp:Button ID="btnCancel" runat="server" CommandName="Cancel" 
                        Text="Cancel Selection" />
                    <br />
                    <br />
                </EditItemTemplate>
                <InsertItemTemplate>
                    vendor_name:
                    <asp:TextBox ID="vendor_nameTextBox0" runat="server" 
                Text='<%# Bind("vendor_name") %>'></asp:TextBox>
                    <br />
                    address1:
                    <asp:TextBox ID="address1TextBox0" runat="server" 
                Text='<%# Bind("address1") %>'></asp:TextBox>
                    <br />
                    address2:
                    <asp:TextBox ID="address2TextBox0" runat="server" 
                Text='<%# Bind("address2") %>'></asp:TextBox>
                    <br />
                    city:
                    <asp:TextBox ID="cityTextBox0" runat="server" Text='<%# Bind("city") %>'></asp:TextBox>
                    <br />
                    state:
                    <asp:TextBox ID="stateTextBox0" runat="server" Text='<%# Bind("state") %>'></asp:TextBox>
                    <br />
                    zip:
                    <asp:TextBox ID="zipTextBox0" runat="server" Text='<%# Bind("zip") %>'></asp:TextBox>
                    <br />
                    phone:
                    <asp:TextBox ID="phoneTextBox0" runat="server" Text='<%# Bind("phone") %>'></asp:TextBox>
                    <br />
                    fax:
                    <asp:TextBox ID="faxTextBox0" runat="server" Text='<%# Bind("fax") %>'></asp:TextBox>
                    <br />
                    website:
                    <asp:TextBox ID="websiteTextBox0" runat="server" Text='<%# Bind("website") %>'></asp:TextBox>
                    <br />
                    vendor_comments:
                    <asp:TextBox ID="vendor_commentsTextBox0" runat="server" 
                Text='<%# Bind("vendor_comments") %>'></asp:TextBox>
                    <br />
                    cust_id:
                    <asp:TextBox ID="cust_idTextBox0" runat="server" Text='<%# Bind("cust_id") %>'></asp:TextBox>
                    <br />
                    current:
                    <asp:CheckBox ID="currentCheckBox0" runat="server" 
                Checked='<%# Bind("current") %>' />
                    <br />
                    vend_nbr:
                    <asp:TextBox ID="vend_nbrTextBox" runat="server" Text='<%# Bind("vend_nbr") %>'></asp:TextBox>
                    <br />
                    vend_addr_typ_cd:
                    <asp:TextBox ID="vend_addr_typ_cdTextBox" runat="server" 
                Text='<%# Bind("vend_addr_typ_cd") %>'></asp:TextBox>
                    <br />
                    purchaser:
                    <asp:TextBox ID="purchaserTextBox" runat="server" 
                Text='<%# Bind("purchaser") %>'></asp:TextBox>
                    <br />
                    <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" 
                CommandName="Insert" Text="Insert"></asp:LinkButton>
                    <asp:LinkButton ID="InsertCancelButton" runat="server" CausesValidation="False" 
                CommandName="Cancel" Text="Cancel"></asp:LinkButton>
                </InsertItemTemplate>
                <ItemTemplate>
                    vendor_id:
                    <asp:Label ID="vendor_idLabel" runat="server" Text='<%# Eval("vendor_id") %>'></asp:Label>
                    <br />
                    vendor_name:
                    <asp:Label ID="vendor_nameLabel" runat="server" 
                Text='<%# Bind("vendor_name") %>'></asp:Label>
                    <br />
                    address1:
                    <asp:Label ID="address1Label" runat="server" Text='<%# Bind("address1") %>'></asp:Label>
                    <br />
                    address2:
                    <asp:Label ID="address2Label" runat="server" Text='<%# Bind("address2") %>'></asp:Label>
                    <br />
                    city:
                    <asp:Label ID="cityLabel" runat="server" Text='<%# Bind("city") %>'></asp:Label>
                    <br />
                    state:
                    <asp:Label ID="stateLabel" runat="server" Text='<%# Bind("state") %>'></asp:Label>
                    <br />
                    zip:
                    <asp:Label ID="zipLabel" runat="server" Text='<%# Bind("zip") %>'></asp:Label>
                    <br />
                    phone:
                    <asp:Label ID="phoneLabel" runat="server" Text='<%# Bind("phone") %>'></asp:Label>
                    <br />
                    fax:
                    <asp:Label ID="faxLabel" runat="server" Text='<%# Bind("fax") %>'></asp:Label>
                    <br />
                    website:
                    <asp:Label ID="websiteLabel" runat="server" Text='<%# Bind("website") %>'></asp:Label>
                    <br />
                    vendor_comments:
                    <asp:Label ID="vendor_commentsLabel" runat="server" 
                Text='<%# Bind("vendor_comments") %>'></asp:Label>
                    <br />
                    cust_id:
                    <asp:Label ID="cust_idLabel" runat="server" Text='<%# Bind("cust_id") %>'></asp:Label>
                    <br />
                    current:
                    <asp:CheckBox ID="currentCheckBox1" runat="server" 
                Checked='<%# Bind("current") %>' Enabled="false" />
                    <br />
                    vend_nbr:
                    <asp:Label ID="vend_nbrLabel" runat="server" Text='<%# Bind("vend_nbr") %>'></asp:Label>
                    <br />
                    vend_addr_typ_cd:
                    <asp:Label ID="vend_addr_typ_cdLabel" runat="server" 
                Text='<%# Bind("vend_addr_typ_cd") %>'></asp:Label>
                    <br />
                    purchaser:
                    <asp:Label ID="purchaserLabel" runat="server" Text='<%# Bind("purchaser") %>'></asp:Label>
                    <br />
                </ItemTemplate>
            </asp:FormView>
            <asp:DropDownList ID="ddlAlpha" runat="server" AutoPostBack="True" 
        CssClass="noprint" DataSourceID="xmlAlpha" DataTextField="label" 
        DataValueField="label">
            </asp:DropDownList>
            &nbsp; <a href="add_vendor.aspx">Add new Vendor</a>
            <asp:GridView ID="GridView1" runat="server" AllowSorting="True" 
        AutoGenerateColumns="False" CssClass="noprint" DataKeyNames="vendor_id" 
        DataSourceID="sdsVendors" PageSize="30">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="vendor_name" HeaderText="Vendor" 
                SortExpression="vendor_name">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="city" HeaderText="City" SortExpression="city">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="state" HeaderText="State" SortExpression="state">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="zip" HeaderText="Zip" SortExpression="zip">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="phone" HeaderText="Phone" SortExpression="phone">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="fax" HeaderText="Fax" SortExpression="fax">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:CheckBoxField DataField="current" HeaderText="Current?" 
                SortExpression="current" />
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

