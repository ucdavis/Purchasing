using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Models;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Helpers;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// Controller for the AutoApproval class
    /// </summary>
    [AuthorizeWorkgroupRole(Role.Codes.Approver)]
    public class AutoApprovalController : ApplicationController
    {
	    private readonly IRepository<AutoApproval> _autoApprovalRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IMapper _mapper;

        public AutoApprovalController(IRepository<AutoApproval> autoApprovalRepository, IRepositoryWithTypedId<User,string> userRepository, IMapper mapper)
        {
            _autoApprovalRepository = autoApprovalRepository;
            _userRepository = userRepository;
            _mapper = mapper;
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

            //TODO: Check to make sure user is an approver

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
                return this.RedirectToAction(nameof(Index), new { showAll });
            }

            if(autoApproval.User.Id != CurrentUser.Identity.Name)
            {
                ErrorMessage = "No Access";
                return this.RedirectToAction(nameof(ErrorController.Index), typeof(ErrorController).ControllerName());
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

        /// <summary>
        /// #4
        /// POST: /AutoApproval/Create
        /// </summary>
        /// <param name="autoApproval">auto approval to create</param>
        /// <param name="showAll">show inactive and expired too(Used to pass along to index)</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(AutoApproval autoApproval, bool showAll = false)
        {
            autoApproval.Equal = !autoApproval.LessThan; //only one can be true, the other must be false
            autoApproval.User = _userRepository.GetNullableById(CurrentUser.Identity.Name);            
            ModelState.Clear();
            autoApproval.TransferValidationMessagesTo(ModelState);
            if(autoApproval.Expiration.HasValue && autoApproval.Expiration.Value.Date <= DateTime.UtcNow.ToPacificTime().Date)
            {
                ModelState.AddModelError("AutoApproval.Expiration", "Expiration date has already passed");
            }
            autoApproval.IsActive = true;

            if (ModelState.IsValid)
            {
                _autoApprovalRepository.EnsurePersistent(autoApproval);

                Message = "AutoApproval Created Successfully";
                if(autoApproval.Expiration.HasValue && autoApproval.Expiration.Value.Date <= DateTime.UtcNow.ToPacificTime().Date.AddDays(5))
                {
                    Message = Message + " Warning, will expire in 5 days or less";
                }
                return this.RedirectToAction(nameof(Index), new { showAll });
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

        /// <summary>
        /// #5
        /// GET: /AutoApproval/Edit/5
        /// </summary>
        /// <param name="id">AutoApproval Id</param>
        /// <param name="showAll">show inactive and expired too(Used to pass along to index)</param>
        /// <returns></returns>
        public ActionResult Edit(int id, bool showAll = false)
        {
            var autoApproval = _autoApprovalRepository.GetNullableById(id);

            if (autoApproval == null)
            {
                return this.RedirectToAction(nameof(Index), new { showAll });
            }

            if(autoApproval.User.Id != CurrentUser.Identity.Name)
            {
                ErrorMessage = "No Access";
                return this.RedirectToAction(nameof(ErrorController.Index), typeof(ErrorController).ControllerName());
            }

            var viewModel = AutoApprovalViewModel.Create(Repository, CurrentUser.Identity.Name);
			viewModel.AutoApproval = autoApproval;
            ViewBag.ShowAll = showAll;
            ViewBag.IsCreate = false;

			return View(viewModel);
        }
        
        /// <summary>
        /// #6
        /// POST: /AutoApproval/Edit/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="autoApproval"></param>
        /// <param name="showAll"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, AutoApproval autoApproval, bool showAll = false)
        {
            var autoApprovalToEdit = _autoApprovalRepository.GetNullableById(id);

            if (autoApprovalToEdit == null)
            {
                return this.RedirectToAction(nameof(Index), new { showAll });
            }

            if(autoApprovalToEdit.User.Id != CurrentUser.Identity.Name)
            {
                ErrorMessage = "No Access";
                return this.RedirectToAction(nameof(ErrorController.Index), typeof(ErrorController).ControllerName());
            }

            TransferValues(autoApproval, autoApprovalToEdit);
            autoApprovalToEdit.Equal = !autoApprovalToEdit.LessThan;

            ModelState.Clear();
            autoApprovalToEdit.TransferValidationMessagesTo(ModelState);

            if (ModelState.IsValid)
            {
                _autoApprovalRepository.EnsurePersistent(autoApprovalToEdit);

                Message = "AutoApproval Edited Successfully";

                return this.RedirectToAction(nameof(Index), new { showAll });
            }
            else
            {
                var viewModel = AutoApprovalViewModel.Create(Repository, CurrentUser.Identity.Name);
                viewModel.AutoApproval = autoApprovalToEdit;
                ViewBag.ShowAll = showAll;
                ViewBag.IsCreate = false;

                return View(viewModel);
            }
        }
        
        /// <summary>
        /// #7
        /// GET: /AutoApproval/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="showAll"></param>
        /// <returns></returns>
        public ActionResult Delete(int id, bool showAll = false)
        {
			var autoApproval = _autoApprovalRepository.GetNullableById(id);

            if (autoApproval == null)
            {
                return this.RedirectToAction(nameof(Index), new { showAll });
            }

            if(autoApproval.User.Id != CurrentUser.Identity.Name)
            {
                ErrorMessage = "No Access";
                return this.RedirectToAction(nameof(ErrorController.Index), typeof(ErrorController).ControllerName());
            }
            if(!autoApproval.IsActive)
            {
                Message = "Already deactivated";
                return this.RedirectToAction(nameof(Index), new { showAll });
            }

            ViewBag.ShowAll = showAll;


            return View(autoApproval);
        }

        /// <summary>
        /// #8
        /// POST: /AutoApproval/Delete/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="autoApproval"></param>
        /// <param name="showAll"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, AutoApproval autoApproval, bool showAll = false)
        {
			var autoApprovalToDelete = _autoApprovalRepository.GetNullableById(id);

            if (autoApprovalToDelete == null)
            {
                return this.RedirectToAction(nameof(Index), new { showAll });
            }

            if(autoApprovalToDelete.User.Id != CurrentUser.Identity.Name)
            {
                ErrorMessage = "No Access";
                return this.RedirectToAction(nameof(ErrorController.Index), typeof(ErrorController).ControllerName());
            }

            if(autoApprovalToDelete.IsActive)
            {
                autoApprovalToDelete.IsActive = false;

                _autoApprovalRepository.EnsurePersistent(autoApprovalToDelete);

                Message = "AutoApproval Deactivated Successfully";
            }
            return this.RedirectToAction(nameof(Index), new { showAll });
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private void TransferValues(AutoApproval source, AutoApproval destination)
        {
            _mapper.Map(source, destination);
        }


    }
}
