﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
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

        private readonly string[] _fakeUsers = new []{"pjfry", "awong", "hconrad"};

        public TrainingController(IRepositoryFactory repositoryFactory, IDirectorySearchService directorySearchService, IQueryRepositoryFactory queryRepositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
            _directorySearchService = directorySearchService;
            _queryRepositoryFactory = queryRepositoryFactory;
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
            //users.Add(new User("anlai") {FirstName = "Alan", LastName = "Last", Email = "anlai@ucdavis.edu", IsActive = true});

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
                }
            }

            TrainingDbHelper.ConfigureDatabase(role, users);

            return Redirect("Index");
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
