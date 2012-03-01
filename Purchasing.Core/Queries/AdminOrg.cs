using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class AdminOrg : DomainObject
    {
        public virtual string AccessUserId { get; set; }
        public virtual string OrgId { get; set; }
        public virtual string Name { get; set; }
        public virtual string ImmediateParentId { get; set; }
        public virtual string RollupParentId { get; set; }
        public virtual bool IsActive { get; set; }
    }

    public class AdminOrgMap : ClassMap<AdminOrg>
    {
        public AdminOrgMap()
        {
            Id(x => x.Id);

            Table("vAdminOrgs");
            ReadOnly();

            Map(x => x.AccessUserId);
            Map(x => x.OrgId);
            Map(x => x.Name);
            Map(x => x.ImmediateParentId);
            Map(x => x.RollupParentId);
            Map(x => x.IsActive);
        }
    }
}
