

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using MvcContrib;
using Purchasing.Core.Domain;
using Purchasing.Web.Helpers;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using Purchasing.Web.Utility;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;

namespace Purchasing.Web.Controllers.Dev
{
    /// <summary>
    /// Controller for the Wizard class
    /// </summary>
    public class WizardController : ApplicationController
    {
	  private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<Role, string> _roleRepository;
        private readonly IRepository<WorkgroupPermission> _workgroupPermissionRepository;
        private readonly ISecurityService _securityService;
        private readonly IDirectorySearchService _searchService;
        private readonly IRepository<WorkgroupVendor> _workgroupVendorRepository;
        private readonly IRepositoryWithTypedId<Vendor, string> _vendorRepository;
        private readonly IRepositoryWithTypedId<VendorAddress, Guid> _vendorAddressRepository;
        private readonly IRepositoryWithTypedId<State, string> _stateRepository;
        private readonly IRepositoryWithTypedId<EmailPreferences, string> _emailPreferencesRepository;
        private readonly IRepository<WorkgroupAccount> _workgroupAccountRepository;
        private readonly IWorkgroupAddressService _workgroupAddressService;

        public WizardController(IRepository<Workgroup> workgroupRepository, 
            IRepositoryWithTypedId<User, string> userRepository, 
            IRepositoryWithTypedId<Role, string> roleRepository, 
            IRepository<WorkgroupPermission> workgroupPermissionRepository,
            ISecurityService securityService, IDirectorySearchService searchService,
            IRepository<WorkgroupVendor> workgroupVendorRepository, 
            IRepositoryWithTypedId<Vendor, string> vendorRepository, 
            IRepositoryWithTypedId<VendorAddress, Guid> vendorAddressRepository,
            IRepositoryWithTypedId<State, string> stateRepository,
            IRepositoryWithTypedId<EmailPreferences, string> emailPreferencesRepository, 
            IRepository<WorkgroupAccount> workgroupAccountRepository,
            IWorkgroupAddressService workgroupAddressService)
        {
            _workgroupRepository = workgroupRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _workgroupPermissionRepository = workgroupPermissionRepository;
            _securityService = securityService;
            _searchService = searchService;
            _workgroupVendorRepository = workgroupVendorRepository;
            _vendorRepository = vendorRepository;
            _vendorAddressRepository = vendorAddressRepository;
            _stateRepository = stateRepository;
            _emailPreferencesRepository = emailPreferencesRepository;
            _workgroupAccountRepository = workgroupAccountRepository;
            _workgroupAddressService = workgroupAddressService;
        }
    

        /// <summary>
        /// GET: /Wizard/
        /// View To start the Workgroup create wizard
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            return View();
        }

        //NOTE: This wizard will only allow adding stuff. No delete, edit or view. For that they will complete the wizard and go back to the Workgroup management

        /// <summary>
        /// Step 1
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateWorkgroup()
        {

            var user = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Single();

            var model = WorkgroupModifyModel.Create(user);

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateWorkgroup(Workgroup workgroup)
        {
            if (!ModelState.IsValid)
            {
                var model = WorkgroupModifyModel.Create(GetCurrentUser());
                model.Workgroup = workgroup;

                return View(model);
            }

            var workgroupToCreate = new Workgroup();

            Mapper.Map(workgroup, workgroupToCreate);
            workgroupToCreate.IsActive = false;

            if (!workgroupToCreate.Organizations.Contains(workgroupToCreate.PrimaryOrganization))
            {
                workgroupToCreate.Organizations.Add(workgroupToCreate.PrimaryOrganization);
            }

            _workgroupRepository.EnsurePersistent(workgroupToCreate);

            Message = string.Format("{0} workgroup was created",
                                    workgroup.Name);

            return this.RedirectToAction(a => a.AddSubOrganizations(workgroupToCreate.Id));
        }

       

        /// <summary>
        /// Step 2
        /// </summary>
        /// <returns></returns>
        public ActionResult AddSubOrganizations(int id)
        {
            Workgroup workgroup = _workgroupRepository.Queryable.Where(a => a.Id == id).Single();

            var user = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Single();

            var model = WorkgroupModifyModel.Create(user);
            model.Workgroup = workgroup;
            ViewBag.WorkgroupId = workgroup.Id;

            return View(model);
        }

        [HttpPost]
        public ActionResult AddSubOrganizations(int id, string[] selectedOrganizations)
        {
            Workgroup workgroup = _workgroupRepository.Queryable.Where(a => a.Id == id).Single();
            
            

            if (selectedOrganizations != null)
            {
                var existingOrganizations = workgroup.Organizations.Select(a => a.Id).ToList();
                var organizationsToAdd =
                    Repository.OfType<Organization>().Queryable.Where(a => selectedOrganizations.Contains(a.Id)).Select(
                        b => b.Id).ToList().Union(existingOrganizations).ToList();
                workgroup.Organizations =
                    Repository.OfType<Organization>().Queryable.Where(a => organizationsToAdd.Contains(a.Id)).ToList();
            }

            _workgroupRepository.EnsurePersistent(workgroup);

           return this.RedirectToAction(a => a.SubOrganizations(workgroup.Id));
        }

        public ActionResult SubOrganizations(int id)
        {
            Workgroup workgroup = _workgroupRepository.Queryable.Where(a => a.Id == id).Single();

            return View(workgroup);
        }

        /// <summary>
        /// Step 3
        /// </summary>
        /// <returns></returns>
        public ActionResult AddPeople(int id, string roleFilter)
        {
            ActionResult redirectToAction;
            var workgroup = GetWorkgroupAndCheckAccess(id, out redirectToAction);
            if (workgroup == null)
            {
                return redirectToAction;
            }

            var viewModel = WorgroupPeopleCreateModel.Create(_roleRepository, workgroup);
            if (!string.IsNullOrWhiteSpace(roleFilter))
            {
                viewModel.Role = _roleRepository.Queryable.Where(a => a.Level >= 1 && a.Level <= 4 && a.Id == roleFilter).SingleOrDefault();
            }
            ViewBag.rolefilter = roleFilter;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddPeople(int id, WizardWorkgroupPeoplePostModel workgroupPeoplePostModel, string roleFilter)
        {
            ActionResult redirectToAction;
            var workgroup = GetWorkgroupAndCheckAccess(id, out redirectToAction);
            if (workgroup == null)
            {
                return redirectToAction;
            }

            workgroupPeoplePostModel.Role = _roleRepository.Queryable.Where(a => a.Id == roleFilter).FirstOrDefault();
            //Ensure role picked is valid.
            if (workgroupPeoplePostModel.Role != null)
            {
                if (!_roleRepository.Queryable.Where(a => a.Level >= 1 && a.Level <= 4 && a.Id == workgroupPeoplePostModel.Role.Id).Any())
                {
                    ModelState.AddModelError("Role", "Invalid Role Selected - don't mess with the query string!");
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
                ViewBag.rolefilter = roleFilter;
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

                        var emailPrefs = new EmailPreferences(user.Id);
                        _emailPreferencesRepository.EnsurePersistent(emailPrefs);

                    }
                }

                if (user == null)
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

            return this.RedirectToAction(a => a.People(id, workgroupPeoplePostModel.Role.Id));
        }

        public ActionResult People(int id, string roleFilter)
        {
            ActionResult redirectToAction;
            var workgroup = GetWorkgroupAndCheckAccess(id, out redirectToAction);
            if (workgroup == null)
            {
                return redirectToAction;
            }

            var viewModel = WorgroupPeopleListModel.Create(_workgroupPermissionRepository, _roleRepository, workgroup, roleFilter);
            ViewBag.rolefilter = roleFilter;
            return View(viewModel);
        }


        /// <summary>
        /// Step 4
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAccounts(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(a => a.Index());
            }

            var viewModel = WorkgroupAccountModel.Create(Repository, workgroup);

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult AddAccounts(string temp)
        {
            return View();
        }

        public ActionResult Accounts()
        {
            return View();
        }

        /// <summary>
        /// Step 5
        /// </summary>
        /// <returns></returns>
        public ActionResult AddVendors()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddVendors(string temp)
        {
            return View();
        }

        public ActionResult Vendors()
        {
            return View();
        }

        /// <summary>
        /// Step 6
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAddresses()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAddresses(string temp)
        {
            return View();
        }

        public ActionResult Addresses()
        {
            return View();
        }

        /// <summary>
        /// Step 7
        /// </summary>
        /// <returns></returns>
        public ActionResult AddConditionalApproval()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddConditionalApproval(string temp)
        {
            return View();
        }

        public ActionResult ConditionalApprovals()
        {
            return View();
        }

        #region Private Helpers
        private Workgroup GetWorkgroupAndCheckAccess(int id, out ActionResult redirectToAction)
        {
            Workgroup workgroup;
            workgroup = _workgroupRepository.Queryable.Where(a=>a.Id==id).Single();

            string message;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(workgroup, null, out message))
            {
                Message = message;
                {
                    redirectToAction = this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
                    return null;
                }
            }
            redirectToAction = null;
            return workgroup;
        }
        #endregion

        public JsonNetResult SearchUsers(string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Trim();

            var users = _userRepository.Queryable.Where(a => a.Email == searchTerm || a.Id == searchTerm).ToList();
            if (users.Count == 0)
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
    }


}
