using System;
using System.Web.Mvc;
using AutoMapper;
using Purchasing.Core.Domain;
using Purchasing.Web.Attributes;
using Purchasing.Web.Models;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using Purchasing.Web.Services;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for UserVoice
    /// </summary>
    /// <remarks>
    /// </remarks>
    [HandleTransactionsManually]
    public class HelpController : ApplicationController
    {
        private readonly IUservoiceService _uservoiceService;

        public HelpController(IUservoiceService uservoiceService)
        {
            _uservoiceService = uservoiceService;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Gets the number of active issues in the purchasing uservoice forum
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 60)] //Cache this call for a while
        public ActionResult GetActiveIssuesCount()
        {
            var issuesCount = _uservoiceService.GetActiveIssuesCount();

            return Json(new {HasIssues = issuesCount > 0, IssuesCount = issuesCount, TimeStamp = DateTime.Now.Ticks},
                        JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 60)] //Cache this call for a while
        public ActionResult OpenIssues()
        {
            var issues = _uservoiceService.GetOpenIssues();

            return new JsonNetResult(issues);
        }

        [OutputCache(Duration = 60)] //Cache this call for a while
        public ActionResult IssuesForUser(string user)
        {
            var issues = _uservoiceService.GetActiveIssuesForUser(user ?? CurrentUser.Identity.Name);

            return new JsonNetResult(issues);
        }

        //[HandleTransactionsManually]
        //public ActionResult SetIssueStatus(int id, string status)
        //{
        //    Check.Require(id != default(int));
        //    Check.Require(status != null);

        //    var message = "Status updated by " + CurrentUser.Identity.Name;

        //    _uservoiceService.SetIssueStatus(id, status, message);

        //    return Content("Status set to " + status);
        //}

        /*
        [IpFilter]
        public ActionResult AutomaticJob()
        {
            //TODO: put logic for automatic job for emailing

            // email blast to all opp-users after 1 day old?

            // email followup to original created user after 5-7 days old?

            // force close and potentially notify, if uservoice doesn't?
        }*/
    }
}