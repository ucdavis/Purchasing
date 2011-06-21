<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Modal.Master" Inherits="System.Web.Mvc.ViewPage<OrAdmin.Web.Areas.Business.Models.Purchasing._StopRequestViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <% if (TempData["close"] != null)
       { %>
    <script type="text/javascript">
        parent.CloseAllWindows(true, '<%= Url.Action("myrequestdetails", new{ requestUniqueId = Model.RequestUniqueId }) %>#history');
    </script>
    <% } %>
    <p>Please tell us <strong><em>why</em></strong> you would like to stop this request.</p>
    <%: Html.TextAreaFor(Model => Model.Comments, new { style = "width: 458px; height: 100px;" })%>
    <%: Html.HiddenFor(Model => Model.RequestId) %>
    <%: Html.HiddenFor(Model => Model.RequestUniqueId) %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Commands" runat="server">
    <%: Html.SpriteCtrl("Submit stop request", HtmlHelpers.CtrlType.Button, "tick") %>
</asp:Content>
