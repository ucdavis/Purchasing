

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Web.Attributes;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the WorkgroupPermissions class
    /// </summary>
    public class WorkgroupPermissionsController : ApplicationController
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IWorkgroupService _workgroupService;
        private readonly string[] _validRoleCodes = new []
        {
            Role.Codes.Reviewer, Role.Codes.Requester, Role.Codes.Approver,
            Role.Codes.AccountManager, Role.Codes.Purchaser
        };
        private const string AddAction = "A";
        private const string RemoveAction = "D";
        private readonly string[] _validActions = new[]{AddAction, RemoveAction};


        public WorkgroupPermissionsController(IRepositoryFactory repositoryFactory,
            IWorkgroupService workgroupService)
        {
            _repositoryFactory = repositoryFactory;
            _workgroupService = workgroupService;
        }

        [FilterIP(AllowedIPs = "169.237.124.237;127.0.0.1")] //JCS -- My IP. So I can test
        [HttpPost]
        [BypassAntiForgeryToken]
        public JsonNetResult Update(Guid id, Permission[] permissions)
        {
            var result = "Failure";
            var added = 0;
            var deleted = 0;
            var failed = 0;
            var alreadyDeleted = 0;
            var alreadyAdded = 0;

            try
            {
                var wsl = new WorkgroupSyncLog();


                var workgroup = _repositoryFactory.WorkgroupRepository.Queryable.Single(a => a.SyncKey == id);
                result = "Success";
                foreach (var permission in permissions)
                {
                    if (!Validate(workgroup, permission))
                    {
                        result = "Errors";
                        continue;
                    }

                    permission.UserId = permission.UserId.ToLower();
                    if (permission.Action == RemoveAction)
                    {
                        var user = _repositoryFactory.UserRepository.GetNullableById(permission.UserId);
                        if(user == null)
                        {
                            result = "Errors";
                            wsl = new WorkgroupSyncLog();
                            wsl.WorkGroup = workgroup;
                            wsl.NameAndId = permission.UserId;
                            wsl.Action = permission.Action;
                            wsl.Message = "User not found";
                            Repository.OfType<WorkgroupSyncLog>().EnsurePersistent(wsl);
                            continue;
                        }
                        var workgroupPermission = _repositoryFactory.WorkgroupPermissionRepository.Queryable
                            .SingleOrDefault(a =>a.Workgroup == workgroup && a.User == user && a.Role.Id == permission.RoleId && !a.IsAdmin);
                        if(workgroupPermission == null)
                        {
                            alreadyDeleted++;
                        }
                        else
                        {
                            _workgroupService.RemoveFromCache(workgroupPermission);

                            var relatedPermissionsToDelete =
                                _repositoryFactory.WorkgroupPermissionRepository.Queryable.Where(
                                    a =>
                                    a.ParentWorkgroup == workgroup && a.User == workgroupPermission.User &&
                                    a.Role == workgroupPermission.Role).ToList();

                            _workgroupService.RemoveUserFromAccounts(workgroupPermission);
                            _workgroupService.RemoveUserFromPendingApprovals(workgroupPermission); // TODO: Check for pending/open orders for this person. Set order to workgroup.
                            _repositoryFactory.WorkgroupPermissionRepository.Remove(workgroupPermission);
                            deleted++;

                            foreach (var relatedWgp in relatedPermissionsToDelete)
                            {
                                _workgroupService.RemoveFromCache(relatedWgp);
                                _repositoryFactory.WorkgroupPermissionRepository.Remove(relatedWgp);
                            }
                        }
                    }
                    if(permission.Action == AddAction)
                    {
                        var role = _repositoryFactory.RoleRepository.Queryable.Single(a => a.Id == permission.RoleId);
                        var notAddedKvp = new List<KeyValuePair<string, string>>();

                        int successCount = 0;
                        int failCount = 0;
                        int duplicateCount = 0;

                        successCount = _workgroupService.TryToAddPeople(workgroup.Id, role, workgroup, successCount, permission.UserId, ref failCount, ref duplicateCount, notAddedKvp);
                        if (successCount == 0 && failCount == 1 && duplicateCount == 0)
                        {
                            result = "Errors";
                            wsl = new WorkgroupSyncLog();
                            wsl.WorkGroup = workgroup;
                            wsl.NameAndId = permission.UserId;
                            wsl.Action = permission.Action;
                            wsl.Message = "User not found (LDAP)";
                            Repository.OfType<WorkgroupSyncLog>().EnsurePersistent(wsl);
                            failed++;
                            continue;
                        }
                        added += successCount;
                        alreadyAdded += duplicateCount;

                    }
                }
            }
            catch (Exception ex)
            {
                result = "Failure";
                return new JsonNetResult(new { result, added, deleted, alreadyAdded, alreadyDeleted, failed });
            }
            return new JsonNetResult(new { result, added, deleted, alreadyAdded, alreadyDeleted, failed });
        }


        private bool Validate(Workgroup workgroup, Permission permission )
        {
            var wsl = new WorkgroupSyncLog();
            if (!_validRoleCodes.Contains(permission.RoleId))
            {
                wsl = new WorkgroupSyncLog();
                wsl.WorkGroup = workgroup;
                wsl.NameAndId = permission.UserId;
                wsl.Action = "E";
                wsl.Message = string.Format("Invalid Role: {0}", permission.RoleId);
                Repository.OfType<WorkgroupSyncLog>().EnsurePersistent(wsl);

                return false;
            }

            if (!_validActions.Contains(permission.Action))
            {
                wsl = new WorkgroupSyncLog();
                wsl.WorkGroup = workgroup;
                wsl.NameAndId = permission.UserId;
                wsl.Action = "E";
                wsl.Message = string.Format("Invalid Action: {0}", permission.Action);
                Repository.OfType<WorkgroupSyncLog>().EnsurePersistent(wsl);

                return false;
            }

            if (workgroup.Administrative && permission.RoleId == Role.Codes.Reviewer)
            {
                wsl = new WorkgroupSyncLog();
                wsl.WorkGroup = workgroup;
                wsl.NameAndId = permission.UserId;
                wsl.Action = "E";
                wsl.Message = "Invalid Role for Admin Workgroup";
                Repository.OfType<WorkgroupSyncLog>().EnsurePersistent(wsl);

                return false;
            }

            return true;
        }

        [Authorize(Roles = Role.Codes.Admin)]
        public ActionResult Test()
        {
            return View();
        }

    }

    public class Permission
    {
        public string UserId { get; set; }
        public string Action { get; set; }
        public string RoleId { get; set; }
    }


}
