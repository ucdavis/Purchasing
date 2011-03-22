<%@ Page Title="" Language="C#" MasterPageFile="~/master/default.master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="delegates.aspx.cs" Inherits="business_purchasing_orders_delegates" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" runat="Server" ContentPlaceHolderID="LeftSidebarContentPlaceHolder">
    <!-- ********** Left sidebar area of the page ********** -->
    <div id="left_sidebar">
        <div id="level2_nav">
            <a name="nav"></a>
            <h2>In this section</h2>
            <asp:Menu ID="Menu3" runat="server">
            </asp:Menu>
        </div>
        <div class="other_links">
            <h2>Tasks</h2>
            <ul>
                <li>
                    <asp:LinkButton ID="addLinkButton" runat="server" CausesValidation="false" OnClick="addLinkButton_Click">Add Delegate</asp:LinkButton></li>
            </ul>
        </div>
    </div>
    <!-- ********** End left sidebar area of the page ********** -->
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="MainContentPlaceHolder">
    <!-- ********** Main content area of the page ********** -->
    <div id="formwrap">
        <h1>
            <asp:Label ID="sectionTitleLabel" runat="server" Text="Delegates"></asp:Label></h1>
        <div class="fixfc">
            <!-- this is necessary to fix a problem with multiple right floats in IE 6 -->
        </div>
        <asp:MultiView ID="delegatesMultiView" runat="server">
            <asp:View ID="View_List" runat="server">
                <telerik:RadGrid ID="delegatesRadGrid" runat="server" AutoGenerateColumns="false" OnItemCommand="delegatesRadGrid_ItemCommand">
                    <MasterTableView>
                        <Columns>
                            <telerik:GridTemplateColumn>
                                <ItemTemplate>
                                    <asp:ImageButton ID="editImageButton" ToolTip="Edit Delegate" ImageUrl="~/images/10/edit.gif" CommandName="EditDelegate" CommandArgument='<%# Eval("id") %>' runat="server" />
                                    <asp:ImageButton ID="deleteImageButton" ToolTip="Delete Delegate" ImageUrl="~/images/10/delete.gif" OnClientClick='return confirm("Are you sure you want to delete this delegate");' CommandName="DeleteDelegate" CommandArgument='<%# Eval("id") %>' runat="server" />
                                </ItemTemplate>
                                <ItemStyle Width="25px" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn Visible="false" HeaderText="PI">
                                <ItemTemplate>
                                    <%#ParsePI(Eval("pi_profile_id")) %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Target User">
                                <ItemTemplate>
                                    <%#ParsePI(Eval("target_profile_id")) %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="account_name" HeaderText="Account Keywords">
                                <ItemTemplate>
                                    <%#ParseAccounts(Eval("account_name"))%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="unit" HeaderText="Rule">
                                <ItemTemplate>
                                    <%#ParseUnit(Eval("unit"))%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn DataField="amount" HeaderText="Amount">
                                <ItemTemplate>
                                    <%#ParseAmount(Eval("amount"))%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Date">
                                <ItemTemplate>
                                    <%# ParseEffectiveDate(Eval("start_dt"), Eval("end_dt")) %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </asp:View>
            <asp:View ID="View_AddEdit" runat="server">
                <asp:UpdatePanel ID="delegateUpdatePanel" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="adminPanel" runat="server">
                            <table class="formtable bdr" cellspacing="0">
                                <tr class="alt">
                                    <td style="width: 150px">
                                        <asp:Label ID="piLabel" runat="server" Text="PI:" /></td>
                                    <td>
                                        <telerik:RadComboBox ID="piRadComboBox" DataTextField="ListName" DataValueField="UserName" runat="server">
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                            </table>
                            <br />
                        </asp:Panel>
                        <table class="formtable bdr" cellspacing="0">
                            <tr class="alt">
                                <td style="width: 150px">Automatically approve:</td>
                                <td>
                                    <telerik:RadComboBox ID="targetUserRadComboBox" AutoPostBack="true" AppendDataBoundItems="true" DataTextField="ListName" DataValueField="UserName" runat="server" OnSelectedIndexChanged="targetUserRadComboBox_SelectedIndexChanged">
                                        <Items>
                                            <telerik:RadComboBoxItem Text="Select One..." Value="Select One..." />
                                        </Items>
                                    </telerik:RadComboBox>
                                    <asp:CompareValidator ID="targetUserCompareValidator" Operator="NotEqual" ValueToCompare="Select One..." ControlToValidate="targetUserRadComboBox" runat="server" ErrorMessage="*"></asp:CompareValidator>
                                </td>
                            </tr>
                        </table>
                        <h2>Optional Rules</h2>
                        <br />
                        <table class="formtable bdr" cellspacing="0">
                            <tr class="alt">
                                <td style="width: 150px">When the order is:</td>
                                <td>
                                    <telerik:RadComboBox ID="unitRadComboBox" AutoPostBack="true" runat="server" OnSelectedIndexChanged="unitRadComboBox_SelectedIndexChanged">
                                        <Items>
                                            <telerik:RadComboBoxItem Value="N/A" Text="N/A" />
                                            <telerik:RadComboBoxItem Value="<" Text="Less Than" />
                                            <telerik:RadComboBoxItem Value=">" Text="Greater Than" />
                                            <telerik:RadComboBoxItem Value="=" Text="Equal To" />
                                        </Items>
                                    </telerik:RadComboBox>
                                    &nbsp;
                                    <telerik:RadNumericTextBox ID="amountRadNumericTextBox" AutoPostBack="true" EnableEmbeddedSkins="false" Width="70px" Type="Currency" runat="server" OnTextChanged="amountRadNumericTextBox_TextChanged">
                                    </telerik:RadNumericTextBox>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <p>
                            <asp:RadioButton ID="andor1AndRadioButton" AutoPostBack="true" GroupName="andor1" Text=" AND" runat="server" />
                            <asp:RadioButton ID="andor1OrRadioButton" AutoPostBack="true" GroupName="andor1" Checked="true" Text=" OR" runat="server" OnCheckedChanged="andor1OrRadioButton_CheckedChanged" />
                        </p>
                        <table class="formtable bdr" cellspacing="0">
                            <tr class="alt">
                                <td style="width: 150px">When the choosen account contains or does not contain the following word(s) (seperated by comma):</td>
                                <td>
                                    <telerik:RadComboBox ID="containRadComboBox" AutoPostBack="true" runat="server" OnSelectedIndexChanged="containRadComboBox_SelectedIndexChanged">
                                        <Items>
                                            <telerik:RadComboBoxItem Value="True" Text="Contains" />
                                            <telerik:RadComboBoxItem Value="False" Text="Does Not Contain" />
                                        </Items>
                                    </telerik:RadComboBox>
                                    &nbsp;
                                    <asp:TextBox ID="accountTextBox" MaxLength="256" AutoPostBack="true" Width="200px" runat="server" OnTextChanged="accountTextBox_TextChanged"></asp:TextBox></td>
                            </tr>
                        </table>
                        <br />
                        <p>
                            <asp:RadioButton ID="andor2AndRadioButton" AutoPostBack="true" GroupName="andor2" Text=" AND" runat="server" />
                            <asp:RadioButton ID="andor2OrRadioButton" AutoPostBack="true" GroupName="andor2" Checked="true" Text=" OR" runat="server" OnCheckedChanged="andor2OrRadioButton_CheckedChanged" />
                        </p>
                        <table class="formtable bdr" cellspacing="0">
                            <tr class="alt">
                                <td style="width: 150px">Effective for the following period:</td>
                                <td>
                                    <asp:TextBox ID="fromTextBox" runat="server" AutoPostBack="true" OnTextChanged="fromTextBox_TextChanged"></asp:TextBox>&nbsp;Through:&nbsp;<asp:TextBox ID="toTextBox" runat="server" AutoPostBack="true" OnTextChanged="toTextBox_TextChanged"></asp:TextBox>
                                    <ajaxToolkit:CalendarExtender ID="fromCalendarExtender" TargetControlID="fromTextBox" runat="server">
                                    </ajaxToolkit:CalendarExtender>
                                    <ajaxToolkit:CalendarExtender ID="toCalendarExtender" TargetControlID="toTextBox" runat="server">
                                    </ajaxToolkit:CalendarExtender>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <asp:Panel ID="samplePanel" Visible="false" runat="server">
                            <h2>Sample Delegation</h2>
                            <br />
                            <table class="formtable bdr" cellspacing="0">
                                <tr class="alt">
                                    <td>
                                        <asp:Label ID="sampleLabel" ForeColor="Green" runat="server" /></td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="tableButtons">
                    <asp:Button ID="editUpdateButton" runat="server" Text=" Update " OnClick="editUpdateButton_Click" />
                    <asp:Button ID="editCancelButton" runat="server" CausesValidation="false" Text=" Cancel " OnClick="editCancelButton_Click" />
                </div>
            </asp:View>
            <asp:View ID="View_Confirm" runat="server">
                <asp:Label ID="confirmLabel" runat="server" />
            </asp:View>
        </asp:MultiView>
    </div>
    <!-- ********** End main content area of the page ********** -->
</asp:Content>
