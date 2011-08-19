using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
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
            var viewModel = MultiSelectViewModel.Create(Repository);

            return View(viewModel);
        }

        public JsonNetResult SearchAccounts(string searchTerm)
        {
            var results = Repository.OfType<Account>().Queryable.Where(a => a.Name.Contains(searchTerm) || a.Id.Contains(searchTerm));

            return new JsonNetResult(results.Select(a => new {Id = a.Id, Label = a.Name}));
        }

        public JsonNetResult SearchOrganizations(string searchTerm)
        {
            var results = Repository.OfType<Organization>().Queryable.Where(a => a.Name.Contains(searchTerm) || a.Id.Contains(searchTerm));

            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.Name }));
        }

        public JsonNetResult SearchVendors(string searchTerm)
        {
            var results = Repository.OfType<Vendor>().Queryable.Where(a => a.Name.Contains(searchTerm) || a.Id.Contains(searchTerm));

            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.Name }));
        }

    }

    public class MultiSelectViewModel
    {
        public IEnumerable<Account> Accounts { get; set; }
        public IEnumerable<Organization> Organizations { get; set; }
        public IEnumerable<Vendor> Vendors { get; set; }
        public IEnumerable<ListItem> LocalAccounts { get; set; }

        public static MultiSelectViewModel Create(IRepository repository)
        {
            string[] accts = { "3-6851000", "3-APSAC37", "3-APSAR24" };
            var accounts = repository.OfType<Account>().Queryable.Where(a => accts.Contains(a.Id));

            string[] orgs = {"AARC", "ACCR", "AAES"};
            var oganizations = repository.OfType<Organization>().Queryable.Where(a => orgs.Contains(a.Id));

            string[] vends = { "0000006853", "0000006859", "0000008583" };
            var vendors = repository.OfType<Vendor>().Queryable.Where(a => vends.Contains(a.Id));

            var acts = repository.OfType<Account>().Queryable.Select(a => new ListItem(a.Name, a.Id));

            var viewModel = new MultiSelectViewModel()
                                {
                                    Accounts = accounts.ToList(),
                                    Organizations = oganizations.ToList(),
                                    Vendors = vendors.ToList(),
                                    LocalAccounts = acts.ToList()
                                };

            return viewModel;
        }
    }

}
