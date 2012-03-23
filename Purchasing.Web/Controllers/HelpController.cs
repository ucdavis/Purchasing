using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Newtonsoft.Json.Linq;
using Purchasing.Core.Domain;
using Purchasing.Web.Attributes;
using Purchasing.Web.Models;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using Purchasing.Web.Services;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Faq class
    /// </summary>
    public class HelpController : ApplicationController
    {
	    private readonly IRepository<Faq> _faqRepository;
        private readonly IUservoiceService _uservoiceService;

        public HelpController(IRepository<Faq> faqRepository, IUservoiceService uservoiceService)
        {
            _faqRepository = faqRepository;
            _uservoiceService = uservoiceService;
        }

        /// <summary>
        /// Gets the number of active issues in the purchasing uservoice forum
        /// </summary>
        /// <returns></returns>
        [HandleTransactionsManually]
        [OutputCache(Duration = 60)] //Cache this call for a while
        public ActionResult GetActiveIssuesCount()
        {
            var issuesCount = _uservoiceService.GetActiveIssuesCount();
            
            return Json(new {HasIssues = issuesCount > 0, IssuesCount = issuesCount, TimeStamp = DateTime.Now.Ticks},
                        JsonRequestBehavior.AllowGet);
        }

        [HandleTransactionsManually]
        public ActionResult OpenIssues()
        {
            var issues = _uservoiceService.GetOpenIssues();
            
            return new JsonNetResult(issues);
        }

        public ActionResult SetIssueStatus(int id, string status)
        {
            Check.Require(status != null);

            _uservoiceService.SetIssueStatus(id, status);

            return Content("hopefully");
        }

        [IpFilter]
        public ActionResult AutomaticJob()
        {
            //TODO: put logic for automatic job for emailing

            // email blast to all opp-users after 1 day old?

            // email followup to original created user after 5-7 days old?

            // force close and potentially notify, if uservoice doesn't?

            return View();
        }


        // ************************************************************
        // Stuff below, is being replaced with above.
        // ************************************************************

        //
        // GET: /Faq/
        public ActionResult Index()
        {
            var faqList = _faqRepository.Queryable;

            return View(faqList);
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
            Mapper.Map(source, destination);
        }

    }
}
