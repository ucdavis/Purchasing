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

            _repositoryFactory.WorkgroupVendorRepository.EnsurePersistent(workgroupVendor);

            return new JsonNetResult(new {id = workgroupVendor.Id, name = workgroupVendor.Name});
        }

    }
}