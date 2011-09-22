using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Purchasing.Web.Services;
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
        public ActionResult People(int id, string rolefilter)
        {
            //if (!CurrentUser.IsInRole(Role.Codes.DepartmentalAdmin))
            //{
            //    Message = "You must be a department admin to access a workgroup's people";
            //    return this.RedirectToAction<ErrorController>(a => a.Index());
            //}
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return RedirectToAction("Index");
            }

            if(!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = WorgroupPeopleListModel.Create(Repository, workgroup, rolefilter);
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

            if(!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = WorgroupPeopleCreateModel.Create(_roleRepository, workgroup);
            
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddPeople(int id, WorkgroupPeoplePostModel workgroupPeoplePostModel)
        {
            // TODO double check role was selected, and users not empty, workgroup is valid
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup==null)
            {
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (!ModelState.IsValid)
            {
                
                var viewModel = WorgroupPeopleCreateModel.Create(_roleRepository, workgroup);

                if (workgroupPeoplePostModel.Role != null)
                {
                    viewModel.Role = workgroupPeoplePostModel.Role;
                }
                if (workgroupPeoplePostModel.Users != null && workgroupPeoplePostModel.Users.Count > 0)
                {
                    //viewModel.Users =
                    //    _userRepository.Queryable.Where(a => workgroupPeoplePostModel.Users.Contains(a.Id)).Select(
                    //        a => new IdAndName(a.Id, string.Format("{0} {1}", a.FirstName, a.LastName))).ToList();
                    var users = new List<IdAndName>();
                    foreach (var user in workgroupPeoplePostModel.Users)
                    {
                        var temp = _userRepository.GetNullableById(user);
                        if (temp != null)
                        {
                            users.Add(new IdAndName(temp.Id, temp.FullName));
                        }
                        else
                        {
                            var ldapuser = _searchService.FindUser(user);
                            if (ldapuser != null)
                            {
                               users.Add(new IdAndName(ldapuser.LoginId, string.Format("{0} {1}", ldapuser.FirstName, ldapuser.LastName)));
                            }
                        }
                    }
                    viewModel.Users = users;
                }

                return View(viewModel);
            }

            int successCount = 0;
            int failCount = 0;
            foreach (var u in workgroupPeoplePostModel.Users)
            {
                var user = _userRepository.GetNullableById(u);
                if (user == null)
                {
                    var ldapuser = _searchService.FindUser(u);
                    if (ldapuser != null)
                    {
                        user = new User(ldapuser.LoginId);
                        user.Email = ldapuser.EmailAddress;
                        user.FirstName = ldapuser.FirstName;
                        user.LastName = ldapuser.LastName;

                        _userRepository.EnsurePersistent(user);
                    }
                }

                if (!_workgroupPermissionRepository.Queryable.Where(a => a.Role == workgroupPeoplePostModel.Role && a.User == user && a.Workgroup == workgroup).Any())
                {
                    var workgroupPermission = new WorkgroupPermission();
                    workgroupPermission.Role = workgroupPeoplePostModel.Role;
                    workgroupPermission.User = _userRepository.GetNullableById(u);
                    workgroupPermission.Workgroup = Repository.OfType<Workgroup>().GetNullableById(id);

                    _workgroupPermissionRepository.EnsurePersistent(workgroupPermission);
                    successCount++;
                }
                else
                {
                    failCount++;
                }
                
            }

            Message = string.Format("Successfully added {0} people to workgroup as {1}. {2} not added because of duplicated role.", successCount,
                                    workgroupPeoplePostModel.Role.Name, failCount);

            return this.RedirectToAction(a=> a.People(id, workgroupPeoplePostModel.Role.Id));

        }
        public JsonNetResult SearchUsers(string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Trim();

            //var results =
            //    _userRepository.Queryable.Where(a => a.LastName.Contains(searchTerm) || a.Id.Contains(searchTerm) || a.FirstName.Contains(searchTerm)).Select(a => new IdAndName(a.Id, string.Format("{0} {1}", a.FirstName, a.LastName))).ToList();

            var users = _userRepository.Queryable.Where(a => a.Email == searchTerm || a.Id == searchTerm).ToList();
            if (users.Count!=1)
            {
                var ldapuser = _searchService.FindUser(searchTerm);
                if (ldapuser != null)
                {
                    Check.Require(!string.IsNullOrWhiteSpace(ldapuser.LoginId));
                    Check.Require(!string.IsNullOrWhiteSpace(ldapuser.EmailAddress));

                    var user = new User(ldapuser.LoginId);
                    user.Email = ldapuser.EmailAddress;
                    user.FirstName = ldapuser.FirstName;
                    user.LastName = ldapuser.LastName;

                    users.Add(user);
                }
            }

            var results =
                users.Select(a => new IdAndName(a.Id, string.Format("{0} {1}", a.FirstName, a.LastName))).ToList();
            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.Name }));
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

    public class WorgroupPeopleListModel
    {
        public Workgroup Workgroup { get; set; }
        public IEnumerable<WorkgroupPermission> WorkgroupPermissions { get; set; }
        public Role Role { get; set; }
        public List<IdAndName> Users { get; set; }
        public List<Role> Roles { get; set; }  

        public  static WorgroupPeopleListModel Create(IRepository repository, Workgroup workgroup, string rolefilter)
        {
            Check.Require(repository != null);

            Check.Require(workgroup != null);

            var viewModel = new WorgroupPeopleListModel()
                                {
                                    Workgroup = workgroup
                                };
            viewModel.WorkgroupPermissions =
                repository.OfType<WorkgroupPermission>().Queryable.Where(a => a.Workgroup == workgroup && a.User.IsActive);
            if (rolefilter != null)
            {
                viewModel.WorkgroupPermissions = viewModel.WorkgroupPermissions.Where(a => a.Role.Id == rolefilter);
            }
            viewModel.Roles = new List<Role>();
            return viewModel;
        }

    }

    public class WorgroupPeopleCreateModel
    {
        public Workgroup Workgroup { get; set; }
        [Required]
        public Role Role { get; set; }
        public List<IdAndName> Users { get; set; }
        public List<Role> Roles { get; set; }

        public static WorgroupPeopleCreateModel Create(IRepositoryWithTypedId<Role, string> roleRepository, Workgroup workgroup)
        {
            Check.Require(roleRepository != null);

            Check.Require(workgroup != null);

            var viewModel = new WorgroupPeopleCreateModel()
            {
                Workgroup = workgroup
            };
            
            viewModel.Roles = new List<Role>();
            viewModel.Roles.Add(roleRepository.GetNullableById(Role.Codes.AccountManager));
            viewModel.Roles.Add(roleRepository.GetNullableById(Role.Codes.Purchaser));
            viewModel.Roles.Add(roleRepository.GetNullableById(Role.Codes.Approver));
            viewModel.Roles.Add(roleRepository.GetNullableById(Role.Codes.Requester));
            return viewModel;
        }

    }


    public class WorkgroupPeoplePostModel
    {
        [Required(ErrorMessage = "Must add at least one user")]
        public List<string> Users { get; set; }
        [Required]
        public Role Role { get; set; }
    }


}
