using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;
using UCDArch.Web.Helpers;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the DepartmentalAdminRequest class
    /// </summary>
    public class DepartmentalAdminRequestController : ApplicationController
    {
	    private readonly IRepository<DepartmentalAdminRequest> _departmentalAdminRequestRepository;
        private readonly IDirectorySearchService _directorySearchService;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;

        public DepartmentalAdminRequestController(IRepository<DepartmentalAdminRequest> departmentalAdminRequestRepository, IRepositoryWithTypedId<User, string> userRepository, IDirectorySearchService directorySearchService )
        {
            _departmentalAdminRequestRepository = departmentalAdminRequestRepository;
            _userRepository = userRepository; 
            _directorySearchService = directorySearchService;
        }

        //
        // GET: /DepartmentalAdminRequest/
        public ActionResult Index()
        {
            var departmentalAdminRequestList = _departmentalAdminRequestRepository.Queryable;

            return View(departmentalAdminRequestList.ToList());
        }

        public ActionResult Create()
        {
            var user = _userRepository.GetNullableById(CurrentUser.Identity.Name);
            DirectoryUser ldap = null;
            if (user == null)
            {
               ldap = _directorySearchService.FindUser(CurrentUser.Identity.Name);    
            }
            Check.Require(user != null || ldap != null, "Person requesting Departmental Access ID not found. ID = " + CurrentUser.Identity.Name);
            DepartmentalAdminRequest model = null;
            if (user != null)
            {
                model = new DepartmentalAdminRequest(user.Id);
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Email = user.Email;
            } else
            {
                model = new DepartmentalAdminRequest(ldap.LoginId.ToLower());
                model.FirstName = ldap.FirstName;
                model.LastName = ldap.LastName;
                model.Email = ldap.EmailAddress;
                model.PhoneNumber = ldap.PhoneNumber;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DepartmentalAdminRequest request, List<string> orgs )
        {
            Check.Require(request != null);
            if (orgs == null || orgs.Count() == 0)
            {
                ErrorMessage = "Must select at least one organization";
                return this.RedirectToAction(x => x.Create());
            }
            var user = _userRepository.GetNullableById(CurrentUser.Identity.Name);
            DirectoryUser ldap = null;
            if (user == null)
            {
                ldap = _directorySearchService.FindUser(CurrentUser.Identity.Name);
            }
            Check.Require(user != null || ldap != null, "Person requesting Departmental Access ID not found. ID = " + CurrentUser.Identity.Name);
            DepartmentalAdminRequest model = null;
            if (user != null)
            {
                request.FirstName = user.FirstName;
                request.LastName = user.LastName;
                request.Email = user.Email;
            }
            else
            {
                request.FirstName = ldap.FirstName;
                request.LastName = ldap.LastName;
                request.Email = ldap.EmailAddress;
            }
            request.DateCreated = DateTime.Now;
            request.Organizations = string.Join(", ", orgs);
            ModelState.Clear();
            request.TransferValidationMessagesTo(ModelState);
            if(ModelState.IsValid)
            {
                _departmentalAdminRequestRepository.EnsurePersistent(request);
                Message = "Request created.";
                return this.RedirectToAction<HomeController>(x => x.Index());
            }
            return View(request);

        }

    }

    
    /// <summary>
    /// ViewModel for the DepartmentalAdminRequest class
    /// </summary>
    public class DepartmentalAdminRequestViewModel
	{
		public DepartmentalAdminRequest DepartmentalAdminRequest { get; set; }
 
		public static DepartmentalAdminRequestViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new DepartmentalAdminRequestViewModel {DepartmentalAdminRequest = new DepartmentalAdminRequest()};
 
			return viewModel;
		}
	}
}
