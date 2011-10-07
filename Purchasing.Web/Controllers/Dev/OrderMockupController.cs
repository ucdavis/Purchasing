

using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using System.Web;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the OrderMockup class
    /// </summary>
    public class OrderMockupController : ApplicationController
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepositoryWithTypedId<SubAccount, Guid> _subAccountRepository;

        public OrderMockupController(IRepository<Order> orderRepository, IRepositoryWithTypedId<SubAccount, Guid> subAccountRepository )
        {
            _orderRepository = orderRepository;
            _subAccountRepository = subAccountRepository;
        }

        //
        // GET: /OrderMockup/
        public ActionResult Index()
        {
            return View();
        }


        public new ActionResult Request()
        {
            ViewBag.Units = Repository.OfType<UnitOfMeasure>().GetAll();
            ViewBag.Accounts = Repository.OfType<WorkgroupAccount>().Queryable.Select(x=>x.Account).ToList();
            ViewBag.Vendors = Repository.OfType<WorkgroupVendor>().GetAll();
            ViewBag.Approvers =
                Repository.OfType<WorkgroupPermission>().Queryable.Where(x => x.Role.Id == Role.Codes.Approver).Select(
                    x => x.User).ToList();
            ViewBag.AccountManagers =
                Repository.OfType<WorkgroupPermission>().Queryable.Where(x => x.Role.Id == Role.Codes.AccountManager).Select(
                    x => x.User).ToList();

            return View();
        }

        public new ActionResult LineItems()
        {
            ViewBag.Units = Repository.OfType<UnitOfMeasure>().GetAll();
            ViewBag.Accounts = Repository.OfType<WorkgroupAccount>().Queryable.Select(x => x.Account).ToList();

            return View();
        }

        public JsonNetResult SearchKfsAccounts(string searchTerm)
        {
            var results = Repository.OfType<Account>().Queryable.Where(a => a.Id.Contains(searchTerm) || a.Name.Contains(searchTerm)).Select(a => new {Id=a.Id, Name=a.Name}).ToList();
            return new JsonNetResult(results);
        }

        public JsonNetResult SearchSubAccounts(string accountNumber)
        {
            var results = _subAccountRepository.Queryable.Where(a => a.AccountNumber == accountNumber).Select(a => new {Id=a.SubAccountNumber, Name=a.SubAccountNumber}).ToList();
            return new JsonNetResult(results);
        }

        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult AddVendor()
        {
            return Json(new {id = new Random().Next(100)});
        }
        
        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult AddAddress()
        {
            return Json(new { id = new Random().Next(100) });
        }

        /// <summary>
        /// Testing fileupload with chunked uploads
        /// TODO: Note here we are just reading to a memorystream and throwing the data away
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult Upload()
        {
            var request = ControllerContext.HttpContext.Request;

            /*
            const long maxAllowedUploadLength = 4*(1000000);

            if (maxAllowedUploadLength < request.ContentLength) //TODO: this is never displayed because the request just fails
            {
                return Json(new {error = "The max file upload size is 4MB"});
            }
             */

            try
            {
                var buffer = new byte[4096];
                using (var stream = new MemoryStream())//TODO: eventually want to write into the DB
                {
                    //while (request.InputStream.Read(buffer,0,buffer.Length) != 0){ }

                    int bytesRead = 0;
                    do
                    {
                        bytesRead = request.InputStream.Read(buffer, 0, buffer.Length);
                        stream.Write(buffer, 0, bytesRead);
                    } while (bytesRead > 0);
                }
            }
            catch
            {
                // TODO: Return/Log error?
                return new JsonResult();
            }

            return Json(new {success = true});
        }
    }
}
