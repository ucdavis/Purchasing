<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Default.Master" Inherits="System.Web.Mvc.ViewPage<OrAdmin.Web.Areas.System.Models.Roles.IndexViewModel>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Title" runat="server">
    System Roles
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>System Roles</h1>
    <% if (Model.Roles.Any())
       { %>
    <% Html.Telerik().Grid(Model.Roles)
               .Name("grid")
               .Columns(column =>
               {
                   column.Template(role =>
                   { %>
    <%= Html.SpriteCtrl("Details", HtmlHelpers.CtrlType.Link, "document--pencil", "View this role", null, "details", "roles", new { id = role.RoleName })%>
    <% }).HtmlAttributes(new { @style = "text-align: center; width: 80px;" });

                   column.Bound(role => role.RoleName).Template(role =>
                                         { 
    %>
    <%: Html.ActionLink(role.RoleName, "details", new { id = role.RoleName }) %>
    <%
        }).Title("Role Name");

                   column.Template(role =>
                                 { %>
    <% using (Html.BeginForm("DeleteRole", "roles", new { id = role.RoleName }))
       { %>
    <%= Html.SpriteCtrl("Delete", HtmlHelpers.CtrlType.Link, "bin") %>
    <% } %>
    <% }).HtmlAttributes(new { @style = "text-align: center; width: 80px;" }); ;
               })
               .Pageable(pager => pager.PageSize(10).Style(GridPagerStyles.NextPreviousAndNumeric))
               .Sortable()
               .Filterable()
               .Render();
    %>
    <% }
       else
       { %>
    <p>No roles have been created.</p>
    <% } %>
    <br />
    <% using (Html.BeginForm("CreateRole", "roles"))
       { %>
    <h2>Create a new role</h2>
    <fieldset>
        <label for="id">
            Role name:
        </label>
        <%= Html.TextBox("id") %>
    </fieldset>
    <br />
    <%: Html.SpriteCtrl("Create role", HtmlHelpers.CtrlType.Button, "tick") %>
    <% } %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="QuickLinks" runat="server">
</asp:Content>
