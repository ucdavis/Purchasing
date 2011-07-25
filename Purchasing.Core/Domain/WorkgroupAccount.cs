using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class WorkgroupAccount : DomainObject
    {
        public virtual Workgroup Workgroup { get; set; }
        public virtual Account Account { get; set; }
    }

    public class WorkgroupAccountMap : ClassMap<WorkgroupAccount>
    {
        public WorkgroupAccountMap()
        {
            Id(x => x.Id);

            References(x => x.Workgroup);
            References(x => x.Account);
        }
    }
}