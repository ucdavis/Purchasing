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
    }
}