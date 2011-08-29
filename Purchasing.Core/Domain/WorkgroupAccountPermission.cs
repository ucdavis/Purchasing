using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class WorkgroupAccountPermission : DomainObject
    {
        public virtual WorkgroupAccount WorkgroupAccount { get; set; }
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }

    public class WorkgroupAccountPermissionMap : ClassMap<WorkgroupAccountPermission>
    {
        public WorkgroupAccountPermissionMap()
        {
            Id(x => x.Id);

            References(x => x.WorkgroupAccount).Cascade.All();
            References(x => x.User);
            References(x => x.Role);
        }
    }
}