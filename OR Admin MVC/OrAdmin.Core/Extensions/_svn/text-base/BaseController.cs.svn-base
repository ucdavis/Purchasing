using System;
using System.Web.Mvc;
using OrAdmin.Core.Enums.App;
using Telerik.Web.Mvc;

namespace OrAdmin.Core.Extensions
{
    [PopulateSiteMap(SiteMapName = "SiteMap", ViewDataKey = "SiteMap")]
    public abstract class BaseController : Controller
    {
        public BaseController()
        {
            // Add global contoller logic here
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // OnActionExecuted has access to Session, ViewData, Request etc...

            SiteMapBase siteMap = SiteMapManager.SiteMaps[GlobalProperty.App.SiteMap.ToString()];
            ViewData[GlobalProperty.App.CurrentNode.ToString()] = GetCurrentNode(siteMap.RootNode); // Store in ViewData
            base.OnActionExecuted(filterContext);
        }

        private SiteMapNode GetCurrentNode(SiteMapNode node)
        {
            // Remove extraneous slashes
            string path = this.Request.Path;
            if (path.EndsWith("/"))
                path = path.Substring(0, path.LastIndexOf("/"));

            // Handle requests for "/"... return root node
            if (path.Length == 0)
                return node;

            // Compare the node's url to the current url
            if (this.Url.Action(node.ActionName, node.ControllerName, node.RouteValues).Equals(path, StringComparison.OrdinalIgnoreCase))
            {
                // Match!
                return node;
            }

            // Recurse through childnodes
            foreach (SiteMapNode child in node.ChildNodes)
            {
                SiteMapNode match = GetCurrentNode(child);
                if (match != null)
                    return match;
            }

            // No matches found...
            return null;
        }
    }
}
