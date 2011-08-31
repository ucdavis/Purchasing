using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class WorkgroupVendor : DomainObject
    {
        [Required]
        public virtual Workgroup Workgroup { get; set; }
        
        //TODO: Link to vendor lookup table or keep separate?
        public virtual string VendorId { get; set; }
        public virtual string VendorAddressId { get; set; }

        public virtual string Name { get; set; }

        //Address info
        public virtual string Line1 { get; set; }
        public virtual string Line2 { get; set; }
        public virtual string Line3 { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
        public virtual string CountryCode { get; set; }
    }

    public class WorkgroupVendorMap : ClassMap<WorkgroupVendor>
    {
        public WorkgroupVendorMap()
        {
            Id(x => x.Id);

            Map(x => x.VendorId);
            Map(x => x.VendorAddressId);

            Map(x => x.Name);
            Map(x => x.Line1);
            Map(x => x.Line2);
            Map(x => x.Line3);
            Map(x => x.City);
            Map(x => x.State);
            Map(x => x.Zip);
            Map(x => x.CountryCode);

            References(x => x.Workgroup);
        }
    }
}