using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the MultiSelect class
    /// </summary>
    public class MultiSelectController : ApplicationController
    {
        //
        // GET: /MultiSelect/
        public ActionResult Index()
        {
            string[] accts = {"3-6851000", "3-APSAC37", "3-APSAR24"};

            var accounts = Repository.OfType<Account>().Queryable.Where(a => accts.Contains(a.Id));

            return View(accounts);
        }

        public JsonNetResult Search(string searchTerm)
        {
            var results = Repository.OfType<Account>().Queryable.Where(a => a.Name.Contains(searchTerm));

            return new JsonNetResult(results.Select(a => new {Id = a.Id, Label = a.Name}));

        }

    }

}
