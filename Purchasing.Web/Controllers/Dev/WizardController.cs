

using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers.Dev
{
    /// <summary>
    /// Controller for the Wizard class
    /// </summary>
    public class WizardController : ApplicationController
    {
	    private readonly IRepository<Workgroup> _workgroupRepository;

        public WizardController(IRepository<Workgroup> workgroupRepository)
        {
            _workgroupRepository = workgroupRepository;
        }
    

        /// <summary>
        /// GET: /Wizard/
        /// View To start the Workgroup create wizard
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            return View();
        }

        //NOTE: This wizard will only allow adding stuff. No delete, edit or view. For that they will complete the wizard and go back to the Workgroup management

        /// <summary>
        /// Step 1
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateWorkgroup()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CreateWorkgroup(Workgroup workgroup)
        {

            return View();
        }

        /// <summary>
        /// Step 2
        /// </summary>
        /// <returns></returns>
        public ActionResult AddSubOrganizations()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddSubOrganizations(string temp)
        {
            return View();
        }

        /// <summary>
        /// Step 3
        /// </summary>
        /// <returns></returns>
        public ActionResult AddPeople()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPeople(string temp)
        {
            return View();
        }


        /// <summary>
        /// Step 4
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAccounts()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddAccounts(string temp)
        {
            return View();
        }



        /// <summary>
        /// Step 5
        /// </summary>
        /// <returns></returns>
        public ActionResult AddVendors()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddVendors(string temp)
        {
            return View();
        }
        /// <summary>
        /// Step 6
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAddresses()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAddresses(string temp)
        {
            return View();
        }

    }

}
