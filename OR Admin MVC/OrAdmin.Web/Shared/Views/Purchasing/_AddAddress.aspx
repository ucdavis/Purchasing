<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Modal.Master" Inherits="System.Web.Mvc.ViewPage<OrAdmin.Entities.Purchasing.ShipToAddress>" %>

<asp:Content ID="Content0" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        p { margin-bottom: 5px; }
        input[type=text], textarea { margin-bottom: 10px; width: 458px; }
        table td { padding-right: 10px; }
        table th { padding-bottom: 5px; font-weight: normal; text-align: left; }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <% if (TempData["close"] != null)
       { %>
    <script type="text/javascript">
        parent.purchasing._reloadAddresses('<%= TempData["Id"] %>');
    </script>
    <% } %>
    <p>
        <%= Html.LabelFor(o => Model.FriendlyName) %>
        (EX: Chemistry Loading Dock, Room 505)
        <%= Html.ValidationMessage("FriendlyName") %></p>
    <%= Html.TextBoxFor(o => Model.FriendlyName) %>
    <p>
        <%= Html.LabelFor(o => Model.Campus) %></p>
    <%= Html.TextBoxFor(o => Model.Campus) %>
    <p>
        <%= Html.LabelFor(o => Model.Building) %></p>
    <%= Html.TextBoxFor(o => Model.Building) %>
    <p>
        <%= Html.LabelFor(o => Model.Room)%></p>
    <%= Html.TextBoxFor(o => Model.Room)%>
    <p>
        <%= Html.LabelFor(o => Model.Street)%></p>
    <%= Html.TextBoxFor(o => Model.Street)%>
    <table cellspacing="0">
        <tr>
            <th>
                <%= Html.LabelFor(o => Model.City) %>
            </th>
            <th>
                <%= Html.LabelFor(o => Model.State) %>
            </th>
            <th>
                <%= Html.LabelFor(o => Model.Zip) %>
            </th>
        </tr>
        <tr>
            <td>
                <%= Html.TextBoxFor(o => Model.City, new { style="width: 100px;" }) %>
            </td>
            <td>
                <%= Html.TextBoxFor(o => Model.State, new { style="width: 20px;" }) %>
            </td>
            <td>
                <%= Html.TextBoxFor(o => Model.Zip, new { style="width: 80px;" }) %>
            </td>
        </tr>
    </table>
    <table cellspacing="0">
        <tr>
            <th>
                <%= Html.LabelFor(o => Model.Phone) %>
            </th>
            <th>
                <%= Html.LabelFor(o => Model.Fax) %>
            </th>
        </tr>
        <tr>
            <td>
                <%= Html.TextBoxFor(o => Model.Phone, new { style="width: 100px" }) %>
            </td>
            <td>
                <%= Html.TextBoxFor(o => Model.Fax, new { style="width: 100px" }) %>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Commands" runat="server">
    <%: Html.SpriteCtrl("Submit new address", HtmlHelpers.CtrlType.Button, "tick") %>
</asp:Content>
