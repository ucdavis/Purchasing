using System;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class VendorAddress : DomainObjectWithTypedId<Guid>
    {
        [Required]
        [StringLength(4)]
        public virtual string TypeCode { get; set; }
        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }
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
        [StringLength(2)]
        public virtual string CountryCode { get; set; }
        [StringLength(15)]
        public virtual string PhoneNumber { get; set; }
        [StringLength(15)]
        public virtual string FaxNumber { get; set; }
        [StringLength(50)]
        public virtual string Email { get; set; }
        [StringLength(128)]
        public virtual string Url { get; set; }
        [Required]
        public virtual Vendor Vendor { get; set; }

        public virtual bool IsDefault { get; set; }
        public virtual string DisplayName
        {
            get
            {
                //TODO: If the CountryCode is null, do you want the comma at the end? possible fix below
                //return string.Format("({0}) {1}, {2}, {3} {4}{5}", TypeCode, Line1, City, State, Zip, CountryCode != null ? ", " + CountryCode:string.Empty);
                return string.Format("({0}) {1}, {2}, {3} {4}, {5}", TypeCode, Line1, City, State, Zip, CountryCode);
            }
        }

        public virtual string DisplayNameWithDefault
        {
            get
            {
                return string.Format("{0}{1}", IsDefault==true ? "DEFAULT " : string.Empty, DisplayName);
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
            Map(x => x.PhoneNumber);
            Map(x => x.Email);
            Map(x => x.Url);
            Map(x => x.FaxNumber);
            Map(x => x.IsDefault);
            References(x => x.Vendor);
        }
    }
}