using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Web.Utility;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using MvcContrib;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the WorkgroupManagement class
    /// </summary>
    public class JandJWorkgroupManagementController : ApplicationController
    {
        private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IRepository<WorkgroupAddress> _workgroupAddressRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<Role, string> _roleRepository;


        public JandJWorkgroupManagementController(IRepository<Workgroup> workgroupRepository, IRepository<WorkgroupAddress> workgroupAddressRepository, IRepositoryWithTypedId<User, string> userRepository, IRepositoryWithTypedId<Role, string> roleRepository)
        {
            _workgroupRepository = workgroupRepository;
            _workgroupAddressRepository = workgroupAddressRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
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

        #region People Actions
        /// <summary>
        /// People Index Page
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public ActionResult People(int id)
        {
            if (!CurrentUser.IsInRole(Role.Codes.DepartmentalAdmin))
            {
                Message = "You must be a department admin to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return RedirectToAction("Index");
            }

            var viewModel = WorgroupPeopleModel.Create(Repository, workgroup);
            return View(viewModel);
            
        }

        public ActionResult AddPeople(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            var viewModel = WorgroupPeopleModel.Create(Repository, workgroup);
            viewModel.Roles.Add(_roleRepository.GetNullableById((Role.Codes.AccountManager)));
            viewModel.Roles.Add(_roleRepository.GetNullableById(Role.Codes.Purchaser));
            viewModel.Roles.Add(_roleRepository.GetNullableById(Role.Codes.Approver));
            viewModel.Roles.Add(_roleRepository.GetNullableById(Role.Codes.Requester));
            return View(viewModel);
        }

        public JsonNetResult SearchUsers(string searchTerm)
        {
            var results =
                _userRepository.Queryable.Where(a => a.LastName.Contains(searchTerm) || a.Id.Contains(searchTerm) || a.FirstName.Contains(searchTerm)).Select(a => new IdAndName(a.Id, string.Format("{0} {1}", a.FirstName, a.LastName))).ToList();

            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.Name }));
        }

        [HttpPost]
        public  ActionResult AddPeople(int id, WorkgroupPeoplePostModel workgroupPeoplePostModel )
        {
            return View();

        }

        #endregion People Actions

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

    public class WorgroupPeopleModel
    {
        public Workgroup Workgroup { get; set; }
        public IEnumerable<WorkgroupPermission> WorkgroupPermissions { get; set; }
        public Role Role { get; set; }
        public List<IdAndName> Users { get; set; }
        public List<Role> Roles { get; set; }  

        public  static WorgroupPeopleModel Create(IRepository repository, Workgroup workgroup)
        {
            Check.Require(repository != null);

            Check.Require(workgroup != null);

            var viewModel = new WorgroupPeopleModel()
                                {
                                    Workgroup = workgroup
                                };
            viewModel.WorkgroupPermissions =
                repository.OfType<WorkgroupPermission>().Queryable.Where(a => a.Workgroup == workgroup && a.User.IsActive);
            viewModel.Roles = new List<Role>();
            return viewModel;
        }

    }

    public class WorkgroupPeoplePostModel
    {
        public List<string> Users { get; set; }
        public Role Role { get; set; }
    }


}
