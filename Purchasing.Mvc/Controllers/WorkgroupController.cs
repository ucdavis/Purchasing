using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Services;
using Purchasing.Mvc.Utility;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Helpers;
using Purchasing.Mvc.Models;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Helpers;
using IdAndName = Purchasing.Core.Services.IdAndName;
using Microsoft.AspNetCore.Http;
using Purchasing.Core.Services;
using System.Threading.Tasks;
using Serilog;
using Purchasing.Core.Helpers;
using Purchasing.Core.Models.AggieEnterprise;
using Purchasing.Mvc.Models.Finjector;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// Controller for the Workgroup class
    /// </summary>
    [Authorize(Policy = Role.Codes.DepartmentalAdmin)]
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
        private readonly IMapper _mapper;
        private readonly IAggieEnterpriseService _aggieEnterpriseService;

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
            IWorkgroupService workgroupService,
            IMapper mapper, 
            IAggieEnterpriseService aggieEnterpriseService)
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
            _mapper = mapper;
            _aggieEnterpriseService = aggieEnterpriseService;
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
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
            var workgroup = _workgroupRepository.Queryable.Fetch(x => x.Accounts).Single(x => x.Id == id);

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
                return this.RedirectToAction(nameof(ErrorController.Index), typeof(ErrorController).ControllerName());
            }

            var whatWasChanged = new WorkgroupChanged();
            whatWasChanged.AdminChanged = workgroup.Administrative != workgroupToEdit.Administrative;
            whatWasChanged.IsFullFeaturedChanged = workgroup.IsFullFeatured != workgroupToEdit.IsFullFeatured;
            whatWasChanged.IsActiveChanged = workgroup.IsActive != workgroupToEdit.IsActive;
            whatWasChanged.DoNotInheritPermissionsChanged = workgroup.DoNotInheritPermissions != workgroupToEdit.DoNotInheritPermissions;
            whatWasChanged.OriginalSubOrgIds = workgroupToEdit.Organizations.Select(a => a.Id).ToList();


            _mapper.Map(workgroup, workgroupToEdit);
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
                if (Repository.OfType<ConditionalApproval>().Queryable.Any(a => a.Workgroup.Id == workgroupToEdit.Id))
                {
                    ModelState.AddModelError("Workgroup.Administrative", "A workgroup can't be made administrative if there are any conditional Approvals.");
                }
            }

            if(workgroupToEdit.Administrative && workgroupToEdit.SyncAccounts)
            {
                ModelState.AddModelError("Workgroup.Administrative", "Can not have both Administrative and Sync Accounts selected.");
            }

            if (workgroupToEdit.Administrative && workgroupToEdit.DoNotInheritPermissions)
            {
                ModelState.AddModelError("Workgroup.DoNotInheritPermissions", "Can not have both Administrative and Do Not Inherit Permissions selected.");
            }

            if (workgroup.IsFullFeatured && !workgroup.Administrative)
            {
                ModelState.AddModelError("Workgroup.Administrative", "If Full Featured, workgroup must be administrative.");
            }

            //TODO: Test this.
            if(!ModelState.IsValid)
            {
                //Moved here because if you just pass workgroup, it doesn't have any selected organizations.
                return View(WorkgroupModifyModel.Create(GetCurrentUser(), _queryRepositoryFactory, workgroupToEdit));
            }

            _workgroupRepository.EnsurePersistent(workgroupToEdit);

            _workgroupService.UpdateRelatedPermissions(workgroupToEdit, whatWasChanged);

            Message = string.Format("{0} was modified successfully",
                                    workgroup.Name);

            return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });

        }

        public class WorkgroupChanged
        {
            public bool IsActiveChanged { get; set; }
            public bool AdminChanged { get; set; }
            public bool IsFullFeaturedChanged { get; set; }
            public bool OrganizationsChanged { get; set; }
            public bool DoNotInheritPermissionsChanged { get; set; }
            public List<string> OriginalSubOrgIds { get; set; }
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            workgroupToDelete.IsActive = false;

            _workgroupRepository.EnsurePersistent(workgroupToDelete);

            Message = string.Format("{0} was disabled successfully",
                                    workgroup.Name);

            return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });

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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Account may not be added to an administrative workgroup.";
                return this.RedirectToAction(nameof(Details), new { id = id });
            }

            if(workgroup.SyncAccounts)
            {
                ErrorMessage = "Accounts should not be added when Synchronize Accounts is selected because they my be overwritten.";
                return this.RedirectToAction(nameof(Details), new { id = id });
            }

            var viewModel = WorkgroupAccountModel.Create(Repository, workgroup); //TODO: Remove accounts and just have CCOA?

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
        public async Task<ActionResult> AddAccount(int id, WorkgroupAccount workgroupAccount, string account_search)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Accounts may not be added to an administrative workgroup.";
                return this.RedirectToAction(nameof(Details), new { id = id });
            }

            var workgroupAccounts = _workgroupAccountRepository.Queryable.Where(a => a.Workgroup.Id == id).ToArray(); //All workgroup accounts


            var workgroupAccountToCreate = new WorkgroupAccount() {Workgroup = workgroup};
            //_mapper.Map(workgroupAccount, workgroupAccountToCreate); //Mapper was causing me an exception JCS
            workgroupAccountToCreate.Account                = workgroupAccount.Account;
            workgroupAccountToCreate.AccountManager         = workgroupAccount.AccountManager;
            workgroupAccountToCreate.Approver               = workgroupAccount.Approver;
            workgroupAccountToCreate.Purchaser              = workgroupAccount.Purchaser;
            workgroupAccountToCreate.Name                   = workgroupAccount.Name?.Trim();
            workgroupAccountToCreate.FinancialSegmentString = workgroupAccount.FinancialSegmentString?.Trim()?.ToUpper();


            ModelState.Clear();
            //workgroupAccountToCreate.TransferValidationMessagesTo(ModelState);

            

            if(workgroupAccountToCreate.Account == null)
            {
                if (!string.IsNullOrWhiteSpace(account_search))
                {
                    workgroupAccountToCreate.Account = _repositoryFactory.AccountRepository.GetNullableById(account_search);
                }
            }

            if(workgroupAccountToCreate.Account == null)
            {
                //ModelState.AddModelError("WorkgroupAccount.Account", "Account not found");
                // Ok, not required
            }
            else
            {
                if (workgroupAccounts.Any(a => a.Account != null && a.Account.Id == workgroupAccountToCreate.Account.Id))
                {
                    ModelState.AddModelError("WorkgroupAccount.Account", "Account already exists for this workgroup");
                }
            }

            if (string.IsNullOrWhiteSpace(workgroupAccountToCreate.FinancialSegmentString))
            {
                ModelState.AddModelError("WorkgroupAccount.FinancialSegmentString", "CoA is required");
            }
            else
            {
                var accountValid = await _aggieEnterpriseService.ValidateAccount(workgroupAccountToCreate.FinancialSegmentString);
                if (!accountValid.IsValid)
                {
                    ModelState.AddModelError("WorkgroupAccount.FinancialSegmentString", accountValid.Message);
                }
                if (workgroupAccounts.Any(a => a.FinancialSegmentString != null && a.FinancialSegmentString.Equals(workgroupAccountToCreate.FinancialSegmentString, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("WorkgroupAccount.FinancialSegmentString", "CoA already exists for this workgroup");
                }
            }
            if (string.IsNullOrWhiteSpace(workgroupAccountToCreate.Name))
            {
                ModelState.AddModelError("WorkgroupAccount.Name", "Name is required");
            }
            else
            {
                if (workgroupAccounts.Any(a => a.Name != null && a.Name.Equals(workgroupAccountToCreate.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("WorkgroupAccount.Name", "Name already exists for this workgroup");
                }
            }


            if (ModelState.IsValid)
            {
                workgroupAccountToCreate.TransferValidationMessagesTo(ModelState);
            }

            if (ModelState.IsValid)
            {
                Message = "Workgroup account saved.";

                _workgroupAccountRepository.EnsurePersistent(workgroupAccountToCreate);

                //return this.RedirectToAction("Accounts", new {id = id});
                return this.RedirectToAction(nameof(AccountDetails), new { id = id, accountId = workgroupAccountToCreate.Id });
            }

            var viewModel = WorkgroupAccountModel.Create(Repository, workgroup, workgroupAccountToCreate);
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult UpdateAccountsFromFinjector(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if (workgroup.Administrative)
            {
                ErrorMessage = "Accounts may not be added to an administrative workgroup.";
                return this.RedirectToAction(nameof(Details), new { id = id });
            }

            return View(workgroup);
        }

        [HttpPost]
        public ActionResult UpdateAccountsFromFinjector(int id, string chartsString, bool onlyAdd = false)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if (workgroup.Administrative)
            {
                ErrorMessage = "Accounts may not be added to an administrative workgroup.";
                return this.RedirectToAction(nameof(Details), new { id = id });
            }


            //chartsString is a stringified json object. deserialized it into a list of WorkgroupFinjectorChart
            var charts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WorkgroupFinjectorChart>>(chartsString);

            if(charts == null || charts.Count == 0)
            {
                ErrorMessage = "No accounts were selected from Finjector.";
                return this.RedirectToAction(nameof(Details), new { id = id });
            }

            var defaults = _workgroupPermissionRepository.Queryable.Where(a => a.Workgroup.Id == id && a.IsAdmin == false && a.IsDefaultForAccount).Fetch(a => a.User).ToList();

            //Find all workgroup accounts that are not in the model
            var workgroupAccounts = _workgroupAccountRepository.Queryable.Where(a => a.Workgroup.Id == id).ToArray(); //All workgroup accounts
            var workgroupAccountsToDelete = workgroupAccounts.Where(a => !charts.Any(b => b.ChartString == a.FinancialSegmentString)).ToArray();
            var workgroupAccountsToUpdate = workgroupAccounts.Where(a => charts.Any(b => b.ChartString == a.FinancialSegmentString)).ToArray();
            var workgroupAccountsToAdd = charts.Where(a => !workgroupAccounts.Any(b => b.FinancialSegmentString == a.ChartString)).ToArray();

            var deleteCount = 0;
            var updateCount = 0;
            var addCount = 0;

            //Get a count of the number of items in the model that have unique chart strings
            var uniqueChartStrings = charts.Select(a => a.ChartString).Distinct().Count();

            if (onlyAdd == false)
            {
                //Delete
                foreach (var workgroupAccount in workgroupAccountsToDelete)
                {
                    _workgroupAccountRepository.Remove(workgroupAccount);
                    deleteCount++;
                }
            }
            //Update
            foreach (var workgroupAccount in workgroupAccountsToUpdate)
            {
                var modelAccount = charts.First(a => a.ChartString == workgroupAccount.FinancialSegmentString);
                if (modelAccount.Name.SafeTruncate(64) != workgroupAccount.Name)
                {
                    workgroupAccount.Name = modelAccount.Name.SafeTruncate(64); //64 characters
                    workgroupAccount.FinancialSegmentString = modelAccount.ChartString;
                    _workgroupAccountRepository.EnsurePersistent(workgroupAccount);
                    updateCount++;
                }
            }
            //Add
            foreach (var workgroupAccount in workgroupAccountsToAdd)
            {
                var newWorkgroupAccount = new WorkgroupAccount()
                {
                    Workgroup = workgroup,
                    Name = workgroupAccount.Name,
                    FinancialSegmentString = workgroupAccount.ChartString,
                    AccountManager = defaults.SingleOrDefault(a => a.Role.Id == Role.Codes.AccountManager)?.User,
                    Approver = defaults.SingleOrDefault(a => a.Role.Id == Role.Codes.Approver)?.User,
                    Purchaser = defaults.SingleOrDefault(a => a.Role.Id == Role.Codes.Purchaser)?.User
                };
                _workgroupAccountRepository.EnsurePersistent(newWorkgroupAccount);
                addCount++;
            }


            Message = $"{uniqueChartStrings} unique chart strings were found in the import from Finjector. {deleteCount} accounts were deleted. {updateCount} accounts were updated. {addCount} accounts were added.";
            

            return this.RedirectToAction(nameof(Accounts), new { id = id });
        }

        /// <summary>
        /// Accounts #4
        /// GET: Workgroup/AccountDetails
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="accountId">Workgroup Account Id</param>
        /// <returns></returns>
        public async Task<ActionResult> AccountDetails(int id, int accountId)
        {
            var model = new WorkgroupAccountDetails();
            model.AccountValidationModel = new AccountValidationModel();
            
            var account = _workgroupAccountRepository.GetNullableById(accountId);

            if (account == null)
            {
                ErrorMessage = "Account could not be found";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if(account.Workgroup.Id != id)
            {
                ErrorMessage = "Account not part of workgroup";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            model.WorkgroupAccount = account;
            if (!string.IsNullOrWhiteSpace(account.FinancialSegmentString))
            {
                model.AccountValidationModel = await _aggieEnterpriseService.ValidateAccount(account.FinancialSegmentString);
            }

            return View(model);
        }

        /// <summary>
        /// Accounts #5
        /// GET: Workgroup/AccountEdit
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="accountId"> Workgroup Account Id</param>
        /// <returns></returns>
        public async Task<ActionResult> EditAccount(int id, int accountId)
        {
            var account = _workgroupAccountRepository.GetNullableById(accountId);

            if (account == null)
            {
                ErrorMessage = "Account could not be found";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if(account.Workgroup.Id != id)
            {
                ErrorMessage = "Account not part of workgroup";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            var viewModel = WorkgroupAccountModel.Create(Repository, account.Workgroup, account);
            if (string.IsNullOrWhiteSpace(viewModel.WorkgroupAccount.Name))
            {
                viewModel.WorkgroupAccount.Name = $"{viewModel.WorkgroupAccount.GetName} [{viewModel.WorkgroupAccount.GetAccount}]".SafeTruncate(64);
            }
            
            if (string.IsNullOrWhiteSpace(viewModel.WorkgroupAccount.FinancialSegmentString))
            {
                //Lookup.
                try
                {
                    viewModel.WorkgroupAccount.FinancialSegmentString = await _aggieEnterpriseService.ConvertKfsAccount(viewModel.WorkgroupAccount.Account);
                    Message = "CoA was defaulted from KFS. Please review, edit, and save.";
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error getting financial segment string from Aggie Enterprise Service");
                }
            }
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
        public async Task<ActionResult> EditAccount(int id, int accountId, WorkgroupAccount workgroupAccount)
        {
            var accountToEdit = _workgroupAccountRepository.GetNullableById(accountId);

            if (accountToEdit == null)
            {
                ErrorMessage = "Account could not be found";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if(accountToEdit.Workgroup.Id != id)
            {
                ErrorMessage = "Account not part of workgroup";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            //accountToEdit.Account = workgroupAccount.Account;
            accountToEdit.AccountManager = workgroupAccount.AccountManager;
            accountToEdit.Approver = workgroupAccount.Approver;
            accountToEdit.Purchaser = workgroupAccount.Purchaser;
            accountToEdit.Name = workgroupAccount.Name?.Trim();
            accountToEdit.FinancialSegmentString = workgroupAccount.FinancialSegmentString?.Trim()?.ToUpper();

            // _mapper.Map(workgroupAccount, accountToEdit); //I was getting an exception on test, planet express workgroup when using the mapper. JCS

            ModelState.Clear();
            accountToEdit.TransferValidationMessagesTo(ModelState);


            var workgroupAccounts = _workgroupAccountRepository.Queryable.Where(a => a.Workgroup.Id == id && a.Id != accountToEdit.Id).ToArray(); //All wg accounts except the one being edited

            //If we need to support both this and AE, we will need a flag here
            //if(_workgroupAccountRepository.Queryable.Any(a => a.Id != accountToEdit.Id &&  a.Workgroup.Id == accountToEdit.Workgroup.Id && a.Account.Id == accountToEdit.Account.Id ))
            //{
            //    ModelState.AddModelError("WorkgroupAccount.Account", "Account already exists for this workgroup");
            //}

            if (string.IsNullOrWhiteSpace(accountToEdit.FinancialSegmentString))
            {
                ModelState.AddModelError("WorkgroupAccount.FinancialSegmentString", "CoA is required");
            }
            else
            {
                var accountValid = await _aggieEnterpriseService.ValidateAccount(accountToEdit.FinancialSegmentString);
                if (!accountValid.IsValid)
                {
                    ModelState.AddModelError("WorkgroupAccount.FinancialSegmentString", accountValid.Message);
                }
                if (workgroupAccounts.Any(a => a.FinancialSegmentString != null && a.FinancialSegmentString.Equals(accountToEdit.FinancialSegmentString, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("WorkgroupAccount.FinancialSegmentString", "CoA already exists for this workgroup");
                }
            }
            if (string.IsNullOrWhiteSpace(accountToEdit.Name))
            {
                ModelState.AddModelError("WorkgroupAccount.Name", "Name is required");
            }
            else
            {
                if (workgroupAccounts.Any(a => a.Name != null && a.Name.Equals(accountToEdit.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("WorkgroupAccount.Name", "Name already exists for this workgroup");
                }
            }

            

            if (accountToEdit.Account != null && workgroupAccounts.Any(a => a.Account != null && a.Account.Id == accountToEdit.Account.Id))
            {
                ModelState.AddModelError("WorkgroupAccount.Account", "Account already exists for this workgroup");
            }



            if (ModelState.IsValid)
            {
                Message = "Workgroup account has been updated.";

                _workgroupAccountRepository.EnsurePersistent(accountToEdit);

                //return RedirectToAction("Accounts", new { id = accountToEdit.Workgroup.Id });
                return this.RedirectToAction(nameof(AccountDetails), new { id = accountToEdit.Workgroup.Id, accountId = accountToEdit.Id });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if(account.Workgroup.Id != id)
            {
                ErrorMessage = "Account not part of workgroup";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if (account.Workgroup.SyncAccounts)
            {
                ErrorMessage =
                    "Accounts should not be deleted from Workgroups when Synchronize Accounts is selected because they will be added back overnight.";
                return this.RedirectToAction(nameof(Accounts), new { id = account.Workgroup.Id });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if(accountToDelete.Workgroup.Id != id)
            {
                ErrorMessage = "Account not part of workgroup";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if (accountToDelete.Workgroup.SyncAccounts)
            {
                ErrorMessage =
                    "Accounts should not be deleted from Workgroups when Synchronize Accounts is selected because they will be added back overnight.";
                return this.RedirectToAction(nameof(Accounts), new { id = accountToDelete.Workgroup.Id });
            }

            var saveWorkgroupId = accountToDelete.Workgroup.Id;

            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            _workgroupAccountRepository.Remove(accountToDelete);

            Message = "Account Removed from Workgroup";

            return this.RedirectToAction(nameof(Accounts), new { id = saveWorkgroupId });
        }

        /// <summary>
        /// Accounts #9 (43)
        /// </summary>
        /// <param name="id">Workgroup id</param>
        /// <returns></returns>
        public ActionResult UpdateMultipleAccounts(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            var viewModel = UpdateMultipleAccountsViewModel.Create(workgroup);
            var approver = _workgroupPermissionRepository.Queryable.FirstOrDefault(a => a.Workgroup == workgroup && a.Role.Id == Role.Codes.Approver && a.IsDefaultForAccount);
            if (approver != null)
            {
                viewModel.SelectedApprover = approver.User.Id;
                viewModel.DefaultSelectedApprover = true;
            }

            var accountManager = _workgroupPermissionRepository.Queryable.FirstOrDefault(a => a.Workgroup == workgroup && a.Role.Id == Role.Codes.AccountManager && a.IsDefaultForAccount);
            if (accountManager != null)
            {
                viewModel.SelectedAccountManager = accountManager.User.Id;
                viewModel.DefaultSelectedAccountManager = true;
            }

            var purchaser = _workgroupPermissionRepository.Queryable.FirstOrDefault(a => a.Workgroup == workgroup && a.Role.Id == Role.Codes.Purchaser && a.IsDefaultForAccount);
            if (purchaser != null)
            {
                viewModel.SelectedPurchaser = purchaser.User.Id;
                viewModel.DefaultSelectedPurchaser = true;
            }

            viewModel.ApproverChoices.AddRange(_workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.Role.Id == Role.Codes.Approver && !a.IsAdmin).OrderBy(a => a.User.LastName).ToList().Select(b => new Tuple<string, string>(b.User.Id, b.User.FullNameAndIdLastFirst)).ToList());
            viewModel.AccountManagerChoices.AddRange(_workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.Role.Id == Role.Codes.AccountManager && !a.IsAdmin).OrderBy(a => a.User.LastName).ToList().Select(b => new Tuple<string, string>(b.User.Id, b.User.FullNameAndIdLastFirst)).ToList());
            viewModel.PurchaserChoices.AddRange(_workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.Role.Id == Role.Codes.Purchaser && !a.IsAdmin).OrderBy(a => a.User.LastName).ToList().Select(b => new Tuple<string, string>(b.User.Id, b.User.FullNameAndIdLastFirst)).ToList());


            return View(viewModel);
        }

        /// <summary>
        /// Account #10 (44)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateMultipleAccountsViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateMultipleAccounts(int id, UpdateMultipleAccountsViewModel updateMultipleAccountsViewModel)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if (updateMultipleAccountsViewModel.DefaultSelectedApprover)
            {
                if (updateMultipleAccountsViewModel.SelectedApprover == "DO_NOT_UPDATE" || updateMultipleAccountsViewModel.SelectedApprover == "CLEAR_ALL")
                {
                    ModelState.AddModelError("DefaultSelectedApprover", "If you select a Default for new Account, it must be a Person, not Do Not Update or Clear All");
                }
            }

            if (updateMultipleAccountsViewModel.DefaultSelectedAccountManager)
            {
                if (updateMultipleAccountsViewModel.SelectedAccountManager == "DO_NOT_UPDATE" || updateMultipleAccountsViewModel.SelectedAccountManager == "CLEAR_ALL")
                {
                    ModelState.AddModelError("DefaultSelectedAccountManager", "If you select a Default for new Account, it must be a Person, not Do Not Update or Clear All");
                }
            }

            if (updateMultipleAccountsViewModel.DefaultSelectedPurchaser)
            {
                if (updateMultipleAccountsViewModel.SelectedPurchaser == "DO_NOT_UPDATE" || updateMultipleAccountsViewModel.SelectedPurchaser == "CLEAR_ALL")
                {
                    ModelState.AddModelError("DefaultSelectedPurchaser", "If you select a Default for new Account, it must be a Person, not Do Not Update or Clear All");
                }
            }


            if (!ModelState.IsValid)
            {
                var viewModel = UpdateMultipleAccountsViewModel.Create(workgroup);

                viewModel.SelectedApprover = updateMultipleAccountsViewModel.SelectedApprover;
                viewModel.DefaultSelectedApprover = updateMultipleAccountsViewModel.DefaultSelectedApprover;

                viewModel.SelectedAccountManager = updateMultipleAccountsViewModel.SelectedAccountManager;
                viewModel.DefaultSelectedAccountManager = updateMultipleAccountsViewModel.DefaultSelectedAccountManager;

                viewModel.SelectedPurchaser = updateMultipleAccountsViewModel.SelectedPurchaser;
                viewModel.DefaultSelectedPurchaser = updateMultipleAccountsViewModel.DefaultSelectedPurchaser;
       

                viewModel.ApproverChoices.AddRange(_workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.Role.Id == Role.Codes.Approver && !a.IsAdmin).OrderBy(a => a.User.LastName).ToList().Select(b => new Tuple<string, string>(b.User.Id, b.User.FullNameAndIdLastFirst)).ToList());
                viewModel.AccountManagerChoices.AddRange(_workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.Role.Id == Role.Codes.AccountManager && !a.IsAdmin).OrderBy(a => a.User.LastName).ToList().Select(b => new Tuple<string, string>(b.User.Id, b.User.FullNameAndIdLastFirst)).ToList());
                viewModel.PurchaserChoices.AddRange(_workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.Role.Id == Role.Codes.Purchaser && !a.IsAdmin).OrderBy(a => a.User.LastName).ToList().Select(b => new Tuple<string, string>(b.User.Id, b.User.FullNameAndIdLastFirst)).ToList());


                return View(viewModel);
            }

            
            _workgroupService.UpdateDefaultAccountApprover(workgroup, updateMultipleAccountsViewModel.DefaultSelectedApprover, updateMultipleAccountsViewModel.SelectedApprover, Role.Codes.Approver);
            _workgroupService.UpdateDefaultAccountApprover(workgroup, updateMultipleAccountsViewModel.DefaultSelectedAccountManager, updateMultipleAccountsViewModel.SelectedAccountManager, Role.Codes.AccountManager);
            _workgroupService.UpdateDefaultAccountApprover(workgroup, updateMultipleAccountsViewModel.DefaultSelectedPurchaser, updateMultipleAccountsViewModel.SelectedPurchaser, Role.Codes.Purchaser);


            Message = "Values Updated";


            return this.RedirectToAction(nameof(UpdateMultipleAccounts), new { id = id });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), new { showAll = false });
            }

            var workgroupVendorList = _workgroupVendorRepository.Queryable.Where(a => a.Workgroup == workgroup && a.IsActive);
            ViewBag.WorkgroupId = id;
            return View(workgroupVendorList.ToList());
        }

        /// <summary>
        /// Vendors #??
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ExportableVendorList(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ViewBag.WorkgroupName = string.Empty;
            }
            else
            {
                ViewBag.WorkgroupName = workgroup.Name;
            }
            return VendorList(id);
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), new { showAll = false });
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Vendors may not be added to an administrative workgroup.";
                return this.RedirectToAction(nameof(Details), new { id = id });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), new { showAll = false });
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Vendors may not be added to an administrative workgroup.";
                return this.RedirectToAction(nameof(Details), new { id = id });
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

            if (ModelState.ContainsKey("WorkgroupVendor.Email"))
            {
                //Message = "Vendor Email is invalid";
                workgroupVendorToCreate.Email = null;
                ModelState.Clear();
                workgroupVendorToCreate.TransferValidationMessagesTo(ModelState);
            }

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
                        return this.RedirectToAction(nameof(VendorList), new { id = id });
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
                        return this.RedirectToAction(nameof(VendorList), new { id = id });
                    }
                }
                if(!string.IsNullOrWhiteSpace(workgroupVendorToCreate.AeSupplierNumber) && !string.IsNullOrWhiteSpace(workgroupVendorToCreate.AeSupplierSiteCode))
                {
                    if (_workgroupVendorRepository.Queryable
                        .Any(a => a.Workgroup.Id == id &&
                            a.AeSupplierNumber == workgroupVendorToCreate.AeSupplierNumber &&
                            a.AeSupplierSiteCode == workgroupVendorToCreate.AeSupplierSiteCode &&
                            a.IsActive))
                    {
                        Message = "Campus vendor has already been added";
                        return this.RedirectToAction(nameof(VendorList), new { id = id });
                    }
                    //Possibly do a check to see if it is still valid/active
                    var inactiveAeVendor = _workgroupVendorRepository.Queryable
                        .FirstOrDefault(a => a.Workgroup.Id == id &&
                            a.AeSupplierNumber == workgroupVendorToCreate.AeSupplierNumber &&
                            a.AeSupplierSiteCode == workgroupVendorToCreate.AeSupplierSiteCode &&
                            !a.IsActive);
                    if(inactiveAeVendor != null)
                    {
                        inactiveAeVendor.IsActive = true;
                        _workgroupVendorRepository.EnsurePersistent(inactiveAeVendor);
                        Message = "Aggie Enterprise vendor added back. It was previously deleted from this workgroup.";
                        return this.RedirectToAction(nameof(VendorList), new { id = id });
                    }

                }
                _workgroupVendorRepository.EnsurePersistent(workgroupVendorToCreate);

                Message = "WorkgroupVendor Created Successfully";

                return this.RedirectToAction(nameof(VendorList), new { id = id });
            }

            ErrorMessage = "Unable to Add Vendor";

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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if(workgroupVendor.Workgroup.Id != id)
            {
                ErrorMessage = "WorkgroupVendor not part of workgroup";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if (!string.IsNullOrWhiteSpace(workgroupVendor.VendorId) && !string.IsNullOrWhiteSpace(workgroupVendor.VendorAddressTypeCode))
            {
                ErrorMessage = "Cannot edit KFS Vendors.  Please delete the vendor and add a new vendor.";
                return this.RedirectToAction(nameof(VendorList), new { id = workgroupVendor.Workgroup.Id });
            }

            if (!string.IsNullOrWhiteSpace(workgroupVendor.AeSupplierNumber) && !string.IsNullOrWhiteSpace(workgroupVendor.AeSupplierSiteCode))
            {
                ErrorMessage = "Cannot edit Aggie Enterprise Vendors (Suppliers).  Please delete the vendor and add a new vendor.";
                return this.RedirectToAction(nameof(VendorList), new { id = workgroupVendor.Workgroup.Id });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if(oldWorkgroupVendor.Workgroup.Id != id)
            {
                ErrorMessage = "WorkgroupVendor not part of workgroup";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if (!string.IsNullOrWhiteSpace(oldWorkgroupVendor.VendorId) && !string.IsNullOrWhiteSpace(oldWorkgroupVendor.VendorAddressTypeCode))
            {
                ErrorMessage = "Cannot edit KFS Vendors.  Please delete the vendor and add a new vendor.";
                return this.RedirectToAction(nameof(VendorList), new { id = oldWorkgroupVendor.Workgroup.Id });
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
                return this.RedirectToAction(nameof(VendorList), new { id = newWorkgroupVendor.Workgroup.Id });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if(workgroupVendor.Workgroup.Id != id)
            {
                ErrorMessage = "WorkgroupVendor not part of workgroup";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if(workgroupVendorToDelete.Workgroup.Id != id)
            {
                ErrorMessage = "WorkgroupVendor not part of workgroup";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            workgroupVendorToDelete.IsActive = false;

            _workgroupVendorRepository.EnsurePersistent(workgroupVendorToDelete);

            Message = "WorkgroupVendor Removed Successfully";

            return this.RedirectToAction(nameof(VendorList), new { id = workgroupVendorToDelete.Workgroup.Id });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), new { showAll = false });
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Vendors may not be added to an administrative workgroup.";
                return this.RedirectToAction(nameof(Details), new { id = workgroup.Id });
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
        public ActionResult BulkVendor(int id, IFormFile file)
        {
            // Verify that the user selected a file 

            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(nameof(WorkgroupController.Index), new { showAll = false });
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Vendors may not be added to an administrative workgroup.";
                return this.RedirectToAction(nameof(Details), new { id = workgroup.Id });
            }

            if (!file.FileName.EndsWith("xls"))
            {
                ErrorMessage = "Must be a valid Excel (.xls) file";
                return this.RedirectToAction(nameof(VendorList), new { id = id });
            }
            if (file != null && file.Length > 0)
            {
                Stream uploadFileStream = file.OpenReadStream();

                
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
                    workgroupVendorToCreate.Name =  HtmlEncoder.Default.Encode(sheet.GetRow(row).GetCell(0).ToString());
                    workgroupVendorToCreate.Line1 = HtmlEncoder.Default.Encode(sheet.GetRow(row).GetCell(1).ToString());
                    workgroupVendorToCreate.Line2 = HtmlEncoder.Default.Encode(sheet.GetRow(row).GetCell(2) != null ? sheet.GetRow(row).GetCell(2).ToString() : null);
                    workgroupVendorToCreate.Line3 = HtmlEncoder.Default.Encode(sheet.GetRow(row).GetCell(3) != null ? sheet.GetRow(row).GetCell(3).ToString() : null);
                    workgroupVendorToCreate.City = HtmlEncoder.Default.Encode(sheet.GetRow(row).GetCell(4).StringCellValue);
                    workgroupVendorToCreate.State = HtmlEncoder.Default.Encode(sheet.GetRow(row).GetCell(5).StringCellValue);
                    workgroupVendorToCreate.Zip = HtmlEncoder.Default.Encode(sheet.GetRow(row).GetCell(6).ToString());
                    workgroupVendorToCreate.CountryCode = HtmlEncoder.Default.Encode(sheet.GetRow(row).GetCell(7).ToString());
                    workgroupVendorToCreate.Phone = HtmlEncoder.Default.Encode(sheet.GetRow(row).GetCell(8) != null ? sheet.GetRow(row).GetCell(8).ToString() : null);
                    workgroupVendorToCreate.Fax = HtmlEncoder.Default.Encode(sheet.GetRow(row).GetCell(9) != null ? sheet.GetRow(row).GetCell(9).ToString() : null);
                    workgroupVendorToCreate.Email = HtmlEncoder.Default.Encode(sheet.GetRow(row).GetCell(10) != null ? sheet.GetRow(row).GetCell(10).ToString() : null);
                    workgroupVendorToCreate.Url = HtmlEncoder.Default.Encode(sheet.GetRow(row).GetCell(11) != null ? sheet.GetRow(row).GetCell(11).ToString() : null);
                    
                    workgroupVendorToCreate.Workgroup = workgroup;

                    ModelState.Clear();
                    workgroupVendorToCreate.TransferValidationMessagesTo(ModelState);

                    if (ModelState.IsValid)
                    {
                        _workgroupVendorRepository.EnsurePersistent(workgroupVendorToCreate);

                        successCount++;

                        //return this.RedirectToAction(nameof(VendorList), new { id = id });
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
            return this.RedirectToAction(nameof(VendorList), new { id = workgroup.Id });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Addresses may not be added to an administrative workgroup.";
                return this.RedirectToAction(nameof(Details), new { id = workgroup.Id });
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
        public async Task<ActionResult> AddAddress(int id, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if(workgroup.Administrative)
            {
                ErrorMessage = "Addresses may not be added to an administrative workgroup.";
                return this.RedirectToAction(nameof(Details), new { id = workgroup.Id });
            }

            if(!string.IsNullOrWhiteSpace(workgroupAddress.AeLocationCode))
            {
                var location = await _aggieEnterpriseService.GetShippingAddress(workgroupAddress);
                if(location == null)
                {
                    ErrorMessage = "Active Campus Location Code not found.";
                    return this.RedirectToAction(nameof(Details), new { id = workgroup.Id });
                }

                workgroupAddress.Address     = location.Address;
                workgroupAddress.City        = location.City;
                workgroupAddress.State       = location.State;
                workgroupAddress.Zip         = location.Zip;
                workgroupAddress.Building    = location.Building;
                workgroupAddress.Room        = location.Room;
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
            return this.RedirectToAction(nameof(Addresses), new { id = id });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            var workgroupAddress = workgroup.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (workgroupAddress == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction(nameof(Addresses), new { id = id });
            }
            if(workgroupAddress.Workgroup.Id != id)
            {
                ErrorMessage = "Address not part of this workgroup";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            var workgroupAddressToDelete = workgroup.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (workgroupAddressToDelete == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction(nameof(Addresses), new { id = id });
            }
            if(workgroupAddressToDelete.Workgroup.Id != id)
            {
                ErrorMessage = "Address not part of this workgroup";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            workgroupAddressToDelete.IsActive = false;
            _workgroupRepository.EnsurePersistent(workgroup);
            Message = "Address deleted.";
            return this.RedirectToAction(nameof(Addresses), new { id = id });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            var workgroupAddress = workgroup.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (workgroupAddress == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction(nameof(Addresses), new { id = id });
            }
            if(workgroupAddress.Workgroup.Id != id)
            {
                ErrorMessage = "Address not part of this workgroup";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            var workgroupAddress = workgroup.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (workgroupAddress == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction(nameof(Addresses), new { id = id });
            }
            if(workgroupAddress.Workgroup.Id != id)
            {
                ErrorMessage = "Address not part of this workgroup";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
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
        public async Task<ActionResult> EditAddress(int id, int addressId, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found.";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            var workgroupAddressToEdit = workgroup.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (workgroupAddressToEdit == null)
            {
                ErrorMessage = "Address not found.";
                return this.RedirectToAction(nameof(Addresses), new { id = id });
            }
            if(workgroupAddressToEdit.Workgroup.Id != id)
            {
                ErrorMessage = "Address not part of this workgroup";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if (!string.IsNullOrWhiteSpace(workgroupAddress.AeLocationCode))
            {
                var location = await _aggieEnterpriseService.GetShippingAddress(workgroupAddress);
                if (location == null)
                {
                    ErrorMessage = "Active Campus Location Code not found.";
                    return this.RedirectToAction(nameof(Details), new { id = workgroup.Id });
                }

                workgroupAddress.Address = location.Address;
                workgroupAddress.City = location.City;
                workgroupAddress.State = location.State;
                workgroupAddress.Zip = location.Zip;
                workgroupAddress.Building = location.Building;
                workgroupAddress.Room = location.Room;
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
                _mapper.Map(workgroupAddress, newAddress);
                newAddress.Workgroup = workgroup;
                workgroup.AddAddress(newAddress);
            }
            workgroup.Addresses.Where(a => a.Id == addressId).Single().IsActive = false;

            _workgroupRepository.EnsurePersistent(workgroup);
            return this.RedirectToAction(nameof(Addresses), new { id = id });

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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if(workgroup.Administrative && roleFilter == Role.Codes.Requester)
            {
                ErrorMessage = "Requester may not be added to an administrative workgroup.";
                return this.RedirectToAction(nameof(Details), new { id = workgroup.Id });
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
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
            
            if (duplicateCount > 0)
            {
                Message = $"Successfully added {successCount} people to workgroup as {workgroupPeoplePostModel.Role.Name}. {duplicateCount} not added because of duplicated role.";
            }
            else
            {
                Message = $"Successfully added {successCount} people to workgroup as {workgroupPeoplePostModel.Role.Name}.";
            }

            return this.RedirectToAction(nameof(People), new { id = id, roleFilter = workgroupPeoplePostModel.Role.Id });

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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            var workgroupPermission = _workgroupPermissionRepository.GetNullableById(workgroupPermissionId);
            if(workgroupPermission == null)
            {
                return this.RedirectToAction(nameof(People), new { id = id, roleFilter = rolefilter });
            }

            if(workgroupPermission.Workgroup != workgroup || workgroupPermission.IsAdmin) //Need this because you might have DA access to a different workgroup 
            {
                Message = "Person does not belong to workgroup.";
                return this.RedirectToAction(nameof(ErrorController.Index), typeof(ErrorController).ControllerName());
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
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            var workgroupPermissionToDelete = _workgroupPermissionRepository.GetNullableById(workgroupPermissionId);
            if (workgroupPermissionToDelete == null)
            {
                return this.RedirectToAction(nameof(People), new { id = id, roleFilter = rolefilter });
            }

            if(workgroupPermissionToDelete.Workgroup != workgroup || workgroupPermissionToDelete.IsAdmin) //Need this because you might have DA access to a different workgroup 
            {
                Message = "Person does not belong to workgroup.";
                return this.RedirectToAction(nameof(ErrorController.Index), typeof(ErrorController).ControllerName());
            }

            var availableWorkgroupPermissions = _workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.User == workgroupPermissionToDelete.User && !a.Role.IsAdmin && !a.IsAdmin).ToList();
            if (availableWorkgroupPermissions.Count() == 1)
            {
                // invalid the cache for the user that was just given permissions
                //System.Web.HttpContext.Current.Cache.Remove(string.Format(Resources.Role_CacheId, workgroupPermissionToDelete.User.Id));
                _workgroupService.RemoveFromCache(workgroupPermissionToDelete);

                var relatedPermissionsToDelete =
                    _workgroupPermissionRepository.Queryable.Where(
                        a =>
                        a.ParentWorkgroup == workgroup && a.User == workgroupPermissionToDelete.User &&
                        a.Role == workgroupPermissionToDelete.Role).ToList();

                
                _workgroupService.RemoveUserFromAccounts(workgroupPermissionToDelete);
                _workgroupService.RemoveUserFromPendingApprovals(workgroupPermissionToDelete); // TODO: Check for pending/open orders for this person. Set order to workgroup.
                _workgroupPermissionRepository.Remove(workgroupPermissionToDelete);

                foreach (var permission in relatedPermissionsToDelete)
                {
                    _workgroupPermissionRepository.Remove(permission);
                }

                Message = "Person successfully removed from role.";
                return this.RedirectToAction(nameof(People), new { id = id, roleFilter = rolefilter });
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
                    
                    var wp = _workgroupPermissionRepository.Queryable.Single(a => a.Workgroup == workgroup && a.User == workgroupPermissionToDelete.User && a.Role.Id == role && !a.IsAdmin);

                    var relatedPermissionsToDelete =
                        _workgroupPermissionRepository.Queryable.Where(
                            a =>
                            a.ParentWorkgroup == workgroup && a.User == wp.User &&
                            a.Role == wp.Role).ToList();

                    // invalid the cache for the user that was just given permissions
                    //System.Web.HttpContext.Current.Cache.Remove(string.Format(Resources.Role_CacheId, wp.User.Id));
                    _workgroupService.RemoveFromCache(wp);
                    _workgroupService.RemoveUserFromAccounts(wp);
                    _workgroupService.RemoveUserFromPendingApprovals(wp); // TODO: Check for pending/open orders for this person. Set order to workgroup.
                    _workgroupPermissionRepository.Remove(wp);

                    foreach (var permission in relatedPermissionsToDelete)
                    {
                        _workgroupPermissionRepository.Remove(permission);
                    }

                    removedCount++;
                }

                Message = string.Format("{0} {1} removed from {2}", removedCount, removedCount == 1 ? "role" : "roles", workgroupPermissionToDelete.User.FullName);
                return this.RedirectToAction(nameof(People), new { id = id, rolefilter = rolefilter });
            }


        }

        #endregion

        public ActionResult WhoHasAccessToWorkgroup(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            var model = WhoHasWorkgroupAccessViewModel.Create(workgroup);

            var primaryOrg = workgroup.PrimaryOrganization; //We will use this to determine who can edit the workgroup.

            // Find Parent DA Users.
            var parentOrgIds = _queryRepositoryFactory.OrganizationDescendantRepository.Queryable.Where(a => a.OrgId == primaryOrg.Id).Select(b => b.RollupParentId).Distinct().ToList();
            var parentOrganizations = new List<Organization>();
            foreach (var orgId in parentOrgIds)
            {
                var org = _repositoryFactory.OrganizationRepository.GetNullableById(orgId);
                if (org != null)
                {
                    parentOrganizations.Add(org);
                }
            }

            model.ParentOrgsExistingUsers = new List<KeyValuePair<string, string>>();
            model.OrganizationsWhithParentUsers = new List<Organization>();
            foreach (var organization in parentOrganizations)
            {
                Organization organization1 = organization;
                var users = _repositoryFactory.UserRepository.Queryable.Where(a => a.Organizations.Contains(organization1)).Select(b => b.Email).ToList();
                if (users.Count > 0)
                {
                    model.OrganizationsWhithParentUsers.Add(organization1);
                }
                foreach (var userEmail in users)
                {
                    model.ParentOrgsExistingUsers.Add(new KeyValuePair<string, string>(organization.Id, userEmail));
                }
            }

            model.AllExistingUsers = model.ParentOrgsExistingUsers.Select(a => a.Value).Distinct().ToList();

            return View(model);

        }

        #region Ajax Helpers

        /// <summary>
        /// Vendors #8
        /// Ajax action for retrieving kfs vendor addresses
        /// </summary>
        /// <returns></returns>
        [Obsolete("This is no longer used. Use the new worgroupVendorController instead.")]
        public JsonNetResult GetVendorAddresses(string vendorId)
        {
            var vendorAddresses = _vendorAddressRepository.Queryable.Where(a => a.Vendor.Id == vendorId).OrderByDescending(b => b.IsDefault).ToList();

            var results = vendorAddresses.Select(a => new { TypeCode = a.TypeCode, Name = a.DisplayNameWithDefault }).ToList();

            return new JsonNetResult(results);
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

            return new JsonNetResult(new {results.Room,  results.Building, results.City, results.State, results.Zip, results.Address});

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
        /// Tested
        /// Returns all of requesters for a given workgroup
        /// Note, this code was moved to the orders controller because it was limiting to departmental admins.
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

        /// <summary>
        /// #45
        /// </summary>
        /// <param name="id">Workgroup id</param>
        /// <param name="workgroupAccountId"></param>
        /// <param name="approver"></param>
        /// <param name="accountManager"></param>
        /// <param name="purchaser"></param>
        /// <returns></returns>
        public JsonNetResult UpdateAccount(int id, int workgroupAccountId, string approver, string accountManager, string purchaser)
        {          
            //Note, the workgroup id (id) is needed I think to determine workgroup access
            var success = false;
            var message = "Done";
            var rtApprover = string.Empty;
            var rtAccountManager = string.Empty;
            var rtPurchaser = string.Empty;

            var somethingChanged = false;
            try
            {
                var workgroupAccount = _workgroupAccountRepository.Queryable.Single(a => a.Id == workgroupAccountId && a.Workgroup.Id == id); //I'm checking the workgroup id here as well as an extra security check.
                switch (approver)
                {
                    case "DO_NOT_UPDATE":
                        break;
                    case "CLEAR_ALL":
                        if (workgroupAccount.Approver != null)
                        {
                            workgroupAccount.Approver = null;
                            somethingChanged = true;                           
                        }
                        break;
                    default:
                        if (workgroupAccount.Approver == null || workgroupAccount.Approver.Id != approver)
                        {                                     
                            var user = _userRepository.Queryable.Single(a => a.Id == approver);
                            workgroupAccount.Approver = user;
                            somethingChanged = true;
                        }
                        break;
                }

                switch (accountManager)
                {
                    case "DO_NOT_UPDATE":
                        break;
                    case "CLEAR_ALL":
                        if (workgroupAccount.AccountManager != null)
                        {
                            workgroupAccount.AccountManager = null;
                            somethingChanged = true;                          
                        }
                        break;
                    default:
                        if (workgroupAccount.AccountManager == null || workgroupAccount.AccountManager.Id != accountManager)
                        {
                            var user = _userRepository.Queryable.Single(a => a.Id == accountManager);
                            workgroupAccount.AccountManager = user;
                            somethingChanged = true;
                        }
                        break;
                }
                switch (purchaser)
                {
                    case "DO_NOT_UPDATE":
                        break;
                    case "CLEAR_ALL":
                        if (workgroupAccount.Purchaser != null)
                        {
                            workgroupAccount.Purchaser = null;
                            somethingChanged = true;                           
                        }
                        break;
                    default:
                        if (workgroupAccount.Purchaser == null || workgroupAccount.Purchaser.Id != purchaser)
                        {
                            var user = _userRepository.Queryable.Single(a => a.Id == purchaser);
                            workgroupAccount.Purchaser = user;
                            somethingChanged = true;
                        }
                        break;
                }

                if (somethingChanged)
                {
                    _workgroupAccountRepository.EnsurePersistent(workgroupAccount);
                }
                success = true;
                rtApprover = workgroupAccount.Approver != null ? workgroupAccount.Approver.FullNameAndId : string.Empty;
                rtAccountManager = workgroupAccount.AccountManager != null ? workgroupAccount.AccountManager.FullNameAndId : string.Empty;
                rtPurchaser = workgroupAccount.Purchaser != null ? workgroupAccount.Purchaser.FullNameAndId : string.Empty;
            }
            catch(Exception)
            {
                success = false;
                message = "Error";
            }

            return new JsonNetResult(new { success, message, rtApprover, rtAccountManager, rtPurchaser });

        }
        #endregion
    }


    public class WhoHasWorkgroupAccessViewModel
    {
        public Workgroup Workgroup { get; set; }
        public IList<KeyValuePair<string, string>> ParentOrgsExistingUsers { get; set; }
        public IList<Organization> OrganizationsWhithParentUsers { get; set; }
        public IList<string> AllExistingUsers { get; set; }

        public static WhoHasWorkgroupAccessViewModel Create(Workgroup workgroup)
        {
            var viewModel = new WhoHasWorkgroupAccessViewModel ();
            viewModel.Workgroup = workgroup;

            return viewModel;
        }
    }

    public class UpdateMultipleAccountsViewModel
    {
        public Workgroup Workgroup { get; set; }

        public string SelectedApprover { get; set; }
        public string SelectedAccountManager { get; set; }
        public string SelectedPurchaser { get; set; }

        public bool DefaultSelectedApprover { get; set; }
        public bool DefaultSelectedAccountManager { get; set; }
        public bool DefaultSelectedPurchaser { get; set; }

        public List<Tuple<string, string>> ApproverChoices { get; set; }
        public List<Tuple<string, string>> AccountManagerChoices { get; set; }
        public List<Tuple<string, string>> PurchaserChoices { get; set; }

        public static UpdateMultipleAccountsViewModel Create(Workgroup workgroup)
        {
            var viewModel = new UpdateMultipleAccountsViewModel {Workgroup = workgroup};
            
            viewModel.ApproverChoices = new List<Tuple<string, string>>();
            viewModel.AccountManagerChoices = new List<Tuple<string, string>>();
            viewModel.PurchaserChoices = new List<Tuple<string, string>>();

            viewModel.ApproverChoices.Add(new Tuple<string, string>("DO_NOT_UPDATE", "-- Do Not Update --"));
            viewModel.ApproverChoices.Add(new Tuple<string, string>("CLEAR_ALL", "-- Clear All --"));

            viewModel.AccountManagerChoices.Add(new Tuple<string, string>("DO_NOT_UPDATE", "-- Do Not Update --"));
            viewModel.AccountManagerChoices.Add(new Tuple<string, string>("CLEAR_ALL", "-- Clear All --"));

            viewModel.PurchaserChoices.Add(new Tuple<string, string>("DO_NOT_UPDATE", "-- Do Not Update --"));
            viewModel.PurchaserChoices.Add(new Tuple<string, string>("CLEAR_ALL", "-- Clear All --"));

            viewModel.DefaultSelectedApprover = false;
            viewModel.DefaultSelectedAccountManager = false;
            viewModel.DefaultSelectedPurchaser = false;

            return viewModel;
        }

    }

}
