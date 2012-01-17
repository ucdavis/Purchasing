

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using UCDArch.Data.NHibernate;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Purchasing.Web.Services;
using MvcContrib;

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
        private readonly IWorkgroupService _workgroupService;
        public const string WorkgroupType = "Workgroup";
        public const string OrganizationType = "Organization";

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
            _workgroupAddressService = workgroupAddressService;
            _workgroupService = workgroupService;
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
            workgroupToCreate.IsActive = true;

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
        [ValidateInput(false)]
        public ActionResult AddPeople(int id, WizardWorkgroupPeoplePostModel workgroupPeoplePostModel, string roleFilter, string bulkEmail, string bulkKerb)
        {
            var notAddedSb = new StringBuilder();
            
            ActionResult redirectToAction;
            var workgroup = GetWorkgroupAndCheckAccess(id, out redirectToAction);
            if (workgroup == null)
            {
                return redirectToAction;
            }
            if(workgroupPeoplePostModel.Users == null)
            {
                workgroupPeoplePostModel.Users = new List<string>();
            }

            ModelState.Clear();

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
                if(!string.IsNullOrWhiteSpace(roleFilter))
                {
                    viewModel.Role = _roleRepository.Queryable.Where(a => a.Level >= 1 && a.Level <= 4 && a.Id == roleFilter).SingleOrDefault();
                }
                return View(viewModel);
            }

            int successCount = 0;
            int failCount = 0;
            foreach (var u in workgroupPeoplePostModel.Users)
            {
                successCount += TryToAddPeople(id, workgroupPeoplePostModel, workgroup, successCount, u, notAddedSb, ref failCount);
            }


            
            #region Bulk Load Email
           
            const string regexPattern = @"\b[A-Z0-9._-]+@[A-Z0-9][A-Z0-9.-]{0,61}[A-Z0-9]\.[A-Z.]{2,6}\b";

            // Find matches
            System.Text.RegularExpressions.MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(bulkEmail, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            foreach(System.Text.RegularExpressions.Match match in matches)
            {
                var temp = match.ToString().ToLower();
                successCount += TryToAddPeople(id, workgroupPeoplePostModel, workgroup, successCount, temp, notAddedSb, ref failCount);
            }
            #endregion

            #region Bulk Load Kerb

            const string regexPattern4Kerb = @"\b[A-Z0-9]{2,10}\b";

            // Find matches
            System.Text.RegularExpressions.MatchCollection matchesKerb = System.Text.RegularExpressions.Regex.Matches(bulkKerb, regexPattern4Kerb, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            foreach(System.Text.RegularExpressions.Match match in matchesKerb)
            {
                var temp = match.ToString().ToLower();
                successCount += TryToAddPeople(id, workgroupPeoplePostModel, workgroup, successCount, temp, notAddedSb, ref failCount);
            }
            #endregion

            Message = string.Format("Successfully added {0} people to workgroup as {1}. {2} not added because of duplicated role.", successCount,
                                    workgroupPeoplePostModel.Role.Name, failCount);

            if(failCount > 0)
            {
                var viewModel = WorgroupPeopleCreateModel.Create(_roleRepository, workgroup);
                ViewBag.DetailedMessage = notAddedSb.ToString();
                ViewBag.rolefilter = roleFilter;
                if(!string.IsNullOrWhiteSpace(roleFilter))
                {
                    viewModel.Role = _roleRepository.Queryable.Where(a => a.Level >= 1 && a.Level <= 4 && a.Id == roleFilter).SingleOrDefault();
                }
                return View(viewModel);
            }

            Message = string.Format("Successfully added {0} people to workgroup as {1}.", successCount,
                                   workgroupPeoplePostModel.Role.Name);
            return this.RedirectToAction(a => a.People(id, workgroupPeoplePostModel.Role.Id));
        }

        private int TryToAddPeople(int id, WizardWorkgroupPeoplePostModel workgroupPeoplePostModel, Workgroup workgroup, int successCount, string u, StringBuilder notAddedSb, ref int failCount)
        {
            var saveParm = u;
            var user = _userRepository.GetNullableById(u);
            if (user == null)
            {
                var ldapuser = _searchService.FindUser(u);
                if (ldapuser != null)
                {
                    u = ldapuser.LoginId;
                    user = _userRepository.GetNullableById(ldapuser.LoginId);
                    if(user == null)
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
            }

            if (user == null)
            {
                notAddedSb.AppendLine(string.Format("{0} : Not found", saveParm));
                failCount++;
                return successCount;
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
                notAddedSb.AppendLine(string.Format("{0} : Is a duplicate", u));
                failCount++;
            }
            return successCount;
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
            var workgroup = _workgroupRepository.Queryable.Single(a=>a.Id==id);
           
            var viewModel = WorkgroupAccountModel.Create(Repository, workgroup);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddAccounts(int id, WorkgroupAccount workgroupAccount)
        {
            var workgroup = _workgroupRepository.Queryable.Single(a => a.Id == id);
           
            var workgroupAccountToCreate = new WorkgroupAccount() { Workgroup = workgroup };
            Mapper.Map(workgroupAccount, workgroupAccountToCreate);

            ModelState.Clear();
            workgroupAccountToCreate.TransferValidationMessagesTo(ModelState);

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

        public ActionResult Accounts(int id)
        {
            var workgroup = _workgroupRepository.Queryable.Single(a=>a.Id==id);
           
            return View(workgroup);
        }

        /// <summary>
        /// Step 5
        /// </summary>
        /// <returns></returns>
        public ActionResult AddKfsVendor(int id)
        {
            var workgroup = _workgroupRepository.Queryable.Single(a=>a.Id==id);
            
            var viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, new WorkgroupVendor() { Workgroup = workgroup });

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddKfsVendor(int id, WorkgroupVendor workgroupVendor)
        {
            var workgroup = _workgroupRepository.Queryable.Single(a=>a.Id==id);

           var workgroupVendorToCreate = new WorkgroupVendor();

            _workgroupService.TransferValues(workgroupVendor, workgroupVendorToCreate);

            workgroupVendorToCreate.Workgroup = workgroup;

            ModelState.Clear();
            workgroupVendorToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _workgroupVendorRepository.EnsurePersistent(workgroupVendorToCreate);

                Message = "WorkgroupVendor Created Successfully";

                return this.RedirectToAction(a => a.Vendors(id));
            }

            WorkgroupVendorViewModel viewModel;

            
                viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, workgroupVendorToCreate, newVendor: true);
            
            return View(viewModel);
        }

        public ActionResult AddNewVendor(int id)
        {
            var workgroup = _workgroupRepository.Queryable.Single(a => a.Id == id);

            var viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, new WorkgroupVendor() { Workgroup = workgroup });

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddNewVendor(int id, WorkgroupVendor workgroupVendor)
        {
            var workgroup = _workgroupRepository.Queryable.Single(a=>a.Id==id);
            
            var workgroupVendorToCreate = new WorkgroupVendor();

            _workgroupService.TransferValues(workgroupVendor, workgroupVendorToCreate);

            workgroupVendorToCreate.Workgroup = workgroup;

            ModelState.Clear();
            workgroupVendorToCreate.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _workgroupVendorRepository.EnsurePersistent(workgroupVendorToCreate);

                Message = "WorkgroupVendor Created Successfully";

                return this.RedirectToAction(a => a.Vendors(id));
            }

            WorkgroupVendorViewModel viewModel;

           
                var vendor = _vendorRepository.GetNullableById(workgroupVendor.VendorId);
                var vendorAddress = _vendorAddressRepository.Queryable.Where(a => a.Vendor == vendor && a.TypeCode == workgroupVendor.VendorAddressTypeCode).FirstOrDefault();
                viewModel = WorkgroupVendorViewModel.Create(_vendorRepository, workgroupVendorToCreate, vendor, vendorAddress, true);
            


            return View(viewModel);
        }
        public ActionResult Vendors(int id)
        {
            var workgroup = _workgroupRepository.Queryable.Single(a => a.Id == id);

            var workgroupVendorList = _workgroupVendorRepository.Queryable.Where(a => a.Workgroup == workgroup && a.IsActive);
            ViewBag.WorkgroupId = id;
            return View(workgroupVendorList.ToList());
        }

        /// <summary>
        /// Step 6
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAddresses(int id)
        {
            var workgroup = _workgroupRepository.Queryable.Single(a => a.Id == id);

            var viewModel = WorkgroupAddressViewModel.Create(workgroup, _stateRepository, true);
            viewModel.WorkgroupAddress = new WorkgroupAddress();
            viewModel.WorkgroupAddress.Workgroup = workgroup;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddAddresses(int id, WorkgroupAddress workgroupAddress)
        {
            var workgroup = _workgroupRepository.Queryable.Single(a => a.Id == id);

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
                var matchedAddress = workgroup.Addresses.Where(a => a.Id == matchFound).Single();
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

        public ActionResult Addresses(int id)
        {
            var workgroup = _workgroupRepository.Queryable.Single(a=>a.Id==id);
            var viewModel = WorkgroupAddressListModel.Create(workgroup);
            return View(viewModel);
        }

        /// <summary>
        /// Step 7
        /// </summary>
        /// <returns></returns>
        public ActionResult AddConditionalApproval(int id)
        {
            var workgroup = _workgroupRepository.Queryable.Single(a => a.Id == id);
            var model = new ConditionalApproval();
            model.Workgroup = workgroup;

           return View(model);
        }

        [HttpPost]
        public ActionResult AddConditionalApproval(int id, ConditionalApproval conditionalApproval, string primaryApproverParm, string secondaryApproverParm)
        {
            // TODO Check that primary and secondary approvers are in db, add if not. Double check against ConditionalApprover controller
            var workgroup = _workgroupRepository.Queryable.Single(a => a.Id == id);

            var primaryApproverInDb = GetUserBySearchTerm(primaryApproverParm);
            var secondaryApproverInDb = string.IsNullOrWhiteSpace(secondaryApproverParm)
                                            ? null
                                            : GetUserBySearchTerm(secondaryApproverParm);

            if (primaryApproverInDb == null)
            {
                DirectoryUser primaryApproverInLdap = _searchService.FindUser(primaryApproverParm);

                if (primaryApproverInLdap == null)
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

            if (!string.IsNullOrWhiteSpace(secondaryApproverParm)) //only check if a value was provided
            {
                if (secondaryApproverInDb == null)
                {
                    DirectoryUser secondaryApproverInLdap = _searchService.FindUser(secondaryApproverParm);

                    if (secondaryApproverInLdap == null)
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

            if (ModelState.IsValid)
            {
                var newConditionalApproval = new ConditionalApproval()
                                                 {
                                                     Question = conditionalApproval.Question,
                                                     Organization = conditionalApproval.Organization,
                                                     Workgroup = workgroup,
                                                     PrimaryApprover = primaryApproverInDb,
                                                     SecondaryApprover = secondaryApproverInDb
                                                 };
                Repository.OfType<ConditionalApproval>().EnsurePersistent(newConditionalApproval);
                return this.RedirectToAction(a => a.ConditionalApprovals(id));
            }
            conditionalApproval.Workgroup = workgroup;
            return View(conditionalApproval);
        }

        public ActionResult ConditionalApprovals(int id )
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

        private ConditionalApprovalModifyModel CreateModifyModel(string approvalType, ConditionalApprovalModifyModel existingModel = null)
        {
            var model = existingModel ?? new ConditionalApprovalModifyModel { ApprovalType = approvalType };

            var userWithOrgs = GetUserWithOrgs();

            if (approvalType == WorkgroupType)
            {
                model.Workgroups = GetWorkgroups(userWithOrgs).ToList();

                if (model.Workgroups.Count() == 0)
                {
                    return null;
                }
            }
            else if (approvalType == OrganizationType)
            {
                model.Organizations = userWithOrgs.Organizations.ToList();

                if (model.Organizations.Count() == 0)
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
            return _userRepository.Queryable.Where(x => x.Id == searchTerm || x.Email == searchTerm).SingleOrDefault();
        }
    }



}
