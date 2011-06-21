<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Default.Master" Inherits="System.Web.Mvc.ViewPage<OrAdmin.Web.Areas.System.Models.Membership.DetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Title" runat="server">
    User Details:
    <%: Model.DisplayName %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        .pad { padding-bottom: 5px; }
        .pad li { padding-bottom: 10px; }
        .lbl { width: 100px; }
        ._50 { float: left; width: 50%; }
    </style>
    <script type="text/javascript">
    <!--
        $(function () {
            $('.jHeight').eqHeight();
        });
    //-->
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="topRight">
        <%: Html.SpriteCtrl("Back to list of users", HtmlHelpers.CtrlType.Link, "arrow-curve-180-left", null, null, "index", null, null, HtmlHelpers.ImagePosition.Left, null, true) %>
    </div>
    <h1>User Details:
        <%: Model.DisplayName %>
        [<%= Model.Status %>]</h1>
    <fieldset>
        <legend>Roles</legend>
        <table class="tbl simple" cellspacing="0">
            <% foreach (var role in Model.Roles)
               { %>
            <tr>
                <td>
                    <%: Html.ActionLink(role.Key, "details", new { controller= "roles", id = role.Key }) %>
                </td>
                <td style="width: 150px;">
                    <% if (role.Value)
                       { %>
                    <% using (Html.BeginForm("RemoveFromRole", "membership", new { id = Model.User.ProviderUserKey, role = role.Key }))
                       { %>
                    <%= Html.SpriteCtrl("Remove from role", HtmlHelpers.CtrlType.Link, "cross", "Remove from this role") %>
                    <% } %>
                    <% }
                       else
                       { %>
                    <% using (Html.BeginForm("AddToRole", "membership", new { id = Model.User.ProviderUserKey, role = role.Key }))
                       { %>
                    <%= Html.SpriteCtrl("Add to this role", HtmlHelpers.CtrlType.Link, "plus", "Add to this role") %>
                    <% } %>
                    <% } %>
                </td>
            </tr>
            <% } %>
        </table>
    </fieldset>
    <br />
    <div class="_50">
        <fieldset style="margin-right: 15px;" class="jHeight">
            <legend>Account</legend>
            <ul class="pad">
                <li><strong>User Name:</strong>
                    <%: Model.User.UserName %></li>
                <% if (Model.User.LastActivityDate == Model.User.CreationDate)
                   { %>
                <li><strong>Last Active:</strong> <em>Never</em></li>
                <li><strong>Last Login:</strong> <em>Never</em></li>
                <% }
                   else
                   { %>
                <li><strong>Last Active:</strong>
                    <%= Model.User.LastActivityDate.ToString("MMMM dd, yyyy h:mm tt") %></li>
                <li><strong>Last Login:</strong>
                    <%= Model.User.LastLoginDate.ToString("MMMM dd, yyyy h:mm tt") %></li>
                <li><strong>Created:</strong>
                    <%= Model.User.CreationDate.ToString("MMMM dd, yyyy h:mm tt") %></li>
                <% } %>
            </ul>
        </fieldset>
        <br />
        <% using (Html.BeginForm("ChangeApproval", "membership", new { id = Model.User.ProviderUserKey }))
           { %>
        <%= Html.Hidden("isApproved", !Model.User.IsApproved) %>
        <%= Html.SpriteCtrl((Model.User.IsApproved ? "Un-approve" : "Approve"), HtmlHelpers.CtrlType.Button, (Model.User.IsApproved ? "thumb" : "thumb-up"))%>
        <% } %>
        <% using (Html.BeginForm("DeleteUser", "membership", new { id = Model.User.ProviderUserKey }))
           { %>
        <%= Html.SpriteCtrl("Delete account", HtmlHelpers.CtrlType.Button, "cross") %>
        <% } %>
    </div>
    <div class="_50">
        <% using (Html.BeginForm("details", "membership", new { id = Model.User.ProviderUserKey }))
           { %>
        <fieldset class="jHeight">
            <legend>Email Address &amp; Comments</legend>
            <table class="tbl" cellspacing="0">
                <tr>
                    <td class="lbl">
                        <p>
                            <label for="User_Email">
                                Email Address:</label>
                        </p>
                    </td>
                    <td>
                        <%= Html.TextBoxFor(u=>u.User.Email).Attribute("style", "width: 200px") %>
                    </td>
                </tr>
                <tr>
                    <td class="lbl">
                        <p>
                            <label for="User_Comment">
                                Comments:</label></p>
                    </td>
                    <td>
                        <%= Html.TextAreaFor(u => u.User.Comment).Attribute("style", "width: 200px; height: 75px;") %>
                    </td>
                </tr>
            </table>
        </fieldset>
        <br />
        <%= Html.SpriteCtrl("Save e-mail address and comment", HtmlHelpers.CtrlType.Button, "disk") %>
        <% } %>
    </div>
    <div class="clear">
        &nbsp;
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="QuickLinks" runat="server">
</asp:Content>
