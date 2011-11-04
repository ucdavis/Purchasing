

using System;
using System.Collections.Generic;
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
using Purchasing.Web.Services;
using System.Reflection;

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
        private readonly IOrderService _orderService;
        private readonly IOrderAccessService _orderAccessService;

        public OrderMockupController(IRepository<Order> orderRepository, 
            IRepositoryWithTypedId<SubAccount, Guid> subAccountRepository, 
            IRepositoryFactory repositoryFactory, IOrderService orderService,
            IOrderAccessService orderAccessService)
        {
            _orderRepository = orderRepository;
            _subAccountRepository = subAccountRepository;
            _repositoryFactory = repositoryFactory;
            _orderService = orderService;
            _orderAccessService = orderAccessService;
        }

        private Workgroup CurrentWorkgroup { get { return _repositoryFactory.WorkgroupRepository.Queryable.First(); } }

        //
        // GET: /OrderMockup/
        public ActionResult Index()
        {
            return View();
        }


        public new ActionResult Request()
        {
            var model = new OrderModifyModel
                            {
                                Order = new Order(),
                                Units = Repository.OfType<UnitOfMeasure>().GetAll(),
                                Accounts =
                                    Repository.OfType<WorkgroupAccount>().Queryable.Select(x => x.Account).ToList(),
                                Vendors = Repository.OfType<WorkgroupVendor>().GetAll(),
                                Addresses = Repository.OfType<WorkgroupAddress>().GetAll(),
                                ShippingTypes = Repository.OfType<ShippingType>().GetAll(),
                                Approvers =
                                    Repository.OfType<WorkgroupPermission>().Queryable.Where(
                                        x => x.Role.Id == Role.Codes.Approver).Select(
                                            x => x.User).ToList(),
                                AccountManagers =
                                    Repository.OfType<WorkgroupPermission>().Queryable.Where(
                                        x => x.Role.Id == Role.Codes.AccountManager).Select(
                                            x => x.User).ToList(),
                                ConditionalApprovals =
                                    CurrentWorkgroup.AllConditioanlApprovals
                            };

            return View(model);
        }

        [HttpPost]
        [BypassAntiForgeryToken] //TODO: implement the token
        public new ActionResult Request(OrderViewModel model)
        {
            //TODO: no validation will be done!
            var order = new Order();
            
            BindOrderModel(order, model, includeLineItemsAndSplits: true);

            _orderService.CreateApprovalsForNewOrder(order, accountId: model.Account, approverId: model.Approvers, accountManagerId: model.AccountManagers, conditionalApprovalIds: model.ConditionalApprovals);

            _orderRepository.EnsurePersistent(order); //TODO: we are just saving the order and not doing any approvals

            return RedirectToAction("ReadOnly", new {id = order.Id});
        }

        /// <summary>
        /// Edit the given order
        /// </summary>
        public ActionResult Edit(int id)
        {
            var model = new OrderModifyModel {Order = _orderRepository.GetNullableById(id)};

            Check.Require(model.Order != null);

            if (model.Order.HasControlledSubstance)
            {
                model.ControlledSubstanceInformation =
                    _repositoryFactory.ControlledSubstanceInformationRepository.Queryable.Where(
                        x => x.Order.Id == model.Order.Id).Single();
            }

            /*
            model.Splits = _repositoryFactory.SplitRepository.Queryable.Where(x => x.Order.Id == id).ToList();
            model.LineItems = _repositoryFactory.LineItemRepository.Queryable.Where(x => x.Order.Id == id).ToList();

            if (model.Splits.Any(x => x.LineItem != null))
            {
                model.SplitType = OrderViewModel.SplitTypes.Line;
            }
            else
            {
                model.SplitType = model.Splits.Count() == 1
                                      ? OrderViewModel.SplitTypes.None
                                      : OrderViewModel.SplitTypes.Order;
            }
             */

            model.Units = Repository.OfType<UnitOfMeasure>().GetAll();
            model.Accounts = Repository.OfType<WorkgroupAccount>().Queryable.Select(x => x.Account).ToList();
            model.Vendors = Repository.OfType<WorkgroupVendor>().GetAll();
            model.Addresses = Repository.OfType<WorkgroupAddress>().GetAll();
            model.ShippingTypes = Repository.OfType<ShippingType>().GetAll();
            model.Approvers =
                Repository.OfType<WorkgroupPermission>().Queryable.Where(x => x.Role.Id == Role.Codes.Approver).Select(
                    x => x.User).ToList();
            model.AccountManagers =
                Repository.OfType<WorkgroupPermission>().Queryable.Where(x => x.Role.Id == Role.Codes.AccountManager).Select(
                    x => x.User).ToList();

            return View(model);
        }

        [HttpPost]
        [BypassAntiForgeryToken] //TODO: implement the token
        public ActionResult Edit(int id, OrderViewModel model)
        {
            var order = _repositoryFactory.OrderRepository.GetNullableById(id);

            Check.Require(order != null);

            var adjustRouting = model.AdjustRouting.HasValue && model.AdjustRouting.Value;
            
            BindOrderModel(order, model, includeLineItemsAndSplits: adjustRouting);

            if (adjustRouting)
            {
                _orderService.ReRouteApprovalsForExistingOrder(order);
            }
            else
            {
                _orderService.EditExistingOrder(order);
            }

            _orderRepository.EnsurePersistent(order);

            Message = "Order Edited!";

            return RedirectToAction("ReadOnly", new {id});
        }
        
        /// <summary>
        /// Test page for testing the javascript run during an order edit load
        /// </summary>
        public ActionResult Test()
        {
            var order = CreateFakeOrder(OrderSampleType.Normal);

            var model = new OrderModifyModel
                            {
                                Order = order,
                                Units = Repository.OfType<UnitOfMeasure>().GetAll(),
                                Accounts =
                                    Repository.OfType<WorkgroupAccount>().Queryable.Select(x => x.Account).ToList(),
                                Vendors = Repository.OfType<WorkgroupVendor>().GetAll(),
                                Addresses = Repository.OfType<WorkgroupAddress>().GetAll(),
                                ShippingTypes = Repository.OfType<ShippingType>().GetAll(),
                                Approvers =
                                    Repository.OfType<WorkgroupPermission>().Queryable.Where(
                                        x => x.Role.Id == Role.Codes.Approver).Select(
                                            x => x.User).ToList(),
                                AccountManagers =
                                    Repository.OfType<WorkgroupPermission>().Queryable.Where(
                                        x => x.Role.Id == Role.Codes.AccountManager).Select(
                                            x => x.User).ToList()
                            };


            return View(model);
        }

        public ActionResult LocalStorage()
        {
            var order = CreateFakeOrder(OrderSampleType.Normal);

            var model = new OrderModifyModel
            {
                Order = order,
                Units = Repository.OfType<UnitOfMeasure>().GetAll(),
                Accounts =
                    Repository.OfType<WorkgroupAccount>().Queryable.Select(x => x.Account).ToList(),
                Vendors = Repository.OfType<WorkgroupVendor>().GetAll(),
                Addresses = Repository.OfType<WorkgroupAddress>().GetAll(),
                ShippingTypes = Repository.OfType<ShippingType>().GetAll(),
                Approvers =
                    Repository.OfType<WorkgroupPermission>().Queryable.Where(
                        x => x.Role.Id == Role.Codes.Approver).Select(
                            x => x.User).ToList(),
                AccountManagers =
                    Repository.OfType<WorkgroupPermission>().Queryable.Where(
                        x => x.Role.Id == Role.Codes.AccountManager).Select(
                            x => x.User).ToList()
            };

            return View(model);
        }

        public class OrderModifyModel
        {
            public OrderModifyModel()
            {
                ControlledSubstanceInformation = new ControlledSubstanceInformation();
                
            }

            public Order Order { get; set; }
            public OrderViewModel.SplitTypes SplitType { get; set; }
            public IList<LineItem> LineItems { get; set; }
            public IList<Split> Splits { get; set; }
            public ControlledSubstanceInformation ControlledSubstanceInformation { get; set; }
            public IList<UnitOfMeasure> Units { get; set; }
            public IList<Account> Accounts { get; set; }
            public IList<WorkgroupVendor> Vendors { get; set; }
            public IList<WorkgroupAddress> Addresses { get; set; }
            public IList<ShippingType> ShippingTypes { get; set; }
            public IList<ConditionalApproval> ConditionalApprovals { get; set; }
            public IList<User> Approvers { get; set; }
            public IList<User> AccountManagers { get; set; }
            public bool IsNewOrder { get { return Order.IsTransient(); } }
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

        public JsonNetResult GetLineItems(int id)
        {
            var lineItems = _repositoryFactory.LineItemRepository
                .Queryable
                .Where(x => x.Order.Id == id)
                .Select(
                    x =>
                    new OrderViewModel.LineItem
                        {
                            CatalogNumber = x.CatalogNumber,
                            CommodityCode = x.Commodity.Id,
                            Description = x.Description,
                            Id = x.Id,
                            Notes = x.Notes,
                            Price = x.UnitPrice.ToString(),
                            Quantity = x.Quantity.ToString(),
                            Units = x.Unit,
                            Url = x.Url
                        });

            return new JsonNetResult(new {id, lineItems});
        }

        public JsonNetResult GetSplits(int id)
        {
            var splits = _repositoryFactory.SplitRepository
                .Queryable
                .Where(x => x.Order.Id == id)
                .Select(
                    x =>
                    new OrderViewModel.Split
                        {
                            Account = x.Account,
                            Amount = x.Amount.ToString(),
                            LineItemId = x.LineItem == null ? 0 : x.LineItem.Id,
                            Project = x.Project,
                            SubAccount = x.SubAccount
                        });

            OrderViewModel.SplitTypes splitType;

            if (splits.Any(x => x.LineItemId != 0))
            {
                splitType = OrderViewModel.SplitTypes.Line;
            }
            else
            {
                splitType = splits.Count() == 1
                                      ? OrderViewModel.SplitTypes.None
                                      : OrderViewModel.SplitTypes.Order;
            }

            return new JsonNetResult(new {id, splits, splitType = splitType.ToString()});
        }

        public JsonNetResult GetLineItemsAndSplits(int id)
        {
            var lineItems = _repositoryFactory.LineItemRepository
                .Queryable
                .Where(x => x.Order.Id == id)
                .Select(
                    x =>
                    new OrderViewModel.LineItem
                    {
                        CatalogNumber = x.CatalogNumber,
                        CommodityCode = x.Commodity.Id,
                        Description = x.Description,
                        Id = x.Id,
                        Notes = x.Notes,
                        Price = x.UnitPrice.ToString(),
                        Quantity = x.Quantity.ToString(),
                        Units = x.Unit,
                        Url = x.Url
                    })
                .ToList();

            var splits = _repositoryFactory.SplitRepository
                .Queryable
                .Where(x => x.Order.Id == id)
                .Select(
                    x =>
                    new OrderViewModel.Split
                    {
                        Account = x.Account,
                        Amount = x.Amount.ToString(),
                        LineItemId = x.LineItem == null ? 0 : x.LineItem.Id,
                        Project = x.Project,
                        SubAccount = x.SubAccount
                    })
                .ToList();

            OrderViewModel.SplitTypes splitType;

            if (splits.Any(x => x.LineItemId != 0))
            {
                splitType = OrderViewModel.SplitTypes.Line;
            }
            else
            {
                splitType = splits.Count() == 1
                                      ? OrderViewModel.SplitTypes.None
                                      : OrderViewModel.SplitTypes.Order;
            }

            return new JsonNetResult(new {id, lineItems, splits, splitType = splitType.ToString()});
        }

        public JsonNetResult GetTestLineItemsAndSplits()
        {
            var order = CreateFakeOrder(OrderSampleType.LineItemSplit);

            var lineItems = order.LineItems
                .Select(
                    x =>
                    new OrderViewModel.LineItem
                    {
                        CatalogNumber = x.CatalogNumber,
                        //CommodityCode = x.Commodity.Id,
                        Description = x.Description,
                        Id = x.Id,
                        Notes = x.Notes,
                        Price = x.UnitPrice.ToString(),
                        Quantity = x.Quantity.ToString(),
                        Units = x.Unit,
                        Url = x.Url
                    })
                .ToList();

            var splits = order.Splits
                .Select(
                    x =>
                    new OrderViewModel.Split
                    {
                        Account = x.Account,
                        Amount = x.Amount.ToString(),
                        LineItemId = x.LineItem == null ? 0 : x.LineItem.Id,
                        Project = x.Project,
                        SubAccount = x.SubAccount
                    })
                .ToList();

            const OrderViewModel.SplitTypes splitType = OrderViewModel.SplitTypes.Line;

            return new JsonNetResult(new {id = 0, lineItems, splits, splitType = splitType.ToString()});
        }

        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult AddVendor(WorkgroupVendor vendor)
        {
            var workgroup = CurrentWorkgroup;
            
            workgroup.AddVendor(vendor);

            _repositoryFactory.WorkgroupRepository.EnsurePersistent(workgroup);

            return Json(new {id = vendor.Id});
        }
        
        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult AddAddress(WorkgroupAddress workgroupAddress)
        {
            var workgroup = CurrentWorkgroup;

            workgroup.AddAddress(workgroupAddress);

            _repositoryFactory.WorkgroupRepository.EnsurePersistent(workgroup);

            return Json(new {id = workgroupAddress.Id});
        }

        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult UploadFile()
        {
            var request = ControllerContext.HttpContext.Request;
            var qqFile = request["qqfile"];

            var attachment = new Attachment
                                 {
                                     DateCreated = DateTime.Now,
                                     User = GetCurrentUser(),
                                     FileName = qqFile,
                                     ContentType = request.Headers["X-File-Type"]
                                 };

            //TODO: IE 9 doesn't work, it tries to intercept the ajax POST for some reason.
            if (String.IsNullOrEmpty(qqFile)) // IE
            {
                Check.Require(request.Files.Count > 0, "No file provided to upload method");
                var file = request.Files[0];

                attachment.FileName = Path.GetFileNameWithoutExtension(file.FileName) +
                    Path.GetExtension(file.FileName).ToLower();

                attachment.ContentType = file.ContentType;
            }

            using (var binaryReader = new BinaryReader(request.InputStream))
            {
                attachment.Contents = binaryReader.ReadBytes((int) request.InputStream.Length);
            }

            _repositoryFactory.AttachmentRepository.EnsurePersistent(attachment);

            return Json(new {success = true, id = attachment.Id});
        }

        public ActionResult ReadOnly(int id = 0, OrderSampleType type = OrderSampleType.Normal)
        {
            var order = id == 0 ? CreateFakeOrder(type) : _orderRepository.GetById(id);

            var status = _orderAccessService.GetAccessLevel(order);

            ViewBag.CanEdit = status == OrderAccessLevel.Edit;

            return View(order);
        }

        /// <summary>
        /// Approve an order in the context of a specific user
        /// </summary>
        [HttpPost]
        public ActionResult Approve(int id /*order*/)
        {
            var order = _repositoryFactory.OrderRepository.Queryable.Fetch(x => x.Approvals).Where(x => x.Id == id).Single();

            _orderService.Approve(order);

            _repositoryFactory.OrderRepository.EnsurePersistent(order); //Save approval changes

            Message = "Order Approved by " + CurrentUser.Identity.Name;
            return RedirectToAction("Index", "JamesTest");
        }

        /// <summary>
        /// TODO: move this to a new controller, check permissions
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public FileResult ViewFile(Guid fileId)
        {
            var attachment = _repositoryFactory.AttachmentRepository.GetNullableById(fileId);

            Check.Require(attachment != null);

            return File(attachment.Contents, attachment.ContentType, attachment.FileName);
        }

        public enum OrderSampleType
        {
            Normal = 0, OrderSplit, LineItemSplit
        }

        private void BindOrderModel(Order order, OrderViewModel model, bool includeLineItemsAndSplits = false)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.Queryable.First(); //TODO: assocaite with the proper workgroup

            //TODO: automapper?
            order.Vendor = _repositoryFactory.WorkgroupVendorRepository.GetById(model.Vendor);
            order.Address = _repositoryFactory.WorkgroupAddressRepository.GetById(model.ShipAddress);
            order.ShippingType = _repositoryFactory.ShippingTypeRepository.GetById(model.ShippingType);
            order.DateNeeded = model.DateNeeded;
            order.AllowBackorder = model.AllowBackorder;
            order.Workgroup = workgroup;
            order.Organization = workgroup.PrimaryOrganization; //why is this needed?
            order.DeliverTo = model.ShipTo;
            order.DeliverToEmail = model.ShipEmail;
            order.OrderType = order.OrderType ?? _repositoryFactory.OrderTypeRepository.GetById(OrderType.Types.OrderRequest);
            order.CreatedBy = _repositoryFactory.UserRepository.GetById(CurrentUser.Identity.Name);
            order.Justification = model.Justification;

            if (!string.IsNullOrWhiteSpace(model.Comments))
            {
                var comment = new OrderComment
                {
                    DateCreated = DateTime.Now,
                    User = _repositoryFactory.UserRepository.GetById(CurrentUser.Identity.Name),
                    Text = model.Comments
                };

                order.AddOrderComment(comment);
            }
            
            if (model.FileIds != null)
            {
                foreach (var fileId in model.FileIds)
                {
                    order.AddAttachment(_repositoryFactory.AttachmentRepository.GetById(fileId));
                }
            }

            if (model.Restricted.IsRestricted)
            {
                var restricted = new ControlledSubstanceInformation
                {
                    AuthorizationNum = model.Restricted.Rua,
                    ClassSchedule = model.Restricted.Class,
                    Custodian = model.Restricted.Custodian,
                    EndUser = model.Restricted.Users,
                    StorageSite = model.Restricted.StorageSite,
                    Use = model.Restricted.Use
                };

                order.SetAuthorizationInfo(restricted);
            }
            else
            {
                order.ClearAuthorizationInfo();
            }

            if (includeLineItemsAndSplits)
            {
                order.EstimatedTax = decimal.Parse(model.Tax.TrimEnd('%'));
                order.ShippingAmount = decimal.Parse(model.Shipping.TrimStart('$'));

                order.LineItems.Clear(); //replace line items and splits
                order.Splits.Clear();

                //Add in line items
                foreach (var lineItem in model.Items)
                {
                    if (lineItem.IsValid())
                    {
                        var commodity = string.IsNullOrWhiteSpace(lineItem.CommodityCode)
                                            ? null
                                            : _repositoryFactory.CommodityRepository.GetById(lineItem.CommodityCode);

                        //TODO: could use automapper later, but need to do validation
                        var orderLineItem = new LineItem
                        {
                            CatalogNumber = lineItem.CatalogNumber,
                            Commodity = commodity,
                            Description = lineItem.Description,
                            Notes = lineItem.Notes,
                            Quantity = decimal.Parse(lineItem.Quantity),
                            Unit = lineItem.Units,//TODO: shouldn't this link to UOM?
                            UnitPrice = decimal.Parse(lineItem.Price),
                            Url = lineItem.Url
                        };

                        order.AddLineItem(orderLineItem);

                        if (model.SplitType == OrderViewModel.SplitTypes.Line)
                        {
                            var lineItemId = lineItem.Id;

                            //Go through each split created for this line item
                            foreach (var split in model.Splits.Where(x => x.LineItemId == lineItemId))
                            {
                                if (split.IsValid())
                                {
                                    order.AddSplit(new Split
                                    {
                                        Account = split.Account,
                                        Amount = decimal.Parse(split.Amount),
                                        LineItem = orderLineItem,
                                        SubAccount = split.SubAccount,
                                        Project = split.Project
                                    });
                                }
                            }
                        }
                    }
                }

                //TODO: not that I am not checking an order split actually has valid splits, or that they add to the total.
                if (model.SplitType == OrderViewModel.SplitTypes.Order)
                {
                    foreach (var split in model.Splits)
                    {
                        if (split.IsValid())
                        {
                            order.AddSplit(new Split
                            {
                                Account = split.Account,
                                Amount = decimal.Parse(split.Amount),
                                SubAccount = split.SubAccount,
                                Project = split.Project
                            });
                        }
                    }
                }
                else if (model.SplitType == OrderViewModel.SplitTypes.None)
                {
                    order.AddSplit(new Split { Amount = order.Total(), Account = model.Account, SubAccount = model.SubAccount, Project = model.Project }); //Order with "no" splits get one split for the full amount
                }
            }
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

                HasControlledSubstance = false,

                DateNeeded = DateTime.Now.AddDays(5),
                AllowBackorder = false,

                ShippingAmount = 19.99m,
                EstimatedTax = 8.89m,

                CreatedBy = user,
                StatusCode = accountmgr
            };

            var line1 = new LineItem() { Quantity = 1, UnitPrice = 2.99m, Unit = "each", Description = "pencils" };
            var line2 = new LineItem() { Quantity = 3, UnitPrice = 17.99m, Unit = "dozen", Description = "pen", Url = "http://fake.com/product1", Notes = "I want the good pens." };

            var fieldInfo = typeof(LineItem).GetProperty("Id");

            if (fieldInfo != null)
            {
                fieldInfo.SetValue(line1, 1, null);
                fieldInfo.SetValue(line2, 2, null);
            }

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

                    // add in some line items
                    split1.LineItem = line1;
                    split2.LineItem = line1;

                    split3.LineItem = line2;
                    split4.LineItem = line2;
                    
                    order.AddSplit(split1);
                    order.AddSplit(split2);
                    order.AddSplit(split3);
                    order.AddSplit(split4);

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

        public string Account { get; set; }
        public string SubAccount { get; set; }
        public string Project { get; set; }
        public string Approvers { get; set; }
        public string AccountManagers { get; set; }
        public int[] ConditionalApprovals { get; set; }

        public bool? AdjustRouting { get; set; }

        public ControlledSubstance Restricted { get; set; }

        public string Backorder { get; set; }
        public bool AllowBackorder { get { return Backorder == "on"; } }

        public Guid[] FileIds { get; set; }

        public DateTime? DateNeeded { get; set; }
        public string ShippingType { get; set; }
        public string Comments { get; set; }
        
        public class Split
        {
            public int? LineItemId { get; set; }
            public string Account { get; set; }
            public string SubAccount { get; set; }
            public string Project { get; set; }
            public string Amount { get; set; }
            public string Percent { get; set; }

            /// <summary>
            /// Split is valid if it has an account and amount
            /// TODO: is that true?
            /// </summary>
            /// <returns></returns>
            public bool IsValid()
            {
                return !string.IsNullOrWhiteSpace(Account) && !string.IsNullOrWhiteSpace(Amount);
            }
        }

        public class LineItem
        {
            public int Id { get; set; }
            public string Quantity { get; set; }
            public string Price { get; set; }
            public string Units { get; set; }
            public string CatalogNumber { get; set; }
            public string Description { get; set; }
            public string CommodityCode { get; set; }
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

        public class ControlledSubstance
        {
            public bool IsRestricted { get { return Status; } }
            public bool Status { get; set; }
            public string Rua { get; set; }
            public string Class { get; set; }
            public string Use { get; set; }
            public string StorageSite { get; set; }
            public string Custodian { get; set; }
            public string Users { get; set; }
        }

        public enum SplitTypes
        {
            None,
            Order,
            Line
        }
    }
}
