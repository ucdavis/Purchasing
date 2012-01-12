using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Web.Services
{
    public interface IWorkgroupService
    {
        void TransferValues(WorkgroupVendor source, WorkgroupVendor destination);
    }

    public class WorkgroupService : IWorkgroupService
    {
        private readonly IRepositoryWithTypedId<Vendor, string> _vendorRepository;
        private readonly IRepositoryWithTypedId<VendorAddress, Guid> _vendorAddressRepository;

        public WorkgroupService(IRepositoryWithTypedId<Vendor, string> vendorRepository, IRepositoryWithTypedId<VendorAddress, Guid> vendorAddressRepository  )
        {
            _vendorRepository = vendorRepository;
            _vendorAddressRepository = vendorAddressRepository;
        }


        public void TransferValues(WorkgroupVendor source, WorkgroupVendor destination)
        {
            Mapper.Map(source, destination);

            // existing vendor, set the values
            if (!string.IsNullOrWhiteSpace(source.VendorId) && !string.IsNullOrWhiteSpace(source.VendorAddressTypeCode))
            {
                var vendor = _vendorRepository.GetNullableById(source.VendorId);
                var vendorAddress = _vendorAddressRepository.Queryable.Where(a => a.Vendor == vendor && a.TypeCode == source.VendorAddressTypeCode).FirstOrDefault();

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
    }
}