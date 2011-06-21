<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<% 
    var navAuthorization = Telerik.Web.Mvc.Infrastructure.ServiceLocator.Current.Resolve<Telerik.Web.Mvc.Infrastructure.INavigationItemAuthorization>();
    var siteMap = Telerik.Web.Mvc.SiteMapManager.SiteMaps[OrAdmin.Core.Enums.App.GlobalProperty.App.SiteMap.ToString()];
    var currentNode = ViewData[OrAdmin.Core.Enums.App.GlobalProperty.App.CurrentNode.ToString()] as Telerik.Web.Mvc.SiteMapNode;
    IList<Telerik.Web.Mvc.SiteMapNode> sectionNodes;

    if (currentNode.Attributes.Any(a => a.Key == "section") && currentNode.ChildNodes.Any())
        sectionNodes = currentNode.ChildNodes.Where(s => s.IsAccessible(navAuthorization, ViewContext) && s.Visible).ToList();
    else if (siteMap.RootNode.ChildNodes.Where(c => c.Attributes.Any(a => a.Key == "section")).Any())
        sectionNodes = siteMap.RootNode.ChildNodes.Where(s => s.Attributes.Any(a => a.Key == "section") && s.IsAccessible(navAuthorization, ViewContext) && s.Visible).ToList();
    else
        sectionNodes = null;
%>

<% if (sectionNodes.Any())
   { %>
<ul>
    <% var counter = 0; %>
    <% foreach (Telerik.Web.Mvc.SiteMapNode section in sectionNodes)
       { %>
    <li title='<%: section.Attributes["description"].ToString() %>' onclick='document.location.href = "<%: Url.Action(section.ActionName, section.ControllerName, section.RouteValues) %>";' style='background-image: url(/Content/Img/SectionIcons/<%: section.Attributes["icon"].ToString() %>.png);'>
        <p><a href='<%: Url.Action(section.ActionName, section.ControllerName, section.RouteValues) %>'><strong>
            <%: section.Title %></strong></a><br />
            <%: section.Attributes["description"].ToString() %></p>
    </li>
    <% counter++; %>
    <% } %>
    <% while (counter % 3 != 0)
       { %>
    <li class="blank">&nbsp;</li>
    <% counter++; %>
    <% } %>
</ul>
<% } %>