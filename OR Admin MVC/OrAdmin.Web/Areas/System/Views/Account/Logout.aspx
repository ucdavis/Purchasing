<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Default.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Title" runat="server">
    Logout Successful
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        #centerwrap h1, #centerwrap p { text-align: center; }
        #centerwrap h1 { margin-top: 50px; }
        #centerwrap p { padding: 0 200px; }
        #centerwrap p.smaller { font-weight: bold; font-size: 9px; }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <h1>You are logged out of
        <%: OrAdmin.Core.Settings.GlobalSettings.SystemTitle %></h1>
    <p class="desc">To ensure your complete privacy and security, close ALL Web browsers.</p>
    <p class="desc">
        <%: Html.ActionLink("Re-enter " + OrAdmin.Core.Settings.GlobalSettings.SystemTitle, "overview", new { controller = "home", area = "home" })%></p>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="QuickLinks" runat="server">
</asp:Content>
