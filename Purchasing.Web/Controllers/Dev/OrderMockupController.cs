

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
using System.Text;
using Purchasing.Core;
using System.Globalization;
using AutoMapper;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the OrderMockup class
    /// </summary>
    public class OrderMockupController : ApplicationController
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepositoryWithTypedId<SubAccount, Guid> _subAccountRepository;
        private readonly IRepositoryFactory _repositoryFactory;

        public OrderMockupController(IRepository<Order> orderRepository, IRepositoryWithTypedId<SubAccount, Guid> subAccountRepository, IRepositoryFactory repositoryFactory)
        {
            _orderRepository = orderRepository;
            _subAccountRepository = subAccountRepository;
            _repositoryFactory = repositoryFactory;
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
            ViewBag.Addresses = Repository.OfType<WorkgroupAddress>().GetAll();
            ViewBag.Approvers =
                Repository.OfType<WorkgroupPermission>().Queryable.Where(x => x.Role.Id == Role.Codes.Approver).Select(
                    x => x.User).ToList();
            ViewBag.AccountManagers =
                Repository.OfType<WorkgroupPermission>().Queryable.Where(x => x.Role.Id == Role.Codes.AccountManager).Select(
                    x => x.User).ToList();

            return View();
        }

        [HttpPost]
        [BypassAntiForgeryToken] //TODO: implement the token
        public new ActionResult Request(OrderViewModel model)
        {
            //TODO: no validation will be done!

            var workgroup = _repositoryFactory.WorkgroupRepository.Queryable.First(); //TODO: assocaite with the proper workgroup

            var order = new Order
            {
                Vendor = _repositoryFactory.WorkgroupVendorRepository.GetById(model.Vendor),
                Address = _repositoryFactory.WorkgroupAddressRepository.GetById(model.ShipAddress),
                ShippingType = Repository.OfType<ShippingType>().Queryable.First(),//TODO: add shipping type to the order form
                DateNeeded = model.DateNeeded,
                AllowBackorder = model.AllowBackorder,
                EstimatedTax = decimal.Parse(model.Tax.TrimEnd('%')),
                Workgroup = workgroup,
                Organization = workgroup.PrimaryOrganization, //why is this needed?
                ShippingAmount = decimal.Parse(model.Shipping.TrimStart('$')),
                OrderType = Repository.OfType<OrderType>().Queryable.First(), //TODO: why needed?
                CreatedBy = _repositoryFactory.UserRepository.GetById(CurrentUser.Identity.Name),
                StatusCode = Repository.OfType<OrderStatusCode>().Queryable.Where(x => x.Id == OrderStatusCode.Codes.Approver).Single()
            };

            //Add in line items
            foreach (var lineItem in model.Items)
            {
                if (lineItem.IsValid())
                {
                    //TODO: could use automapper later, but need to do validation
                    order.AddLineItem(new LineItem
                                          {
                                              CatalogNumber = lineItem.CatalogNumber,
                                              Commodity = null, //TODO: add in commodity codes
                                              Description = lineItem.Description,
                                              Notes = lineItem.Notes,
                                              Quantity = int.Parse(lineItem.Quantity), //TODO: quanity should maybe not be an int?
                                              Unit = lineItem.Units, //TODO: shouldn't this link to UOM?
                                              UnitPrice = decimal.Parse(lineItem.Price),
                                              Url = lineItem.Url
                                          });
                }
            }

            return Content("check it out");
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

        public ActionResult ReadOnly(OrderSampleType type = OrderSampleType.Normal)
        {
            var order = CreateFakeOrder(type);

            return View(order);
        }

        public enum OrderSampleType
        {
            Normal = 0, OrderSplit, LineItemSplit
        }

        private Order CreateFakeOrder(OrderSampleType type)
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

            var accts = Repository.OfType<Account>().Queryable.Where(a => a.SubAccounts.Count > 0).Take(4).ToList();

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

            var line1 = new LineItem() { Quantity = 1, UnitPrice = 2.99m, Unit = "each", Description = "pencils" };
            var line2 = new LineItem() { Quantity = 3, UnitPrice = 17.99m, Unit = "dozen", Description = "pen", Url = "http://fake.com/product1", Notes = "I want the good pens." };
            order.AddLineItem(line1);
            order.AddLineItem(line2);

            var split1 = new Split() { Account = accts[0].Id, SubAccount = accts[0].SubAccounts.First().SubAccountNumber, Project = "ARG11", Amount = 1m };
            var split2 = new Split() {Account = accts[1].Id, Amount = 1.99m};
            var split3 = new Split() {Account = accts[2].Id, Amount = 17.99m};
            var split4 = new Split() { Account = accts[3].Id, SubAccount = accts[3].SubAccounts.First().SubAccountNumber, Amount = 40m };

            switch(type)
            {
                case OrderSampleType.Normal:

                    split1.Amount = order.Total();
                    order.AddSplit(split1);

                    break;
                case OrderSampleType.OrderSplit:

                    order.AddSplit(split1);
                    order.AddSplit(split2);

                    break;
                case OrderSampleType.LineItemSplit:

                    // add in some line itesm
                    
                    line1.AddSplit(split1);
                    line1.AddSplit(split2);
                    
                    line2.AddSplit(split3);
                    line2.AddSplit(split4);


                    order.AddSplit(line1.Splits[0]);
                    order.AddSplit(line1.Splits[1]);
                    order.AddSplit(line2.Splits[0]);
                    order.AddSplit(line2.Splits[1]);

                    break;
            }

            // add in some tracking
            var tracking1 = new OrderTracking() { DateCreated = DateTime.Now.AddDays(-2), Description = "Order was submitted by " + user.FullName, StatusCode = requester, User = user };
            var tracking2 = new OrderTracking() { DateCreated = DateTime.Now.AddDays(-1), Description = string.Format("Order was accepted by {0} at {1} review level.", user2.FullName, approver.Name), StatusCode = approver, User = user2 };
            order.AddTracking(tracking1);
            order.AddTracking(tracking2);

            // add in commeents
            var comment1 = new OrderComment() { DateCreated = DateTime.Now, Text = "this order is necessary for me to do my work.", User = user };
            order.AddOrderComment(comment1);

            return order;
        }
    }

    public class OrderViewModel
    {
        public SplitTypes SplitType { get; set; }
        public string Justification { get; set; }
        public int Vendor { get; set; }
        public string ShipTo { get; set; }
        public string ShipEmail { get; set; }
        public int ShipAddress { get; set; }

        public string Shipping { get; set; }
        public string Freight { get; set; }
        public string Tax { get; set; }

        public LineItem[] Items { get; set; }
        public Split[] Splits { get; set; }

        public string Approvers { get; set; }
        public string AccountManagers { get; set; }

        public RestrictedOrder Restricted { get; set; }

        public string Backorder { get; set; }
        public bool AllowBackorder { get { return Backorder == "on"; } }

        public DateTime? DateNeeded { get; set; }
        public string Comments { get; set; }
        
        public class Split
        {
            public int? LineItemId { get; set; }
            public string Account { get; set; }
            public string SubAccount { get; set; }
            public string Project { get; set; }
            public string Amount { get; set; }
            public string Percent { get; set; }
        }

        public class LineItem
        {
            public int Id { get; set; }
            public string Quantity { get; set; }
            public string Price { get; set; }
            public string Units { get; set; }
            public string CatalogNumber { get; set; }
            public string Description { get; set; }
            public string CommodityCode { get; set; }//TODO: int???
            public string Url { get; set; }
            public string Notes { get; set; }

            /// <summary>
            /// A line item is valid if it has a price and quantity
            /// TODO: is that true?
            /// </summary>
            /// <returns></returns>
            public bool IsValid()
            {
                return !string.IsNullOrWhiteSpace(Quantity) && !string.IsNullOrWhiteSpace(Price);
            }
        }

        public class RestrictedOrder
        {
            public bool IsRestricted { get { return Status == "on"; } }
            public string Status { get; set; }
            public string Rua { get; set; }
            public string Class { get; set; }
            public string Use { get; set; }
            public string StorageSite { get; set; }
            public string Custodian { get; set; }
            public string Users { get; set; }
        }

        public enum SplitTypes
        {
            Order,
            Line,
            None
        }
    }
}
