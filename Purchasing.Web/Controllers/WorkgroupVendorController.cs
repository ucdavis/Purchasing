using System.Linq;
using System.Web.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Web.App_GlobalResources;
using Purchasing.Web.Attributes;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using Purchasing.Core.Repositories;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the WorkgroupVendor class
    /// </summary>
    [AuthorizeApplicationAccess]
    public class WorkgroupVendorController : ApplicationController
    {
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<VendorAddress> _vendorAddressRepository;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly ISearchRepository _searchRepository;
        private readonly ISecurityService _securityService;

        public WorkgroupVendorController(IRepository<Vendor> vendorRepository, IRepository<VendorAddress> vendorAddressRepository, IRepositoryFactory repositoryFactory, ISearchRepository searchRepository, ISecurityService securityService)
        {
            _vendorRepository = vendorRepository;
            _vendorAddressRepository = vendorAddressRepository;
            _repositoryFactory = repositoryFactory;
            _searchRepository = searchRepository;
            _securityService = securityService;
        }

        public JsonNetResult SearchVendor(string searchTerm)
        {
            var results = _searchRepository.SearchVendors(searchTerm).ToList();

            return new JsonNetResult(results.Select(a => new {a.Id, Name = string.Format("{0} ({1})", a.Name, a.Id)}));
        }

        public JsonNetResult SearchVendorAddress(string vendorId)
        {
            var results = _vendorAddressRepository.Queryable.Where(a => a.Vendor.Id == vendorId).ToList();

            return new JsonNetResult(results.Select(a => new { Id = a.TypeCode, Name = a.DisplayName }));
        }

        [HttpPost]
        public JsonNetResult AddKfsVendor(int workgroupId, string vendorId, string addressTypeCode)
        {
            var workgroup = _repositoryFactory.WorkgroupRepository.GetById(workgroupId);
            var vendor = _vendorRepository.Queryable.Single(a => a.Id == vendorId);
            var vendorAddress = _vendorAddressRepository.Queryable.First(a => a.Vendor == vendor && a.TypeCode == addressTypeCode);
            var added = true;
            var duplicate = false;
            var wasInactive = false;

            // just make sure user has access to the workgroup
            Check.Require(_securityService.HasWorkgroupAccess(workgroup), Resources.NoAccess_Workgroup);

            var workgroupVendor = new WorkgroupVendor
                                      {
                                          Workgroup = workgroup,
                                          VendorId = vendor.Id,
                                          VendorAddressTypeCode = vendorAddress.TypeCode,
                                          Name = vendor.Name,
                                          Line1 = vendorAddress.Line1,
                                          Line2 = vendorAddress.Line2,
                                          Line3 = vendorAddress.Line3,
                                          City = vendorAddress.City,
                                          State = vendorAddress.State,
                                          Zip = vendorAddress.Zip,
                                          CountryCode = vendorAddress.CountryCode
                                      };
            if(!_repositoryFactory.WorkgroupVendorRepository.Queryable
                .Any(a => a.Workgroup.Id == workgroupId && 
                    a.VendorId == workgroupVendor.VendorId && 
                    a.VendorAddressTypeCode == workgroupVendor.VendorAddressTypeCode))
            {
                //doesn't find any
                _repositoryFactory.WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);
                added = true;
                duplicate = false;
                wasInactive = false;
            }
            else
            {
                //found one
                var inactiveVendor = _repositoryFactory.WorkgroupVendorRepository.Queryable
                    .FirstOrDefault(a => a.Workgroup.Id == workgroupId &&
                                         a.VendorId == workgroupVendor.VendorId &&
                                         a.VendorAddressTypeCode == workgroupVendor.VendorAddressTypeCode &&
                                         !a.IsActive);
                if(inactiveVendor != null)
                {
                    // we found one this is disabled, activate it and return it
                    inactiveVendor.IsActive = true;
                    _repositoryFactory.WorkgroupVendorRepository.EnsurePersistent(inactiveVendor);
                    added = false;
                    duplicate = false;
                    wasInactive = true;
                    return new JsonNetResult(new { id = inactiveVendor.Id, name = inactiveVendor.Name, added, duplicate, wasInactive });
                }
                // there was an active duplicate, return the first one.
                workgroupVendor = _repositoryFactory.WorkgroupVendorRepository.Queryable
                    .First(a => a.Workgroup.Id == workgroupId &&
                                         a.VendorId == workgroupVendor.VendorId &&
                                         a.VendorAddressTypeCode == workgroupVendor.VendorAddressTypeCode &&
                                         a.IsActive);
                added = false;
                duplicate = true;
                wasInactive = false;
            }

            return new JsonNetResult(new {id = workgroupVendor.Id, name = workgroupVendor.Name, added, duplicate, wasInactive});
        }

    }
}