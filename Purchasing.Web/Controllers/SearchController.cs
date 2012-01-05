using System;
using System.Linq;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Search class
    /// </summary>
    [Authorize]
    public class SearchController : ApplicationController
    {
        public SearchController()
        {
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(string id)
        {
            return View();
        }

    }
	
}
