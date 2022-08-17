
using System;

namespace Purchasing.Mvc.Controllers
{
    public static class ControllerExtensions
    {
        public static string ControllerName(this Type type)
        {
            return type.Name.Replace("Controller", "");
        }
    }
}