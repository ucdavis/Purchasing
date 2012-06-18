using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Attributes;
using Purchasing.Web.Helpers;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;
using AutoMapper;
using System.Collections.Generic;
using UCDArch.Web.ActionResults;
using Purchasing.Web.Utility;
using MvcContrib;
using UCDArch.Web.Attributes;
using UCDArch.Web.Helpers;
using NPOI.HSSF.UserModel;
using Purchasing.Core.Repositories;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Workgroup class
    /// </summary>
    [Authorize(Roles = Role.Codes.DepartmentalAdmin)]
    [AuthorizeWorkgroupAccess]
    public class WorkgroupController : ApplicationController
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
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IWorkgroupAddressService _workgroupAddressService;
        private readonly IWorkgroupService _workgroupService;

        public WorkgroupController(IRepository<Workgroup> workgroupRepository, 
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
            IQueryRepositoryFactory queryRepositoryFactory,
            IRepositoryFactory repositoryFactory,
            IWorkgroupAddressService workgroupAddressService,
            IWorkgroupService workgroupService)
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
            _queryRepositoryFactory = queryRepositoryFactory;
            _repositoryFactory = repositoryFactory;
            _workgroupAddressService = workgroupAddressService;
            _workgroupService = workgroupService;
        }

        

        #region Workgroup Actions
        /// <summary>
        /// Actions #1
        /// GET: /Workgroup/
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(bool showAll = false)
        {
            /*
            // load the person's orgs
            var person = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Fetch(x => x.Organizations).Single();
            var porgs = person.Organizations.Select(x => x.Id).ToList();
            
            // get the administrative rollup on orgs
            var wgIds = _queryRepositoryFactory.AdminWorkgroupRepository.Queryable.Where(a => porgs.Contains(a.RollupParentId)).Select(a => a.WorkgroupId).ToList();

            // get the workgroups
            var workgroups = _workgroupRepository.Queryable.Where(a => wgIds.Contains(a.Id));
            if(!showAll)
            {
                workgroups = workgroups.Where(a => a.IsActive);
            }
            */

            var workgroups = _workgroupService.LoadAdminWorkgroups(showAll);

            var viewModel = new WorkgroupIndexModel();
            viewModel.WorkGroups = workgroups.ToList();
            viewModel.ShowAll = showAll;
            var columnPreferences =
                _repositoryFactory.ColumnPreferencesRepository.GetNullableById(CurrentUser.Identity.Name) ??
                new ColumnPreferences(CurrentUser.Identity.Name);
            ViewBag.DataTablesPageSize = columnPreferences.DisplayRows;

            return View(viewModel);
        }

        ///// <summary>
        ///// Actions #2
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult Create()
        //{
        //    var user = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Single();

        //    var model = WorkgroupModifyModel.Create(user, _queryRepositoryFactory);

        //    return View(model);
        //}

        ///// <summary>
        ///// Actions #3
        ///// </summary>
        ///// <param name="workgroup"></param>
        ///// <param name="selectedOrganizations"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult Create(Workgroup workgroup, string[] selectedOrganizations)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var model = WorkgroupModifyModel.Create(GetCurrentUser(), _queryRepositoryFactory);
        //        model.Workgroup = workgroup;

        //        return View(model);
        //    }

        //    var createdWorkgroup = _workgroupService.CreateWorkgroup(workgroup, selectedOrganizations);

        //    Message = string.Format("{0} workgroup was created",
        //                            createdWorkgroup.Name);

        //    return this.RedirectToAction(a => a.Index(false));
        //}

        /// <summary>
        /// Actions #4
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(a => a.Index(false));
            }

            var viewModel = WorkgroupDetailsViewModel.Create(_workgroupPermissionRepository, Repository.OfType<ConditionalApproval>(), workgroup);

            return View(viewModel);

        }

        /// <summary>
        /// Actions #5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var user = _userRepository.Queryable.Single(x => x.Id == CurrentUser.Identity.Name);
            var workgroup = _workgroupRepository.GetNullableById(id);

            var model = WorkgroupModifyModel.Create(user, _queryRepositoryFactory, workgroup);

            return View(model);
        }

        /// <summary>
        /// Actions #6
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="workgroup"></param>
        /// <param name="selectedOrganizations"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, Workgroup workgroup, string[] selectedOrganizations)
        {
            var workgroupToEdit = _workgroupRepository.GetNullableById(id);
            if(workgroupToEdit == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            Mapper.Map(workgroup, workgroupToEdit);
            workgroupToEdit.PrimaryOrganization = workgroup.PrimaryOrganization;

            if (selectedOrganizations != null)
            {                
                workgroupToEdit.Organizations =
                    Repository.OfType<Organization>().Queryable.Where(a => selectedOrganizations.Contains(a.Id)).ToList();
            }

            if (!workgroupToEdit.Organizations.Contains(workgroupToEdit.PrimaryOrganization))
            {
                workgroupToEdit.Organizations.Add(workgroupToEdit.PrimaryOrganization);
            }

            if(workgroupToEdit.Administrative)
            {
                if(_workgroupPermissionRepository.Queryable.Any(a => a.Workgroup.Id == workgroupToEdit.Id && a.Role.Id == Role.Codes.Requester))
                {
                    ModelState.AddModelError("Workgroup.Administrative", "A workgroup can't be made administrative if there are any Requestors.");
                }
                if(_workgroupAccountRepository.Queryable.Any(a => a.Workgroup.Id == workgroupToEdit.Id))
                {
                    ModelState.AddModelError("Workgroup.Administrative", "A workgroup can't be made administrative if there are any Accounts.");
                }
                if(_workgroupVendorRepository.Queryable.Any(a => a.Workgroup.Id == workgroupToEdit.Id && a.IsActive))
                {
                    ModelState.AddModelError("Workgroup.Administrative", "A workgroup can't be made administrative if there are any vendors.");
                }
                if(Repository.OfType<WorkgroupAddress>().Queryable.Any(a => a.Workgroup.Id == workgroupToEdit.Id && a.IsActive))
                {
                    ModelState.AddModelError("Workgroup.Administrative", "A workgroup can't be made administrative if there are any addresses.");
                }
            }

            if(workgroupToEdit.Administrative && workgroupToEdit.SyncAccounts)
            {
                ModelState.AddModelError("Workgroup.Administrative", "Can not have both Administrative and Sync Accounts selected.");
            }

            if (workgroup.SharedOrCluster && !workgroup.Administrative)
            {
                ModelState.AddModelError("Workgroup.Administrative", "If shared or cluster, workgroup must be administrative.");
            }

            //TODO: Test this.
            if(!ModelState.IsValid)
            {
                //Moved here because if you just pass workgroup, it doesn't have any selected organizations.
                return View(WorkgroupModifyModel.Create(GetCurrentUser(), _queryRepositoryFactory, workgroupToEdit));
            }

            _workgroupRepository.EnsurePersistent(workgroupToEdit);

            Message = string.Format("{0} was modified successfully",
                                    workgroup.Name);

            return this.RedirectToAction(a => a.Index(false));

        }

        /// <summary>
        /// Actions #7
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                ErrorMessage = "Workgroup not found";
                return this.RedirectToAction(a => a.Index(false));
            }

            return View(workgroup);
        }

        /// <summary>
        /// Actions #8
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="workgroup"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, Workgroup workgroup)
        {
            var workgroupToDelete = _workgroupRepository.GetNullableById(id);
            if(workgroupToDelete == null)
            {
                ErrorMessage = "Workgroup not found";
                return this.RedirectToAction(a => a.Index(false));
            }
            workgroupToDelete.IsActive = false;

            _workgroupRepository.EnsurePersistent(workgroupToDelete);

            Message = string.Format("{0} was disabled successfully",
                                    workgroup.Name);

            return this.RedirectToAction(a => a.Index(false));

        }
        #endregion

        #region Workgroup Accounts
        /// <summary>
        /// Accounts #1
        /// GET: Workgroup/Accounts/{Workgroup Id}
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public ActionResult Accounts(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(a => a.Index(false));
            }

            return View(workgroup);
        }

        /// <summary>
        /// Accounts #2
        /// GET : Workgroup/AddAccount/{Workgroup Id}
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public ActionResult AddAccount(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(a => a.Index(false));
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Account may not be added to an administrative workgroup.";
                return this.RedirectToAction(a => a.Details(workgroup.Id));
            }

            if(workgroup.SyncAccounts)
            {
                ErrorMessage = "Accounts should not be added when Synchronize Accounts is selected because they my be overwritten.";
                return this.RedirectToAction(a => a.Details(workgroup.Id));
            }

            var viewModel = WorkgroupAccountModel.Create(Repository, workgroup);

            return View(viewModel);
        }

        /// <summary>
        /// Accounts #3
        /// POST: Workgroup/AddAccount
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="workgroupAccount">Workgroup Account Model</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddAccount(int id, WorkgroupAccount workgroupAccount, string account_search)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(a => a.Index(false));
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Accounts may not be added to an administrative workgroup.";
                return this.RedirectToAction(a => a.Details(workgroup.Id));
            }


            var workgroupAccountToCreate = new WorkgroupAccount() {Workgroup = workgroup};
            Mapper.Map(workgroupAccount, workgroupAccountToCreate);

            ModelState.Clear();
            //workgroupAccountToCreate.TransferValidationMessagesTo(ModelState);

            if(workgroupAccountToCreate.Account == null)
            {
                workgroupAccountToCreate.Account = _repositoryFactory.AccountRepository.GetNullableById(account_search);
            }

            if(workgroupAccountToCreate.Account == null)
            {
                ModelState.AddModelError("WorkgroupAccount.Account", "Account not found");
            }
            else
            {
                if (_workgroupAccountRepository.Queryable.Any(a => a.Workgroup.Id == workgroup.Id && a.Account.Id == workgroupAccountToCreate.Account.Id))
                {
                    ModelState.AddModelError("WorkgroupAccount.Account", "Account already exists for this workgroup");
                }
            }

            if(ModelState.IsValid)
            {
                workgroupAccountToCreate.TransferValidationMessagesTo(ModelState);
            }

            if (ModelState.IsValid)
            {
                _workgroupAccountRepository.EnsurePersistent(workgroupAccountToCreate);
                Message = "Workgroup account saved.";
                //return this.RedirectToAction("Accounts", new {id = id});
                return this.RedirectToAction(a => a.Accounts(id));
            }

            var viewModel = WorkgroupAccountModel.Create(Repository, workgroup, workgroupAccountToCreate);
            return View(viewModel);
        }

        /// <summary>
        /// Accounts #4
        /// GET: Workgroup/AccountDetails
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="accountId">Workgroup Account Id</param>
        /// <returns></returns>
        public ActionResult AccountDetails(int id, int accountId)
        {
            var account = _workgroupAccountRepository.GetNullableById(accountId);

            if (account == null)
            {
                ErrorMessage = "Account could not be found";
                return this.RedirectToAction(a => a.Index(false));
            }
            if(account.Workgroup.Id != id)
            {
                ErrorMessage = "Account not part of workgroup";
                return this.RedirectToAction(a => a.Index(false));
            }

            return View(account);
        }

        /// <summary>
        /// Accounts #5
        /// GET: Workgroup/AccountEdit
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="accountId"> Workgroup Account Id</param>
        /// <returns></returns>
        public ActionResult EditAccount(int id, int accountId)
        {
            var account = _workgroupAccountRepository.GetNullableById(accountId);

            if (account == null)
            {
                ErrorMessage = "Account could not be found";
                return this.RedirectToAction(a => a.Index(false));
            }

            if(account.Workgroup.Id != id)
            {
                ErrorMessage = "Account not part of workgroup";
                return this.RedirectToAction(a => a.Index(false));
            }

            var viewModel = WorkgroupAccountModel.Create(Repository, account.Workgroup, account);
            return View(viewModel);
        }

        /// <summary>
        /// Accounts #6
        /// Post: Workgroup/AccountEdit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountId"> </param>
        /// <param name="workgroupAccount"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditAccount(int id, int accountId, WorkgroupAccount workgroupAccount)
        {
            var accountToEdit = _workgroupAccountRepository.GetNullableById(accountId);

            if (accountToEdit == null)
            {
                ErrorMessage = "Account could not be found";
                return this.RedirectToAction(a => a.Index(false));
            }

            if(accountToEdit.Workgroup.Id != id)
            {
                ErrorMessage = "Account not part of workgroup";
                return this.RedirectToAction(a => a.Index(false));
            }

            Mapper.Map(workgroupAccount, accountToEdit);

            ModelState.Clear();
            accountToEdit.TransferValidationMessagesTo(ModelState);

            if(_workgroupAccountRepository.Queryable.Any(a => a.Id != accountToEdit.Id &&  a.Workgroup.Id == accountToEdit.Workgroup.Id && a.Account.Id == accountToEdit.Account.Id ))
            {
                ModelState.AddModelError("WorkgroupAccount.Account", "Account already exists for this workgroup");
            }

            if (ModelState.IsValid)
            {
                _workgroupAccountRepository.EnsurePersistent(accountToEdit);
                Message = "Workgroup account has been updated.";
                //return RedirectToAction("Accounts", new { id = accountToEdit.Workgroup.Id });
                return this.RedirectToAction(a => a.Accounts(accountToEdit.Workgroup.Id));
            }

            var viewModel = WorkgroupAccountModel.Create(Repository, accountToEdit.Workgroup, accountToEdit);
            return View(viewModel);
        }

        /// <summary>
        /// Accounts #7
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="accountId">WorkgroupAccount Id</param>
        /// <returns></returns>
        public ActionResult AccountDelete(int id, int accountId)
        {
            var account = _workgroupAccountRepository.GetNullableById(accountId);

            if(account == null)
            {
                ErrorMessage = "Account could not be found";
                return this.RedirectToAction(a => a.Index(false));
            }

            if(account.Workgroup.Id != id)
            {
                ErrorMessage = "Account not part of workgroup";
                return this.RedirectToAction(a => a.Index(false));
            }

           return View(account);
        }


        /// <summary>
        /// Accounts #8
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="accountId">WorkgroupAccount Id</param>
        /// <param name="workgroupAccount"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AccountDelete(int id, int accountId, WorkgroupAccount workgroupAccount)
        {
            var accountToDelete = _workgroupAccountRepository.GetNullableById(accountId);

            if(accountToDelete == null)
            {
                ErrorMessage = "Account could not be found";
                return this.RedirectToAction(a => a.Index(false));
            }

            if(accountToDelete.Workgroup.Id != id)
            {
                ErrorMessage = "Account not part of workgroup";
                return this.RedirectToAction(a => a.Index(false));
            }

            var saveWorkgroupId = accountToDelete.Workgroup.Id;

            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction(a => a.Index(false));
            }

            _workgroupAccountRepository.Remove(accountToDelete);

            Message = "Account Removed from Workgroup";

            return this.RedirectToAction(a => a.Accounts(saveWorkgroupId));
        }

        #endregion

        #region Workgroup Vendors
        /// <summary>
        /// Vendors #1
        /// GET: Workgroup/Vendor/{workgroup id}
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public ActionResult VendorList(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction<WorkgroupController>(a => a.Index(false));
            }

            var workgroupVendorList = _workgroupVendorRepository.Queryable.Where(a => a.Workgroup == workgroup && a.IsActive);
            ViewBag.WorkgroupId = id;
            return View(workgroupVendorList.ToList());
        }

        /// <summary>
        /// Vendors #2
        /// GET: Workgroup/Vendor/Create/{workgroup id}
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public ActionResult CreateVendor(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction<WorkgroupController>(a => a.Index(false));
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Vendors may not be added to an administrative workgroup.";
                return this.RedirectToAction(a => a.Details(workgroup.Id));
            }

            var viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, new WorkgroupVendor() { Workgroup = workgroup });

            return View(viewModel);
        }

        /// <summary>
        /// Vendors #3
        /// POST: Workgroup/Vendor/Create/{workgroup id}
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="workgroupVendor">Workgroup Vendor Object</param>
        /// <param name="newVendor">New Vendor or KFS Existing Vendor</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateVendor(int id, WorkgroupVendor workgroupVendor, bool newVendor, bool skipAddress = false)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction<WorkgroupController>(a => a.Index(false));
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Vendors may not be added to an administrative workgroup.";
                return this.RedirectToAction(a => a.Details(workgroup.Id));
            }

            if (skipAddress)
            {
                workgroupVendor.Line1 = "N/A";
                workgroupVendor.City = "N/A";
                workgroupVendor.State = "CA";
                workgroupVendor.Zip = "95616";
                workgroupVendor.CountryCode = "US";
            }


            var workgroupVendorToCreate = new WorkgroupVendor();

            _workgroupService.TransferValues(workgroupVendor, ref workgroupVendorToCreate);

            workgroupVendorToCreate.Workgroup = workgroup;

            ModelState.Clear();
            workgroupVendorToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                if(!string.IsNullOrWhiteSpace(workgroupVendorToCreate.VendorId) && !string.IsNullOrWhiteSpace(workgroupVendorToCreate.VendorAddressTypeCode))
                {
                    if(_workgroupVendorRepository.Queryable
                        .Any(a => a.Workgroup.Id == id && 
                            a.VendorId == workgroupVendorToCreate.VendorId && 
                            a.VendorAddressTypeCode == workgroupVendorToCreate.VendorAddressTypeCode && 
                            a.IsActive))
                    {
                        Message = "KFS vendor has already been added";
                        return this.RedirectToAction(a => a.VendorList(id));
                    }
                    var inactiveKfsVendor = _workgroupVendorRepository.Queryable
                        .FirstOrDefault(a => a.Workgroup.Id == id && 
                            a.VendorId == workgroupVendorToCreate.VendorId && 
                            a.VendorAddressTypeCode == workgroupVendorToCreate.VendorAddressTypeCode && 
                            !a.IsActive);
                    if(inactiveKfsVendor != null)
                    {
                        inactiveKfsVendor.IsActive = true;
                        _workgroupVendorRepository.EnsurePersistent(inactiveKfsVendor);
                        Message = "KFS vendor added back. It was previously deleted from this workgroup.";
                        return this.RedirectToAction(a => a.VendorList(id));
                    }
                }
                _workgroupVendorRepository.EnsurePersistent(workgroupVendorToCreate);

                Message = "WorkgroupVendor Created Successfully";

                return this.RedirectToAction(a => a.VendorList(id));
            }

            WorkgroupVendorViewModel viewModel;

            if (!newVendor)
            {
                Vendor vendor = null;
                VendorAddress vendorAddress = null;
                if(!string.IsNullOrWhiteSpace(workgroupVendor.VendorId))
                {
                    vendor = _vendorRepository.GetNullableById(workgroupVendor.VendorId);
                    if(vendor != null)
                    {
                        vendorAddress = _vendorAddressRepository.Queryable.FirstOrDefault(a => a.Vendor == vendor && a.TypeCode == workgroupVendor.VendorAddressTypeCode);
                    }
                }
                viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, workgroupVendorToCreate, vendor, vendorAddress, newVendor);
            }
            else
            {
                viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, workgroupVendorToCreate, newVendor: true);
            }


            return View(viewModel);
        }

        /// <summary>
        /// Vendors #4
        /// GET: Workgroup/EditWorkgroupVendor/{workgroup vendor id}
        /// </summary>
        /// <remarks>Only allow editing of non-kfs workgroup vendors</remarks>
        /// <param name="id">Workgroup Vendor Id</param>
        /// <param name="workgroupVendorId"> </param>
        /// <returns></returns>
        public ActionResult EditWorkgroupVendor(int id, int workgroupVendorId)
        {
            var workgroupVendor = _workgroupVendorRepository.GetNullableById(workgroupVendorId);

            if (workgroupVendor == null)
            {
                ErrorMessage = "Workgroup Vendor not found.";
                return this.RedirectToAction(a => a.Index(false));
            }

            if(workgroupVendor.Workgroup.Id != id)
            {
                ErrorMessage = "WorkgroupVendor not part of workgroup";
                return this.RedirectToAction(a => a.Index(false));
            }

            if (!string.IsNullOrWhiteSpace(workgroupVendor.VendorId) && !string.IsNullOrWhiteSpace(workgroupVendor.VendorAddressTypeCode))
            {
                ErrorMessage = "Cannot edit KFS Vendors.  Please delete the vendor and add a new vendor.";
                return this.RedirectToAction(a => a.VendorList(workgroupVendor.Workgroup.Id));
            }

            var viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, workgroupVendor);
            return View(viewModel);
        }

        /// <summary>
        /// Vendors #5
        /// POST: Workgroup/EditWorkgroupVendor/{workgroup vendor id}
        /// </summary>
        /// <remarks>Only allow editing of non-kfs workgroup vendors</remarks>
        /// <param name="id"></param>
        /// <param name="workgroupVendorId"> </param>
        /// <param name="workgroupVendor"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditWorkgroupVendor(int id, int workgroupVendorId, WorkgroupVendor workgroupVendor, bool skipAddress = false)
        {
            var oldWorkgroupVendor = _workgroupVendorRepository.GetNullableById(workgroupVendorId);

            if(oldWorkgroupVendor == null)
            {
                ErrorMessage = "Workgroup Vendor not found.";
                return this.RedirectToAction(a => a.Index(false));
            }

            if(oldWorkgroupVendor.Workgroup.Id != id)
            {
                ErrorMessage = "WorkgroupVendor not part of workgroup";
                return this.RedirectToAction(a => a.Index(false));
            }

            if (!string.IsNullOrWhiteSpace(oldWorkgroupVendor.VendorId) && !string.IsNullOrWhiteSpace(oldWorkgroupVendor.VendorAddressTypeCode))
            {
                ErrorMessage = "Cannot edit KFS Vendors.  Please delete the vendor and add a new vendor.";
                return this.RedirectToAction(a => a.VendorList(oldWorkgroupVendor.Workgroup.Id));
            }

            if (skipAddress)
            {
                workgroupVendor.Line1 = "N/A";
                workgroupVendor.City = "N/A";
                workgroupVendor.State = "CA";
                workgroupVendor.Zip = "95616";
                workgroupVendor.CountryCode = "US";
            }

            Check.Require(string.IsNullOrWhiteSpace(workgroupVendor.VendorId), "Can't have VendorId when editing");
            Check.Require(string.IsNullOrWhiteSpace(workgroupVendor.VendorAddressTypeCode), "Can't have vendorAddresstypeCode when editing");

            var newWorkgroupVendor = new WorkgroupVendor();
            newWorkgroupVendor.Workgroup = oldWorkgroupVendor.Workgroup;

            _workgroupService.TransferValues(workgroupVendor, ref newWorkgroupVendor);
            ModelState.Clear();
            newWorkgroupVendor.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                oldWorkgroupVendor.IsActive = false;
                _workgroupVendorRepository.EnsurePersistent(oldWorkgroupVendor);
                _workgroupVendorRepository.EnsurePersistent(newWorkgroupVendor);

                Message = "WorkgroupVendor Edited Successfully";

                //return RedirectToAction("VendorList", new { Id = newWorkgroupVendor.Workgroup.Id });
                return this.RedirectToAction(a => a.VendorList(newWorkgroupVendor.Workgroup.Id));
            }

            var viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, newWorkgroupVendor);

            return View(viewModel);
        }

        /// <summary>
        /// Vendors #6
        /// GET: Workgroup/DeleteWorkgroupVendor/{workgroup vendor id}
        /// </summary>
        /// <param name="id">Workgroup Vendor Id</param>
        /// <param name="workgroupVendorId"> </param>
        /// <returns></returns>
        public ActionResult DeleteWorkgroupVendor(int id, int workgroupVendorId)
        {
            var workgroupVendor = _workgroupVendorRepository.GetNullableById(workgroupVendorId);

            if (workgroupVendor == null)
            {                
                return this.RedirectToAction(a => a.Index(false));
            }

            if(workgroupVendor.Workgroup.Id != id)
            {
                ErrorMessage = "WorkgroupVendor not part of workgroup";
                return this.RedirectToAction(a => a.Index(false));
            }

            return View(workgroupVendor);
        }

        /// <summary>
        /// Vendors #7
        /// POST: /WorkgroupVendor/Delete/5
        /// </summary>
        /// <param name="id">Workgroup Vendor Id</param>
        /// <param name="workgroupVendorId"> </param>
        /// <param name="workgroupVendor">ignored</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteWorkgroupVendor(int id, int workgroupVendorId, WorkgroupVendor workgroupVendor)
        {
            var workgroupVendorToDelete = _workgroupVendorRepository.GetNullableById(workgroupVendorId);

            if (workgroupVendorToDelete == null)
            {
                ErrorMessage = "WorkgroupVendor not found.";
                return this.RedirectToAction(a => a.Index(false));
            }

            if(workgroupVendorToDelete.Workgroup.Id != id)
            {
                ErrorMessage = "WorkgroupVendor not part of workgroup";
                return this.RedirectToAction(a => a.Index(false));
            }

            workgroupVendorToDelete.IsActive = false;

            _workgroupVendorRepository.EnsurePersistent(workgroupVendorToDelete);

            Message = "WorkgroupVendor Removed Successfully";

            return this.RedirectToAction(a => a.VendorList(workgroupVendorToDelete.Workgroup.Id));
        }

        /// <summary>
        /// Vendors #9 (39)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult BulkVendor(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction<WorkgroupController>(a => a.Index(false));
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Vendors may not be added to an administrative workgroup.";
                return this.RedirectToAction(a => a.Details(workgroup.Id));
            }
            return View(workgroup);
        }

        /// <summary>
        /// Vendors #10 (40)
        /// This action handles the form POST and the upload
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BulkVendor(int id, HttpPostedFileBase file)
        {
            // Verify that the user selected a file 

            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction<WorkgroupController>(a => a.Index(false));
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Vendors may not be added to an administrative workgroup.";
                return this.RedirectToAction(a => a.Details(workgroup.Id));
            }

            if (!file.FileName.EndsWith("xls"))
            {
                ErrorMessage = "Must be a valid Excel (.xls) file";
                return this.RedirectToAction(a => a.VendorList(id));
            }
            if (file != null && file.ContentLength > 0)
            {
                Stream uploadFileStream = file.InputStream;

                
                HSSFWorkbook wBook = new HSSFWorkbook(uploadFileStream);
                int successCount = 0;
                int failCount = 0;
                var sheet = wBook.GetSheetAt(0);
                for (int row = 0; row <= sheet.LastRowNum; row++)
                {
                    var workgroupVendorToCreate = new WorkgroupVendor();
                    if (sheet.GetRow(row).GetCell(0).StringCellValue == "Name")
                    {
                        continue;
                    }
                    workgroupVendorToCreate.Name =  Server.HtmlEncode(sheet.GetRow(row).GetCell(0).ToString());
                    workgroupVendorToCreate.Line1 = Server.HtmlEncode(sheet.GetRow(row).GetCell(1).ToString());
                    workgroupVendorToCreate.Line2 = Server.HtmlEncode(sheet.GetRow(row).GetCell(2) != null ? sheet.GetRow(row).GetCell(2).ToString() : null);
                    workgroupVendorToCreate.Line3 = Server.HtmlEncode(sheet.GetRow(row).GetCell(3) != null ? sheet.GetRow(row).GetCell(3).ToString() : null);
                    workgroupVendorToCreate.City = Server.HtmlEncode(sheet.GetRow(row).GetCell(4).StringCellValue);
                    workgroupVendorToCreate.State = Server.HtmlEncode(sheet.GetRow(row).GetCell(5).StringCellValue);
                    workgroupVendorToCreate.Zip = Server.HtmlEncode(sheet.GetRow(row).GetCell(6).ToString());
                    workgroupVendorToCreate.CountryCode = Server.HtmlEncode(sheet.GetRow(row).GetCell(7).ToString());
                    workgroupVendorToCreate.Phone = Server.HtmlEncode(sheet.GetRow(row).GetCell(8) != null ? sheet.GetRow(row).GetCell(8).ToString() : null);
                    workgroupVendorToCreate.Fax = Server.HtmlEncode(sheet.GetRow(row).GetCell(9) != null ? sheet.GetRow(row).GetCell(9).ToString() : null);
                    workgroupVendorToCreate.Email = Server.HtmlEncode(sheet.GetRow(row).GetCell(10) != null ? sheet.GetRow(row).GetCell(10).ToString() : null);
                    workgroupVendorToCreate.Url = Server.HtmlEncode(sheet.GetRow(row).GetCell(11) != null ? sheet.GetRow(row).GetCell(11).ToString() : null);
                    
                    workgroupVendorToCreate.Workgroup = workgroup;

                    ModelState.Clear();
                    workgroupVendorToCreate.TransferValidationMessagesTo(ModelState);

                    if (ModelState.IsValid)
                    {
                        _workgroupVendorRepository.EnsurePersistent(workgroupVendorToCreate);

                        successCount++;

                        //return this.RedirectToAction(a => a.VendorList(id));
                    }
                    else
                    {
                        failCount++;
                    }
                }

                Message = string.Format("Successfully added {0} vendor(s) to workgroup. {1} vendor(s) failed to load.", successCount, failCount);
                
                //{2} not added because of duplicated role.

            }
            // redirect back to the index action to show the form once again
            return this.RedirectToAction(a=>a.VendorList(workgroup.Id));
        }

       

        #endregion

        #region Workgroup Addresses
        /// <summary>
        /// Address #1
        /// </summary>
        /// <param name="id">Workgroup id</param>
        /// <returns></returns>
        public ActionResult Addresses(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index(false));
            }
            var viewModel = WorkgroupAddressListModel.Create(workgroup);
            return View(viewModel);
        }

        /// <summary>
        /// Address #2
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddAddress(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index(false));
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Addresses may not be added to an administrative workgroup.";
                return this.RedirectToAction(a => a.Details(workgroup.Id));
            }
            var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
            viewModel.WorkgroupAddress = new WorkgroupAddress();
            viewModel.WorkgroupAddress.Workgroup = workgroup;
            return View(viewModel);

        }

        /// <summary>
        /// Address #3
        /// </summary>
        /// <param name="id"></param>
        /// <param name="workgroupAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddAddress(int id, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index(false));
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Addresses may not be added to an administrative workgroup.";
                return this.RedirectToAction(a => a.Details(workgroup.Id));
            }
            workgroupAddress.Workgroup = workgroup;
            ModelState.Clear();
            workgroupAddress.TransferValidationMessagesTo(ModelState);
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Address not valid";
                var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
                viewModel.WorkgroupAddress = workgroupAddress;
                viewModel.WorkgroupAddress.Workgroup = workgroup;
                return View(viewModel);
            }
            var matchFound = 0;
            foreach (var address in workgroup.Addresses)
            {
                matchFound = _workgroupAddressService.CompareAddress(workgroupAddress, address);
                if (matchFound > 0)
                {
                    break;
                }
            }
            if (matchFound > 0)
            {
                var matchedAddress = workgroup.Addresses.Single(a => a.Id == matchFound);
                if (!matchedAddress.IsActive)
                {
                    Message = "Address created.";
                    matchedAddress.IsActive = true;
                    _workgroupRepository.EnsurePersistent(workgroup);
                }
                else
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

        /// <summary>
        /// Address #4
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="addressId">Address id</param>
        /// <returns></returns>
        public ActionResult DeleteAddress(int id, int addressId)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index(false));
            }
            var workgroupAddress = workgroup.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (workgroupAddress == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction((a => a.Addresses(id)));
            }
            if(workgroupAddress.Workgroup.Id != id)
            {
                ErrorMessage = "Address not part of this workgroup";
                return this.RedirectToAction(a => a.Index(false));
            }
            var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository);
            viewModel.WorkgroupAddress = workgroupAddress;
            viewModel.State = _stateRepository.GetNullableById(workgroupAddress.State);
            return View(viewModel);
        }

        /// <summary>
        /// Address #5
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="addressId">Address id</param>
        /// <param name="workgroupAddress">Dummy parm</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteAddress(int id, int addressId, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index(false));
            }
            var workgroupAddressToDelete = workgroup.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (workgroupAddressToDelete == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction((a => a.Addresses(id)));
            }
            if(workgroupAddressToDelete.Workgroup.Id != id)
            {
                ErrorMessage = "Address not part of this workgroup";
                return this.RedirectToAction(a => a.Index(false));
            }
            workgroupAddressToDelete.IsActive = false;
            _workgroupRepository.EnsurePersistent(workgroup);
            Message = "Address deleted.";
            return this.RedirectToAction(a => a.Addresses(id));
        }

        /// <summary>
        /// Address #6
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="addressId">Address id</param>
        /// <returns></returns>
        public ActionResult AddressDetails(int id, int addressId)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index(false));
            }
            var workgroupAddress = workgroup.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (workgroupAddress == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction((a => a.Addresses(id)));
            }
            if(workgroupAddress.Workgroup.Id != id)
            {
                ErrorMessage = "Address not part of this workgroup";
                return this.RedirectToAction(a => a.Index(false));
            }
            var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository);
            viewModel.WorkgroupAddress = workgroupAddress;
            viewModel.State = _stateRepository.GetNullableById(workgroupAddress.State);
            return View(viewModel);
        }

        /// <summary>
        /// Address #7
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="addressId">Address id</param>
        /// <returns></returns>
        public ActionResult EditAddress(int id, int addressId)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index(false));
            }
            var workgroupAddress = workgroup.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (workgroupAddress == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction((a => a.Addresses(id)));
            }
            if(workgroupAddress.Workgroup.Id != id)
            {
                ErrorMessage = "Address not part of this workgroup";
                return this.RedirectToAction(a => a.Index(false));
            }
            var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
            viewModel.WorkgroupAddress = workgroupAddress;
            return View(viewModel);
        }

        /// <summary>
        /// Address #8
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="addressId">Address id</param>
        /// <param name="workgroupAddress">address's new values</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditAddress(int id, int addressId, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(a => a.Index(false));
            }
            var workgroupAddressToEdit = workgroup.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (workgroupAddressToEdit == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction((a => a.Addresses(id)));
            }
            if(workgroupAddressToEdit.Workgroup.Id != id)
            {
                ErrorMessage = "Address not part of this workgroup";
                return this.RedirectToAction(a => a.Index(false));
            }
            workgroupAddress.Workgroup = workgroup;
            ModelState.Clear();
            workgroupAddress.TransferValidationMessagesTo(ModelState);
            if(!ModelState.IsValid)
            {
                ErrorMessage = "Unable to save due to errors.";
                var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
                viewModel.WorkgroupAddress = workgroupAddress;
                return View(viewModel);
            }

            if(_workgroupAddressService.CompareAddress(workgroupAddress, workgroup.Addresses.Where(a => a.Id == addressId).Single()) > 0)
            {
                Message = "No changes made";
                var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
                viewModel.WorkgroupAddress = workgroupAddress;
                return View(viewModel);
            }


            foreach (var activeAddress in workgroup.Addresses.Where(a => a.IsActive && a.Id != addressId))
            {
                var activeMatchFound = _workgroupAddressService.CompareAddress(workgroupAddress, activeAddress);
                if(activeMatchFound > 0)
                {
                    ErrorMessage = "The address you are changing this to already exists. Unable to save.";
                    var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
                    viewModel.WorkgroupAddress = workgroupAddress;
                    return View(viewModel);
                }
            }

            var matchFound = 0;
            foreach(var activeAddress in workgroup.Addresses.Where(a => !a.IsActive && a.Id != addressId))
            {
                matchFound = _workgroupAddressService.CompareAddress(workgroupAddress, activeAddress);
                if(matchFound > 0)
                {
                    break;
                }
            }

            if(matchFound > 0)
            {
                Message = "Address updated";
                workgroup.Addresses.Where(a => a.Id == matchFound).Single().IsActive = true;                
            }
            else
            {
                Message = "Address updated.";
                var newAddress = new WorkgroupAddress();
                Mapper.Map(workgroupAddress, newAddress);
                newAddress.Workgroup = workgroup;
                workgroup.AddAddress(newAddress);
            }
            workgroup.Addresses.Where(a => a.Id == addressId).Single().IsActive = false;

            _workgroupRepository.EnsurePersistent(workgroup);
            return this.RedirectToAction(a => a.Addresses(id));

        }
        #endregion

        #region Workgroup People

        /// <summary>
        /// People #1
        /// List of all people within a workgroup
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="roleFilter"></param>
        /// <returns></returns>
        public ActionResult People(int id, string roleFilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup==null)
            {
                ErrorMessage = "Workgroup not found";
                return this.RedirectToAction(a => a.Index(false));
            }

            var viewModel = WorgroupPeopleListModel.Create(_workgroupPermissionRepository, _roleRepository, workgroup, roleFilter);
            ViewBag.rolefilter = roleFilter;
            return View(viewModel);
        }

        /// <summary>
        /// People #2
        /// GET: add a person with a role into a workgroup
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="roleFilter"></param>
        /// <returns></returns>
        public ActionResult AddPeople(int id, string roleFilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup not found";
                return this.RedirectToAction(a => a.Index(false));
            }

            if(workgroup.Administrative && roleFilter == Role.Codes.Requester)
            {
                ErrorMessage = "Requester may not be added to an administrative workgroup.";
                return this.RedirectToAction(a => a.Details(workgroup.Id));
            }

            var viewModel = WorgroupPeopleCreateModel.Create(_roleRepository, workgroup);
            if(!string.IsNullOrWhiteSpace(roleFilter))
            {
                viewModel.Role = _repositoryFactory.RoleRepository.Queryable.SingleOrDefault(a => !a.IsAdmin && a.Id == roleFilter);
            }
            ViewBag.rolefilter = roleFilter;

            return View(viewModel);
        }

        /// <summary>
        /// People #3
        /// POST: add a person with a role into a workgroup
        /// </summary>
        /// <param name="id"></param>
        /// <param name="workgroupPeoplePostModel"></param>
        /// <param name="roleFilter"></param>
        /// <returns></returns>
       [HttpPost]
        public ActionResult AddPeople(int id, WorkgroupPeoplePostModel workgroupPeoplePostModel, string roleFilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup not found";
                return this.RedirectToAction(a => a.Index(false));
            }

            //Ensure role picked is valid.
            if (workgroupPeoplePostModel.Role != null)
            {
                if(!_repositoryFactory.RoleRepository.Queryable.Where(a => !a.IsAdmin && a.Id == workgroupPeoplePostModel.Role.Id).Any())
                {
                    ModelState.AddModelError("Role", "Invalid Role Selected");
                }
            }

           if(workgroup.Administrative && workgroupPeoplePostModel.Role.Id == Role.Codes.Requester)
            {
                ModelState.AddModelError("Role", "Administrative workgroups may not have Requesters");
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
            int duplicateCount = 0;
            //var notAddedSb = new StringBuilder();
            var notAddedKvp = new List<KeyValuePair<string, string>>();

            foreach (var u in workgroupPeoplePostModel.Users)
            {
                successCount = _workgroupService.TryToAddPeople(id, workgroupPeoplePostModel.Role, workgroup, successCount, u, ref failCount, ref duplicateCount, notAddedKvp);
            }

            Message = string.Format("Successfully added {0} people to workgroup as {1}. {2} not added because of duplicated role.", successCount,
                                    workgroupPeoplePostModel.Role.Name, duplicateCount);

            return this.RedirectToAction(a => a.People(id, workgroupPeoplePostModel.Role.Id));

        }

        /// <summary>
        /// People #4
        /// GET: remove a person/role from a workgroup
        /// </summary>
        /// <param name="id">Workgroup ID</param>
       /// <param name="workgroupPermissionId">Workgroup Permission ID</param>
        /// <param name="rolefilter"></param>
        /// <returns></returns>
        public ActionResult DeletePeople(int id, int workgroupPermissionId, string rolefilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup not found";
                return this.RedirectToAction(a => a.Index(false));
            }

            var workgroupPermission = _workgroupPermissionRepository.GetNullableById(workgroupPermissionId);
            if(workgroupPermission == null)
            {
                return this.RedirectToAction(a => a.People(id, rolefilter));
            }

            if(workgroupPermission.Workgroup != workgroup) //Need this because you might have DA access to a different workgroup 
            {
                Message = "Person does not belong to workgroup.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = WorkgroupPeopleDeleteModel.Create(_workgroupPermissionRepository, workgroupPermission);

            ViewBag.rolefilter = rolefilter;
            return View(viewModel);

        }

        /// <summary>
        /// People #5
        /// POST: remove a person/role from a workgroup
        /// </summary>
        /// <param name="id">Workgroup ID</param>
        /// <param name="workgroupPermissionId">Workgroup Permission ID</param>
        /// <param name="rolefilter"></param>
        /// <param name="workgroupPermission"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeletePeople(int id, int workgroupPermissionId, string rolefilter, WorkgroupPermission workgroupPermission, string[] roles)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup not found";
                return this.RedirectToAction(a => a.Index(false));
            }

            var workgroupPermissionToDelete = _workgroupPermissionRepository.GetNullableById(workgroupPermissionId);
            if (workgroupPermissionToDelete == null)
            {
                return this.RedirectToAction(a => a.People(id, rolefilter));
            }

            if(workgroupPermissionToDelete.Workgroup != workgroup) //Need this because you might have DA access to a different workgroup 
            {
                Message = "Person does not belong to workgroup.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var availableWorkgroupPermissions = _workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.User == workgroupPermissionToDelete.User && !a.Role.IsAdmin).ToList();
            if (availableWorkgroupPermissions.Count() == 1)
            {
                // invalid the cache for the user that was just given permissions
                //System.Web.HttpContext.Current.Cache.Remove(string.Format(Resources.Role_CacheId, workgroupPermissionToDelete.User.Id));
                _workgroupService.RemoveFromCache(workgroupPermissionToDelete);

                // TODO: Check for pending/open orders for this person. Set order to workgroup.
                _workgroupPermissionRepository.Remove(workgroupPermissionToDelete);
                Message = "Person successfully removed from role.";
                return this.RedirectToAction(a => a.People(id, rolefilter));
            }
            else
            {
                if (roles == null || roles.Count() == 0)
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
                    var wp = _workgroupPermissionRepository.Queryable.Single(a => a.Workgroup == workgroup && a.User == workgroupPermissionToDelete.User && a.Role.Id == role);

                    // invalid the cache for the user that was just given permissions
                    //System.Web.HttpContext.Current.Cache.Remove(string.Format(Resources.Role_CacheId, wp.User.Id));
                    _workgroupService.RemoveFromCache(wp);
                    
                    _workgroupPermissionRepository.Remove(wp);
                    removedCount++;
                }

                Message = string.Format("{0} {1} removed from {2}", removedCount, removedCount == 1 ? "role" : "roles", workgroupPermissionToDelete.User.FullName);
                return this.RedirectToAction(a => a.People(id, rolefilter));
            }


        }

        #endregion
        
        #region Ajax Helpers

        /// <summary>
        /// Vendors #8
        /// Ajax action for retrieving kfs vendor addresses
        /// </summary>
        /// <returns></returns>
        public JsonNetResult GetVendorAddresses(string vendorId)
        {
            var vendorAddresses = _vendorAddressRepository.Queryable.Where(a => a.Vendor.Id == vendorId).ToList();

            var results = vendorAddresses.Select(a => new { TypeCode = a.TypeCode, Name = a.DisplayName }).ToList();

            return new JsonNetResult(results);
        }

        /// <summary>
        /// Vendors #11 (41)
        /// </summary>
        /// <param name="workgrougpId"></param>
        /// <param name="name"></param>
        /// <param name="line1"></param>
        /// <returns></returns>
        public JsonNetResult CheckDuplicateVendor (int workgrougpId, string name, string line1)
        {
            var message = string.Empty;
            name = name.Trim();
            line1 = line1 !=null ?  line1.Trim() : string.Empty;
            if (_workgroupVendorRepository.Queryable.Any(
                    a => a.Workgroup.Id == workgrougpId && a.Name == name && a.Line1 == line1 && a.IsActive))
            {
                message = "It appears this vendor has already been added to this workgroup.";
            } 
            return new JsonNetResult(new {message});
        }

        ///// <summary>
        ///// TODO: Don't think this is being used. Remove when confirmed.
        ///// </summary>
        ///// <param name="searchTerm"></param>
        ///// <returns></returns>
        //public JsonNetResult SearchOrganizations(string searchTerm)
        //{
        //    var results = Repository.OfType<Organization>().Queryable.Where(a => a.Name.Contains(searchTerm) || a.Id.Contains(searchTerm)).Select(a => new IdAndName(a.Id, a.Name)).ToList();

        //    return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.DisplayNameAndId }));
        //}

        /// <summary>
        /// People #6
        /// Search Users in the User Table and LDAP lookup if none found.
        /// </summary>
        /// <param name="searchTerm">Email, LoginId, or FullName (for LDAP lookup)</param>
        /// <returns>List of matching users, up to 10 results</returns>
        public JsonNetResult SearchUsers(string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Trim();

            var users = _userRepository.Queryable.Where(a => a.Email == searchTerm || a.Id == searchTerm).ToList();
            if (users.Count == 0)
            {
                var ldapusers = _searchService.SearchUsers(searchTerm);

                foreach (var ldapuser in ldapusers.Where(x=>x.LoginId != null).Take(10))
                {
                    var user = new User(ldapuser.LoginId)
                                   {
                                       FirstName = ldapuser.FirstName,
                                       LastName = ldapuser.LastName
                                   };

                    users.Add(user);
                }
            }

            //We don't want to show users that are not active
            var results =
                users.Where(a => a.IsActive).Select(a => new IdAndName(a.Id, string.Format("{0} {1} ({2})", a.FirstName, a.LastName, a.Id))).ToList();
            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.Name }));
        }

        /// <summary>
        /// Vendors 12 
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
        /// Tested
        /// Returns all of requesters for a given workgroup
        /// </summary>
        /// <param name="id">Workgroup</param>
        /// <returns></returns>
        public JsonNetResult GetRequesters(int id)
        {
            var requestersForWorkgroup = (from wp in _workgroupPermissionRepository.Queryable
                                          where wp.Workgroup.Id == id
                                                && wp.User.IsActive
                                                && wp.Role.Id == Role.Codes.Requester
                                          select new {wp.User}).ToList();

            var requesterInfo = requestersForWorkgroup.Select(x => new {Name = x.User.FullNameAndId, x.User.Id});

            return new JsonNetResult(requesterInfo);
        }

        #endregion
    }
}
