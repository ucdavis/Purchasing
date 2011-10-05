using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Castle.Components.DictionaryAdapter;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using Purchasing.Web.Utility;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using MvcContrib;
using UCDArch.Web.Helpers;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the WorkgroupManagement class
    /// </summary>
    public class JandJWorkgroupManagementController : ApplicationController
    {
        private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IDirectorySearchService _searchService;
        private readonly IRepository<WorkgroupAddress> _workgroupAddressRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<Role, string> _roleRepository;
        private readonly IRepository<WorkgroupPermission> _workgroupPermissionRepository;
        private readonly IHasAccessService _hasAccessService;


        public JandJWorkgroupManagementController(IRepository<Workgroup> workgroupRepository, IDirectorySearchService searchService, IRepository<WorkgroupAddress> workgroupAddressRepository, IRepositoryWithTypedId<User, string> userRepository, IRepositoryWithTypedId<Role, string> roleRepository, IRepository<WorkgroupPermission> workgroupPermission, IHasAccessService  hasAccessService)
        {
            _workgroupRepository = workgroupRepository;
            _searchService = searchService;
            _workgroupAddressRepository = workgroupAddressRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _workgroupPermissionRepository = workgroupPermission;
            _hasAccessService = hasAccessService;
        }

        //
        // GET: /WorkgroupManagement/
        /// <summary>
        /// TODO: For testing only!
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var workgroups = _workgroupRepository.Queryable;

            return View(workgroups.ToList());
        }


        public ActionResult Manage(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return RedirectToAction("Index");
            }

            var workgroupPermsByGroup = (from wp in Repository.OfType<WorkgroupPermission>().Queryable
                                         where wp.Workgroup.Id == workgroup.Id
                                         group wp.Role by wp.Role.Id
                                             into role
                                             select new { count = role.Count(), name = role.Key }).ToList();

            var model = new WorkgroupManageModel
                            {
                                Workgroup = workgroup,
                                OrganizationCount = workgroup.Organizations.Count(),
                                AccountCount = workgroup.Accounts.Count(),
                                VendorCount = workgroup.Vendors.Count(),
                                AddressCount = workgroup.Addresses.Count(),
                                UserCount =
                                    workgroupPermsByGroup.Where(x => x.name == Role.Codes.Requester).Select(x => x.count).
                                    SingleOrDefault(),
                                ApproverCount =
                                    workgroupPermsByGroup.Where(x => x.name == Role.Codes.Approver).Select(x => x.count)
                                    .SingleOrDefault(),
                                AccountManagerCount =
                                    workgroupPermsByGroup.Where(x => x.name == Role.Codes.AccountManager).Select(
                                        x => x.count).SingleOrDefault(),
                                PurchaserCount =
                                    workgroupPermsByGroup.Where(x => x.name == Role.Codes.Purchaser).Select(x => x.count)
                                    .SingleOrDefault()
                            };

            return View(model);
        }

        public ActionResult Accounts(int id)
        {
            throw new NotImplementedException();
        }

        public ActionResult Vendors(int id)
        {
            throw new NotImplementedException();
        }

        //public ActionResult Addresses(int id)
        //{
        //    var workgroup =
        //        _workgroupRepository.Queryable.Where(x => x.Id == id).Fetch(x => x.Addresses).SingleOrDefault();

        //    if (workgroup == null)
        //    {
        //        ErrorMessage = "Workgroup could not be found";
        //        return RedirectToAction("Index");
        //    }

        //    return View(workgroup);
        //}

        #region Workgroup Address

        public ActionResult Addresses(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var viewModel = WorkgroupAddressListModel.Create(workgroup);
            return View(viewModel);
        }

        public ActionResult AddAddress (int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            var viewModel = WorkgroupAddressViewModel.Create(workgroup);
            viewModel.WorkgroupAddress = new WorkgroupAddress();
            viewModel.WorkgroupAddress.Workgroup = workgroup;
            return View(viewModel);

        }

        [HttpPost]
        public ActionResult AddAddress (int id, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index());
            }
            workgroupAddress.Workgroup = workgroup;
            ModelState.Clear();
            workgroupAddress.TransferValidationMessagesTo(ModelState);
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Address not valid";
                var viewModel = WorkgroupAddressViewModel.Create(workgroup);
                viewModel.WorkgroupAddress = workgroupAddress;
                viewModel.WorkgroupAddress.Workgroup = workgroup;
                return View(viewModel);
            }
            var matchFound = 0;
            foreach (var address in workgroup.Addresses )
            {
                matchFound = CompareAddress(workgroupAddress, address);
                if (matchFound > 0 )
                {
                    break;
                }
            }
            if (matchFound > 0)
            {
                var matchedAddress = workgroup.Addresses.Where(a => a.Id == matchFound).Single();
                if (!matchedAddress.IsActive)
                {
                    Message = "Address created.";
                    matchedAddress.IsActive = true;
                    _workgroupRepository.EnsurePersistent(workgroup);
                } else
                {
                    Message = "This Address already exists.";
                }
            }
            else
            {
                Message = "Address created";
                workgroup.AddAddress(workgroupAddress);
                _workgroupRepository.EnsurePersistent(workgroup);
            }
            return this.RedirectToAction(a => a.Addresses(id));
        }

        private static int CompareAddress(WorkgroupAddress workgroupAddress, WorkgroupAddress address)
        {
            int matchFound = address.Id;
            if (workgroupAddress.Address.ToLower() != address.Address.ToLower())
            {
                matchFound = 0;
            }
            if (!string.IsNullOrWhiteSpace(workgroupAddress.Building) && !string.IsNullOrWhiteSpace(address.Building))
            {
                if (workgroupAddress.Building.ToLower() != address.Building.ToLower())
                {
                    matchFound = 0;
                }
            }
            if ((!string.IsNullOrWhiteSpace(workgroupAddress.Building) && string.IsNullOrWhiteSpace(address.Building)) ||
                (string.IsNullOrWhiteSpace(workgroupAddress.Building) && !string.IsNullOrWhiteSpace(address.Building)))
            {
                matchFound = 0;
            }
            if (!string.IsNullOrWhiteSpace(workgroupAddress.Room) && !string.IsNullOrWhiteSpace(address.Room))
            {
                if (workgroupAddress.Room.ToLower() != address.Room.ToLower())
                {
                    matchFound = 0;
                }
            }
            if ((!string.IsNullOrWhiteSpace(workgroupAddress.Room) && string.IsNullOrWhiteSpace(address.Room)) ||
                (string.IsNullOrWhiteSpace(workgroupAddress.Room) && !string.IsNullOrWhiteSpace(address.Room)))
            {
                matchFound = 0;
            }
            if (workgroupAddress.Name.ToLower() != address.Name.ToLower())
            {
                matchFound = 0;
            }
            if (workgroupAddress.City.ToLower() != address.City.ToLower())
            {
                matchFound = 0;
            }
            if (workgroupAddress.State.ToLower() != address.State.ToLower())
            {
                matchFound = 0;
            }
            if (workgroupAddress.Zip.ToLower() != address.Zip.ToLower())
            {
                matchFound = 0;
            }
            if (!string.IsNullOrWhiteSpace(workgroupAddress.Phone) && !string.IsNullOrWhiteSpace(address.Phone))
            {
                if (workgroupAddress.Phone.ToLower() != address.Phone.ToLower())
                {
                    matchFound = 0;
                }
            }
            if ((!string.IsNullOrWhiteSpace(workgroupAddress.Phone) && string.IsNullOrWhiteSpace(address.Phone)) ||
                (string.IsNullOrWhiteSpace(workgroupAddress.Phone) && !string.IsNullOrWhiteSpace(address.Phone)))
            {
                matchFound = 0;
            }
            return matchFound;
        }

        #endregion Workgroup Address


        [HttpPost]
        [BypassAntiForgeryToken] //TODO: Add in token
        public ActionResult EditAddress(int workgroupId, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _workgroupRepository.GetById(workgroupId);
            workgroupAddress.Workgroup = workgroup;

            _workgroupAddressRepository.EnsurePersistent(workgroupAddress);

            return Json(new { id = workgroupAddress.Id });
        }
    }

    //public class WorkgroupManageModel
    //{
    //    public Workgroup Workgroup { get; set; }

    //    public virtual int OrganizationCount { get; set; }
    //    public virtual int AccountCount { get; set; }
    //    public virtual int VendorCount { get; set; }
    //    public virtual int AddressCount { get; set; }
    //    public virtual int UserCount { get; set; }
    //    public virtual int ApproverCount { get; set; }
    //    public virtual int AccountManagerCount { get; set; }
    //    public virtual int PurchaserCount { get; set; }
    //}


    public class WorkgroupAddressListModel
    {
        public Workgroup Workgroup { get; set; }
        public IEnumerable<WorkgroupAddress> WorkgroupAddresses { get; set; }

        public static WorkgroupAddressListModel Create (Workgroup workgroup)
        {
            Check.Require(workgroup != null);
            var viewModel = new WorkgroupAddressListModel {Workgroup = workgroup};
            viewModel.WorkgroupAddresses = workgroup.Addresses.Where(a=>a.IsActive);
            return viewModel;
        }
    }

    public class WorkgroupAddressViewModel
    {
        public Workgroup Workgroup { get; set; }
        public WorkgroupAddress WorkgroupAddress { get; set; }

        public static WorkgroupAddressViewModel Create (Workgroup workgroup)
        {
            Check.Require(workgroup != null);
            var viewModel = new WorkgroupAddressViewModel {Workgroup = workgroup};
            return viewModel;
        }
    }

}
