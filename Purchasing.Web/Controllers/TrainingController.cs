using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Helpers;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Training class
    /// </summary>
    [Authorize]
    public class TrainingController : ApplicationController
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IDirectorySearchService _directorySearchService;
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;
        private readonly IUserIdentity _userIdentity;

        private readonly string[] _fakeUsers = new []{"pjfry", "awong", "hconrad"};

        public TrainingController(IRepositoryFactory repositoryFactory, IDirectorySearchService directorySearchService, IQueryRepositoryFactory queryRepositoryFactory, IUserIdentity userIdentity)
        {
            _repositoryFactory = repositoryFactory;
            _directorySearchService = directorySearchService;
            _queryRepositoryFactory = queryRepositoryFactory;
            _userIdentity = userIdentity;
        }

        // GET: /Training/
        public ActionResult Index()
        {
            var users = _repositoryFactory.UserRepository.Queryable.Where(a => a.WorkgroupPermissions.Any() && !_fakeUsers.Contains(a.Id));

            return View(users);
        }

        public ActionResult Wipe()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Wipe(string nothing)
        {
            TrainingDbHelper.ResetDatabase();

            return Redirect("Index");
        }

        public ActionResult Setup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Setup(string role, List<TrainingSetupPostModel> postModel)
        {
            var users = new List<User>();
            var notFound = new List<string>();

            foreach (var pm in postModel)
            {
                if (pm.HasValues)
                {
                    var result = _directorySearchService.FindUser(pm.UserId);

                    if (result != null)
                    {
                        var user = new User(pm.UserId) { FirstName = result.FirstName, LastName = result.LastName, Email = result.EmailAddress, IsActive = true };

                        // get the orgs
                        var org = _repositoryFactory.OrganizationRepository.GetNullableById(pm.OrgId);
                        if (org != null) user.Organizations.Add(org);

                        users.Add(user);
                    }
                    else
                    {
                        notFound.Add(pm.UserId);
                    }
                }
            }

            if (role == "DA")
            {
                TrainingDbHelper.ConfigureAdmin(users);
                foreach (var user in users)
                {
                    _userIdentity.RemoveUserRoleFromCache(Resources.Role_CacheId, user.Id);
                }
            }
            else
            {
                TrainingDbHelper.ConfigureDatabase(role, users);
            }
            

            SetServiceMessage(role);

            if (notFound.Any())
            {
                Message = string.Format("The following user(s) where not found: {0}", string.Join(", ", notFound));
            }

            return Redirect("Index");
        }

        private const string CacheKey = "ServiceMessage";
        private void SetServiceMessage(string role)
        {
            // invalidate the cache
            System.Web.HttpContext.Current.Cache.Remove(CacheKey);

            // deactivate any existing ones   
            var sms = _repositoryFactory.ServiceMessageRepository.Queryable.Where(a => a.Message.Contains("Training session for") && a.IsActive).ToList();
            foreach (var s in sms)
            {
                s.IsActive = false;
                _repositoryFactory.ServiceMessageRepository.EnsurePersistent(s);
            }

            string rolename = string.Empty;
            switch (role)
            {
                case Role.Codes.Requester: rolename = "Requester"; break;
                case Role.Codes.Approver: rolename = "Approver"; break;
                case Role.Codes.AccountManager: rolename = "Account Manager"; break;
                case Role.Codes.Purchaser: rolename = "Purchaser"; break;
            }
            // insert the training message
            var sm = new ServiceMessage()
            {
                Message = string.Format("Training session for {0}", rolename)
            };
            _repositoryFactory.ServiceMessageRepository.EnsurePersistent(sm);

            
        }

        public JsonNetResult GetOrderCount(string userId)
        {
            var ordercount = _queryRepositoryFactory.AccessRepository.Queryable.Count(a => a.AccessUserId == userId);

            return new JsonNetResult(ordercount);
        }

    }

	/// <summary>
    /// ViewModel for the Training class
    /// </summary>
    public class TrainingViewModel
	{
		public static TrainingViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new TrainingViewModel {};
 
			return viewModel;
		}
	}

    public class TrainingSetupPostModel
    {
        public string UserId { get; set; }
        public string OrgId { get; set; }

        public bool HasValues { get { return !string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(OrgId); } }
    }
}
