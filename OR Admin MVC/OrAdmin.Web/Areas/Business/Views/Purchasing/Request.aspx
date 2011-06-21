<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Default.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Title" runat="server">
    Place a Request
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        .tbl { border-top: 1px solid #989898; border-left: 1px solid #989898; }
        .tbl tr td { border-bottom: 1px solid #989898; border-right: 1px solid #989898; }
        .tbl tr td div { width: 170px; margin: 0px auto; }
        .tbl tr.smaller td { padding: 15px; font-size: 10px; line-height: 13px; }
        .tbl tr.alt td { text-align: center; width: 33.33%; background-color: #F5F5F5; padding: 10px 15px; font-size: 11px; border-top: 1px solid #989898; border-bottom: 1px solid #989898; }
        .tbl tr.alt td a { text-decoration: none; }
        .tbl tr.alt td a:hover { text-decoration: underline; }
    </style>
    <script type="text/javascript">
    <!--
        // Define embedded scripts here
    //-->
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Place a Request</h1>
    <p class="desc">Select the type of request you would like to make.</p>
    <table class="tbl" cellspacing="0">
        <tr>
            <td>
                <div>
                    <a href="<%: Url.Action("dpo") %>">
                        <img id="dbo" src="<%= Url.Content("~/Content/Img/Common/dpo.jpg") %>" title="New Purchase Request or Requisition" alt="New Purchase Order or Requisition" /></a></div>
            </td>
            <td>
                <div>
                    <a href="<%: Url.Action("dro") %>">
                        <img id="dro" src="<%= Url.Content("~/Content/Img/Common/dro.jpg") %>" title="New Repair Request" alt="New Repair Request" /></a></div>
            </td>
            <td>
                <div>
                    <a href="<%: Url.Action("ba") %>">
                        <img id="ba" src="<%= Url.Content("~/Content/Img/Common/ba.jpg") %>" title="New Business Agreement" alt="New Business Agreement" /></a></div>
            </td>
        </tr>
        <tr class="alt">
            <td>
                <%: Html.ActionLink("Purchase Request or Requisition (DPO)", "dpo") %>
            </td>
            <td>
                <%: Html.ActionLink("Repair Request (DRO)", "dro") %>
            </td>
            <td>
                <%: Html.ActionLink("Business Agreement", "ba")%>
            </td>
        </tr>
        <tr class="smaller">
            <td>
                <p><strong>Used For:</strong> Supplies</p>
                <p><strong>Limitations:</strong> Orders under $5000 per vendor/day can be placed by the department. Orders exceeding this limit will be submitted through campus purchasing and will take longer to process.</p>
            </td>
            <td>
                <p><strong>Used For:</strong> Procuring equipment repair services.</p>
                <p><strong>Limitations:</strong> Orders under $5000 per vendor/day can be placed by the department. Orders exceeding this limit will be submitted through campus purchasing and will take longer to process.</p>
            </td>
            <td>
                <p><strong>Used For:</strong> Procuring one-time or ongoing services based on an agreement or contract.</p>
                <p><strong>Limitations:</strong> Orders will be submitted to campus purchasing and require extra time to process.</p>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="QuickLinks" runat="server">
</asp:Content>
