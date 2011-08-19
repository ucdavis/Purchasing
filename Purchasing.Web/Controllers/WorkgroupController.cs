using System;
using System.Linq;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Workgroup class
    /// </summary>
    public class WorkgroupController : ApplicationController
    {
	    private readonly IRepository<Workgroup> _workgroupRepository;

        public WorkgroupController(IRepository<Workgroup> workgroupRepository)
        {
            _workgroupRepository = workgroupRepository;
        }
    
        //
        // GET: /Workgroup/
        public ActionResult Index()
        {
            var workgroupList = _workgroupRepository.Queryable;

            return View(workgroupList.ToList());
        }

    }

	/// <summary>
    /// ViewModel for the Workgroup class
    /// </summary>
    public class WorkgroupViewModel
	{
		public Workgroup Workgroup { get; set; }
 
		public static WorkgroupViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new WorkgroupViewModel {Workgroup = new Workgroup()};
 
			return viewModel;
		}
	}
}
