using System.Text;

namespace System.Web.Mvc.Html
{
    public static class ValidationSummaryExtension
    {
        /// <summary>
        /// Creates a validation summary with a container element
        /// surrounding the summary and error messages.
        /// </summary>        
        public static string ValidationSummaryWithContainer(this HtmlHelper ext)
        {
            return ValidationSummaryWithContainer(ext, null);
        }

        /// <summary>
        /// Creates a validation summary with a container element
        /// surrounding the summary and error messages.
        /// </summary>        
        public static string ValidationSummaryWithContainer(this HtmlHelper ext, string message)
        {
            MvcHtmlString summaryoutput = ext.ValidationSummary(message);
            if (summaryoutput != null && !String.IsNullOrEmpty(summaryoutput.ToString()))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<div class=\"validation-summary-errors-container\">");
                sb.Append(summaryoutput);
                sb.Append("</div>");
                return sb.ToString();
            }
            else
                return string.Empty;
        }
    }
}
