﻿using System.Web.Mvc;
using UCDArch.Web.Controller;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// Controller for the Ping class
    /// </summary>
    public class PingController : SuperController
    {

        //
        // GET: /Ping/
        public ActionResult Index()
        {
            return View();
        }

    }

}
