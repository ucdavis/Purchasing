using System;
using System.Linq;
using System.Web.Mvc;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;
using AutoMapper;
using System.Collections.Generic;
using UCDArch.Web.ActionResults;
using Purchasing.Web.Utility;
using System.Web.UI.WebControls;

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
            var user = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Single();

            var model = WorkgroupModifyModel.Create(Repository, user);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Workgroup workgroup, string[] selectedOrganizations)
        {
            if (!ModelState.IsValid)
            {
                return View(new WorkgroupViewModel { Workgroup = workgroup });
            }

            var _workgroup = new Workgroup();

            Mapper.Map(workgroup, _workgroup);
            
            _workgroup.Organizations = Repository.OfType<Organization>().Queryable.Where(a => selectedOrganizations.Contains(a.Id)).ToList();

            _workgroupRepository.EnsurePersistent(_workgroup);

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

        public ActionResult Edit(int id)
        {
            var user = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Single();
            var workgroup = _workgroupRepository.GetNullableById(id);

            var model = WorkgroupModifyModel.Create(Repository, user, workgroup);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit( Workgroup workgroup, string[] selectedOrganizations )
        {

            if (!ModelState.IsValid)
            {
                return View(new WorkgroupModifyModel { Workgroup = workgroup });
            }

            var _workgroup = _workgroupRepository.GetNullableById(workgroup.Id);

            Mapper.Map(workgroup, _workgroup);

            _workgroup.Organizations = Repository.OfType<Organization>().Queryable.Where(a => selectedOrganizations.Contains(a.Id)).ToList();

            _workgroupRepository.EnsurePersistent(_workgroup);

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

        public JsonNetResult SearchOrganizations(string searchTerm)
        {
            var results = Repository.OfType<Organization>().Queryable.Where(a => a.Name.Contains(searchTerm) || a.Id.Contains(searchTerm)).Select(a => new IdAndName(a.Id, a.Name)).ToList();

            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.DisplayNameAndId }));
        }
    }

    /// <summary>
    /// ModifyModel for the Workgroup class
    /// </summary>
    public class WorkgroupModifyModel
    {
        public Workgroup Workgroup { get; set; }
        public List<ListItem> Organizations { get; set; } 

        public static WorkgroupModifyModel Create(IRepository repository, User user, Workgroup workgroup = null)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var modifyModel = new WorkgroupModifyModel { Workgroup = workgroup ?? new Workgroup() };

            if (workgroup != null)
            {
                modifyModel.Organizations = workgroup.Organizations.Select(x => new ListItem(x.Name, x.Id, true)
                {
                    Selected = true
                }
            ).ToList();
            } else
            {
                modifyModel.Organizations = new List<ListItem>();
            }

            var userOrgs = user.Organizations.Where(x => !modifyModel.Organizations.Select(y => y.Value).Contains(x.Id));
            modifyModel.Organizations.AddRange(userOrgs.Select(x => new ListItem(x.Name, x.Id, true)));

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
