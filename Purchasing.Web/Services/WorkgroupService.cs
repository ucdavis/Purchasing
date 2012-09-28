using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using AutoMapper;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Controllers;
using Purchasing.Web.Models;
using Purchasing.Web.Utility;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Data.NHibernate;

namespace Purchasing.Web.Services
{
    public interface IWorkgroupService
    {
        void TransferValues(WorkgroupVendor source, ref WorkgroupVendor destination);
        int TryToAddPeople(int id, Role role, Workgroup workgroup, int successCount, string lookupUser, ref int failCount, ref int duplicateCount, List<KeyValuePair<string, string>> notAddedKvp);
        int TryBulkLoadPeople(string bulk, bool isEmail, int id, Role role, Workgroup workgroup, int successCount, ref int failCount, ref int duplicateCount, List<KeyValuePair<string, string>> notAddedKvp);
        Workgroup CreateWorkgroup(Workgroup workgroup, string[] selectedOrganizations);
        void RemoveFromCache(WorkgroupPermission workgroupPermissionToDelete);
        List<int> GetChildWorkgroups(int workgroupId);
        //List<int> GetChildWorkgroups2(int workgroupId);
        //List<int> GetParentWorkgroups2(int workgroupId);
        List<int> GetParentWorkgroups(int workgroupId);
        void AddRelatedAdminUsers(Workgroup workgroup);

        IEnumerable<Workgroup> LoadAdminWorkgroups(bool showActive = false);
        void UpdateRelatedPermissions(Workgroup workgroupToEdit, WorkgroupController.WorkgroupChanged whatWasChanged);

        void RemoveUserFromAccounts(WorkgroupPermission workgroupPermission);
        void RemoveUserFromPendingApprovals(WorkgroupPermission workgroupPermission);
    }

    public class WorkgroupService : IWorkgroupService
    {
        private readonly IRepositoryWithTypedId<Vendor, string> _vendorRepository;
        private readonly IRepositoryWithTypedId<VendorAddress, Guid> _vendorAddressRepository;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<EmailPreferences, string> _emailPreferencesRepository;
        private readonly IRepository<WorkgroupPermission> _workgroupPermissionRepository;
        private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IRepositoryWithTypedId<Organization, string> _organizationRepository;
        private readonly IDirectorySearchService _searchService;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;
        private readonly IUserIdentity _userIdentity;
        

        public WorkgroupService(IRepositoryWithTypedId<Vendor, string> vendorRepository, 
            IRepositoryWithTypedId<VendorAddress, Guid> vendorAddressRepository, 
            IRepositoryWithTypedId<User, string> userRepository,
            IRepositoryWithTypedId<EmailPreferences, string> emailPreferencesRepository,
            IRepository<WorkgroupPermission> workgroupPermissionRepository,
            IRepository<Workgroup> workgroupRepository,
            IRepositoryWithTypedId<Organization, string> organizationRepository,
            IDirectorySearchService searchService, IRepositoryFactory repositoryFactory, IQueryRepositoryFactory queryRepositoryFactory, IUserIdentity userIdentity)
        {
            _vendorRepository = vendorRepository;
            _vendorAddressRepository = vendorAddressRepository;
            _userRepository = userRepository;
            _emailPreferencesRepository = emailPreferencesRepository;
            _workgroupPermissionRepository = workgroupPermissionRepository;
            _workgroupRepository = workgroupRepository;
            _organizationRepository = organizationRepository;
            _searchService = searchService;
            _repositoryFactory = repositoryFactory;
            _queryRepositoryFactory = queryRepositoryFactory;
            _userIdentity = userIdentity;
        }

        /// <summary>
        /// Common Code moved to service
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination">Note, this is a ref so tests work</param>
        public void TransferValues(WorkgroupVendor source, ref WorkgroupVendor destination)
        {
            Mapper.Map(source, destination);

            // existing vendor, set the values
            if (!string.IsNullOrWhiteSpace(source.VendorId) && !string.IsNullOrWhiteSpace(source.VendorAddressTypeCode))
            {
                var vendor = _vendorRepository.GetNullableById(source.VendorId);
                var vendorAddress = _vendorAddressRepository.Queryable.FirstOrDefault(a => a.Vendor == vendor && a.TypeCode == source.VendorAddressTypeCode);

                if (vendor != null && vendorAddress != null)
                {
                    destination.Name = vendor.Name;
                    destination.Line1 = vendorAddress.Line1;
                    destination.Line2 = vendorAddress.Line2;
                    destination.Line3 = vendorAddress.Line3;
                    destination.City = vendorAddress.City;
                    destination.State = vendorAddress.State ?? "XX"; // If vendor is in another Country like Australia, the State may/is null
                    destination.Zip = vendorAddress.Zip ?? "XXXXX";
                    destination.CountryCode = vendorAddress.CountryCode;

                    destination.Phone = vendorAddress.PhoneNumber;
                    destination.Fax = vendorAddress.FaxNumber;

                    destination.Email = vendorAddress.Email;
                    destination.Url = vendorAddress.Url;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Workgroup Id</param>
        /// <param name="role">Role being for people being added</param>
        /// <param name="workgroup">workgroup</param>
        /// <param name="successCount">how many have already been successfully added</param>
        /// <param name="lookupUser">user being added</param>
        /// <param name="failCount">count of number not added</param>
        /// <param name="notAddedKvp">list of users not added and reason why.</param>
        /// <returns></returns>
        public int TryToAddPeople(int id, Role role, Workgroup workgroup, int successCount, string lookupUser,  ref int failCount, ref int duplicateCount, List<KeyValuePair<string, string>> notAddedKvp)
        {
            var saveParm = lookupUser;
            var user = _userRepository.GetNullableById(lookupUser);
            if(user == null)
            {
                var ldapuser = _searchService.FindUser(lookupUser);
                if(ldapuser != null)
                {
                    lookupUser = ldapuser.LoginId;
                    user = _userRepository.GetNullableById(ldapuser.LoginId);
                    if(user == null)
                    {
                        user = new User(ldapuser.LoginId);
                        user.Email = ldapuser.EmailAddress;
                        user.FirstName = ldapuser.FirstName;
                        user.LastName = ldapuser.LastName;

                        _userRepository.EnsurePersistent(user);
                        _userIdentity.RemoveUserRoleFromCache(Resources.Role_CacheId, user.Id);  

                        var emailPrefs = new EmailPreferences(user.Id);
                        _emailPreferencesRepository.EnsurePersistent(emailPrefs);
                    }
                }
            }

            if(user == null)
            {
                //notAddedSb.AppendLine(string.Format("{0} : Not found", saveParm));
                notAddedKvp.Add(new KeyValuePair<string, string>(saveParm, "Not found"));
                failCount++;
                return successCount;
            }

            if(!_workgroupPermissionRepository.Queryable.Any(a => a.Role == role && a.User == user && a.Workgroup == workgroup && a.IsAdmin== false))
            {
                var workgroupPermission = new WorkgroupPermission();
                workgroupPermission.Role = role;
                workgroupPermission.User = _userRepository.GetNullableById(lookupUser);
                workgroupPermission.Workgroup = _workgroupRepository.GetNullableById(id);

                _workgroupPermissionRepository.EnsurePersistent(workgroupPermission);
                
                if (workgroup.Administrative)
                {
                    var ids = GetChildWorkgroups(workgroup.Id);
                    foreach (var childid in ids)
                    {
                        var childWorkgroup = _workgroupRepository.Queryable.Single(a => a.Id == childid);
                        if (!_workgroupPermissionRepository.Queryable.Any(a=> a.Workgroup==childWorkgroup && a.Role==role && a.User==user && a.IsAdmin && a.ParentWorkgroup==workgroup))
                        {
                            Check.Require(role.Id != Role.Codes.Requester);
                            var childPermission = new WorkgroupPermission();
                            childPermission.Role = role;
                            childPermission.User = workgroupPermission.User;
                            childPermission.Workgroup = childWorkgroup;
                            childPermission.IsAdmin = true;
                            childPermission.IsFullFeatured = workgroup.IsFullFeatured;
                            childPermission.ParentWorkgroup = workgroup;
                            _workgroupPermissionRepository.EnsurePersistent(childPermission);
                        }
                    }
                }
                // invalid the cache for the user that was just given permissions
                _userIdentity.RemoveUserRoleFromCache(Resources.Role_CacheId, workgroupPermission.User.Id);                

                successCount++;
            }
            else
            {
                //notAddedSb.AppendLine(string.Format("{0} : Is a duplicate", lookupUser));
                notAddedKvp.Add(new KeyValuePair<string, string>(lookupUser, "Is a duplicate"));
                failCount++;
                duplicateCount++;
            }

            return successCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bulk">string containing emails or kerb ids</param>
        /// <param name="isEmail">is the bulk vale an list of emails or kerb ids?</param>
        /// <param name="id">Workgroup Id</param>
        /// <param name="role">Role being for people being added</param>
        /// <param name="workgroup">workgroup</param>
        /// <param name="successCount">how many have already been successfully added</param>
        /// <param name="lookupUser">user being added</param>
        /// <param name="failCount">count of number not added</param>
        /// <param name="duplicateCount"> </param>
        /// <param name="notAddedKvp">list of users not added and reason why.</param>
        /// <returns></returns>
        public int TryBulkLoadPeople(string bulk, bool isEmail, int id, Role role, Workgroup workgroup, int successCount, ref int failCount, ref int duplicateCount, List<KeyValuePair<string, string>> notAddedKvp)
        {
            const string regexEmailPattern = @"\b[A-Z0-9._-]+@[A-Z0-9][A-Z0-9.-]{0,61}[A-Z0-9]\.[A-Z.]{2,6}\b";
            const string regexKerbPattern = @"\b[A-Z0-9]{2,10}\b";
            string pattern;

            if(isEmail)
            {
                pattern = regexEmailPattern;
            }
            else
            {
                pattern = regexKerbPattern;
            }

            // Find matches
            System.Text.RegularExpressions.MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(bulk, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            foreach(System.Text.RegularExpressions.Match match in matches)
            {
                var temp = match.ToString().ToLower();
                successCount = TryToAddPeople(id, role, workgroup, successCount, temp, ref failCount, ref duplicateCount, notAddedKvp);
            }

            return successCount;
        }

        /// <summary>
        /// Create workgroup
        /// </summary>
        /// <param name="workgroup"></param>
        /// <param name="selectedOrganizations"></param>
        /// <returns>Created Workgroup</returns>
        public Workgroup CreateWorkgroup(Workgroup workgroup, string[] selectedOrganizations)
        {
            var workgroupToCreate = new Workgroup();

            Mapper.Map(workgroup, workgroupToCreate);
            workgroupToCreate.PrimaryOrganization = workgroup.PrimaryOrganization;

            if(selectedOrganizations != null)
            {
                workgroupToCreate.Organizations =
                    _organizationRepository.Queryable.Where(a => selectedOrganizations.Contains(a.Id)).ToList();
            }

            if(!workgroupToCreate.Organizations.Contains(workgroupToCreate.PrimaryOrganization))
            {
                workgroupToCreate.Organizations.Add(workgroupToCreate.PrimaryOrganization);
            }

            // do the initial load on accounts
            if (workgroup.SyncAccounts)
            {
               // workgroupToCreate.Accounts = _repositoryFactory.AccountRepository.Queryable.Where(a => a.OrganizationId == workgroup.PrimaryOrganization.Id).Select(a => new WorkgroupAccount(){Account = a, Workgroup = workgroupToCreate}).ToList();
                var accts = _repositoryFactory.AccountRepository.Queryable.Where(a => a.OrganizationId == workgroup.PrimaryOrganization.Id && a.IsActive).ToList();
                foreach (var a in accts) workgroupToCreate.AddAccount(new WorkgroupAccount() { Account = a });
            }

            _workgroupRepository.EnsurePersistent(workgroupToCreate);

            AddRelatedAdminUsers(workgroupToCreate);

            return workgroupToCreate;
        }

        public void AddRelatedAdminUsers(Workgroup workgroup)
        {
            if (!workgroup.Administrative)
            {
                //if this isn't admin, we want to check if we should add users from admin workgroups
                var parentWorkgroupIds = GetParentWorkgroups(workgroup.Id);
                foreach (var parentWorkgroupId in parentWorkgroupIds)
                {
                    var parentWorkgroup = _repositoryFactory.WorkgroupRepository.Queryable.Single(a => a.Id == parentWorkgroupId);
                    foreach (var workgroupPermission in parentWorkgroup.Permissions)
                    {
                        if (!_workgroupPermissionRepository.Queryable.Any(a => a.Workgroup == workgroup && a.Role == workgroupPermission.Role && a.User == workgroupPermission.User && a.ParentWorkgroup == parentWorkgroup))
                        {
                            Check.Require(workgroupPermission.Role.Id != Role.Codes.Requester);
                            var wp = new WorkgroupPermission();
                            wp.Role = workgroupPermission.Role;
                            wp.User = workgroupPermission.User;
                            wp.Workgroup = workgroup;
                            wp.IsAdmin = true;
                            wp.IsFullFeatured = parentWorkgroup.IsFullFeatured;
                            wp.ParentWorkgroup = parentWorkgroup;

                            _workgroupPermissionRepository.EnsurePersistent(wp);
                        }
                    }
                }
                var workgroupPermissionsThatMightNeedToBeRemoved = _workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.IsAdmin && !parentWorkgroupIds.Contains(a.ParentWorkgroup.Id)).ToList();
                foreach (var workgroupPermission in workgroupPermissionsThatMightNeedToBeRemoved)
                {
                    _workgroupPermissionRepository.Remove(workgroupPermission);
                }
            }
            else
            {
                var wp = _workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup).ToList();

                if (wp.Count > 0)
                {                   
                    var wpActions =
                        _workgroupPermissionRepository.Queryable.Where(a => a.ParentWorkgroup == workgroup).Select(
                            b => new WorkgroupPermissionActions(b, WorkgroupPermissionActions.Actions.Delete)).ToList();
                    var ids = GetChildWorkgroups(workgroup.Id);
                    foreach (var adminWP in wp) //Go through each permission in the workgroup
                    {
                        Check.Require(adminWP.Role.Id != Role.Codes.Requester);
                        foreach (var childid in ids)
                        {
                            var wpAction =
                                wpActions.SingleOrDefault(
                                    a =>
                                    a.WorkgroupPermission.Workgroup.Id == childid &&
                                    a.WorkgroupPermission.User == adminWP.User &&
                                    a.WorkgroupPermission.Role == adminWP.Role);
                            if (wpAction != null)
                            {
                                wpAction.Action = WorkgroupPermissionActions.Actions.Nothing;
                            }
                            else
                            {
                                wpAction = new WorkgroupPermissionActions(new WorkgroupPermission(),
                                                                          WorkgroupPermissionActions.Actions.Add);
                                wpAction.WorkgroupPermission.Role = adminWP.Role;
                                wpAction.WorkgroupPermission.User = adminWP.User;
                                wpAction.WorkgroupPermission.Workgroup =
                                    _workgroupRepository.Queryable.Single(a => a.Id == childid);
                                wpAction.WorkgroupPermission.IsAdmin = true;
                                wpAction.WorkgroupPermission.IsFullFeatured = workgroup.IsFullFeatured;
                                wpAction.WorkgroupPermission.ParentWorkgroup = workgroup;
                                wpActions.Add(wpAction);
                            }
                        }
                    }
                    foreach (var wpAction in wpActions)
                    {
                        switch (wpAction.Action)
                        {
                            case WorkgroupPermissionActions.Actions.Delete:
                                _workgroupPermissionRepository.Remove(wpAction.WorkgroupPermission);
                                break;
                            case WorkgroupPermissionActions.Actions.Add:
                                _workgroupPermissionRepository.EnsurePersistent(wpAction.WorkgroupPermission);
                                break;
                            default:
                                break;
                        }
                    }
                }

            }
        }

        public void UpdateRelatedPermissions(Workgroup workgroupToEdit, WorkgroupController.WorkgroupChanged whatWasChanged)
        {
            whatWasChanged.OrganizationsChanged = false;
            if (workgroupToEdit.Organizations.Count != whatWasChanged.OriginalSubOrgIds.Count)
            {
                whatWasChanged.OrganizationsChanged = true;
            }
            else
            {
                foreach (var originalSubOrgId in whatWasChanged.OriginalSubOrgIds)
                {
                    if (!workgroupToEdit.Organizations.Any(a => a.Id == originalSubOrgId))
                    {
                        whatWasChanged.OrganizationsChanged = true;
                        break;
                    }
                }
            }

            if (!workgroupToEdit.IsActive || (whatWasChanged.AdminChanged && workgroupToEdit.Administrative == false))
            {
                //Delete any related wp
                var wps = _workgroupPermissionRepository.Queryable.Where(a => a.ParentWorkgroup == workgroupToEdit).ToList();
                foreach (var wp in wps)
                {
                    Check.Require(wp.IsAdmin);
                    _workgroupPermissionRepository.Remove(wp);
                }
            }
            else
            {
                if (whatWasChanged.IsFullFeaturedChanged)
                {
                    var wps = _workgroupPermissionRepository.Queryable.Where(a => a.ParentWorkgroup == workgroupToEdit).ToList();
                    foreach (var wp in wps)
                    {
                        Check.Require(wp.IsAdmin);
                        wp.IsFullFeatured = workgroupToEdit.IsFullFeatured;
                        _workgroupPermissionRepository.EnsurePersistent(wp);
                    }
                }

                //TODO: What about if it is now administrative. I think we have to clear out non admin wps first.
                if ((whatWasChanged.AdminChanged && workgroupToEdit.Administrative) || 
                    (whatWasChanged.OrganizationsChanged) || 
                    (whatWasChanged.IsActiveChanged && workgroupToEdit.IsActive))
                {
                    //add/update related wps
                    AddRelatedAdminUsers(workgroupToEdit);
                }
            }
            //TODO: Test
        }

        public void RemoveUserFromAccounts(WorkgroupPermission workgroupPermission)
        {
            switch (workgroupPermission.Role.Id)
            {
                case Role.Codes.Approver:
                    foreach (var workgroupAccount in workgroupPermission.Workgroup.Accounts.Where(a => a.Approver == workgroupPermission.User))
                    {
                        workgroupAccount.Approver = null;
                        _repositoryFactory.WorkgroupAccountRepository.EnsurePersistent(workgroupAccount);
                    }
                    break;
                case Role.Codes.AccountManager:
                    foreach (var workgroupAccount in workgroupPermission.Workgroup.Accounts.Where(a => a.AccountManager == workgroupPermission.User))
                    {
                        workgroupAccount.AccountManager = null;
                        _repositoryFactory.WorkgroupAccountRepository.EnsurePersistent(workgroupAccount);
                    }
                    break;
                case Role.Codes.Purchaser:
                    foreach (var workgroupAccount in workgroupPermission.Workgroup.Accounts.Where(a => a.Purchaser == workgroupPermission.User))
                    {
                        workgroupAccount.Purchaser = null;
                        _repositoryFactory.WorkgroupAccountRepository.EnsurePersistent(workgroupAccount);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// When a 
        /// </summary>
        /// <param name="workgroupPermission"></param>
        public void RemoveUserFromPendingApprovals(WorkgroupPermission workgroupPermission)
        {
            if(workgroupPermission.Role.Id != Role.Codes.Approver && workgroupPermission.Role.Id != Role.Codes.AccountManager && workgroupPermission.Role.Id != Role.Codes.Purchaser)
            {
                return;
            }

            Check.Require(OrderStatusCode.Codes.Approver == Role.Codes.Approver);
            Check.Require(OrderStatusCode.Codes.AccountManager == Role.Codes.AccountManager);
            Check.Require(OrderStatusCode.Codes.Purchaser == Role.Codes.Purchaser);

            var user = workgroupPermission.User;
            var workgroup = workgroupPermission.Workgroup;
            var approvals = _repositoryFactory.ApprovalRepository.Queryable.Where(a => !a.Completed && a.User == user && a.Order.Workgroup == workgroup && a.StatusCode.Id == workgroupPermission.Role.Id);
            foreach (var approval in approvals)
            {
                approval.User = null;
                _repositoryFactory.ApprovalRepository.EnsurePersistent(approval);
            }

        }

        public void RemoveFromCache(WorkgroupPermission workgroupPermissionToDelete)
        {
            //System.Web.HttpContext.Current.Cache.Remove(string.Format(Resources.Role_CacheId, workgroupPermissionToDelete.User.Id));
            System.Web.HttpContext.Current.Session.Remove(string.Format(Resources.Role_CacheId, workgroupPermissionToDelete.User.Id));
        }


        public List<int> GetChildWorkgroups(int workgroupId)
        {
            return _queryRepositoryFactory.RelatatedWorkgroupsRepository.Queryable.Where(
                    a => a.AdminWorkgroupId == workgroupId).Select(b => b.WorkgroupId).
                    Distinct().ToList();
            
        }


        //public List<int> GetChildWorkgroups2(int workgroupId)
        //{
        //    var workgroupOrgIds = _workgroupRepository.Queryable.Single(a => a.Id == workgroupId).Organizations.Select(b => b.Id).ToList();
        //    var childOrgIds = _queryRepositoryFactory.OrganizationDescendantRepository.Queryable.Where(a => workgroupOrgIds.Contains(a.RollupParentId)).Select(b => b.OrgId).Distinct().ToList();
        //    var childOrgs = _repositoryFactory.OrganizationRepository.Queryable.Where(a => childOrgIds.Contains(a.Id));

        //    List<int> rtValue = new List<int>();

        //    foreach (var organization in childOrgs)
        //    {
        //        var tempIds = _workgroupRepository.Queryable.Where(a => !a.Administrative && a.IsActive && a.Organizations.Contains(organization)).Select(b => b.Id);
        //        rtValue.AddRange(tempIds);
        //    }

        //    rtValue = rtValue.Distinct().ToList();

        //    #region Testing Query
        //    var viewsIds = GetChildWorkgroups2(workgroupId);
        //    var viewHasExtraIds = viewsIds.Except(rtValue);
        //    var ViewHasMissingIds = rtValue.Except(viewsIds);
        //    Check.Require(!viewHasExtraIds.Any(), "View Returned More Ids");
        //    Check.Require(!ViewHasMissingIds.Any(), "View Returned Fewer Ids");
        //    #endregion

        //    return rtValue;
        //}


        //public List<int> GetParentWorkgroups2(int workgroupId)
        //{
        //    var workgroupOrgIds = _workgroupRepository.Queryable.Single(a => a.Id == workgroupId).Organizations.Select(b => b.Id).ToList();
        //    var parentOrgIds =
        //        _queryRepositoryFactory.OrganizationDescendantRepository.Queryable.Where(
        //            a => workgroupOrgIds.Contains(a.OrgId)).Select(b => b.RollupParentId).Distinct().ToList();
        //    var parentOrgs = _repositoryFactory.OrganizationRepository.Queryable.Where(a => parentOrgIds.Contains(a.Id));

        //    List<int> rtValue = new List<int>();

        //    foreach (var organization in parentOrgs)
        //    {
        //        var tempIds =
        //            _workgroupRepository.Queryable.Where(a => a.Administrative && a.IsActive && a.Organizations.Contains(organization))
        //                .Select(b => b.Id);
        //        rtValue.AddRange(tempIds);
        //    }

        //    rtValue = rtValue.Distinct().ToList();

        //    #region Testing Query
        //    var viewsIds = GetParentWorkgroups2(workgroupId);
        //    var viewHasExtraIds = viewsIds.Except(rtValue);
        //    var ViewHasMissingIds = rtValue.Except(viewsIds);
        //    Check.Require(!viewHasExtraIds.Any(), "View Returned More Ids");
        //    Check.Require(!ViewHasMissingIds.Any(), "View Returned Fewer Ids");
        //    #endregion

        //    return rtValue;

        //}

        /// <summary>
        /// Get a list of admin workgroup ids that are active
        /// </summary>
        /// <param name="workgroupId"></param>
        /// <returns></returns>
        public List<int> GetParentWorkgroups(int workgroupId)
        {
            return _queryRepositoryFactory.RelatatedWorkgroupsRepository.Queryable.Where(
                    a => a.WorkgroupId == workgroupId).Select(b => b.AdminWorkgroupId).
                    Distinct().ToList();

        }

        public IEnumerable<Workgroup> LoadAdminWorkgroups(bool showActive = false)
        {
            // load the person's orgs
            var person = _repositoryFactory.UserRepository.Queryable.Where(x => x.Id == _userIdentity.CurrentPrincipal.Identity.Name).Fetch(x => x.Organizations).Single();
            var porgs = person.Organizations.Select(x => x.Id).ToList();

            // get the administrative rollup on orgs
            var wgIds = _queryRepositoryFactory.AdminWorkgroupRepository.Queryable.Where(a => porgs.Contains(a.RollupParentId)).Select(a => a.WorkgroupId).ToList();

            // get the workgroups
            var workgroups = _workgroupRepository.Queryable.Where(a => wgIds.Contains(a.Id));

            if (!showActive)
            {
                workgroups = workgroups.Where(a => a.IsActive);
            }

            return workgroups.ToList();
        }


    }
}