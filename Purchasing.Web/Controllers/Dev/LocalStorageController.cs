using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;

namespace Purchasing.Web.Controllers.Dev
{
    /// <summary>
    /// Controller for the LocalStorage class
    /// </summary>
    public class LocalStorageController : ApplicationController
    {
        public new ActionResult Request()
        {
            ViewBag.Units = Repository.OfType<UnitOfMeasure>().GetAll();
            ViewBag.Accounts = Repository.OfType<WorkgroupAccount>().Queryable.Select(x => x.Account).ToList();
            ViewBag.Vendors = Repository.OfType<WorkgroupVendor>().GetAll();
            ViewBag.Addresses = Repository.OfType<WorkgroupAddress>().GetAll();
            ViewBag.ShippingTypes = Repository.OfType<ShippingType>().GetAll();
            ViewBag.Approvers =
                Repository.OfType<WorkgroupPermission>().Queryable.Where(x => x.Role.Id == Role.Codes.Approver).Select(
                    x => x.User).ToList();
            ViewBag.AccountManagers =
                Repository.OfType<WorkgroupPermission>().Queryable.Where(x => x.Role.Id == Role.Codes.AccountManager).Select(
                    x => x.User).ToList();

            return View();
        }

        /// <summary>
        /// Ajax call to search for any commodity codes, match by name
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public JsonNetResult SearchCommodityCodes(string searchTerm)
        {
            var results =
                Repository.OfType<Commodity>().Queryable.Where(c => c.Name.Contains(searchTerm)).Select(a => new { a.Id, a.Name }).ToList();
            return new JsonNetResult(results);
        }

        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult AddVendor()
        {
            return Json(new { id = new Random().Next(100) });
        }

        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult AddAddress()
        {
            return Json(new { id = new Random().Next(100) });
        }

        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult UploadFile()
        {
            return Json(new { success = true, id = Guid.NewGuid() });
        }
    }
}
