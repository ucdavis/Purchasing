using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.WS;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Test class
    /// </summary>
    public class TestController : Controller
    {
        private readonly IAzureStorageService _azureStorageService;

        public TestController(IAzureStorageService azureStorageService)
        {
            _azureStorageService = azureStorageService;
        }

        public ActionResult Index()
        {
            var username = "Opp";
            var password = "NtQj9EeW6LlyRol3VL4m";
            var servername = "qb5fm1u0eb";
            var blobStorageKey = "zx1TiwX6jAdl3uzXivE4vjmDjz81iPIfrQnV0tmYglDlAKfLylL9XXb7qAEZn+2S3q8TfGAhelHQQPVyPLZ4UQ==";

            _azureStorageService.BackupDatabase(username, password, servername, blobStorageKey);

            return View();
        }
    }
}
