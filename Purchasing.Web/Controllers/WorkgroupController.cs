using System;
using System.Linq;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;
using AutoMapper;
using System.Collections.Generic;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Workgroup class
    /// </summary>
    [Authorize]
    public class WorkgroupController : ApplicationController
    {
	    private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IRepository<User> _userRepository;

        public WorkgroupController(IRepository<Workgroup> workgroupRepository, IRepository<User> userRepository)
        {
            _workgroupRepository = workgroupRepository;
            _userRepository = userRepository;
        }
    
        //
        // GET: /Workgroup/
        public ActionResult Index()
        {
            var person =
                _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Fetch(x => x.Organizations).Single();

            var orgIds = person.Organizations.Select(x => x.Id).ToArray();

            var workgroupList =
                _workgroupRepository.Queryable.Where(x => x.Organizations.Any(a => orgIds.Contains(a.Id)));

            return View(workgroupList.ToList());
        }

        public ActionResult Create()
        {
            var workgroup = new Workgroup() { IsActive = true };
            var user = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Single();

            var model = new WorkgroupModifyModel
            {
                Workgroup = workgroup,
                UserOrganizations = user.Organizations
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
            
            // Get current user's organization and assign to the new workgroup
            //var user = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Single();
            //workgroup.Organizations.Add(user.Organizations);

            _workgroupRepository.EnsurePersistent(workgroup);

            Message = string.Format("{0} workgroup was created",
                                    workgroup.Name);

            return RedirectToAction("Index");
        }

        public ActionResult Details(int Id)
        {
            var workgroup = _workgroupRepository.GetNullableById(Id);

            var model = new WorkgroupViewModel
            {
                Workgroup = workgroup
            };

            return View(model);
        }

        public ActionResult Edit(int Id)
        {
            var workgroup = _workgroupRepository.GetNullableById(Id);

            var model = new WorkgroupViewModel
            {
                Workgroup = workgroup
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(WorkgroupViewModel workgroupViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(workgroupViewModel);
            }

            var workgroup = _workgroupRepository.GetNullableById(workgroupViewModel.Workgroup.Id);

            Mapper.Map(workgroupViewModel.Workgroup, workgroup);
            
            _workgroupRepository.EnsurePersistent(workgroup);

            Message = string.Format("{0} was modified successfully",
                                    workgroup.Name);

            return RedirectToAction("Index");

        }

        public ActionResult Delete(int Id)
        {
            var workgroup = _workgroupRepository.GetNullableById(Id);

            var model = new WorkgroupViewModel
            {
                Workgroup = workgroup
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(WorkgroupViewModel workgroupViewModel)
        {
            var workgroup = _workgroupRepository.GetNullableById(workgroupViewModel.Workgroup.Id);

            workgroup.IsActive = false;

            _workgroupRepository.EnsurePersistent(workgroup);

            Message = string.Format("{0} was disabled successfully",
                                    workgroup.Name);

            return RedirectToAction("Index");

        }
    }

    /// <summary>
    /// ModifyModel for the Workgroup class
    /// </summary>
    public class WorkgroupModifyModel
    {
        public Workgroup Workgroup { get; set; }
        public IList<Organization> UserOrganizations { get; set; }

        public static WorkgroupModifyModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var modifyModel = new WorkgroupModifyModel { Workgroup = new Workgroup() };

            return modifyModel;
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
