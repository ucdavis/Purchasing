using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using Newtonsoft.Json.Linq;
using Purchasing.Core.Domain;
using Purchasing.Web.Helpers;
using Purchasing.Web.Models;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Faq class
    /// </summary>
    public class HelpController : ApplicationController
    {
	    private readonly IRepository<Faq> _faqRepository;

        public HelpController(IRepository<Faq> faqRepository)
        {
            _faqRepository = faqRepository;
        }

        //const string query = "https://ucdavis.uservoice.com/api/v1/forums/126891/suggestions.json?category=31579&sort=newest";
        //const string query = "https://ucdavis.uservoice.com/api/v1/forums/126891/categories/31577.json";
        //const string query = "https://ucdavis.uservoice.com/api/v1/forums/126891/categories.json";
        //const string query = "https://ucdavis.uservoice.com/api/v1/users/24484752.json";
            
        /// <summary>
        /// Gets the number of active issues in the purchasing uservoice forum
        /// </summary>
        /// <returns></returns>
        [HandleTransactionsManually]
        public ActionResult GetActiveIssuesCount()
        {
            const string query = "https://ucdavis.uservoice.com/api/v1/forums/126891/categories.json";
            
            var oauth = new Manager();
            oauth["consumer_key"] = "RfVVMKtNAL9AhXDHVx0FyQ";
            oauth["consumer_secret"] = "r0H7iLuZZokE0BWiszUr9mxuDYFuhmoghdomUsvqw";

            var header = oauth.GenerateAuthzHeader(query, "GET");

            var req = WebRequest.Create(query);
            req.Headers.Add("Authorization", header);

            using (var response = (HttpWebResponse)req.GetResponse())
            {
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    var obj = JObject.Parse(reader.ReadToEnd());

                    var issueCategory =
                        obj["categories"].Children().Single(c => c["name"].Value<string>() == "Issues");
                    
                    var issuesCount = issueCategory["suggestions_count"].Value<int>();

                    return Json(new {HasIssues = issuesCount > 0, IssuesCount = issuesCount});
                }
            }
        }
    
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
