

using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using MvcContrib;
using Purchasing.Core.Domain;
using Purchasing.Web.Helpers;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

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
            return View();
        }

        [HttpPost]
        public ActionResult AddSubOrganizations(string temp)
        {
            return View();
        }

        public ActionResult SubOrganizations()
        {
            return View();
        }

        /// <summary>
        /// Step 3
        /// </summary>
        /// <returns></returns>
        public ActionResult AddPeople()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPeople(string temp)
        {
            return View();
        }

        public ActionResult People()
        {
            return View();
        }


        /// <summary>
        /// Step 4
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAccounts()
        {
            return View();
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
    }

}
