using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the WorkgroupAccount class
    /// </summary>
    public class WorkgroupAccountController : ApplicationController
    {
	    private readonly IRepository<WorkgroupAccount> _workgroupAccountRepository;

        public WorkgroupAccountController(IRepository<WorkgroupAccount> workgroupAccountRepository)
        {
            _workgroupAccountRepository = workgroupAccountRepository;
        }
    
        //
        // GET: /WorkgroupAccount/
        public ActionResult Index(int id)
        {
            if (CurrentUser.IsInRole(Role.Codes.DepartmentalAdmin))
            {
                Message = "You must be a department admin to access a workgroup's accounts";
                return this.RedirectToAction<HomeController>(a => a.Index());
            }
            var workgroupAccountList = _workgroupAccountRepository.Queryable.Where(a=>a.Workgroup!=null && a.Workgroup.Id == id);

            return View(workgroupAccountList.ToList());
        }

        //
        // GET: /WorkgroupAccount/Details/5
        public ActionResult Details(int id)
        {
            var workgroupAccount = _workgroupAccountRepository.GetNullableById(id);

            if (workgroupAccount == null) return RedirectToAction("Index");

            return View(workgroupAccount);
        }

        //
        // GET: /WorkgroupAccount/Create
        public ActionResult Create()
        {
			var viewModel = WorkgroupAccountViewModel.Create(Repository);
            
            return View(viewModel);
        } 

        //
        // POST: /WorkgroupAccount/Create
        [HttpPost]
        public ActionResult Create(WorkgroupAccount workgroupAccount)
        {
            var workgroupAccountToCreate = new WorkgroupAccount();

            TransferValues(workgroupAccount, workgroupAccountToCreate);

            if (ModelState.IsValid)
            {
                _workgroupAccountRepository.EnsurePersistent(workgroupAccountToCreate);

                Message = "WorkgroupAccount Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = WorkgroupAccountViewModel.Create(Repository);
                viewModel.WorkgroupAccount = workgroupAccount;

                return View(viewModel);
            }
        }

        //
        // GET: /WorkgroupAccount/Edit/5
        public ActionResult Edit(int id)
        {
            var workgroupAccount = _workgroupAccountRepository.GetNullableById(id);

            if (workgroupAccount == null) return RedirectToAction("Index");

			var viewModel = WorkgroupAccountViewModel.Create(Repository);
			viewModel.WorkgroupAccount = workgroupAccount;

			return View(viewModel);
        }
        
        //
        // POST: /WorkgroupAccount/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, WorkgroupAccount workgroupAccount)
        {
            var workgroupAccountToEdit = _workgroupAccountRepository.GetNullableById(id);

            if (workgroupAccountToEdit == null) return RedirectToAction("Index");

            TransferValues(workgroupAccount, workgroupAccountToEdit);

            if (ModelState.IsValid)
            {
                _workgroupAccountRepository.EnsurePersistent(workgroupAccountToEdit);

                Message = "WorkgroupAccount Edited Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = WorkgroupAccountViewModel.Create(Repository);
                viewModel.WorkgroupAccount = workgroupAccount;

                return View(viewModel);
            }
        }
        
        //
        // GET: /WorkgroupAccount/Delete/5 
        public ActionResult Delete(int id)
        {
			var workgroupAccount = _workgroupAccountRepository.GetNullableById(id);

            if (workgroupAccount == null) return RedirectToAction("Index");

            return View(workgroupAccount);
        }

        //
        // POST: /WorkgroupAccount/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, WorkgroupAccount workgroupAccount)
        {
			var workgroupAccountToDelete = _workgroupAccountRepository.GetNullableById(id);

            if (workgroupAccountToDelete == null) return RedirectToAction("Index");

            _workgroupAccountRepository.Remove(workgroupAccountToDelete);

            Message = "WorkgroupAccount Removed Successfully";

            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(WorkgroupAccount source, WorkgroupAccount destination)
        {
			//Recommendation: Use AutoMapper
			//Mapper.Map(source, destination)
            throw new NotImplementedException();
        }

    }

	/// <summary>
    /// ViewModel for the WorkgroupAccount class
    /// </summary>
    public class WorkgroupAccountViewModel
	{
		public WorkgroupAccount WorkgroupAccount { get; set; }
 
		public static WorkgroupAccountViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new WorkgroupAccountViewModel {WorkgroupAccount = new WorkgroupAccount()};
 
			return viewModel;
		}
	}
}
