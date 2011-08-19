using System.Collections.Generic;
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
        }

        public virtual string Name { get; set; }

        public virtual IList<WorkgroupAccount> Accounts { get; set; }
        public virtual IList<Organization> Organizations { get; set; }

        public virtual bool IsActive { get; set; }
    }

    public class WorkgroupMap : ClassMap<Workgroup>
    {
        public WorkgroupMap()
        {
            Id(x => x.Id);

            Map(x => x.Name);
            Map(x => x.IsActive);

            HasMany(x => x.Accounts);

            HasManyToMany(x => x.Organizations).Table("WorkgroupsXOrganizations").ParentKeyColumn("WorkgroupId").
                ChildKeyColumn("OrganizationId");
        }
    }
}