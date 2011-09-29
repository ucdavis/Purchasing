using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Castle.Components.DictionaryAdapter;
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

        #endregion Workgroup Address


        #region People Actions
        /// <summary>
        /// People Index Page
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="roleFilter">Role Id</param>
        /// <returns></returns>
        public ActionResult People(int id, string roleFilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(a => a.Index());
            }

            if(!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = WorgroupPeopleListModel.Create(Repository, _roleRepository, workgroup, roleFilter);
            ViewBag.rolefilter = roleFilter;
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
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup==null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if(!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            //Ensure role picked is valid.
            if(workgroupPeoplePostModel.Role != null)
            {
                var validRoleIds = new List<string>();
                validRoleIds.Add(Role.Codes.AccountManager);                
                validRoleIds.Add(Role.Codes.Purchaser);
                validRoleIds.Add(Role.Codes.Approver);
                validRoleIds.Add(Role.Codes.Requester);

                if (!validRoleIds.Contains(workgroupPeoplePostModel.Role.Id))
                {
                    ModelState.AddModelError("Role", "Invalid Role Selected");
                }
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

                if(user == null)
                {
                    //TODO: Do we want to just ignore these? Or report an error to the user?
                    continue;
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
        /// <summary>
        /// Search Users in the User Table and LDAP lookup if none found.
        /// </summary>
        /// <param name="searchTerm">Email or LoginId</param>
        /// <returns></returns>
        public JsonNetResult SearchUsers(string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Trim();

            var users = _userRepository.Queryable.Where(a => a.Email == searchTerm || a.Id == searchTerm).ToList();
            if (users.Count==0)
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

            //We don't want to show users that are not active
            var results =
                users.Where(a => a.IsActive).Select(a => new IdAndName(a.Id, string.Format("{0} {1}", a.FirstName, a.LastName))).ToList();
            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.Name }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">WorkgroupPermission ID</param>
        /// <param name="workgroupid"></param>
        /// <param name="rolefilter"></param>
        /// <returns></returns>
        public ActionResult DeletePeople(int id, int workgroupid, string rolefilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(workgroupid);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            
            if (!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            var workgrouppermission = _workgroupPermissionRepository.GetNullableById(id);
            if (workgrouppermission == null)
            {
                return this.RedirectToAction(a => a.People(workgroupid, rolefilter));
            }

            var viewModel = WorkgroupPeopleDeleteModel.Create(_workgroupPermissionRepository, workgrouppermission);

            ViewBag.rolefilter = rolefilter;
            return View(viewModel);

        }

        [HttpPost]
        public ActionResult DeletePeople(int id, int workgroupid, string rolefilter, WorkgroupPermission workgroupPermission, string[] roles)
        {
            var workgroup = _workgroupRepository.GetNullableById(workgroupid);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            if (!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var workgroupPermissionToDelete = _workgroupPermissionRepository.GetNullableById(id);
            if(workgroupPermissionToDelete == null)
            {
                return this.RedirectToAction(a => a.People(workgroupid, rolefilter));
            }

            var availableWorkgroupPermissions = _workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.User == workgroupPermissionToDelete.User && a.Role.Level >= 1 && a.Role.Level <= 4).ToList();
            if(availableWorkgroupPermissions.Count() == 1)
            {
                // TODO: Check for pending/open orders for this person. Set order to workgroup.
                _workgroupPermissionRepository.Remove(workgroupPermissionToDelete);
                Message = "Person successfully removed from role.";
                return this.RedirectToAction(a => a.People(workgroupid, rolefilter)); 
            }
            else
            {
                if(roles == null || roles.Count() == 0)
                {
                    Message = "Must select at least 1 role to delete";
                    var viewModel = WorkgroupPeopleDeleteModel.Create(_workgroupPermissionRepository, workgroupPermissionToDelete);

                    ViewBag.rolefilter = rolefilter;
                    return View(viewModel);
                }
                var removedCount = 0;
                foreach (var role in roles)
                {
                    // TODO: Check for pending/open orders for this person. Set order to workgroup.
                    var wp = _workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.User == workgroupPermissionToDelete.User && a.Role.Id == role).Single();
                    _workgroupPermissionRepository.Remove(wp);
                    removedCount++;
                }

                Message = string.Format("{0} roles removed from {1}", removedCount, workgroupPermissionToDelete.User.FullName);
                return this.RedirectToAction(a => a.People(workgroupid, rolefilter)); 
            }


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
        //public IEnumerable<WorkgroupPermission> WorkgroupPermissions { get; set; }
        public Role Role { get; set; }
        public List<IdAndName> Users { get; set; }
        public List<Role> Roles { get; set; }
        public List<UserRoles> UserRoles { get; set; } 

        /// <summary>
        /// Create ViewModel
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="roleRepository"></param>
        /// <param name="workgroup"></param>
        /// <param name="rolefilter">Role Id</param>
        /// <returns></returns>
        public static WorgroupPeopleListModel Create(IRepository repository, IRepositoryWithTypedId<Role, string> roleRepository ,Workgroup workgroup, string rolefilter)
        {
            Check.Require(repository != null);
            Check.Require(roleRepository != null);
            Check.Require(workgroup != null);

            var viewModel = new WorgroupPeopleListModel()
                                {
                                    Workgroup = workgroup,
                                    UserRoles = new List<UserRoles>()
                                };
            var allWorkgroupPermissions =
                repository.OfType<WorkgroupPermission>().Queryable.Where(a => a.Workgroup == workgroup && a.User.IsActive && a.Role.Level >= 1 && a.Role.Level <= 4);


            var workgroupPermissions = !string.IsNullOrWhiteSpace(rolefilter) ? allWorkgroupPermissions.Where(a => a.Role.Id == rolefilter) : allWorkgroupPermissions;
            foreach(var workgroupPermission in allWorkgroupPermissions)
            {
                if(workgroupPermissions.Where(a => a.User.Id == workgroupPermission.User.Id).Any())
                {
                    if (viewModel.UserRoles.Where(a => a.User.Id == workgroupPermission.User.Id).Any())
                    {
                        viewModel.UserRoles.Where(a => a.User.Id == workgroupPermission.User.Id).Single().Roles.Add(workgroupPermission.Role);
                    }
                    else
                    {
                        viewModel.UserRoles.Add(new UserRoles(workgroupPermission));
                    }
                }
            }

            viewModel.Roles = roleRepository.Queryable.Where(a => a.Level >= 1 && a.Level <= 4).ToList();
            return viewModel;
        }

    }

    public class UserRoles
    {
        public User User { get; set; }
        public IList<Role> Roles { get; set; }
        public int FirstWorkgroupPermissionId { get; set; }

        public UserRoles(WorkgroupPermission workgroupPermission)
        {
            User = workgroupPermission.User;
            FirstWorkgroupPermissionId = workgroupPermission.Id;
            Roles = new List<Role>();
            Roles.Add(workgroupPermission.Role);
        }

        public string RolesList
        {
            get
            {
                return string.Join(" ", Roles.Select(a => a.Name).ToArray());
            } 
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
            
            viewModel.Roles = roleRepository.Queryable.Where(a => a.Level >= 1 && a.Level <= 4).ToList();
            return viewModel;
        }

    }

    public class WorkgroupPeopleDeleteModel
    {
        public WorkgroupPermission WorkgroupPermission { get; set; }
        public List<WorkgroupPermission> WorkgroupPermissions { get; set; }
        public static WorkgroupPeopleDeleteModel Create(IRepository<WorkgroupPermission> workgroupPermissionRepository, WorkgroupPermission workgroupPermission)
        {
            Check.Require(workgroupPermissionRepository != null);
            Check.Require(workgroupPermission != null);
            var viewModel = new WorkgroupPeopleDeleteModel{WorkgroupPermission = workgroupPermission};
            viewModel.WorkgroupPermissions = workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroupPermission.Workgroup && a.User == workgroupPermission.User && a.Role.Level >=1 && a.Role.Level <= 4).ToList();

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

    public class WorkgroupAddressListModel
    {
        public Workgroup Workgroup { get; set; }
        public IEnumerable<WorkgroupAddress> WorkgroupAddresses { get; set; }

        public static WorkgroupAddressListModel Create (Workgroup workgroup)
        {
            Check.Require(workgroup != null);
            var viewModel = new WorkgroupAddressListModel {Workgroup = workgroup};
            viewModel.WorkgroupAddresses = workgroup.Addresses;
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
