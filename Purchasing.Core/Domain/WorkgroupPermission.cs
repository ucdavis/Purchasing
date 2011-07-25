using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class WorkgroupPermission : DomainObject
    {
        public virtual Workgroup Workgroup { get; set; }
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }

    public class WorkgroupPermissionMap : ClassMap<WorkgroupPermission>
    {
        public WorkgroupPermissionMap()
        {
            Id(x => x.Id);

            References(x => x.Workgroup);
            References(x => x.User);
            References(x => x.Role);
        }
    }
}