using System;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class VendorAddress : DomainObjectWithTypedId<Guid>
    {
        public virtual string TypeCode { get; set; }
        public virtual string Name { get; set; }
        public virtual string Line1 { get; set; }
        public virtual string Line2 { get; set; }
        public virtual string Line3 { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
        public virtual string CountryCode { get; set; }

        public virtual Vendor Vendor { get; set; }

        public virtual string DisplayName
        {
            get
            {
                return string.Format("({0}) {1}, {2}, {3} {4}, {5}", TypeCode, Line1, City, State, Zip, CountryCode);
            }
        }
    }

    public class VendorAddressMap : ClassMap<VendorAddress>
    {
        public VendorAddressMap()
        {
            ReadOnly();

            Table("vVendorAddresses");

            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.TypeCode);
            Map(x => x.Name);
            Map(x => x.Line1);
            Map(x => x.Line2);
            Map(x => x.Line3);
            Map(x => x.City);
            Map(x => x.State);
            Map(x => x.Zip);
            Map(x => x.CountryCode);

            References(x => x.Vendor);
        }
    }
}