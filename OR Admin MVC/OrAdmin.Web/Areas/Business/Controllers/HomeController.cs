using System.Web.Mvc;
using OrAdmin.Core.Attributes.Authorization;
using OrAdmin.Core.Extensions;

namespace OrAdmin.Web.Areas.Business.Controllers
{
    [BusinessUser]
    public class HomeController : BaseController
    {
        //
        // GET: /Business/Home/
        public ActionResult Index()
        {
            return View();
        }
    }
}
