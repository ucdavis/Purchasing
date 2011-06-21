<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Modal.Master" Inherits="System.Web.Mvc.ViewPage<OrAdmin.Web.Areas.Business.Models.Purchasing._MoreItemInfoViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Title" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <% if (TempData["close"] != null)
       { %>
    <script type="text/javascript">
        $(function () {
            parent._updateMoreInfo(<%= Model.Index %>, '<%= Url.Encode(Model.Url) %>', '<%=  Url.Encode(Model.Notes) %>', '<%=  Url.Encode(Model.PromoCode) %>', <%= Model.CommodityTypeId %>);
        });
    </script>
    <% } %>
    <style type="text/css">
        p { margin-bottom: 5px; }
        .t-popup .t-item { font-size:  }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        <%= Html.LabelFor(o=>Model.Url) %></p>
    <%= Html.TextBoxFor(o => Model.Url, new { style = "width: 458px; margin-bottom: 10px;" }) %>
    <p>
        <%= Html.LabelFor(o=>Model.Notes) %></p>
    <%= Html.TextAreaFor(o => Model.Notes, new { style = "width: 458px; margin-bottom: 10px;", rows = "5" }) %>
    <p>
        <%= Html.LabelFor(o=>Model.PromoCode) %></p>
    <%= Html.TextBoxFor(o => Model.PromoCode, new { style = "width: 458px; margin-bottom: 10px;" }) %>
    <p>
        <%= Html.LabelFor(o=>Model.CommodityTypeId) %></p>
    <%= Html.Telerik().DropDownList()
        .BindTo(Model.CommodityTypes)
        .Name("CommodityTypeId")
        .Effects(effects => effects.Opacity()) 
    %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Commands" runat="server">
    <%= Html.SpriteCtrl("Done", HtmlHelpers.CtrlType.Button, "tick").Attribute("id", "done") %>
</asp:Content>
