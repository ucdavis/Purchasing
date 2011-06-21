using System.Web.Mvc;
using OrAdmin.Core.Extensions;

namespace OrAdmin.Web.Areas.System.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        //
        // GET: /System/Home/

        public ActionResult Index()
        {
            return View();
        }

    }
}
