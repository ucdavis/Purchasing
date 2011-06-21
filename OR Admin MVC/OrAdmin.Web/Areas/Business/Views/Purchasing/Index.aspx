<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Default.Master" Inherits="System.Web.Mvc.ViewPage<OrAdmin.Web.Areas.Business.Models.Purchasing.IndexViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Title" runat="server">
    Purchase Request System
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        ._33 { width: 33.33%; float: left; }
        .tbl ul li { padding: 2px 0; }
    </style>
    <script type="text/javascript">
    <!--
        $(function () {
            $('._33 table td').eqHeight({ 'height': 'outer' });
        });
    //-->
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Purchasing</h1>
    <p class="desc">An overview of your recent activity.</p>
    <table class="tbl gray" cellspacing="0">
        <tr>
            <th>
                What's new
            </th>
        </tr>
        <tr>
            <td class="totals">
                <span style="font-style: italic">Nothing new to report</span>
            </td>
        </tr>
    </table>
    <div class="_33">
        <div style="padding-right: .5em;">
            <table class="tbl gray" cellspacing="0">
                <tr>
                    <th>
                        Requests
                    </th>
                </tr>
                <tr>
                    <td>
                        <% if (Model.Requests.Any())
                           { %>
                        <ul>
                            <% foreach (OrAdmin.Entities.Purchasing.Request request in Model.Requests)
                               { %>
                            <li>
                                <%: request.SubmittedOn.ToString("M-d-yy") %>:
                                <%: Html.ActionLink(request.Vendor.FriendlyName, "myrequestdetails", new { requestUniqueId = request.UniqueId })%></li>
                            <% } %>
                        </ul>
                        <% }
                           else
                           { %>
                        <span style="font-style: italic">No requests to display</span>
                        <% } %>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="_33">
        <div style="padding: 0 .5em;">
            <table class="tbl gray" cellspacing="0">
                <tr>
                    <th>
                        Activity
                    </th>
                </tr>
                <tr>
                    <td>
                        <% if (Model.HistoryItems.Any())
                           { %>
                        <ul>
                            <% foreach (OrAdmin.Entities.Purchasing.HistoryItem historyItem in Model.HistoryItems)
                               { %>
                            <li>
                                <img src="<%= Url.Content("~/Content/Img/Fugue/"+ historyItem.ImageName +".png") %>" style="vertical-align: middle;" title="<%= historyItem.Description %>" alt="<%= historyItem.Description %>" />
                                <%: historyItem.SubmittedOn.ToString("M-d-yy") %>: <a href="<%: Url.Action("myrequestdetails", new { requestUniqueId = historyItem.RequestUniqueId }) %>#history">
                                    <%: historyItem.Description %>
                                    by
                                    <%: historyItem.SubmittedBy %></a></li>
                            <% } %>
                        </ul>
                        <% }
                           else
                           { %>
                        <span style="font-style: italic">No notes to display</span>
                        <% } %>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="_33">
        <div style="padding-left: .5em;">
            <table class="tbl gray" cellspacing="0">
                <tr>
                    <th>
                        Comments
                    </th>
                </tr>
                <tr>
                    <td>
                        <% if (Model.RequestComments.Any())
                           { %>
                        <ul>
                            <% foreach (var comment in Model.RequestComments)
                               { %>
                            <li>
                                <%: comment.SubmittedOn.ToString("M-d-yy")%>: <a href="<%: Url.Action("myrequestdetails", new { requestUniqueId = comment.Request.UniqueId }) %>#comments">
                                    <%: comment.Comments %></a> </li>
                            <% } %>
                        </ul>
                        <% }
                           else
                           { %>
                        <span style="font-style: italic">No notes to display</span>
                        <% } %>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <br class="clear" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="QuickLinks" runat="server">
</asp:Content>
