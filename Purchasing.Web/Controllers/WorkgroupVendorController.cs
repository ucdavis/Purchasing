using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using Purchasing.Core.Repositories;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the WorkgroupVendor class
    /// </summary>
    public class WorkgroupVendorController : ApplicationController
    {
        private readonly IRepository<VendorAddress> _vendorAddressRepository;
        private readonly ISearchRepository _searchRepository;

        public WorkgroupVendorController(IRepository<VendorAddress> vendorAddressRepository, ISearchRepository searchRepository)
        {
            _vendorAddressRepository = vendorAddressRepository;
            _searchRepository = searchRepository;
        }

        public JsonNetResult SearchVendor(string searchTerm)
        {
            var results = _searchRepository.SearchVendors(searchTerm).ToList();

            return new JsonNetResult(results.Select(a => new {a.Id, a.Name}));
        }

        public JsonNetResult SearchVendorAddress(string vendorId)
        {
            var results = _vendorAddressRepository.Queryable.Where(a => a.Vendor.Id == vendorId).ToList();

            return new JsonNetResult(results.Select(a => new { Id = a.TypeCode, Name = a.DisplayName }));
        }

        [HttpPost]
        [BypassAntiForgeryToken]
        public JsonNetResult AddKfsVendor(int id, string vendorId, string addressTypeCode)
        {
            var vendor = Repository.OfType<Vendor>().Queryable.FirstOrDefault(a => a.Id == vendorId);
            var vendorAddress = Repository.OfType<VendorAddress>().Queryable.FirstOrDefault(a => a.Vendor == vendor && a.TypeCode == addressTypeCode);

            var workgroupVendor = new WorkgroupVendor
                                      {
                                          Name = vendor.Name,
                                          Line1 = vendorAddress.Line1,
                                          Line2 = vendorAddress.Line2,
                                          Line3 = vendorAddress.Line3,
                                          City = vendorAddress.City,
                                          State = vendorAddress.State,
                                          Zip = vendorAddress.Zip,
                                          CountryCode = vendorAddress.CountryCode
                                      };
            
            return new JsonNetResult(new { id = 10, name = workgroupVendor.DisplayName });
        }

    }
}