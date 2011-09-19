using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.Helpers;

namespace Purchasing.Web.Controllers.Dev
{
    /// <summary>
    /// Controller for the AutoApproval class
    /// </summary>
    [Authorize]
    public class AutoApprovalController : ApplicationController
    {
	    private readonly IRepository<AutoApproval> _autoApprovalRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;

        public AutoApprovalController(IRepository<AutoApproval> autoApprovalRepository, IRepositoryWithTypedId<User,string> userRepository)
        {
            _autoApprovalRepository = autoApprovalRepository;
            _userRepository = userRepository;
        }

        //
        // GET: /AutoApproval/
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


        //
        // GET: /AutoApproval/Details/5
        public ActionResult Details(int id)
        {
            var autoApproval = _autoApprovalRepository.GetNullableById(id);

            if (autoApproval == null) return RedirectToAction("Index");

            return View(autoApproval);
        }

        //
        // GET: /AutoApproval/Create
        public ActionResult Create()
        {
			var viewModel = AutoApprovalViewModel.Create(Repository);

            return View(viewModel);
        } 

        //
        // POST: /AutoApproval/Create
        [HttpPost]
        public ActionResult Create(AutoApproval autoApproval)
        {
            autoApproval.Equal = !autoApproval.LessThan; //only one can be true, the other must be false
            autoApproval.User = _userRepository.GetById(CurrentUser.Identity.Name);
            ModelState.Clear();
            autoApproval.TransferValidationMessagesTo(ModelState);


            if (ModelState.IsValid)
            {
                _autoApprovalRepository.EnsurePersistent(autoApproval);

                Message = "AutoApproval Created Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = AutoApprovalViewModel.Create(Repository);
                viewModel.AutoApproval = autoApproval;

                return View(viewModel);
            }
        }

        //
        // GET: /AutoApproval/Edit/5
        public ActionResult Edit(int id)
        {
            var autoApproval = _autoApprovalRepository.GetNullableById(id);

            if (autoApproval == null) return RedirectToAction("Index");

			var viewModel = AutoApprovalViewModel.Create(Repository);
			viewModel.AutoApproval = autoApproval;

			return View(viewModel);
        }
        
        //
        // POST: /AutoApproval/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, AutoApproval autoApproval)
        {
            var autoApprovalToEdit = _autoApprovalRepository.GetNullableById(id);

            if (autoApprovalToEdit == null) return RedirectToAction("Index");

            TransferValues(autoApproval, autoApprovalToEdit);

            if (ModelState.IsValid)
            {
                _autoApprovalRepository.EnsurePersistent(autoApprovalToEdit);

                Message = "AutoApproval Edited Successfully";

                return RedirectToAction("Index");
            }
            else
            {
				var viewModel = AutoApprovalViewModel.Create(Repository);
                viewModel.AutoApproval = autoApproval;

                return View(viewModel);
            }
        }
        
        //
        // GET: /AutoApproval/Delete/5 
        public ActionResult Delete(int id)
        {
			var autoApproval = _autoApprovalRepository.GetNullableById(id);

            if (autoApproval == null) return RedirectToAction("Index");

            return View(autoApproval);
        }

        //
        // POST: /AutoApproval/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, AutoApproval autoApproval)
        {
			var autoApprovalToDelete = _autoApprovalRepository.GetNullableById(id);

            if (autoApprovalToDelete == null) return RedirectToAction("Index");

            _autoApprovalRepository.Remove(autoApprovalToDelete);

            Message = "AutoApproval Removed Successfully";

            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(AutoApproval source, AutoApproval destination)
        {
			//Recommendation: Use AutoMapper
			//Mapper.Map(source, destination)
            throw new NotImplementedException();
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
                temp = temp.Where(a => a.IsActive);
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
 
		public static AutoApprovalViewModel Create(IRepository repository)
		{
			Check.Require(repository != null, "Repository must be supplied");

		    var viewModel = new AutoApprovalViewModel
		                        {
		                            AutoApproval = new AutoApproval { IsActive = true, Expiration = DateTime.Now.AddYears(1) },
                                    Accounts = repository.OfType<Account>().GetAll()//TODO: filter or setup a multi-select
		                        };

		    return viewModel;
		}
	}
}
