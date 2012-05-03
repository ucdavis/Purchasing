﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Elmah;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.WS;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Attributes;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using Purchasing.Web.Utility;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;
using Purchasing.Core;
using UCDArch.Data.NHibernate;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Order class
    /// </summary>
    public class OrderController : ApplicationController
    {
        private readonly IOrderService _orderService;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly ISecurityService _securityService;
        private readonly IDirectorySearchService _directorySearchService; //TODO: Review if this is needed
        private readonly IFinancialSystemService _financialSystemService;
        private readonly IQueryRepositoryFactory _queryRepository;


        public OrderController(
            IRepositoryFactory repositoryFactory, 
            IOrderService orderService, 
            ISecurityService securityService, 
            IDirectorySearchService directorySearchService, 
            IFinancialSystemService financialSystemService,
            IQueryRepositoryFactory queryRepository)
        {
            _orderService = orderService;
            _repositoryFactory = repositoryFactory;
            _securityService = securityService;
            _directorySearchService = directorySearchService;
            _financialSystemService = financialSystemService;
            _queryRepository = queryRepository;
        }

        /// <summary>
        /// List of orders
        /// </summary>
        /// <param name="selectedOrderStatus"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="showPending"></param>
        /// <param name="showLast"></param>
        /// <returns></returns>
        public ActionResult Index(string selectedOrderStatus, DateTime? startDate, DateTime? endDate, bool showPending = false, bool showCreated = false, string showLast = null) //, bool showAll = false, bool showCompleted = false, bool showOwned = false, bool hideOrdersYouCreated = false)
        {
            //TODO: Review even/odd display of table once Trish has look at it. (This page is a single, and the background color is the same as the even background color.
            var saveSelectedOrderStatus = selectedOrderStatus;
            if (selectedOrderStatus == "All")
            {
                selectedOrderStatus = null;
            }
            
            var isComplete = (selectedOrderStatus == OrderStatusCode.Codes.Complete);

            if (selectedOrderStatus == "Received" || selectedOrderStatus == "UnReceived")
            {
                selectedOrderStatus = OrderStatusCode.Codes.Complete;
                isComplete = true;
            }
            
            IList<Order> orders;
            if (string.IsNullOrWhiteSpace(showLast))
            {
                orders = _orderService.GetListofOrders(isComplete, showPending, selectedOrderStatus, startDate, endDate, showCreated);
            } else
            {
                if (showLast== "month")
                {
                    orders =
                        Repository.OfType<CompletedOrdersThisMonth>().Queryable.Where(
                            a => a.OrderTrackingUser == CurrentUser.Identity.Name).Select(b => b.Order).ToList();
                } else
                {
                    orders = Repository.OfType<CompletedOrdersThisWeek>().Queryable.Where(
                            a => a.OrderTrackingUser == CurrentUser.Identity.Name).Select(b => b.Order).ToList();
                }
            }
            if (saveSelectedOrderStatus == "Received")
            {
                orders = orders.Where(a => a.OrderReceived).ToList();
            }
            else if (saveSelectedOrderStatus == "UnReceived")
            {
                orders = orders.Where(a => a.OrderReceived == false).ToList();
            }

            var orderIds = orders.Select(x => x.Id).ToList();

            var model = new FilteredOrderListModelDto
                             {
                                 ShowLast = showLast,
                                 SelectedOrderStatus = selectedOrderStatus,
                                 StartDate = startDate,
                                 EndDate = endDate,
                                 ShowPending = showPending,
                                 ShowCreated = showCreated,
                                 ColumnPreferences =
                                     _repositoryFactory.ColumnPreferencesRepository.GetNullableById(
                                         CurrentUser.Identity.Name) ??
                                     new ColumnPreferences(CurrentUser.Identity.Name)
                             };

            PopulateModel(orderIds, model);

            return View(model);

        }

        /// <summary>
        /// Page to view Administrative Workgroup Orders
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminOrders(string selectedOrderStatus, DateTime? startDate, DateTime? endDate, bool showPending = false)
        {
            //TODO: Review even/odd display of table once Trish has look at it. (This page is a single, and the background color is the same as the even background color.
            var saveSelectedOrderStatus = selectedOrderStatus;
            if (selectedOrderStatus == "All")
            {
                selectedOrderStatus = null;
            }
            var isComplete = selectedOrderStatus == OrderStatusCode.Codes.Complete;

            if (selectedOrderStatus == "Received" || selectedOrderStatus == "UnReceived")
            {
                selectedOrderStatus = OrderStatusCode.Codes.Complete;
                isComplete = true;
            }

            var orders = _orderService.GetAdministrativeListofOrders(isComplete, showPending, selectedOrderStatus, startDate, endDate);

            if (saveSelectedOrderStatus == "Received")
            {
                orders = orders.Where(a => a.OrderReceived).ToList();
            }
            else if (saveSelectedOrderStatus == "UnReceived")
            {
                orders = orders.Where(a => a.OrderReceived == false).ToList();
            }

            var orderIds = orders.Select(x => x.Id).ToList();

            var model = new FilteredOrderListModelDto
            {
                SelectedOrderStatus = selectedOrderStatus,
                StartDate = startDate,
                EndDate = endDate,
                ShowPending = showPending,
                ColumnPreferences =
                    _repositoryFactory.ColumnPreferencesRepository.GetNullableById(
                        CurrentUser.Identity.Name) ??
                    new ColumnPreferences(CurrentUser.Identity.Name)
            };

            PopulateModel(orderIds, model);
            
            return View(model);
        }

        private void PopulateModel(List<int> orderIds, FilteredOrderListModelDto model)
        {
            model.OrderHistoryDtos = (from o in _repositoryFactory.OrderRepository.Queryable
                                      where orderIds.Contains(o.Id)
                                      select new FilteredOrderListModelDto.OrderHistoryDto
                                      {
                                          Order = o,
                                          Workgroup = o.Workgroup.Name,
                                          Vendor = o.Vendor,
                                          CreatedBy = o.CreatedBy.FirstName + " " + o.CreatedBy.LastName,
                                          Status = o.StatusCode.Name
                                      }).ToList();

            if (model.RequresOrderTracking())
            {
                model.OrderTracking =
                    (from o in _repositoryFactory.OrderTrackingRepository.Queryable.Fetch(x => x.User).Fetch(x => x.StatusCode)
                     where orderIds.Contains(o.Order.Id)
                     select o).ToList();
            }

            if (model.RequiresSplits())
            {
                model.Splits = (from s in _repositoryFactory.SplitRepository.Queryable
                                where orderIds.Contains(s.Order.Id)
                                select s).ToList();
            }

            if (model.RequiresApprovals())
            {
                model.Approvals =
                    (from a in
                         _repositoryFactory.ApprovalRepository.Queryable.Fetch(x => x.User).Fetch(x => x.SecondaryUser)
                     where orderIds.Contains(a.Order.Id)
                     select a).ToList();
            }

            model.PopulateStatusCodes(_repositoryFactory.OrderStatusCodeRepository);
        }

        /// <summary>
        /// If user has more than one workgroup, they select it for their order
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectWorkgroup()
        {
            var user = GetCurrentUser();
            var role = _repositoryFactory.RoleRepository.GetNullableById(Role.Codes.Requester);
            var workgroups = user.WorkgroupPermissions.Where(a => a.Role == role && !a.Workgroup.Administrative && a.Workgroup.IsActive).Select(a=>a.Workgroup);

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
        public new ActionResult Request(int id)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                return RedirectToAction("SelectWorkgroup");
            }

            var requesterInWorkgroup = _repositoryFactory.WorkgroupPermissionRepository
                .Queryable.Where(x => x.Workgroup.Id == id && x.User.Id == CurrentUser.Identity.Name)
                .Where(x => x.Role.Id == Role.Codes.Requester);

            if (!requesterInWorkgroup.Any())
            {
                return new HttpUnauthorizedResult(Resources.NoAccess_Workgroup);
            }

            var model = CreateOrderModifyModel(workgroup);

            return View(model);
        }

        [HttpPost]
        public new ActionResult Request(int id, OrderViewModel model)
        {
            var canCreateOrderInWorkgroup =
                _securityService.HasWorkgroupAccess(_repositoryFactory.WorkgroupRepository.GetById(model.Workgroup));

            Check.Require(canCreateOrderInWorkgroup);

            var order = new Order();

            BindOrderModel(order, model, includeLineItemsAndSplits: true);

            _orderService.CreateApprovalsForNewOrder(order, accountId: model.Account, approverId: model.Approvers, accountManagerId: model.AccountManagers, conditionalApprovalIds: model.ConditionalApprovals);

            _orderService.HandleSavedForm(order, model.FormSaveId);

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            Message = Resources.NewOrder_Success;

            return RedirectToAction("Review", new { id = order.Id });
        }

        /// <summary>
        /// Edit the given order
        /// </summary>
        [AuthorizeEditOrder]
        public ActionResult Edit(int id)
        {
            var order = _repositoryFactory.OrderRepository.GetNullableById(id);
            Check.Require(order != null);
            
            var model = CreateOrderModifyModel(order.Workgroup);
            model.Order = order;

            var inactiveAccounts = GetInactiveAccountsForOrder(id);

            if (inactiveAccounts.Any())
            {
                ErrorMessage = Resources.InactiveAccounts_Warning +
                               string.Join(", ", inactiveAccounts);
            }

            return View(model);
        }

        [HttpPost]
        [AuthorizeEditOrder]
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

                _orderService.ReRouteApprovalsForExistingOrder(order, approverId: model.Approvers, accountManagerId: model.AccountManagers);
            }
            else
            {
                _orderService.EditExistingOrder(order);
            }

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            Message = Resources.OrderEdit_Success;

            return RedirectToAction("Review", new { id });
        }
        
        /// <summary>
        /// Copy the existing order given 
        /// </summary>
        [AuthorizeReadOrEditOrder]
        public ActionResult Copy(int id)
        {
            var order = _repositoryFactory.OrderRepository.GetNullableById(id);
            Check.Require(order != null);

            var model = CreateOrderModifyModel(order.Workgroup);
            model.IsCopyOrder = true;
            model.Order = order;
            model.Order.Attachments.Clear(); //Clear out attachments so they don't get included w/ copied order

            var inactiveAccounts = GetInactiveAccountsForOrder(id);
            
            if (inactiveAccounts.Any())
            {
                ErrorMessage = Resources.InactiveAccounts_Warning +
                               string.Join(", ", inactiveAccounts);
            }

            return View(model);
        }

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public ActionResult Copy(int id, OrderViewModel model)
        {
            var order = new Order();

            BindOrderModel(order, model, includeLineItemsAndSplits: true);

            _orderService.CreateApprovalsForNewOrder(order, accountId: model.Account, approverId: model.Approvers, accountManagerId: model.AccountManagers, conditionalApprovalIds: model.ConditionalApprovals);

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            Message = Resources.OrderCopy_Success;

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
        [AuthorizeReadOrEditOrder]
        public ActionResult Review(int id)
        {
            var orderQuery = _repositoryFactory.OrderRepository.Queryable.Where(x => x.Id == id);
            
            var model = orderQuery
                .Select(x => new ReviewOrderViewModel
                                 {
                                     Order = x,
                                     Complete = x.StatusCode.IsComplete,
                                     Status = x.StatusCode.Name,
                                     WorkgroupName = x.Workgroup.Name,
                                     OrganizationName = x.Organization.Name,
                                 }).Single();

            model.Vendor = orderQuery.Select(x => x.Vendor).Single();
            model.Address = orderQuery.Select(x => x.Address).Single();
            model.LineItems =
                _repositoryFactory.LineItemRepository.Queryable.Fetch(x => x.Commodity).Where(x => x.Order.Id == id).
                    ToList();
            model.Splits = _repositoryFactory.SplitRepository.Queryable.Where(x => x.Order.Id == id).Fetch(x=>x.DbAccount).ToList();

            var splitsWithSubAccounts = model.Splits.Where(a => a.Account != null && a.SubAccount != null).ToList();

            if (splitsWithSubAccounts.Any())
            {
                var accts = splitsWithSubAccounts.Select(a => a.Account).ToList();
                var subAccts = splitsWithSubAccounts.Select(a => a.SubAccount).ToList();

                model.SubAccounts =
                    _repositoryFactory.SubAccountRepository.Queryable.Where(
                        a =>
                        accts.Contains(a.AccountNumber) &&
                        subAccts.Contains(a.SubAccountNumber)).ToList();
            }
            
            if (model.Order.HasControlledSubstance)
            {
                model.ControllerSubstance =
                    _repositoryFactory.ControlledSubstanceInformationRepository.Queryable.First(x => x.Order.Id == id);
            }

            model.CustomFieldsAnswers =
                _repositoryFactory.CustomFieldAnswerRepository.Queryable.Fetch(x => x.CustomField).Where(
                    x => x.Order.Id == id).ToList();

            model.Approvals =
                _repositoryFactory.ApprovalRepository.Queryable.Fetch(x => x.StatusCode).Fetch(x => x.User).Fetch(
                    x => x.SecondaryUser).Where(x => x.Order.Id == id).ToList();

            model.Comments =
                _repositoryFactory.OrderCommentRepository.Queryable.Fetch(x => x.User).Where(x => x.Order.Id == id).ToList();
            model.Attachments =
                _repositoryFactory.AttachmentRepository.Queryable.Fetch(x => x.User).Where(x => x.Order.Id == id).ToList();

            model.OrderTracking =
                _repositoryFactory.OrderTrackingRepository.Queryable.Fetch(x => x.StatusCode).Fetch(x => x.User).Where(
                    x => x.Order.Id == id).ToList();

            model.IsRequesterInWorkgroup = _repositoryFactory.WorkgroupPermissionRepository.Queryable
                .Any(
                    x =>
                    x.Workgroup.Id == model.Order.Workgroup.Id && x.Role.Id == Role.Codes.Requester &&
                    x.User.Id == CurrentUser.Identity.Name);

            if (model.Complete){   //complete orders can't ever be edited or cancelled so just return now
                return View(model);
            }

            model.CanEditOrder = _securityService.GetAccessLevel(model.Order) == OrderAccessLevel.Edit;
            model.CanCancelOrder = model.Order.CreatedBy.Id == CurrentUser.Identity.Name; //Can cancel the order if you are the one who created it
            model.IsPurchaser = model.Order.StatusCode.Id == OrderStatusCode.Codes.Purchaser;
            model.IsAccountManager = model.Order.StatusCode.Id == OrderStatusCode.Codes.AccountManager;

            if (model.CanEditOrder)
            {
                if (model.IsAccountManager)
                {
                    model.HasAssociatedAccounts =
                        _repositoryFactory.SplitRepository.Queryable
                            .Any(s => s.Order.Id == model.Order.Id && s.Account != null);
                }

                if (model.IsPurchaser)
                {
                    model.OrderTypes = _repositoryFactory.OrderTypeRepository.Queryable.Where(x => x.PurchaserAssignable).ToList();
                }

                var app = from a in _repositoryFactory.ApprovalRepository.Queryable
                          where a.Order.Id == id && a.StatusCode.Level == a.Order.StatusCode.Level 
                                && a.Split != null && a.StatusCode.Id != OrderStatusCode.Codes.ConditionalApprover
                                && (!_repositoryFactory.WorkgroupAccountRepository.Queryable.Any(
                                  x => x.Workgroup.Id == model.Order.Workgroup.Id && x.Account.Id == a.Split.Account))
                          select a;

                model.ExternalApprovals = app.ToList();
            }

            return View(model);
        }

        /// <summary>
        /// Find an order by request number and redirect to the review page
        /// </summary>
        public ActionResult Lookup(string id)
        {
            var relatedOrderId =
                _repositoryFactory.OrderRepository.Queryable
                    .Where(x => x.RequestNumber == id)
                    .Select(x => x.Id)
                    .SingleOrDefault();

            if (relatedOrderId == default(int))
            {
                return new HttpNotFoundResult();
            }

            return RedirectToAction("Review", new {id = relatedOrderId});
        }

        [HttpPost]
        [AuthorizeEditOrder]
        public ActionResult Approve(int id /*order*/, string action, string comment, string orderType, string kfsDocType)
        {
            var order =
                _repositoryFactory.OrderRepository.Queryable.Fetch(x => x.Approvals).Single(x => x.Id == id);
            
            if (!string.IsNullOrWhiteSpace(comment))
            {
                order.AddComment(new OrderComment {Text = comment, User = GetCurrentUser()});
            }

            if (action == "Approve")
            {
                _orderService.Approve(order);   
            }
            else if (action == "Deny")
            {
                if (string.IsNullOrWhiteSpace(comment))
                {
                    ErrorMessage = Resources.CommentRequired_Order;
                    return RedirectToAction("Review", new {id});
                }

                _orderService.Deny(order, comment);
            }
            else if (action == "Complete")
            {
                var newOrderType = _repositoryFactory.OrderTypeRepository.GetNullableById(orderType);
                var isPurchaser = order.StatusCode.Id == OrderStatusCode.Codes.Purchaser;

                Check.Require(isPurchaser);
                Check.Require(newOrderType != null);
                
                newOrderType.DocType = kfsDocType;

                var errors = _orderService.Complete(order, newOrderType, kfsDocType);

                if (errors.Any()) //if we have any errors, raise them in ELMAH and redirect back to the review page without saving change
                {
                    ErrorMessage =
                        "There was a problem completing this order. Please try again later and notify support if problems persist."; 
                    
                    ErrorSignal.FromCurrentContext().Raise(
                        new System.ApplicationException(string.Join(Environment.NewLine, errors)));
                    
                    return RedirectToAction("Review", new {id});
                }
            }

            _repositoryFactory.OrderRepository.EnsurePersistent(order); //Save approval changes

            Message = string.Format(Resources.ApprovalAction_Success, action, CurrentUser.Identity.Name);

            return RedirectToAction("Review", "Order", new {id});
        }

        [HttpPost]
        public ActionResult Cancel(int id, string comment)
        {
            var order = _repositoryFactory.OrderRepository.GetNullableById(id);

            Check.Require(order != null);

            if (order.CreatedBy.Id != CurrentUser.Identity.Name)
            {
                ErrorMessage = Resources.CancelOrder_NoAccess;
                return RedirectToAction("Review", new {id});
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                ErrorMessage = Resources.CancelOrder_CommentRequired;
                return RedirectToAction("Review", new {id});
            }

            order.AddComment(new OrderComment { Text = comment, User = GetCurrentUser() });

            _orderService.Cancel(order, comment);

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            Message = Resources.OrderCancelled_Success;

            return RedirectToAction("Review", "Order", new { id });
        }

        /// <summary>
        /// Reroute the approval given by Id to the kerb person instead of the currently assigned user(s)
        /// </summary>
        [HttpPost]
        [AuthorizeEditOrder]
        public ActionResult ReRouteApproval(int id, int approvalId, string kerb)
        {
            var approval = _repositoryFactory.ApprovalRepository.GetNullableById(approvalId);

            Check.Require(approval != null);
            Check.Require(!approval.Completed);
            Check.Require(!approval.Order.Workgroup.Accounts.Select(a => a.Account.Id).Contains(approval.Split.Account), Resources.ReRouteApproval_AccountError);

            var user = _securityService.GetUser(kerb);

            _orderService.ReRouteSingleApprovalForExistingOrder(approval, user);

            _repositoryFactory.ApprovalRepository.EnsurePersistent(approval);

            return Json(new {success = true, name = user.FullName});
        }

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public JsonNetResult AddComment(int id, string comment)
        {
            var order = Repository.OfType<Order>().GetNullableById(id);

            var orderComment = new OrderComment() {Text = comment, User = GetCurrentUser()};
            order.AddComment(orderComment);

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            return
                new JsonNetResult(
                    new {Date = DateTime.Now.ToShortDateString(), Text = comment, User = orderComment.User.FullName});
        }

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public JsonNetResult UpdateReferenceNumber(int id, string referenceNumber)
        {
            //Get the matching order, and only if the order is complete
            var order =
                _repositoryFactory.OrderRepository.Queryable.Single(x => x.Id == id && x.StatusCode.IsComplete);

            order.ReferenceNumber = referenceNumber;

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            return new JsonNetResult(new {success = true, referenceNumber});
        }


        /// <summary>
        /// Ajax call to search for any commodity codes, match by name
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public JsonResult SearchCommodityCodes(string searchTerm)
        {
            //var results =
            //    _repositoryFactory.SearchRepository.SearchCommodities(searchTerm).Select(a => new {a.Id, a.NameAndId});

            var results =
                _repositoryFactory.SearchRepository.SearchCommodities(searchTerm).Select(a => new IdAndName(a.Id, a.Name));

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeReadOrEditOrder]
        public JsonNetResult GetLineItemsAndSplits(int id)
        {
            var orderDetail = _repositoryFactory.OrderRepository
                .Queryable
                .Where(x => x.Id == id)
                .Select(x => new {Shipping = x.ShippingAmount, Freight = x.FreightAmount, Tax = x.EstimatedTax})
                .Single();

            var inactiveAccounts = GetInactiveAccountsForOrder(id);

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

            var splits = (from s in _repositoryFactory.SplitRepository.Queryable
                          join a in _repositoryFactory.AccountRepository.Queryable on s.Account equals a.Id
                          where s.Order.Id == id
                          select new OrderViewModel.Split
                                     {
                                         Account = inactiveAccounts.Contains(a.Id) ? string.Empty : a.Id,
                                         AccountName = a.Name,
                                         Amount = s.Amount.ToString(CultureInfo.InvariantCulture),
                                         LineItemId = s.LineItem == null ? 0 : s.LineItem.Id,
                                         Project = s.Project,
                                         SubAccount = s.SubAccount
                                     }).ToList();

            OrderViewModel.SplitTypes splitType;

            if (splits.Any(x => x.LineItemId != 0))
            {
                splitType = OrderViewModel.SplitTypes.Line;
            }
            else
            {
                //splits count = 0 if no account specified, 1 if only one account was specified with no splits
                splitType = splits.Count <= 1 ? OrderViewModel.SplitTypes.None : OrderViewModel.SplitTypes.Order;
            }

            return new JsonNetResult(new { id, orderDetail, lineItems, splits, splitType = splitType.ToString() });
        }

        [AuthorizeReadOrEditOrder]
        public JsonNetResult GetSubAccounts(int id)
        {
            var accounts =
                _repositoryFactory.SplitRepository.Queryable.Where(x => x.Order.Id == id).Select(x => x.Account).ToList();

            var subAccounts =
                _repositoryFactory.SubAccountRepository.Queryable
                    .Where(x => x.IsActive && accounts.Contains(x.AccountNumber))
                    .Select(x => new {x.AccountNumber, x.SubAccountNumber})
                    .ToList();

            var groupedSubAccounts =
                subAccounts.GroupBy(x => x.AccountNumber).Select(
                    x => new {Account = x.Key, SubAccounts = x.Select(sa => sa.SubAccountNumber)});

            return new JsonNetResult(groupedSubAccounts);
        }

        [HttpPost]
        public ActionResult AddVendor(int workgroupId, WorkgroupVendor vendor)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetById(workgroupId);

            Check.Require(_securityService.HasWorkgroupAccess(workgroup));

            workgroup.AddVendor(vendor);

            _repositoryFactory.WorkgroupRepository.EnsurePersistent(workgroup);

            return Json(new { id = vendor.Id });
        }

        [HttpPost]
        public ActionResult AddAddress(int workgroupId, WorkgroupAddress workgroupAddress)
        {
            //TODO: Consider using same logic as workgroup: _workgroupAddressService.CompareAddress
            var workgroup = _repositoryFactory.WorkgroupRepository.GetById(workgroupId);

            Check.Require(_securityService.HasWorkgroupAccess(workgroup));

            workgroup.AddAddress(workgroupAddress);

            _repositoryFactory.WorkgroupRepository.EnsurePersistent(workgroup);

            return Json(new { id = workgroupAddress.Id });
        }

        [HttpPost]
        [BypassAntiForgeryToken] //required because upload is being done by plugin
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
                Check.Require(request.Files.Count > 0, Resources.FileUpload_NoFile);
                var file = request.Files[0];

                attachment.FileName = Path.GetFileNameWithoutExtension(file.FileName) +
                    Path.GetExtension(file.FileName).ToLower();

                attachment.ContentType = file.ContentType;
            }

            using (var binaryReader = new BinaryReader(request.InputStream))
            {
                attachment.Contents = binaryReader.ReadBytes((int)request.InputStream.Length);
            }

            if (orderId.HasValue) //Save directly to order if a value is passed & user has access, otherwise it needs to be assoc. by form
            {
                var accessLevel = _securityService.GetAccessLevel(orderId.Value);
                const OrderAccessLevel allowed = OrderAccessLevel.Readonly | OrderAccessLevel.Edit;

                if (!allowed.HasFlag(accessLevel))
                {
                    return Json(new {success = false, id = 0}, "text/html");
                }

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
            var file = _repositoryFactory.AttachmentRepository.GetNullableById(fileId);

            if (file == null) return HttpNotFound(Resources.ViewFile_NotFound);

            var accessLevel = _securityService.GetAccessLevel(file.Order);

            if (!(OrderAccessLevel.Edit | OrderAccessLevel.Readonly).HasFlag(accessLevel))
            {
                return new HttpUnauthorizedResult(Resources.ViewFile_AccessDenied);
            }

            return File(file.Contents, file.ContentType, file.FileName);
        }

        [AuthorizeReadOrEditOrder]
        public ActionResult ReceiveItems(int id)
        {
            var order = _repositoryFactory.OrderRepository.Queryable.Single(a => a.Id == id);

            if(!order.StatusCode.IsComplete || order.StatusCode.Id == OrderStatusCode.Codes.Cancelled || order.StatusCode.Id == OrderStatusCode.Codes.Denied)
            {
                Message = "Order must be complete before receiving line items.";
                return this.RedirectToAction(a => a.Review(id));
            }
            

            return View(OrderReceiveModel.Create(order, _repositoryFactory.HistoryReceivedLineItemRepository));

        }

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public JsonNetResult ReceiveItems(int id, int lineItemId, decimal? receivedQuantity, bool updateNote, string note)
        {
            var success = true;
            var message = "Succeeded";
            var lastUpdatedBy = string.Empty;
            if(updateNote)
            {
                var lineItem = _repositoryFactory.LineItemRepository.GetNullableById(lineItemId);
                if(lineItem == null)
                {
                    success = false;
                    message = "Line Item not found";
                    return new JsonNetResult(new { success, lineItemId, message, lastUpdatedBy });
                }

                if(lineItem.Order.Id != id)
                {
                    success = false;
                    message = "Order Id does not match";
                    return new JsonNetResult(new { success, lineItemId, message, lastUpdatedBy });
                }

                if(!lineItem.Order.StatusCode.IsComplete)
                {
                    success = false;
                    message = "Order is not complete";
                    return new JsonNetResult(new { success, lineItemId, message, lastUpdatedBy });
                }


                try
                {
                    var saveNote = lineItem.ReceivedNotes;
                    lineItem.ReceivedNotes = note;
                    _repositoryFactory.LineItemRepository.EnsurePersistent(lineItem);
                    var history = new HistoryReceivedLineItem();
                    history.LineItem = lineItem;
                    history.User = _repositoryFactory.UserRepository.Queryable.Single(a => a.Id == CurrentUser.Identity.Name);
                    history.CommentsUpdated = true;
                    history.OldReceivedQuantity = lineItem.QuantityReceived;
                    history.NewReceivedQuantity = lineItem.QuantityReceived;
                    if (lineItem.ReceivedNotes != saveNote)
                    {
                        _repositoryFactory.HistoryReceivedLineItemRepository.EnsurePersistent(history);
                        lastUpdatedBy = history.User.FullName;
                    }
                    message = "Updated";
                    success = true;
                }
                catch(Exception ex)
                {
                    success = false;
                    message = "There was a problem updating the notes."; //ex.Message;
                }

                return new JsonNetResult(new { success, lineItemId, message, lastUpdatedBy });
            }
            else
            {
                var showRed = false;
                var unaccounted = string.Empty;
                var receivedQuantityReturned = receivedQuantity.HasValue ? receivedQuantity.Value.ToString() : string.Empty;

                var lineItem = _repositoryFactory.LineItemRepository.GetNullableById(lineItemId);
                if (lineItem == null)
                {
                    success = false;
                    message = "Line Item not found";
                    return new JsonNetResult(new { success, lineItemId, receivedQuantity, message, showRed, unaccounted, lastUpdatedBy });
                }

                if (lineItem.Order.Id != id)
                {
                    success = false;
                    message = "Order Id does not match";
                    return new JsonNetResult(new { success, lineItemId, receivedQuantity, message, showRed, unaccounted, lastUpdatedBy });
                }

                if (!lineItem.Order.StatusCode.IsComplete)
                {
                    success = false;
                    message = "Order is not complete";
                    return new JsonNetResult(new { success, lineItemId, receivedQuantity, message, showRed, unaccounted, lastUpdatedBy });
                }

                try
                {
                    var history = new HistoryReceivedLineItem();
                    history.User = _repositoryFactory.UserRepository.Queryable.Single(a => a.Id == CurrentUser.Identity.Name);
                    history.OldReceivedQuantity = lineItem.QuantityReceived;
                    history.NewReceivedQuantity = receivedQuantity;
                    history.LineItem = lineItem;

                    lineItem.QuantityReceived = receivedQuantity;
                    _repositoryFactory.LineItemRepository.EnsurePersistent(lineItem);
                    if (history.NewReceivedQuantity != history.OldReceivedQuantity)
                    {
                        _repositoryFactory.HistoryReceivedLineItemRepository.EnsurePersistent(history);
                        lastUpdatedBy = history.User.FullName;
                    }
                    receivedQuantityReturned = string.Format("{0:0.000}", lineItem.QuantityReceived);
                    success = true;
                    message = "Updated";
                    var diff = lineItem.Quantity - lineItem.QuantityReceived;
                    if (diff > 0)
                    {
                        unaccounted = string.Format("({0})", string.Format("{0:0.000}", diff));
                        showRed = true;
                    }
                    else
                    {
                        unaccounted = string.Format("{0}", string.Format("{0:0.000}", (diff*-1)));
                        showRed = false;
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    message = "There was a problem updating the quantity."; //ex.Message;
                }
                return new JsonNetResult(new { success, lineItemId, receivedQuantityReturned, message, showRed, unaccounted, lastUpdatedBy });
            }
        }

        /// <summary>
        /// Get a list of people who have access to approve an order at a particular status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="orderStatusCodeId"></param>
        /// <returns></returns>
        public JsonNetResult GetPeeps (int id, string orderStatusCodeId)
        {
            var success = true;
            List<string> peeps = null;
            try
            {
                var order = _repositoryFactory.OrderRepository.Queryable.Single(a=> a.Id==id);
               peeps =
                    _queryRepository.OrderPeepRepository.Queryable.Where(
                        b =>
                        b.OrderId == id && b.WorkgroupId == order.Workgroup.Id &&
                        b.OrderStatusCodeId == orderStatusCodeId).Select(c=> c.Fullname).Distinct().ToList();
            }
            catch (Exception)
            {
                success = false;
                return new JsonNetResult(new {success, peeps});
            }
           return new JsonNetResult(new {success, peeps});
        }

        public JsonNetResult GetReceiveHistory (int id, int lineItemId)
        {
            var success = true;
            var history = new List<HistoryReceivedLineItem>();
            try
            {
                history = _repositoryFactory.HistoryReceivedLineItemRepository.Queryable.Where(a => a.LineItem.Id == lineItemId).OrderBy(a => a.UpdateDate).ToList();
            }
            catch (Exception)
            {
                success = false;
            }
               
            return new JsonNetResult(new { success, history = history.Select(a => new {a.User.FullName, updateDate = a.UpdateDate.ToString("MMM dd, yyyy - h:mm tt"), whatWasUpdated = a.CommentsUpdated ? "Comments" : "Quantity"})});
        }

        private List<string> GetInactiveAccountsForOrder(int id)
        {
            var orderAccounts = _repositoryFactory.SplitRepository.Queryable.Where(x => x.Order.Id == id && x.Account != null).Select(x => x.Account).ToArray();

            var inactiveAccounts =
                _repositoryFactory.AccountRepository.Queryable.Where(a => orderAccounts.Contains(a.Id) && !a.IsActive).
                    Select(x => x.Id).ToList();

            return inactiveAccounts;
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
            order.CreatedBy = order.CreatedBy ?? _repositoryFactory.UserRepository.GetById(CurrentUser.Identity.Name); //Only replace created by if it doesn't already exist
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

            if (model.Restricted != null && model.Restricted.IsRestricted)
            {
                var restricted = new ControlledSubstanceInformation
                {
                    
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
                                            : _repositoryFactory.CommodityRepository.GetNullableById(lineItem.CommodityCode);

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

                //TODO: note that I am not checking an order split actually has valid splits, or that they add to the total.
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
            var model = new OrderModifyModel
            {
                Order = new Order(),
                Workgroup = workgroup,
                Units = _repositoryFactory.UnitOfMeasureRepository.Queryable.Cache().ToList(),
                Accounts = _repositoryFactory.WorkgroupAccountRepository.Queryable.Where(x => x.Workgroup.Id == workgroup.Id).Select(x => x.Account).ToFuture(),
                Vendors = _repositoryFactory.WorkgroupVendorRepository.Queryable.Where(x => x.Workgroup.Id == workgroup.Id && x.IsActive).ToFuture(),
                Addresses = _repositoryFactory.WorkgroupAddressRepository.Queryable.Where(x => x.Workgroup.Id == workgroup.Id && x.IsActive).ToFuture(),
                ShippingTypes = _repositoryFactory.ShippingTypeRepository.Queryable.Cache().ToList(),
                Approvers = _repositoryFactory.WorkgroupPermissionRepository.Queryable.Where(x => x.Workgroup.Id == workgroup.Id && x.Role.Id == Role.Codes.Approver).Select(x => x.User).ToFuture(),
                AccountManagers = _repositoryFactory.WorkgroupPermissionRepository.Queryable.Where(x => x.Workgroup.Id == workgroup.Id && x.Role.Id == Role.Codes.AccountManager).Select(x => x.User).ToFuture(),
                ConditionalApprovals = workgroup.AllConditionalApprovals,
                CustomFields = _repositoryFactory.CustomFieldRepository.Queryable.Where(x => x.Organization.Id == workgroup.PrimaryOrganization.Id && x.IsActive).ToFuture().ToList() //call to list to exec the future batch
            };

            return model;
        }

        /// <summary>
        /// Calls the Campus Financial System to get updated status on order
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <returns></returns>
        public JsonNetResult GetKfsStatus(int id)
        {
            // load the order
            var order = _repositoryFactory.OrderRepository.GetNullableById(id);

            // make the call
            var result = _financialSystemService.GetOrderStatus(order.ReferenceNumber);

            return new JsonNetResult(result);
        }

        /// <summary>
        /// Search for building
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public JsonNetResult SearchBuilding(string term)
        {
            term = term.ToLower().Trim();

            var results = _repositoryFactory.SearchRepository.SearchBuildings(term);

            return new JsonNetResult(results.Select(a => new { id = a.Id, label = a.BuildingName }).ToList());
        }

        /// <summary>
        /// Save an order request
        /// </summary>
        /// <param name="saveId">Save Id, if just updating a save</param>
        /// <param name="saveName">Name to remember the save by</param>
        /// <param name="formData">Serialized form data</param>
        /// <param name="accountData">Serialized JSON of account info</param>
        /// <param name="preparedFor">Who can access saved form. If null, current user</param>
        /// <param name="workgroupId">Workgroup the save is associated with</param>
        /// <returns></returns>
        public JsonNetResult SaveOrderRequest(string saveId, string saveName, string formData, string accountData, string preparedFor, int workgroupId)
        {
            bool newSave = false;
            var user = string.IsNullOrWhiteSpace(preparedFor) ? CurrentUser.Identity.Name : preparedFor;

            var requestSave = _repositoryFactory.OrderRequestSaveRepository.GetNullableById(new Guid(saveId));

            if (requestSave == null)
            {
                newSave = true;
                requestSave = new OrderRequestSave(new Guid(saveId));
            }
            
            requestSave.Name = saveName;
            requestSave.User = _repositoryFactory.UserRepository.GetById(user);
            requestSave.PreparedBy = _repositoryFactory.UserRepository.GetById(CurrentUser.Identity.Name);
            requestSave.Workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(workgroupId);
            requestSave.FormData = formData;
            requestSave.AccountData = accountData;
            requestSave.LastUpdate = DateTime.Now;

            var version = ControllerContext.HttpContext.Cache["Version"] as string;
            requestSave.Version = version ?? "N/A";

            _repositoryFactory.OrderRequestSaveRepository.EnsurePersistent(requestSave, newSave);

            Message = "Order Saved Successfully";

            return new JsonNetResult(new {success = true, redirect = Url.Action("Landing", "Home")});
        }

        public ActionResult SavedOrderRequests()
        {
            var saves = _repositoryFactory.OrderRequestSaveRepository.Queryable.Where(a => a.User.Id == CurrentUser.Identity.Name);

            return View(saves);
        }
    }
}