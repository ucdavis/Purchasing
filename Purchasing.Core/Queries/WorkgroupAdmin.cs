using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class WorkgroupAdmin : DomainObject
    {
        public virtual int WorkgroupId { get; set; }
        public virtual string WorkgroupName { get; set; }
        public virtual string PrimaryOrgId { get; set; }
        public virtual string UserId { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
    }

    public class WorkgroupAdminMap : ClassMap<WorkgroupAdmin>
    {
        public WorkgroupAdminMap()
        {
            Table("vWorkgroupAdmins");
            ReadOnly();

            Id(x => x.Id);

            Map(x => x.WorkgroupId);
            Map(x => x.WorkgroupName);
            Map(x => x.PrimaryOrgId);
            Map(x => x.UserId);
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Email);
        }
    }
}
