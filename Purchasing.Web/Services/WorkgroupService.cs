using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using AutoMapper;
using Purchasing.Core.Domain;
using Purchasing.Web.Models;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

namespace Purchasing.Web.Services
{
    public interface IWorkgroupService
    {
        void TransferValues(WorkgroupVendor source, ref WorkgroupVendor destination);
        int TryToAddPeople(int id, Role role, Workgroup workgroup, int successCount, string lookupUser, ref int failCount, List<KeyValuePair<string, string>> notAddedKvp);
        Workgroup CreateWorkgroup(Workgroup workgroup, string[] selectedOrganizations);
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

        public WorkgroupService(IRepositoryWithTypedId<Vendor, string> vendorRepository, 
            IRepositoryWithTypedId<VendorAddress, Guid> vendorAddressRepository, 
            IRepositoryWithTypedId<User, string> userRepository,
            IRepositoryWithTypedId<EmailPreferences, string> emailPreferencesRepository,
            IRepository<WorkgroupPermission> workgroupPermissionRepository,
            IRepository<Workgroup> workgroupRepository,
            IRepositoryWithTypedId<Organization, string> organizationRepository,
            IDirectorySearchService searchService)
        {
            _vendorRepository = vendorRepository;
            _vendorAddressRepository = vendorAddressRepository;
            _userRepository = userRepository;
            _emailPreferencesRepository = emailPreferencesRepository;
            _workgroupPermissionRepository = workgroupPermissionRepository;
            _workgroupRepository = workgroupRepository;
            _organizationRepository = organizationRepository;
            _searchService = searchService;
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
                    destination.State = vendorAddress.State;
                    destination.Zip = vendorAddress.Zip;
                    destination.CountryCode = vendorAddress.CountryCode;
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
        public int TryToAddPeople(int id, Role role, Workgroup workgroup, int successCount, string lookupUser,  ref int failCount, List<KeyValuePair<string, string>> notAddedKvp)
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
                successCount++;
            }
            else
            {
                //notAddedSb.AppendLine(string.Format("{0} : Is a duplicate", lookupUser));
                notAddedKvp.Add(new KeyValuePair<string, string>(lookupUser, "Is a duplicate"));
                failCount++;
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

            if(selectedOrganizations != null)
            {
                workgroupToCreate.Organizations =
                    _organizationRepository.Queryable.Where(a => selectedOrganizations.Contains(a.Id)).ToList();
            }

            if(!workgroupToCreate.Organizations.Contains(workgroupToCreate.PrimaryOrganization))
            {
                workgroupToCreate.Organizations.Add(workgroupToCreate.PrimaryOrganization);
            }

            _workgroupRepository.EnsurePersistent(workgroupToCreate);

            return workgroupToCreate;
        }
    }
}