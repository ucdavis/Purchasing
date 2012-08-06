using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Purchasing.Web.Helpers
{
    /// <summary>
    /// Extension methods for the TimeSpan.
    /// </summary>
    public static class TimeSpanExtensions
    {

        /// <summary>
        /// Converts the value of TimeSpan into a string which reads in wordy explanation of
        /// the parts of the TimeSpan. For example, it could read "39 days, 23 hours, 12 minutes, 34 seconds".
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>Returns the wordy version of the value of the TimeSpan.</returns>
        public static string ToLongString(this TimeSpan value)
        {
            StringBuilder txt = new StringBuilder();

            if (value.Hours <= 0)
            {
                txt.Append("Less than an hour");
                return txt.ToString();
            }
            if (value.Days > 0)
            {
                txt.AppendFormat(", {0} day", value.Days);
                if (value.Days > 1)
                {
                    txt.Append("s");
                }
                value = value.Subtract(new TimeSpan(value.Days, 0, 0, 0));
            }
            if (value.Hours > 0)
            {
                txt.AppendFormat(", {0} hour", value.Hours);
                if (value.Hours > 1)
                {
                    txt.Append("s");
                }
                value = value.Subtract(new TimeSpan(0, value.Hours, 0, 0));
            }
            //if (value.Minutes > 0)
            //{
            //    txt.AppendFormat(", {0} minute", value.Minutes);
            //    if (value.Minutes > 1)
            //    {
            //        txt.Append("s");
            //    }
            //    value = value.Subtract(new TimeSpan(0, 0, value.Minutes, 0));
            //}
            //if (value.Seconds > 0)
            //{
            //    txt.AppendFormat(", {0} second", value.TotalSeconds);
            //    if (value.Seconds > 1)
            //    {
            //        txt.Append("s");
            //    }
            //}

            // Remove the leading ", ".
            if (txt.Length > 0)
            {
                txt.Remove(0, 2);
            }

            // Return the result.
            return txt.ToString();
        }

    }
}