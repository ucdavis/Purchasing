<%@ Page Title="" Language="C#" MasterPageFile="~/master/default.master" AutoEventWireup="true" CodeFile="type.aspx.cs" Inherits="business_purchasing_orders_type" %>

<asp:Content ID="Content1" runat="Server" ContentPlaceHolderID="LeftSidebarContentPlaceHolder">
    <!-- ********** Left sidebar area of the page ********** -->
    <div id="left_sidebar">
        <div id="level2_nav">
            <a name="nav"></a>
            <h2>In this section</h2>
            <asp:Menu ID="Menu3" runat="server">
            </asp:Menu>
        </div>
    </div>
    <!-- ********** End left sidebar area of the page ********** -->
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="MainContentPlaceHolder">
    <!-- ********** Main content area of the page ********** -->
    <div id="formwrap">
        <h1>
            <asp:Label ID="sectionTitleLabel" runat="server" /></h1>
        <table class="formtable bdr" cellspacing="0">
            <tr>
                <td>
                    <div style="width: 170px; margin: 0px auto;">
                        <asp:ImageButton ID="dpoImageButton" CommandArgument="dpo.aspx" OnClick="ImageButton_Click" ImageUrl="~/images/photos/dpo.jpg" AlternateText="" ToolTip="New Purchase Order or Requisition" runat="server" /></div>
                </td>
                <td>
                    <div style="width: 170px; margin: 0px auto;">
                        <asp:ImageButton ID="agreementImageButton" CommandArgument="ba.aspx" OnClick="ImageButton_Click" ImageUrl="~/images/photos/ba.jpg" AlternateText="" ToolTip="New Business Agreement" runat="server" /></div>
                </td>
                <td>
                    <div style="width: 170px; margin: 0px auto;">
                        <asp:ImageButton ID="repairImageButton" CommandArgument="dro.aspx" OnClick="ImageButton_Click" ImageUrl="~/images/photos/repair.jpg" AlternateText="" ToolTip="New Repair Order (DRO)" runat="server" /></div>
                </td>
            </tr>
            <tr class="alt">
                <td style="width: 33%; text-align: center;">
                    <asp:HyperLink ID="dpoHyperLink" runat="server" Text="Purchase Orders &amp; Requisitions" NavigateUrl="dpo.aspx" /></td>
                <td style="width: 33%; text-align: center;">
                    <asp:HyperLink ID="agreementHyperLink" runat="server" Text="Business Agreement" NavigateUrl="ba.aspx" /></td>
                <td style="width: 33%; text-align: center;">
                    <asp:HyperLink ID="droHyperLink" runat="server" Text="Repair Order (DRO)" NavigateUrl="dro.aspx" /></td>
            </tr>
            <tr class="smaller">
                <td style="padding: 10px !important; vertical-align: top;">
                    <p><strong>Used For:</strong> Supplies</p>
                    <p><strong>Limitations:</strong> Orders under $5000 per vendor/day can be placed by the department. Orders exceeding this limit will be submitted through campus purchasing and will take longer to process.</p>
                </td>
                <td style="padding: 10px !important; vertical-align: top;">
                    <p><strong>Used For:</strong> Procuring one-time or ongoing services based on an agreement or contract.</p>
                    <p><strong>Limitations:</strong> Orders will be submitted to campus purchasing and require extra time to process.</p>
                </td>
                <td style="padding: 10px !important; vertical-align: top;">
                    <p><strong>Used For:</strong> Procuring equipment repair services.</p>
                    <p><strong>Limitations:</strong> Orders under $5000 per vendor/day can be placed by the department. Orders exceeding this limit will be submitted through campus purchasing and will take longer to process.</p>
                </td>
            </tr>
        </table>
    </div>
    <!-- ********** End main content area of the page ********** -->
</asp:Content>
