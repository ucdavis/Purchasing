using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class WorkgroupAddress : DomainObject
    {
        public WorkgroupAddress()
        {
            IsActive = true;
        }

        [Required]
        public virtual Workgroup Workgroup { get; set; }
        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }
        [StringLength(50)]
        public virtual string Building { get; set; }
        [StringLength(50)]
        public virtual string Room { get; set; }
        [Required]
        [StringLength(100)]
        public virtual string Address { get; set; }
        [Required]
        [StringLength(100)]
        public virtual string City { get; set; }
        [Required]
        [StringLength(2)]
        public virtual string State { get; set; }
        [Required]
        [StringLength(10)]
        [RegularExpression(@"^\d{5}$|^\d{5}-\d{4}$", ErrorMessage = "Zip must be ##### or #####-####")]
        public virtual string Zip { get; set; }
        [StringLength(15)]
        public virtual string Phone { get; set; }
        public virtual bool IsActive { get; set; }
    }

    public class WorkgroupAddressMap : ClassMap<WorkgroupAddress>
    {
        public WorkgroupAddressMap()
        {
            Id(x => x.Id);

            Map(x => x.Name);
            Map(x => x.Building);
            Map(x => x.Room);
            Map(x => x.Address);
            Map(x => x.City);
            Map(x => x.State).Column("StateId"); //TODO: make reference to state table?
            Map(x => x.Zip);
            Map(x => x.Phone);
            Map(x => x.IsActive);

            References(x => x.Workgroup);
        }
    }
}