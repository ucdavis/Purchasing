using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class WorkgroupRole : DomainObject
    {
        public virtual int WorkgroupId { get; set; }
        public virtual string AccessUserId { get; set; }
        public virtual string RoleId { get; set; }
        public virtual bool IsAdmin { get; set; }
    }

    public class WorkgroupRoleMap : ClassMap<WorkgroupRole>
    {
        public WorkgroupRoleMap()
        {
            Id(x => x.Id);

            Table("vWorkgroupRoles");
            ReadOnly();

            Map(x => x.WorkgroupId);
            Map(x => x.AccessUserId);
            Map(x => x.RoleId);
            Map(x => x.IsAdmin);
        }
    }
}
