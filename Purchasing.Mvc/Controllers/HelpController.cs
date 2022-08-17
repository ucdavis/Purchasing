﻿using System;
using Purchasing.Core.Helpers;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Services;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// help controller
    /// </summary>
    /// <remarks>
    /// </remarks>
    [HandleTransactionsManually]
    public class HelpController : ApplicationController
    {
        

        public ActionResult Index()
        {
            return View();
        }

    }
}