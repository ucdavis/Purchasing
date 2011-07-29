using System.Collections.Generic;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Vendor : DomainObjectWithTypedId<string>
    {
        public Vendor()
        {
            VendorAddresses = new List<VendorAddress>();
        }
        public virtual string Name { get; set; }
        public virtual string OwnershipCode { get; set; } //TODO: Decide if we want these codes mapped
        public virtual string BusinessTypeCode { get; set; }

        public virtual IList<VendorAddress> VendorAddresses { get; set; }
    }

    public class VendorMap : ClassMap<Vendor>
    {
        public VendorMap()
        {
            ReadOnly();

            Table("vVendors");
            
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);
            Map(x => x.OwnershipCode);
            Map(x => x.BusinessTypeCode);

            HasMany(x => x.VendorAddresses);
        }
    }
}
