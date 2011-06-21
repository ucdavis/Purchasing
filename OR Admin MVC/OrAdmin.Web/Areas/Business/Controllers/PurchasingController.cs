using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using OrAdmin.Core.Attributes.Authorization;
using OrAdmin.Core.Enums.App;
using OrAdmin.Core.Enums.Purchasing;
using OrAdmin.Core.Extensions;
using OrAdmin.Core.Filters;
using OrAdmin.Core.Helpers;
using OrAdmin.Core.Settings;
using OrAdmin.Entities.Purchasing;
using OrAdmin.Repositories.Purchasing;
using OrAdmin.Web.Areas.Business.Models.Purchasing;
using SystemIO = System.IO;
using OrAdmin.Entities.App;

namespace OrAdmin.Web.Areas.Business.Controllers
{
    [BusinessUser]
    public class PurchasingController : BaseController
    {
        //
        // GET: /Business/Purchasing/
        public ActionResult Index()
        {
            RequestRepository requestRepo = new RequestRepository();
            HistoryItemRepository historyRepo = new HistoryItemRepository();
            CommentRepository commentRepo = new CommentRepository();

            IndexViewModel model = new IndexViewModel()
            {
                Requests = requestRepo.GetRequestsByUser(User.Identity.Name).Take(5),
                HistoryItems = historyRepo.GetHistoryByUser(User.Identity.Name).Take(5),
                RequestComments = commentRepo.GetCommentsByUser(User.Identity.Name).Take(5)
            };

            return View(model);
        }

        //
        // GET: /Business/Purchasing/Request
        [ActionName("Request")]
        public ActionResult PurchaseRequest()
        {
            return View();
        }

        #region Forms

        //
        // GET: /Business/Purchasing/Dpo
        public ActionResult Dpo(Guid? requestUniqueId, Types.FormMode? mode)
        {
            PiApprovalRepository approverRepo = new PiApprovalRepository();
            VendorRepository vendorRepo = new VendorRepository();
            ShipToAddressRepository addressRepo = new ShipToAddressRepository();
            RequestRepository requestRepo = new RequestRepository();
            ShippingMethodRepository shippingRepo = new ShippingMethodRepository();
            Request lastRequest = requestRepo.GetRequestsByUser(User.Identity.Name).FirstOrDefault();

            // Get the current request or t-up a new one
            Request currentRequest = requestUniqueId != null ?
                requestRepo.GetRequest((Guid)requestUniqueId) :
                new Request();

            /* Items
            --------------------------------------------------------- */

            // Add the items form the current request (if any)
            List<DpoItem> dpoItems = new List<DpoItem>();
            dpoItems.AddRange(currentRequest.DpoItems.ToList());

            // Add the remaining rows
            while (dpoItems.Count < GlobalSettings.PurchasingMaxItemRows)
                dpoItems.Add(new DpoItem() { Unit = "each" });

            /* Approvals & approvers
            --------------------------------------------------------- */

            // Get the approvers for this request (if any)
            List<PiApproval> piApprovals = new List<PiApproval>();
            piApprovals.AddRange(currentRequest.PiApprovals.ToList());

            // Add remaining rows
            while (piApprovals.Count < GlobalSettings.PurchasingMaxApprovers)
                piApprovals.Add(new PiApproval() { });

            // TODO, check for user's "hide from list" status in profile
            // TODO handle users without profile?
            var approvers = Roles.GetUsersInRole(RoleName.PurchaseApprover.ToString()).Where(userName => User.Profile(userName).DefaultUnitId == User.Profile().DefaultUnitId)
                .Select(userName => new { Name = User.Profile(userName).LastName + ", " + User.Profile(userName).FirstName, UserName = userName });
            List<SelectListItem> approversList = new SelectList(approvers.OrderBy(u => u.Name), "UserName", "Name").ToList();
            approversList.Insert(0, new SelectListItem { Text = "Select one...", Value = null }); // Insert "select one..."

            /* Purchaser
            --------------------------------------------------------- */

            // TODO, check for user's "hide from list" status in profile
            // TODO handle users without profile?
            var purchasers = Roles.GetUsersInRole(RoleName.PurchaseAdmin.ToString()).Where(userName => User.Profile(userName).DefaultUnitId == User.Profile().DefaultUnitId);
            var purchaserList = purchasers.Select(userName => new { Name = User.Profile(userName).LastName + ", " + User.Profile(userName).FirstName, UserName = userName });

            /* Init model
            --------------------------------------------------------- */

            DpoViewModel model = new DpoViewModel()
            {
                FormMode = mode,
                Request = currentRequest,
                LastRequest = lastRequest,
                DpoItems = dpoItems,
                PIApprovals = piApprovals,
                Attachments = currentRequest.RequestComments.Any() ? currentRequest.RequestComments.OrderBy(c => c.SubmittedOn).First().Attachments.ToList() : null,
                RequestedAccountList = new SelectList(requestRepo.GetRequestsByUser(User.Identity.Name).Select(r => r.RequestedAccount).Distinct(), !String.IsNullOrEmpty(currentRequest.RequestedAccount) ? currentRequest.RequestedAccount : (lastRequest != null) ? lastRequest.RequestedAccount : null),
                VendorList = new SelectList(vendorRepo.GetVendors(User.Profile().DefaultUnitId, (int)RequestRepository.RequestType.Dpo), "Id", "FriendlyName", currentRequest.VendorId > 0 ? (object)currentRequest.VendorId : null),
                ShipToAddressList = new SelectList(addressRepo.GetAddresses(User.Profile().DefaultUnitId), "Id", "FriendlyName", lastRequest != null ? lastRequest.ShipToId : (int?)null),
                ApproverList = new SelectList(approversList, "Value", "Text"),
                PurchaserList = new SelectList(purchaserList.OrderBy(u => u.Name), "UserName", "Name", !String.IsNullOrEmpty(currentRequest.PurchaserId) ? currentRequest.PurchaserId : (lastRequest != null ? lastRequest.PurchaserId : null)),
                ShippingMethodList = new SelectList(
                    shippingRepo.GetShippingMethods(),
                    "Id",
                    "ShippingMethodName",
                    currentRequest.ShippingMethodId > 0 ?
                        currentRequest.ShippingMethodId :
                        (lastRequest != null ? (object)lastRequest.ShippingMethodId : null))
            };

            return View(model);
        }

        //
        // POST: /Business/Purchasing/Dpo
        [HttpPost]
        [ValidateInput(false)]
        [ValidateOnlyIncomingValues]
        public ActionResult Dpo(
            string submitAction,
            Types.FormMode? mode,
            Request request,
            [Bind(Prefix = "DpoItem")]IList<DpoItem> items,
            [Bind(Prefix = "PiApproval")]IList<PiApproval> approvals,
            [Bind(Prefix = "Attachment")]IList<Attachment> attachments
            )
        {
            /* Custom business rules
            --------------------------------------------------------- */
            // Ensure at least one item was entered
            if (items.Count < 1)
                ModelState.AddModelError("noItems", "You must enter at least one item");

            // Ensure at least one approver was selected
            approvals = approvals.Where(a => a.PiId != "Select one...").ToList();
            if (approvals.Count == 0)
                ModelState.AddModelError("noApprovers", "You must select at least one approver");

            // Remove attachments without a filename
            if (attachments != null)
                attachments = attachments.Where(a => !String.IsNullOrEmpty(a.FileName)).ToList();

            if (ModelState.IsValid && (!mode.HasValue || mode.Value == Types.FormMode.copy))
            {
                /* INSERT
                --------------------------------------------------------- */

                RequestRepository requestRepo = new RequestRepository();

                try
                {
                    // Init some predefine fields
                    request.SubmittedOn = DateTime.Now;
                    request.LastModified = DateTime.Now;
                    request.RequesterId = User.Identity.Name;
                    request.LastModifiedBy = User.Identity.Name;
                    request.RequestType = Convert.ToInt32(RequestRepository.RequestType.Dpo);
                    request.OverLimit = items.Sum(i => i.Quantity * i.PricePerUnit) * (GlobalSettings.PurchasingTax / 100) > 5000 ? true : false;
                    request.UnitId = User.Profile().DefaultUnitId;
                    request.Archived = false;

                    // Insert request
                    requestRepo.InsertRequest(request);

                    // Associate items
                    request.DpoItems.AddRange(from i in items
                                              select new DpoItem()
                                              {
                                                  RequestId = request.Id,
                                                  CatalogNumber = i.CatalogNumber,
                                                  CommodityTypeId = i.CommodityTypeId,
                                                  Description = i.Description,
                                                  Notes = i.Notes,
                                                  PricePerUnit = i.PricePerUnit,
                                                  PromoCode = i.PromoCode,
                                                  Quantity = i.Quantity,
                                                  SubmittedOn = DateTime.Now,
                                                  SubmittedBy = User.Identity.Name,
                                                  Url = i.Url,
                                                  Unit = i.Unit
                                              });

                    // Associate attachments
                    if (attachments != null)
                    {
                        // Create new comment
                        RequestComment comment = new RequestComment()
                        {
                            RequestId = request.Id,
                            SubmittedOn = DateTime.Now,
                            SubmittedBy = User.Identity.Name,
                            Comments = !String.IsNullOrEmpty(request.Comments) ? request.Comments.Trim() : String.Empty
                        };

                        comment.Attachments.AddRange(
                            from a in attachments
                            select new Attachment()
                            {
                                FileName = HttpContext.Server.UrlDecode(a.FileName),
                                FileSizeBytes = a.FileSizeBytes,
                                CommentId = comment.Id,
                                SubmittedBy = a.SubmittedBy,
                                SubmittedOn = a.SubmittedOn
                            });

                        request.RequestComments.Add(comment);
                    }

                    // Associate approvals
                    request.PiApprovals.AddRange(
                        from a in approvals
                        select new PiApproval()
                        {
                            Approval = false,
                            RequestId = request.Id,
                            SubmittedOn = DateTime.Now,
                            PiId = a.PiId
                        });


                    // Finally, save the request
                    requestRepo.Save();

                    // Save or submit for approval?
                    if (submitAction == "Save and submit later")
                    {
                        // Insert saved status
                        request.MilestoneMaps.Add(new MilestoneMap()
                        {
                            RequestId = request.Id,
                            Comments = null,
                            SubmittedBy = User.Identity.Name,
                            SubmittedOn = DateTime.Now,
                            MilestoneValue = Convert.ToInt32(MilestoneMapRepository.MilestoneType.Saved)
                        });
                    }
                    else
                    {
                        // Submit for approval
                        request.MilestoneMaps.Add(new MilestoneMap()
                        {
                            RequestId = request.Id,
                            Comments = request.Comments,
                            SubmittedBy = User.Identity.Name,
                            SubmittedOn = DateTime.Now,
                            MilestoneValue = Convert.ToInt32(MilestoneMapRepository.MilestoneType.Requested)
                        });
                    }

                    // Update unique ID
                    request.FriendlyUniqueId = String.Format("{0}-{1}", User.Profile().Unit.UnitAbbreviation, request.Id);
                    requestRepo.Save();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("err", "Failed to insert request: " + ex.Message);
                    return DpoResult(mode, request, items, approvals, attachments);
                }

                return new RedirectResult("http://google.com");
            }
            else if (ModelState.IsValid && mode.HasValue && mode.Value == Types.FormMode.edit)
            {
                /* UPDATE
                --------------------------------------------------------- */

                return new RedirectResult("http://google.com");
            }
            else
            {
                // Validation failed, so return the model
                return DpoResult(mode, request, items, approvals, attachments);
            }
        }

        // DPO Result
        private ViewResult DpoResult(
            Types.FormMode? mode,
            Request request,
            IList<DpoItem> items,
            IList<PiApproval> approvals,
            IList<Attachment> attachments)
        {
            RequestRepository requestRepo = new RequestRepository();
            VendorRepository vendorRepo = new VendorRepository();
            ShipToAddressRepository addressRepo = new ShipToAddressRepository();
            ShippingMethodRepository shippingRepo = new ShippingMethodRepository();
            Request lastRequest = requestRepo.GetRequestsByUser(User.Identity.Name).FirstOrDefault();

            /* ---- Items ---- */

            // Add the items form the current request (if any)
            List<DpoItem> dpoItems = new List<DpoItem>();
            dpoItems.AddRange(items);

            // Add the remaining rows
            while (dpoItems.Count < GlobalSettings.PurchasingMaxItemRows)
                dpoItems.Add(new DpoItem() { Unit = "each" });

            /* ---- Approvals & approvers ---- */

            // Get the approvers for this request (if any)
            List<PiApproval> piApprovals = new List<PiApproval>();
            piApprovals.AddRange(approvals);

            // Add remaining rows
            while (piApprovals.Count < GlobalSettings.PurchasingMaxApprovers)
                piApprovals.Add(new PiApproval() { });

            // TODO, check for user's "hide from list" status in profile
            // TODO handle users without profile?
            var approvers = Roles.GetUsersInRole(RoleName.PurchaseApprover.ToString()).Where(userName => User.Profile(userName).DefaultUnitId == User.Profile().DefaultUnitId)
                .Select(userName => new { Name = User.Profile(userName).LastName + ", " + User.Profile(userName).FirstName, UserName = userName });
            List<SelectListItem> approversList = new SelectList(approvers.OrderBy(u => u.Name), "UserName", "Name").ToList();
            approversList.Insert(0, new SelectListItem { Text = "Select one...", Value = null }); // Insert "select one..."

            /* ---- Purchaser ---- */

            // TODO, check for user's "hide from list" status in profile
            // TODO handle users without profile?
            var purchasers = Roles.GetUsersInRole(RoleName.PurchaseAdmin.ToString()).Where(userName => User.Profile(userName).DefaultUnitId == User.Profile().DefaultUnitId);
            var purchaserList = purchasers.Select(userName => new { Name = User.Profile(userName).LastName + ", " + User.Profile(userName).FirstName, UserName = userName });

            /* ---- Init model ---- */

            DpoViewModel model = new DpoViewModel()
            {
                FormMode = mode,
                Request = request,
                LastRequest = lastRequest,
                DpoItems = dpoItems,
                PIApprovals = piApprovals,
                Attachments = attachments,
                RequestedAccountList = new SelectList(requestRepo.GetRequestsByUser(User.Identity.Name).Select(r => r.RequestedAccount), request.RequestedAccount),
                VendorList = new SelectList(vendorRepo.GetVendors(User.Profile().DefaultUnitId, (int)RequestRepository.RequestType.Dpo), "Id", "FriendlyName", request.VendorId),
                ShipToAddressList = new SelectList(addressRepo.GetAddresses(User.Profile().DefaultUnitId), "Id", "FriendlyName", lastRequest != null ? lastRequest.ShipToId : (int?)null),
                ApproverList = new SelectList(approversList, "Value", "Text"),
                PurchaserList = new SelectList(purchaserList.OrderBy(u => u.Name), "UserName", "Name", request.PurchaserId),
                ShippingMethodList = new SelectList(
                    shippingRepo.GetShippingMethods(),
                    "Id",
                    "ShippingMethodName", request.ShippingMethodId
                    )
            };

            return View(model);
        }

        //
        // GET: /Business/Purchasing/Dro
        public ActionResult Dro()
        {
            return View();
        }

        //
        // GET: /Business/Purchasing/Ba
        public ActionResult Ba()
        {
            return View();
        }

        #endregion

        #region Partial

        [HttpPost]
        public JsonResult _Upload()
        {
            string writePath = null;
            DateTime submittedOn = DateTime.Now;
            string submittedBy = User.Identity.Name;
            string fileName = null;
            long streamLengthBytes = 0;

            try
            {
                int length = GlobalSettings.PurchasingMaxUploadMB > 0 ? Convert.ToInt32(GlobalSettings.PurchasingMaxUploadMB * 1000) : 4096;
                int bytesRead = 0;
                Byte[] buffer = new Byte[length];

                // Get the name from qqfile url parameter here
                if (String.IsNullOrEmpty(Request["qqfile"]))
                {
                    // IE
                    fileName =
                        SystemIO.Path.GetFileNameWithoutExtension(Request.Files[0].FileName).Slug() +
                        SystemIO.Path.GetExtension(Request.Files[0].FileName).ToLower();
                }
                else
                {
                    // Webkit, Mozilla
                    fileName =
                        SystemIO.Path.GetFileNameWithoutExtension(Request["qqfile"]).Slug() +
                        SystemIO.Path.GetExtension(Request["qqfile"]).ToLower();
                }

                // Get file path with unique name
                writePath = FileHelper.GetRelativeFilePath(
                                    Applications.ApplicationName.Purchasing,
                                    fileName,
                                    User.Identity.Name,
                                    DateTime.Now,
                                    true
                                    );

                try
                {
                    using (SystemIO.FileStream fileStream = new SystemIO.FileStream(Server.MapPath(writePath), SystemIO.FileMode.Create))
                    {
                        do
                        {
                            bytesRead = Request.InputStream.Read(buffer, 0, length);
                            fileStream.Write(buffer, 0, bytesRead);
                            streamLengthBytes = fileStream.Position;
                        }
                        while (bytesRead > 0);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // TODO: Log error hinting to set the write permission of ASPNET or the identity accessing the code
                    return new JsonResult();
                }
            }
            catch (Exception)
            {
                // TODO: Log error
                return new JsonResult();
            }

            return Json(
                new
                {
                    success = true,
                    SubmittedOn = submittedOn.ToString(),
                    SubmittedBy = submittedBy,
                    FileName = SystemIO.Path.GetFileName(writePath), // Get file name from path
                    FileSizeBytes = streamLengthBytes
                },
                "application/json"
            );
        }

        public ActionResult _RequestDetails(Guid requestUniqueId)
        {
            RequestRepository requestRepo = new RequestRepository();
            HistoryItemRepository historyRepo = new HistoryItemRepository();
            Request request = requestRepo.GetRequest(requestUniqueId);

            return View(new _RequestDetailsViewModel()
            {
                Request = request,
                HistoryItems = historyRepo.GetHistoryByRequest(request.Id)
            });
        }

        [HttpPost]
        public ActionResult _RequestDetails(Guid requestUniqueId, FormCollection collection)
        {
            RequestRepository requestRepo = new RequestRepository();
            Request request = requestRepo.GetRequest(requestUniqueId);

            if (request.RequesterId.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase))
            {
                request.MilestoneMaps.Add(new MilestoneMap()
                {
                    MilestoneValue = (int)MilestoneMapRepository.MilestoneType.Requested,
                    Comments = null,
                    RequestId = request.Id,
                    SubmittedBy = User.Identity.Name,
                    SubmittedOn = DateTime.Now
                });
                requestRepo.Save();

                TempData[GlobalProperty.Message.SuccessMessage.ToString()] = "Request successfully submitted";
            }
            else
                TempData[GlobalProperty.Message.FailMessage.ToString()] = "You are not the requester";

            return RedirectToAction("MyRequestDetails", new { requestUniqueId = requestUniqueId });
        }

        public ActionResult _AddComment(int requestId, Guid requestUniqueId)
        {
            return View(new _AddCommentViewModel()
            {
                RequestId = requestId,
                RequestUniqueId = requestUniqueId
            });
        }

        [HttpPost]
        public ActionResult _AddComment(_AddCommentViewModel model, [Bind(Prefix = "Attachment")]IList<Attachment> attachments)
        {
            if (ModelState.IsValid)
            {
                CommentRepository repo = new CommentRepository();

                RequestComment comment = new RequestComment()
                {
                    Comments = model.Comments,
                    RequestId = model.RequestId,
                    SubmittedBy = User.Identity.Name,
                    SubmittedOn = DateTime.Now
                };

                if (attachments != null && attachments.Any())
                {
                    comment.Attachments.AddRange(
                        from a in attachments
                        select new Attachment()
                        {
                            FileName = HttpContext.Server.UrlDecode(a.FileName),
                            FileSizeBytes = a.FileSizeBytes,
                            CommentId = comment.Id,
                            SubmittedBy = a.SubmittedBy,
                            SubmittedOn = a.SubmittedOn
                        });
                }

                repo.InsertComment(comment);
                repo.Save();

                // TODO notify other parties

                TempData["close"] = true;
                TempData[GlobalProperty.Message.SuccessMessage.ToString()] = "Comments successfully submitted";
                return RedirectToAction("_AddComment", new { requestId = model.RequestId, requestUniqueId = model.RequestUniqueId });
            }

            return View(model);
        }

        public ActionResult _AddVendor(string name)
        {
            return View(new Vendor() { State = "CA", Name = name });
        }

        [HttpPost]
        public ActionResult _AddVendor(Vendor vendor, FormCollection collection)
        {
            VendorRepository repo = new VendorRepository();

            vendor.Type = (int)RequestRepository.RequestType.Dpo;
            vendor.SubmittedOn = DateTime.Now;
            vendor.SubmittedBy = User.Identity.Name;
            vendor.LastModified = DateTime.Now;
            vendor.LastModifiedBy = User.Identity.Name;
            vendor.State = vendor.State.ToUpper();
            vendor.UnitId = User.Profile().DefaultUnitId;

            if (ModelState.IsValid)
            {
                if (repo.VendorNameExists(vendor.Name, User.Profile().DefaultUnitId, (int)RequestRepository.RequestType.Dpo))
                {
                    ModelState.AddModelError("Name", "This name already exists");
                    return View(vendor);
                }

                if (repo.VendorFriendlyNameExists(vendor.FriendlyName, User.Profile().DefaultUnitId, (int)RequestRepository.RequestType.Dpo))
                {
                    ModelState.AddModelError("FriendlyName", "This name already exists");
                    return View(vendor);
                }

                repo.InsertVendor(vendor);
                repo.Save();
                TempData["close"] = true;
                TempData["Id"] = vendor.Id;
                return RedirectToAction("_AddVendor");
            }

            return View(vendor);
        }

        public ActionResult _AddAddress(string name)
        {
            return View(new ShipToAddress()
            {
                FriendlyName = name,
                State = "CA",
                Campus = "University of California, Davis",
                City = "Davis",
                Street = "One Shields Ave.",
                Zip = "95616-8527"
            });
        }

        [HttpPost]
        public ActionResult _AddAddress(ShipToAddress address, FormCollection collection)
        {
            ShipToAddressRepository repo = new ShipToAddressRepository();
            address.SubmittedOn = DateTime.Now;
            address.SubmittedBy = User.Identity.Name;
            address.State = address.State.ToUpper();
            address.UnitId = User.Profile().DefaultUnitId;

            if (ModelState.IsValid)
            {
                if (repo.AddressFriendlyNameExists(address.FriendlyName, User.Profile().DefaultUnitId))
                {
                    ModelState.AddModelError("FriendlyName", "This name already exists");
                    return View(address);
                }

                repo.InsertAddress(address);
                repo.Save();
                TempData["close"] = true;
                TempData["Id"] = address.Id;
                return RedirectToAction("_AddAddress");
            }

            return View(address);
        }

        public ActionResult _StopRequest(int requestId, Guid requestUniqueId)
        {
            return View(new _StopRequestViewModel()
            {
                RequestId = requestId,
                RequestUniqueId = requestUniqueId,
                Comments = null
            });
        }

        [HttpPost]
        public ActionResult _StopRequest(_StopRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                EventMapRepository eventRepo = new EventMapRepository();

                eventRepo.InsertEventMap(new EventMap()
                {
                    RequestId = model.RequestId,
                    Comments = model.Comments,
                    SubmittedBy = User.Identity.Name,
                    SubmittedOn = DateTime.Now,
                    EventValue = Convert.ToInt32(EventMapRepository.EventMapType.Stop_Requested)
                });

                eventRepo.Save();

                TempData["close"] = true;
                TempData[GlobalProperty.Message.SuccessMessage.ToString()] = "Stop successfully requested!";
                return RedirectToAction("_StopRequest", new { requestId = model.RequestId, requestUniqueId = model.RequestUniqueId });
            }

            return View(model);
        }

        public ActionResult _MoreItemInfo(_MoreItemInfoViewModel model)
        {
            CommodityTypeRepository repo = new CommodityTypeRepository();
            model.CommodityTypes = new SelectList(repo.GetCommodityTypesByUnit(User.Profile().DefaultUnitId), "Id", "CommodityName", model.CommodityTypeId);
            return View(model);
        }

        [HttpPost]
        public ActionResult _MoreItemInfo(_MoreItemInfoViewModel model, FormCollection collection)
        {
            CommodityTypeRepository repo = new CommodityTypeRepository();
            model.CommodityTypes = new SelectList(repo.GetCommodityTypesByUnit(User.Profile().DefaultUnitId), "Id", "CommodityName", model.CommodityTypeId);

            if (ModelState.IsValid)
                TempData["close"] = true;

            return View(model);
        }

        public ActionResult _GetVendors(int? selectedValue, int vendorType)
        {
            VendorRepository repo = new VendorRepository();
            return new JsonResult { Data = new SelectList(repo.GetVendors(User.Profile().DefaultUnitId, vendorType), "Id", "FriendlyName", selectedValue) };
        }

        public ActionResult _GetAddresses(int? selectedValue)
        {
            ShipToAddressRepository repo = new ShipToAddressRepository();
            return new JsonResult { Data = new SelectList(repo.GetAddresses(User.Profile().DefaultUnitId), "Id", "FriendlyName", selectedValue) };
        }

        #endregion

        #region MyRequests

        //
        // GET: /Business/Purchasing/My-Requests
        [ActionName("my-requests")]
        public ActionResult MyRequests()
        {
            RequestRepository requestRepo = new RequestRepository();
            return View(
                new MyRequestsViewModel()
                {
                    Requests = requestRepo.GetRequestsByUser(User.Identity.Name),
                    FilterList = from f in Enum.GetValues(typeof(RequestRepository.MyRequestFilter)).Cast<RequestRepository.MyRequestFilter>()
                                 select new RequestRepository.MyRequestFilterListItem()
                                 {
                                     Filter = f,
                                     Selected = f == RequestRepository.MyRequestFilter.All | false,
                                     RequestCount = requestRepo.GetRequestsByUser(User.Identity.Name, f).Count()
                                 }
                });
        }

        //
        // GET: /Business/Purchasing/MyRequestsFiltered
        public ActionResult MyRequestsFiltered(RequestRepository.MyRequestFilter filter)
        {
            RequestRepository requestRepo = new RequestRepository();

            return View(
                new MyRequestsViewModel()
                {
                    Requests = requestRepo.GetRequestsByUser(User.Identity.Name, filter),
                    Filter = filter,
                    FilterList = from f in Enum.GetValues(typeof(RequestRepository.MyRequestFilter)).Cast<RequestRepository.MyRequestFilter>()
                                 select new RequestRepository.MyRequestFilterListItem()
                                 {
                                     Filter = f,
                                     Selected = f == filter,
                                     RequestCount = requestRepo.GetRequestsByUser(User.Identity.Name, f).Count()
                                 }
                });
        }

        //
        // GET: /Business/Purchasing/MyRequestDetails
        public ActionResult MyRequestDetails(Guid requestUniqueId)
        {
            return View(
                new MyRequestDetailsViewModel()
                {
                    RequestUniqueId = requestUniqueId
                });
        }

        #endregion

        #region Admin

        //
        // GET: /Business/Purchasing/Admin
        [AdminUser]
        public ActionResult Admin()
        {
            RequestRepository requestRepo = new RequestRepository();
            Unit unit = User.Profile().Unit;

            return View(

                new AdminViewModel()
                {
                    Requests = requestRepo.GetRequestsByUnit(unit.Id),
                    Unit = unit,
                    FilterList = from f in Enum.GetValues(typeof(RequestRepository.AdminRequestFilter)).Cast<RequestRepository.AdminRequestFilter>()
                                 select new RequestRepository.AdminRequestFilterListItem()
                                 {
                                     Filter = f,
                                     Selected = f == RequestRepository.AdminRequestFilter.All | false,
                                     RequestCount = requestRepo.GetRequestsByUnit(unit.Id, f).Count()
                                 }
                });
        }

        //
        // GET: /Business/Purchasing/AdminFiltered
        [AdminUser]
        public ActionResult AdminFiltered(RequestRepository.AdminRequestFilter filter)
        {
            RequestRepository requestRepo = new RequestRepository();
            Unit unit = User.Profile().Unit;

            return View(

                new AdminViewModel()
                {
                    Requests = requestRepo.GetRequestsByUnit(unit.Id, filter),
                    Filter = filter,
                    Unit = unit,
                    FilterList = from f in Enum.GetValues(typeof(RequestRepository.AdminRequestFilter)).Cast<RequestRepository.AdminRequestFilter>()
                                 select new RequestRepository.AdminRequestFilterListItem()
                                 {
                                     Filter = f,
                                     Selected = f == filter | false,
                                     RequestCount = requestRepo.GetRequestsByUnit(unit.Id, f).Count()
                                 }
                });
        }

        #endregion
    }
}
