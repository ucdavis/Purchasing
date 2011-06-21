<%@ Page Title="" Language="C#" MasterPageFile="~/Shared/Master/Default.Master" Inherits="System.Web.Mvc.ViewPage<OrAdmin.Web.Areas.Business.Models.Purchasing.AdminViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Title" runat="server">
    AdminHome
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <link href="<%= Url.Content("~/Content/Css/Min/Purchasing.css") %>" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        <%= Model.Unit.UnitAbbreviation %>
        Requests</h1>
    <p class="desc">A listing of
        <%= Model.Unit.UnitAbbreviation %>
        requests. Click "Process" to view the request details.</p>
    <% Html.Telerik().Grid(Model.Requests)
        .Name("requests")
        .Columns(
            column =>
            {
                // Details
                column.Template(request =>
                {
    %>
    <%: Html.SpriteCtrl("Process", HtmlHelpers.CtrlType.Link, "gear--pencil", "Process this request", null, "myrequestdetails", "purchasing", new { requestUniqueId = request.UniqueId })%>
    <%
        }).HtmlAttributes(new { style = "width: 80px; text-align: center;" });
                // ID
                column.Bound(request => request.FriendlyUniqueId).Title("ID");
                // Vendor - Items
                column.Bound(request => request.VendorId).Template(request =>
                { %>
    <% string items = " - "; %>
    <% int count = 0; %>
    <% while (count < request.DpoItems.Count && count < 2)
       { %>
    <% items += request.DpoItems[count].Description + " "; %>
    <% count++; %>
    <% } %>
    <div style="white-space: normal; overflow: hidden;">
        <div style="white-space: nowrap; overflow: hidden; width: 2000%;">
            <div style="white-space: normal; overflow: hidden;" title="<%: request.Vendor.FriendlyName + items %>">
                <%: request.Vendor.FriendlyName %>
                <span class="itemList">
                    <%: items %></span>
            </div>
        </div>
    </div>
    <% 
        }).Title("Vendor <span class=\"itemList\">- Items</span>").Filterable(false);
                // Total
                column.Bound(request => request.FinalTotal).Template(request =>
                { %>
    <%= request.FinalTotal.HasValue ? request.FinalTotal.Value.ToString("C2") : "<span style=\"color: #999; font-style: italic;\">~" + (request.DpoItems.Sum(i=>i.Quantity * i.PricePerUnit) * ((OrAdmin.Core.Settings.GlobalSettings.PurchasingTax / 100) + 1)).Value.ToString("C2") + "</span>" %>
    <% }).Title("Total");
                // Approved
                column.Template(request =>
                { 
    %>
    <% if (request.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue >= (int)OrAdmin.Repositories.Purchasing.MilestoneMapRepository.MilestoneType.PIApproved)
       { %>
    <% OrAdmin.Entities.Purchasing.MilestoneMap map = request.MilestoneMaps.OrderBy(m => m.SubmittedOn).Where(m => m.MilestoneValue == (int)OrAdmin.Repositories.Purchasing.MilestoneMapRepository.MilestoneType.PIApproved).First(); %>
    <img src="/Content/Img/Fugue/tick.png" style="vertical-align: middle;" alt="Y" title="Approved on <%= map.SubmittedOn.ToString("MMMM d, yyyy h:mm tt") %> by <%= OrAdmin.Core.Helpers.ProfileHelper.GetNameOrUsername(map.SubmittedBy, OrAdmin.Core.Helpers.ProfileHelper.NameFormat.FirstSpaceLast) %>" />
    <% }
       else
       { %>
    -
    <% } %>
    <%
        }).Title("Approved")
          .HtmlAttributes(new { style = "text-align: center;" })
          .HeaderHtmlAttributes(new { style = "text-align: center;" });
                // Placed
                column.Template(request =>
                { 
    %>
    <% if (request.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().MilestoneValue >= (int)OrAdmin.Repositories.Purchasing.MilestoneMapRepository.MilestoneType.Placed)
       { %>
    <% OrAdmin.Entities.Purchasing.MilestoneMap map = request.MilestoneMaps.OrderBy(m => m.SubmittedOn).Where(m => m.MilestoneValue == (int)OrAdmin.Repositories.Purchasing.MilestoneMapRepository.MilestoneType.Placed).First(); %>
    <img src="/Content/Img/Fugue/tick.png" style="vertical-align: middle;" alt="Y" title="Placed on <%= map.SubmittedOn.ToString("MMMM d, yyyy h:mm tt") %> by <%= OrAdmin.Core.Helpers.ProfileHelper.GetNameOrUsername(map.SubmittedBy, OrAdmin.Core.Helpers.ProfileHelper.NameFormat.FirstSpaceLast) %>" />
    <% }
       else
       { %>
    -
    <% } %>
    <%
        }).Title("Placed")
          .HtmlAttributes(new { style = "text-align: center;" })
          .HeaderHtmlAttributes(new { style = "text-align: center;" });
                // Current status
                column.Template(request =>
                {
    %>
    <% OrAdmin.Entities.Purchasing.Milestone milestone = request.MilestoneMaps.OrderBy(m => m.SubmittedOn).First().Milestone; %>
    <img src="/Content/Img/Fugue/<%: milestone.ImageName %>.png" style="vertical-align: middle;" alt="<%: milestone.Description %>" title="<%: milestone.Description %>" />
    <span style="vertical-align: middle;">
        <%: milestone.Description %></span>
    <%
        }).HtmlAttributes(new { style = "white-space: nowrap;" })
        .Title("Current status");
            }
        )
        .Pageable(pager => pager.PageSize(20))
        .Filterable()
        .Sortable()
        .Render();
    %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="QuickLinks" runat="server">
    <div class="other-links">
        <h2>Filter by</h2>
        <% if (Model.FilterList.Any())
           { %>
        <ul>
            <% foreach (OrAdmin.Repositories.Purchasing.RequestRepository.AdminRequestFilterListItem filter in Model.FilterList)
               { %>
            <% if (filter.Selected)
               { %>
            <li class="here">
                <% }
               else
               { %>
                <li>
                    <% } %>
                    <%= Html.ActionLink(String.Format("{0} ({1})", filter.Filter.ToString().Replace("_", " "), filter.RequestCount), "AdminFiltered", new { filter = filter.Filter.ToString().ToLower() })%></li>
                <% } %>
        </ul>
        <% } %>
    </div>
</asp:Content>
