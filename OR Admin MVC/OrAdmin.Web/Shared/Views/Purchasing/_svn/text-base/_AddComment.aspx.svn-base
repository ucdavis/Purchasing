<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Modal.Master" Inherits="System.Web.Mvc.ViewPage<OrAdmin.Web.Areas.Business.Models.Purchasing._AddCommentViewModel>" %>

<asp:Content ID="Content0" ContentPlaceHolderID="Head" runat="server">
    <link href="<%= Url.Content("~/Content/Css/Min/FileUploader.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%= Url.Content("~/Scripts/Min/FileUploader.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
    <!--
        var purchasing;

        $(function(){

            $('#submitComments a').click(function(e){
                if($('#Comments').val().length == 0){
                    $('#Comments').addClass('input-validation-error');
                    e.stopPropagation();
                    e.preventDefault();
                }
            });

            /* Init file uploader
            ----------------------------------------------------------------------- */
            var uploadIndex = 0;
            fileUploadAction = '<%= Url.Action("_Upload") %>';
            maxUploadWarning = 'Some files were not uploaded (highlighted in red). Please contact support and/or try again. Please note that the maximum allowed file-size is ' + <%= OrAdmin.Core.Settings.GlobalSettings.PurchasingMaxUploadMB.ToString() %> + ' MB per file.';
            removeUploadedFileWarning = 'Are you sure you want to remove this attachment?';
            
            var uploader = new qq.FileUploader({
                element: document.getElementById('file-uploader'),
                action: fileUploadAction,
                onComplete: function (id, fileName, responseJSON) {
                    if (responseJSON.success) {
                        alert(escape(responseJSON.FileName));
                        $('ul.qq-upload-list li').eq(uploadIndex).append(
                                '<span class="qq-upload-remove">&nbsp;</span>' +
                                '<input type="hidden" name="Attachment[' + uploadIndex + '].FileName" value="' + escape(responseJSON.FileName) + '" />' +
                                '<input type="hidden" name="Attachment[' + uploadIndex + '].FileSizeBytes" value="' + escape(responseJSON.FileSizeBytes) + '" />' +
                                '<input type="hidden" name="Attachment[' + uploadIndex + '].SubmittedOn" value="' + responseJSON.SubmittedOn + '" />' +
                                '<input type="hidden" name="Attachment[' + uploadIndex + '].SubmittedBy" value="' + escape(responseJSON.SubmittedBy) + '" />'
                                // Remaining fields are set in controller
                            );
                        uploadIndex++;
                    }
                    else {
                        uploadIndex++;
                        alert(maxUploadWarning);
                    }
                }
            });
            if ($('#uploaded-files li').size() > 0) {
                $('#uploaded-files li').appendTo('ul.qq-upload-list');
                uploadIndex = $('ul.qq-upload-list li').size();
            }
            $('.qq-upload-remove').live('click', function (e) {
                if (confirm(removeUploadedFileWarning)) {
                    $(this).closest('li.qq-upload-success').fadeOut(500, function () {
                        $(this).find('input[name*=FileName]').remove();
                    });
                }
            });
        });

    //-->
    </script>
    <style type="text/css">
        ul.qq-upload-list { margin: 20px 0 -5px 2em; }
        ul.qq-upload-list li { font-size: 11px !important; padding-top: 2px !important; }
        .t-window-content-area { height: 316px; }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(o => o.RequestId) %>
    <%: Html.HiddenFor(o => o.RequestUniqueId) %>
    <% if (TempData["close"] != null)
       { %>
    <script type="text/javascript">
        parent.CloseAllWindows(true, '<%= Url.Action("myrequestdetails", new { requestUniqueId = Model.RequestUniqueId }) %>#comments');
    </script>
    <% } %>
    <p>Enter your comments below.</p>
    <%: Html.TextAreaFor(o => o.Comments, new { style = "width: 456px; height: 100px; margin-bottom: 10px;" }).Attribute("id", "comments") %>
    <div id="file-uploader">
        <noscript>
            <em style="color: red;">Please enable JavaScript to use the file uploader.</em>
        </noscript>
    </div>
    <div id="uploaded-files">
        <ul>
            <% int row = 0; %>
            <% if (Model.Attachments != null)
               { %>
            <% foreach (var attachment in Model.Attachments)
               { %>
            <li class="qq-upload-success"><span class="qq-upload-file">
                <%: HttpContext.Current.Server.UrlDecode(attachment.FileName) %></span><span class="qq-upload-size" style="display: inline;"><%= OrAdmin.Core.Extensions.StringExtensions.FormatBytes((long)attachment.FileSizeBytes)%></span><span class="qq-upload-remove">&nbsp;</span>
                <input type="hidden" name="Attachment[<%= row %>].Id" value="<%= attachment.Id %>" />
                <input type="hidden" name="Attachment[<%= row %>].FileName" value="<%= attachment.FileName %>" />
                <input type="hidden" name="Attachment[<%= row %>].FileSizeBytes" value="<%= attachment.FileSizeBytes %>" />
                <input type="hidden" name="Attachment[<%= row %>].SubmittedBy" value="<%= attachment.SubmittedBy %>" />
                <input type="hidden" name="Attachment[<%= row %>].SubmittedOn" value="<%= attachment.SubmittedOn %>" />
            </li>
            <% row++; %>
            <% } %>
            <% } %>
        </ul>
    </div>
    <ul style="margin: 15px 0;">
        <% if (!User.IsInRole(OrAdmin.Core.Enums.App.RoleName.PurchaseAdmin.ToString()))
           { %>
        <li>
            <%: Html.CheckBoxFor(o => o.NotifyAdmin, new { style = "vertical-align: middle;", @checked = "checked" })%>&nbsp;&nbsp;<%: Html.LabelFor(o => o.NotifyAdmin)%></li>
        <% } %>
        <% if (!User.IsInRole(OrAdmin.Core.Enums.App.RoleName.PurchaseApprover.ToString()))
           { %>
        <li>
            <%: Html.CheckBoxFor(o => o.NotifyApprover, new { style = "vertical-align: middle;", @checked = "checked" })%>&nbsp;&nbsp;<%: Html.LabelFor(o => o.NotifyApprover)%></li>
        <% } %>
        <% if (!User.IsInRole(OrAdmin.Core.Enums.App.RoleName.PurchaseManager.ToString()) &&
               !User.IsInRole(OrAdmin.Core.Enums.App.RoleName.PurchaseUser.ToString()))
           { %>
        <li>
            <%: Html.CheckBoxFor(o => o.NotifyManager, new { style = "vertical-align: middle;", @checked = "checked" })%>&nbsp;&nbsp;<%: Html.LabelFor(o => o.NotifyManager) %></li>
        <% } %>
        <% if (!User.IsInRole(OrAdmin.Core.Enums.App.RoleName.PurchaseUser.ToString()))
           { %>
        <li>
            <%: Html.CheckBoxFor(o => o.NotifyRequester, new { style = "vertical-align: middle;", @checked = "checked" })%>&nbsp;&nbsp;<%: Html.LabelFor(o => o.NotifyRequester) %></li>
        <% } %>
    </ul>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Commands" runat="server">
    <%: Html.SpriteCtrl("Submit comments", HtmlHelpers.CtrlType.Button, "tick").Attribute("id", "submitComments") %>
</asp:Content>
