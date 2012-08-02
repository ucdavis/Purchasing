using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using AutoMapper;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Models;
using UCDArch.Core.PersistanceSupport;
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

        IEnumerable<Workgroup> LoadAdminWorkgroups(bool showActive = false);
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

            if(!_workgroupPermissionRepository.Queryable.Any(a => a.Role == role && a.User == user && a.Workgroup == workgroup))
            {
                var workgroupPermission = new WorkgroupPermission();
                workgroupPermission.Role = role;
                workgroupPermission.User = _userRepository.GetNullableById(lookupUser);
                workgroupPermission.Workgroup = _workgroupRepository.GetNullableById(id);

                _workgroupPermissionRepository.EnsurePersistent(workgroupPermission);
                
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

            return workgroupToCreate;
        }

        public void RemoveFromCache(WorkgroupPermission workgroupPermissionToDelete)
        {
            System.Web.HttpContext.Current.Cache.Remove(string.Format(Resources.Role_CacheId, workgroupPermissionToDelete.User.Id));
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