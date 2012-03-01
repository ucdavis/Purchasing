using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Organization class
    /// </summary>
    public class OrganizationController : ApplicationController
    {
        private readonly IRepositoryWithTypedId<Organization, string> _organizationRepository;
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;

        public OrganizationController(IRepositoryWithTypedId<Organization, string> organizationRepository, IQueryRepositoryFactory queryRepositoryFactory)
        {
            _organizationRepository = organizationRepository;
            _queryRepositoryFactory = queryRepositoryFactory;
        }


        /// <summary>
        /// Display list of orgs, that the current user is a dept admin for
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var orgIds = _queryRepositoryFactory.AdminOrgRepository.Queryable.Where(a => a.AccessUserId == CurrentUser.Identity.Name && a.IsActive).Select(a => a.OrgId).ToList();
            var orgs = _organizationRepository.Queryable.Where(a => orgIds.Contains(a.Id)).ToList();

            return View(orgs);
        }

        /// <summary>
        /// Display details for an org that a user has access to.
        /// </summary>
        /// <param name="id">Organization id</param>
        /// <returns></returns>
        public ActionResult Details(string id)
        {
            var adminOrg = _queryRepositoryFactory.AdminOrgRepository.Queryable.Where(a => a.AccessUserId == CurrentUser.Identity.Name && a.IsActive && a.OrgId == id).Select(a => a.OrgId).FirstOrDefault();
            var org = _organizationRepository.GetNullableById(adminOrg);

            if (org == null)
            {
                return RedirectToAction("NotAuthorized", "Error");
            }

            return View(org);
        }
    }

	/// <summary>
    /// ViewModel for the Organization class
    /// </summary>
    public class OrganizationViewModel
	{
		public Organization Organization { get; set; }
 
		public static OrganizationViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new OrganizationViewModel {Organization = new Organization()};
 
			return viewModel;
		}
	}
}
