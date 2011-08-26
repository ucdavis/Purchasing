using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Workgroup : DomainObject
    {
        public Workgroup()
        {
            Accounts = new List<WorkgroupAccount>();
            Organizations = new List<Organization>();
            IsActive = true;
        }

        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }

        public virtual IList<WorkgroupAccount> Accounts { get; set; }
        public virtual IList<Organization> Organizations { get; set; }
        public virtual IList<WorkgroupVendor> Vendors { get; set; }

        [Required]
        public virtual Organization PrimaryOrganization { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual void AddAccount(WorkgroupAccount workgroupAccount)
        {
            workgroupAccount.Workgroup = this;
            Accounts.Add(workgroupAccount);
        }
    }

    public class WorkgroupMap : ClassMap<Workgroup>
    {
        public WorkgroupMap()
        {
            Id(x => x.Id);

            Map(x => x.Name);
            Map(x => x.IsActive);

            References(x => x.PrimaryOrganization).Column("PrimaryOrganizationId").Not.Nullable();

            HasMany(x => x.Vendors).ExtraLazyLoad().Cascade.SaveUpdate().Inverse();
            HasMany(x => x.Accounts).ExtraLazyLoad().Cascade.SaveUpdate().Inverse();

            HasManyToMany(x => x.Organizations).Table("WorkgroupsXOrganizations").ParentKeyColumn("WorkgroupId").
                ChildKeyColumn("OrganizationId").ExtraLazyLoad();
        }
    }
}