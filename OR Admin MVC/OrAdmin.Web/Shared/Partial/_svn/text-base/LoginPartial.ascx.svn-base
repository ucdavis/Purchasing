<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="login-wrap">
    <% if (Request.IsAuthenticated)
       { %>
    Logged in as:
    <%: Html.ActionLink(Page.User.Identity.Name, "index", new { controller = "profile", area = "system" }, new { @class = "profile" })%>
    | <a class="login" href="<%= OrAdmin.Core.Settings.GlobalSettings.LogoutUrl %>" title="Logout of <%: OrAdmin.Core.Settings.GlobalSettings.SystemTitle %>">Logout</a>
    <% }
       else
       { %>
    <%= Html.ActionLink("Login", "overview", new { controller = "home", area = "home" }, new { @class = "login", @title="Log in to " + OrAdmin.Core.Settings.GlobalSettings.SystemTitle.Encode() })%>
    <% } %>
</div>
