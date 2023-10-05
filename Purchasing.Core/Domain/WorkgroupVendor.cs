using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using Purchasing.Core.Helpers;
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
        
        [Display(Name = "Kfs Vendor")]
        [StringLength(10)]
        public virtual string VendorId { get; set; }
        [Display(Name = "Vendor Address")]
        [StringLength(4)]
        public virtual string VendorAddressTypeCode { get; set; }

        [Display(Name = "Campus Vendor")]
        [StringLength(30)] //Docs say 30, so we either need to increase vendorId size, or just use it here... 
        public virtual string AeSupplierNumber { get;set;}

        [StringLength(15)] //Length confirmed in Slack.
        public virtual string AeSupplierSiteCode { get; set; }

        [Required]
        [StringLength(45)]
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
        [Display(Name = "Country Code")]
        public virtual string CountryCode { get; set; }

        public virtual bool IsActive { get; set; }

        [StringLength(15)]
        public virtual string Phone { get; set; }

        [StringLength(15)]
        public virtual string Fax { get; set; }

        [StringLength(50)]
        [EmailAddress]
        public virtual string Email { get; set; }

        [StringLength(128)]
        public virtual string Url { get; set; }

        public virtual string ShortDisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AeSupplierSiteCode))
                {
                    return $"{Name} ({Line1.Summarize()}, {City} {State})";
                }
                else
                {
                    return $"{Name} ({AeSupplierSiteCode}) ({Line1.Summarize()}, {City} {State})";
                }

            }
            //get { return string.Format("{0} ({1}, {2} {3})", Name, Line1.Summarize(), City, State); }
        }

        public virtual string DisplayName { 
            get
            {
                return string.Format("{0} ({1}, {2}, {3} {4}, {5})", Name, Line1, City, State, Zip, CountryCode);
            }
        }
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
            Map(x => x.Phone);
            Map(x => x.Fax);
            Map(x => x.Email);
            Map(x => x.Url);
            Map(x => x.AeSupplierNumber);
            Map(x => x.AeSupplierSiteCode);

            References(x => x.Workgroup);
        }
    }
}