using System.Web;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace OrAdmin.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Encode(this string input)
        {
            return HttpContext.Current.Server.HtmlEncode(input);
        }

        public static string UrlEncode(this string input)
        {
            return HttpContext.Current.Server.UrlEncode(input);
        }

        public static string FormatBytes(this long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }
            return "0 Bytes";
        }

        /// <summary>
        /// Returns URL/SEO friendly slug
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Slug(this string text)
        {
            var sb = new StringBuilder();

            text = text.Replace("-", " ");

            foreach (char c in text)
            {
                if (!char.IsPunctuation(c))
                    sb.Append(c);
            }

            text = sb.ToString();
            text = RemoveExtaSpaces(text);

            text = text.Replace(" ", "-");
            text = text.Replace("~", "");
            text = text.Replace("`", "");
            text = text.Replace("^", "");
            text = text.Replace("+", "");
            text = text.Replace("=", "");
            text = text.Replace("|", "");
            text = text.Replace("<", "");
            text = text.Replace(">", "");
            text = text.Replace(".", "");
            text = text.Replace("&", "and");
            text = text.Replace("%", "");
            text = text.Replace("$", "");
            text = text.Replace(":", "");
            text = text.Replace("*", "");
            text = text.Replace("\"", "");
            text = text.Replace("\\", "");

            return String.IsNullOrEmpty(text) ? "untitled" : text.ToLower();
        }

        public static string RemoveExtaSpaces(this string text)
        {
            Regex regex = new Regex(@"\s{2,}", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
            return regex.Replace(text.Trim(), " ");
        }
    }
}
