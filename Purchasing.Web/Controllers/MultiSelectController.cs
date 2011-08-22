using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Purchasing.Core.Domain;
using Purchasing.Web.Utility;
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
            var results = Repository.OfType<Account>().Queryable.Where(a => a.Name.Contains(searchTerm) || a.Id.Contains(searchTerm)).Select(a=>  new IdAndName(a.Id, a.Name)).ToList();

            return new JsonNetResult(results.Select(a => new {Id = a.Id, Label = a.DisplayNameAndId}));
        }

        public JsonNetResult SearchOrganizations(string searchTerm)
        {
            var results = Repository.OfType<Organization>().Queryable.Where(a => a.Name.Contains(searchTerm) || a.Id.Contains(searchTerm)).Select(a => new IdAndName(a.Id, a.Name)).ToList();

            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.DisplayNameAndId }));
        }

        public JsonNetResult SearchVendors(string searchTerm)
        {
            var results = Repository.OfType<Vendor>().Queryable.Where(a => a.Name.Contains(searchTerm) || a.Id.Contains(searchTerm)).Select(a => new IdAndName(a.Id, a.Name)).ToList();

            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.DisplayNameAndId }));
        }

        public ActionResult BindingExample()
        {
            var viewModel = MultiSelectViewModel.Create(Repository);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult BindingExample(string[] accounts, string[] organizations, string[] vendors)
        {
            Message = string.Format("Posted {0} Accounts, {1} Organizations, {2} Vendors", accounts.Count(), organizations.Count(), vendors.Count());
            var viewModel = MultiSelectViewModel.Create(Repository);
            viewModel.Accounts = Repository.OfType<Account>().Queryable.Where(a => accounts.Contains(a.Id)).Select(a => new IdAndName(a.Id, a.Name)).ToList();
            viewModel.Organizations = Repository.OfType<Organization>().Queryable.Where(a => organizations.Contains(a.Id)).Select(a => new IdAndName(a.Id, a.Name)).ToList();
            viewModel.Vendors = Repository.OfType<Vendor>().Queryable.Where(a => vendors.Contains(a.Id)).Select(a => new IdAndName(a.Id, a.Name)).ToList();
            return View(viewModel);
        }

    }

    public class MultiSelectViewModel
    {
        public IEnumerable<IdAndName> Accounts { get; set; }
        public IEnumerable<IdAndName> Organizations { get; set; }
        public IEnumerable<IdAndName> Vendors { get; set; }
        public IEnumerable<ListItem> LocalAccounts { get; set; }

        public static MultiSelectViewModel Create(IRepository repository)
        {
            string[] accts = { "3-6851000", "3-APSAC37", "3-APSAR24" };
            var accounts = repository.OfType<Account>().Queryable.Where(a => accts.Contains(a.Id)).Select(a => new IdAndName(a.Id, a.Name)).ToList();

            string[] orgs = {"AARC", "ACCR", "AAES"};
            var oganizations = repository.OfType<Organization>().Queryable.Where(a => orgs.Contains(a.Id)).Select(a => new IdAndName(a.Id, a.Name)).ToList();

            string[] vends = { "0000006853", "0000006859", "0000008583" };
            var vendors = repository.OfType<Vendor>().Queryable.Where(a => vends.Contains(a.Id)).Select(a => new IdAndName(a.Id, a.Name)).ToList();

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

    public class MultiSelectPostModel
    {
        public List<Account> Accounts { get; set; }
        public List<Organization> Organizations { get; set; }
        public List<Vendor> Vendors { get; set; } 
    }

}
