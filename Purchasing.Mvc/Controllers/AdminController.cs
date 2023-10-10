using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using AzureActiveDirectorySearcher;
using Ietws;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Mvc.App_GlobalResources;
using Purchasing.Mvc.Services;
using Purchasing.Mvc.Controllers;
using Serilog;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using Microsoft.Extensions.Configuration;
using Purchasing.Core.Services;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// Controller for the Admin class
    /// </summary>
    [Authorize(Policy = Role.Codes.Admin)]
    public class AdminController : ApplicationController
    {
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<Role, string> _roleRepository;
        private readonly IRepositoryWithTypedId<Organization, string> _organizationRepository;
        private readonly IDirectorySearchService _searchService;
        private readonly IRepositoryWithTypedId<EmailPreferences, string> _emailPreferencesRepository;
        private readonly IUserIdentity _userIdentity;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IWorkgroupService _workgroupService;
        private readonly SendGridSettings _sendGridSettings;
        private readonly IConfiguration _configuration;
        private readonly IAggieEnterpriseService _aggieEnterpriseService;

        public AdminController(
            IRepositoryWithTypedId<User, string> userRepository,
            IRepositoryWithTypedId<Role, string> roleRepository,
            IRepositoryWithTypedId<Organization, string> organizationRepository,
            IDirectorySearchService searchService,
            IRepositoryWithTypedId<EmailPreferences, string> emailPreferencesRepository,
            IUserIdentity userIdentity,
            IRepositoryFactory repositoryFactory,
            IWorkgroupService workgroupService,
            IOptions<SendGridSettings> sendGridSettings,
            IConfiguration configuration,
            IAggieEnterpriseService aggieEnterpriseService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _organizationRepository = organizationRepository;
            _searchService = searchService;
            _emailPreferencesRepository = emailPreferencesRepository;
            _userIdentity = userIdentity;
            _repositoryFactory = repositoryFactory;
            _workgroupService = workgroupService;
            _sendGridSettings = sendGridSettings.Value;
            _configuration = configuration;
            _aggieEnterpriseService = aggieEnterpriseService;
        }

        //
        // GET: /Admin/
        public ActionResult Index()
        {
            var admins = _roleRepository.Queryable.Where(x => x.Name.EndsWith("Admin")).Fetch(x => x.Users).ToList();

            var model = new AdminListModel()
            {
                Admins = admins.Single(x => x.Id == Role.Codes.Admin).Users.ToList(),
                DepartmentalAdmins = admins.Single(x => x.Id == Role.Codes.DepartmentalAdmin).Users.ToList(),
                SscAdmins = admins.Single(x => x.Id == Role.Codes.SscAdmin).Users.ToList()
            };

            return View(model);
        }

        public ActionResult ModifyDepartmental(string id)
        {
            var user = _userRepository.Queryable.Where(x => x.Id == id).Fetch(x => x.Organizations).SingleOrDefault() ??
                       new User(null) { IsActive = true };
            var isSscAdmin = user.Roles.Any(x => x.Id == Role.Codes.SscAdmin);

            var model = new DepartmentalAdminModel
            {
                User = user,
                IsSscAdmin = isSscAdmin
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ModifyDepartmental(DepartmentalAdminModel departmentalAdminModel, List<string> orgs)
        {
            if (orgs == null || orgs.Count == 0)
            {
                ModelState.AddModelError("User.Organizations", "You must select at least one department for a departmental Admin.");
            }
            if (!ModelState.IsValid)
            {
                return View(departmentalAdminModel);
            }

            var user = _userRepository.GetNullableById(departmentalAdminModel.User.Id) ?? new User(departmentalAdminModel.User.Id);


            departmentalAdminModel.User.Roles = user.Roles;

            //_mapper.Map(departmentalAdminModel.User, user); // This was causing problems if an existing DA was saved.
            user.FirstName = departmentalAdminModel.User.FirstName;
            user.LastName = departmentalAdminModel.User.LastName;
            user.Email = departmentalAdminModel.User.Email;
            user.IsActive = departmentalAdminModel.User.IsActive;

            var isDeptAdmin = user.Roles.Any(x => x.Id == Role.Codes.DepartmentalAdmin);
            var isSscAdmin = user.Roles.Any(x => x.Id == Role.Codes.SscAdmin);

            if (!isDeptAdmin)
            {
                user.Roles.Add(_roleRepository.GetById(Role.Codes.DepartmentalAdmin));
            }

            user.Organizations = new List<Organization>();
            foreach (var org in orgs)
            {
                user.Organizations.Add(_organizationRepository.Queryable.Single(a => a.Id == org));
            }


            _userRepository.EnsurePersistent(user);

            // invalid the cache for the user that was just given permissions
            _userIdentity.RemoveUserRoleFromCache(Resources.Role_CacheId, user.Id);

            if (isSscAdmin && departmentalAdminModel.UpdateAllSscAdmins)
            {
                var userList = new List<string>();
                var users = _roleRepository.Queryable.Where(x => x.Id == Role.Codes.SscAdmin).SelectMany(x => x.Users).Where(w => w.IsActive && w.Id != user.Id).ToList();

                foreach (var user1 in users)
                {
                    user1.Organizations = new List<Organization>();
                    foreach (var org in orgs)
                    {
                        user1.Organizations.Add(_organizationRepository.Queryable.Single(a => a.Id == org));
                    }
                    _userRepository.EnsurePersistent(user1);
                    // invalid the cache for the user that was just given permissions
                    _userIdentity.RemoveUserRoleFromCache(Resources.Role_CacheId, user1.Id);
                    userList.Add(user1.FullNameAndId);
                }
                Message =
                    string.Format(
                        "{0} was added as a departmental admin to the specified organization(s) Also added perms for {1}.",
                        user.FullNameAndId, string.Join(",", userList.ToArray()));
            }
            else
            {
                Message = string.Format("{0} was added as a departmental admin to the specified organization(s)",
                                    user.FullNameAndId);
            }

            //return this.RedirectToAction(nameof(Index));
            return this.RedirectToAction(nameof(Index));
        }

        public ActionResult ModifyAdmin(string id)
        {
            var user = (!string.IsNullOrWhiteSpace(id) ? _userRepository.GetNullableById(id) : new User(null) { IsActive = true }) ??
                       new User(null) { IsActive = true };

            return View(user);
        }

        [HttpPost]
        public ActionResult ModifyAdmin(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var userToSave = _userRepository.GetNullableById(user.Id) ?? new User(user.Id);
            //user.Organizations = userToSave.Organizations; //Transfer the orgs and roles since they aren't managed on this page
            //user.Roles = userToSave.Roles;
            userToSave.FirstName = user.FirstName;
            userToSave.LastName = user.LastName;
            userToSave.Email = user.Email;
            userToSave.IsActive = user.IsActive;

            //_mapper.Map(user, userToSave);


            var isAdmin = userToSave.Roles.Any(x => x.Id == Role.Codes.Admin);

            if (!isAdmin)
            {
                userToSave.Roles.Add(_roleRepository.GetById(Role.Codes.Admin));
            }

            _userRepository.EnsurePersistent(userToSave);

            // invalid the cache for the user that was just given permissions
            _userIdentity.RemoveUserRoleFromCache(Resources.Role_CacheId, userToSave.Id);


            if (_emailPreferencesRepository.GetNullableById(userToSave.Id) == null)
            {
                _emailPreferencesRepository.EnsurePersistent(new EmailPreferences(userToSave.Id));
            }

            Message = string.Format("{0} was edited under the administrator role", user.FullNameAndId);

            return this.RedirectToAction(nameof(Index));
        }

        public ActionResult ModifySscAdmin(string id)
        {
            var user = (!string.IsNullOrWhiteSpace(id) ? _userRepository.GetNullableById(id) : new User(null) { IsActive = true }) ??
                       new User(null) { IsActive = true };

            return View(user);
        }

        [HttpPost]
        public ActionResult ModifySscAdmin(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var userToSave = _userRepository.GetNullableById(user.Id) ?? new User(user.Id);
            //user.Organizations = userToSave.Organizations; //Transfer the orgs and roles since they aren't managed on this page
            //user.Roles = userToSave.Roles;
            userToSave.FirstName = user.FirstName;
            userToSave.LastName = user.LastName;
            userToSave.Email = user.Email;
            userToSave.IsActive = user.IsActive;

            //_mapper.Map(user, userToSave);


            var isAdmin = userToSave.Roles.Any(x => x.Id == Role.Codes.SscAdmin);

            if (!isAdmin)
            {
                userToSave.Roles.Add(_roleRepository.GetById(Role.Codes.SscAdmin));
            }

            _userRepository.EnsurePersistent(userToSave);

            // invalid the cache for the user that was just given permissions
            _userIdentity.RemoveUserRoleFromCache(Resources.Role_CacheId, userToSave.Id);


            if (_emailPreferencesRepository.GetNullableById(userToSave.Id) == null)
            {
                _emailPreferencesRepository.EnsurePersistent(new EmailPreferences(userToSave.Id));
            }

            Message = string.Format("{0} was edited under the SSC administrator role", user.FullNameAndId);

            return this.RedirectToAction(nameof(Index));
        }

        public ActionResult RemoveSscAdmin(string id)
        {
            var user = _userRepository.GetNullableById(id);

            if (user == null)
            {
                ErrorMessage = string.Format("User {0} not found.", id);
                return this.RedirectToAction(nameof(Index));
            }

            if (_userIdentity.IsUserInRole(id, Role.Codes.SscAdmin) == false)
            {
                Message = id + " is not an SSC admin";
                return this.RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult RemoveSscAdminRole(string id)
        {
            var user = _userRepository.GetNullableById(id);
            if (user == null)
            {
                ErrorMessage = string.Format("User {0} not found.", id);
                return this.RedirectToAction(nameof(Index));
            }

            var adminRole = user.Roles.Where(x => x.Id == Role.Codes.SscAdmin).Single();

            user.Roles.Remove(adminRole);

            _userRepository.EnsurePersistent(user);

            // invalid the cache for the user that was just given permissions
            _userIdentity.RemoveUserRoleFromCache(Resources.Role_CacheId, user.Id);

            Message = user.FullNameAndId + " was successfully removed from the SSC admin role";

            return this.RedirectToAction(nameof(Index));
        }



        /// <summary>
        /// Note, post of this method is RemoveAdminRole
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RemoveAdmin(string id)
        {
            var user = _userRepository.GetNullableById(id);

            if (user == null)
            {
                ErrorMessage = string.Format("User {0} not found.", id);
                return this.RedirectToAction(nameof(Index));
            }

            if (_userIdentity.IsUserInRole(id, Role.Codes.Admin) == false)
            {
                Message = id + " is not an admin";
                return this.RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult RemoveAdminRole(string id)
        {
            var user = _userRepository.GetNullableById(id);
            if (user == null)
            {
                ErrorMessage = string.Format("User {0} not found.", id);
                return this.RedirectToAction(nameof(Index));
            }

            var adminRole = user.Roles.Where(x => x.Id == Role.Codes.Admin).Single();

            user.Roles.Remove(adminRole);

            _userRepository.EnsurePersistent(user);

            // invalid the cache for the user that was just given permissions
            _userIdentity.RemoveUserRoleFromCache(Resources.Role_CacheId, user.Id);

            Message = user.FullNameAndId + " was successfully removed from the admin role";

            return this.RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Note post method of this is RemoveDepartmentalRole
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RemoveDepartmental(string id)
        {
            var user = _userRepository.GetNullableById(id);
            if (user == null)
            {
                ErrorMessage = string.Format("User {0} not found.", id);
                return this.RedirectToAction(nameof(Index));
            }

            if (_userIdentity.IsUserInRole(id, Role.Codes.DepartmentalAdmin, true) == false)
            {
                Message = id + " is not a departmental admin";
                return this.RedirectToAction(nameof(Index));
            }

            user.Organizations.ToList(); //pull in the orgs

            return View(user);
        }



        [HttpPost]
        public ActionResult RemoveDepartmentalRole(string id)
        {
            var user = _userRepository.GetNullableById(id);
            if (user == null)
            {
                ErrorMessage = string.Format("User {0} not found.", id);
                return this.RedirectToAction(nameof(Index));
            }
            var adminRole = user.Roles.Where(x => x.Id == Role.Codes.DepartmentalAdmin).Single();

            user.Roles.Remove(adminRole);
            user.Organizations.Clear();

            _userRepository.EnsurePersistent(user);

            // invalid the cache for the user that was just given permissions
            _userIdentity.RemoveUserRoleFromCache(Resources.Role_CacheId, user.Id);

            Message = user.FullNameAndId + " was successfully removed from the departmental admin role";

            return this.RedirectToAction(nameof(Index));
        }

        public ActionResult Clone(string id)
        {
            var userToClone = _userRepository.GetNullableById(id);
            if (userToClone == null)
            {
                ErrorMessage = string.Format("User {0} not found.", id);
                return this.RedirectToAction(nameof(Index));
            }

            var newUser = new User { Organizations = userToClone.Organizations.ToList(), IsActive = true };

            var model = new DepartmentalAdminModel
            {
                User = newUser
            };

            Message =
                string.Format(
                    "Please enter the new user's information. Department associations for {0} have been selected by default.",
                    userToClone.FullNameAndId);

            //Using the modify departmental since it already has the proper logic
            return View("ModifyDepartmental", model);
        }

        /// <summary>
        /// #13 Get a list of Admin workgroups so their child workgroups can be updated.
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateChildWorkgroups()
        {
            return View(_repositoryFactory.WorkgroupRepository.Queryable.Where(a => a.IsActive && a.Administrative).ToList());
        }

        /// <summary>
        /// #14
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonNetResult ProcessWorkGroup(int id)
        {
            var success = true;
            var message = "Updated";
            try
            {
                var workgroup = _repositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == id);
                Check.Require(workgroup.Administrative);
                Check.Require(workgroup.IsActive);

                _workgroupService.AddRelatedAdminUsers(workgroup);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return new JsonNetResult(new { success, message });
        }

        /// <summary>
        /// #15
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonNetResult GetChildWorkgroupIds(int id)
        {
            var success = true;
            var message = "Updated";
            try
            {
                var workgroup = _repositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == id);
                Check.Require(workgroup.Administrative);
                Check.Require(workgroup.IsActive);

                var sb = new StringBuilder();
                var ids = _workgroupService.GetChildWorkgroups(id);
                if (ids != null && ids.Any())
                {
                    ids.Sort();
                    foreach (var childIds in ids)
                    {
                        sb.Append(" " + childIds);
                    }
                    message = sb.ToString();
                }
                else
                {
                    message = "None";
                }

            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return new JsonNetResult(new { success, message });
        }

        /// <summary>
        /// #16
        /// </summary>
        /// <returns></returns>
        public ActionResult ValidateChildWorkgroups()
        {
            //Example for comparing lists
            //var list1 = new[] {new {id = 1, name = "One"}}.ToList();
            //var list2 = new[] { new { id = 1, name = "One" } }.ToList();

            //list1.Add(new {id = 3, name = "Three"});
            //list1.Add(new { id = 4, name = "Four" });
            //list1.Add(new { id = 5, name = "Five" });

            //list2.Add(new { id = 2, name = "Two" });
            //list2.Add(new {id = 3, name = "Three"});
            //list2.Add(new { id = 4, name = "Four" });

            //var list2IsMissing = list1.Where(a => !list2.Any(b => b.id == a.id)); //5
            //var list1IsMissing = list2.Where(a => !list1.Any(b => b.id == a.id)); //2

            var view = new List<ValidateChildWorkgroupsViewModel>();

            var childWorkGroups = _repositoryFactory.WorkgroupRepository.Queryable.Where(a => a.IsActive && !a.Administrative);
            foreach (var childWorkGroup in childWorkGroups)
            {
                var parentWorkGroupIds = _workgroupService.GetParentWorkgroups(childWorkGroup.Id);
                var parentPermissions = _repositoryFactory.WorkgroupPermissionRepository.Queryable.Where(a => parentWorkGroupIds.Contains(a.Workgroup.Id)).Select(s => new { s.Id, role = s.Role.Id, user = s.User.Id, parentWorkgroupId = s.Workgroup.Id, s.Workgroup.IsFullFeatured }).ToList();
                var childPermissions = _repositoryFactory.WorkgroupPermissionRepository.Queryable.Where(a => a.Workgroup == childWorkGroup && a.IsAdmin).Select(s => new { s.Id, role = s.Role.Id, user = s.User.Id, parentWorkgroupId = s.ParentWorkgroup.Id, s.IsFullFeatured }).ToList();

                var missingChildPermissions = parentPermissions.Where(a => !childPermissions.Any(b => b.role == a.role && b.user == a.user && b.parentWorkgroupId == a.parentWorkgroupId && b.IsFullFeatured == a.IsFullFeatured)).ToList();
                var extraChildPermissions = childPermissions.Where(a => !parentPermissions.Any(b => b.role == a.role && b.user == a.user && b.parentWorkgroupId == a.parentWorkgroupId && b.IsFullFeatured == a.IsFullFeatured)).ToList();

                if (missingChildPermissions.Count > 0 || extraChildPermissions.Count > 0)
                {
                    var temp = new ValidateChildWorkgroupsViewModel();
                    temp.ChildWorkgroup = childWorkGroup;
                    temp.ExtraChildPermissions = new List<WorkgroupPermission>();
                    temp.MissingChildPermissions = new List<WorkgroupPermission>();
                    if (missingChildPermissions.Count > 0)
                    {
                        foreach (var missingChildPermission in missingChildPermissions)
                        {
                            temp.MissingChildPermissions.Add(_repositoryFactory.WorkgroupPermissionRepository.Queryable.Single(a => a.Id == missingChildPermission.Id));
                        }
                    }
                    if (extraChildPermissions.Count > 0)
                    {
                        foreach (var extraChildPermission in extraChildPermissions)
                        {
                            temp.ExtraChildPermissions.Add(_repositoryFactory.WorkgroupPermissionRepository.Queryable.Single(a => a.Id == extraChildPermission.Id));
                        }
                    }
                    view.Add(temp);
                }
            }

            return View(view);
        }

        /// <summary>
        /// #20
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public virtual bool NeedToCheckWorkgroupPermissions(string key)
        {
            Log.Information("NeedToCheckWorkgroupPermissions Starting");
            if (string.IsNullOrWhiteSpace(key) || key != _configuration["ValidationKey"])
            {
                Log.Warning("NeedToCheckWorkgroupPermissions Validation Key missing");
                return true;
            }
            var childWorkGroups = _repositoryFactory.WorkgroupRepository.Queryable.Where(a => a.IsActive && !a.Administrative);
            foreach (var childWorkGroup in childWorkGroups)
            {
                var parentWorkGroupIds = _workgroupService.GetParentWorkgroups(childWorkGroup.Id);
                var parentPermissions = _repositoryFactory.WorkgroupPermissionRepository.Queryable.Where(a => parentWorkGroupIds.Contains(a.Workgroup.Id)).Select(s => new { s.Id, role = s.Role.Id, user = s.User.Id, parentWorkgroupId = s.Workgroup.Id, s.Workgroup.IsFullFeatured }).ToList();
                var childPermissions = _repositoryFactory.WorkgroupPermissionRepository.Queryable.Where(a => a.Workgroup == childWorkGroup && a.IsAdmin).Select(s => new { s.Id, role = s.Role.Id, user = s.User.Id, parentWorkgroupId = s.ParentWorkgroup.Id, s.IsFullFeatured }).ToList();

                var missingChildPermissions = parentPermissions.Where(a => !childPermissions.Any(b => b.role == a.role && b.user == a.user && b.parentWorkgroupId == a.parentWorkgroupId && b.IsFullFeatured == a.IsFullFeatured)).ToList();
                var extraChildPermissions = childPermissions.Where(a => !parentPermissions.Any(b => b.role == a.role && b.user == a.user && b.parentWorkgroupId == a.parentWorkgroupId && b.IsFullFeatured == a.IsFullFeatured)).ToList();

                if (missingChildPermissions.Count > 0 || extraChildPermissions.Count > 0)
                {
                    var sgMessage = new MailMessage(
                        new MailAddress("opp-noreply@ucdavis.edu", "OPP No Reply"),
                        new MailAddress("apprequests@caes.ucdavis.edu"))
                    {
                        Subject = "Check Workgroup Permissions",
                        Body = "Run the Check"
                    };

                    Log.Warning("NeedToCheckWorkgroupPermissions Run the Workgroup Permissions Check");

                    var smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                    var credentials = new NetworkCredential(_sendGridSettings.SendGridUserName,
                        _sendGridSettings.SendGridPassword);
                    smtpClient.Credentials = credentials;

                    smtpClient.Send(sgMessage);

                    Log.Information("NeedToCheckWorkgroupPermissions Done (true)");

                    return true;
                }
            }
            Log.Information("NeedToCheckWorkgroupPermissions Done (false)");
            return false;
        }

        /// <summary>
        /// #17
        /// </summary>
        /// <returns></returns>
        public ActionResult RemoveExtraChildPermissions()
        {
            var view = new List<ValidateChildWorkgroupsViewModel>();
            var count = 0;
            var childWorkGroups = _repositoryFactory.WorkgroupRepository.Queryable.Where(a => a.IsActive && !a.Administrative);
            foreach (var childWorkGroup in childWorkGroups)
            {
                var parentWorkGroupIds = _workgroupService.GetParentWorkgroups(childWorkGroup.Id);
                var parentPermissions = _repositoryFactory.WorkgroupPermissionRepository.Queryable.Where(a => parentWorkGroupIds.Contains(a.Workgroup.Id)).Select(s => new { s.Id, role = s.Role.Id, user = s.User.Id, parentWorkgroupId = s.Workgroup.Id, s.Workgroup.IsFullFeatured }).ToList();
                var childPermissions = _repositoryFactory.WorkgroupPermissionRepository.Queryable.Where(a => a.Workgroup == childWorkGroup && a.IsAdmin).Select(s => new { s.Id, role = s.Role.Id, user = s.User.Id, parentWorkgroupId = s.ParentWorkgroup.Id, s.IsFullFeatured }).ToList();

                //var missingChildPermissions = parentPermissions.Where(a => !childPermissions.Any(b => b.role == a.role && b.user == a.user && b.parentWorkgroupId == a.parentWorkgroupId && b.IsFullFeatured == a.IsFullFeatured)).ToList();
                var extraChildPermissions = childPermissions.Where(a => !parentPermissions.Any(b => b.role == a.role && b.user == a.user && b.parentWorkgroupId == a.parentWorkgroupId && b.IsFullFeatured == a.IsFullFeatured)).ToList();


                if (extraChildPermissions.Count > 0)
                {
                    foreach (var extraChildPermission in extraChildPermissions)
                    {
                        var wp = _repositoryFactory.WorkgroupPermissionRepository.GetNullableById(extraChildPermission.Id);
                        if (wp != null)
                        {
                            _repositoryFactory.WorkgroupPermissionRepository.Remove(wp);
                            count++;
                        }
                    }
                }
            }
            Message = string.Format("{0} permissions removed", count);
            return this.RedirectToAction(nameof(ValidateChildWorkgroups));
        }

        /// <summary>
        /// #18
        /// </summary>
        /// <returns></returns>
        public ActionResult TestException()
        {
            throw new Exception("Test -- Test -- Test");
        }

        public async Task<ActionResult> TestAe(string id)
        {
            var isValid  = await _aggieEnterpriseService.ValidateAccount(id);



            return Content(isValid.IsValid.ToString());
        }

        public async Task<ActionResult> TestAe2()
        {
            var rtValue = await _aggieEnterpriseService.LookupOracleErrors("d594a0f3-73f3-41d6-bd02-e122bf98d384");



            return Content("Done");
        }

        public ActionResult TestEmail()
        {
            var sgMessage = new MailMessage(
                new MailAddress("opp-noreply@ucdavis.edu", "OPP No Reply"),
                new MailAddress("opp-tech@ucdavis.edu"))
            {
                Subject = "OPP Test Email from Sendgrid",
                Body = "Just a test"
            };

            var smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
            var credentials = new NetworkCredential(_sendGridSettings.SendGridUserName,
                _sendGridSettings.SendGridPassword);
            smtpClient.Credentials = credentials;

            smtpClient.Send(sgMessage);

            Message = "Test message sent";
            return this.RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Debugging method for when azure isn't returning what is expected
        /// </summary>
        /// <returns></returns>
        public async Task<JsonNetResult> Test(string id)
        {
            //var _searcher =
            //    new GraphSearchClient(
            //        new ActiveDirectoryConfigurationValues(
            //            tenantName: configuration["AzureSearchTenantName"],
            //            tenantId: configuration["AzureSearchTenantId"],
            //            clientId: configuration["AzureSearchClientId"],
            //            clientSecret: configuration["AzureSearchClientSecret"]));

            var _searcher = new IetClient(_configuration["IetWsKey"]);
            if (id != null && id.Contains("@"))
            {
                var ucdContactResult = await _searcher.Contacts.Search(ContactSearchField.email, id);
                if (ucdContactResult.ResponseStatus != 0 || ucdContactResult.ResponseData.Results.Length <= 0)
                {
                    return new JsonNetResult(new IetWsDirectorySearchService.Person());
                }
                var ucdContact = ucdContactResult.ResponseData.Results.First();

                // now look up the whole person's record by ID including kerb
                var ucdKerbResult = await _searcher.Kerberos.Search(KerberosSearchField.iamId, ucdContact.IamId);
                var allTheSame = ucdKerbResult.ResponseData.Results.Select(a => new { a.UserId, a.IamId }).Distinct().ToList();
                if (allTheSame.Count > 1)
                {
                    throw new Exception("More than 1 unique kerb values found.");
                }
                if (ucdKerbResult.ResponseStatus != 0 || ucdKerbResult.ResponseData.Results.Length <= 0)
                {
                    return new JsonNetResult(new IetWsDirectorySearchService.Person());
                }
                var ucdKerbPerson = ucdKerbResult.ResponseData.Results.First();
                var xxx = new IetWsDirectorySearchService.Person

                {
                    GivenName = ucdKerbPerson.DFirstName,
                    Surname = ucdKerbPerson.DLastName,
                    FullName = ucdKerbPerson.DFullName,
                    Kerberos = ucdKerbPerson.UserId,
                    Mail = ucdContact.Email,
                    WorkPhone = ucdContact.WorkPhone

                };
                return new JsonNetResult(xxx);
            }
            else
            {
                var ucdKerbResult = await _searcher.Kerberos.Search(KerberosSearchField.userId, id);
                if (ucdKerbResult.ResponseStatus != 0 || ucdKerbResult.ResponseData.Results.Length <= 0)
                {
                    return new JsonNetResult(new IetWsDirectorySearchService.Person());
                }

                var allTheSame = ucdKerbResult.ResponseData.Results.Select(a => new { a.UserId, a.IamId }).Distinct().ToList();
                if (allTheSame.Count > 1)
                {
                    throw new Exception("More than 1 unique kerb values found.");
                }
                var ucdKerbPerson = ucdKerbResult.ResponseData.Results.First();

                var ucdContactResult = await _searcher.Contacts.Search(ContactSearchField.iamId, ucdKerbPerson.IamId);
                if (ucdContactResult.ResponseStatus != 0 || ucdContactResult.ResponseData.Results.Length <= 0)
                {
                    return new JsonNetResult(new IetWsDirectorySearchService.Person());
                }
                var ucdContact = ucdContactResult.ResponseData.Results.First();
                var xxx = new IetWsDirectorySearchService.Person

                {
                    GivenName = ucdKerbPerson.DFirstName,
                    Surname = ucdKerbPerson.DLastName,
                    FullName = ucdKerbPerson.DFullName,
                    Kerberos = ucdKerbPerson.UserId,
                    Mail = ucdContact.Email,
                    WorkPhone = ucdContact.WorkPhone

                };

                return new JsonNetResult(xxx);
            }

        }



        #region AJAX Helpers
        public JsonNetResult FindUser(string searchTerm)
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

            if (users.Count() == 0)
            {
                return null;
            }
            return new JsonNetResult(users.Select(a => new { id = a.Id, FirstName = a.FirstName, LastName = a.LastName, Email = a.Email, IsActive = a.IsActive }));
        }

        public JsonNetResult SearchOrgs(string searchTerm)
        {
            var orgs = _organizationRepository.Queryable.Where(a => a.Id.Contains(searchTerm) || a.Name.Contains(searchTerm)).OrderBy(o => o.Name);

            return new JsonNetResult(orgs.Select(a => new { id = a.Id, label = string.Format("{0} ({1})", a.Name, a.Id) }));
        }

        #endregion AJAX Helpers
    }

    public class DepartmentalAdminModel
    {
        public User User { get; set; }
        public virtual IEnumerable<Organization> Organizations { get; set; }

        public virtual bool IsSscAdmin { get; set; }
        public virtual bool UpdateAllSscAdmins { get; set; }
    }

    public class AdminListModel
    {
        public IList<User> Admins { get; set; }
        public IList<User> DepartmentalAdmins { get; set; }

        public IList<User> SscAdmins { get; set; }
    }


    public class ValidateChildWorkgroupsViewModel
    {
        public Workgroup ChildWorkgroup { get; set; }
        public List<WorkgroupPermission> MissingChildPermissions { get; set; }
        public List<WorkgroupPermission> ExtraChildPermissions { get; set; }
    }

}
