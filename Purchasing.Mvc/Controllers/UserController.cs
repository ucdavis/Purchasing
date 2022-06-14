using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Purchasing.Core;
using Purchasing.Core.Helpers;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Models;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Helpers;
using Purchasing.Mvc.Services;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// Controller for the User class
    /// </summary>
    [AuthorizeApplicationAccess]
    public class UserController : ApplicationController
    {
	    private readonly IRepositoryWithTypedId<User,string> _userRepository;
        private readonly IRepositoryWithTypedId<EmailPreferences, string> _emailPreferencesRepository;
        private readonly IRepositoryWithTypedId<ColumnPreferences, string> _columnPreferencesRepository;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;

        private readonly IDirectorySearchService _searchService;

        public UserController(IRepositoryWithTypedId<User, string> userRepository, IRepositoryWithTypedId<EmailPreferences, string> emailPreferencesRepository, IRepositoryWithTypedId<ColumnPreferences, string> columnPreferencesRepository, IRepositoryFactory repositoryFactory, IQueryRepositoryFactory queryRepositoryFactory, IDirectorySearchService searchService )
        {
            _userRepository = userRepository;
            _emailPreferencesRepository = emailPreferencesRepository;
            _columnPreferencesRepository = columnPreferencesRepository;
            _repositoryFactory = repositoryFactory;
            _queryRepositoryFactory = queryRepositoryFactory;
            _searchService = searchService;
        }

        //
        // GET: /User/Profile
        /// <summary>
        /// Return the profile for the current user
        /// </summary>
        /// <returns></returns>
        public ActionResult Profile()
        {
            var user = GetCurrent();

            if (user == null)
            {
                ErrorMessage = "You do not have an account in this system.  Please contact application support.";
                return RedirectToAction("Index", "Home");
            }
            
            return View(user);
        }

        [HttpPost]
        public ActionResult UpdateFromIAM()
        {
            var user = GetCurrent();

            if (user == null)
            {
                ErrorMessage = "You do not have an account in this system.  Please contact application support.";
                return RedirectToAction("Index", "Home");
            }

            var iamUser = _searchService.SearchUsers(user.Id).First();
            if (iamUser.LoginId != user.Id)
            {
                throw new Exception("IAM user login id does not match user id");
            }
            user.FirstName = iamUser.FirstName;
            user.LastName = iamUser.LastName;
            user.Email = string.IsNullOrWhiteSpace(iamUser.EmailAddress) ? user.Email : iamUser.EmailAddress;

            _userRepository.EnsurePersistent(user);
            Message = "User updated from Campus Source";

            return RedirectToAction("Profile");
        }

        public ActionResult EmailPreferences(string id)
        {
            var userEmailPreferences = _emailPreferencesRepository.GetNullableById(id) ?? new EmailPreferences(id);
            var user = _userRepository.Queryable.Single(a => a.Id.ToLower() == id.ToLower());

            ViewBag.IsConditionalApprover =
                Repository.OfType<ConditionalApproval>().Queryable.Any(
                    a => a.PrimaryApprover == user || a.SecondaryApprover == user);

            return View(userEmailPreferences);
        }

        [HttpPost]
        public ActionResult EmailPreferences(EmailPreferences emailPreferences)
        {
            Check.Require(emailPreferences.Id == CurrentUser.Identity.Name,
                          string.Format("User {0} attempted to save the email preferences for {1}",
                                        CurrentUser.Identity.Name, emailPreferences.Id));

            if (!ModelState.IsValid)
            {
                return View(emailPreferences);
            }

            Message = "Your email preferences have been updated";

            _emailPreferencesRepository.EnsurePersistent(emailPreferences);

            return RedirectToAction("Profile");
        }

        public ActionResult EditEmail()
        {
            var user = GetCurrent();

            return View(user);
        }

        [HttpPost]
        public ActionResult EditEmail(User user)
        {
            if (user.Id.ToLower() != CurrentUser.Identity.Name.ToLower())
            {
                return this.RedirectToAction(nameof(ErrorController.NotAuthorized), typeof(ErrorController).ControllerName());
            }

            var userToEdit = GetCurrent();
            userToEdit.Email = user.Email.ToLower();

            ModelState.Clear();
            userToEdit.TransferValidationMessagesTo(ModelState);

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Unable to Save";
                return View(user);
            }
           
            _userRepository.EnsurePersistent(userToEdit);
            Message = "Email Updated";

            return this.RedirectToAction(nameof(Profile));

        }

        public ActionResult AwayStatus(string id)
        {
            var user = GetCurrent();

            return View(user);
        }
        
        [HttpPost]
        public ActionResult AwayStatus(User user)
        {
            var currentUser = GetCurrent();
            currentUser.AwayUntil = user.AwayUntil;

            Message = "Your away status has been set";

            _userRepository.EnsurePersistent(currentUser);

            return RedirectToAction("Profile");
        }

        public ActionResult ColumnPreferences(string id, bool fromList = false)
        {
            var columnPreferences = _columnPreferencesRepository.GetNullableById(id) ?? new ColumnPreferences(id);
            if(fromList)
            {
                ViewBag.FromList = true;
            }
            else
            {
                ViewBag.FromList = false;
            }
            return View(columnPreferences);
        }

        [HttpPost]
        public ActionResult ColumnPreferences(ColumnPreferences columnPreferences, bool fromList = false)
        {
            Check.Require(columnPreferences.Id == CurrentUser.Identity.Name,
                         string.Format("User {0} attempted to save the column preferences for {1}",
                                       CurrentUser.Identity.Name, columnPreferences.Id));

            if (!ModelState.IsValid)
            {
                return View(columnPreferences);
            }

            Message = "Your column preferences have been updated";

            _columnPreferencesRepository.EnsurePersistent(columnPreferences);
            
            return fromList ? RedirectToAction("Index", "History") : RedirectToAction("Profile");
        }

        [HttpPost]
        public JsonNetResult SetAwayStatus(string userId, DateTime awayUntil)
        {
            var user = _userRepository.GetNullableById(userId);

            if (user == null) return new JsonNetResult(null);

            user.AwayUntil = awayUntil;
            _userRepository.EnsurePersistent(user);

            return new JsonNetResult(awayUntil.Date > DateTime.UtcNow.ToPacificTime().Date);
        }

        /// <summary>
        /// Displays contact information for a user, so they can find out why they have access
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Contact()
        {
            var user = _repositoryFactory.UserRepository.GetNullableById(CurrentUser.Identity.Name);
            var viewModel = ContactViewModel.Create(_repositoryFactory, _queryRepositoryFactory, user);

            return View(viewModel);
        }

        private User GetCurrent()
        {
            return _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).SingleOrDefault();
        }
    }
}