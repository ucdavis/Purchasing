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
	    private readonly IRepositoryWithTypedId<DepartmentalAdminRequest, string> _departmentalAdminRequestRepository;
        private readonly IRepositoryWithTypedId<Organization, string> _organizationRepository;
        private readonly IDirectorySearchService _directorySearchService;


        public DepartmentalAdminRequestController(IRepositoryWithTypedId<DepartmentalAdminRequest, string> departmentalAdminRequestRepository,IRepositoryWithTypedId<Organization, string> organizationRepository , IDirectorySearchService directorySearchService)
        {
            _departmentalAdminRequestRepository = departmentalAdminRequestRepository;
            _organizationRepository = organizationRepository;
            _directorySearchService = directorySearchService;
        }

        //
        // GET: /DepartmentalAdminRequest/
        public ActionResult Index()
        {
            var departmentalAdminRequestList = _departmentalAdminRequestRepository.Queryable.Where(a => !a.Complete).OrderBy(b => b.DateCreated);

            return View(departmentalAdminRequestList.ToList());
        }

        public ActionResult Create()
        {
            var request = _departmentalAdminRequestRepository.GetNullableById(CurrentUser.Identity.Name);
            var ldap = _directorySearchService.FindUser(CurrentUser.Identity.Name);
            Check.Require(ldap != null, "Person requesting Departmental Access ID not found. ID = " + CurrentUser.Identity.Name);

            var model = DepartmentalAdminRequestViewModel.Create();

            model.DepartmentalAdminRequest = request ?? new DepartmentalAdminRequest(ldap.LoginId.ToLower());
            model.DepartmentalAdminRequest.FirstName = ldap.FirstName;
            model.DepartmentalAdminRequest.LastName = ldap.LastName;
            model.DepartmentalAdminRequest.Email = ldap.EmailAddress;
            model.DepartmentalAdminRequest.PhoneNumber = ldap.PhoneNumber;

            foreach (var orgId in model.DepartmentalAdminRequest.Organizations.Split(','))
            {
                var org = _organizationRepository.GetNullableById(orgId);
                if (org != null)
                {
                    model.Organizations.Add(org);
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(DepartmentalAdminRequestViewModel request, List<string> orgs)
        {
            Check.Require(request != null);
            if (orgs == null || !orgs.Any())
            {
                ErrorMessage = "Must select at least one organization";
                return this.RedirectToAction(x => x.Create());
            }
           
            var ldap = _directorySearchService.FindUser(CurrentUser.Identity.Name);
           
            Check.Require(ldap != null, "Person requesting Departmental Access ID not found. ID = " + CurrentUser.Identity.Name);

            var requestToSave = _departmentalAdminRequestRepository.GetNullableById(request.DepartmentalAdminRequest.Id) ?? new DepartmentalAdminRequest(request.DepartmentalAdminRequest.Id);
            requestToSave.RequestCount++;
            requestToSave.FirstName = ldap.FirstName;
            requestToSave.LastName = ldap.LastName;
            requestToSave.Email = ldap.EmailAddress;
            requestToSave.DateCreated = DateTime.Now;
            requestToSave.Organizations = string.Join(",", orgs);
            requestToSave.PhoneNumber = request.DepartmentalAdminRequest.PhoneNumber;
            requestToSave.SharedOrCluster = request.DepartmentalAdminRequest.SharedOrCluster;
            requestToSave.DepartmentSize = request.DepartmentalAdminRequest.DepartmentSize;
            
            ModelState.Clear();
            requestToSave.TransferValidationMessagesTo(ModelState);            

            if(ModelState.IsValid)
            {
                _departmentalAdminRequestRepository.EnsurePersistent(requestToSave);
                Message = "Request created.";
                return this.RedirectToAction<HomeController>(x => x.Index());
            }

            ErrorMessage = "There were Errors, please correct and try again.";
            request.DepartmentalAdminRequest = requestToSave;
            return View(request);

        }

    }

    
    /// <summary>
    /// ViewModel for the DepartmentalAdminRequest class
    /// </summary>
    public class DepartmentalAdminRequestViewModel
	{
		public DepartmentalAdminRequest DepartmentalAdminRequest { get; set; }
        public IList<Organization> Organizations { get; set; } 
 
		public static DepartmentalAdminRequestViewModel Create()
		{
			
			var viewModel = new DepartmentalAdminRequestViewModel {DepartmentalAdminRequest = new DepartmentalAdminRequest()};
            viewModel.Organizations = new List<Organization>();
 
			return viewModel;
		}
	}
}
