using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the WorkgroupVendor class
    /// </summary>
    public class WorkgroupVendorController : ApplicationController
    {
	    private readonly IRepository<WorkgroupVendor> _workgroupVendorRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<VendorAddress> _vendorAddressRepository;

        public WorkgroupVendorController(IRepository<WorkgroupVendor> workgroupVendorRepository, IRepository<Vendor> vendorRepository, IRepository<VendorAddress> vendorAddressRepository  )
        {
            _workgroupVendorRepository = workgroupVendorRepository;
            _vendorRepository = vendorRepository;
            _vendorAddressRepository = vendorAddressRepository;
        }

        public JsonNetResult SearchVendor(string searchTerm)
        {
            var results = _vendorRepository.Queryable.Where(a => a.Name.Contains(searchTerm)).ToList();

            return new JsonNetResult(results.Select(a => new { Id = a.Id, Name = a.Name }));
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

            var workgroup = Repository.OfType<Workgroup>().GetNullableById(id);
            var vendor = Repository.OfType<Vendor>().Queryable.Where(a => a.Id == vendorId).FirstOrDefault();
            var vendorAddress = Repository.OfType<VendorAddress>().Queryable.Where(a => a.Vendor == vendor && a.TypeCode == addressTypeCode).FirstOrDefault();

            var workgroupVendor = new WorkgroupVendor();

            workgroupVendor.Name = vendor.Name;
            workgroupVendor.Line1 = vendorAddress.Line1;
            workgroupVendor.Line2 = vendorAddress.Line2;
            workgroupVendor.Line3 = vendorAddress.Line3;
            workgroupVendor.City = vendorAddress.City;
            workgroupVendor.State = vendorAddress.State;
            workgroupVendor.Zip = vendorAddress.Zip;
            workgroupVendor.CountryCode = vendorAddress.CountryCode;

            return new JsonNetResult(new { id = 10, name = workgroupVendor.DisplayName });
        }

    }
}
