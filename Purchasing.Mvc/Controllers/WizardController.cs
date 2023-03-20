using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
using Purchasing.Core.Services;
using System.Threading.Tasks;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// Controller for the Wizard class
    /// </summary>
    [Authorize(Policy = Role.Codes.DepartmentalAdmin)]
    [AuthorizeWorkgroupAccess]
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
        private readonly IRepository<WorkgroupAccount> _workgroupAccountRepository;
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;
        private readonly IWorkgroupAddressService _workgroupAddressService;
        private readonly IWorkgroupService _workgroupService;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IAggieEnterpriseService _aggieEnterpriseService;
        public const string WorkgroupType = "Workgroup";
        public const string OrganizationType = "Organization";


        public WizardController(IRepository<Workgroup> workgroupRepository,
            IRepositoryWithTypedId<User, string> userRepository,
            IRepositoryWithTypedId<Role, string> roleRepository,
            IRepository<WorkgroupPermission> workgroupPermissionRepository,
            ISecurityService securityService,
            IDirectorySearchService searchService,
            IRepository<WorkgroupVendor> workgroupVendorRepository,
            IRepositoryWithTypedId<Vendor, string> vendorRepository,
            IRepositoryWithTypedId<VendorAddress, Guid> vendorAddressRepository,
            IRepositoryWithTypedId<State, string> stateRepository,
            IRepository<WorkgroupAccount> workgroupAccountRepository,
            IQueryRepositoryFactory queryRepositoryFactory,
            IWorkgroupAddressService workgroupAddressService,
            IWorkgroupService workgroupService,
            IRepositoryFactory repositoryFactory,
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
            _workgroupAccountRepository = workgroupAccountRepository;
            _queryRepositoryFactory = queryRepositoryFactory; //New, need to add to get tests to run.
            _workgroupAddressService = workgroupAddressService;
            _workgroupService = workgroupService;
            _repositoryFactory = repositoryFactory;
            _aggieEnterpriseService = aggieEnterpriseService;
        }


        /// <summary>
        /// GET: /Wizard/
        /// View To start the Workgroup create wizard
        /// Test #1
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        //NOTE: This wizard will only allow adding stuff. No delete, edit or view. For that they will complete the wizard and go back to the Workgroup management

        /// <summary>
        /// Step 1
        /// Test #2
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateWorkgroup()
        {
            ViewBag.StepNumber = 1;
            var user = _userRepository.Queryable.Single(x => x.Id == CurrentUser.Identity.Name);

            var model = WorkgroupModifyModel.Create(user, _queryRepositoryFactory);

            return View(model);
        }

        /// <summary>
        /// Step #1
        /// Test #3
        /// </summary>
        /// <param name="workgroup"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateWorkgroup(Workgroup workgroup)
        {
            ViewBag.StepNumber = 1;

            var user = GetCurrentUser();

            //if(!user.Organizations.Any(a => a.Id == workgroup.PrimaryOrganization.Id))
            //{
            //    ModelState.AddModelError("Workgroup.PrimaryOrganization", "You do not have access to the selected organization");
            //}
            // takes into account department traversal down
            string message;
            if (!_securityService.HasWorkgroupOrOrganizationAccess(null, workgroup.PrimaryOrganization, out message))
            {
                ModelState.AddModelError("Workgroup.PrimaryOrganization", "You do not have access to the selected organization");
            }

            if(workgroup.Administrative && workgroup.SyncAccounts)
            {
                ModelState.AddModelError("Workgroup.Administrative", "Can not have both Administrative and Sync Accounts selected.");
            }

            if(workgroup.IsFullFeatured && !workgroup.Administrative)
            {
                ModelState.AddModelError("Workgroup.Administrative", "If Full Featured, workgroup must be administrative.");
            }

            if (workgroup.Administrative && !string.IsNullOrEmpty(workgroup.NotificationEmailList))
            {
                ModelState.AddModelError("Workgroup.NotificationEmailList", "Notification email list will not do anything for an administrative group.");
            }

            if (workgroup.Administrative && workgroup.DoNotInheritPermissions)
            {
                ModelState.AddModelError("Workgroup.DoNotInheritPermissions", "Can not have both Administrative and Do Not Inherit Permissions selected.");
            }



            if(!ModelState.IsValid)
            {
                var model = WorkgroupModifyModel.Create(user, _queryRepositoryFactory);
                model.Workgroup = workgroup;

                return View(model);
            }

            var createdWorkgroup = _workgroupService.CreateWorkgroup(workgroup, null);

            Message = string.Format("{0} workgroup was created",
                                    createdWorkgroup.Name);

            return this.RedirectToAction(nameof(AddSubOrganizations), new { id = createdWorkgroup.Id });
        }

        /// <summary>
        /// Step #2
        /// Test #4
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public ActionResult AddSubOrganizations(int id)
        {
            if(id == 0)
            {
                Message = "Workgroup must be created before proceeding";
                return this.RedirectToAction(nameof(CreateWorkgroup));
            }
            ViewBag.StepNumber = 2;

            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            var user = _userRepository.Queryable.Single(x => x.Id == CurrentUser.Identity.Name);

            var model = WorkgroupModifyModel.Create(user, _queryRepositoryFactory);
            model.Workgroup = workgroup;
            ViewBag.WorkgroupId = workgroup.Id;

            return View(model);
        }

        /// <summary>
        /// Step #2
        /// Test #5
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="selectedOrganizations"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddSubOrganizations(int id, string[] selectedOrganizations)
        {
            ViewBag.StepNumber = 2;
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }


            if(selectedOrganizations != null)
            {
                //Don't/can't have this anymore jcs 2012/3/12
                //var user = GetCurrentUser();
                //foreach(var selectedOrganization in selectedOrganizations)
                //{
                //    Check.Require(user.Organizations.Any(a => a.Id == selectedOrganization), string.Format("The organization '{0}' was being added by {1} to a workgroup's subOrganizations.", selectedOrganization, user.FullNameAndId));
                //}

                var existingOrganizations = workgroup.Organizations.Select(a => a.Id).ToList();
                var organizationsToAdd =
                    Repository.OfType<Organization>().Queryable.Where(a => selectedOrganizations.Contains(a.Id)).Select(
                        b => b.Id).ToList().Union(existingOrganizations).ToList();

                workgroup.Organizations =
                    Repository.OfType<Organization>().Queryable.Where(a => organizationsToAdd.Contains(a.Id)).ToList();
            }

            _workgroupRepository.EnsurePersistent(workgroup);

            _workgroupService.AddRelatedAdminUsers(workgroup); //This will search through all the related parent admin workgroups for users to add. 

            return this.RedirectToAction(nameof(SubOrganizations), new { id = workgroup.Id });
        }

        /// <summary>
        /// Step #2
        /// Test #6
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <returns></returns>
        public ActionResult SubOrganizations(int id)
        {
            ViewBag.StepNumber = 2;
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            return View(workgroup);
        }

        /// <summary>
        /// Step #3, #4, #5, #6, #7
        /// Test #7
        /// </summary>
        /// <returns></returns>
        public ActionResult AddPeople(int id, string roleFilter)
        {
            if(id == 0)
            {
                Message = "Workgroup must be created before proceeding";
                return this.RedirectToAction(nameof(CreateWorkgroup));
            }
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if(workgroup.Administrative && roleFilter == Role.Codes.Requester)
            {
                return this.RedirectToAction(nameof(AddPeople), new { id = id, roleFilter = Role.Codes.Approver });
            }
            var viewModel = WorgroupPeopleCreateModel.Create(_roleRepository, workgroup);
            if(!string.IsNullOrWhiteSpace(roleFilter))
            {
                viewModel.Role = _roleRepository.Queryable.SingleOrDefault(a => !a.IsAdmin && a.Id == roleFilter);
            }

            Check.Require(viewModel.Roles.Single(a => a.Level == 1).Id == "RQ", "RQ"); //Used for navigation in _StatusBar.cshtml
            Check.Require(viewModel.Roles.Single(a => a.Level == 2).Id == "AP", "AP"); //Used for navigation in _StatusBar.cshtml
            Check.Require(viewModel.Roles.Single(a => a.Level == 3).Id == "AM", "AM"); //Used for navigation in _StatusBar.cshtml
            Check.Require(viewModel.Roles.Single(a => a.Level == 4).Id == "PR", "PR"); //Used for navigation in _StatusBar.cshtml
            Check.Require(viewModel.Roles.Single(a => a.Level == 0).Id == "RV", "RV"); //Used for navigation in _StatusBar.cshtml

            ViewBag.rolefilter = roleFilter;
            if (viewModel.Role.Level == 0)
            {
                ViewBag.StepNumber = 3 + (5 - 1); // Ugliness because a reviewer is level zero
            }
            else
            {
                ViewBag.StepNumber = 3 + (viewModel.Role.Level - 1);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddPeople(int id, WizardWorkgroupPeoplePostModel workgroupPeoplePostModel, string roleFilter, string bulkEmail, string bulkKerb)
        {
            var notAddedKvp = new List<KeyValuePair<string, string>>();

            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if(workgroupPeoplePostModel.Users == null)
            {
                workgroupPeoplePostModel.Users = new List<string>();
            }

            ModelState.Clear();

            workgroupPeoplePostModel.Role = _roleRepository.Queryable.FirstOrDefault(a => a.Id == roleFilter);
            //Ensure role picked is valid.
            if(workgroupPeoplePostModel.Role != null)
            {
                if(!_roleRepository.Queryable.Any(a => !a.IsAdmin && a.Id == workgroupPeoplePostModel.Role.Id))
                {
                    ModelState.AddModelError("Role", "Invalid Role Selected - don't mess with the query string!");
                }

            }

            if(!ModelState.IsValid)
            {
                var viewModel = WorgroupPeopleCreateModel.Create(_roleRepository, workgroup);

                if(workgroupPeoplePostModel.Role != null)
                {
                    viewModel.Role = workgroupPeoplePostModel.Role;
                }
                if(workgroupPeoplePostModel.Users != null && workgroupPeoplePostModel.Users.Count > 0)
                {
                    var users = new List<IdAndName>();
                    foreach(var user in workgroupPeoplePostModel.Users)
                    {
                        var temp = _userRepository.GetNullableById(user);
                        if(temp != null)
                        {
                            users.Add(new IdAndName(temp.Id, temp.FullName));
                        }
                        else
                        {
                            var ldapuser = _searchService.FindUser(user);
                            if(ldapuser != null)
                            {
                                users.Add(new IdAndName(ldapuser.LoginId, string.Format("{0} {1}", ldapuser.FirstName, ldapuser.LastName)));
                            }
                        }
                    }
                    viewModel.Users = users;
                }
                ViewBag.rolefilter = roleFilter;
                if(!string.IsNullOrWhiteSpace(roleFilter))
                {
                    viewModel.Role = _roleRepository.Queryable.SingleOrDefault(a => !a.IsAdmin && a.Id == roleFilter);
                }
                if (viewModel.Role.Level == 0)
                {
                    ViewBag.StepNumber = 3 + (5 - 1); // Ugliness because a reviewer is level zero
                }
                else
                {
                    ViewBag.StepNumber = 3 + (viewModel.Role.Level - 1);
                }
                return View(viewModel);
            }

            int successCount = 0;
            int failCount = 0;
            int duplicateCount = 0;
            foreach(var u in workgroupPeoplePostModel.Users)
            {
                successCount = _workgroupService.TryToAddPeople(id, workgroupPeoplePostModel.Role, workgroup, successCount, u, ref failCount, ref duplicateCount, notAddedKvp);
            }



            #region Bulk Load Email
            successCount = _workgroupService.TryBulkLoadPeople(bulkEmail, true, id, workgroupPeoplePostModel.Role, workgroup, successCount, ref failCount, ref duplicateCount, notAddedKvp);

            //const string regexPattern = @"\b[A-Z0-9._-]+@[A-Z0-9][A-Z0-9.-]{0,61}[A-Z0-9]\.[A-Z.]{2,6}\b";

            //// Find matches
            //System.Text.RegularExpressions.MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(bulkEmail, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //foreach(System.Text.RegularExpressions.Match match in matches)
            //{
            //    var temp = match.ToString().ToLower();
            //    successCount = _workgroupService.TryToAddPeople(id, workgroupPeoplePostModel.Role, workgroup, successCount, temp,  ref failCount, notAddedKvp);
            //}
            #endregion

            #region Bulk Load Kerb

            successCount = _workgroupService.TryBulkLoadPeople(bulkKerb, false, id, workgroupPeoplePostModel.Role, workgroup, successCount, ref failCount, ref duplicateCount, notAddedKvp);

            //const string regexPattern4Kerb = @"\b[A-Z0-9]{2,10}\b";

            //// Find matches
            //System.Text.RegularExpressions.MatchCollection matchesKerb = System.Text.RegularExpressions.Regex.Matches(bulkKerb, regexPattern4Kerb, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //foreach(System.Text.RegularExpressions.Match match in matchesKerb)
            //{
            //    var temp = match.ToString().ToLower();
            //    successCount = _workgroupService.TryToAddPeople(id, workgroupPeoplePostModel.Role, workgroup, successCount, temp, ref failCount, notAddedKvp);
            //}
            #endregion

            Message = string.Format("Successfully added {0} people to workgroup as {1}. {2} not added because of duplicated role or not found.", successCount,
                                    workgroupPeoplePostModel.Role.Name, failCount);

            if(failCount > 0 || successCount == 0)
            {
                if(failCount == 0)
                {
                    ModelState.AddModelError("Users", "Must add at least 1 user or Skip");
                }
                else if (failCount == duplicateCount)
                {
                    Message = string.Format("The {0} users you added already have the role {1}", duplicateCount,
                       workgroupPeoplePostModel.Role.Name);
                    return this.RedirectToAction(nameof(People), new { id = id, roleFilter = workgroupPeoplePostModel.Role.Id });
                }
                

                var viewModel = WorgroupPeopleCreateModel.Create(_roleRepository, workgroup);
                viewModel.ErrorDetails = notAddedKvp;
                // ViewBag.DetailedMessage = notAddedSb.ToString();

                ViewBag.rolefilter = roleFilter;
                if(!string.IsNullOrWhiteSpace(roleFilter))
                {
                    viewModel.Role = _roleRepository.Queryable.SingleOrDefault(a => !a.IsAdmin && a.Id == roleFilter);
                }
                if (viewModel.Role.Level == 0)
                {
                    ViewBag.StepNumber = 3 + (5 - 1); // Ugliness because a reviewer is level zero
                }
                else
                {
                    ViewBag.StepNumber = 3 + (viewModel.Role.Level - 1);
                }
                return View(viewModel);
            }

            Message = string.Format("Successfully added {0} people to workgroup as {1}.", successCount,
                                   workgroupPeoplePostModel.Role.Name);
            return this.RedirectToAction(nameof(People), new { id = id, roleFilter = workgroupPeoplePostModel.Role.Id });
        }


        /// <summary>
        /// Step 3, 4, 5, 6, 7
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roleFilter"></param>
        /// <returns></returns>
        public ActionResult People(int id, string roleFilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if (workgroup.Administrative && roleFilter == Role.Codes.Requester)
            {
                return this.RedirectToAction(nameof(SubOrganizations), new { id = id });
            }
            var viewModel = WorgroupPeopleListModel.Create(_workgroupPermissionRepository, _roleRepository, workgroup, roleFilter);
            viewModel.CurrentRole = _roleRepository.Queryable.SingleOrDefault(a => !a.IsAdmin && a.Id == roleFilter);
            ViewBag.rolefilter = roleFilter;
            if (viewModel.CurrentRole.Level == 0)
            {
                ViewBag.StepNumber = 3 + (5 - 1); // Ugliness because a reviewer is level zero
            }
            else
            {
                ViewBag.StepNumber = 3 + (viewModel.CurrentRole.Level - 1);
            }
            
            return View(viewModel);
        }


        /// <summary>
        /// Step 8
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAccounts(int id)
        {
            if(id == 0)
            {
                Message = "Workgroup must be created before proceeding";
                return this.RedirectToAction(nameof(CreateWorkgroup));
            }
            ViewBag.StepNumber = 8;
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if (workgroup.Administrative)
            {
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if(workgroup.SyncAccounts)
            {
                return this.RedirectToAction(nameof(Vendors), new { id = id });
            }
            var viewModel = WorkgroupAccountModel.Create(Repository, workgroup);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> AddAccounts(int id, WorkgroupAccount workgroupAccount, string account_search)
        {
            ViewBag.StepNumber = 8;
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if(workgroupAccount != null && workgroupAccount.Account == null)
            {
                if (account_search == null) //Hacky fix so the call below doesn't throw an exception. Ideally, this would bind as an empty string value not a null
                {
                    account_search = string.Empty;
                }
                workgroupAccount.Account = _repositoryFactory.AccountRepository.GetNullableById(account_search);
            }


            if(workgroupAccount == null || workgroupAccount.Account == null || string.IsNullOrWhiteSpace(workgroupAccount.Account.Id))
            {
                if(workgroupAccount == null)
                {
                    workgroupAccount.Account = new Account();
                }
                ModelState.Clear();
                if(!string.IsNullOrWhiteSpace(account_search))
                {
                    ModelState.AddModelError("WorkgroupAccount.Account", "Account not found.");
                }
                //else
                //{
                //    ModelState.AddModelError("WorkgroupAccount.Account", "Select Account or skip.");
                //}
                //var viewModel1 = WorkgroupAccountModel.Create(Repository, workgroup, workgroupAccount);
                //return View(viewModel1);
            }

            var workgroupAccounts = _workgroupAccountRepository.Queryable.Where(a => a.Workgroup.Id == id).ToArray(); //All workgroup accounts

            var workgroupAccountToCreate = new WorkgroupAccount { Workgroup = workgroup };

            //_mapper.Map(workgroupAccount, workgroupAccountToCreate);//Mapper was causing me an exception JCS
            workgroupAccountToCreate.Account                = workgroupAccount.Account;
            workgroupAccountToCreate.AccountManager         = workgroupAccount.AccountManager;
            workgroupAccountToCreate.Approver               = workgroupAccount.Approver;
            workgroupAccountToCreate.Purchaser              = workgroupAccount.Purchaser;
            workgroupAccountToCreate.Name                   = workgroupAccount.Name?.Trim();
            workgroupAccountToCreate.FinancialSegmentString = workgroupAccount.FinancialSegmentString?.Trim()?.ToUpper();

            ModelState.Clear();
            workgroupAccountToCreate.TransferValidationMessagesTo(ModelState);

            if(workgroupAccountToCreate.Account != null && workgroupAccounts.Any(a => a.Workgroup.Id == workgroup.Id && a.Account != null && a.Account.Id == workgroupAccountToCreate.Account.Id))
            {
                ModelState.AddModelError("WorkgroupAccount.Account", "Account already exists for this workgroup.");
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
                if (accountValid.Warnings.Any())
                {
                    ErrorMessage = $"Warning (Review accounts after wizard completes): {accountValid.Warnings.FirstOrDefault().Value}";
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
                Message = "Workgroup account saved.";
                
                _workgroupAccountRepository.EnsurePersistent(workgroupAccountToCreate);

                //return this.RedirectToAction("Accounts", new {id = id});
                return this.RedirectToAction(nameof(Accounts), new { id = id });
            }

            var viewModel = WorkgroupAccountModel.Create(Repository, workgroup, workgroupAccountToCreate);
            return View(viewModel);
        }

        public ActionResult Accounts(int id)
        {
            ViewBag.StepNumber = 8;
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if (workgroup.Administrative)
            {
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if(workgroup.SyncAccounts)
            {
                return this.RedirectToAction(nameof(Vendors), new { id = id });
            }
            return View(workgroup);
        }

        /// <summary>
        /// Step 9
        /// </summary>
        /// <returns></returns>
        public ActionResult AddKfsVendor(int id)
        {
            ViewBag.StepNumber = 9;
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if (workgroup.Administrative)
            {
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            var viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, new WorkgroupVendor { Workgroup = workgroup });

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> AddKfsVendor(int id, WorkgroupVendor workgroupVendor)
        {
            ViewBag.StepNumber = 9;
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            var workgroupVendorToCreate = new WorkgroupVendor();

            workgroupVendorToCreate = await _workgroupService.TransferValues(workgroupVendor, workgroupVendorToCreate);

            workgroupVendorToCreate.Workgroup = workgroup;

            ModelState.Clear();
            //workgroupVendorToCreate.TransferValidationMessagesTo(ModelState);
            if(string.IsNullOrWhiteSpace(workgroupVendorToCreate.AeSupplierNumber))
            {
                ModelState.AddModelError("WorkgroupVendor.AeSupplierNumber", "Please select a Campus Vendor");
            }
            if(string.IsNullOrWhiteSpace(workgroupVendorToCreate.AeSupplierSiteCode))
            {
                ModelState.AddModelError("WorkgroupVendor.AeSupplierSiteCode", "Please select a Vendor Address");
            }
            if (ModelState.IsValid)
            {
                workgroupVendorToCreate.TransferValidationMessagesTo(ModelState);
                if (ModelState.ContainsKey("WorkgroupVendor.Email"))
                {
                    //Message = "Vendor Email is invalid";
                    workgroupVendorToCreate.Email = null;
                    ModelState.Clear();
                    //workgroupVendorToCreate.TransferValidationMessagesTo(ModelState); Note: Not checking this again so it *WILL* throw an exception that can be investigated.
                }
                
            }

            if(ModelState.IsValid)
            {
                //if(_workgroupVendorRepository.Queryable
                //    .Any(a => a.Workgroup.Id == id &&
                //        a.VendorId == workgroupVendorToCreate.VendorId &&
                //        a.VendorAddressTypeCode == workgroupVendorToCreate.VendorAddressTypeCode &&
                //        a.IsActive))
                //{
                //    Message = "KFS vendor has already been added";
                //    return this.RedirectToAction(nameof(Vendors), new { id = id });
                //}
                //var inactiveKfsVendor = _workgroupVendorRepository.Queryable
                //    .FirstOrDefault(a => a.Workgroup.Id == id &&
                //        a.VendorId == workgroupVendorToCreate.VendorId &&
                //        a.VendorAddressTypeCode == workgroupVendorToCreate.VendorAddressTypeCode &&
                //        !a.IsActive);
                //if(inactiveKfsVendor != null)
                //{
                //    inactiveKfsVendor.IsActive = true;
                //    _workgroupVendorRepository.EnsurePersistent(inactiveKfsVendor);
                //    Message = "KFS vendor added back. It was previously deleted from this workgroup.";
                //    return this.RedirectToAction(nameof(Vendors), new { id = id });
                //}

                if (_workgroupVendorRepository.Queryable
                    .Any(a => a.Workgroup.Id == id &&
                        a.AeSupplierNumber == workgroupVendorToCreate.AeSupplierNumber &&
                        a.AeSupplierSiteCode == workgroupVendorToCreate.AeSupplierSiteCode &&
                        a.IsActive))
                {
                    Message = "KFS vendor has already been added";
                    return this.RedirectToAction(nameof(Vendors), new { id = id });
                }

                //Possibly do a check to see if it is still valid/active
                var inactiveAeVendor = _workgroupVendorRepository.Queryable
                    .FirstOrDefault(a => a.Workgroup.Id == id &&
                        a.AeSupplierNumber == workgroupVendorToCreate.AeSupplierNumber &&
                        a.AeSupplierSiteCode == workgroupVendorToCreate.AeSupplierSiteCode &&
                        !a.IsActive);
                if (inactiveAeVendor != null)
                {
                    inactiveAeVendor.IsActive = true;
                    _workgroupVendorRepository.EnsurePersistent(inactiveAeVendor);
                    Message = "Aggie Enterprise vendor added back. It was previously deleted from this workgroup.";
                    return this.RedirectToAction(nameof(Vendors), new { id = id });
                }

                _workgroupVendorRepository.EnsurePersistent(workgroupVendorToCreate);

                Message = "WorkgroupVendor Created Successfully";

                return this.RedirectToAction(nameof(Vendors), new { id = id });
            }

            ErrorMessage = "Please correct errors before continuing, of skip this step.";
            WorkgroupVendorViewModel viewModel;


            viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, workgroupVendorToCreate, newVendor: true);

            return View(viewModel);
        }

        public ActionResult AddNewVendor(int id)
        {
            ViewBag.StepNumber = 9;
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if (workgroup.Administrative)
            {
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            var viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, new WorkgroupVendor { Workgroup = workgroup });

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> AddNewVendor(int id, WorkgroupVendor workgroupVendor, bool skipAddress)
        {
            ViewBag.StepNumber = 9;
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
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

            workgroupVendorToCreate = await _workgroupService.TransferValues(workgroupVendor, workgroupVendorToCreate);

            workgroupVendorToCreate.Workgroup = workgroup;

            ModelState.Clear();
            workgroupVendorToCreate.TransferValidationMessagesTo(ModelState);

            if(ModelState.IsValid)
            {
                _workgroupVendorRepository.EnsurePersistent(workgroupVendorToCreate);

                Message = "WorkgroupVendor Created Successfully";

                return this.RedirectToAction(nameof(Vendors), new { id = id });
            }

            WorkgroupVendorViewModel viewModel;


            //var vendor = _vendorRepository.GetNullableById(workgroupVendor.VendorId);
            //var vendorAddress = _vendorAddressRepository.Queryable.FirstOrDefault(a => a.Vendor == vendor && a.TypeCode == workgroupVendor.VendorAddressTypeCode);
            viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, workgroupVendorToCreate, null, null, true);



            return View(viewModel);
        }
        public ActionResult Vendors(int id)
        {
            if(id == 0)
            {
                Message = "Workgroup must be created before proceeding";
                return this.RedirectToAction(nameof(CreateWorkgroup));
            }
            ViewBag.StepNumber = 9;

            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if (workgroup.Administrative)
            {
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            var workgroupVendorList = _workgroupVendorRepository.Queryable.Where(a => a.Workgroup == workgroup && a.IsActive);
            ViewBag.WorkgroupId = id;
            ViewBag.Title = workgroup.Name;
            ViewBag.IsAdministrative = workgroup.Administrative;
            ViewBag.IsAccountSync = workgroup.SyncAccounts;
            return View(workgroupVendorList.ToList());
        }

        /// <summary>
        /// Step 10
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAddresses(int id)
        {
            if(id == 0)
            {
                Message = "Workgroup must be created before proceeding";
                return this.RedirectToAction(nameof(CreateWorkgroup));
            }
            ViewBag.StepNumber = 10;
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if (workgroup.Administrative)
            {
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
            viewModel.WorkgroupAddress = new WorkgroupAddress();
            viewModel.WorkgroupAddress.Workgroup = workgroup;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> AddAddresses(int id, WorkgroupAddress workgroupAddress)
        {
            ViewBag.StepNumber = 10;
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }

            if (!string.IsNullOrWhiteSpace(workgroupAddress.AeLocationCode))
            {
                var location = await _aggieEnterpriseService.GetShippingAddress(workgroupAddress);
                if (location == null)
                {
                    ErrorMessage = "Active Campus Location Code not found.";
                    var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
                    viewModel.WorkgroupAddress = workgroupAddress;
                    viewModel.WorkgroupAddress.Workgroup = workgroup;
                    return View(viewModel);
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
                ErrorMessage = "Address not valid";
                var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
                viewModel.WorkgroupAddress = workgroupAddress;
                viewModel.WorkgroupAddress.Workgroup = workgroup;
                return View(viewModel);
            }
            var matchFound = 0;
            foreach(var address in workgroup.Addresses)
            {
                matchFound = _workgroupAddressService.CompareAddress(workgroupAddress, address);
                if(matchFound > 0)
                {
                    break;
                }
            }
            if(matchFound > 0)
            {
                var matchedAddress = workgroup.Addresses.Single(a => a.Id == matchFound);
                if(!matchedAddress.IsActive)
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

        public ActionResult Addresses(int id)
        {
            ViewBag.StepNumber = 10;
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if (workgroup.Administrative)
            {
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            var viewModel = WorkgroupAddressListModel.Create(workgroup);
            return View(viewModel);
        }

        /// <summary>
        /// Step 11
        /// </summary>
        /// <returns></returns>
        public ActionResult AddConditionalApproval(int id)
        {
            if(id == 0)
            {
                Message = "Workgroup must be created before proceeding";
                return this.RedirectToAction(nameof(CreateWorkgroup));
            }
            ViewBag.StepNumber = 11;
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if (workgroup.Administrative)
            {
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            var model = new ConditionalApproval();
            model.Workgroup = workgroup;
            ViewBag.IsAccountSync = workgroup.SyncAccounts;

            return View(model);
        }

        [HttpPost]
        public ActionResult AddConditionalApproval(int id, ConditionalApproval conditionalApproval, string primaryApproverParm, string secondaryApproverParm)
        {
            ViewBag.StepNumber = 11;
            // TODO Check that primary and secondary approvers are in db, add if not. Double check against ConditionalApprover controller
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            ViewBag.IsAccountSync = workgroup.SyncAccounts;

            var primaryApproverInDb = GetUserBySearchTerm(primaryApproverParm);
            var secondaryApproverInDb = string.IsNullOrWhiteSpace(secondaryApproverParm)
                                            ? null
                                            : GetUserBySearchTerm(secondaryApproverParm);

            if(primaryApproverInDb == null)
            {
                DirectoryUser primaryApproverInLdap = _searchService.FindUser(primaryApproverParm);

                if(primaryApproverInLdap == null)
                {
                    ModelState.AddModelError("primaryApproverParm",
                         "No user could be found with the kerberos or email address entered");
                }
                else //found the primary approver in ldap
                {
                    primaryApproverInDb = new User(primaryApproverInLdap.LoginId)
                    {
                        FirstName = primaryApproverInLdap.FirstName,
                        LastName = primaryApproverInLdap.LastName,
                        Email = primaryApproverInLdap.EmailAddress,
                        IsActive = true
                    };

                    _userRepository.EnsurePersistent(primaryApproverInDb, forceSave: true);
                }
            }

            if(!string.IsNullOrWhiteSpace(secondaryApproverParm)) //only check if a value was provided
            {
                if(secondaryApproverInDb == null)
                {
                    DirectoryUser secondaryApproverInLdap = _searchService.FindUser(secondaryApproverParm);

                    if(secondaryApproverInLdap == null)
                    {
                        ModelState.AddModelError("secondaryApproverParm",
                                                 "No user could be found with the kerberos or email address entered");
                    }
                    else //found the secondary approver in ldap
                    {
                        secondaryApproverInDb = new User(secondaryApproverInLdap.LoginId)
                        {
                            FirstName = secondaryApproverInLdap.FirstName,
                            LastName = secondaryApproverInLdap.LastName,
                            Email = secondaryApproverInLdap.EmailAddress,
                            IsActive = true
                        };

                        _userRepository.EnsurePersistent(secondaryApproverInDb, forceSave: true);
                    }
                }
            }

            conditionalApproval.PrimaryApprover = primaryApproverInDb;
            conditionalApproval.SecondaryApprover = secondaryApproverInDb;
            conditionalApproval.Workgroup = workgroup;
            ModelState.Clear();
            conditionalApproval.TransferValidationMessagesTo(ModelState);

            if(ModelState.IsValid)
            {
                var newConditionalApproval = new ConditionalApproval
                                                 {
                                                     Question = conditionalApproval.Question,
                                                     Organization = conditionalApproval.Organization,
                                                     Workgroup = workgroup,
                                                     PrimaryApprover = primaryApproverInDb,
                                                     SecondaryApprover = secondaryApproverInDb
                                                 };
                Repository.OfType<ConditionalApproval>().EnsurePersistent(newConditionalApproval);
                return this.RedirectToAction(nameof(ConditionalApprovals), new { id = id });
            }
            conditionalApproval.Workgroup = workgroup;
            return View(conditionalApproval);
        }

        public ActionResult ConditionalApprovals(int id)
        {
            ViewBag.StepNumber = 11;
            if(id == 0)
            {
                Message = "Workgroup must be created before proceeding";
                return this.RedirectToAction(nameof(CreateWorkgroup));
            }

            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if (workgroup.Administrative)
            {
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            ViewBag.IsAccountSync = workgroup.SyncAccounts;
            var workgroupConditionalApprovals =
                Repository.OfType<ConditionalApproval>().Queryable.Where(
                    a => a.Workgroup != null && a.Workgroup.Id == workgroup.Id);
            ViewBag.WorkgroupId = id;
            ViewBag.Title = workgroup.Name;
            return View(workgroupConditionalApprovals.ToList());

        }

        /// <summary>
        /// Step 12
        /// </summary>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            ViewBag.StepNumber = 12;
            var workgroup = _workgroupRepository.GetNullableById(id);
            if(workgroup == null)
            {
                Message = "Workgroup not found.";
                this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            if (workgroup.Administrative)
            {
                return this.RedirectToAction(nameof(WorkgroupController.Index), typeof(WorkgroupController).ControllerName(), new { showAll = false });
            }
            var viewModel = WorkgroupDetailModel.Create(_workgroupPermissionRepository, workgroup);
            return View(viewModel);
        }


        public JsonNetResult SearchUsers(string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Trim();

            var users = _userRepository.Queryable.Where(a => a.Email == searchTerm || a.Id == searchTerm).ToList();
            if(users.Count == 0)
            {
                var ldapusers = _searchService.SearchUsers(searchTerm);

                foreach(var ldapuser in ldapusers.Where(x => x.LoginId != null).Take(10))
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




        private ConditionalApprovalModifyModel CreateModifyModel(string approvalType, ConditionalApprovalModifyModel existingModel = null)
        {
            var model = existingModel ?? new ConditionalApprovalModifyModel { ApprovalType = approvalType };

            var userWithOrgs = GetUserWithOrgs();

            if(approvalType == WorkgroupType)
            {
                model.Workgroups = GetWorkgroups(userWithOrgs).ToList();

                if(!model.Workgroups.Any())
                {
                    return null;
                }
            }
            else if(approvalType == OrganizationType)
            {
                model.Organizations = userWithOrgs.Organizations.ToList();

                if(!model.Organizations.Any())
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            return model;
        }

        private User GetUserWithOrgs()
        {
            return
                _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Fetch(x => x.Organizations).
                    Single();
        }

        private IQueryable<Workgroup> GetWorkgroups(User user)
        {
            var orgIds = user.Organizations.Select(x => x.Id).ToArray();

            return _workgroupRepository.Queryable.Where(x => x.Organizations.Any(a => orgIds.Contains(a.Id)));

        }

        private User GetUserBySearchTerm(string searchTerm)
        {
            return _userRepository.Queryable.SingleOrDefault(x => x.Id == searchTerm || x.Email == searchTerm);
        }


    }




}
