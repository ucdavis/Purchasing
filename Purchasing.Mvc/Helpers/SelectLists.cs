using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Purchasing.Mvc.Helpers
{
    public static class SelectLists
    {
        public static List<SelectListItem> PostStatusList = new List<SelectListItem>()
        {
            new SelectListItem() {Text = "", Value = ""},
            new SelectListItem() {Text = "Awaiting User", Value = "Awaiting User"},
            new SelectListItem() {Text = "Receiving", Value = "Receiving"},
            new SelectListItem() {Text = "Paying", Value = "Paying"},
            new SelectListItem() {Text = "Complete", Value = "Complete"},
        };

    }
}