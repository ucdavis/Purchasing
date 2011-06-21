using System;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Web;

namespace OrAdmin.Core.Settings
{
    public static class GlobalSettings
    {
        public static string SSL
        {
            get
            {
                return GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string ProgrammerLogin
        {
            get
            {
                return GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string ProgrammerName
        {
            get
            {
                return GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string ProgrammerEmail
        {
            get
            {
                return GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string Ownership
        {
            get
            {
                if (HttpContext.Current.Cache["ownership"] != null)
                    return HttpContext.Current.Cache["ownership"].ToString();
                else
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(String.Format("<a href=\"{0}\" title=\"{1}\">", OwnershipLogoUrl, OwnershipLogoAlt));
                    sb.AppendLine(String.Format("<img src=\"{0}\" alt=\"{1}\" />", OwnershipLogoPath, OwnershipLogoAlt));
                    sb.AppendLine("</a>");
                    sb.AppendLine(String.Format("<p>{0}</p>", OwnershipAddressHtml));
                    sb.AppendLine(String.Format("<p>{0}</p>", OwnershipContactHtml));

                    HttpContext.Current.Cache["ownership"] = sb.ToString();
                    return sb.ToString();
                }
            }
        }

        public static string OwnershipLogoUrl
        {
            get
            {
                return GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string OwnershipLogoPath
        {
            get
            {
                return GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string OwnershipLogoAlt
        {
            get
            {
                return GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string OwnershipAddressHtml
        {
            get
            {
                return GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string OwnershipContactHtml
        {
            get
            {
                return GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string LogoutUrl
        {
            get
            {
                return GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string CASHost
        {
            get
            {
                return GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string SystemFromAddress
        {
            get
            {
                return GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string SystemTitle
        {
            get
            {
                return GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name);
            }
        }

        public static string IconImageUrl
        {
            get
            {
                string path = GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name);
                return path.EndsWith("/") ? path : path + "/";
            }
        }

        public static decimal PurchasingTax
        {
            get
            {
                return Convert.ToDecimal(GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name));
            }
        }

        public static int PurchasingInitialItemRows
        {
            get
            {
                return Convert.ToInt32(GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name));
            }
        }

        public static int PurchasingMaxItemRows
        {
            get
            {
                return Convert.ToInt32(GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name));
            }
        }

        public static int PurchasingMaxApprovers
        {
            get
            {
                return Convert.ToInt32(GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name));
            }
        }

        public static decimal PurchasingMaxUploadMB
        {
            get
            {
                return Convert.ToDecimal(GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name));
            }
        }

        public static string UploadPath
        {
            get
            {
                string path = GetSettingFromCacheOrConfig(MethodBase.GetCurrentMethod().Name);
                return path.EndsWith("/") ? path : path + "/";
            }
        }

        private static string GetSettingFromCacheOrConfig(string settingName)
        {
            settingName = TrimMethodName(settingName);

            try
            {
                if (HttpContext.Current.Cache[settingName] == null)
                    HttpContext.Current.Cache[settingName] = ConfigurationManager.AppSettings[settingName];

                return HttpContext.Current.Cache[settingName].ToString();
            }
            catch (Exception)
            {
                throw new Exception("Setting: " + settingName + " not defined in Settings.config");
            }
        }

        private static string TrimMethodName(this string methodName)
        {
            return methodName.Substring(4);
        }
    }
}
