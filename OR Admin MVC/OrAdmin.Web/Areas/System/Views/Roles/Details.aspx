<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Default.Master" Inherits="System.Web.Mvc.ViewPage<OrAdmin.Web.Areas.System.Models.Roles.RoleViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Title" runat="server">
    Role:
    <%: Model.Role %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="topRight">
        <%: Html.SpriteCtrl("Back to list of roles", HtmlHelpers.CtrlType.Link, "arrow-curve-180-left", null, "index") %>
    </div>
    <h1>Role:
        <%: Model.Role %></h1>
    <p class="desc">Below is a list of users in the
        <%: Model.Role %>
        role. Click on a name to view the user's profile or click "Remove" to remove the user from this role.</p>
    <h2>Users in role</h2>
    <% if (Model.Users.Any())
       { %>
    <% Html.Telerik().Grid(Model.Users)
               .Name("grid")
               .Columns(column =>
               {
                   column.Template(user =>
                   { %>
    <% var profile = User.Profile(user.UserName); %>
    <%: Html.ActionLink(profile.LastName + ", " + profile.FirstName, "details", "membership", new { id = user.ProviderUserKey }, null) %>
    <% }).Title("Name").HtmlAttributes(new { @style = "width: 180px;" });
                   column.Bound(user => user.UserName).Title("Username");
                   column.Template(user =>
                   { %>
    <% var profile = User.Profile(user.UserName); %>
    <% using (Html.BeginForm("RemoveFromRole", "membership", new { id = user.ProviderUserKey, role = Model.Role }))
       { %>
    <%= Html.SpriteCtrl("Remove", HtmlHelpers.CtrlType.Link, "cross", "Remove " + profile.FirstName + " " + profile.LastName + " from " + Model.Role)%>
    <% } %>
    <% }).HtmlAttributes(new { @style = "width: 100px; text-align: center;" }).HeaderHtmlAttributes(new { @style = "text-align: center" });
               })
               .Pageable(pager => pager.PageSize(10).Style(GridPagerStyles.NextPreviousAndNumeric))
               .Filterable()
               .Sortable()
               .Render();
    %>
    <% }
       else
       { %>
    There are no users are in this role.
    <% } %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="QuickLinks" runat="server">
</asp:Content>
