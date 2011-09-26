using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.Helpers;
using MvcContrib;

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

        /// <summary>
        /// #1
        /// GET: /AutoApproval/
        /// </summary>
        /// <param name="showAll">show expired, not active, and active where user is current user.</param>
        /// <returns></returns>
        public ActionResult Index(bool showAll = false)
        {
            //This code my be useful for validation or presenting list of people/accounts
            //var workgroups = Repository.OfType<WorkgroupPermission>().Queryable.Where(a => a.Role != null && a.Role.Id == Role.Codes.Approver && a.User != null && a.User.Id == CurrentUser.Identity.Name).Select(b => b.Workgroup).Distinct().ToList();
            //var approveableAccountIds = workgroups.SelectMany(a => a.Accounts).Select(b => b.Account).Distinct().Select(c => c.Id).ToList();
            //var approveableUserIds = workgroups.SelectMany(a => a.Permissions).Where(b => b.Role != null && b.Role.Id == Role.Codes.Requester).Select(c => c.User.Id).Distinct().ToList();

            //var personAutoApprovalIds = _autoApprovalRepository.Queryable.Where(a => a.User != null && approveableUserIds.Contains(a.TargetUser.Id)).Select(b => b.Id).ToArray();
            //var accountAutoApprovalIds = _autoApprovalRepository.Queryable.Where(a => a.Account != null && approveableAccountIds.Contains(a.Account.Id)).Select(b => b.Id).ToArray();

            //var allAutoApprovalIds = personAutoApprovalIds.Union(accountAutoApprovalIds).ToList();
            
            //var autoApprovalList = _autoApprovalRepository.Queryable.Where(a => allAutoApprovalIds.Contains(a.Id));

            var viewModel = AutoApprovalListModel.Create(_autoApprovalRepository, CurrentUser.Identity.Name, showAll);

            return View(viewModel);
        }


        /// <summary>
        /// #2
        /// GET: /AutoApproval/Details/5
        /// </summary>
        /// <param name="id">AutoApproval Id</param>
        /// <param name="showAll">Show inactive and expired</param>
        /// <returns></returns>
        public ActionResult Details(int id, bool showAll = false)
        {
            var autoApproval = _autoApprovalRepository.GetNullableById(id);

            if (autoApproval == null)
            {
                return this.RedirectToAction(a => a.Index(showAll));
            }

            if(autoApproval.User.Id != CurrentUser.Identity.Name)
            {
                ErrorMessage = "No Access";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            ViewBag.ShowAll = showAll;

            return View(autoApproval);
        }
        /// <summary>
        /// #3
        /// GET: /AutoApproval/Create
        /// </summary>
        /// <param name="showAll"></param>
        /// <returns></returns>
        public ActionResult Create(bool showAll = false)
        {
			var viewModel = AutoApprovalViewModel.Create(Repository, CurrentUser.Identity.Name);
            ViewBag.ShowAll = showAll;
            ViewBag.IsCreate = true;

            return View(viewModel);
        } 

        //
        // POST: /AutoApproval/Create
        [HttpPost]
        public ActionResult Create(AutoApproval autoApproval, bool showAll = false)
        {
            autoApproval.Equal = !autoApproval.LessThan; //only one can be true, the other must be false
            autoApproval.User = _userRepository.GetById(CurrentUser.Identity.Name);            
            ModelState.Clear();
            autoApproval.TransferValidationMessagesTo(ModelState);
            if(autoApproval.Expiration.HasValue && autoApproval.Expiration.Value.Date <= DateTime.Now.Date)
            {
                ModelState.AddModelError("AutoApproval.Expiration", "Expiration date has already passed");
            }
            autoApproval.IsActive = true;

            if (ModelState.IsValid)
            {
                _autoApprovalRepository.EnsurePersistent(autoApproval);

                Message = "AutoApproval Created Successfully";
                if(autoApproval.Expiration.HasValue && autoApproval.Expiration.Value.Date <= DateTime.Now.Date.AddDays(5))
                {
                    Message = Message + " Warning, will expire in 5 days or less";
                }
                return this.RedirectToAction(a => a.Index(showAll));
            }
            else
            {
				var viewModel = AutoApprovalViewModel.Create(Repository, CurrentUser.Identity.Name);
                viewModel.AutoApproval = autoApproval;
                ViewBag.ShowAll = showAll;
                ViewBag.IsCreate = true;

                return View(viewModel);
            }
        }

        //
        // GET: /AutoApproval/Edit/5
        public ActionResult Edit(int id, bool showAll = false)
        {
            var autoApproval = _autoApprovalRepository.GetNullableById(id);

            if (autoApproval == null)
            {
                return this.RedirectToAction(a => a.Index(showAll));
            }

            if(autoApproval.User.Id != CurrentUser.Identity.Name)
            {
                ErrorMessage = "No Access";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = AutoApprovalViewModel.Create(Repository, CurrentUser.Identity.Name);
			viewModel.AutoApproval = autoApproval;
            ViewBag.ShowAll = showAll;

			return View(viewModel);
        }
        
        //
        // POST: /AutoApproval/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, AutoApproval autoApproval, bool showAll = false)
        {
            var autoApprovalToEdit = _autoApprovalRepository.GetNullableById(id);

            if (autoApprovalToEdit == null)
            {
                return this.RedirectToAction(a => a.Index(showAll));
            }

            if(autoApprovalToEdit.User.Id != CurrentUser.Identity.Name)
            {
                ErrorMessage = "No Access";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            TransferValues(autoApproval, autoApprovalToEdit);
            autoApprovalToEdit.Equal = !autoApprovalToEdit.LessThan;

            ModelState.Clear();
            autoApprovalToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _autoApprovalRepository.EnsurePersistent(autoApprovalToEdit);

                Message = "AutoApproval Edited Successfully";

                return this.RedirectToAction(a => a.Index(showAll));
            }
            else
            {
                var viewModel = AutoApprovalViewModel.Create(Repository, CurrentUser.Identity.Name);
                viewModel.AutoApproval = autoApproval;
                ViewBag.ShowAll = showAll;

                return View(viewModel);
            }
        }
        
        //
        // GET: /AutoApproval/Delete/5 
        public ActionResult Delete(int id, bool showAll = false)
        {
			var autoApproval = _autoApprovalRepository.GetNullableById(id);

            if (autoApproval == null)
            {
                return this.RedirectToAction(a => a.Index(showAll));
            }

            if(autoApproval.User.Id != CurrentUser.Identity.Name)
            {
                ErrorMessage = "No Access";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if(!autoApproval.IsActive)
            {
                Message = "Already deactivated";
                return this.RedirectToAction(a => a.Index(showAll));
            }

            ViewBag.ShowAll = showAll;


            return View(autoApproval);
        }

        //
        // POST: /AutoApproval/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, AutoApproval autoApproval, bool showAll = false)
        {
			var autoApprovalToDelete = _autoApprovalRepository.GetNullableById(id);

            if (autoApprovalToDelete == null)
            {
                return this.RedirectToAction(a => a.Index(showAll));
            }

            if(autoApprovalToDelete.User.Id != CurrentUser.Identity.Name)
            {
                ErrorMessage = "No Access";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            if(autoApprovalToDelete.IsActive)
            {
                autoApprovalToDelete.IsActive = false;

                _autoApprovalRepository.EnsurePersistent(autoApprovalToDelete);

                Message = "AutoApproval Deactivated Successfully";
            }
            return this.RedirectToAction(a => a.Index(showAll));
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(AutoApproval source, AutoApproval destination)
        {
            Mapper.Map(source, destination);
        }


    }

    public class AutoApprovalListModel
    {
        public List<AutoApproval> AutoApprovals { get; set; }
        public bool ShowAll { get; set; }

        public static AutoApprovalListModel Create(IRepository<AutoApproval> autoApprovalRepository, string userName, bool showAll)
        {
            Check.Require(autoApprovalRepository != null);
            Check.Require(!string.IsNullOrWhiteSpace(userName));

            var viewModel = new AutoApprovalListModel {ShowAll = showAll};
            var temp = autoApprovalRepository.Queryable.Where(a => a.User != null && a.User.Id == userName);
            if (!showAll)
            {
                temp = temp.Where(a => a.IsActive && (a.Expiration == null || a.Expiration >= DateTime.Now.Date));
            }

            viewModel.AutoApprovals = temp.ToList();

            return viewModel;
        }

    }

    /// <summary>
    /// ViewModel for the AutoApproval class
    /// </summary>
    public class AutoApprovalViewModel
	{
		public AutoApproval AutoApproval { get; set; }
	    public IList<Account> Accounts { get; set; }
        public IList<User> Users { get; set; } 
 
		public static AutoApprovalViewModel Create(IRepository repository, string userName)
		{
			Check.Require(repository != null, "Repository must be supplied");
            Check.Require(!string.IsNullOrWhiteSpace(userName));

		    var viewModel = new AutoApprovalViewModel
		                        {
		                            AutoApproval = new AutoApproval { IsActive = true, Expiration = DateTime.Now.AddYears(1) }
		                        };

            var workgroups = repository.OfType<WorkgroupPermission>().Queryable.Where(a => a.Role != null && a.Role.Id == Role.Codes.Approver && a.User != null && a.User.Id == userName).Select(b => b.Workgroup).Distinct().ToList();
            viewModel.Accounts = repository.OfType<WorkgroupAccount>().Queryable.Where(a => a.Approver != null && a.Approver.Id == userName && a.Account.IsActive).Select(b => b.Account).ToList(); //workgroups.SelectMany(a => a.Accounts).Select(b => b.Account).Distinct().ToList();
            viewModel.Users = workgroups.SelectMany(a => a.Permissions).Where(b => b.Role != null && b.Role.Id == Role.Codes.Requester).Select(c => c.User).Where(c => c.IsActive).Distinct().ToList();

		    return viewModel;
		}
	}
}
