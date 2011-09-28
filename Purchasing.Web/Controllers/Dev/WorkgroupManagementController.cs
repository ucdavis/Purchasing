using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;
using UCDArch.Web.Attributes;
using MvcContrib;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the WorkgroupManagement class
    /// </summary>
    public class WorkgroupManagementController : ApplicationController
    {
        private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IRepository<WorkgroupAddress> _workgroupAddressRepository;

        public WorkgroupManagementController(IRepository<Workgroup> workgroupRepository, IRepository<WorkgroupAddress> workgroupAddressRepository)
        {
            _workgroupRepository = workgroupRepository;
            _workgroupAddressRepository = workgroupAddressRepository;
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

        #region People Actions
        /// <summary>
        /// People Index Page
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public ActionResult People(int id)
        {
            throw new NotImplementedException();
        }

        public ActionResult AddPeople(int id)
        {
            throw new NotImplementedException();
        }

        #endregion People Actions

        #region Workgroup Accounts
        public ActionResult Accounts(int id)
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
        #endregion

        #region Workgroup Vendors
        public ActionResult Vendors(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(a => a.Index());
            }

            var viewModel = WorkgroupVendorsModel.Create(Repository, workgroup);

            return View(viewModel);
        }
        #endregion

        #region Addresses
        public ActionResult Addresses(int id)
        {
            var workgroup =
                _workgroupRepository.Queryable.Where(x => x.Id == id).Fetch(x => x.Addresses).SingleOrDefault();

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return RedirectToAction("Index");
            }

            return View(workgroup);
        }

        [HttpPost]
        [BypassAntiForgeryToken] //TODO: Add in token
        public ActionResult EditAddress(int workgroupId, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _workgroupRepository.GetById(workgroupId);
            workgroupAddress.Workgroup = workgroup;

            _workgroupAddressRepository.EnsurePersistent(workgroupAddress);

            return Json(new { id = workgroupAddress.Id });
        }
        #endregion
    }

    public class WorkgroupManageModel
    {
        public Workgroup Workgroup { get; set; }

        public virtual int OrganizationCount { get; set; }
        public virtual int AccountCount { get; set; }
        public virtual int VendorCount { get; set; }
        public virtual int AddressCount { get; set; }
        public virtual int UserCount { get; set; }
        public virtual int ApproverCount { get; set; }
        public virtual int AccountManagerCount { get; set; }
        public virtual int PurchaserCount { get; set; }
    }

    public class WorkgroupVendorsModel
    {
        public Workgroup Workgroup { get; set; }
        public IEnumerable<WorkgroupPermission> WorkGroupPermissions { get; set; }

        public static WorkgroupVendorsModel Create(IRepository repository, Workgroup workgroup)
        {
            Check.Require(repository != null);
            Check.Require(workgroup != null);
            var viewModel = new WorkgroupVendorsModel { Workgroup = workgroup };
            viewModel.WorkGroupPermissions = repository.OfType<WorkgroupPermission>().Queryable.Where(a => a.Workgroup == workgroup).ToList();

            return viewModel;
        }
    }
}
