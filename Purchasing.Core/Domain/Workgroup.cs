using System.Collections.Generic;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Workgroup : DomainObject
    {
        public virtual string Name { get; set; }
        public virtual Department Department { get; set; }

        public virtual IList<Account> Accounts { get; set; }

        public virtual bool IsActive { get; set; }
    }

    public class WorkgroupMap : ClassMap<Workgroup>
    {
        public WorkgroupMap()
        {
            Id(x => x.Id);

            Map(x => x.Name);
            Map(x => x.IsActive);

            References(x => x.Department);

            HasManyToMany(x => x.Accounts).Table("WorkgroupAccounts");
        }
    }
}