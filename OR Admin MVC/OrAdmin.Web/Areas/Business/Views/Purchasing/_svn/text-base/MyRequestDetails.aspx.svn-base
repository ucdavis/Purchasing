<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Default.Master" Inherits="System.Web.Mvc.ViewPage<OrAdmin.Web.Areas.Business.Models.Purchasing.MyRequestDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Title" runat="server">
    Request Details
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <link href="<%= Url.Content("~/Content/Css/Min/Purchasing.css") %>" rel="stylesheet" type="text/css" />
    <link href="<%= Url.Content("~/Content/Css/Min/RequestDetails.css") %>" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <% Html.RenderAction("_RequestDetails", "purchasing", new { area = "business", requestUniqueId = Model.RequestUniqueId }); %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="QuickLinks" runat="server">
</asp:Content>
