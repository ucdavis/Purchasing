<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Default.Master" Inherits="System.Web.Mvc.ViewPage<OrAdmin.Entities.App.Profile>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Title" runat="server">
    Index
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        .lbl { width: 120px; }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Welcome,
        <%: User.HasProfile() ? User.Profile().FirstName : User.Identity.Name %>!</h1>
    <p class="desc">Please update your profile below and click the "Save profile" button when you're finished. Information entered on this page will remain private.</p>
    <% using (Html.BeginForm())
       {%>
    <%: Html.HiddenFor(model=>model.Id) %>
    <%: Html.HiddenFor(model=>model.UserName) %>
    <fieldset>
        <legend>Personal Information</legend>
        <table class="tbl" cellspacing="0">
            <tr>
                <td class="lbl">
                    <p>*
                        <%: Html.LabelFor(model => model.FirstName) %>: </p>
                </td>
                <td>
                    <%: Html.TextBoxFor(model => model.FirstName).Attribute("style", "width: 200px") %>
                    <%: Html.ValidationMessageFor(model => model.FirstName) %>
                </td>
            </tr>
            <tr>
                <td class="lbl">
                    <p>*
                        <%: Html.LabelFor(model => model.LastName) %>: </p>
                </td>
                <td>
                    <%: Html.TextBoxFor(model => model.LastName).Attribute("style", "width: 200px")%>
                    <%: Html.ValidationMessageFor(model => model.LastName) %>
                </td>
            </tr>
            <tr>
                <td class="lbl">
                    <p>*
                        <%: Html.LabelFor(model => model.Email) %>: </p>
                </td>
                <td>
                    <%: Html.TextBoxFor(model => model.Email).Attribute("style", "width: 200px")%>
                    <%: Html.ValidationMessageFor(model => model.Email) %>
                </td>
            </tr>
        </table>
    </fieldset>
    <div class="sprite-bar">
        <%= Html.SpriteCtrl("Save profile", HtmlHelpers.CtrlType.Button, "disk") %>
    </div>
    <% } %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="QuickLinks" runat="server">
</asp:Content>
