using System.Web.Mvc;

namespace OrAdmin.Core.Extensions
{
    public static class HtmlHelperExtensions
    {
        private const string EndTag = ">";

        public static MvcHtmlString Attribute(this MvcHtmlString htmlString, string attributeName, string attributeValue)
        {
            if (string.IsNullOrEmpty(attributeValue) || string.IsNullOrEmpty(attributeName))
                return htmlString;

            string html = htmlString.ToString();
            int endTagIndex = html.IndexOf(EndTag);
            if (endTagIndex > -1)
                html = html.Insert(endTagIndex, string.Format(" {0}=\"{1}\" ", attributeName, attributeValue));

            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString Disabled(this MvcHtmlString htmlString, bool isDisabled)
        {
            string html = htmlString.ToString();
            if (isDisabled)
            {
                int endTagIndex = html.IndexOf(EndTag);
                if (endTagIndex > -1)
                    html = html.Insert(endTagIndex, " disabled=\"disabled\" ");
            }

            return MvcHtmlString.Create(html);
        }
    }

}
