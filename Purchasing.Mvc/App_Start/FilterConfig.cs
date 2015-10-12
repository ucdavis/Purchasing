using System.Web;
using System.Web.Mvc;
using Purchasing.Mvc.Attributes;

namespace Purchasing.Mvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleAndLogErrorAttribute());
        }
    }
}
