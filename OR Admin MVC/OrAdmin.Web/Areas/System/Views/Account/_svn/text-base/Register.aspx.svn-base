<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Default.Master" Inherits="System.Web.Mvc.ViewPage<OrAdmin.Web.Areas.System.Models.Account.AccountViewModel+RegisterModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Title" runat="server">
    Register
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        table { width: 100%; border: 0 none; }
        table tr td { border: 0 none; padding: 0 20px; vertical-align: top; }
        fieldset { padding: 20px; }
        legend img { margin-bottom: 5px; }
        .acct-type { margin-top: 15px; border: dotted 1px #AAA; background-color: #F2F2F2; padding: 20px; display: none; }
    </style>
    <script type="text/javascript">
        $(function () {
            $('input[name=type]').click(function () {
                if ($(this).attr('id') == 'uc-system') {
                    $('#corporate-msg').slideUp(function () {
                        $('#uc-msg').slideDown();
                    });
                }
                else {
                    $('#uc-msg').slideUp(function () {
                        $('#corporate-msg').slideDown();
                    });
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Register for
        <%= OrAdmin.Core.Settings.GlobalSettings.SystemTitle %></h1>
    <p class="desc">
        <%= Html.ValidationSummary(true, "Account creation was unsuccessful. Please correct the errors and try again.") %></p>
    <p class="desc">Use the form below to create a new account.</p>
    <fieldset>
        <legend>
            <img src="/Content/Img/Common/ucd-registration.png" alt="" /><br />
            UC Davis Registration</legend>
        <% using (Html.BeginForm())
           { %>
        <table class="tbl" cellspacing="0">
            <tbody>
                <tr>
                    <td>
                        UC Davis Kerberos Login ID
                    </td>
                </tr>
                <tr>
                    <td>
                        <%= Html.TextBox("Kerberos") %>
                        <!-- Need validation here -->
                    </td>
                </tr>
                <tr>
                    <td>
                        Passphrase
                    </td>
                </tr>
                <tr>
                    <td>
                        <input type="text" value="Will be your UC Davis passphrase" disabled="disabled" style="width: 250px; color: #AAA; font-style: italic;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                        <%: Html.SpriteCtrl("Submit UC Davis Registration") %>
                    </td>
                </tr>
            </tbody>
        </table>
        <% } %>
    </fieldset>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="QuickLinks" runat="server">
</asp:Content>
