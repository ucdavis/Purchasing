

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

        public ActionResult VendorSearch()
        {
            ViewBag.Vendors = Repository.OfType<WorkgroupVendor>().GetAll();

            return View();
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

        public ActionResult ReadOnly()
        {
            var workgroup = Repository.OfType<Workgroup>().Queryable.FirstOrDefault();
            var address = workgroup.Addresses.FirstOrDefault();
            var vendor = workgroup.Vendors.FirstOrDefault();
            var orderType = Repository.OfType<OrderType>().Queryable.FirstOrDefault();
            var shippingType = Repository.OfType<ShippingType>().Queryable.FirstOrDefault();
            var user = Repository.OfType<User>().Queryable.Where(a => a.Id == "anlai").FirstOrDefault();
            var user2 = Repository.OfType<User>().Queryable.Where(a => a.Id == "postit").FirstOrDefault();
            var requester = Repository.OfType<OrderStatusCode>().Queryable.Where(a => a.Id == "RQ").FirstOrDefault();
            var accountmgr = Repository.OfType<OrderStatusCode>().Queryable.Where(a => a.Id == "AM").FirstOrDefault();
            var approver = Repository.OfType<OrderStatusCode>().Queryable.Where(a => a.Id == "AP").FirstOrDefault();

            var order = new Order()
                            {
                                Justification = "I want to place this order because i need some stuff.",
                                OrderType = orderType,
                                Vendor = vendor,
                                Address = address,
                                Workgroup = workgroup,
                                Organization = workgroup.PrimaryOrganization,
                                ShippingType = shippingType,

                                DateNeeded = DateTime.Now.AddDays(5),
                                AllowBackorder = false,

                                ShippingAmount = 19.99m,
                                EstimatedTax = 8.89m,

                                CreatedBy = user,
                                StatusCode = accountmgr
                            };


            // add in some line itesm
            var line1 = new LineItem() {Quantity = 1, UnitPrice = 2.99m, Unit = "each", Description = "pencils"};
            var line2 = new LineItem() {Quantity = 3, UnitPrice = 17.99m, Unit = "dozen", Description = "pen", Url = "http://fake.com/product1", Notes = "I want the good pens."};
            order.AddLineItem(line1);
            order.AddLineItem(line2);

            // add in some tracking
            var tracking1 = new OrderTracking() { DateCreated = DateTime.Now.AddDays(-2), Description = "Create", StatusCode = requester, User = user };
            var tracking2 = new OrderTracking() { DateCreated = DateTime.Now.AddDays(-1), Description = "Accepted", StatusCode = approver, User = user2 };
            order.AddTracking(tracking1);
            order.AddTracking(tracking2);

            // add in commeents
            var comment1 = new OrderComment() { DateCreated = DateTime.Now, Text = "this order is necessary for me to do my work.", User = user };
            order.AddOrderComment(comment1);

            return View(order);
        }
    }
}
