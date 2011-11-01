using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Organization : DomainObjectWithTypedId<string>
    {
        public Organization()
        {
            Accounts = new List<Account>();
            ConditionalApprovals = new List<ConditionalApproval>();
        }

        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }
        [Required]
        [StringLength(1)]
        public virtual string TypeCode { get; set; }
        [Required]
        [StringLength(50)]
        public virtual string TypeName { get; set; }
        public virtual bool IsActive { get; set; }

        public virtual Organization Parent { get; set; }

        public virtual IList<Account> Accounts { get; set; }
        public virtual IList<ConditionalApproval> ConditionalApprovals { get; set; }
    }

    public class OrganizationMap : ClassMap<Organization>
    {
        public OrganizationMap()
        {
            ReadOnly();

            Table("vOrganizations");

            Id(x => x.Id).GeneratedBy.Assigned();  //ID is 4 characters

            Map(x => x.Name);
            Map(x => x.TypeCode);
            Map(x => x.TypeName);
            Map(x => x.IsActive);
            
            References(x => x.Parent).Column("ParentId");

            HasMany(a => a.Accounts);
            HasMany(a => a.ConditionalApprovals).Inverse().ExtraLazyLoad().Cascade.AllDeleteOrphan();
        }
    }
}