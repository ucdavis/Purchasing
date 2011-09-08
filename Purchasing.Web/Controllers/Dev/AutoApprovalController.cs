using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers.Dev
{
    /// <summary>
    /// Controller for the AutoApproval class
    /// </summary>
    public class AutoApprovalController : ApplicationController
    {
	    private readonly IRepository<AutoApproval> _autoApprovalRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;

        public AutoApprovalController(IRepository<AutoApproval> autoApprovalRepository, IRepositoryWithTypedId<User,string> userRepository)
        {
            _autoApprovalRepository = autoApprovalRepository;
            _userRepository = userRepository;
        }

        //
        // GET: /AutoApproval/
        public ActionResult Index()
        {
            var autoApprovalList = _autoApprovalRepository.Queryable;

            return View(autoApprovalList.ToList());
        }


        //
        // GET: /AutoApproval/Details/5
        public ActionResult Details(int id)
        {
            var autoApproval = _autoApprovalRepository.GetNullableById(id);

            if (autoApproval == null) return RedirectToAction("Index");

            return View(autoApproval);
        }

        //
        // GET: /AutoApproval/Create
        public ActionResult Create()
        {
			var viewModel = AutoApprovalViewModel.Create(Repository);
            
            return View(viewModel);
        } 

        //
        // POST: /AutoApproval/Create
        [HttpPost]
        public ActionResult Create(AutoApproval autoApproval)
        {
            autoApproval.Equal = !autoApproval.LessThan; //only one can be true, the other must be false

            if (ModelState.IsValid)
            {
                _autoApprovalRepository.EnsurePersistent(autoApproval);

                Message = "AutoApproval Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = AutoApprovalViewModel.Create(Repository);
                viewModel.AutoApproval = autoApproval;

                return View(viewModel);
            }
        }

        //
        // GET: /AutoApproval/Edit/5
        public ActionResult Edit(int id)
        {
            var autoApproval = _autoApprovalRepository.GetNullableById(id);

            if (autoApproval == null) return RedirectToAction("Index");

			var viewModel = AutoApprovalViewModel.Create(Repository);
			viewModel.AutoApproval = autoApproval;

			return View(viewModel);
        }
        
        //
        // POST: /AutoApproval/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, AutoApproval autoApproval)
        {
            var autoApprovalToEdit = _autoApprovalRepository.GetNullableById(id);

            if (autoApprovalToEdit == null) return RedirectToAction("Index");

            TransferValues(autoApproval, autoApprovalToEdit);

            if (ModelState.IsValid)
            {
                _autoApprovalRepository.EnsurePersistent(autoApprovalToEdit);

                Message = "AutoApproval Edited Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = AutoApprovalViewModel.Create(Repository);
                viewModel.AutoApproval = autoApproval;

                return View(viewModel);
            }
        }
        
        //
        // GET: /AutoApproval/Delete/5 
        public ActionResult Delete(int id)
        {
			var autoApproval = _autoApprovalRepository.GetNullableById(id);

            if (autoApproval == null) return RedirectToAction("Index");

            return View(autoApproval);
        }

        //
        // POST: /AutoApproval/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, AutoApproval autoApproval)
        {
			var autoApprovalToDelete = _autoApprovalRepository.GetNullableById(id);

            if (autoApprovalToDelete == null) return RedirectToAction("Index");

            _autoApprovalRepository.Remove(autoApprovalToDelete);

            Message = "AutoApproval Removed Successfully";

            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(AutoApproval source, AutoApproval destination)
        {
			//Recommendation: Use AutoMapper
			//Mapper.Map(source, destination)
            throw new NotImplementedException();
        }


    }

	/// <summary>
    /// ViewModel for the AutoApproval class
    /// </summary>
    public class AutoApprovalViewModel
	{
		public AutoApproval AutoApproval { get; set; }
	    public IList<Account> Accounts { get; set; }
 
		public static AutoApprovalViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");

		    var viewModel = new AutoApprovalViewModel
		                        {
		                            AutoApproval = new AutoApproval { IsActive = true, Expiration = DateTime.Now.AddYears(1) },
                                    Accounts = repository.OfType<Account>().GetAll()//TODO: filter or setup a multi-select
		                        };

		    return viewModel;
		}
	}
}
