<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Default.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Title" runat="server">
    Login
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        table { width: 100%; }
        table tr td { width: 33%; padding: 10px; text-align: center; }
        table tr td p { line-height: 1.3em; }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Login to
        <%: OrAdmin.Core.Settings.GlobalSettings.SystemTitle %></h1>
    <% if (User.Identity.IsAuthenticated)
       { %>
    <p class="desc">You are already logged in as: <strong>
        <%: User.Identity.Name %></strong>.
        <%: Html.ActionLink("Click here to logout", "logout") %></p>
    <% }
       else
       { %>
    <p class="desc">Please select your authentication method below or
        <%: Html.ActionLink("Register", "register", new { ReturnUrl = Request.QueryString["ReturnUrl"] }) %>
        if you don't have an account.</p>
    <table cellspacing="0">
        <tr>
            <td>
                <% using (Html.BeginForm())
                   { %>
                <%: Html.ValidationSummary(true, "Login was unsuccessful. Please correct the errors and try again.") %>
                <input type="image" src="/Content/Img/Common/ucd-login.png" alt="UC Davis Login" />
                <% } %>
            </td>
            <td>
                <a href="<%: Url.Action("publiclogon", new { ReturnUrl = Request.QueryString["ReturnUrl"] }) %>">
                    <img src="/Content/Img/Common/public-login.png" style="border: 0 none;" alt="Public Login" /></a>
            </td>
            <td>
                <a href="<%: Url.Action("register", new { ReturnUrl = Request.QueryString["ReturnUrl"] }) %>">
                    <img src="/Content/Img/Common/register.png" style="border: 0 none;" alt="Register" /></a>
            </td>
        </tr>
        <tr>
            <td>
                <p><strong>UC Davis Login</strong><br />
                    For UC Davis clients</p>
            </td>
            <td>
                <p><strong>Public Login</strong><br />
                    For Non-profit, UC System, For-profit (corporate), and other academic clients</p>
            </td>
            <td>
                <p><strong>Register</strong><br />
                    If you don't have an account</p>
            </td>
        </tr>
    </table>
    <% } %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="QuickLinks" runat="server">
</asp:Content>
