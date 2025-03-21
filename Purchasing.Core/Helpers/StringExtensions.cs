﻿using System;
using System.Text.RegularExpressions;

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
            value = value.ReplaceLineEndings(" ");
            //replace multiple spaces with single space
            value = System.Text.RegularExpressions.Regex.Replace(value, @"\s+", " ");

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="regEx"></param>
        /// <returns></returns>
        public static string SafeRegexRemove(this string value, string regEx = @"[^a-zA-Z0-9_\-@#&() ]")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            try
            {
                return Regex.Replace(value, regEx, string.Empty);
            }
            catch (Exception)
            {
                return value;
            }


        }
    }
}