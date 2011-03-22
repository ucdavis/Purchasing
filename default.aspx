<%@ Page Title="" Language="C#" MasterPageFile="~/master/default.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="tools_purchasing_default" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="LeftSidebarContentPlaceHolder" runat="Server">
    <!-- ********** Left sidebar area of the page ********** -->
    <div id="left_sidebar">
        <div id="level2_nav">
            <a name="nav"></a>
            <h2>In this section</h2>
            <asp:Menu ID="Menu3" runat="server">
            </asp:Menu>
        </div>
    </div>
    <!-- ********** End left sidebar area of the page ********** -->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="Server">
    <!-- ********** Main content area of the page ********** -->
    <div id="formwrap">
        <h1>
            <asp:Label ID="sectionTitleLabel" runat="server" /></h1>
        <telerik:RadWindow ID="requestRadWindow" VisibleStatusbar="false" IconUrl="~/images/common/blank.gif" Modal="true" Height="600px" Width="500" Behaviors="Move" VisibleTitlebar="true" NavigateUrl="register.aspx" runat="server">
        </telerik:RadWindow>
    </div>
    <!-- ********** End main content area of the page ********** -->
</asp:Content>
