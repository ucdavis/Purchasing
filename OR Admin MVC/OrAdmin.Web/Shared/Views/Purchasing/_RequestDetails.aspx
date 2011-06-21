<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<OrAdmin.Web.Areas.Business.Models.Purchasing._RequestDetailsViewModel>" %>

<script type="text/javascript">
<!--
    $(function () {

        $('.details-col fieldset').eqHeight();

        $('#add-comments').click(function (e) {
            var win = $('#AddCommentsWindow');
            win.data('tWindow').center().open();
            var iframe = $('#addcomments')[0];
            iframe.src = iframe.src;
            e.preventDefault();
            e.stopPropagation();
        });

        $('#stop-request').click(function (e) {
            var win = $('#StopRequest');
            win.data('tWindow').center().open();
            var iframe = $('#stoprequest')[0];
            iframe.src = iframe.src;
            e.preventDefault();
            e.stopPropagation();
        });

    });

    function CloseAllWindows(reload, href) {

        $('.t-window').each(function () {
            $('#' + $(this).attr('id')).data('tWindow').close();
        });

        if (reload) {
            if (href)
                document.location.href = href;
            else
                document.location.href = document.location.href;

            window.location.reload()
        }
    }

//-->
</script>
<div class="roundedBox">
    <div class="roundedBoxTop">
        <span></span>
    </div>
    <div class="roundedBoxContent">
        <!-- REQUEST -->
        <div class="topRight ctrls noprint">
            <% var status = Model.HistoryItems.ElementAtOrDefault(0); %>
            <%: Html.SpriteCtrl("Back", HtmlHelpers.CtrlType.Button, "arrow-curve-180-left", "Back to request listings", null, "my-requests") %>
            <%: Html.SpriteCtrl("Print", HtmlHelpers.CtrlType.Button, "printer", "Print this page") %>
            <%: Html.SpriteCtrl("Copy", HtmlHelpers.CtrlType.Button, "document-copy", "Tee-up a new request with similar specs", null, "dpo", "purchasing", new { requestUniqueId = Model.Request.UniqueId, mode = OrAdmin.Core.Enums.Purchasing.Types.FormMode.copy }) %>
            <%: Html.SpriteCtrl("Edit", HtmlHelpers.CtrlType.Button, "document--pencil", "Modify this request (required re-approval)", null, "dpo", "purchasing", new { requestUniqueId = Model.Request.UniqueId, mode = OrAdmin.Core.Enums.Purchasing.Types.FormMode.edit }) %>
            <%: Html.SpriteCtrl("Stop", HtmlHelpers.CtrlType.Button, "cross-circle", "Stop this request", null, null, null, null, HtmlHelpers.ImagePosition.Left, HtmlHelpers.ButtonColor.Red, true).Attribute("id", "stop-request") %>
            <% if (User.Identity.Name.Equals(Model.Request.RequesterId, StringComparison.OrdinalIgnoreCase) &&
                   status.TypeValue == (int)OrAdmin.Repositories.Purchasing.MilestoneMapRepository.MilestoneType.Saved &&
                   status.IsMilestone)
               { %>
            <% using (Html.BeginForm("_RequestDetails", "purchasing", new { requestUniqueId = Model.Request.UniqueId }, FormMethod.Post))
               { %>
            <%: Html.SpriteCtrl("Submit", HtmlHelpers.CtrlType.Button, "tick", "Submit request", null, null, null, null, HtmlHelpers.ImagePosition.Left, HtmlHelpers.ButtonColor.Green, true).Attribute("style", "margin-left: 5px;") %>
            <% } %>
            <% } %>
            <% Html.Telerik().Window()
                    .Visible(false)
                    .Name("StopRequest")
                    .Title("Stop request")
                    .Icon(Url.Content("~/Content/Img/Fugue/cross-octagon.png"), "X")
                    .Content(() =>
                    { %>
            <iframe id="stoprequest" src="<%= Url.Action("_StopRequest", "purchasing", new { requestId = Model.Request.Id, requestUniqueId = Model.Request.UniqueId }) %>" style="width: 100%; height: 100%;" frameborder="0"></iframe>
            <% })
                    .Draggable(true)
                    .Buttons(b => b.Close())
                    .Scrollable(false)
                    .Width(500)
                    .Height(221)
                    .Modal(true)
                    .Render();
            %>
        </div>
        <h1>
            <%: Model.Request.FriendlyUniqueId %>:
            <%: Model.Request.Vendor.FriendlyName %></h1>
        <p class="detail-date-needed">Items needed by:
            <%: Model.Request.DateNeeded.HasValue ? Model.Request.DateNeeded.Value.ToString("M/d/yyyy") : "N/A" %></p>
        <fieldset class="first">
            <legend>Items</legend>
            <table cellspacing="0" class="tbl detailsTbl">
                <tr>
                    <th style="width: 5%">
                        Qty
                    </th>
                    <th style="width: 7%">
                        Unit
                    </th>
                    <th style="width: 12%">
                        Catalog #
                    </th>
                    <th style="width: 59%">
                        Description
                    </th>
                    <th style="width: 12%">
                        Unit Price
                    </th>
                    <th style="width: 5%">
                        Notes
                    </th>
                </tr>
                <% int index = 1; %>
                <% foreach (var item in Model.Request.DpoItems)
                   { %>
                <tr class="<%= index % 2 == 0 ? String.Empty : "alt" %>">
                    <td>
                        <%: item.Quantity %>
                    </td>
                    <td>
                        <%: item.Unit %>
                    </td>
                    <td>
                        <%: item.CatalogNumber %>
                    </td>
                    <td>
                        <%= String.IsNullOrEmpty(item.Url) ? Html.Encode(item.Description) : "<a href='" + item.Url + "' title='View item on vendor Web site' target='_blank'>" + Html.Encode(item.Description) + "</a><span class='external-img'><img src='" + Url.Content("~/Content/Img/Fugue/arrow-045-small.png") + "' alt='' /></span>" %>
                    </td>
                    <td>
                        <%: item.PricePerUnit.HasValue ? item.PricePerUnit.Value.ToString("C2") : String.Empty %>
                    </td>
                    <td style="text-align: center;">
                        <%: Html.SpriteCtrl(null, HtmlHelpers.CtrlType.Link, "balloon", null, null, null, null, null, HtmlHelpers.ImagePosition.Left, null, true) %>
                    </td>
                </tr>
                <% index++; %>
                <% } %>
                <tr id="subtotalRow">
                    <td colspan="4" style="text-align: right">
                        Subtotal:
                    </td>
                    <td colspan="2">
                        <span style="font-weight: bold;">
                            <% decimal subTotal = Model.Request.DpoItems.Sum(i => (i.Quantity.HasValue ? i.Quantity.Value : 0) * (i.PricePerUnit.HasValue ? i.PricePerUnit.Value : 0)); %>
                            <%= subTotal.ToString("C2") %>
                        </span>
                    </td>
                </tr>
                <tr class="last">
                    <td colspan="4" style="text-align: right">
                        Estimated Total (including
                        <%: OrAdmin.Core.Settings.GlobalSettings.PurchasingTax %>% tax but not including shipping fees):
                    </td>
                    <td colspan="2">
                        <span style="font-weight: bold;">
                            <%= (subTotal * (OrAdmin.Core.Settings.GlobalSettings.PurchasingTax / 100) + subTotal).ToString("C2") %></span>
                    </td>
                </tr>
            </table>
        </fieldset>
        <br class="clear" />
        <br />
        <div class="details-col">
            <fieldset id="col-1">
                <legend>Vendor</legend>
                <h2>
                    <%: Model.Request.Vendor.FriendlyName %></h2>
                <ul>
                    <li><strong>DaFIS Vendor ID:</strong>&nbsp;<%: Model.Request.Vendor.DafisVendorId.HasValue ? Model.Request.Vendor.DafisVendorId.Value.ToString() : "Unassigned" %></li>
                    <li><strong>Address:</strong>&nbsp;<%: Model.Request.Vendor.Address %><br />
                        <%: Model.Request.Vendor.City %>,
                        <%: Model.Request.Vendor.State %>
                        <%: Model.Request.Vendor.Zip %></li>
                    <li><strong>Phone:</strong>&nbsp;<%: Model.Request.Vendor.Phone %></li>
                    <li><strong>Fax:</strong>&nbsp;<%: Model.Request.Vendor.Fax %></li>
                    <% if (!String.IsNullOrEmpty(Model.Request.Vendor.Url))
                       { %>
                    <li><a href="<%= Url.Encode(Model.Request.Vendor.Url) %>" title="Visit vendor Web site" target="_blank">Vendor Web site</a> <span class="external-img">
                        <img alt="" src="<%= Url.Content("~/Content/Img/Fugue/arrow-045-small.png") %>" /></span></li>
                    <% } %>
                    <li><a href="#">Vendor notes</a></li>
                </ul>
            </fieldset>
        </div>
        <div class="details-col">
            <fieldset id="col-2">
                <legend>Deliver To</legend>
                <h2>
                    <%: Model.Request.ShipToAddress.FriendlyName %></h2>
                <ul>
                    <li><strong>ATTN:</strong>&nbsp;<%: Model.Request.ShipToName %></li>
                    <li><strong>Campus:</strong>&nbsp;<%: Model.Request.ShipToAddress.Campus %></li>
                    <li><strong>Building/Room:</strong>&nbsp;<%: Model.Request.ShipToAddress.Building %>/<%: Model.Request.ShipToAddress.Room %></li>
                    <li><strong>Address:</strong>&nbsp;<%: Model.Request.ShipToAddress.Street %><br />
                        <%: Model.Request.ShipToAddress.City %>,
                        <%: Model.Request.ShipToAddress.State %>
                        <%: Model.Request.ShipToAddress.Zip %>
                    </li>
                    <li><strong>Phone:</strong>&nbsp;<%: Model.Request.ShipToAddress.Phone %></li>
                    <li><strong>Fax:</strong>&nbsp;<%: Model.Request.ShipToAddress.Fax %></li>
                </ul>
                <!-- 
                            <asp:View ID="View_OffSite" runat="server">
                                <h2>Service/Repair Site</h2>
                                <p>Off site at vendor&#39;s location.</p>
                            </asp:View>
                            <asp:View ID="View_Pickup" runat="server">
                                <h2>No Shipping Required</h2>
                                <p>The requester will pick up items.</p>
                            </asp:View>
                            -->
            </fieldset>
        </div>
        <div class="details-col">
            <fieldset id="col-3">
                <legend>Request Details</legend>
                <ul>
                    <li><strong>Shipping:</strong>&nbsp;<%: Model.Request.ShippingMethod.ShippingMethodName %></li>
                    <li><strong>Okay to Backorder:</strong>&nbsp;<%: Model.Request.OkayToBackorder ? "Yes" : "No" %></li>
                    <% if (!String.IsNullOrEmpty(Model.Request.DafisDoc))
                       { %>
                    <li><strong>DaFIS Doc:</strong>&nbsp;<%: Model.Request.DafisDoc %></li>
                    <% } %>
                    <% if (!String.IsNullOrEmpty(Model.Request.DafisPO))
                       { %>
                    <li><strong>DaFIS PO:</strong>&nbsp;<%: Model.Request.DafisPO %></li>
                    <% } %>
                    <% if (!String.IsNullOrEmpty(Model.Request.VendorConfirmationNum))
                       { %>
                    <li><strong>Vendor Confirmation #:</strong>&nbsp;<%: Model.Request.VendorConfirmationNum%></li>
                    <% } %>
                    <% var purchaser = User.Profile(Model.Request.PurchaserId); %>
                    <li><strong>Purchasing Agent:</strong>&nbsp;<%: purchaser.FirstName + " " + purchaser.LastName %></li>
                    <li><strong>Approver(s):</strong>
                        <ul>
                            <% foreach (var approval in Model.Request.PiApprovals)
                               { %>
                            <% var approver = User.Profile(approval.PiId); %>
                            <li class="<%: approval.Approval.ToString().ToLower() %>" title='<%: approval.Approval ? "Approved by " + approver.FirstName + " " + approver.LastName : "Awaiting approval by " + approver.FirstName + " " + approver.LastName %>'>
                                <img src='<%= Url.Content("~/Content/Img/Fugue/" + (approval.Approval ? "tick-small" : "clock-small") + ".png") %>' alt='' />
                                <%: approver.FirstName + " " + approver.LastName %></li>
                            <% } %>
                        </ul>
                    </li>
                    <li><strong>Requested Account(s):</strong>&nbsp;
                        <%: Model.Request.RequestedAccount %></li>
                    <li><strong>Account(s):</strong>
                        <ul>
                            <% foreach (var approval in Model.Request.PiApprovals)
                               { %>
                            <% var approver = User.Profile(approval.PiId); %>
                            <li class="<%: approval.Approval.ToString().ToLower() %>" title='<%: approval.Approval ? "Approved by " + approver.FirstName + " " + approver.LastName : "Awaiting approval by " + approver.FirstName + " " + approver.LastName %>'>
                                <img src='<%= Url.Content("~/Content/Img/Fugue/" + (approval.Approval ? "tick-small" : "clock-small") + ".png") %>' alt='' />
                                <%: approver.FirstName + " " + approver.LastName %></li>
                            <% } %>
                        </ul>
                    </li>
                </ul>
            </fieldset>
        </div>
        <br class="clear" />
        <br />
        <fieldset>
            <legend>History<a name="history"></a></legend>
            <table cellspacing="0" class="tbl detailsTbl">
                <tr>
                    <th style="width: 350px; padding-left: 10px;">
                        Action / Event
                    </th>
                    <th>
                        Comments
                    </th>
                </tr>
                <% index = 1; %>
                <% foreach (var historyItem in Model.HistoryItems)
                   { %>
                <tr class="<%= index % 2 == 0 ? String.Empty : "alt " %><%= historyItem.IsMilestone ? "milestone" : String.Empty %>">
                    <td>
                        <img src="<%= Url.Content("~/Content/Img/Fugue/"+ historyItem.ImageName +".png") %>" style="float: left; margin: 0 5px;" title="<%= historyItem.Description %>" alt="<%= historyItem.Description %>" />
                        <%= historyItem.SubmittedOn.ToString("M/d/yy") %>
                        at
                        <%= historyItem.SubmittedOn.ToString("h:mm tt")%>:
                        <%= historyItem.Description %>
                        by
                        <%= historyItem.SubmittedBy %>
                    </td>
                    <td>
                        <%: historyItem.Comments %>
                    </td>
                </tr>
                <% index++; %>
                <% } %>
            </table>
        </fieldset>
        <br />
        <fieldset class="last">
            <legend>Comments &amp; Attachments<a name="comments"></a></legend>
            <%: Html.SpriteCtrl("Add comments or attachments", HtmlHelpers.CtrlType.Link, "balloon--plus").Attribute("id","add-comments") %>
            <% Html.Telerik().Window()
                    .Visible(false)
                    .Name("AddCommentsWindow")
                    .Title("Add comments")
                    .Icon(Url.Content("~/Content/Img/Fugue/balloon--plus.png"), "Comments")
                    .Content(() =>
                    { %>
            <iframe id="addcomments" src="<%= Url.Action("_AddComment", "purchasing", new { requestId = Model.Request.Id, requestUniqueId = Model.Request.UniqueId }) %>" style="width: 100%; height: 100%;" frameborder="0"></iframe>
            <% })
                    .Draggable(true)
                    .Buttons(b => b.Close())
                    .Scrollable(false)
                    .Width(500)
                    .Height(400)
                    .Modal(true)
                    .Render();
            %>
            <!-- COMMENTS -->
            <% if (Model.Request.RequestComments.Any())
               { %>
            <ul class="comments-list">
                <% var comments = Model.Request.RequestComments.OrderByDescending(c => c.SubmittedOn); %>
                <% foreach (var comment in comments)
                   { %>
                <li class="<%= comment.SubmittedBy == User.Identity.Name ? "owner" : String.Empty %><%= comment.Equals(comments.Last()) ? " last" : String.Empty %>">
                    <div class="comment-date">
                        <%= comment.SubmittedOn.ToString("M/d/yy @h:mm tt")%>
                        (<%= comment.SubmittedOn.ToRelativeTime()%>)
                    </div>
                    <p><strong>
                        <% var author = User.Profile(comment.SubmittedBy); %>
                        <% if (author != null)
                           { %>
                        <%: author.FirstName%>
                        <%: author.LastName%>
                        <% }
                           else
                           { %>
                        <%: comment.SubmittedBy%>
                        <% } %></strong> said:
                        <%: !String.IsNullOrEmpty(comment.Comments) ? comment.Comments : "N/A" %>
                    </p>
                    <!-- ATTACHMENTS -->
                    <% if (comment.Attachments.Any())
                       { %>
                    <ul>
                        <% foreach (OrAdmin.Entities.Purchasing.Attachment attachment in comment.Attachments)
                           {  %>
                        <li><a href="<%= ResolveClientUrl(OrAdmin.Core.Helpers.FileHelper.GetRelativeFilePath(OrAdmin.Core.Enums.App.Applications.ApplicationName.Purchasing, attachment.FileName, attachment.SubmittedBy, attachment.SubmittedOn)) %>">
                            <%: attachment.FileName %>
                        </a></li>
                        <% } %>
                    </ul>
                    <% } %>
                    <!-- END ATTACHMENTS -->
                </li>
                <% } %>
            </ul>
            <% }
               else
               { %>
            <p style="margin: 0; font-style: italic;">No comments to display.</p>
            <% } %>
            <!-- END COMMENTS -->
        </fieldset>
        <!-- REQUEST END -->
    </div>
    <div class="roundedBoxBottom">
        <span></span>
    </div>
</div>
