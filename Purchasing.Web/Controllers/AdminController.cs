using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using Purchasing.Core.Domain;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using MvcContrib;
using UCDArch.Web.Providers;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Admin class
    /// </summary>
    [Authorize(Roles = Role.Codes.Admin)]
    public class AdminController : ApplicationController
    {
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<Role, string> _roleRepository;
        private readonly IRepositoryWithTypedId<Organization, string> _organizationRepository;
        private readonly IDirectorySearchService _searchService;
        private readonly IRepositoryWithTypedId<EmailPreferences, string> _emailPreferencesRepository;
        private readonly IUserIdentity _userIdentity;

        public AdminController(IRepositoryWithTypedId<User, string> userRepository, IRepositoryWithTypedId<Role, string> roleRepository, IRepositoryWithTypedId<Organization,string> organizationRepository, IDirectorySearchService searchService, IRepositoryWithTypedId<EmailPreferences, string> emailPreferencesRepository, IUserIdentity userIdentity )
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _organizationRepository = organizationRepository;
            _searchService = searchService;
            _emailPreferencesRepository = emailPreferencesRepository;
            _userIdentity = userIdentity;
        }

        //
        // GET: /Admin/
        public ActionResult Index()
        {
            var admins = _roleRepository.Queryable.Where(x => x.Name.EndsWith("Admin")).Fetch(x => x.Users).ToList();

            var model = new AdminListModel()
                            {
                                Admins = admins.Single(x => x.Id == Role.Codes.Admin).Users.Where(x=>x.IsActive).ToList(),
                                DepartmentalAdmins = admins.Single(x => x.Id == Role.Codes.DepartmentalAdmin).Users.Where(x=>x.IsActive).ToList()
                            };

            return View(model);
        }

        public ActionResult ModifyDepartmental(string id)
        {
            var user = _userRepository.Queryable.Where(x => x.Id == id).Fetch(x => x.Organizations).SingleOrDefault() ??
                       new User(null) {IsActive = true};

            var model = new DepartmentalAdminModel
                            {
                                User = user
                            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ModifyDepartmental(DepartmentalAdminModel departmentalAdminModel, List<string> orgs)
        {
            if(orgs == null || orgs.Count == 0)
            {
                ModelState.AddModelError("User.Organizations", "You must select at least one department for a departmental Admin.");
            }
            if (!ModelState.IsValid)
            {
                return View(departmentalAdminModel);
            }

            var user = _userRepository.GetNullableById(departmentalAdminModel.User.Id) ?? new User(departmentalAdminModel.User.Id);


            departmentalAdminModel.User.Roles = user.Roles;

            //Mapper.Map(departmentalAdminModel.User, user); // This was causing problems if an existing DA was saved.
            user.FirstName = departmentalAdminModel.User.FirstName;
            user.LastName = departmentalAdminModel.User.LastName;
            user.Email = departmentalAdminModel.User.Email;
            user.IsActive = departmentalAdminModel.User.IsActive;

            var isDeptAdmin = user.Roles.Any(x => x.Id == Role.Codes.DepartmentalAdmin);
            
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


            Message = string.Format("{0} was added as a departmental admin to the specified organization(s)",
                                    user.FullNameAndId);

            return this.RedirectToAction(a => a.Index());
        }

        public ActionResult ModifyAdmin(string id)
        {
            var user = (!string.IsNullOrWhiteSpace(id) ? _userRepository.GetNullableById(id)  : new User(null) { IsActive = true }) ??
                       new User(null) {IsActive = true};

            return View(user);
        }

        [HttpPost]
        public ActionResult ModifyAdmin(User user)
        {
            if(!ModelState.IsValid)
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

            //Mapper.Map(user, userToSave);


            var isAdmin = userToSave.Roles.Any(x => x.Id == Role.Codes.Admin);

            if(!isAdmin)
            {
                userToSave.Roles.Add(_roleRepository.GetById(Role.Codes.Admin));
            }

            _userRepository.EnsurePersistent(userToSave);

            // invalid the cache for the user that was just given permissions
            _userIdentity.RemoveUserRoleFromCache(Resources.Role_CacheId, userToSave.Id);
            

            if(_emailPreferencesRepository.GetNullableById(userToSave.Id) == null)
            {
                _emailPreferencesRepository.EnsurePersistent(new EmailPreferences(userToSave.Id));
            }

            Message = string.Format("{0} was edited under the administrator role", user.FullNameAndId);

            return this.RedirectToAction(a => a.Index());
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
                return this.RedirectToAction(a => a.Index());
            }
            
            if (_userIdentity.IsUserInRole(id, Role.Codes.Admin) == false)
            {
                Message = id + " is not an admin";
                return this.RedirectToAction(a => a.Index());
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
                return this.RedirectToAction(a => a.Index());
            }

            var adminRole = user.Roles.Where(x => x.Id == Role.Codes.Admin).Single();

            user.Roles.Remove(adminRole);

            _userRepository.EnsurePersistent(user);

            // invalid the cache for the user that was just given permissions
            _userIdentity.RemoveUserRoleFromCache(Resources.Role_CacheId, user.Id);

            Message = user.FullNameAndId + " was successfully removed from the admin role";

            return this.RedirectToAction(a => a.Index());
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
                return this.RedirectToAction(a => a.Index());
            }

            if (_userIdentity.IsUserInRole(id, Role.Codes.DepartmentalAdmin) == false)
            {
                Message = id + " is not a departmental admin";
                return this.RedirectToAction(a=> a.Index());
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
                return this.RedirectToAction(a => a.Index());
            }
            var adminRole = user.Roles.Where(x => x.Id == Role.Codes.DepartmentalAdmin).Single();

            user.Roles.Remove(adminRole);
            user.Organizations.Clear(); 

            _userRepository.EnsurePersistent(user);

            // invalid the cache for the user that was just given permissions
            _userIdentity.RemoveUserRoleFromCache(Resources.Role_CacheId, user.Id);

            Message = user.FullNameAndId + " was successfully removed from the departmental admin role";

            return RedirectToAction("Index");
        }

        public ActionResult Clone(string id)
        {
            var userToClone = _userRepository.GetNullableById(id);

            var newUser = new User {Organizations = userToClone.Organizations.ToList(), IsActive = true};

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

        #region AJAX Helpers
        public JsonNetResult FindUser(string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Trim();

            var users = _userRepository.Queryable.Where(a => a.Email == searchTerm || a.Id == searchTerm).ToList();
            if(users.Count == 0)
            {
                var ldapuser = _searchService.FindUser(searchTerm);
                if(ldapuser != null)
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

            if(users.Count() == 0)
            {
                return null;
            }
            return new JsonNetResult(users.Select(a => new { id = a.Id, FirstName = a.FirstName, LastName = a.LastName, Email = a.Email, IsActive = a.IsActive }));
        } 

        public JsonNetResult SearchOrgs(string searchTerm)
        {
            var orgs = _organizationRepository.Queryable.Where(a => a.Id.Contains(searchTerm) || a.Name.Contains(searchTerm)).OrderBy(o => o.Name);

            return new JsonNetResult(orgs.Select(a => new {id = a.Id, label = string.Format("{0} ({1})", a.Name, a.Id) }));
        }

        #endregion AJAX Helpers
    }

    public class DepartmentalAdminModel
    {
        public User User { get; set; }
        public virtual IEnumerable<Organization> Organizations { get; set; }
    }

    public class AdminListModel
    {
        public IList<User> Admins { get; set; }
        public IList<User> DepartmentalAdmins { get; set; }
    }


}
