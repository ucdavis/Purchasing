using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using OrAdmin.Core.Enums.App;
using OrAdmin.Core.Settings;
using Telerik.Web.Mvc;

namespace OrAdmin.Core.Extensions
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString SpriteCtrl(this HtmlHelper helper, string text)
        {
            return SpriteCtrl(helper, text, CtrlType.Button, null);
        }

        public static MvcHtmlString SpriteCtrl(this HtmlHelper helper, string text, CtrlType type)
        {
            return SpriteCtrl(helper, text, type, null);
        }

        public static MvcHtmlString SpriteCtrl(this HtmlHelper helper, string text, CtrlType type, string imageName)
        {
            return SpriteCtrl(helper, text, type, imageName, null);
        }

        public static MvcHtmlString SpriteCtrl(this HtmlHelper helper, string text, CtrlType type, string imageName, string toolTip)
        {
            return SpriteCtrl(helper, text, type, imageName, toolTip, null);
        }

        public static MvcHtmlString SpriteCtrl(this HtmlHelper helper, string text, CtrlType type, string imageName, string toolTip, string inputName)
        {
            return SpriteCtrl(helper, text, type, imageName, toolTip, inputName, null);
        }

        public static MvcHtmlString SpriteCtrl(this HtmlHelper helper, string text, CtrlType type, string imageName, string toolTip, string inputName, string actionName)
        {
            return SpriteCtrl(helper, text, type, imageName, toolTip, inputName, actionName, null, null);
        }

        public static MvcHtmlString SpriteCtrl(this HtmlHelper helper, string text, CtrlType type, string imageName, string toolTip, string inputName, string actionName, string controllerName, object routeValues)
        {
            return SpriteCtrl(helper, text, type, imageName, toolTip, inputName, actionName, controllerName, routeValues, ImagePosition.Left, null, false);
        }

        public static MvcHtmlString SpriteCtrl(this HtmlHelper helper, string text, CtrlType type, string imageName, string toolTip, string inputName, string actionName, string controllerName, object routeValues, ImagePosition? imagePosition, ButtonColor? buttonColor, bool? last)
        {
            UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            // Build wrapper
            TagBuilder wrapBuilder = new TagBuilder("div");
            wrapBuilder.AddCssClass("sprite");
            wrapBuilder.MergeAttribute("title", String.IsNullOrEmpty(toolTip) ? text : toolTip);

            if (type == CtrlType.Button)
                wrapBuilder.AddCssClass("sprite-btn");
            else if (type == CtrlType.Link)
                wrapBuilder.AddCssClass("sprite-link");

            if (last.HasValue && last.Value)
                wrapBuilder.AddCssClass("sprite-last");

            if (buttonColor != null)
            {
                switch (buttonColor)
                {
                    case ButtonColor.Green: wrapBuilder.AddCssClass("sprite-btn-green"); break;
                    case ButtonColor.Red: wrapBuilder.AddCssClass("sprite-btn-red"); break;
                    case ButtonColor.Gold: wrapBuilder.AddCssClass("sprite-btn-gold"); break;
                    default: break;
                }
            }

            // Add image
            if (!String.IsNullOrEmpty(imageName))
            {
                // Add image class to wrapper
                if (imagePosition.HasValue)
                {
                    if (type == CtrlType.Button)
                        wrapBuilder.AddCssClass(imagePosition.Value == ImagePosition.Right ? "sprite-btn-img-right" : "sprite-btn-img-left");
                    else if (type == CtrlType.Link)
                        wrapBuilder.AddCssClass(imagePosition.Value == ImagePosition.Right ? "sprite-link-img-right" : "sprite-link-img-left");
                }

                // Insert image
                TagBuilder imgBuilder = new TagBuilder("img");
                string imgUrl = urlHelper.Content(GlobalSettings.IconImageUrl + (imageName.EndsWith(".png") ? imageName : imageName + ".png"));
                imgBuilder.MergeAttribute("src", imgUrl);
                imgBuilder.MergeAttribute("alt", text);
                wrapBuilder.InnerHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);
            }

            TagBuilder linkBuilder = new TagBuilder("a");
            linkBuilder.InnerHtml = text;

            // Link or input?
            if (!String.IsNullOrEmpty(actionName))
                linkBuilder.MergeAttribute("href", urlHelper.Action(actionName, controllerName, routeValues));
            else
            {
                TagBuilder inputBuilder = new TagBuilder("input");
                inputBuilder.MergeAttribute("type", "submit");
                if (!String.IsNullOrEmpty(inputName))
                    inputBuilder.MergeAttribute("name", inputName);
                inputBuilder.MergeAttribute("value", text);
                wrapBuilder.InnerHtml += inputBuilder.ToString(TagRenderMode.SelfClosing);
            }

            wrapBuilder.InnerHtml += linkBuilder.ToString(TagRenderMode.Normal);

            // Wrap and return
            return MvcHtmlString.Create(wrapBuilder.ToString(TagRenderMode.Normal));
        }

        public enum CtrlType
        {
            Button,
            Link
        }

        public enum ImagePosition
        {
            Left, // Default
            Right
        }

        public enum ButtonColor
        {
            Green,
            Red,
            Gold
        }

        public static MvcHtmlString SpriteSeparator(this HtmlHelper helper)
        {
            return SpriteSeparator(helper, null);
        }

        public static MvcHtmlString SpriteSeparator(this HtmlHelper helper, string text)
        {
            TagBuilder tb = new TagBuilder("div") { InnerHtml = "&nbsp;" };
            tb.AddCssClass("sprite-sep");

            if (!String.IsNullOrEmpty(text))
            {
                tb.AddCssClass("sprite-sep-text");
                tb.InnerHtml = helper.Encode(text);
            }

            return MvcHtmlString.Create(tb.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Builds a list of parent pages
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static MvcHtmlString BreadcrumbNav(this HtmlHelper helper)
        {
            // Get the current node
            SiteMapNode currentNode = helper.ViewContext.ViewData[GlobalProperty.App.CurrentNode.ToString()] as SiteMapNode;

            if (currentNode != null)
            {
                UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
                string linkFormat = "<a href=\"{1}\" title=\"{0}\">{0}</a>";
                string hereFormat = "<span title=\"{0}\">{0}</span>";
                StringBuilder sb = new StringBuilder();

                // Get the parent nodes of the current node (if any)
                List<SiteMapNode> parents = GetParentNodes(new List<SiteMapNode>(), currentNode);
                parents.Reverse();

                // Loop through parents
                foreach (SiteMapNode node in parents)
                {
                    if (parents.IndexOf(node) != parents.Count - 1)
                    {
                        // Not the current node, append a link
                        sb.Append(String.Format(linkFormat, helper.Encode(node.Title), urlHelper.Action(node.ActionName, node.ControllerName, node.RouteValues)));
                        sb.Append("&nbsp;&rsaquo;&nbsp;");
                    }
                    else
                        // Current node, append the "here" format
                        sb.Append(String.Format(hereFormat, helper.Encode(node.Title)));
                }

                return MvcHtmlString.Create(sb.ToString());
            }
            else
                // Not found, return a warning message
                return MvcHtmlString.Create("Could not find this page in the sitemap or the controller does not inherit from BaseController");
        }

        private static List<SiteMapNode> GetParentNodes(List<SiteMapNode> parents, SiteMapNode node)
        {
            // Add the starting node to the set
            if (!parents.Contains(node))
                parents.Add(node);

            // Recurse through parent nodes
            if (node.Parent != null && !parents.Contains(node.Parent))
            {
                parents.Add(node.Parent);
                GetParentNodes(parents, node.Parent);
            }

            // Finally, return list of parents
            return parents;
        }

        /// <summary>
        /// Constructs the secondary "section" nav in the left-sidebar
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="levelsToRender"></param>
        /// <returns></returns>
        public static MvcHtmlString SecondaryNav(this HtmlHelper helper)
        {
            // Get the current node
            SiteMapNode currentNode = helper.ViewContext.ViewData[GlobalProperty.App.CurrentNode.ToString()] as SiteMapNode;
            if (currentNode != null)
            {
                SiteMapNode startingNode = GetParentNodes(new List<SiteMapNode>(), currentNode)
                    .Find(
                        delegate(SiteMapNode e)
                        {
                            return e.Attributes[SiteMapNodeAttribute.SiteMapAttribute.section.ToString()] != null && Convert.ToBoolean(e.Attributes[SiteMapNodeAttribute.SiteMapAttribute.section.ToString()]);
                        });

                if (startingNode != null)
                {
                    string linkFormat = "<a href=\"{1}\" title=\"{0}\">{0}</a>";
                    string hereClass = "here";

                    if (currentNode.ChildNodes.Any())
                    {
                        StringBuilder nav = new StringBuilder(String.Format("<h2>{0}</h2>", "In this section"));
                        
                        nav.Append(MvcHtmlString.Create(
                            BuildSecondayNav(
                                helper,
                                new UrlHelper(helper.ViewContext.RequestContext),
                                currentNode,
                                startingNode,
                                linkFormat,
                                hereClass,
                                true,
                                startingNode.Attributes[SiteMapNodeAttribute.SiteMapAttribute.secondaryLevelsToRender.ToString()] != null ?
                                Convert.ToInt32(startingNode.Attributes[SiteMapNodeAttribute.SiteMapAttribute.secondaryLevelsToRender.ToString()]) :
                                -1
                            )));

                        return MvcHtmlString.Create(nav.ToString());
                    }
                    else
                        return MvcHtmlString.Empty;
                }
            }

            // Not found, return a warning message
            return MvcHtmlString.Create("<ul><li><a>Could not find this page in the sitemap or the controller does not inherit from BaseController</a></li></ul>");
        }

        private static string BuildSecondayNav(HtmlHelper helper, UrlHelper urlHelper, SiteMapNode currentNode, SiteMapNode startingNode, string linkFormat, string hereClass, bool includeSectionRoot, int levelsToRender)
        {
            StringBuilder sb = new StringBuilder();
            var navAuthorization = Telerik.Web.Mvc.Infrastructure.ServiceLocator.Current.Resolve<Telerik.Web.Mvc.Infrastructure.INavigationItemAuthorization>();

            // If the starting node is visible/accessible/included OR there are any child nodes
            if (
                (startingNode.Visible &&
                navAuthorization.IsAccessibleToUser(helper.ViewContext.RequestContext, startingNode) &&
                includeSectionRoot) ||
                startingNode.ChildNodes.Any(n => navAuthorization.IsAccessibleToUser(helper.ViewContext.RequestContext, n) && n.Visible)
                )
            {
                sb.AppendLine("<ul>");

                // Add section root to list?
                if (includeSectionRoot)
                {
                    sb.AppendFormat("\t\t");

                    // Add here class for current node
                    TagBuilder tb = new TagBuilder("li");
                    if (startingNode.Equals(currentNode))
                        tb.AddCssClass(hereClass);

                    sb.Append(tb.ToString(TagRenderMode.StartTag));
                    sb.Append(String.Format(linkFormat, helper.Encode(startingNode.Title), urlHelper.Action(startingNode.ActionName, startingNode.ControllerName, startingNode.RouteValues)));
                    sb.AppendLine("</li>");
                }

                levelsToRender--;

                // Add child nodes to list
                
                foreach (SiteMapNode child in startingNode.ChildNodes.Where(n => navAuthorization.IsAccessibleToUser(helper.ViewContext.RequestContext, n) && n.Visible && (n.ChildNodes.Count == 0 || n.Parent.Parent != null || n.ChildNodes.Any(c => c.Visible && navAuthorization.IsAccessibleToUser(helper.ViewContext.RequestContext, c)))))
                {
                    sb.AppendFormat("\t\t");

                    // Add here class for current node (and parent nodes)
                    TagBuilder tb = new TagBuilder("li");
                    if (child.Equals(currentNode) || child.ChildNodes.Contains(currentNode))
                        tb.AddCssClass(hereClass);

                    sb.Append(tb.ToString(TagRenderMode.StartTag));
                    sb.Append(String.Format(linkFormat, helper.Encode(child.Title), urlHelper.Action(child.ActionName, child.ControllerName, child.RouteValues)));

                    // Recurse through child nodes
                    if (levelsToRender != 0)
                        sb.Append(BuildSecondayNav(helper, urlHelper, currentNode, child, linkFormat, hereClass, false, levelsToRender));

                    sb.AppendLine("</li>");
                }

                sb.AppendFormat("\t");
                sb.AppendFormat("    ");
                sb.AppendLine("</ul>");
            }

            return sb.ToString();
        }
    }
}
