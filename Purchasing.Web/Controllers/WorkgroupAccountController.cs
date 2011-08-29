using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Purchasing.Core.Domain;
using Purchasing.Web.Services;
using Purchasing.Web.Utility;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Helpers;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the WorkgroupAccount class
    /// </summary>
    [Authorize]
    public class WorkgroupAccountController : ApplicationController
    {
        private readonly IRepository<WorkgroupAccount> _workgroupAccountRepository;
        private readonly IRepository<WorkgroupAccountPermission> _workgroupAccountPermissionRepository; 
        private readonly IRepositoryWithTypedId<Account, string> _accountRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<Role, string> _roleRepository;
        private readonly IDirectorySearchService _directorySearchService;

        public WorkgroupAccountController(IRepository<WorkgroupAccount> workgroupAccountRepository, IRepository<WorkgroupAccountPermission> workgroupAccountPermisionRepository, IRepositoryWithTypedId<Account, string> accountRepository, IRepositoryWithTypedId<User, string> userRespository, IRepositoryWithTypedId<Role,string> roleRepository, IDirectorySearchService directorySearchService )
        {
            _workgroupAccountRepository = workgroupAccountRepository;
            _workgroupAccountPermissionRepository = workgroupAccountPermisionRepository;
            _accountRepository = accountRepository;
            _userRepository = userRespository;
            _roleRepository = roleRepository;
            _directorySearchService = directorySearchService;
        }

        #region Old Code

        //
        // GET: /WorkgroupAccount/
        public ActionResult Index(int id)
        {
            if (!CurrentUser.IsInRole(Role.Codes.DepartmentalAdmin))
            {
                Message = "You must be a department admin to access a workgroup's accounts";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            var workgroup = Repository.OfType<Workgroup>().GetNullableById(id);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            var viewModel = WorkgroupListViewModel.Create(_workgroupAccountRepository, id);

            return View(viewModel);
        }

        //
        // GET: /WorkgroupAccount/Details/5
        public ActionResult Details(int id)
        {
            var workgroupAccount = _workgroupAccountRepository.GetNullableById(id);

            if (workgroupAccount == null) return RedirectToAction("Index");

            return View(workgroupAccount);
        }


        /// <summary>
        /// GET: /WorkgroupAccount/Create  
        /// </summary>
        /// <param name="id">Workgroup ID</param>
        /// <returns></returns>
        public ActionResult Create(int id)
        {
            var workgroup = Repository.OfType<Workgroup>().GetNullableById(id);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            var viewModel = WorkgroupAccountViewModel.Create(Repository, workgroup);

            return View(viewModel);
        }

        public JsonNetResult SearchAccounts(string searchTerm, int id)
        {
            var workgroupAccounts = Repository.OfType<WorkgroupAccount>().GetNullableById(id);
            //Add logic to exclude current workgroup accounts!
            var results = _accountRepository.Queryable.Where((a => a.Name.Contains(searchTerm) || a.Id.Contains(searchTerm))).Select(a => new IdAndName(a.Id, a.Name)).ToList();

            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.Name }));
        }

        public JsonNetResult SearchUsers(string searchTerm)
        {
            var results =
                _userRepository.Queryable.Where(a => a.LastName.Contains(searchTerm) || a.FirstName.Contains(searchTerm)).Select(a => new IdAndName(a.Id, string.Format("{0} {1}", a.FirstName, a.LastName))).ToList();

            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.Name }));
        }

        /// <summary>
        /// POST: /WorkgroupAccount/Create
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="accounts">list of Account Ids</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(int id, string[] accounts, string[] approvers, string[] managers, string[] purchasers)
        {
            var workgroup = Repository.OfType<Workgroup>().GetNullableById(id);
            Check.Require(workgroup != null);

            var addedCount = 0;
            var errorCount = 0;
            foreach (var account in accounts)
            {
                string account1 = account;
                if (_workgroupAccountRepository.Queryable.Where(a => a.Workgroup != null && a.Workgroup.Id == id && a.Account.Id == account1).Any())
                {
                    //Already there

                    continue;
                }
                var workgroupAccountToCreate = new WorkgroupAccount();
                workgroupAccountToCreate.Workgroup = workgroup;
                workgroupAccountToCreate.Account = _accountRepository.GetNullableById(account1);
                ModelState.Clear();
                workgroupAccountToCreate.TransferValidationMessagesTo(ModelState);
                if (approvers.Length > 0)
                {
                    foreach (var approver in approvers)
                    {
                         var workgroupAccountPermissionToCreate = new WorkgroupAccountPermission();
                        //Need Role Codes for approver, account manager, and purchaser!
                        //Not sure how to save this role
                        workgroupAccountPermissionToCreate.Role = _roleRepository.GetById(Role.Codes.Admin);
                        workgroupAccountPermissionToCreate.User = _userRepository.GetNullableById(approver);
                        _workgroupAccountPermissionRepository.EnsurePersistent(workgroupAccountPermissionToCreate);
                    }
                }
                if (managers.Length > 0)
                {
                    foreach (var manager in managers)
                    {
                        var workgroupAccountPermissionToCreate = new WorkgroupAccountPermission();
                        //Need Role Codes for approver, account manager, and purchaser!
                        //Not sure how to save this role
                        workgroupAccountPermissionToCreate.Role = _roleRepository.GetById(Role.Codes.Admin);
                        workgroupAccountPermissionToCreate.User = _userRepository.GetNullableById(manager);
                        _workgroupAccountPermissionRepository.EnsurePersistent(workgroupAccountPermissionToCreate);
                    }
                }
                if (purchasers.Length > 0)
                {
                    foreach (var purchaser in purchasers)
                    {
                        var workgroupAccountPermissionToCreate = new WorkgroupAccountPermission();
                        //Need Role Codes for approver, account manager, and purchaser!
                        //Not sure how to save this role
                        workgroupAccountPermissionToCreate.Role = _roleRepository.GetById(Role.Codes.Admin);
                        workgroupAccountPermissionToCreate.User = _userRepository.GetNullableById(purchaser);
                        _workgroupAccountPermissionRepository.EnsurePersistent(workgroupAccountPermissionToCreate);
                    }
                }

                if (ModelState.IsValid)
                {
                    _workgroupAccountRepository.EnsurePersistent(workgroupAccountToCreate);
                    addedCount++;
                }
                else
                {
                    errorCount++;
                }


            }
            Message = string.Format("{0}: Added", addedCount);
            if (errorCount > 0)
            {
                var viewModel = WorkgroupAccountViewModel.Create(Repository, workgroup);
                var accountsList = _accountRepository.Queryable.Where(a => accounts.Contains(a.Id)).Select(a => new IdAndName(a.Id, a.Name)).ToList();
                viewModel.Accounts = accountsList;
                return View(viewModel);
            }
            return this.RedirectToAction<WorkgroupAccountController>(a => a.Index(id));


        }

        //
        // GET: /WorkgroupAccount/Edit/5
        public ActionResult Edit(int id)
        {
            var workgroupAccount = _workgroupAccountRepository.GetNullableById(id);

            if (workgroupAccount == null) return RedirectToAction("Index");

            var viewModel = WorkgroupAccountViewModel.Create(Repository, null);
            viewModel.WorkgroupAccount = workgroupAccount;

            return View(viewModel);
        }

        //
        // POST: /WorkgroupAccount/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, WorkgroupAccount workgroupAccount)
        {
            var workgroupAccountToEdit = _workgroupAccountRepository.GetNullableById(id);

            if (workgroupAccountToEdit == null) return RedirectToAction("Index");

            TransferValues(workgroupAccount, workgroupAccountToEdit);

            if (ModelState.IsValid)
            {
                _workgroupAccountRepository.EnsurePersistent(workgroupAccountToEdit);

                Message = "WorkgroupAccount Edited Successfully";

                return RedirectToAction("Index");
            }
            else
            {
                var viewModel = WorkgroupAccountViewModel.Create(Repository, null);
                viewModel.WorkgroupAccount = workgroupAccount;

                return View(viewModel);
            }
        }

        //
        // GET: /WorkgroupAccount/Delete/5 
        public ActionResult Delete(int id)
        {
            var workgroupAccount = _workgroupAccountRepository.GetNullableById(id);

            if (workgroupAccount == null) return RedirectToAction("Index");

            return View(workgroupAccount);
        }

        //
        // POST: /WorkgroupAccount/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, WorkgroupAccount workgroupAccount)
        {
            var workgroupAccountToDelete = _workgroupAccountRepository.GetNullableById(id);

            if (workgroupAccountToDelete == null) return RedirectToAction("Index");

            _workgroupAccountRepository.Remove(workgroupAccountToDelete);

            Message = "WorkgroupAccount Removed Successfully";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Transfer editable values from source to destination
        /// </summary>
        private static void TransferValues(WorkgroupAccount source, WorkgroupAccount destination)
        {
            //Recommendation: Use AutoMapper
            //Mapper.Map(source, destination)
            throw new NotImplementedException();
        }

        #endregion


        /// <summary>
        /// Edit of Accounts for a workgroup
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AssignAccount(int id)
        {
            var workgroup = Repository.OfType<Workgroup>().Queryable.FirstOrDefault();

            var viewModel = AssignAccountViewModel.Create(Repository, workgroup);

            return View(viewModel);
        }

        /// <summary>
        /// Add accounts to a workgroup
        /// </summary>
        /// <param name="id">workgroup Id</para>
        /// <returns></returns>
        public ActionResult CreateWorkgroupAccount(int id)
        {
            var workgroup = Repository.OfType<Workgroup>().Queryable.FirstOrDefault();
            return View(workgroup);
        }

        [HttpPost]
        public ActionResult CreateWorkgroupAccount(int id, WorkgroupAccountPostModel workgroupAccountPostModel)
        {
            var workgroup = Repository.OfType<Workgroup>().GetNullableById(id);

            if (workgroup == null || workgroupAccountPostModel.Accounts == null || workgroupAccountPostModel.Accounts.Count <= 0) return this.RedirectToAction("Index", "Error");

            var workgroupAccounts = new List<WorkgroupAccount>();

            foreach (var a in workgroupAccountPostModel.Accounts)
            {
                // does it already exist in the workgroup?
                if (!workgroup.Accounts.Select(b => b.Account.Id).Contains(a))
                {
                    // no, get it and create new workgroup account
                    var account = Repository.OfType<Account>().Queryable.Where(b => b.Id == a).FirstOrDefault();

                    if (account != null)
                    {
                        workgroupAccounts.Add(new WorkgroupAccount(){Account = account, Workgroup = workgroup});
                    }
                }
            }

            // have any account managers?
            AddUsersToRole(workgroupAccounts, workgroupAccountPostModel.AccountManagers, Role.Codes.AccountManager);
            // approvers
            AddUsersToRole(workgroupAccounts, workgroupAccountPostModel.Approvers, Role.Codes.Approver);
            // purchasers
            AddUsersToRole(workgroupAccounts, workgroupAccountPostModel.Purchasers, Role.Codes.Purchaser);

            Message = "Successfully Added Accounts";

            return RedirectToAction("AssignAccount", new {id=id});
        }

        private void AddUsersToRole(List<WorkgroupAccount> workgroupAccounts, List<string> peeps, string roleId )
        {
            if (peeps != null && peeps.Count > 0)
            {
                foreach (var uid in peeps)
                {
                    // get the user
                    var user = _userRepository.GetNullableById(uid);

                    if (user == null)
                    {

                        var person = _directorySearchService.FindUser(uid);

                        if (person != null)
                        {
                            user = new User(person.LoginId) { FirstName = person.FirstName, LastName = person.LastName, Email = person.EmailAddress };
                        }

                    }

                    // if stil null, didn't find
                    if (user != null)
                    {
                        var role = _roleRepository.GetNullableById(roleId);

                        foreach (var wa in workgroupAccounts)
                        {
                            var wap = new WorkgroupAccountPermission() {WorkgroupAccount = wa, User = user, Role = role};
                            _workgroupAccountPermissionRepository.EnsurePersistent(wap);
                        }
                        
                    }
                }
            }            
        }

        /// <summary>
        /// Ajax function to search for people
        /// </summary>
        /// <param name="loginId"></param>
        /// <returns></returns>
        public JsonNetResult SearchPerson(string loginId)
        {
            // swap this out with an ldap search
            var user = _userRepository.Queryable.Where(a => a.Id == loginId).FirstOrDefault();

            if (user != null)
            {
                return new JsonNetResult(new {result=true, loginId=user.Id, name=user.FullName});
            }

            return new JsonNetResult(new {result=false});

        }

        /// <summary>
        /// Ajax function to search accounts
        /// </summary>
        /// <param name="loginId"></param>
        /// <returns></returns>
        public JsonNetResult SearchAccount(string searchTerm)
        {
            var results = Repository.OfType<Account>().Queryable.Where(a => a.Name.Contains(searchTerm) || a.Id.Contains(searchTerm)).Select(a => new IdAndName(a.Id, a.Name)).ToList();

            return new JsonNetResult(results.Select(a => new { id = a.Id, label = a.DisplayNameAndId }));
        }

    }

    /// <summary>
    /// ViewModel for the WorkgroupAccount class
    /// </summary>
    public class WorkgroupAccountViewModel
    {
        public WorkgroupAccount WorkgroupAccount { get; set; }
        public List<IdAndName> Accounts { get; set; }
        public Workgroup Workgroup { get; set; }
        public List<IdAndName> Users { get; set; }


        public static WorkgroupAccountViewModel Create(IRepository repository, Workgroup workgroup)
        {
            Check.Require(repository != null, "Repository must be supplied");
            //var accounts = repository.OfType<Account>().Queryable.Where(a => a.IsActive);
            var viewModel = new WorkgroupAccountViewModel { WorkgroupAccount = new WorkgroupAccount(), Workgroup = workgroup };
            viewModel.WorkgroupAccount.Workgroup = workgroup;
            // viewModel.Accounts = viewModel.Workgroup.Accounts.AsEnumerable();
            return viewModel;
        }


    }

    public class WorkgroupListViewModel
    {
        public int WorkgroupId { get; set; }
        public List<WorkgroupAccount> WorkgroupAccountList { get; set; }

        public static WorkgroupListViewModel Create(IRepository<WorkgroupAccount> workgroupAccountRepository, int id)
        {
            var viewModel = new WorkgroupListViewModel() { WorkgroupId = id };
            viewModel.WorkgroupAccountList = workgroupAccountRepository.Queryable.Where(a => a.Workgroup != null && a.Workgroup.Id == id).ToList();
            return viewModel;
        }
    }

    //public class WorkgroupAccountPostModel
    //{
    //    public List<Account> Accounts { get; set; }


    //}

    public class AssignAccountViewModel
    {
        public Workgroup Workgroup { get; set; }
        public IEnumerable<Account> Accounts { get; set; } 

        public static AssignAccountViewModel Create(IRepository repository, Workgroup workgroup = null)
        {
            Check.Require(repository != null, "Repository is required.");

            var accounts = new List<Account>();

            foreach (var a in workgroup.Organizations)
            {
                accounts.AddRange(a.Accounts);
            }

            var viewModel = new AssignAccountViewModel()
                                {
                                    Workgroup = workgroup ?? new Workgroup(),
                                    Accounts = accounts
                                };

            return viewModel;
        }
    }

    public class WorkgroupAccountPostModel
    {
        public List<string> Accounts { get; set; }
        public List<string> AccountManagers { get; set; }
        public List<string> Approvers { get; set; }
        public List<string> Purchasers { get; set; }
    }

}

