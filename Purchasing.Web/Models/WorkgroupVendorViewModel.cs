using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    /// <summary>
    /// ViewModel for the WorkgroupVendor class
    /// </summary>
    public class WorkgroupVendorViewModel
    {
        public WorkgroupVendor WorkgroupVendor { get; set; }
        public IEnumerable<Vendor> Vendors { get; set; }

        public Vendor Vendor { get; set; }
        public VendorAddress VendorAddress { get; set; }
        public MultiSelectList VendorAddresses { get; set; }

        public bool? NewVendor { get; set; }

        public static WorkgroupVendorViewModel Create(IRepository repository, WorkgroupVendor workgroupVendor = null, Vendor vendor = null, VendorAddress vendorAddress = null, bool? newVendor = null)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var addresses = vendor != null ? new MultiSelectList(vendor.VendorAddresses.Select(a => new { TypeCode = a.TypeCode, Name = string.Format("{0} ({1}, {2}, {3} {4})", a.Name, a.Line1, a.City, a.State, a.Zip) }).ToList(), "TypeCode", "Name") : new MultiSelectList(new List<VendorAddress>(), "TypeCode", "Name");

            var viewModel = new WorkgroupVendorViewModel
                                {
                                    WorkgroupVendor = workgroupVendor ?? new WorkgroupVendor(),
                                    Vendor = vendor,
                                    VendorAddress = vendorAddress,
                                    Vendors = repository.OfType<Vendor>().Queryable.OrderBy(a => a.Name).ToList(),
                                    VendorAddresses = addresses,
                                    NewVendor = newVendor
                                };
 
            return viewModel;
        }
    }
}