using System;

namespace Purchasing.Core.Helpers
{
    public static class StringExtensions
    {
        public static string Summarize(this string str, int maxLength = 25)
        {
            if (str == null)
            {
                return null;
            }
            return str.Length < maxLength ? str : string.Format("{0}...", str.Substring(0, maxLength - 3));
        }

        public static string SafeTruncate(this string value, int max)
        {
           
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            value = value.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " "); //remove new lines. Might not be needed but if AE doesn't fix there stuff, we should. And it makes sense to do it here.

            value = value.Trim();
            
            if(value.Length <= max)
            {
                return value;
            }

            if (max <= 0)
            {
                return String.Empty;
            }

            return value.Substring(0, max);
        }
    }
}