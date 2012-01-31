using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;
using Purchasing.Core;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Order class
    /// </summary>
    public class OrderController : ApplicationController
    {
	    private readonly IOrderAccessService _orderAccessService;
        private readonly IOrderService _orderService;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly ISecurityService _securityService;

        public OrderController(IRepositoryFactory repositoryFactory, IOrderAccessService orderAccessService, IOrderService orderService, ISecurityService securityService)
        {
            _orderAccessService = orderAccessService;
            _orderService = orderService;
            _repositoryFactory = repositoryFactory;
            _securityService = securityService;
        }

        /// <summary>
        /// List of orders
        /// </summary>
        /// <param name="statusFilter"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="showAll">Matches AllActive in GetListOfOrders</param>
        /// <param name="showCompleted">Matches All in GetListOfOrders</param>
        /// <param name="showOwned"></param>
        /// <param name="hideOrdersYouCreated">Hide orders which you have created</param>
        /// <returns></returns>
        public ActionResult Index(string[] statusFilter, DateTime? startDate, DateTime? endDate, bool showAll = false, bool showCompleted = false, bool showOwned = false, bool hideOrdersYouCreated = false)
        {
            //TODO: Review even/odd display of table once Trish has look at it. (This page is a single, and the background color is the same as the even background color.
            if(statusFilter == null)
            {
                statusFilter = new string[0];
            }

            var filters = statusFilter.ToList();
            var list = Repository.OfType<OrderStatusCode>().Queryable.Where(a => filters.Contains(a.Id)).ToList();

            var orders = _orderAccessService.GetListofOrders(showAll, showCompleted, showOwned, hideOrdersYouCreated, list, startDate, endDate);
            var viewModel = FilteredOrderListModel.Create(Repository, orders);
            viewModel.CheckedOrderStatusCodes = filters;
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;
            viewModel.ShowAll = showAll;
            viewModel.ShowCompleted = showCompleted;
            viewModel.ShowOwned = showOwned;
            viewModel.HideOrdersYouCreated = hideOrdersYouCreated;
            viewModel.ColumnPreferences = _repositoryFactory.ColumnPreferencesRepository.GetNullableById(CurrentUser.Identity.Name) ??
                                          new ColumnPreferences(CurrentUser.Identity.Name);

            return View(viewModel);

        }

        /// <summary>
        /// Page to view Administrative Workgroup Orders
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminOrders(string[] statusFilter, DateTime? startDate, DateTime? endDate, bool showAll = false, bool showCompleted = false, bool showOwned = false, bool hideOrdersYouCreated = false)
        {
            //TODO: Review even/odd display of table once Trish has look at it. (This page is a single, and the background color is the same as the even background color.
            if(statusFilter == null)
            {
                statusFilter = new string[0];
            }

            var filters = statusFilter.ToList();
            var list = Repository.OfType<OrderStatusCode>().Queryable.Where(a => filters.Contains(a.Id)).ToList();

            //TODO: replace/update this so it gets the admin list of orders.
            var orders = _orderAccessService.GetListofOrders(showAll, showCompleted, showOwned, hideOrdersYouCreated, list, startDate, endDate);
            var viewModel = FilteredOrderListModel.Create(Repository, orders);
            viewModel.CheckedOrderStatusCodes = filters;
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;
            viewModel.ShowAll = showAll;
            viewModel.ShowCompleted = showCompleted;
            viewModel.ShowOwned = showOwned;
            viewModel.HideOrdersYouCreated = hideOrdersYouCreated;
            viewModel.ColumnPreferences = _repositoryFactory.ColumnPreferencesRepository.GetNullableById(CurrentUser.Identity.Name) ??
                                          new ColumnPreferences(CurrentUser.Identity.Name);

            return View(viewModel);

        }

        /// <summary>
        /// If user has more than one workgroup, they select it for their order
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectWorkgroup()
        {
            var user = GetCurrentUser();
            var role = _repositoryFactory.RoleRepository.GetNullableById(Role.Codes.Requester);
            var workgroups = user.WorkgroupPermissions.Where(a => a.Role == role && !a.Workgroup.Administrative).Select(a=>a.Workgroup);

            // only one workgroup, automatically redirect
            if (workgroups.Count() == 1)
            {
                var workgroup = workgroups.Single();
                return this.RedirectToAction(a => a.Request(workgroup.Id));
            }
            
            return View(workgroups.ToList());
        }

        /// <summary>
        /// Make an order request
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public new ActionResult Request(int id /*TODO: Change to workgroup query param?? */)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                return RedirectToAction("SelectWorkgroup");
            }

            var model = CreateOrderModifyModel(workgroup);

            return View(model);
        }

        [HttpPost]
        public new ActionResult Request(int id, OrderViewModel model)
        {
            //TODO: no validation will be done!
            var order = new Order();

            BindOrderModel(order, model, includeLineItemsAndSplits: true);

            _orderService.CreateApprovalsForNewOrder(order, accountId: model.Account, approverId: model.Approvers, accountManagerId: model.AccountManagers, conditionalApprovalIds: model.ConditionalApprovals);

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            Message = "New Order Created Successfully";

            return RedirectToAction("Review", new { id = order.Id });
        }

        /// <summary>
        /// Edit the given order
        /// </summary>
        public ActionResult Edit(int id)
        {
            var order = _repositoryFactory.OrderRepository.GetNullableById(id);
            Check.Require(order != null);
            
            var model = CreateOrderModifyModel(order.Workgroup);
            model.Order = order;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(int id, OrderViewModel model)
        {
            var order = _repositoryFactory.OrderRepository.GetNullableById(id);

            Check.Require(order != null);

            var adjustRouting = model.AdjustRouting.HasValue && model.AdjustRouting.Value;

            BindOrderModel(order, model, includeLineItemsAndSplits: adjustRouting);

            if (adjustRouting)
            {
                //TODO: Add expense validation
                //order.ValidateExpenses().ToArray();
                
                //TODO: For now, when we adjust the approvals we have to save the intermediate bound model so the new approvals can be saved
                _repositoryFactory.OrderRepository.EnsurePersistent(order);
                _orderService.ReRouteApprovalsForExistingOrder(order);
            }
            else
            {
                _orderService.EditExistingOrder(order);
            }

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            Message = "Order Edited!";

            return RedirectToAction("Review", new { id });
        }
        
        /// <summary>
        /// Copy the existing order given 
        /// </summary>
        public ActionResult Copy(int id)
        {
            return Edit(id);
        }

        [HttpPost]
        public ActionResult Copy(int id, OrderViewModel model)
        {
            var order = new Order();

            BindOrderModel(order, model, includeLineItemsAndSplits: true);

            _orderService.CreateApprovalsForNewOrder(order, accountId: model.Account, approverId: model.Approvers, accountManagerId: model.AccountManagers, conditionalApprovalIds: model.ConditionalApprovals);

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            Message = "New Order Created: Existing Order Duplicated Successfully";

            return RedirectToAction("Review", new { id = order.Id });
        }

        /// <summary>
        /// Page to review an order and for approving/denying the order.
        /// </summary>
        /// <remarks>
        /// This page should be used by ad hoc account managers too, but without the link to edit
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Review(int id)
        {
            //TODO: eager fetch or fetch related collections separately to avoid a ton of queries
            var model = new ReviewOrderViewModel {Order = _repositoryFactory.OrderRepository.GetNullableById(id)};
            
            if (model.Order == null)
            {
                Message = "Order not found.";
                //TODO: Workout a way to get a return to where the person came from, rather than just redirecting to the generic index
                //TODO: you can use the UrlReferrer for that //Scott
                return RedirectToAction("index");
            }

            model.CanEditOrder = _orderAccessService.GetAccessLevel(model.Order) == OrderAccessLevel.Edit;

            if (model.CanEditOrder)
            {
                var app = from a in _repositoryFactory.ApprovalRepository.Queryable
                          where a.Order.Id == id && a.StatusCode.Level == a.Order.StatusCode.Level &&
                              (!_repositoryFactory.WorkgroupAccountRepository.Queryable.Any(
                                  x => x.Workgroup.Id == model.Order.Workgroup.Id && x.Account.Id == a.Split.Account))
                          select a;

                model.ExternalApprovals = app.ToList();
            }

            return View(model);
        }

        /// <summary>
        /// Reroute the approval given by Id to the kerb person instead of the currently assigned user(s)
        /// </summary>
        [HttpPost]
        public ActionResult ReRouteApproval(int id, string kerb)
        {
            //TODO: make sure user has access to modify approval
            var approval = _repositoryFactory.ApprovalRepository.GetNullableById(id);

            Check.Require(approval != null);
            Check.Require(!approval.Completed);

            var user = _repositoryFactory.UserRepository.GetNullableById(kerb);

            if (user == null) //TODO: lookup and create new user
            {
                return Json(new {success = false});
            }

            _orderService.ReRouteSingleApprovalForExistingOrder(approval, user);

            _repositoryFactory.ApprovalRepository.EnsurePersistent(approval);

            return Json(new {success = true, name = user.FullName});
        }

        [HttpPost]
        public JsonNetResult AddComment(int id, string comment)
        {
            var order = Repository.OfType<Order>().GetNullableById(id);

            if (_orderAccessService.GetAccessLevel(order) == OrderAccessLevel.Edit)
            {
                var orderComment = new OrderComment() {Text = comment, User = GetCurrentUser()};
                order.AddComment(orderComment);

                Repository.OfType<Order>().EnsurePersistent(order);

                return new JsonNetResult(new {Date=DateTime.Now.ToShortDateString(), Text=comment, User=orderComment.User.FullName});
            }

            // no access
            return new JsonNetResult(false);
        }

        /// <summary>
        /// Ajax call to search for any commodity codes, match by name
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public JsonNetResult SearchCommodityCodes(string searchTerm)
        {
            var results =
                _repositoryFactory.CommodityRepository.Queryable.Where(c => c.Name.Contains(searchTerm)).Select(
                    a => new {a.Id, a.Name}).ToList();
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

            return new JsonNetResult(new { id, lineItems });
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

            return new JsonNetResult(new { id, splits, splitType = splitType.ToString() });
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

            return new JsonNetResult(new { id, lineItems, splits, splitType = splitType.ToString() });
        }
        
        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult AddVendor(int workgroupId, WorkgroupVendor vendor)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetById(workgroupId);

            workgroup.AddVendor(vendor);

            _repositoryFactory.WorkgroupRepository.EnsurePersistent(workgroup);

            return Json(new { id = vendor.Id });
        }

        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult AddAddress(int workgroupId, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetById(workgroupId);
            
            workgroup.AddAddress(workgroupAddress);

            _repositoryFactory.WorkgroupRepository.EnsurePersistent(workgroup);

            return Json(new { id = workgroupAddress.Id });
        }

        [HttpPost]
        [BypassAntiForgeryToken]
        public ActionResult UploadFile(int? orderId)
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
                attachment.Contents = binaryReader.ReadBytes((int)request.InputStream.Length);
            }

            if (orderId.HasValue) //Save directly to order if a value is passed, otherwise it needs to be assoc. by form
            {
                attachment.Order = _repositoryFactory.OrderRepository.GetById(orderId.Value);
            }

            _repositoryFactory.AttachmentRepository.EnsurePersistent(attachment);

            return Json(new { success = true, id = attachment.Id }, "text/html");
        }

        /// <summary>
        /// Allows a user to download any attachment file by providing the file ID
        /// </summary>
        public ActionResult ViewFile(Guid fileId)
        {
            //TODO: check permissions
            var file = _repositoryFactory.AttachmentRepository.GetNullableById(fileId);

            if (file == null) return HttpNotFound("The requested file could not be found");

            return File(file.Contents, file.ContentType, file.FileName);
        }

        private void BindOrderModel(Order order, OrderViewModel model, bool includeLineItemsAndSplits = false)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetById(model.Workgroup);

            //TODO: automapper?
            order.Vendor = model.Vendor == 0 ? null : _repositoryFactory.WorkgroupVendorRepository.GetById(model.Vendor);
            order.Address = _repositoryFactory.WorkgroupAddressRepository.GetById(model.ShipAddress);
            order.ShippingType = _repositoryFactory.ShippingTypeRepository.GetById(model.ShippingType);
            order.DateNeeded = model.DateNeeded;
            order.AllowBackorder = model.AllowBackorder;
            order.Workgroup = workgroup;
            order.Organization = workgroup.PrimaryOrganization; //TODO: why is this needed?
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

            if (model.CustomFields != null)
            {
                order.CustomFieldAnswers.Clear();

                foreach (var customField in model.CustomFields.Where(x => !string.IsNullOrWhiteSpace(x.Answer)))
                {
                    var answer = new CustomFieldAnswer
                    {
                        Answer = customField.Answer,
                        CustomField = _repositoryFactory.CustomFieldRepository.GetById(customField.Id)
                    };

                    order.AddCustomAnswer(answer);
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
                order.FreightAmount = decimal.Parse(model.Freight.TrimStart('$'));

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

                order.TotalFromDb = order.Total();
            }
        }

        private OrderModifyModel CreateOrderModifyModel(Workgroup workgroup)
        {
            //TODO: possibly just use SQL or get this from a view, depending on perf
            //TODO: need to pare down results to workgroup/org specific stuff
            var model = new OrderModifyModel
            {
                Order = new Order(),
                Workgroup = workgroup,
                Units = _repositoryFactory.UnitOfMeasureRepository.GetAll(), //TODO: caching?
                Accounts = workgroup.Accounts.Select(x=>x.Account).ToList(),
                Vendors = workgroup.Vendors,
                Addresses = workgroup.Addresses,
                ShippingTypes = _repositoryFactory.ShippingTypeRepository.GetAll(), //TODO: caching?
                Approvers = _repositoryFactory.WorkgroupPermissionRepository.Queryable.Where(x => x.Role.Id == Role.Codes.Approver).Select(x => x.User).ToList(),
                AccountManagers = _repositoryFactory.WorkgroupPermissionRepository.Queryable.Where(x => x.Role.Id == Role.Codes.AccountManager).Select(x => x.User).ToList(),
                ConditionalApprovals = workgroup.AllConditionalApprovals,
                CustomFields = _repositoryFactory.CustomFieldRepository.Queryable.Where(x=>x.Organization.Id == workgroup.PrimaryOrganization.Id).ToList()
            };

            return model;
        }
    }
}
