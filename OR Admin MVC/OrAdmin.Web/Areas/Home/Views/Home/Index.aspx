<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Default.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Title" runat="server">
    Home
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
    <h1>Welcome to
        <%: OrAdmin.Core.Settings.GlobalSettings.SystemTitle %></h1>
    <p class="desc">To Logon, you must be an authorized user and have a UCD LoginID and Kerberos password.</p>
    <p class="desc">
        <%: Html.ActionLink("Enter " + OrAdmin.Core.Settings.GlobalSettings.SystemTitle, "overview", new { controller = "home", area = "home" })%></p>
    <p class="desc smaller">For more information, help, or documentation, please visit the online help pages</p>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="QuickLinks" runat="server">
</asp:Content>
