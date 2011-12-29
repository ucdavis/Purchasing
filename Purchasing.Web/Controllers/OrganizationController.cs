using System;
using System.Linq;
using System.Web.Mvc;
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
	    private readonly IRepository<Organization> _organizationRepository;

        public OrganizationController(IRepository<Organization> organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }
    

        /// <summary>
        /// Display list of orgs, that the current user is a dept admin for
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var organizationList = _organizationRepository.Queryable;

            return View(organizationList.ToList());
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
