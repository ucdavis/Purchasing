<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Default.Master" Inherits="System.Web.Mvc.ViewPage<OrAdmin.Web.Areas.System.Models.Membership.IndexViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Title" runat="server">
    Membership Users
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        span.isOnline { color: green; }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="topRight">
        <label for="search">
            Search for:</label>
        <%: Html.TextBox("search") %>
    </div>
    <h1>Membership Administration</h1>
    <p class="desc">A listing of system users for all "oradmin" applications.</p>
    <h4>System Users</h4>
    <% if (Model.Users.Any())
       { %>
    <% Html.Telerik().Grid(Model.Users)
               .Name("grid")
               .Columns(columns =>
               {
                   columns.Template(user =>
                   { %>
    <%= Html.ActionLink(User.Profile(user.UserName).LastName + ", " + User.Profile(user.UserName).FirstName, "details", new { id = user.ProviderUserKey })%>
    <%
        }).Title("Name");
                   columns.Bound(user => user.UserName).Title("User Name");
                   columns.Bound(user => user.Email).Format("<a href=\"mailto:{0}\">{0}</a>").Encoded(false);
                   columns.Bound(user => user.LastActivityDate).Template(user =>
                   { %>
    <% if (user.IsOnline)
       { %>
    <span class="isOnline">Online</span>
    <% }
       else
       { %>
    <span class="isOffline">Offline for
        <%
            var offlineSince = (DateTime.Now - user.LastActivityDate);
            if (offlineSince.TotalSeconds <= 60) Response.Write("1 minute.");
            else if (offlineSince.TotalMinutes < 60) Response.Write(Math.Floor(offlineSince.TotalMinutes) + " minutes.");
            else if (offlineSince.TotalMinutes < 120) Response.Write("1 hour.");
            else if (offlineSince.TotalHours < 24) Response.Write(Math.Floor(offlineSince.TotalHours) + " hours.");
            else if (offlineSince.TotalHours < 48) Response.Write("1 day.");
            else Response.Write(Math.Floor(offlineSince.TotalDays) + " days.");
        %>
    </span>
    <% } %>
    <% }).Title("Status");
                   columns.Bound(user => user.IsApproved).Width(100).HeaderHtmlAttributes(new { style = "text-align: center" }).HtmlAttributes(new { style = "text-align: center" });
                   columns.Bound(user => user.Comment);
               })
               .Pageable(pager => pager.PageSize(10).Style(GridPagerStyles.NextPreviousAndNumeric))
               .Sortable()
               .Filterable()
               .Resizable(a => a.Columns(true))
               .Render();
    %>
    <% }
       else
       { %>
    <p>No users have registered.</p>
    <% } %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="QuickLinks" runat="server">
</asp:Content>
