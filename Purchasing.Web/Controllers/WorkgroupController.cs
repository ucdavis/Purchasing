using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Web.Models;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using Purchasing.Core.Domain;
using AutoMapper;
using System.Collections.Generic;
using UCDArch.Web.ActionResults;
using Purchasing.Web.Utility;
using MvcContrib;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Workgroup class
    /// </summary>
    public class WorkgroupController : ApplicationController
    {
	    private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<Role, string> _roleRepository;
        private readonly IRepository<WorkgroupPermission> _workgroupPermissionRepository;
        private readonly IHasAccessService _hasAccessService;
        private readonly IDirectorySearchService _searchService;

        public WorkgroupController(IRepository<Workgroup> workgroupRepository, 
            IRepositoryWithTypedId<User, string> userRepository, 
            IRepositoryWithTypedId<Role, string> roleRepository, 
            IRepository<WorkgroupPermission> workgroupPermissionRepository,
            IHasAccessService hasAccessService, IDirectorySearchService searchService)
        {
            _workgroupRepository = workgroupRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _workgroupPermissionRepository = workgroupPermissionRepository;
            _hasAccessService = hasAccessService;
            _searchService = searchService;
        }

        #region Workgroup Actions
        /// <summary>
        /// Actions #1
        /// GET: /Workgroup/
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var person =
                _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Fetch(x => x.Organizations).Single();

            var orgIds = person.Organizations.Select(x => x.Id).ToArray();

            var workgroupList =
                _workgroupRepository.Queryable.Where(x => x.Organizations.Any(a => orgIds.Contains(a.Id)));

            return View(workgroupList.ToList());
        }

        /// <summary>
        /// Actions #2
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var user = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Single();

            var model = WorkgroupModifyModel.Create(Repository, user);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Workgroup workgroup, string[] selectedOrganizations)
        {
            if (!ModelState.IsValid)
            {
                var model = WorkgroupModifyModel.Create(Repository, GetCurrentUser());
                model.Workgroup = workgroup;

                return View(model);
            }

            var workgroupToCreate = new Workgroup();

            Mapper.Map(workgroup, workgroupToCreate);

            if (selectedOrganizations != null)
            {
                workgroupToCreate.Organizations =
                    Repository.OfType<Organization>().Queryable.Where(a => selectedOrganizations.Contains(a.Id)).ToList();
            }

            if (!workgroupToCreate.Organizations.Contains(workgroupToCreate.PrimaryOrganization))
            {
                workgroupToCreate.Organizations.Add(workgroupToCreate.PrimaryOrganization);
            }

            _workgroupRepository.EnsurePersistent(workgroupToCreate);

            Message = string.Format("{0} workgroup was created",
                                    workgroup.Name);

            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            //var workgroup = _workgroupRepository.GetNullableById(Id);

            //var model = new WorkgroupViewModel
            //{
            //    Workgroup = workgroup
            //};

            //return View(model);

            var workgroup = _workgroupRepository.GetNullableById(id);

            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return RedirectToAction("Index");
            }

            var viewModel = WorkgroupDetailsViewModel.Create(Repository, workgroup);

            return View(viewModel);

        }

        public ActionResult Edit(int id)
        {
            var user = _userRepository.Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Single();
            var workgroup = _workgroupRepository.GetNullableById(id);

            var model = WorkgroupModifyModel.Create(Repository, user, workgroup);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Workgroup workgroup, string[] selectedOrganizations)
        {

            if (!ModelState.IsValid)
            {
                return View(new WorkgroupModifyModel { Workgroup = workgroup });
            }

            var workgroupToEdit = _workgroupRepository.GetNullableById(workgroup.Id);

            Mapper.Map(workgroup, workgroupToEdit);

            if (selectedOrganizations != null)
            {
                workgroupToEdit.Organizations =
                    Repository.OfType<Organization>().Queryable.Where(a => selectedOrganizations.Contains(a.Id)).ToList();
            }

            if (!workgroupToEdit.Organizations.Contains(workgroupToEdit.PrimaryOrganization))
            {
                workgroupToEdit.Organizations.Add(workgroupToEdit.PrimaryOrganization);
            }

            _workgroupRepository.EnsurePersistent(workgroupToEdit);

            Message = string.Format("{0} was modified successfully",
                                    workgroup.Name);

            return RedirectToAction("Index");

        }

        public ActionResult Delete(int id)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);

            var model = new WorkgroupViewModel
            {
                Workgroup = workgroup
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(WorkgroupViewModel workgroupViewModel)
        {
            var workgroup = _workgroupRepository.GetNullableById(workgroupViewModel.Workgroup.Id);

            workgroup.IsActive = false;

            _workgroupRepository.EnsurePersistent(workgroup);

            Message = string.Format("{0} was disabled successfully",
                                    workgroup.Name);

            return RedirectToAction("Index");

        }
        #endregion

        #region Workgroup Accounts
        #endregion

        #region Workgroup Vendors
        #endregion

        #region Addresses
        #endregion

        #region Workgroup People

        /// <summary>
        /// People #1
        /// List of all people within a workgroup
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="roleFilter"></param>
        /// <returns></returns>
        public ActionResult People(int id, string roleFilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                ErrorMessage = "Workgroup could not be found";
                return this.RedirectToAction(a => a.Index());
            }

            if (!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = WorgroupPeopleListModel.Create(_workgroupPermissionRepository, _roleRepository, workgroup, roleFilter);
            ViewBag.rolefilter = roleFilter;
            return View(viewModel);
        }

        /// <summary>
        /// People #2
        /// GET: add a person with a role into a workgroup
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="roleFilter"></param>
        /// <returns></returns>
        public ActionResult AddPeople(int id, string roleFilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            if (!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = WorgroupPeopleCreateModel.Create(_roleRepository, workgroup);
            if(!string.IsNullOrWhiteSpace(roleFilter))
            {
                viewModel.Role = _roleRepository.Queryable.Where(a => a.Level >= 1 && a.Level <= 4 && a.Id == roleFilter).SingleOrDefault();
            }
            ViewBag.rolefilter = roleFilter;

            return View(viewModel);
        }

        /// <summary>
        /// People #3
        /// POST: add a person with a role into a workgroup
        /// </summary>
        /// <param name="id"></param>
        /// <param name="workgroupPeoplePostModel"></param>
        /// <param name="roleFilter"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddPeople(int id, WorkgroupPeoplePostModel workgroupPeoplePostModel, string roleFilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(id);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            if (!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }


            //Ensure role picked is valid.
            if (workgroupPeoplePostModel.Role != null)
            {
                if(!_roleRepository.Queryable.Where(a => a.Level >= 1 && a.Level <= 4 && a.Id == workgroupPeoplePostModel.Role.Id).Any())
                {
                    ModelState.AddModelError("Role", "Invalid Role Selected");
                }

            }

            if (!ModelState.IsValid)
            {
                var viewModel = WorgroupPeopleCreateModel.Create(_roleRepository, workgroup);

                if (workgroupPeoplePostModel.Role != null)
                {
                    viewModel.Role = workgroupPeoplePostModel.Role;
                }
                if (workgroupPeoplePostModel.Users != null && workgroupPeoplePostModel.Users.Count > 0)
                {
                    var users = new List<IdAndName>();
                    foreach (var user in workgroupPeoplePostModel.Users)
                    {
                        var temp = _userRepository.GetNullableById(user);
                        if (temp != null)
                        {
                            users.Add(new IdAndName(temp.Id, temp.FullName));
                        }
                        else
                        {
                            var ldapuser = _searchService.FindUser(user);
                            if (ldapuser != null)
                            {
                                users.Add(new IdAndName(ldapuser.LoginId, string.Format("{0} {1}", ldapuser.FirstName, ldapuser.LastName)));
                            }
                        }
                    }
                    viewModel.Users = users;
                }
                ViewBag.rolefilter = roleFilter;
                return View(viewModel);
            }

            int successCount = 0;
            int failCount = 0;
            foreach (var u in workgroupPeoplePostModel.Users)
            {
                var user = _userRepository.GetNullableById(u);
                if (user == null)
                {
                    var ldapuser = _searchService.FindUser(u);
                    if (ldapuser != null)
                    {
                        user = new User(ldapuser.LoginId);
                        user.Email = ldapuser.EmailAddress;
                        user.FirstName = ldapuser.FirstName;
                        user.LastName = ldapuser.LastName;

                        _userRepository.EnsurePersistent(user);
                    }
                }

                if (user == null)
                {
                    //TODO: Do we want to just ignore these? Or report an error to the user?
                    continue;
                }

                if (!_workgroupPermissionRepository.Queryable.Where(a => a.Role == workgroupPeoplePostModel.Role && a.User == user && a.Workgroup == workgroup).Any())
                {
                    var workgroupPermission = new WorkgroupPermission();
                    workgroupPermission.Role = workgroupPeoplePostModel.Role;
                    workgroupPermission.User = _userRepository.GetNullableById(u);
                    workgroupPermission.Workgroup = Repository.OfType<Workgroup>().GetNullableById(id);

                    _workgroupPermissionRepository.EnsurePersistent(workgroupPermission);
                    successCount++;
                }
                else
                {
                    failCount++;
                }

            }

            Message = string.Format("Successfully added {0} people to workgroup as {1}. {2} not added because of duplicated role.", successCount,
                                    workgroupPeoplePostModel.Role.Name, failCount);

            return this.RedirectToAction(a => a.People(id, workgroupPeoplePostModel.Role.Id));

        }

        /// <summary>
        /// People #4
        /// GET: remove a person/role from a workgroup
        /// </summary>
        /// <param name="id">WorkgroupPermission ID</param>
        /// <param name="workgroupid"></param>
        /// <param name="rolefilter"></param>
        /// <returns></returns>
        public ActionResult DeletePeople(int id, int workgroupid, string rolefilter)
        {
            var workgroup = _workgroupRepository.GetNullableById(workgroupid);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            if (!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }
            var workgroupPermission = _workgroupPermissionRepository.GetNullableById(id);
            if(workgroupPermission == null)
            {
                return this.RedirectToAction(a => a.People(workgroupid, rolefilter));
            }

            if(workgroupPermission.Workgroup != workgroup) //Need this because you might have DA access to a different workgroup 
            {
                Message = "Person does not belong to workgroup.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var viewModel = WorkgroupPeopleDeleteModel.Create(_workgroupPermissionRepository, workgroupPermission);

            ViewBag.rolefilter = rolefilter;
            return View(viewModel);

        }

        /// <summary>
        /// People #5
        /// POST: remove a person/role from a workgroup
        /// </summary>
        /// <param name="id"></param>
        /// <param name="workgroupid"></param>
        /// <param name="rolefilter"></param>
        /// <param name="workgroupPermission"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeletePeople(int id, int workgroupid, string rolefilter, WorkgroupPermission workgroupPermission, string[] roles)
        {
            var workgroup = _workgroupRepository.GetNullableById(workgroupid);
            if (workgroup == null)
            {
                Message = "Workgroup not found";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            if (!_hasAccessService.DaAccessToWorkgroup(workgroup))
            {
                Message = "You must be a department admin for this workgroup to access a workgroup's people";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var workgroupPermissionToDelete = _workgroupPermissionRepository.GetNullableById(id);
            if (workgroupPermissionToDelete == null)
            {
                return this.RedirectToAction(a => a.People(workgroupid, rolefilter));
            }

            if(workgroupPermissionToDelete.Workgroup != workgroup) //Need this because you might have DA access to a different workgroup 
            {
                Message = "Person does not belong to workgroup.";
                return this.RedirectToAction<ErrorController>(a => a.Index());
            }

            var availableWorkgroupPermissions = _workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.User == workgroupPermissionToDelete.User && a.Role.Level >= 1 && a.Role.Level <= 4).ToList();
            if (availableWorkgroupPermissions.Count() == 1)
            {
                // TODO: Check for pending/open orders for this person. Set order to workgroup.
                _workgroupPermissionRepository.Remove(workgroupPermissionToDelete);
                Message = "Person successfully removed from role.";
                return this.RedirectToAction(a => a.People(workgroupid, rolefilter));
            }
            else
            {
                if (roles == null || roles.Count() == 0)
                {
                    Message = "Must select at least 1 role to delete";
                    var viewModel = WorkgroupPeopleDeleteModel.Create(_workgroupPermissionRepository, workgroupPermissionToDelete);

                    ViewBag.rolefilter = rolefilter;
                    return View(viewModel);
                }
                var removedCount = 0;
                foreach (var role in roles)
                {
                    // TODO: Check for pending/open orders for this person. Set order to workgroup.
                    var wp = _workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.User == workgroupPermissionToDelete.User && a.Role.Id == role).Single();
                    _workgroupPermissionRepository.Remove(wp);
                    removedCount++;
                }

                Message = string.Format("{0} {1} removed from {2}", removedCount, removedCount == 1 ? "role" : "roles", workgroupPermissionToDelete.User.FullName);
                return this.RedirectToAction(a => a.People(workgroupid, rolefilter));
            }


        }

        #endregion

        #region Private Helpers

        #endregion

        #region Ajax Helpers
        public JsonNetResult SearchOrganizations(string searchTerm)
        {
            var results = Repository.OfType<Organization>().Queryable.Where(a => a.Name.Contains(searchTerm) || a.Id.Contains(searchTerm)).Select(a => new IdAndName(a.Id, a.Name)).ToList();

            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.DisplayNameAndId }));
        }

        /// <summary>
        /// People #6
        /// Search Users in the User Table and LDAP lookup if none found.
        /// </summary>
        /// <param name="searchTerm">Email or LoginId</param>
        /// <returns></returns>
        public JsonNetResult SearchUsers(string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Trim();

            var users = _userRepository.Queryable.Where(a => a.Email == searchTerm || a.Id == searchTerm).ToList();
            if (users.Count == 0)
            {
                var ldapuser = _searchService.FindUser(searchTerm);
                if (ldapuser != null)
                {
                    Check.Require(!string.IsNullOrWhiteSpace(ldapuser.LoginId));
                    Check.Require(!string.IsNullOrWhiteSpace(ldapuser.EmailAddress));

                    var user = new User(ldapuser.LoginId);
                    user.Email = ldapuser.EmailAddress;
                    user.FirstName = ldapuser.FirstName;
                    user.LastName = ldapuser.LastName;

                    users.Add(user);
                }
            }

            //We don't want to show users that are not active
            var results =
                users.Where(a => a.IsActive).Select(a => new IdAndName(a.Id, string.Format("{0} {1}", a.FirstName, a.LastName))).ToList();
            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.Name }));
        }
        #endregion
    }
}
