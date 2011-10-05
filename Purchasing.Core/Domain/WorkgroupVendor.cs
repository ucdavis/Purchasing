using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class WorkgroupVendor : DomainObject
    {
        public WorkgroupVendor()
        {
            IsActive = true;
        }

        [Required]
        public virtual Workgroup Workgroup { get; set; }
        
        [DisplayName("Kfs Vendor")]
        [StringLength(10)]
        public virtual string VendorId { get; set; }
        [DisplayName("Vendor Address")]
        [StringLength(4)]
        public virtual string VendorAddressTypeCode { get; set; }

        [Required]
        [StringLength(40)]
        public virtual string Name { get; set; }

        //Address info
        [Required]
        [StringLength(40)]
        public virtual string Line1 { get; set; }
        [StringLength(40)]
        public virtual string Line2 { get; set; }
        [StringLength(40)]
        public virtual string Line3 { get; set; }
        [Required]
        [StringLength(40)]
        public virtual string City { get; set; }
        [Required]
        [StringLength(2)]
        public virtual string State { get; set; }
        [Required]
        [StringLength(11)]
        public virtual string Zip { get; set; }
        [Required]
        [StringLength(2)]
        [DisplayName("Country Code")]
        public virtual string CountryCode { get; set; }

        public virtual bool IsActive { get; set; }
    }

    public class WorkgroupVendorMap : ClassMap<WorkgroupVendor>
    {
        public WorkgroupVendorMap()
        {
            Id(x => x.Id);

            Map(x => x.VendorId);
            Map(x => x.VendorAddressTypeCode);

            Map(x => x.Name);
            Map(x => x.Line1);
            Map(x => x.Line2);
            Map(x => x.Line3);
            Map(x => x.City);
            Map(x => x.State);
            Map(x => x.Zip);
            Map(x => x.CountryCode);
            Map(x => x.IsActive);

            References(x => x.Workgroup);
        }
    }
}