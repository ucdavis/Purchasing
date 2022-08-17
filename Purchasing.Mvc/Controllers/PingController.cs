using UCDArch.Web.Controller;
using Microsoft.AspNetCore.Mvc;

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
