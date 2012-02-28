using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Faq class
    /// </summary>
    public class FaqController : ApplicationController
    {
	    private readonly IRepository<Faq> _faqRepository;

        public FaqController(IRepository<Faq> faqRepository)
        {
            _faqRepository = faqRepository;
        }
    
        //
        // GET: /Faq/
        public ActionResult Index()
        {
            var faqList = _faqRepository.Queryable;

            return View(faqList.ToList());
        }

        //
        // GET: /Faq/Details/5
        public ActionResult Details(int id)
        {
            var faq = _faqRepository.GetNullableById(id);

            if (faq == null) return RedirectToAction("Index");

            return View(faq);
        }

        //
        // GET: /Faq/Create
        public ActionResult Create()
        {
			var viewModel = FaqViewModel.Create(Repository);
            
            return View(viewModel);
        } 

        //
        // POST: /Faq/Create
        [HttpPost]
        public ActionResult Create(Faq faq)
        {
            var faqToCreate = new Faq();

            TransferValues(faq, faqToCreate);

            if (ModelState.IsValid)
            {
                _faqRepository.EnsurePersistent(faqToCreate);

                Message = "Faq Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = FaqViewModel.Create(Repository);
                viewModel.Faq = faq;

                return View(viewModel);
            }
        }

        //
        // GET: /Faq/Edit/5
        public ActionResult Edit(int id)
        {
            var faq = _faqRepository.GetNullableById(id);

            if (faq == null) return RedirectToAction("Index");

			var viewModel = FaqViewModel.Create(Repository);
			viewModel.Faq = faq;

			return View(viewModel);
        }
        
        //
        // POST: /Faq/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Faq faq)
        {
            var faqToEdit = _faqRepository.GetNullableById(id);

            if (faqToEdit == null) return RedirectToAction("Index");

            TransferValues(faq, faqToEdit);

            if (ModelState.IsValid)
            {
                _faqRepository.EnsurePersistent(faqToEdit);

                Message = "Faq Edited Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = FaqViewModel.Create(Repository);
                viewModel.Faq = faq;

                return View(viewModel);
            }
        }
        
        //
        // GET: /Faq/Delete/5 
        public ActionResult Delete(int id)
        {
			var faq = _faqRepository.GetNullableById(id);

            if (faq == null) return RedirectToAction("Index");

            return View(faq);
        }

        //
        // POST: /Faq/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Faq faq)
        {
			var faqToDelete = _faqRepository.GetNullableById(id);

            if (faqToDelete == null) return RedirectToAction("Index");

            _faqRepository.Remove(faqToDelete);

            Message = "Faq Removed Successfully";

            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(Faq source, Faq destination)
        {
			//Recommendation: Use AutoMapper
			//Mapper.Map(source, destination)
            throw new NotImplementedException();
        }

    }

	/// <summary>
    /// ViewModel for the Faq class
    /// </summary>
    public class FaqViewModel
	{
		public Faq Faq { get; set; }
 
		public static FaqViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");
			
			var viewModel = new FaqViewModel {Faq = new Faq()};
 
			return viewModel;
		}
	}
}
