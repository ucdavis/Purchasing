<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<% if (TempData[OrAdmin.Core.Enums.App.GlobalProperty.Message.SuccessMessage.ToString()] != null ||
       TempData[OrAdmin.Core.Enums.App.GlobalProperty.Message.FailMessage.ToString()] != null
       )
   { %>
<% if (TempData[OrAdmin.Core.Enums.App.GlobalProperty.Message.SuccessMessage.ToString()] != null)
   { %>
<span class="msg msg-success">
    <%: TempData[OrAdmin.Core.Enums.App.GlobalProperty.Message.SuccessMessage.ToString()].ToString() %>
</span>
<% } %>
<% if (TempData[OrAdmin.Core.Enums.App.GlobalProperty.Message.FailMessage.ToString()] != null)
   { %>
<span class="msg msg-fail">
    <%: TempData[OrAdmin.Core.Enums.App.GlobalProperty.Message.FailMessage.ToString()].ToString() %>
</span>
<% } %>
<script type="text/javascript">
    $(function () {
        $('.msg').fadeIn(100).delay(3000).fadeOut();
    });
</script>
<% } %>