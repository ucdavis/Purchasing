﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Purchasing.Mvc.Helpers
{
    public static class SelectLists
    {
        public static List<SelectListItem> PostStatusList = new List<SelectListItem>()
        {
            new SelectListItem() {Text = "", Value = ""},
            new SelectListItem() {Text = "Awaiting Client information", Value = "Awaiting Client information"},
            new SelectListItem() {Text = "Awaiting Contracting Services", Value = "Awaiting Contracting Services"},
            new SelectListItem() {Text = "Awaiting Vendor information", Value = "Awaiting Vendor information"},
            new SelectListItem() {Text = "Awaiting Invoice", Value = "Awaiting Invoice"},
            new SelectListItem() {Text = "Awaiting receiving", Value = "Awaiting receiving"},
            new SelectListItem() {Text = "Partial payment", Value = "Partial payment"},
            new SelectListItem() {Text = "Paid in full ", Value = "Paid in full "}
        };

        public static List<SelectListItem> RoleList = new List<SelectListItem>()
        {
            
            new SelectListItem() {Text = "Purchaser", Value = "Purchaser"},
            new SelectListItem() {Text = "Approver", Value = "Approver"},
            new SelectListItem() {Text = "Account Manager", Value = "AccountManager"},
        };

    }
}