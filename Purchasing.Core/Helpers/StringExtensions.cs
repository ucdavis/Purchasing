namespace Purchasing.Core.Helpers
{
    public static class StringExtensions
    {
        public static string Summarize(this string str, int maxLength = 25)
        {
            return str.Length < maxLength ? str : string.Format("{0}...", str.Substring(0, maxLength - 3));
        }
    }
}