﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Mvc.App_GlobalResources;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Services;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using Purchasing.WS;
using Serilog;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using UCDArch.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Purchasing.Mvc.Helpers;
using Purchasing.Core.Services;
using AggieEnterpriseApi.Validation;
using NHibernate.Util;

namespace Purchasing.Mvc.Controllers
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
        private readonly IEventService _eventService;
        private readonly IBugTrackingService _bugTrackingService;
        private readonly IFileService _fileService;
        private readonly IMemoryCache _memoryCache;
        private readonly IAggieEnterpriseService _aggieEnterpriseService;

        public OrderController(
            IRepositoryFactory repositoryFactory, 
            IOrderService orderService, 
            ISecurityService securityService, 
            IDirectorySearchService directorySearchService, 
            IFinancialSystemService financialSystemService,
            IQueryRepositoryFactory queryRepository,
            IEventService eventService,
            IBugTrackingService bugTrackingService, 
            IFileService fileService,
            IMemoryCache memoryCache,
            IAggieEnterpriseService aggieEnterpriseService)
        {
            _orderService = orderService;
            _repositoryFactory = repositoryFactory;
            _securityService = securityService;
            _directorySearchService = directorySearchService;
            _financialSystemService = financialSystemService;
            _queryRepository = queryRepository;
            _eventService = eventService;
            _bugTrackingService = bugTrackingService;
            _fileService = fileService;
            _memoryCache = memoryCache;
            _aggieEnterpriseService = aggieEnterpriseService;
        }

        /// <summary>
        /// #1
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return this.RedirectToAction("Index", "History");
        }

        /// <summary>
        /// If user has more than one workgroup, they select it for their order
        /// #2
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
                return this.RedirectToAction(nameof(Request), new { id = workgroup.Id });
            }
            
            return View(workgroups.OrderBy(a => a.Name).ToList());
        }


        // Task put on hold.
        //public ActionResult SharedOrder (string linkId)
        //{
        //    //var order 
        //    return View();
        //}

        [AuthorizeEditOrder]
        public ActionResult RerouteAccountManager(int id, int approvalId)
        {
            var approval = _repositoryFactory.ApprovalRepository.Queryable.Single(a => a.Id == approvalId);
            Check.Require(approval.Order.Id == id);

            if (approval.Order.StatusCode.Id != OrderStatusCode.Codes.AccountManager)
            {
                ErrorMessage = "Order Status must be at account manager to change account manager.";
                return this.RedirectToAction(nameof(Review), new { id = id });
            }
            var model = OrderReRoutePurchaserModel.Create(approval.Order);
            model.ApprovalId = approvalId;

            model.PurchaserPeeps = approval.Order.Workgroup.Permissions.Where(a => a.Role.Id == Role.Codes.AccountManager).Select(b => b.User).Distinct().OrderBy(c => c.LastName).ToList();
            model.Order = approval.Order;
            return View(model);
        }

        [AuthorizeEditOrder]
        [HttpPost]
        public ActionResult RerouteAccountManager(int id, int approvalId, string accountManagerId)
        {
            var approval = _repositoryFactory.ApprovalRepository.Queryable.Single(a => a.Id == approvalId);
            Check.Require(approval.Order.Id == id, "approval.Order.Id");

            if (approval.Order.StatusCode.Id != OrderStatusCode.Codes.AccountManager)
            {
                ErrorMessage = "Order Status must be at account manager to change account manager.";
                return this.RedirectToAction(nameof(Review), new { id = id });
            }

            if (approval.User != null && approval.User.Id == accountManagerId)
            {
                ErrorMessage = "No change detected.";
                return this.RedirectToAction(nameof(Review), new { id = id });
            }

            if (approval.User != null && approval.User.Id != CurrentUser.Identity.Name && !approval.User.IsAway)
            {
                ErrorMessage = "Can't reroute an approval unless it is assigned to you or the assigned user is away";
                return this.RedirectToAction(nameof(Review), new { id = id });
            }

            var originalRouting = approval.User != null ? approval.User.FullName : "Any Workgroup Account Manager";

            var accountManager = _repositoryFactory.UserRepository.Queryable.Single(a => a.Id == accountManagerId);
            var accountManagerCheck = approval.Order.Workgroup.Permissions.Any(a => a.Role.Id == Role.Codes.AccountManager && a.User == accountManager);
            Check.Require(accountManagerCheck, "accountManagerCheck"); // Check that the AM assigned is in the workgroup as an account manager.

            _orderService.ReRouteSingleApprovalForExistingOrder(approval, accountManager, (approval.Order.StatusCode.Id == OrderStatusCode.Codes.AccountManager));
            _eventService.OrderReRoutedToAccountManager(approval.Order, string.Empty, originalRouting, accountManager.FullName);
            _repositoryFactory.ApprovalRepository.EnsurePersistent(approval);

            Message = string.Format("Order {0} rerouted to Account Manager {1}", approval.Order.RequestNumber, accountManager.FullName);
            return this.RedirectToAction(nameof(HomeController.Landing), typeof(HomeController).ControllerName()); //May not have access to it anymore
        }

        /// <summary>
        /// Change the Purchaser assignment for an order.
        /// #3
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <returns></returns>
        [AuthorizeEditOrder]
        public ActionResult ReroutePurchaser(int id)
        {
            var order = _repositoryFactory.OrderRepository.Queryable.Single(a => a.Id == id);
            if (!(order.StatusCode.Id == OrderStatusCode.Codes.Purchaser || order.StatusCode.Id == OrderStatusCode.Codes.AccountManager))
            {
                ErrorMessage = "Order Status must be at account manager or purchaser to change purchaser.";
                return this.RedirectToAction(nameof(Review), new { id = id });
            }
            //if (order.Approvals.Any(a=> a.StatusCode.Id == OrderStatusCode.Codes.Purchaser && a.User!=null))
            //{
            //    ErrorMessage = "Order purchaser can not already be assigned to change purchaser.";
            //    return this.RedirectToAction(nameof(Review));
            //}
            var model = OrderReRoutePurchaserModel.Create(order);
            //var purchaserPeepsIds =
            //       _queryRepository.OrderPeepRepository.Queryable.Where(
            //           b =>
            //           b.OrderId == id && b.WorkgroupId == order.Workgroup.Id &&
            //           b.OrderStatusCodeId == OrderStatusCode.Codes.Purchaser).Select(c => c.UserId).Distinct().ToList();

            model.PurchaserPeeps = order.Workgroup.Permissions.Where(a => a.Role.Id == Role.Codes.Purchaser).Select(b => b.User).Distinct().OrderBy(c => c.LastName).ToList();
            model.Order = order;
            return View(model);
            
        }

        /// <summary>
        /// #4
        /// </summary>
        /// <param name="id"></param>
        /// <param name="purchaserId"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeEditOrder]
        public ActionResult ReroutePurchaser(int id, string purchaserId)
        {
            var order = _repositoryFactory.OrderRepository.Queryable.Single(a => a.Id == id);
            if (!(order.StatusCode.Id == OrderStatusCode.Codes.Purchaser || order.StatusCode.Id == OrderStatusCode.Codes.AccountManager))
            {
                ErrorMessage = "Order Status must be at account manager or purchaser to change purchaser.";
                return this.RedirectToAction(nameof(Review), new { id = id });
            }
            //if (order.Approvals.Any(a => a.StatusCode.Id == OrderStatusCode.Codes.Purchaser && a.User != null))
            //{
            //    ErrorMessage = "Order purchaser can not already be assigned to change purchaser.";
            //    return this.RedirectToAction(nameof(Review), new { id = id });
            //}
            var purchaser = _repositoryFactory.UserRepository.Queryable.Single(a => a.Id == purchaserId);
            //var peepCheck = _queryRepository.OrderPeepRepository.Queryable.Any(a => a.OrderId == order.Id && a.WorkgroupId == order.Workgroup.Id && a.OrderStatusCodeId == OrderStatusCode.Codes.Purchaser && a.UserId == purchaserId);
            var purchaserCheck = order.Workgroup.Permissions.Any(a => a.Role.Id == Role.Codes.Purchaser && a.User == purchaser);
            Check.Require(purchaserCheck); // Check that the purchaser assigned is either in the peeps view or in the workgroup as a purchaser.
            
            var approval = order.Approvals.Single(a => a.StatusCode.Id == OrderStatusCode.Codes.Purchaser);
            _orderService.ReRouteSingleApprovalForExistingOrder(approval, purchaser, (order.StatusCode.Id == OrderStatusCode.Codes.Purchaser));
            
            _eventService.OrderReRoutedToPurchaser(order, purchaser.FullName); //Adds the event to the order tracking.
            
            _repositoryFactory.ApprovalRepository.EnsurePersistent(approval);
            

            Message = string.Format("Order {0} rerouted to purchaser {1}", order.RequestNumber, purchaser.FullName);

            if (order.StatusCode.Id == OrderStatusCode.Codes.AccountManager)
            {
                return this.RedirectToAction(nameof(Review), new { id = order.Id });
            }

            return this.RedirectToAction(nameof(HomeController.Landing), typeof(HomeController).ControllerName());
        }

        /// <summary>
        /// Access checks done inside method.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AssignAccountsPayableUser(int id)
        {
            var order = _repositoryFactory.OrderRepository.Queryable.Single(a => a.Id == id);
            if (order.StatusCode.Id != OrderStatusCode.Codes.Complete)
            {
                ErrorMessage = "Order Status must be at complete to assign an Accounts Payable user.";
                return this.RedirectToAction(nameof(Review), new { id = id });
            }

            var completedBy = order.OrderTrackings.Where(a => a.StatusCode.Id == OrderStatusCode.Codes.Complete).OrderBy(o => o.DateCreated).First().User.Id;

            if (CurrentUser.Identity.Name != completedBy)
            {
                if (order.ApUser == null || order.ApUser.Id != CurrentUser.Identity.Name)
                {
                    ErrorMessage = "You do not have permission to assign an Accounts Payable user.";
                    return this.RedirectToAction(nameof(Review), new { id = id });
                }
            }

            var model = OrderReRoutePurchaserModel.Create(order); //Re-using this model. List reviewers though
            model.PurchaserPeeps = order.Workgroup.Permissions.Where(a => a.Role.Id == Role.Codes.Reviewer).Select(b => b.User).Distinct().OrderBy(c => c.LastName).ToList();


            model.Order = order;
            return View(model);
        }

        [HttpPost]
        public ActionResult AssignAccountsPayableUser(int id, string apUserId)
        {
            var order = _repositoryFactory.OrderRepository.Queryable.Single(a => a.Id == id);
            if (order.StatusCode.Id != OrderStatusCode.Codes.Complete)
            {
                ErrorMessage = "Order Status must be at complete to assign an Accounts Payable user.";
                return this.RedirectToAction(nameof(Review), new { id = id });
            }

            var completedBy = order.OrderTrackings.Where(a => a.StatusCode.Id == OrderStatusCode.Codes.Complete).OrderBy(o => o.DateCreated).First().User.Id;

            if (CurrentUser.Identity.Name != completedBy)
            {
                if (order.ApUser == null || order.ApUser.Id != CurrentUser.Identity.Name)
                {
                    ErrorMessage = "You do not have permission to assign an Accounts Payable user.";
                    return this.RedirectToAction(nameof(Review), new { id = id });
                }
            }

            if (string.IsNullOrWhiteSpace(apUserId))
            {
                order.ApUser = null;
                _eventService.OrderUpdated(order, "Accounts Payable user cleared");
            }
            else
            {
                var user = _repositoryFactory.UserRepository.Queryable.Single(a => a.Id == apUserId.Trim().ToLower());
                order.ApUser = user;
                _eventService.OrderUpdated(order, string.Format("Accounts Payable user set to {0}", user.FullName));
            }
           
            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            return this.RedirectToAction(nameof(Review), new { id = id });
        }

        /// <summary>
        /// Make an order request
        /// #5
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public new ActionResult Request(int id)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetNullableById(id);

            if (workgroup == null || !workgroup.IsActive)
            {
                ErrorMessage = workgroup == null ? "workgroup not found." : "workgroup not active.";
                return this.RedirectToAction(nameof(SelectWorkgroup));                
            }

            var requesterInWorkgroup = _repositoryFactory.WorkgroupPermissionRepository
                .Queryable.Where(x => x.Workgroup.Id == id && x.User.Id == CurrentUser.Identity.Name)
                .Where(x => x.Role.Id == Role.Codes.Requester);

            if (!requesterInWorkgroup.Any())
            {
                ErrorMessage = Resources.NoAccess_Workgroup;
                return this.RedirectToAction(nameof(ErrorController.NotAuthorized), typeof(ErrorController).ControllerName());
            }

            var model = CreateOrderModifyModel(workgroup);

            return View(model);
        }

        /// <summary>
        /// #6
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [RequestFormLimits(ValueCountLimit = int.MaxValue)]
        public new async Task<ActionResult> Request(OrderViewModel model)
        {
            var canCreateOrderInWorkgroup =
                _securityService.HasWorkgroupAccess(_repositoryFactory.WorkgroupRepository.GetById(model.Workgroup));

            Check.Require(canCreateOrderInWorkgroup, "canCreateOrderInWorkgroup");

            var order = new Order();

            BindOrderModel(order, model, includeLineItemsAndSplits: true);

            order.Tag = order.Workgroup.DefaultTag;

            await _orderService.CreateApprovalsForNewOrder(order, accountId: model.Account, approverId: model.Approvers, accountManagerId: model.AccountManagers, conditionalApprovalIds: model.ConditionalApprovals);

            _orderService.HandleSavedForm(order, model.FormSaveId);

            Check.Require(order.LineItems.Count > 0, "Line Items less than 1");

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            //Do attachment stuff Note, must be done by persisting the attachment directly, not adding it to the order and having the order save it, otherwise we get a transient exception if we have to add a new user because of an external account
            if (model.FileIds != null)
            {
                foreach (var fileId in model.FileIds.Where(x => !Guid.Empty.Equals(x)))
                {
                    var attachment = _repositoryFactory.AttachmentRepository.GetById(fileId);
                    attachment.Order = order;
                    _repositoryFactory.AttachmentRepository.EnsurePersistent(attachment);
                }
            }

            Message = Resources.NewOrder_Success;

            //return RedirectToAction("Review", new { id = order.Id });
            return this.RedirectToAction(nameof(Review), new { id = order.Id });
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
        [RequestFormLimits(ValueCountLimit = int.MaxValue)]
        public async Task<ActionResult> Edit(int id, OrderViewModel model)
        {
            var order = _repositoryFactory.OrderRepository.GetNullableById(id);

            Check.Require(order != null, "order is null");

            var adjustRouting = model.AdjustRouting.HasValue && model.AdjustRouting.Value;

            List<string> existingAccounts = new List<string>();
            if (adjustRouting)
            {
                foreach (var split in order.Splits)
                {
                    existingAccounts.Add(split.FullAccountDisplay);
                }
            }
            BindOrderModel(order, model, includeLineItemsAndSplits: adjustRouting);

            if (adjustRouting)
            {
                // Do we really need to adjust the routing?
                if (order.StatusCode.Id == OrderStatusCode.Codes.Purchaser)
                {
                    List<string> accountsNow = new List<string>();
                    foreach (var split in order.Splits)
                    {
                        accountsNow.Add(split.FullAccountDisplay);
                    }
                   
                    if (!accountsNow.Except(existingAccounts).Union( existingAccounts.Except(accountsNow) ).Any())
                    {
                        adjustRouting = false;
                    }
                }
            }

            Check.Require(order.LineItems.Count > 0, "Line count");

            if(adjustRouting)
            {
            //TODO: Add expense validation
                //order.ValidateExpenses().ToArray();

                await _orderService.ReRouteApprovalsForExistingOrder(order, approverId: model.Approvers, accountManagerId: model.AccountManagers);
            }
            else
            {
                _orderService.EditExistingOrder(order);
            }

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            //Do attachment stuff Note, must be done by persisting the attachment directly, not adding it to the order and having the order save it, otherwise we get a transient exception if we have to add a new user because of an external account
            if (model.FileIds != null)
            {
                foreach (var fileId in model.FileIds.Where(x => !Guid.Empty.Equals(x)))
                {
                    var attachment = _repositoryFactory.AttachmentRepository.GetById(fileId);
                    attachment.Order = order;
                    _repositoryFactory.AttachmentRepository.EnsurePersistent(attachment);
                }
            }

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

            if (order.Workgroup == null || !order.Workgroup.IsActive)
            {
                ErrorMessage = order.Workgroup == null ? "workgroup not found." : "workgroup not active.";
                return this.RedirectToAction(nameof(SelectWorkgroup));
            }

            var requesterInWorkgroup = _repositoryFactory.WorkgroupPermissionRepository
                .Queryable.Where(x => x.Workgroup.Id == order.Workgroup.Id && x.User.Id == CurrentUser.Identity.Name)
                .Where(x => x.Role.Id == Role.Codes.Requester);

            if (!requesterInWorkgroup.Any())
            {
                ErrorMessage = Resources.NoAccess_Workgroup;
                return this.RedirectToAction(nameof(ErrorController.NotAuthorized), typeof(ErrorController).ControllerName());
            }

            var model = CreateOrderModifyModel(order.Workgroup);
            model.IsCopyOrder = true;
            model.Order = order;
            model.Order.Attachments.Clear(); //Clear out attachments so they don't get included w/ copied order
            model.Order.DateNeeded = DateTime.MinValue;
           

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
        [RequestFormLimits(ValueCountLimit = int.MaxValue)]
        public async Task<ActionResult> Copy(int id, OrderViewModel model)
        {
            var canCreateOrderInWorkgroup =
                _securityService.HasWorkgroupAccess(_repositoryFactory.WorkgroupRepository.GetById(model.Workgroup));

            Check.Require(canCreateOrderInWorkgroup, "canCreateOrderInWorkgroup");

            var order = new Order();

            BindOrderModel(order, model, includeLineItemsAndSplits: true);

            order.Tag = order.Workgroup.DefaultTag;
            
            await _orderService.CreateApprovalsForNewOrder(order, accountId: model.Account, approverId: model.Approvers, accountManagerId: model.AccountManagers, conditionalApprovalIds: model.ConditionalApprovals);

            Check.Require(order.LineItems.Count > 0, "line count");

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            //Do attachment stuff Note, must be done by persisting the attachment directly, not adding it to the order and having the order save it, otherwise we get a transient exception if we have to add a new user because of an external account
            if (model.FileIds != null)
            {
                foreach (var fileId in model.FileIds.Where(x => !Guid.Empty.Equals(x)))
                {
                    var attachment = _repositoryFactory.AttachmentRepository.GetById(fileId);
                    attachment.Order = order;
                    _repositoryFactory.AttachmentRepository.EnsurePersistent(attachment);
                }
            }

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
        //[AuthorizeReadOrEditOrder] //Doing access directly to avoid duplicate permissions check
        public async Task<ActionResult> Review(int id)
        {
            var orderQuery =
                _repositoryFactory.OrderRepository.Queryable.Where(x => x.Id == id)
                                  .Fetch(x => x.StatusCode)
                                  .Fetch(x => x.Workgroup)
                                  .Fetch(x => x.Organization)
                                  .Single();

            var fav = _repositoryFactory.FavoriteRepository.Queryable
                .Where(x => x.User.Id == CurrentUser.Identity.Name && x.Order.Id == id)
                .SingleOrDefault();
            if(fav == null)
            {
                fav = new Favorite
                {
                    User = _repositoryFactory.UserRepository.GetNullableById(CurrentUser.Identity.Name),
                    Order = orderQuery,
                    IsActive = false
                };
            }

            var model = new ReviewOrderViewModel
                {
                    Order = orderQuery,
                    Favorite = fav,
                    Complete = orderQuery.StatusCode.IsComplete,
                    Status = orderQuery.StatusCode.Name,
                    WorkgroupName = orderQuery.Workgroup.Name,
                    WorkgroupForceAccountApprover = orderQuery.Workgroup.ForceAccountApprover,
                    OrganizationName = orderQuery.Organization.Name,
                    CurrentUser = CurrentUser.Identity.Name,
                };

            const OrderAccessLevel requiredAccessLevel = OrderAccessLevel.Edit | OrderAccessLevel.Readonly;
            var roleAndAccessLevel = _securityService.GetAccessRoleAndLevel(model.Order);

            if (!requiredAccessLevel.HasFlag(roleAndAccessLevel.OrderAccessLevel))
            {
                return ViewHelper.NotAuthorized(Resources.Authorization_PermissionDenied);
            }
            
            model.Vendor = _repositoryFactory.OrderRepository.Queryable.Where(x=>x.Id == id).Select(x=>x.Vendor).Single();
            if(model.Vendor != null && !string.IsNullOrWhiteSpace( model.Vendor.AeSupplierNumber ))
            {
                var aeVendor = await _aggieEnterpriseService.GetSupplier(model.Vendor);
                if(aeVendor != null)
                {
                    model.Vendor.IsValidInAggieEnterprise = true;
                }
                else
                {
                    model.Vendor.IsValidInAggieEnterprise = false;
                }
            }
            model.Address = _repositoryFactory.OrderRepository.Queryable.Where(x=>x.Id == id).Select(x=>x.Address).Single();
            model.LineItems =
                _repositoryFactory.LineItemRepository.Queryable.Fetch(x => x.Commodity).Where(x => x.Order.Id == id).ToList();
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
                _repositoryFactory.ApprovalRepository.Queryable.Fetch(x => x.StatusCode).Where(x => x.Order.Id == id).ToList();

            model.Comments =
                _repositoryFactory.OrderCommentRepository.Queryable.Fetch(x => x.User).Where(x => x.Order.Id == id).OrderBy(o => o.DateCreated).ToList();
            model.Attachments =
                _repositoryFactory.AttachmentRepository.Queryable.Fetch(x => x.User).Where(x => x.Order.Id == id).OrderBy(o => o.DateCreated).ToList();

            model.OrderTracking =
                _repositoryFactory.OrderTrackingRepository.Queryable.Fetch(x => x.StatusCode).Fetch(x => x.User).Where(
                    x => x.Order.Id == id).ToList().ToList();

            model.IsRequesterInWorkgroup = _repositoryFactory.WorkgroupPermissionRepository.Queryable
                .Any(
                    x =>
                    x.Workgroup.Id == model.Order.Workgroup.Id && x.Role.Id == Role.Codes.Requester &&
                    x.User.Id == CurrentUser.Identity.Name);

            if (model.Complete){   //complete orders can't ever be edited or cancelled so just return now
                if (model.Order.StatusCode.Id == OrderStatusCode.Codes.Complete && model.Order.Workgroup.Permissions.Any(a => a.Role.Id == Role.Codes.Purchaser && a.User.Id == CurrentUser.Identity.Name))
                {
                    model.CanCancelCompletedOrder = true;
                }
                return View(model);
            }

            model.CanEditOrder = roleAndAccessLevel.OrderAccessLevel == OrderAccessLevel.Edit;
            model.CanCancelOrder = model.Order.CreatedBy.Id == CurrentUser.Identity.Name; //Can cancel the order if you are the one who created it
            model.IsApprover = model.Order.StatusCode.Id == OrderStatusCode.Codes.Approver;
            model.IsPurchaser = model.Order.StatusCode.Id == OrderStatusCode.Codes.Purchaser;
            model.IsAccountManager = model.Order.StatusCode.Id == OrderStatusCode.Codes.AccountManager;
            model.UserRoles = roleAndAccessLevel.Roles;

            if (model.CanEditOrder)
            {
                if (model.IsAccountManager || model.IsApprover) //need to check if there are associated accounts
                {
                    //I don't think this is used anywhere?
                    model.HasAssociatedAccounts =
                        _repositoryFactory.SplitRepository.Queryable
                            .Any(s => s.Order.Id == model.Order.Id && (s.Account != null || s.FinancialSegmentString != null));
                }

                if (model.IsAccountManager) //TODO: Get Scott to review this to make sure things like toFuture or other performance things are ok.
                {
                    //For orders in the account manager stage, where the approval is not external and the user is null, away, or a match the the current user, allow them to reroute.
                    //Now, this will let an external approver reroute, but they will only be able to reroute to a list of users in the workgroup...
                    model.ReRouteAbleAccountManagerApprovals =
                        model.Approvals.Where(
                            a =>
                            a.StatusCode.Id == OrderStatusCode.Codes.AccountManager && !a.IsExternal &&
                            (a.User == null || a.User.Id == CurrentUser.Identity.Name || a.User.IsAway)).ToList();

                }

                if (model.IsPurchaser)
                {
                    model.OrderTypes = _repositoryFactory.OrderTypeRepository.Queryable.Where(x => x.PurchaserAssignable).ToList();
                }

                //var app = from a in _repositoryFactory.ApprovalRepository.Queryable
                //          where a.Order.Id == id && a.StatusCode.Level == a.Order.StatusCode.Level
                //                && a.Split != null && a.Split.Account != null && a.StatusCode.Id != OrderStatusCode.Codes.ConditionalApprover
                //                && (!_repositoryFactory.WorkgroupAccountRepository.Queryable.Any(
                //                  x => x.Workgroup.Id == model.Order.Workgroup.Id && x.Account.Id == a.Split.Account))
                //          select a;

                //model.ExternalApprovals = app.ToList();
                model.ExternalApprovals = model.Order.Approvals.Where(a => a.IsExternal && !a.Completed).ToList();
            }

            var externalApprovalIds = model.ExternalApprovals.Select(x => x.Id);
            var internalApprovals = Approval.FilterUnique(model.Approvals.Where(x => !externalApprovalIds.Contains(x.Id)).ToList());

            //Takes the external approvals and unions them with the unique internal approvals
            model.OrderedUniqueApprovals =
                internalApprovals.Union(model.ExternalApprovals).OrderBy(a => a.StatusCode.Level);

            var approvalUserIds =
                model.OrderedUniqueApprovals.Where(x => x.User != null).Select(x => x.User.Id).Union(
                    model.OrderedUniqueApprovals.Where(x => x.SecondaryUser != null).Select(x => x.SecondaryUser.Id)).ToArray();

            model.ApprovalUsers =
                _repositoryFactory.UserRepository.Queryable.Where(x => approvalUserIds.Contains(x.Id)).ToList();

            if (model.CanSubmitOrder)
            {
                //So this means that
                //1 the order is not in the completed status (there is an early return above)
                //2 there are accounts or it is at the purchaser level.
                // So lets validate all the accounts...

                var uniqueFinancialSegments = model.Order.Splits.Where(x => x.FinancialSegmentString != null).Select(x => x.FinancialSegmentString).Distinct().ToList();
                var uniqueKfsAccounts = model.Order.Splits.Where(a => a.FinancialSegmentString == null  && a.KfsAccount != null && a.KfsAccount != string.Empty).Select(a => a.KfsAccount).Distinct().ToList();
                var validationDict = new Dictionary<string, string>();
                var validationWarningsDict = new Dictionary<string, string>();
                if (uniqueKfsAccounts.Any())
                {
                    foreach (var account in uniqueKfsAccounts)
                    {
                        var convertedAccount = await _aggieEnterpriseService.ConvertKfsAccount(account);
                        if(string.IsNullOrWhiteSpace(convertedAccount))
                        {
                            validationDict.Add(account, "Account not found in AE conversion");
                        }
                        else
                        {
                            var validated = await _aggieEnterpriseService.ValidateAccount(convertedAccount);
                            if (!validated.IsValid)
                            {
                                validationDict.Add(account, $"Converted KFS account ({convertedAccount}) has validation errors in AE: {validated.Message}");
                            }
                            if(validated.Warnings.Any())
                            {
                                validationWarningsDict.Add(account, $"Converted KFS account ({convertedAccount}) has validation warnings in AE: {string.Join(", ", validated.Warnings)}");
                            }
                        }
                    }
                    validationWarningsDict.Add("KFS Accounts", "<h2>KFS Accounts are being used. On July 1, 2025 these will stop being automatically converted and the order will not be able to be approved without picking a Chart String.</h2>");
                }
                if (uniqueFinancialSegments.Any())
                {
                    foreach (var financialSegment in uniqueFinancialSegments)
                    {
                        var validated = await _aggieEnterpriseService.ValidateAccount(financialSegment);
                        if (!validated.IsValid)
                        {
                            validationDict.Add(financialSegment, validated.Message);
                        }
                        if(validated.Warnings.Any())
                        {
                            validationWarningsDict.Add(financialSegment, string.Join(", ", validated.Warnings));
                        }
                    }
                }
                if (validationDict.Any())
                {
                    var msg = "The following accounts/CoA are invalid: <br/>";
                    foreach (var  item in validationDict)
                    {
                        msg = $"{msg}Account/CoA:{item.Key}<br/>Error: {item.Value}<br/><br/>";
                    }

                    model.HasInvalidAccounts = true;
                    model.InvalidAccountsMessage = msg;
                }
                if(validationWarningsDict.Any())
                {
                    var msg = "The following accounts/CoA have warnings: <br/>";
                    foreach (var item in validationWarningsDict)
                    {
                        msg = $"{msg}Account/CoA:{item.Key}<br/>Warning: {item.Value}<br/><br/>";
                    }

                    model.AccountsWarningsMessage = msg;
                }
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
                Message = "Order Not Found";
                return this.RedirectToAction(nameof(SearchController.Index), typeof(SearchController).ControllerName());
            }

            OrderAccessLevel accessLevel;

            using (var ts = new TransactionScope())
            {
                accessLevel = _securityService.GetAccessLevel(relatedOrderId);
                ts.CommitTransaction();
            }
            if (accessLevel != OrderAccessLevel.Edit && accessLevel != OrderAccessLevel.Readonly)
            {
                if (
                    Repository.OfType<EmailQueueV2>().Queryable.Any(
                        a => a.Order.Id == relatedOrderId && a.User.Id.ToLower() == CurrentUser.Identity.Name.ToLower()))
                {
                    var order = _repositoryFactory.OrderRepository.Queryable.Single(a => a.Id == relatedOrderId);
                    if (order.StatusCode.Id == OrderStatusCode.Codes.Cancelled)
                    {
                        Message = "This order has been cancelled";
                    }
                    else if (order.StatusCode.Id == OrderStatusCode.Codes.Denied)
                    {
                        Message = "This order has been denied";
                    }
                    else if (order.StatusCode.IsComplete)
                    {
                        Message = "This order has been completed";
                    }
                    else
                    {
                        var person = string.Empty;
                        var approval =
                            order.Approvals.Where(a => !a.Completed).OrderBy(b => b.StatusCode.Level).FirstOrDefault();
                        if (approval == null || approval.User == null)
                        {
                            person = "Anyone in the workgroup";
                        }
                        else
                        {
                            person = approval.User.FullName;
                        }
                        Message = string.Format("This order is currently being handled by {0} in the status {1}", person,
                                                order.StatusCode.Name);
                    }
                    return this.RedirectToAction(nameof(ErrorController.NotAuthorized), typeof(ErrorController).ControllerName());
                }
            }


            return RedirectToAction("Review", new {id = relatedOrderId});
        }

        [HttpPost]
        [AuthorizeEditOrder]
        public async Task<ActionResult> Approve(int id /*order*/, string action, string comment, string orderType, string kfsDocType)
        {
            var order =
                _repositoryFactory.OrderRepository.Queryable.Fetch(x => x.Approvals).Single(x => x.Id == id);
            
            if (!string.IsNullOrWhiteSpace(comment))
            {
                if (action == "Deny")
                {
                    order.AddComment(new OrderComment { Text = string.Format("Denied: {0}", comment), User = GetCurrentUser() });
                }
                else
                {
                    order.AddComment(new OrderComment { Text = comment, User = GetCurrentUser() });
                }
                
            }

            if (action == "Approve")
            {
                if (!_orderService.Approve(order))
                {
                    //No Approvals happened
                    Message = string.Format(Resources.ApprovalAction_Fail, action, CurrentUser.Identity.Name);
                    return RedirectToAction("Review", new { id });
                }
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

                Check.Require(isPurchaser, "isPurchaser");
                Check.Require(newOrderType != null, "new order type");
                
                newOrderType.DocType = kfsDocType;

                if (_orderService.WillOrderBeSentToKfs(newOrderType, kfsDocType))
                {
                    //Specific checks for KFS orders
                    if (order.LineItems.Any(a => a.Commodity == null))
                    {
                        ErrorMessage = "Must have commodity codes for all line items to complete a KFS order";
                        return RedirectToAction("Review", new { id });
                    }

                    if (order.LineItems.Any(a => a.Commodity != null && !a.Commodity.IsActive))
                    {
                        var inactiveCodes = string.Empty;
                        foreach (var invalidLineItem in order.LineItems.Where(a => a.Commodity != null && !a.Commodity.IsActive))
                        {
                            inactiveCodes = string.Format("{0} Commodity Code: {1} ", inactiveCodes, invalidLineItem.Commodity.Id);
                        }
                        ErrorMessage = string.Format("Inactive (old) commodity codes detected. Please update to submit to KFS: {0}", inactiveCodes);
                        return RedirectToAction("Review", new { id });
                    }

                    //if (order.Address.BuildingCode == null)
                    //{
                    //    ErrorMessage = "Shipping Address needs to have a building code to complete a KFS order";
                    //    return RedirectToAction("Review", new { id });
                    //}
                }
                if(newOrderType.Id.Trim() == OrderType.Types.AggieEnterprise)
                {
                    //Copied from the KFS, these will change, but there will be something similar
                    if (order.LineItems.Any(a => a.Commodity == null))
                    {
                        ErrorMessage = "Must have commodity codes (Purchasing Category) for all line items to complete an Aggie Enterprise order";
                        return RedirectToAction("Review", new { id });
                    }
                    if (order.LineItems.Any(a => a.Commodity != null && !a.Commodity.IsActive))
                    {
                        var inactiveCodes = string.Empty;
                        foreach (var invalidLineItem in order.LineItems.Where(a => a.Commodity != null && !a.Commodity.IsActive))
                        {
                            inactiveCodes = string.Format("{0} Commodity Code: {1} ", inactiveCodes, invalidLineItem.Commodity.Id);
                        }
                        ErrorMessage = "Inactive (old) commodity codes (Purchasing Category) detected. Please update";
                        return RedirectToAction("Review", new { id });
                    }

                    if (string.IsNullOrWhiteSpace(order.Vendor.AeSupplierNumber) || string.IsNullOrWhiteSpace(order.Vendor.AeSupplierSiteCode))
                    {
                        //Do more checks...
                        if (order.Vendor == null || order.Vendor.VendorId == null)
                        {
                            ErrorMessage = "Must have a Campus vendor (supplier) to complete an Aggie Enterprise order";
                            return RedirectToAction("Review", new { id });
                        }
                        var supplierInfo = await _aggieEnterpriseService.GetSupplier(order.Vendor); //This may update the Vendor to AE...
                        if (supplierInfo == null || string.IsNullOrWhiteSpace(supplierInfo.SupplierNumber) || string.IsNullOrWhiteSpace(supplierInfo.SupplierSiteCode))
                        {
                            ErrorMessage = "Unable to automatically convert KFS vendor to Campus Vendor. Please edit the order to add (Search) a new Campus Vendor to the order.";
                            return RedirectToAction("Review", new { id });
                        }
                        else
                        {
                            if(!supplierInfo.SupplierSiteCode.Contains("PUR"))
                            {
                                ErrorMessage = "Vendor (supplier) address site code must have PUR to complete an Aggie Enterprise order";
                                return RedirectToAction("Review", new { id });
                            }
                        }

                    }
                    else
                    {
                        if (!order.Vendor.AeSupplierSiteCode.Contains("PUR"))
                        {
                            ErrorMessage = "Vendor (supplier) address site code must have PUR to complete an Aggie Enterprise order";
                            return RedirectToAction("Review", new { id });
                        }
                    }

                }

                var errors = await _orderService.Complete(order, newOrderType, kfsDocType);

                if (errors.Any()) //if we have any errors, raise them in ELMAH and redirect back to the review page without saving change
                {
                    if (errors.Contains("Hide Errors"))
                    {
                        ErrorMessage =
                            "There was a problem completing this order. Please try again later and notify support if problems persist.";

                        Log.Error(string.Join(Environment.NewLine, errors));
                    }
                    else
                    {
                        ErrorMessage = string.Format("There was a problem completing this order. Details,  Error: {0}", string.Join(" Error: ", errors));
                    }
                    return RedirectToAction("Review", new {id});
                }
            }

            _repositoryFactory.OrderRepository.EnsurePersistent(order); //Save approval changes

            Message = string.Format(Resources.ApprovalAction_Success, action, order.RequestNumber);

            return RedirectToAction("Landing", "Home");
        }

        [HttpGet]
        public async Task<ActionResult> ViewAeErrorDetails(int id)
        {
            var model = new OrderAeErrorsViewModel();
            var order =
                _repositoryFactory.OrderRepository.Queryable.Fetch(x => x.Approvals).Single(x => x.Id == id);

            const OrderAccessLevel requiredAccessLevel = OrderAccessLevel.Edit | OrderAccessLevel.Readonly;
            var roleAndAccessLevel = _securityService.GetAccessRoleAndLevel(order);

            if (!requiredAccessLevel.HasFlag(roleAndAccessLevel.OrderAccessLevel))
            {
                return ViewHelper.NotAuthorized(Resources.Authorization_PermissionDenied);
            }

            model.Order = order;
        


            //Only do this part to set it back to edit or to show a button to set it back
            if (order.OrderType.Id.Trim() == OrderType.Types.AggieEnterprise.Trim())
            {
                var allowSetBack = _repositoryFactory.WorkgroupPermissionRepository.Queryable.Any(x => x.Workgroup.Id == order.Workgroup.Id && x.Role.Id == Role.Codes.Purchaser && x.User.Id == CurrentUser.Identity.Name);
                if (order.StatusCode.Id == OrderStatusCode.Codes.Complete && allowSetBack)
                {
                    model.AllowSetStatusBack = true;
                }
            }
            else
            {
                ErrorMessage = "This order was not completed as an Aggie Enterprise order.";
                return RedirectToAction("Review", new { id });
            }

            model.Errors = await _aggieEnterpriseService.LookupOracleErrors(order.ReferenceNumber);
            //Check if the person was the purchaser who completed this
            //Get the errors and add them to a view

            //Do I need to uncheck the approval that was completed? Probably

            return View(model);

        }

        [HttpPost] //Change back to a post
        public async Task<ActionResult> SetBackToPurchaserLevel(int id)
        {
            var order = _repositoryFactory.OrderRepository.Queryable.Fetch(x => x.Approvals).Single(x => x.Id == id);
            const OrderAccessLevel requiredAccessLevel = OrderAccessLevel.Edit | OrderAccessLevel.Readonly;
            var roleAndAccessLevel = _securityService.GetAccessRoleAndLevel(order);

            var allowAction = false;

            if (!requiredAccessLevel.HasFlag(roleAndAccessLevel.OrderAccessLevel))
            {
                return ViewHelper.NotAuthorized(Resources.Authorization_PermissionDenied);
            }

            if(order.StatusCode.Id != OrderStatusCode.Codes.Complete)
            {
                ErrorMessage = "This order is not in a completed state. Unable to set back to purchaser level.";
                return RedirectToAction("Review", new { id });
            }

            if (order.OrderType.Id.Trim() == OrderType.Types.AggieEnterprise.Trim())
            {
                if (_repositoryFactory.WorkgroupPermissionRepository.Queryable.Any(x => x.Workgroup.Id == order.Workgroup.Id && x.Role.Id == Role.Codes.Purchaser && x.User.Id == CurrentUser.Identity.Name))
                {
                    var temp = await _aggieEnterpriseService.LookupOrderStatus(order.ReferenceNumber);
                    if(temp.Status.ToUpper() == "ERROR")
                    {
                        allowAction = true;
                    }
                }
            }
            else
            {
                ErrorMessage = "This order was not completed as an Aggie Enterprise order and can't be set back.";
                return RedirectToAction("Review", new { id });
            }

            if (!allowAction)
            {
                ErrorMessage = "This order is not in an error state. Unable to set back to purchaser level.";
                return RedirectToAction("Review", new { id });
            }
            else
            {
                order.StatusCode = _repositoryFactory.OrderStatusCodeRepository.GetById(OrderStatusCode.Codes.Purchaser);
                order.ConsumerTrackingId = Guid.NewGuid(); //Need a new one if it gets past the AE API validation, but fails on the back end.
                _eventService.AeOrderSetBackToPurchaser(order);
                //Update Approval
                var approval = order.Approvals.Single(a => a.StatusCode != null && a.StatusCode.Id == Role.Codes.Purchaser);
                approval.Completed = false;

                _repositoryFactory.OrderRepository.EnsurePersistent(order);
                Message = "Order Set back to Purchaser level.";
                return RedirectToAction("Review", new { id });
            }

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

            if (order.StatusCode.IsComplete)
            {
                ErrorMessage = "Order was already completed. Unable to cancel. (You may want to contact the purchaser.)";
                return RedirectToAction("Review", new { id });
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                ErrorMessage = Resources.CancelOrder_CommentRequired;
                return RedirectToAction("Review", new {id});
            }

            order.AddComment(new OrderComment { Text = string.Format("Canceled: {0}", comment), User = GetCurrentUser() });

            _orderService.Cancel(order, comment);

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            Message = Resources.OrderCancelled_Success;

            return RedirectToAction("Review", "Order", new { id });
        }

        [HttpPost]
        public ActionResult FpdComplete(int id, bool? fpdCompleted)
        {
            var order = _repositoryFactory.OrderRepository.GetNullableById(id);

            Check.Require(order != null);

            order.FpdCompleted = fpdCompleted.HasValue && fpdCompleted.Value;

            _eventService.OrderUpdated(order, "FPD Status updated.");

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            Message = Resources.OrderFdp_Updated;

            return RedirectToAction("Review", "Order", new { id });
        }

        [HttpPost]
        public ActionResult CancelCompletedOrder(int id, string comment)
        {
            var order = _repositoryFactory.OrderRepository.GetNullableById(id);

            Check.Require(order != null);

            if(!order.Workgroup.Permissions.Any(a => a.Role.Id == Role.Codes.Purchaser && a.User.Id == CurrentUser.Identity.Name))
            {
                ErrorMessage = Resources.CancelOrder_NoAccess;
                return RedirectToAction("Review", new { id });
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                ErrorMessage = Resources.CancelOrder_CommentRequired;
                return RedirectToAction("Review", new { id });
            }

            order.AddComment(new OrderComment { Text = string.Format("Canceled: {0}", comment), User = GetCurrentUser() });

            _orderService.Cancel(order, comment);

            _eventService.OrderAddNote(order, comment);

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
            try
            {
                var approval = _repositoryFactory.ApprovalRepository.GetNullableById(approvalId);

                Check.Require(approval != null, "approve null");
                Check.Require(id == approval.Order.Id, "approve id no match");
                Check.Require(!approval.Completed, "not completed");
                Check.Require(approval.IsExternal, "external");
                Check.Require(!approval.Order.Workgroup.Accounts.Select(a => a.Account.Id).Contains(approval.Split.Account), Resources.ReRouteApproval_AccountError);
                var oldUserName = approval.User.FullName;
                var user = _securityService.GetUser(kerb);
                Check.Require(user != null, "user null");

                _orderService.ReRouteSingleApprovalForExistingOrder(approval, user);
                _eventService.OrderReRoutedToAccountManager(approval.Order, "external ", oldUserName, user.FullName);

                _repositoryFactory.ApprovalRepository.EnsurePersistent(approval);
                return Json(new { success = true, name = user.FullName });
            }
            catch (Exception)
            {
                return Json(new { success = false});

            }

        }

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public JsonNetResult AddComment(int id, string comment)
        {
            var order = Repository.OfType<Order>().GetNullableById(id);

            var orderComment = new OrderComment() {Text = comment, User = GetCurrentUser()};
            order.AddComment(orderComment);

            _eventService.OrderAddNote(order, comment);

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            return
                new JsonNetResult(
                    new {Date = DateTime.UtcNow.ToPacificTime().ToShortDateString(), Text = comment, User = orderComment.User.FullName});
        }

       

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public JsonNetResult UpdateReferenceNumber(int id, string referenceNumber)
        {
            //Get the matching order, and only if the order is complete
            var order =
                _repositoryFactory.OrderRepository.Queryable.Single(x => x.Id == id && x.StatusCode.IsComplete);

            var priorValue = order.ReferenceNumber;

            order.ReferenceNumber = referenceNumber;

            _eventService.OrderUpdated(order, string.Format("Reference # Updated. Prior Value: {0}", priorValue));

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            return new JsonNetResult(new {success = true, referenceNumber});
        }

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public JsonNetResult UpdatePoNumber(int id, string poNumber)
        {
            //Get the matching order, and only if the order is complete
            var order =
                _repositoryFactory.OrderRepository.Queryable.Single(x => x.Id == id && x.StatusCode.IsComplete);

            var priorValue = order.PoNumber;

            order.PoNumber = poNumber;

            _eventService.OrderUpdated(order, string.Format("PO # Updated. Prior Value: {0}", priorValue));

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            return new JsonNetResult(new { success = true, poNumber });
        }

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public JsonNetResult UpdateTag(int id, string tag)
        {
            //Get the matching order, and only if the order is complete
            var order =
                _repositoryFactory.OrderRepository.Queryable.Single(x => x.Id == id );

            var priorValue = order.Tag;

            order.Tag = tag;

            _eventService.OrderUpdated(order, string.Format("Tag Updated. Prior Value: {0}", priorValue));

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            return new JsonNetResult(new { success = true, tag });
        }

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public JsonNetResult UpdateNote(int id, string orderNote)
        {
            //Get the matching order, and only if the order is complete
            var order =
                _repositoryFactory.OrderRepository.Queryable.Single(x => x.Id == id && x.StatusCode.IsComplete);

            var priorValue = order.OrderNote;

            order.OrderNote = orderNote;

            _eventService.OrderUpdated(order, string.Format("Order Note Updated. Prior Value: {0}", priorValue));

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            return new JsonNetResult(new { success = true, orderNote });

        }

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public JsonNetResult UpdatePostStatus(int id, string postStatusValue)
        {
            
            //Get the matching order, and only if the order is complete
            var order =
                _repositoryFactory.OrderRepository.Queryable.Single(x => x.Id == id && x.StatusCode.IsComplete);

            var priorValue = order.PostStatus;

            order.PostStatus = postStatusValue;

            _eventService.OrderUpdated(order, string.Format("Order Post Status Updated. Prior Value: {0}", priorValue));

            _repositoryFactory.OrderRepository.EnsurePersistent(order);

            return new JsonNetResult(new { success = true, postStatusValue });

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
            //TODO: Do a CoA validation that is similar the call above

            //var workgroupAccounts = _repositoryFactory.WorkgroupAccountRepository.Queryable.Where(a => a.Workgroup.Id == id).ToArray();

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
                        Quantity = string.Format("{0:0.###}", x.Quantity),
                        Units = x.Unit,
                        Url = x.Url
                    })
                .ToList();

            //All the new Aggie Enterprise Accounts
            var AeAccountSplits = _repositoryFactory.SplitRepository.Queryable.Where(a => a.Order.Id == id && a.Account == null && a.FinancialSegmentString != null).Select(x =>
                new OrderViewModel.Split
                {
                    Account = x.FinancialSegmentString,
                    AccountName = x.Name,
                    Amount = x.Amount.ToString(CultureInfo.InvariantCulture),
                    LineItemId = x.LineItem == null ? 0 : x.LineItem.Id,
                    Project = x.Project, //Will always be null
                    SubAccount = x.SubAccount //Will always be null
                }).ToList();

            var splits = (from s in _repositoryFactory.SplitRepository.Queryable
                          join a in _repositoryFactory.AccountRepository.Queryable on s.Account equals a.Id
                          where s.Order.Id == id && s.Account != null
                          select new OrderViewModel.Split
                          {
                              Account = inactiveAccounts.Contains(a.Id) ? string.Empty : a.Id,
                              AccountName = a.Name,
                              Amount = s.Amount.ToString(CultureInfo.InvariantCulture),
                              LineItemId = s.LineItem == null ? 0 : s.LineItem.Id,
                              Project = s.Project,
                              SubAccount = s.SubAccount
                          }).ToList();

            if (AeAccountSplits.Any())
            {
                splits.AddRange(AeAccountSplits);
            }

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

        public async Task<ActionResult> ValidateCoA(string financialSegmentString)
        {
            var rtValue = await _aggieEnterpriseService.ValidateAccount(financialSegmentString);
            if (rtValue?.IsValid == true)
            {
                return Json(new { success = true });
            }
            if(rtValue == null)
            {
                return Json(new { success = false, message = "Invalid Account" });
            }

            return Json(new { success = false, message = rtValue.Message});

        }

        public async Task<JsonNetResult> SearchAddress(string searchTerm)
        {
            var results = await _aggieEnterpriseService.SearchShippingAddress(searchTerm);

            return new JsonNetResult(results.Select(a => new { a.Id, a.Name }));
        }

        public async Task<JsonNetResult> GetAddress(string searchTerm)
        {
            var workgroupAddress = new WorkgroupAddress();
            workgroupAddress.AeLocationCode = searchTerm;
            var results = await _aggieEnterpriseService.GetShippingAddress(workgroupAddress);

            return new JsonNetResult(new { results.Room, results.Building, results.City, results.State, results.Zip, results.Address });

        }

        [HttpPost]
        public ActionResult AddVendor(int workgroupId, WorkgroupVendor vendor)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetById(workgroupId);

            Check.Require(_securityService.HasWorkgroupAccess(workgroup));

            var modelState = new ModelStateDictionary();
            vendor.Workgroup = workgroup;
            vendor.TransferValidationMessagesTo(modelState);
            if (!modelState.IsValid)
            {
                return Json(new {success = false});
            }

            workgroup.AddVendor(vendor);

            _repositoryFactory.WorkgroupRepository.EnsurePersistent(workgroup);

            return Json(new { id = vendor.Id, success = true });
        }

        [HttpPost]
        public async Task<ActionResult> AddAddress(int workgroupId, WorkgroupAddress workgroupAddress)
        {
            //TODO: Consider using same logic as workgroup: _workgroupAddressService.CompareAddress
            var workgroup = _repositoryFactory.WorkgroupRepository.GetById(workgroupId);

            Check.Require(_securityService.HasWorkgroupAccess(workgroup));

            if (!string.IsNullOrWhiteSpace(workgroupAddress.AeLocationCode))
            {
                var location = await _aggieEnterpriseService.GetShippingAddress(workgroupAddress);
                if (location != null)
                {

                    workgroupAddress.Address = location.Address;
                    workgroupAddress.City = location.City;
                    workgroupAddress.State = location.State;
                    workgroupAddress.Zip = location.Zip;
                    workgroupAddress.Building = location.Building;
                    workgroupAddress.Room = location.Room;
                }
            }

            workgroup.AddAddress(workgroupAddress);

            _repositoryFactory.WorkgroupRepository.EnsurePersistent(workgroup);

            return Json(new { id = workgroupAddress.Id });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> UploadFile(int? orderId, string qqfile)
        {
            var request = ControllerContext.HttpContext.Request;

            using Stream fileStream = request.BodyReader.AsStream();

            var attachment = new Attachment
            {
                DateCreated = DateTime.UtcNow.ToPacificTime(),
                User = GetCurrentUser(),
                FileName = qqfile,
                ContentType = request.Headers["X-File-Type"],
                IsBlob = true, //Default to using blob storage
                Contents = null //We'll upload to blob storage instead of filling contents
            };
            
            // if (String.IsNullOrEmpty(qqfile.FileName)) // IE
            // {
            //     Check.Require(request.Files.Count > 0, Resources.FileUpload_NoFile);
            //     var file = request.Files[0];

            //     attachment.FileName = Path.GetFileNameWithoutExtension(file.FileName) +
            //         Path.GetExtension(file.FileName).ToLower();

            //     attachment.ContentType = file.ContentType;

            //     fileStream = file.InputStream; //IE uses request.Files[].InputStream instead of request.InputStream
            // }

            if (string.IsNullOrWhiteSpace(attachment.ContentType))
            {
                attachment.ContentType = "application/octet-stream";
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

                _eventService.OrderAddAttachment(attachment.Order);
            }

            _repositoryFactory.AttachmentRepository.EnsurePersistent(attachment);
            await _fileService.UploadAttachment(attachment.Id, fileStream);

            return new JsonNetResult(new { success = true, id = attachment.Id });
        }

        /// <summary>
        /// Allows a user to download any attachment file by providing the file ID
        /// </summary>
        public async Task<ActionResult> ViewFile(Guid fileId)
        {
            var file = await _fileService.GetAttachment(fileId);

            if (file == null) return NotFound(Resources.ViewFile_NotFound);

            var accessLevel = _securityService.GetAccessLevel(file.Order);

            if (!(OrderAccessLevel.Edit | OrderAccessLevel.Readonly).HasFlag(accessLevel))
            {
                return new UnauthorizedObjectResult(Resources.ViewFile_AccessDenied);
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
                return this.RedirectToAction(nameof(Review), new { id = id });
            }

            var viewModel = OrderReceiveModel.Create(order, _repositoryFactory.HistoryReceivedLineItemRepository, false);
            viewModel.ReviewOrderViewModel.Comments = _repositoryFactory.OrderCommentRepository.Queryable.Fetch(x => x.User).Where(x => x.Order.Id == id).OrderBy(o => o.DateCreated).ToList();
            viewModel.ReviewOrderViewModel.Attachments = _repositoryFactory.AttachmentRepository.Queryable.Where(x => x.Order.Id == id).ToList();
            viewModel.ReviewOrderViewModel.CurrentUser = CurrentUser.Identity.Name;
            viewModel.ReviewOrderViewModel.Complete = order.StatusCode.IsComplete;

            if (order.ApUser != null && order.ApUser.Id != CurrentUser.Identity.Name)
            {
                viewModel.Locked = true;
            }
            else
            {
                viewModel.Locked = false;
            }

                   
                foreach (var lineItem in viewModel.LineItems.Where(a => a.Quantity == 0 && a.QuantityReceived == null))
                {
                    lineItem.QuantityReceived = 0;
                    _repositoryFactory.LineItemRepository.EnsurePersistent(lineItem);
                }
            
            return View(viewModel);

        }

        [AuthorizeReadOrEditOrder]
        public ActionResult PayInvoice(int id)
        {
            var order = _repositoryFactory.OrderRepository.Queryable.Single(a => a.Id == id);

            if (!order.StatusCode.IsComplete || order.StatusCode.Id == OrderStatusCode.Codes.Cancelled || order.StatusCode.Id == OrderStatusCode.Codes.Denied)
            {
                Message = "Order must be complete before paying for line items.";
                return this.RedirectToAction(nameof(Review), new { id = id });
            }

            var viewModel = OrderReceiveModel.Create(order, _repositoryFactory.HistoryReceivedLineItemRepository, true);
            viewModel.ReviewOrderViewModel.Comments = _repositoryFactory.OrderCommentRepository.Queryable.Fetch(x => x.User).Where(x => x.Order.Id == id).OrderBy(o => o.DateCreated).ToList();
            viewModel.ReviewOrderViewModel.Attachments = _repositoryFactory.AttachmentRepository.Queryable.Where(x => x.Order.Id == id).ToList();
            viewModel.ReviewOrderViewModel.CurrentUser = CurrentUser.Identity.Name;
            viewModel.ReviewOrderViewModel.Complete = order.StatusCode.IsComplete;

            if (order.ApUser != null && order.ApUser.Id != CurrentUser.Identity.Name)
            {
                viewModel.Locked = true;
            }
            else
            {
                viewModel.Locked = false;
            }
            
            foreach (var lineItem in viewModel.LineItems.Where(a => a.Quantity == 0 && a.QuantityPaid == null))
            {
                lineItem.QuantityPaid = 0;
                _repositoryFactory.LineItemRepository.EnsurePersistent(lineItem);
            }
            
            return View(viewModel);

        }

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public ActionResult ReceiveAll(int id)
        {
            var order = _repositoryFactory.OrderRepository.Queryable.Single(a => a.Id == id);

            if (!order.StatusCode.IsComplete || order.StatusCode.Id == OrderStatusCode.Codes.Cancelled || order.StatusCode.Id == OrderStatusCode.Codes.Denied)
            {
                Message = string.Format("Order must be complete before {0} line items.", "receiving");
                return this.RedirectToAction(nameof(Review), new { id = id });
            }
            if (order.ApUser != null && order.ApUser.Id != CurrentUser.Identity.Name)
            {                
                Message = "Permission Denied to update";                
            }
            else
            {
                foreach (var lineItem in order.LineItems)
                {
                    var history = new HistoryReceivedLineItem();
                    history.User = _repositoryFactory.UserRepository.Queryable.Single(a => a.Id == CurrentUser.Identity.Name);
                    history.OldReceivedQuantity = lineItem.QuantityReceived;
                    history.NewReceivedQuantity = (lineItem.Quantity);
                    history.LineItem = lineItem;
                    history.PayInvoice = false;
                   
                    lineItem.QuantityReceived = lineItem.Quantity;
                   
                    _repositoryFactory.LineItemRepository.EnsurePersistent(lineItem);
                    if (history.NewReceivedQuantity != history.OldReceivedQuantity)
                    {
                       _repositoryFactory.HistoryReceivedLineItemRepository.EnsurePersistent(history);
                    }
                }
                    _eventService.OrderReceived(order, null, 0, "Received all line items");
                    Message = "All Line Items Received";
               
                _repositoryFactory.OrderRepository.EnsurePersistent(order);
            }
            
            return this.RedirectToAction(nameof(ReceiveItems), new { id = id });
        }

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public ActionResult PayAll(int id)
        {
            var order = _repositoryFactory.OrderRepository.Queryable.Single(a => a.Id == id);

            if (!order.StatusCode.IsComplete || order.StatusCode.Id == OrderStatusCode.Codes.Cancelled || order.StatusCode.Id == OrderStatusCode.Codes.Denied)
            {
                Message = string.Format("Order must be complete before {0} line items.", "paying for");
                return this.RedirectToAction(nameof(Review), new { id = id });
            }
            if (order.ApUser != null && order.ApUser.Id != CurrentUser.Identity.Name)
            {
                Message = "Permission Denied to update";
            }
            else
            {
                foreach (var lineItem in order.LineItems)
                {
                    var history = new HistoryReceivedLineItem();
                    history.User = _repositoryFactory.UserRepository.Queryable.Single(a => a.Id == CurrentUser.Identity.Name);
                    history.OldReceivedQuantity = lineItem.QuantityPaid ;
                    history.NewReceivedQuantity = (lineItem.Quantity);
                    history.LineItem = lineItem;
                    history.PayInvoice = true;
                   
                    lineItem.QuantityPaid = lineItem.Quantity;
                   
                    _repositoryFactory.LineItemRepository.EnsurePersistent(lineItem);
                    if (history.NewReceivedQuantity != history.OldReceivedQuantity)
                    {
                        _repositoryFactory.HistoryReceivedLineItemRepository.EnsurePersistent(history);
                    }
                }

                
                    _eventService.OrderPaid(order, null, 0, "Paid all line items");
                    Message = "All Line Items Paid";
                _repositoryFactory.OrderRepository.EnsurePersistent(order);
            }


            return this.RedirectToAction(nameof(PayInvoice), new { id = id });
        }

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public JsonNetResult ReceiveItems(int id, int lineItemId, decimal? receivedQuantity)
        {
            var success = true;
            var message = "Succeeded";
            var lastUpdatedBy = string.Empty;

            var order = _repositoryFactory.OrderRepository.Queryable.Single(a => a.Id == id);
            if (order.ApUser != null && order.ApUser.Id != CurrentUser.Identity.Name)
            {
                success = false;
                message = "Permission Denied to update";
                return new JsonNetResult(new { success, lineItemId, message, lastUpdatedBy });
            }

           
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
                    history.PayInvoice = false;
                                        
                    lineItem.QuantityReceived = receivedQuantity;
                    
                    _repositoryFactory.LineItemRepository.EnsurePersistent(lineItem);
                    if (history.NewReceivedQuantity != history.OldReceivedQuantity)
                    {
                        _repositoryFactory.HistoryReceivedLineItemRepository.EnsurePersistent(history);
                        lastUpdatedBy = history.User.FullName;
                    }
                    receivedQuantityReturned = string.Format("{0:0.###}", lineItem.QuantityReceived);
                    success = true;
                    message = "Updated";
                    var diff = (lineItem.Quantity - lineItem.QuantityReceived);
                    if (diff > 0)
                    {
                        unaccounted = string.Format("({0})", string.Format("{0:0.###}", diff));
                        showRed = true;
                    }
                    else
                    {
                        unaccounted = string.Format("{0}", string.Format("{0:0.###}", (diff*-1)));
                        showRed = false;
                    }

                    var updatedAmount = 0m;
                    if (history.OldReceivedQuantity.HasValue && history.NewReceivedQuantity.HasValue)
                    {
                        updatedAmount = history.NewReceivedQuantity.Value - history.OldReceivedQuantity.Value;
                    }
                    else if (history.OldReceivedQuantity.HasValue == false && history.NewReceivedQuantity.HasValue)
                    {
                        updatedAmount = history.NewReceivedQuantity.Value;
                    }
                                    
                    _eventService.OrderReceived(lineItem.Order, lineItem, updatedAmount);
                   

                    _repositoryFactory.OrderRepository.EnsurePersistent(lineItem.Order);
                }
                catch 
                {
                    success = false;
                    message = "There was a problem updating the quantity."; //ex.Message;
                }
                return new JsonNetResult(new { success, lineItemId, receivedQuantityReturned, message, showRed, unaccounted, lastUpdatedBy });
            
        }

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public JsonNetResult ReceiveItemsNotes(int id, int lineItemId, string note)
        {
            var success = true;
            var message = "Succeeded";
            var lastUpdatedBy = string.Empty;

            var order = _repositoryFactory.OrderRepository.Queryable.Single(a => a.Id == id);
            if (order.ApUser != null && order.ApUser.Id != CurrentUser.Identity.Name)
            {
                success = false;
                message = "Permission Denied to update";
                return new JsonNetResult(new { success, lineItemId, message, lastUpdatedBy });
            }
                
                var lineItem = _repositoryFactory.LineItemRepository.GetNullableById(lineItemId);
                if (lineItem == null)
                {
                    success = false;
                    message = "Line Item not found";
                    return new JsonNetResult(new { success, lineItemId, message, lastUpdatedBy });
                }

                if (lineItem.Order.Id != id)
                {
                    success = false;
                    message = "Order Id does not match";
                    return new JsonNetResult(new { success, lineItemId, message, lastUpdatedBy });
                }

                if (!lineItem.Order.StatusCode.IsComplete)
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
                        history.OldReceivedQuantity = lineItem.QuantityPaid; //These don't matter because it is the note being updated.
                        history.NewReceivedQuantity = lineItem.QuantityPaid;
                        history.PayInvoice = false;
                        if (lineItem.ReceivedNotes != saveNote)
                        {
                            _repositoryFactory.HistoryReceivedLineItemRepository.EnsurePersistent(history);
                            lastUpdatedBy = history.User.FullName;
                        }
                    
                    message = "Updated";
                    success = true;
                }
                catch
                {
                    success = false;
                    message = "There was a problem updating the notes."; //ex.Message;
                }

                return new JsonNetResult(new { success, lineItemId, message, lastUpdatedBy });
                
            
        }

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public JsonNetResult PayInvoice(int id, int lineItemId, decimal? receivedQuantity)
        {
            var success = true;
            var message = "Succeeded";
            var lastUpdatedBy = string.Empty;

            var order = _repositoryFactory.OrderRepository.Queryable.Single(a => a.Id == id);
            if (order.ApUser != null && order.ApUser.Id != CurrentUser.Identity.Name)
            {
                success = false;
                message = "Permission Denied to update";
                return new JsonNetResult(new { success, lineItemId, message, lastUpdatedBy });
            }

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
                    history.OldReceivedQuantity = lineItem.QuantityPaid;
                    history.NewReceivedQuantity = receivedQuantity;
                    history.CommentsUpdated = false;
                    history.LineItem = lineItem;
                    history.PayInvoice = true;

                    lineItem.QuantityPaid = receivedQuantity;

                    _repositoryFactory.LineItemRepository.EnsurePersistent(lineItem);
                    if (history.NewReceivedQuantity != history.OldReceivedQuantity)
                    {
                        _repositoryFactory.HistoryReceivedLineItemRepository.EnsurePersistent(history);
                        lastUpdatedBy = history.User.FullName;
                    }
                    receivedQuantityReturned = string.Format("{0:0.###}", lineItem.QuantityReceived);
                    success = true;
                    message = "Updated";
                    var diff = (lineItem.Quantity - lineItem.QuantityPaid);
                    if (diff > 0)
                    {
                        unaccounted = string.Format("({0})", string.Format("{0:0.###}", diff));
                        showRed = true;
                    }
                    else
                    {
                        unaccounted = string.Format("{0}", string.Format("{0:0.###}", (diff * -1)));
                        showRed = false;
                    }

                    var updatedAmount = 0m;
                    if (history.OldReceivedQuantity.HasValue && history.NewReceivedQuantity.HasValue)
                    {
                        updatedAmount = history.NewReceivedQuantity.Value - history.OldReceivedQuantity.Value;
                    }
                    else if (history.OldReceivedQuantity.HasValue == false && history.NewReceivedQuantity.HasValue)
                    {
                        updatedAmount = history.NewReceivedQuantity.Value;
                    }
                    _eventService.OrderPaid(lineItem.Order, lineItem, updatedAmount);

                    _repositoryFactory.OrderRepository.EnsurePersistent(lineItem.Order);
                }
                catch
                {
                    success = false;
                    message = "There was a problem updating the quantity."; //ex.Message;
                }
                return new JsonNetResult(new { success, lineItemId, receivedQuantityReturned, message, showRed, unaccounted, lastUpdatedBy });
            
        }

        [HttpPost]
        [AuthorizeReadOrEditOrder]
        public JsonNetResult PayInvoiceNote(int id, int lineItemId, string note)
        {
            var success = true;
            var message = "Succeeded";
            var lastUpdatedBy = string.Empty;

            var order = _repositoryFactory.OrderRepository.Queryable.Single(a => a.Id == id);
            if (order.ApUser != null && order.ApUser.Id != CurrentUser.Identity.Name)
            {
                success = false;
                message = "Permission Denied to update";
                return new JsonNetResult(new { success, lineItemId, message, lastUpdatedBy });
            }
            
                var lineItem = _repositoryFactory.LineItemRepository.GetNullableById(lineItemId);
                if (lineItem == null)
                {
                    success = false;
                    message = "Line Item not found";
                    return new JsonNetResult(new { success, lineItemId, message, lastUpdatedBy });
                }

                if (lineItem.Order.Id != id)
                {
                    success = false;
                    message = "Order Id does not match";
                    return new JsonNetResult(new { success, lineItemId, message, lastUpdatedBy });
                }

                if (!lineItem.Order.StatusCode.IsComplete)
                {
                    success = false;
                    message = "Order is not complete";
                    return new JsonNetResult(new { success, lineItemId, message, lastUpdatedBy });
                }


                try
                {
                    
                        var saveNote = lineItem.PaidNotes;
                        lineItem.PaidNotes = note;
                        _repositoryFactory.LineItemRepository.EnsurePersistent(lineItem);
                        var history = new HistoryReceivedLineItem();
                        history.LineItem = lineItem;
                        history.User = _repositoryFactory.UserRepository.Queryable.Single(a => a.Id == CurrentUser.Identity.Name);
                        history.CommentsUpdated = true;
                        history.OldReceivedQuantity = lineItem.QuantityPaid; //These don't matter because it is the note being updated.
                        history.NewReceivedQuantity = lineItem.QuantityPaid;
                        history.PayInvoice = true;
                        if (lineItem.PaidNotes != saveNote)
                        {
                            _repositoryFactory.HistoryReceivedLineItemRepository.EnsurePersistent(history);
                            lastUpdatedBy = history.User.FullName;
                        }
                    
                   
                    message = "Updated";
                    success = true;
                }
                catch
                {
                    success = false;
                    message = "There was a problem updating the notes."; //ex.Message;
                }

                return new JsonNetResult(new { success, lineItemId, message, lastUpdatedBy });
                
            
            
        }

        [HttpPost]
        public JsonNetResult UpdateAttachmentCategory(int id, Guid guidId, string category)
        {
            var success = true;
            var message = "Updated";
            var attachement = _repositoryFactory.AttachmentRepository.GetNullableById(guidId);
            if (attachement == null)
            {
                success = false;
                message = "Error, not found";
                return new JsonNetResult(new{success, message});
            }

            if (id == 0)
            {
                //Order has not been created yet. Just check that the attachment's user is the same as the current user
                if (attachement.User.Id.ToLower() != CurrentUser.Identity.Name)
                {
                    success = false;
                    message = "Error. Attachment category not updated. (No Permission)";
                    return new JsonNetResult(new { success, message });
                }
            }
            else
            {
                if (attachement.Order != null && id != attachement.Order.Id) // it is possible that the id has a value, but the attachment order is null if an attachment is added from the edit page
                {
                    success = false;
                    message = "Error, Id miss-match";
                    return new JsonNetResult(new { success, message });
                }
                var accessLevel = _securityService.GetAccessLevel(id);
                const OrderAccessLevel allowed = OrderAccessLevel.Readonly | OrderAccessLevel.Edit;

                if (!allowed.HasFlag(accessLevel))
                {
                    success = false;
                    message = "Not Updated, No Access";
                    return new JsonNetResult(new { success, message });
                }
            }

            category = category.Trim();
            if (category.Length > 50)
            {
                category = category.Substring(0, 50);
                message = "Updated. But only first 50 characters used.";
            }
            if (string.IsNullOrWhiteSpace(category))
            {
                message = "Cleared";
            }

            attachement.Category = category;
            try
            {
                _repositoryFactory.AttachmentRepository.EnsurePersistent(attachement);
            }
            catch (Exception)
            {
                success = false;
                message = "Error. Update failed.";
                return new JsonNetResult(new { success, message });
            }

            return new JsonNetResult(new { success, message });
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
            dynamic peeps = null;
            try
            {
                var order = _repositoryFactory.OrderRepository.Queryable.Single(a=> a.Id==id);
               
                //Get all normal and full featured users at this level
                peeps =
                    order.Workgroup.Permissions.Where(
                        a => a.Role.Id == orderStatusCodeId && (!a.IsAdmin || (a.IsAdmin && a.IsFullFeatured)))
                        .Select(b => b.User)
                        .Distinct()
                        .Select(u => new {u.FullName, u.Email})
                        .ToList();

            }
            catch (Exception)
            {
                success = false;
                return new JsonNetResult(new {success, peeps});
            }
           return new JsonNetResult(new {success, peeps});
        }

        public JsonNetResult GetReceiveHistory(int id, int lineItemId, bool payInvoice)
        {
            var success = true;
            var history = new List<HistoryReceivedLineItem>();
            try
            {
                history = _repositoryFactory.HistoryReceivedLineItemRepository.Queryable.Where(a => a.LineItem.Id == lineItemId && a.PayInvoice == payInvoice).OrderBy(a => a.UpdateDate).ToList();
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
            var workgroupAccounts = _repositoryFactory.WorkgroupAccountRepository.Queryable.Where(a => a.Workgroup.Id == workgroup.Id).ToArray();

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
            order.DeliverToPhone = model.ShipPhone;
            order.OrderType = order.OrderType ?? _repositoryFactory.OrderTypeRepository.GetById(OrderType.Types.OrderRequest);
            order.CreatedBy = order.CreatedBy ?? _repositoryFactory.UserRepository.GetById(CurrentUser.Identity.Name); //Only replace created by if it doesn't already exist
            order.Justification = model.Justification;
            order.BusinessPurpose = model.BusinessPurpose;
            order.RequestType = model.RequestType;
            
            if (!string.IsNullOrWhiteSpace(model.Comments))
            {
                var comment = new OrderComment
                {
                    DateCreated = DateTime.UtcNow.ToPacificTime(),
                    User = _repositoryFactory.UserRepository.GetById(CurrentUser.Identity.Name),
                    Text = model.Comments
                };

                order.AddOrderComment(comment);
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
                    Use = model.Restricted.Use, 
                    PharmaceuticalGrade = model.Restricted.PharmaceuticalGrade
                };

                order.SetAuthorizationInfo(restricted);
            }
            else
            {
                order.ClearAuthorizationInfo();
            }

            if (includeLineItemsAndSplits)
            {
                _bugTrackingService.CheckForClearedOutSubAccounts(order, model.Splits, model);

                decimal number;
                order.EstimatedTax = decimal.TryParse(model.Tax != null ? model.Tax.TrimEnd('%') : null, out number) ? number : order.EstimatedTax;
                order.ShippingAmount = decimal.TryParse(model.Shipping != null ? model.Shipping.TrimStart('$') : null, out number) ? number : order.ShippingAmount;
                order.FreightAmount = decimal.TryParse(model.Freight != null ? model.Freight.TrimStart('$') : null, out number) ? number : order.FreightAmount;

                order.LineItems.Clear(); //replace line items and splits
                order.Splits.Clear();

                //Add in line items
                foreach (var lineItem in model.Items)
                {
                    if (lineItem.IsValid())
                    {
                        Commodity commodity = null;
                        if (!string.IsNullOrWhiteSpace(lineItem.CommodityCode))
                        {
                            commodity = _repositoryFactory.CommodityRepository.Queryable.SingleOrDefault(a => a.Id == lineItem.CommodityCode && a.IsActive);
                        }

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
                                    order.AddSplit(BuildSplit(workgroupAccounts, split.Account, decimal.Parse(split.Amount), orderLineItem));
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
                            order.AddSplit(BuildSplit(workgroupAccounts, split.Account, decimal.Parse(split.Amount), null));
                        }
                    }
                }
                else if (model.SplitType == OrderViewModel.SplitTypes.None)
                {
                    //This one is using the model, not a split
                    order.AddSplit(BuildSplit(workgroupAccounts, model.Account, order.Total(), null));
                }

                order.TotalFromDb = order.Total();
                order.LineItemSummary = order.GenerateLineItemSummary();
            }
        }

        /// <summary>
        /// Ok, so this tries to find either the Financial Segment String *OR* the account in the workgroup accounts
        /// If it finds it. regardless if it is by CoA or Account it tries to use the CoA if that is set.
        /// This means it should work if a KFS account is used, and that has been updated to a CoA in the workgroup Account.
        /// </summary>
        /// <param name="workgroupAccounts">Array of workgroup account for the order</param>
        /// <param name="split">The view model split</param>
        /// <param name="orderLineItem"></param>
        /// <returns>Split to add to the order</returns>
        private Split BuildSplit(WorkgroupAccount[] workgroupAccounts, string account, decimal amount, LineItem orderLineItem)
        {
            //Ok, this should be if no accounts are entered. Just approvers
            if(account == null && orderLineItem == null)
            {
                return new Split
                {
                    Account = null,
                    FinancialSegmentString = null,
                    Amount = amount,
                    LineItem = null,
                    Name = null
                };
            }

            var foundWorkgroupAccount = workgroupAccounts.FirstOrDefault(a => (a.FinancialSegmentString != null && a.FinancialSegmentString == account) || (a.Account != null && a.Account.Id == account));

            if(foundWorkgroupAccount != null)
            {
                //Ok, we found a wga, check if it is a CoA
                if (!string.IsNullOrWhiteSpace(foundWorkgroupAccount.FinancialSegmentString))
                {
                    return new Split
                    {
                        Account = null,
                        FinancialSegmentString = foundWorkgroupAccount.FinancialSegmentString,
                        Amount = amount,
                        LineItem = orderLineItem,
                        Name = foundWorkgroupAccount?.Name
                    };
                }
                //We found one, but it is an old account
                return new Split
                {
                    Account = foundWorkgroupAccount.Account?.Id,
                    FinancialSegmentString = null,
                    Amount = amount,
                    LineItem = orderLineItem,
                    Name = foundWorkgroupAccount.GetName
                };
            }

            //We didn't find the wga, so check if split.account is a CoA or KFS account
            if(FinancialChartValidation.GetFinancialChartStringType(account) == FinancialChartStringType.Invalid)
            {
                //KFS
                return new Split
                {
                    Account =  account,
                    FinancialSegmentString = null,
                    Amount = amount,
                    LineItem = orderLineItem,
                    Name = "Externally Set"
                };
            }

            //CoA
            return new Split
            {
                Account = null,
                FinancialSegmentString = account,
                Amount = amount,
                LineItem = orderLineItem,
                Name = "Externally Set"
            };
        }

        private OrderModifyModel CreateOrderModifyModel(Workgroup workgroup)
        {
            var model = new OrderModifyModel
            {
                Order = new Order(),
                Workgroup = workgroup,
                Units = _repositoryFactory.UnitOfMeasureRepository.Queryable.Cache().ToList(),
                Accounts = _repositoryFactory.WorkgroupAccountRepository.Queryable.Where(x => x.Workgroup.Id == workgroup.Id).Select(x => x.Account).ToList(),
                WorkgroupAccounts = _repositoryFactory.WorkgroupAccountRepository.Queryable.Where(x => x.Workgroup.Id == workgroup.Id).ToList(), //Should end up replacing the Accounts above...
                Vendors = _repositoryFactory.WorkgroupVendorRepository.Queryable.Where(x => x.Workgroup.Id == workgroup.Id && x.IsActive).OrderBy(a => a.Name).ToList(),
                Addresses = _repositoryFactory.WorkgroupAddressRepository.Queryable.Where(x => x.Workgroup.Id == workgroup.Id && x.IsActive).ToList(),
                ShippingTypes = _repositoryFactory.ShippingTypeRepository.Queryable.Cache().ToList(),
                Approvers = _repositoryFactory.WorkgroupPermissionRepository.Queryable.Where(x => x.Workgroup.Id == workgroup.Id && x.Role.Id == Role.Codes.Approver && !x.IsAdmin).Select(x => x.User).OrderBy(a => a.LastName).ThenBy(a => a.FirstName).ToList(),
                AccountManagers = _repositoryFactory.WorkgroupPermissionRepository.Queryable.Where(x => x.Workgroup.Id == workgroup.Id && x.Role.Id == Role.Codes.AccountManager && !x.IsAdmin).Select(x => x.User).OrderBy(a => a.LastName).ThenBy(a => a.FirstName).ToList(),
                ConditionalApprovals = workgroup.AllConditionalApprovals,
                CustomFields = _repositoryFactory.CustomFieldRepository.Queryable.Where(x => x.Organization.Id == workgroup.PrimaryOrganization.Id && x.IsActive).ToList() //call to list to exec the future batch
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

            try
            {
                // make the call
                var i = 0;
                if (!int.TryParse(order.ReferenceNumber.Trim(), out i))
                {
                    return new JsonNetResult(null);
                }
                var result = _financialSystemService.GetOrderStatus(order.ReferenceNumber.Trim());
                return new JsonNetResult(result);
            }
            catch (Exception)
            {
                return new JsonNetResult(null);
            }
            
        }

        public async Task<JsonNetResult> GetAeStatus(int id)
        {
            // load the order
            var order = _repositoryFactory.OrderRepository.GetNullableById(id);
            try
            {
                if (string.IsNullOrWhiteSpace(order.ReferenceNumber))
                {
                    return new JsonNetResult(null);
                }
                
                var rtValue = await _aggieEnterpriseService.LookupOrderStatus(order.ReferenceNumber.Trim()); 

                return new JsonNetResult(rtValue);

            }
            catch (Exception)
            {
                return new JsonNetResult(null);
            }
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
            requestSave.LastUpdate = DateTime.UtcNow.ToPacificTime();

            if (!_memoryCache.TryGetValue<string>("Version", out var version))
            {
                version = "N/A";
            }
            requestSave.Version = version;

            _repositoryFactory.OrderRequestSaveRepository.EnsurePersistent(requestSave, newSave);

            Message = "Order Saved Successfully";

            return new JsonNetResult(new {success = true, redirect = Url.Action("Landing", "Home")});
        }

        public ActionResult SavedOrderRequests()
        {
            var saves = _repositoryFactory.OrderRequestSaveRepository.Queryable.Where(a => a.User.Id == CurrentUser.Identity.Name);

            return View(saves);
        }

        public ActionResult DeleteSavedOrder(Guid id)
        {
            var savedOrder = _repositoryFactory.OrderRequestSaveRepository.Queryable.Single(a => a.Id == id);

            if (savedOrder.User.Id != CurrentUser.Identity.Name)
            {
                ErrorMessage = "Not your order";
                return this.RedirectToAction(nameof(ErrorController.NotAuthorized), typeof(ErrorController).ControllerName());
            }

            return View(savedOrder);
        }

        [HttpPost]
        public ActionResult DeleteSavedOrder(Guid id, OrderRequestSave orderRequestSave)
        {
            var savedOrder = _repositoryFactory.OrderRequestSaveRepository.Queryable.Single(a => a.Id == id);

            if (savedOrder.User.Id != CurrentUser.Identity.Name)
            {
                ErrorMessage = "Not your order";
                return this.RedirectToAction(nameof(ErrorController.NotAuthorized), typeof(ErrorController).ControllerName());
            }

            _repositoryFactory.OrderRequestSaveRepository.Remove(savedOrder);

            Message = "Saved Order Deleted";

            return this.RedirectToAction(nameof(SavedOrderRequests));
        }

        public ActionResult GetRequesters(int id)
        {
            var requesterInWorkgroup = _repositoryFactory.WorkgroupPermissionRepository
                .Queryable.Where(x => x.Workgroup.Id == id && x.User.Id == CurrentUser.Identity.Name)
                .Where(x => x.Role.Id == Role.Codes.Requester);

            if (!requesterInWorkgroup.Any())
            {
                return new UnauthorizedObjectResult("not your order");
            }


            var requestersForWorkgroup = (from wp in _repositoryFactory.WorkgroupPermissionRepository.Queryable
                                          where wp.Workgroup.Id == id
                                                && wp.User.IsActive
                                                && wp.Role.Id == Role.Codes.Requester
                                          select new { wp.User }).ToList();

            var requesterInfo = requestersForWorkgroup.Select(x => new { Name = x.User.FullNameAndId, x.User.Id });

            return new JsonNetResult(requesterInfo);
        }
    }
}
