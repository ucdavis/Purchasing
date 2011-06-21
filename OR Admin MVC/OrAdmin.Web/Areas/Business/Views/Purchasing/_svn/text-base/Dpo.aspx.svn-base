<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Default.Master" Inherits="System.Web.Mvc.ViewPage<OrAdmin.Web.Areas.Business.Models.Purchasing.DpoViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Title" runat="server">
    Purchase Request or Requisition
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <link href="<%= Url.Content("~/Content/Css/Min/Purchasing.css") %>" rel="stylesheet" type="text/css" />
    <link href="<%= Url.Content("~/Content/Css/Min/RequestForm.css") %>" rel="stylesheet" type="text/css" />
    <link href="<%= Url.Content("~/Content/Css/Min/FileUploader.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%= Url.Content("~/Scripts/Min/FileUploader.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/Min/Purchasing.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
    <!--
        var purchasing;

        $(function(){

            // Init purchasing scripts
            purchasing = new pur.Purchasing({
                moreItemInfoAction: '<%= Url.Action("_MoreItemInfo", "purchasing") %>',
                taxRate: <%= OrAdmin.Core.Settings.GlobalSettings.PurchasingTax %>
            });

            // Init file uploader
            uploadIndex = 0;

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

        function _updateMoreInfo(index, url, notes, promoCode, commodityTypeId) {

            // Receives values from item's "more info" window and updates the corresponding hidden values
            var $row = $('.item-row').eq(parseInt(index));
            $row.find('input[name$=Url]').val(unescape(url));
            $row.find('input[name$=Notes]').val(unescape(notes));
            $row.find('input[name$=PromoCode]').val(unescape(promoCode));
            $row.find('input[name$=CommodityTypeId]').val(commodityTypeId);

            // Close popups
            $('.t-window').each(function () {
                $('#' + $(this).attr('id')).data('tWindow').close();
            });

            purchasing._updateItemMoreInfoLink();
        }
    //-->
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <% /* Purchase request
    -----------------------------------------------*/ %>
    <% if (Model.Request.Id == 0 && Model.LastRequest != null)
       { %>
    <div class="topRight">
        <%= Html.SpriteCtrl("Populate with last request", HtmlHelpers.CtrlType.Link, "pencil", null, null, "dpo", "purchasing", new { requestUniqueId = Model.LastRequest.UniqueId, mode = OrAdmin.Core.Enums.Purchasing.Types.FormMode.copy }, HtmlHelpers.ImagePosition.Left, null, true).Attribute("onclick", "purchasing._options.unloadOkay = true;") %></div>
    <% } %>
    <h1>
        <% if (Model.Request.Id == 0 || (Model.FormMode.HasValue && Model.FormMode == OrAdmin.Core.Enums.Purchasing.Types.FormMode.copy))
           { %>
        Departmental Purchase Request
        <% }
           else if (Model.FormMode.HasValue && Model.FormMode.Value == OrAdmin.Core.Enums.Purchasing.Types.FormMode.edit)
           { %>
        Now Editing: Departmental Purchase Request:
        <%= Model.Request.FriendlyUniqueId %>
        <% }
           else
           { %>
        Departmental Purchase Request:
        <%= Model.Request.FriendlyUniqueId %>
        <% } %>
    </h1>
    <%= Html.ValidationSummaryWithContainer("Please correct the following errors") %>
    <br />
    <% using (Html.BeginForm())
       { %>
    <% /* Hidden fields 
    -----------------------------------------------*/ %>
    <%= Html.HiddenFor(o => Model.Request.Id) %>
    <%= Html.HiddenFor(o => Model.Request.UniqueId) %>
    <%= Html.HiddenFor(o => Model.Request.FriendlyUniqueId) %>
    <% /* Vendor
    -----------------------------------------------*/ %>
    <table class="tbl gray" cellspacing="0">
        <tr>
            <th>
                Vendor <a name="vendor"></a>
            </th>
        </tr>
        <tr class="smaller">
            <td>
                Type the vendor name below. If your vendor is not listed, select "Add new vendor"
            </td>
        </tr>
        <tr class="form">
            <td>
                <%= Html.Telerik().ComboBoxFor(o => Model.Request.VendorId)
                        .Filterable()
                        .HighlightFirstMatch(true)
                        .BindTo(Model.VendorList)
                        .DataBinding(dataBinding => dataBinding
                            .Ajax()
                            .Select("_GetVendors", "purchasing", new { selectedValue = Model.Request.VendorId > 0 ? (int?)Model.Request.VendorId : null, vendorType = (int)OrAdmin.Repositories.Purchasing.RequestRepository.RequestType.Dpo }))
                        .HtmlAttributes(new { style = "width: 422px; vertical-align: middle;" })
                        .AutoFill(true)
                        .Effects(effect => effect.Slide().Opacity())
                        .ClientEvents(events => events.OnDataBound("purchasing._onVendorsDataBound"))
                %>
                &nbsp;<span class="or">(or)</span>
                <%= Html.SpriteCtrl("Add new vendor", HtmlHelpers.CtrlType.Link, "plus-small").Attribute("id", "add-vendor")%>
                <% Html.Telerik().Window()
                    .Name("AddVendor")
                    .Icon(Url.Content("~/Content/Img/Fugue/store--plus.png"), "+")
                    .Visible(false)
                    .Title("Add new vendor")
                    .Content(() =>
                    {
                %>
                <iframe id="add-vendor-frame" src="<%= Url.Action("_AddVendor", "purchasing") %>" style="width: 100%; height: 100%;" frameborder="0"></iframe>
                <%
                    })
                    .Draggable(true)
                    .Buttons(b => b.Close())
                    .Scrollable(false)
                    .Width(500)
                    .Height(412)
                    .Modal(true)
                    .Render();
                %>
            </td>
        </tr>
    </table>
    <% /* Dilivery location
    -----------------------------------------------*/ %>
    <table class="tbl gray" cellspacing="0">
        <tr>
            <th>
                Delivery Location
            </th>
        </tr>
        <tr class="smaller">
            <td>
                Enter a delivery location. If your location is not listed, select "Add new location"
            </td>
        </tr>
        <tr class="form">
            <td>
                <table cellspacing="0" class="nopad">
                    <tr>
                        <td style="padding: 0 10px 5px 0;">
                            <%= Html.LabelFor(o => Model.Request.ShipToName)%>
                        </td>
                        <td style="padding: 0 10px 5px 0;">
                            <%= Html.LabelFor(o => Model.Request.ShipToEmail)%>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding: 0 10px 10px 0;">
                            <% var profile = User.Profile(); %>
                            <%= Html.TextBox("Request.ShipToName", profile != null ? profile.FirstName + " " + profile.LastName : User.Identity.Name, new { style = "width: 200px;" })%>
                        </td>
                        <td style="padding: 0 10px 10px 0;">
                            <%= Html.TextBox("Request.ShipToEmail", profile != null ? profile.Email : String.Empty, new { style = "width: 200px;" })%>
                        </td>
                    </tr>
                </table>
                <%= Html.Telerik().ComboBoxFor(o => Model.Request.ShipToId)
                        .Filterable()
                        .HighlightFirstMatch(true)
                        .BindTo(Model.ShipToAddressList)
                        .DataBinding(dataBinding => dataBinding
                            .Ajax()
                            .Select("_GetAddresses", "purchasing", new { selectedValue = Model.Request.ShipToId > 0 ? (int?)Model.Request.ShipToId : null }))
                        .HtmlAttributes(new { style = "width: 422px; vertical-align: middle;" })
                        .AutoFill(true)
                        .Effects(effect => effect.Slide().Opacity())
                        .ClientEvents(events => events.OnDataBound("purchasing._onAddressesDataBound"))
                %>
                &nbsp;<span class="or">(or)</span>
                <%= Html.SpriteCtrl("Add new location", HtmlHelpers.CtrlType.Link, "plus-small").Attribute("id", "add-address")%>
                <% Html.Telerik().Window()
                    .Name("AddAddress")
                    .Icon(Url.Content("~/Content/Img/Fugue/home--plus.png"), "+")
                    .Visible(false)
                    .Title("Add new address")
                    .Content(() =>
                    { 
                %>
                <iframe id="add-address-frame" src="<%= Url.Action("_AddAddress", "purchasing") %>" style="width: 100%; height: 100%;" frameborder="0"></iframe>
                <%
                    })
                    .Draggable(true)
                    .Buttons(b => b.Close())
                    .Scrollable(false)
                    .Width(500)
                    .Height(470)
                    .Modal(true)
                    .Render();
                %>
            </td>
        </tr>
    </table>
    <% /* Items
    -----------------------------------------------*/ %>
    <table class="tbl gray" cellspacing="0">
        <tr>
            <th>
                <div style="float: right; font-weight: normal">
                    <%= Html.SpriteCtrl("Add another item", HtmlHelpers.CtrlType.Link, "price-tag--plus").Attribute("id", "add-item")%>
                </div>
                Items
            </th>
        </tr>
        <tr class="smaller">
            <td>
                Enter your items below. Click "Add another item" to add more rows.
            </td>
        </tr>
        <tr class="form">
            <td style="padding: 0 0 6px 0;">
                <table id="items" cellspacing="0">
                    <tr class="item-header">
                        <td style="width: 10%;">
                            Quantity
                        </td>
                        <td style="width: 10%;">
                            Unit
                        </td>
                        <td style="width: 15%;">
                            Catalog #
                        </td>
                        <td style="width: 49%;">
                            Complete desciption
                        </td>
                        <td style="width: 11%;">
                            Unit price
                        </td>
                        <td style="width: 5%; text-align: center;">
                            More
                        </td>
                    </tr>
                    <% int row = 0; %>
                    <% foreach (var DpoItem in Model.DpoItems)
                       { %>
                    <tr class="item-row <%= row < OrAdmin.Core.Settings.GlobalSettings.PurchasingInitialItemRows || DpoItem.Id > 0 || DpoItem.Quantity != null || DpoItem.Unit != "each" || DpoItem.CatalogNumber != null || DpoItem.PricePerUnit != null ? String.Empty : "item-hidden" %>">
                        <td>
                            <%= Html.Telerik()
                                .NumericTextBoxFor(o => DpoItem.Quantity)
                                .Name(String.Format("DpoItem[{0}].Quantity", row))
                                .HtmlAttributes(new { id = String.Format("DpoItem_{0}_Quantity", row) })
                                .EmptyMessage(null)
                                .DecimalDigits(0)
                                .MinValue(0)
                                .MaxValue(5000)
                            %>
                        </td>
                        <td>
                            <%= Html.TextBox(String.Format("DpoItem[{0}].Unit", row), DpoItem.Unit).Attribute("id", String.Format("DpoItem_{0}_Unit", row)) %>
                        </td>
                        <td>
                            <%= Html.TextBox(String.Format("DpoItem[{0}].CatalogNumber", row), DpoItem.CatalogNumber).Attribute("id", String.Format("DpoItem_{0}_CatalogNumber", row)) %>
                        </td>
                        <td>
                            <%= Html.TextBox(String.Format("DpoItem[{0}].Description", row), DpoItem.Description).Attribute("id", String.Format("DpoItem_{0}_Description", row)) %>
                        </td>
                        <td>
                            <%= Html.Telerik()
                                .CurrencyTextBoxFor(o => DpoItem.PricePerUnit)
                                .Name(String.Format("DpoItem[{0}].PricePerUnit", row))
                                .HtmlAttributes(new { id = String.Format("DpoItem_{0}_PricePerUnit", row) })                          
                                .EmptyMessage(null)
                            %>
                        </td>
                        <td style="text-align: center; padding: 7px 10px 0 10px;">
                            <img src="<%= Url.Content("~/Content/Img/Fugue/document--plus.png") %>" class="more-item-info" alt="[+]" title="Add additional information to this item" style="cursor: pointer;" />
                            <%= Html.Hidden(String.Format("DpoItem[{0}].Url", row), DpoItem.Url).Attribute("id", String.Format("DpoItem_{0}_Url", row)) %>
                            <%= Html.Hidden(String.Format("DpoItem[{0}].PromoCode", row), DpoItem.PromoCode).Attribute("id", String.Format("DpoItem_{0}_PromoCode", row)) %>
                            <%= Html.Hidden(String.Format("DpoItem[{0}].Notes", row), DpoItem.Notes).Attribute("id", String.Format("DpoItem_{0}_Notes", row)) %>
                            <%= Html.Hidden(String.Format("DpoItem[{0}].CommodityTypeId", row), DpoItem.CommodityTypeId > 0 ? DpoItem.CommodityTypeId : 1).Attribute("id", String.Format("DpoItem_{0}_CommodityTypeId", row)) %>
                        </td>
                    </tr>
                    <% row++; %>
                    <% } %>
                    <tr class="item-footer">
                        <td colspan="4" style="text-align: right; padding-right: 0;">
                            Estimated Total (including
                            <%= OrAdmin.Core.Settings.GlobalSettings.PurchasingTax %>% tax but not including shipping fees):
                        </td>
                        <td colspan="2">
                            <span id="subtotal"></span>
                        </td>
                    </tr>
                </table>
                <% Html.Telerik().Window()
                    .Name("MoreItemInfoWindow")
                    .Icon(Url.Content("~/Content/Img/Fugue/document--plus.png"), "+")
                    .Visible(false)
                    .Title("More information")
                    .Content("placeholder")
                    .Draggable(true)
                    .Buttons(b => b.Close())
                    .Scrollable(false)
                    .Width(500)
                    .Height(355)
                    .Modal(true)
                    .Render();
                %>
            </td>
        </tr>
    </table>
    <% /* Request preferences
    -----------------------------------------------*/ %>
    <table class="tbl gray" cellspacing="0">
        <tr>
            <th>
                Request Preferences
            </th>
        </tr>
        <tr class="smaller">
            <td>
                Help us proccess your request by choosing from the following preferences
            </td>
        </tr>
        <tr class="form">
            <td>
                <table class="tbl nopad" cellspacing="0">
                    <% /* Approvers
                    -----------------------------------------------*/ %>
                    <% row = 0; %>
                    <% foreach (var approver in Model.PIApprovals)
                       { %>
                    <tr class="<%= approver.PiId != null || row == 0 ? String.Empty : "approver-hidden" %>">
                        <td class="lbl">
                            <% if (row == 0)
                               { %>
                            <label for="PiApproval0">
                                Your PI/approver:</label>
                            <% } %>
                        </td>
                        <td>
                            <% SelectList approverList = new SelectList(Model.ApproverList, "Value", "Text", approver.PiId); %>
                            <%= Html.Telerik().DropDownListFor(o => approver.PiId)
                                .BindTo(approverList)
                                .Name(String.Format("PiApproval[{0}].PiId", row))
                                .HtmlAttributes(new { style = "width: 220px; vertical-align: middle;", id = String.Format("PiApproval{0}", row) })
                            %>&nbsp;
                            <% if (row == 0)
                               { %>
                            <%: Html.SpriteCtrl("Add another", HtmlHelpers.CtrlType.Link, "plus-small").Attribute("id", "add-approver")%>
                            <%= Html.ValidationMessage("noApprovers") %>
                            <% } %>
                        </td>
                    </tr>
                    <% row++; %>
                    <% } %>
                    <tr>
                        <td class="lbl">
                            <%: Html.LabelFor(o => Model.Request.PurchaserId)%>:
                        </td>
                        <td>
                            <%= Html.Telerik().DropDownListFor(o => Model.Request.PurchaserId)
                                .BindTo(Model.PurchaserList)
                                .HtmlAttributes(new { style = "width: 220px;" })
                            %>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">
                            <%: Html.LabelFor(o => Model.Request.ShippingMethodId)%>:
                        </td>
                        <td>
                            <%= Html.Telerik().DropDownListFor(o => Model.Request.ShippingMethodId)
                                .BindTo(Model.ShippingMethodList)
                                .HtmlAttributes(new { style = "width: 220px;" })
                            %>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">
                            <%: Html.LabelFor(o => Model.Request.RequestedAccount)%>:
                        </td>
                        <td>
                            <%= Html.Telerik().ComboBoxFor(o => Model.Request.RequestedAccount)
                                .Filterable()
                                .HighlightFirstMatch(true)
                                .BindTo(Model.RequestedAccountList)
                                .HtmlAttributes(new { style = "width: 220px;" })
                                .AutoFill(true)
                                .Effects(effect => effect.Slide().Opacity())
                            %>
                            <%= Html.ValidationMessageFor(o => Model.Request.RequestedAccount) %>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">
                            <%: Html.LabelFor(o => Model.Request.Rua)%>:
                        </td>
                        <td>
                            <%= Html.TextBoxFor(o => Model.Request.Rua, new { style = "width : 214px;" })%>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">
                            <%: Html.LabelFor(o => Model.Request.DateNeeded)%>:
                        </td>
                        <td>
                            <%= Html.Telerik().DatePickerFor(o => Model.Request.DateNeeded)
                                    .Value(Model.Request.DateNeeded)
                                    .InputHtmlAttributes(new { style = "width: 100px;" })
                            %>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">
                            <%: Html.LabelFor(o => Model.Request.OkayToBackorder)%>:
                        </td>
                        <td>
                            <%= Html.RadioButtonFor(o => Model.Request.OkayToBackorder, true, new { id = "Request_OkayToBackorder_Yes" })%>&nbsp;<label for="Request_OkayToBackorder_Yes">Yes</label>
                            <%= Html.RadioButtonFor(o => Model.Request.OkayToBackorder, false, new { id = "Request_OkayToBackorder_No" })%>&nbsp;<label for="Request_OkayToBackorder_No">No</label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">
                            <%: Html.LabelFor(o => Model.Request.IsPerishable)%>:
                        </td>
                        <td>
                            <%= Html.RadioButtonFor(o => Model.Request.IsPerishable, true, new { id = "Request_IsPerishable_Yes" })%>&nbsp;<label for="Request_IsPerishable_Yes">Yes</label>
                            <%= Html.RadioButtonFor(o => Model.Request.IsPerishable, false, new { id = "Request_IsPerishable_No" })%>&nbsp;<label for="Request_IsPerishable_No">No</label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">
                            <%: Html.LabelFor(o => Model.Request.IsRadioactive)%>:
                        </td>
                        <td>
                            <%= Html.RadioButtonFor(o => Model.Request.IsRadioactive, true, new { id = "Request_IsRadioactive_Yes" })%>&nbsp;<label for="Request_IsRadioactive_Yes">Yes</label>
                            <%= Html.RadioButtonFor(o => Model.Request.IsRadioactive, false, new { id = "Request_IsRadioactive_No" })%>&nbsp;<label for="Request_IsRadioactive_No">No</label>
                        </td>
                    </tr>
                    <tr>
                        <td class="lbl">
                            <%: Html.LabelFor(o => Model.Request.Comments)%>:
                        </td>
                        <td>
                            <%= Html.TextAreaFor(o => Model.Request.Comments, new { style = "width: 350px;", rows = "5" })%>
                        </td>
                    </tr>
                    <% /* Attachments
                    -----------------------------------------------*/ %>
                    <tr>
                        <td class="lbl" style="padding-top: 11px;">
                            <label for="file-uploader" style="margin-top: 5px;">
                                Attachments</label>:
                        </td>
                        <td>
                            <div id="file-uploader">
                                <noscript>
                                    <em style="color: red;">Please enable JavaScript to use the file uploader.</em>
                                </noscript>
                            </div>
                            <div id="uploaded-files">
                                <ul>
                                    <% row = 0; %>
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
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <% /* Submit buttons
    -----------------------------------------------*/ %>
    <div style="margin-bottom: 10px; overflow: hidden;">
        <% if (Model.FormMode.HasValue && Model.FormMode.Value == OrAdmin.Core.Enums.Purchasing.Types.FormMode.edit)
           { %>
        <%= Html.SpriteCtrl("Submit updated request", HtmlHelpers.CtrlType.Button, "tick", "Submit this updated request (required re-approval)", "submitAction", null, null, null, HtmlHelpers.ImagePosition.Left, HtmlHelpers.ButtonColor.Green, null).Attribute("id", "submit-request") %>
        <%= Html.SpriteCtrl("Update, save and submit later", HtmlHelpers.CtrlType.Button, "disk", "Update and save this request to your \"My Orders\" section", "submitAction").Attribute("id", "save-request") %>
        <% }
           else
           { %>
        <%= Html.SpriteCtrl("Submit request", HtmlHelpers.CtrlType.Button, "tick", "Submit this request", "submitAction", null, null, null, HtmlHelpers.ImagePosition.Left, HtmlHelpers.ButtonColor.Green, null).Attribute("id", "submit-request") %>
        <%= Html.SpriteCtrl("Save and submit later", HtmlHelpers.CtrlType.Button, "disk", "Save this request to your \"My Orders\" section", "submitAction").Attribute("id", "save-request") %>
        <% } %>
        <%= Html.SpriteSeparator("or")%>
        <%= Html.SpriteCtrl("Cancel", HtmlHelpers.CtrlType.Button, "arrow-curve-180-left", "Cancel this request", null, Model.FormMode.HasValue ? "MyRequestDetails" : "request", "purchasing", Model.FormMode.HasValue ? new { requestUniqueId = Model.Request.UniqueId } : null).Attribute("onclick", "purchasing._options.unloadOkay = true;")%>
    </div>
    <% } %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="QuickLinks" runat="server">
</asp:Content>
