using System;
using System.Linq;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;
using AutoMapper;

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

        public ActionResult Create()
        {
           var workgroup = new Workgroup() { IsActive = true };

            var model = new WorkgroupViewModel
            {
                Workgroup = workgroup
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(WorkgroupViewModel workgroupViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(workgroupViewModel);
            }

            var workgroup = new Workgroup();

            Mapper.Map(workgroupViewModel.Workgroup, workgroup);

            _workgroupRepository.EnsurePersistent(workgroup);

            Message = string.Format("{0} workgroup was created",
                                    workgroup.Name);

            return RedirectToAction("Index");
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
