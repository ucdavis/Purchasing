using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Helpers;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the DepartmentalAdminRequest class
    /// </summary>
    public class DepartmentalAdminRequestController : ApplicationController
    {
	    private readonly IRepositoryWithTypedId<DepartmentalAdminRequest, string> _departmentalAdminRequestRepository;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;
        private readonly IDirectorySearchService _directorySearchService;
        private readonly IUserIdentity _userIdentity;


        public DepartmentalAdminRequestController(IRepositoryWithTypedId<DepartmentalAdminRequest, string> departmentalAdminRequestRepository, IRepositoryFactory repositoryFactory, IQueryRepositoryFactory queryRepositoryFactory, IDirectorySearchService directorySearchService, IUserIdentity userIdentity)
        {
            _departmentalAdminRequestRepository = departmentalAdminRequestRepository;
            _repositoryFactory = repositoryFactory;
            _queryRepositoryFactory = queryRepositoryFactory;
            _directorySearchService = directorySearchService;
            _userIdentity = userIdentity;
        }

        /// <summary>
        /// GET: /DepartmentalAdminRequest/
        /// #1
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Codes.Admin)]
        public ActionResult Index(string filter = null)
        {
            var departmentalAdminRequestList = _departmentalAdminRequestRepository.Queryable;
            if(filter == null)
            {
                departmentalAdminRequestList = departmentalAdminRequestList.Where(a => !a.Complete).OrderBy(b => b.DateCreated);                
            }
            else
            {
                switch (filter)
                {
                    case "showAll":
                        departmentalAdminRequestList = departmentalAdminRequestList.OrderBy(b => b.DateCreated);
                        break;
                    case "onlyShowComplete":
                        departmentalAdminRequestList = departmentalAdminRequestList.Where(a => a.Complete).OrderBy(b => b.DateCreated);
                        break;
                    default:
                        Message = "Filter not found. Default only Active.";
                        departmentalAdminRequestList = departmentalAdminRequestList.Where(a => !a.Complete).OrderBy(b => b.DateCreated);
                        break;

                }
            }
                

            return View(departmentalAdminRequestList.ToList());
        }

        /// <summary>
        /// #2
        /// </summary>
        /// <returns></returns>
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
            if (model.DepartmentalAdminRequest.Organizations != null && model.DepartmentalAdminRequest.Organizations.Any())
            {
                foreach (var orgId in model.DepartmentalAdminRequest.Organizations.Split(','))
                {
                    var org = _repositoryFactory.OrganizationRepository.GetNullableById(orgId);
                    if (org != null)
                    {
                        model.Organizations.Add(org);
                    }
                }
            }

            return View(model);
        }

        /// <summary>
        /// #3
        /// </summary>
        /// <param name="request"></param>
        /// <param name="orgs"></param>
        /// <returns></returns>
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

            var requestToSave = _departmentalAdminRequestRepository.GetNullableById(CurrentUser.Identity.Name.ToLower()) ?? new DepartmentalAdminRequest(CurrentUser.Identity.Name.ToLower());
            requestToSave.RequestCount++;
            requestToSave.Complete = false;
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

                //TODO: Generate email to either all admins or a specific person that a DA request has arrived

                Message = "Request created.";
                return this.RedirectToAction<HomeController>(x => x.Index());
            }

            ErrorMessage = "There were Errors, please correct and try again.";
            request.DepartmentalAdminRequest = requestToSave;
            return View(request);

        }

        /// <summary>
        /// #4
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Codes.Admin)]
        public ActionResult Approve(string id)
        {
            var daRequest = _departmentalAdminRequestRepository.GetNullableById(id);
            if (daRequest == null)
            {
                ErrorMessage = "Request not found";
                return this.RedirectToAction(a => a.Index(null));
            }
            if (daRequest.Complete)
            {
                Message = "Request was already completed";
                return this.RedirectToAction(a => a.Index(null));
            }

            var model = DepartmentalAdminRequestViewModel.Create();
            model.DepartmentalAdminRequest = daRequest;

            var user = _repositoryFactory.UserRepository.GetNullableById(id);
            if (user == null)
            {
                model.UserExists = false;
                model.UserIsAlreadyDA = false;
            }
            else
            {
                model.UserExists = true;
                if (user.Roles.Any(a => a.Id == Role.Codes.DepartmentalAdmin))
                {
                    model.UserIsAlreadyDA = true;
                }
                else
                {
                    model.UserIsAlreadyDA = false;
                }
            }

            var requestedOrgIds = model.DepartmentalAdminRequest.Organizations != null ? model.DepartmentalAdminRequest.Organizations.Split(',').ToList() : new List<string>();

            foreach (var orgId in requestedOrgIds)
            {
                var org = _repositoryFactory.OrganizationRepository.GetNullableById(orgId);
                if (org != null)
                {
                    model.Organizations.Add(org);
                }
            }

            model.ExistingOrganizations = new List<Organization>();
            if (model.UserIsAlreadyDA)
            {
                model.ExistingOrganizations = user.Organizations;
            }

            model.OrgsExistingUsers = new List<KeyValuePair<string, string>>();
            foreach (var organization in model.Organizations)
            {
                Organization organization1 = organization;
                var users =
                    _repositoryFactory.UserRepository.Queryable.Where(a => a.Organizations.Contains(organization1)).Select(b => b.Email).ToList();
                foreach (var userEmail in users)
                {
                    model.OrgsExistingUsers.Add(new KeyValuePair<string, string>(organization.Id, userEmail));
                }
            }


            // Find Children DA Users.
            var childOrgIds = _queryRepositoryFactory.OrganizationDescendantRepository.Queryable.Where(a => requestedOrgIds.Contains(a.RollupParentId)).Select(b => b.OrgId).Distinct().ToList();
            var childOrganizations = new List<Organization>();
            foreach (var orgId in childOrgIds)
            {
                var org = _repositoryFactory.OrganizationRepository.GetNullableById(orgId);
                if (org != null)
                {
                    childOrganizations.Add(org);
                }
            }

            model.ChildOrgsExistingUsers = new List<KeyValuePair<string, string>>();
            model.OrganizationsWhithChildUsers = new List<Organization>();
            foreach (var organization in childOrganizations)
            {
                Organization organization1 = organization;
                var users = _repositoryFactory.UserRepository.Queryable.Where(a => a.Organizations.Contains(organization1) && a.Id != daRequest.Id).Select(b => b.Email).ToList();
                if (users.Count > 0)
                {
                    model.OrganizationsWhithChildUsers.Add(organization1);
                }
                foreach (var userEmail in users)
                {
                    model.ChildOrgsExistingUsers.Add(new KeyValuePair<string, string>(organization.Id, userEmail));
                }
            }

            // Find Parent DA Users.
            var parentOrgIds = _queryRepositoryFactory.OrganizationDescendantRepository.Queryable.Where(a => requestedOrgIds.Contains(a.OrgId)).Select(b => b.RollupParentId).Distinct().ToList();
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
                var users = _repositoryFactory.UserRepository.Queryable.Where(a => a.Organizations.Contains(organization1) && a.Id != daRequest.Id).Select(b => b.Email).ToList();
                if (users.Count > 0)
                {
                    model.OrganizationsWhithParentUsers.Add(organization1);
                }
                foreach (var userEmail in users)
                {
                    model.ParentOrgsExistingUsers.Add(new KeyValuePair<string, string>(organization.Id, userEmail));
                }
            }

            model.AllExistingUsers =
                model.ParentOrgsExistingUsers.Select(a => a.Value).Union(
                    model.ChildOrgsExistingUsers.Select(b => b.Value)).Distinct().ToList();


            return View(model);
        }

        /// <summary>
        /// #5
        /// </summary>
        /// <param name="request"></param>
        /// <param name="orgs"></param>
        /// <param name="existingOrgs"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Role.Codes.Admin)]
        public ActionResult Approve(DepartmentalAdminRequestViewModel request, List<string> orgs, List<string> existingOrgs)
        {
            var requestToUpdate =
                _departmentalAdminRequestRepository.Queryable.Single(a => a.Id == request.DepartmentalAdminRequest.Id);

            if (orgs == null || !orgs.Any())
            {
                ErrorMessage = "Must select at least one organization";
                return this.RedirectToAction(x => x.Approve(request.DepartmentalAdminRequest.Id));
            }
            if (existingOrgs == null)
            {
                existingOrgs = new List<string>();
            }

            var user = _repositoryFactory.UserRepository.GetNullableById(request.DepartmentalAdminRequest.Id);
            if (user == null)
            {
                request.UserExists = false;
                user = new User(request.DepartmentalAdminRequest.Id.ToLower());
                user.FirstName = request.DepartmentalAdminRequest.FirstName;
                user.LastName = request.DepartmentalAdminRequest.LastName;
                user.Email = request.DepartmentalAdminRequest.Email;
            }
            else
            {
                request.UserExists = true;
            }
            var newDa = false;
            if (!user.Roles.Any(x => x.Id == Role.Codes.DepartmentalAdmin))
            {
                newDa = true;
                user.Roles.Add(_repositoryFactory.RoleRepository.GetById(Role.Codes.DepartmentalAdmin));
            }

            user.Organizations = new List<Organization>();
            foreach (var org in orgs)
            {
                user.Organizations.Add(_repositoryFactory.OrganizationRepository.Queryable.Single(a => a.Id == org));
            }
            if (request.MergeExistingOrgs)
            {
                foreach (var existingOrg in existingOrgs.Where(existingOrg => !orgs.Contains(existingOrg)))
                {
                    user.Organizations.Add(_repositoryFactory.OrganizationRepository.Queryable.Single(a => a.Id == existingOrg));
                }
            }

            _repositoryFactory.UserRepository.EnsurePersistent(user);
            _userIdentity.RemoveUserRoleFromCache(Resources.Role_CacheId, user.Id);

            requestToUpdate.Complete = true;
            _departmentalAdminRequestRepository.EnsurePersistent(requestToUpdate);

            var updateMessage = "Granted";
            if (!newDa && request.UserExists)
            {
                updateMessage = request.MergeExistingOrgs ? "Updated" : "Replaced";
            }


            //TODO: Generate email notifying user they now have access
            Message = string.Format("{0} {1} Departmental Admin Access", user.FullNameAndId, updateMessage);

            return this.RedirectToAction(a => a.Index(null));
        }

        /// <summary>
        /// #6
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Codes.Admin)]
        public ActionResult Deny(string id)
        {
            var daRequest = _departmentalAdminRequestRepository.GetNullableById(id);
            if (daRequest == null)
            {
                ErrorMessage = "Request not found";
                return this.RedirectToAction(a => a.Index(null));
            }
            if (daRequest.Complete)
            {
                Message = "Request was already completed";
                return this.RedirectToAction(a => a.Index(null));
            }

            var model = DepartmentalAdminRequestViewModel.Create();
            model.DepartmentalAdminRequest = daRequest;

            var user = _repositoryFactory.UserRepository.GetNullableById(id);
            if (user == null)
            {
                model.UserExists = false;
                model.UserIsAlreadyDA = false;
            }
            else
            {
                model.UserExists = true;
                if (user.Roles.Any(a => a.Id == Role.Codes.DepartmentalAdmin))
                {
                    model.UserIsAlreadyDA = true;
                }
                else
                {
                    model.UserIsAlreadyDA = false;
                }
            }

            var requestedOrgIds = model.DepartmentalAdminRequest.Organizations != null ? model.DepartmentalAdminRequest.Organizations.Split(',').ToList() : new List<string>();

            foreach (var orgId in requestedOrgIds)
            {
                var org = _repositoryFactory.OrganizationRepository.GetNullableById(orgId);
                if (org != null)
                {
                    model.Organizations.Add(org);
                }
            }

            model.ExistingOrganizations = new List<Organization>();
            if (model.UserIsAlreadyDA)
            {
                model.ExistingOrganizations = user.Organizations;
            }

            return View(model);
        }

        /// <summary>
        /// #7
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Role.Codes.Admin)]
        public ActionResult Deny(DepartmentalAdminRequestViewModel request)
        {
            var requestToUpdate = _departmentalAdminRequestRepository.Queryable.Single(a => a.Id == request.DepartmentalAdminRequest.Id);
            if (requestToUpdate.Complete)
            {
                Message = "Request was already completed";
                return this.RedirectToAction(a => a.Index(null));
            }
            requestToUpdate.Complete = true;
            _departmentalAdminRequestRepository.EnsurePersistent(requestToUpdate);

            Message = string.Format("Request Denied for {0}", requestToUpdate.FullNameAndId);

            return this.RedirectToAction(a => a.Index(null));
        }

        [Authorize(Roles = Role.Codes.Admin)]
        public ActionResult Details(string id)
        {
            var daRequest = _departmentalAdminRequestRepository.GetNullableById(id);
            if (daRequest == null)
            {
                ErrorMessage = "Request not found";
                return this.RedirectToAction(a => a.Index(null));
            }

            var model = DepartmentalAdminRequestViewModel.Create();
            model.DepartmentalAdminRequest = daRequest;

            var user = _repositoryFactory.UserRepository.GetNullableById(id);
            if (user == null)
            {
                model.UserExists = false;
                model.UserIsAlreadyDA = false;
            }
            else
            {
                model.UserExists = true;
                if (user.Roles.Any(a => a.Id == Role.Codes.DepartmentalAdmin))
                {
                    model.UserIsAlreadyDA = true;
                }
                else
                {
                    model.UserIsAlreadyDA = false;
                }
            }


            var requestedOrgIds = model.DepartmentalAdminRequest.Organizations != null ? model.DepartmentalAdminRequest.Organizations.Split(',').ToList() : new List<string>();

            foreach (var orgId in requestedOrgIds)
            {
                var org = _repositoryFactory.OrganizationRepository.GetNullableById(orgId);
                if (org != null)
                {
                    model.Organizations.Add(org);
                }
            }


            model.ExistingOrganizations = new List<Organization>();
            if (model.UserIsAlreadyDA)
            {
                model.ExistingOrganizations = user.Organizations;
            }

            model.OrgsExistingUsers = new List<KeyValuePair<string, string>>();
            foreach (var organization in model.Organizations)
            {
                Organization organization1 = organization;
                var users =
                    _repositoryFactory.UserRepository.Queryable.Where(a => a.Organizations.Contains(organization1)).Select(b => b.Email).ToList();
                foreach (var userEmail in users)
                {
                    model.OrgsExistingUsers.Add(new KeyValuePair<string, string>(organization.Id, userEmail));
                }
            }


            // Find Children DA Users.
            var childOrgIds = _queryRepositoryFactory.OrganizationDescendantRepository.Queryable.Where(a => requestedOrgIds.Contains(a.RollupParentId)).Select(b => b.OrgId).Distinct().ToList();
            var childOrganizations = new List<Organization>();
            foreach (var orgId in childOrgIds)
            {
                var org = _repositoryFactory.OrganizationRepository.GetNullableById(orgId);
                if (org != null)
                {
                    childOrganizations.Add(org);
                }
            }

            model.ChildOrgsExistingUsers = new List<KeyValuePair<string, string>>();
            model.OrganizationsWhithChildUsers = new List<Organization>();
            foreach (var organization in childOrganizations)
            {
                Organization organization1 = organization;
                var users = _repositoryFactory.UserRepository.Queryable.Where(a => a.Organizations.Contains(organization1) && a.Id != daRequest.Id).Select(b => b.Email).ToList();
                if (users.Count > 0)
                {
                    model.OrganizationsWhithChildUsers.Add(organization1);
                }
                foreach (var userEmail in users)
                {
                    model.ChildOrgsExistingUsers.Add(new KeyValuePair<string, string>(organization.Id, userEmail));
                }
            }

            // Find Parent DA Users.
            var parentOrgIds = _queryRepositoryFactory.OrganizationDescendantRepository.Queryable.Where(a => requestedOrgIds.Contains(a.OrgId)).Select(b => b.RollupParentId).Distinct().ToList();
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
                var users = _repositoryFactory.UserRepository.Queryable.Where(a => a.Organizations.Contains(organization1) && a.Id != daRequest.Id).Select(b => b.Email).ToList();
                if (users.Count > 0)
                {
                    model.OrganizationsWhithParentUsers.Add(organization1);
                }
                foreach (var userEmail in users)
                {
                    model.ParentOrgsExistingUsers.Add(new KeyValuePair<string, string>(organization.Id, userEmail));
                }
            }


            return View(model);
        }

        /// <summary>
        /// Can't use the one in Admin controller because the class has a special authorizer
        /// #8
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public JsonNetResult SearchOrgs(string searchTerm)
        {
            var orgs = _repositoryFactory.OrganizationRepository.Queryable.Where(a => a.Id.Contains(searchTerm) || a.Name.Contains(searchTerm)).OrderBy(o => o.Name);

            return new JsonNetResult(orgs.Select(a => new { id = a.Id, label = string.Format("{0} ({1})", a.Name, a.Id) }));
        }

        [Authorize(Roles = Role.Codes.Admin)]
        public ActionResult TookTraining(string id)
        {
            id = id.ToLower();
            var ldap = _directorySearchService.FindUser(id);

            Check.Require(ldap != null, "Person requesting Departmental Access ID not found. ID = " + id);

            var requestToSave = _departmentalAdminRequestRepository.GetNullableById(id) ?? new DepartmentalAdminRequest(id);
            requestToSave.FirstName = ldap.FirstName;
            requestToSave.LastName = ldap.LastName;
            requestToSave.Email = ldap.EmailAddress;
            requestToSave.AttendedTraining = true;

            ModelState.Clear();
            requestToSave.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _departmentalAdminRequestRepository.EnsurePersistent(requestToSave);

                Message = "Request created/Updated.";
                return this.RedirectToAction(x => x.Details(id));
            }

            ErrorMessage = "There were Errors, please correct and try again.";
            return this.RedirectToAction(x => x.Index(null));
        }
    }

    
    /// <summary>
    /// ViewModel for the DepartmentalAdminRequest class
    /// </summary>
    public class DepartmentalAdminRequestViewModel
	{
		public DepartmentalAdminRequest DepartmentalAdminRequest { get; set; }
        public IList<Organization> Organizations { get; set; }
        public IList<Organization> ExistingOrganizations { get; set; }
        [Display(Name = "User Exists")]
        public bool UserExists { get; set; }
        [Display(Name = "Already a Dept Admin")]
        public bool UserIsAlreadyDA { get; set; }
        [Display(Name = "Merge Existing Orgs")]
        public bool MergeExistingOrgs { get; set; }

        public IList<KeyValuePair<string, string>> OrgsExistingUsers { get; set; }
        public IList<KeyValuePair<string, string>> ChildOrgsExistingUsers { get; set; }
        public IList<KeyValuePair<string, string>> ParentOrgsExistingUsers { get; set; }

        public IList<Organization> OrganizationsWhithChildUsers { get; set; }
        public IList<Organization> OrganizationsWhithParentUsers { get; set; }

        public IList<string> AllExistingUsers { get; set; } 
 
		public static DepartmentalAdminRequestViewModel Create()
		{
			
			var viewModel = new DepartmentalAdminRequestViewModel {DepartmentalAdminRequest = new DepartmentalAdminRequest()};
            viewModel.Organizations = new List<Organization>();
 
			return viewModel;
		}
	}
}
