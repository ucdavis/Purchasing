using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the CustomField class
    /// </summary>
    public class CustomFieldController : ApplicationController
    {
	    private readonly IRepository<CustomField> _customFieldRepository;
        private readonly IRepositoryWithTypedId<Organization, string> _organizationRepository;

        public CustomFieldController(IRepository<CustomField> customFieldRepository, IRepositoryWithTypedId<Organization, string> organizationRepository )
        {
            _customFieldRepository = customFieldRepository;
            _organizationRepository = organizationRepository;
        }

        /// <summary>
        /// List of Custom Fields for the organization
        /// </summary>
        /// <param name="id">Organization Id</param>
        /// <returns></returns>
        public ActionResult Index(string id)
        {
            var org = _organizationRepository.GetNullableById(id);

            if (org == null)
            {
                Message = "Organization not found.";
                return RedirectToAction("Index", "Workgroup");
            }

            return View(org);
        }

        //
        // GET: /CustomField/Details/5
        public ActionResult Details(int id)
        {
            var customField = _customFieldRepository.GetNullableById(id);

            if (customField == null) return RedirectToAction("Index");

            return View(customField);
        }

        //
        // GET: /CustomField/Create
        public ActionResult Create()
        {
			var viewModel = CustomFieldViewModel.Create(Repository);
            
            return View(viewModel);
        } 

        //
        // POST: /CustomField/Create
        [HttpPost]
        public ActionResult Create(CustomField customField)
        {
            var customFieldToCreate = new CustomField();

            TransferValues(customField, customFieldToCreate);

            if (ModelState.IsValid)
            {
                _customFieldRepository.EnsurePersistent(customFieldToCreate);

                Message = "CustomField Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = CustomFieldViewModel.Create(Repository);
                viewModel.CustomField = customField;

                return View(viewModel);
            }
        }

        //
        // GET: /CustomField/Edit/5
        public ActionResult Edit(int id)
        {
            var customField = _customFieldRepository.GetNullableById(id);

            if (customField == null) return RedirectToAction("Index");

			var viewModel = CustomFieldViewModel.Create(Repository);
			viewModel.CustomField = customField;

			return View(viewModel);
        }
        
        //
        // POST: /CustomField/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, CustomField customField)
        {
            var customFieldToEdit = _customFieldRepository.GetNullableById(id);

            if (customFieldToEdit == null) return RedirectToAction("Index");

            TransferValues(customField, customFieldToEdit);

            if (ModelState.IsValid)
            {
                _customFieldRepository.EnsurePersistent(customFieldToEdit);

                Message = "CustomField Edited Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = CustomFieldViewModel.Create(Repository);
                viewModel.CustomField = customField;

                return View(viewModel);
            }
        }
        
        //
        // GET: /CustomField/Delete/5 
        public ActionResult Delete(int id)
        {
			var customField = _customFieldRepository.GetNullableById(id);

            if (customField == null) return RedirectToAction("Index");

            return View(customField);
        }

        //
        // POST: /CustomField/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, CustomField customField)
        {
			var customFieldToDelete = _customFieldRepository.GetNullableById(id);

            if (customFieldToDelete == null) return RedirectToAction("Index");

            _customFieldRepository.Remove(customFieldToDelete);

            Message = "CustomField Removed Successfully";

            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(CustomField source, CustomField destination)
        {
			//Recommendation: Use AutoMapper
			//Mapper.Map(source, destination)
            throw new NotImplementedException();
        }

    }

	/// <summary>
    /// ViewModel for the CustomField class
    /// </summary>
    public class CustomFieldViewModel
	{
		public CustomField CustomField { get; set; }
 
		public static CustomFieldViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new CustomFieldViewModel {CustomField = new CustomField()};
 
			return viewModel;
		}
	}
}
