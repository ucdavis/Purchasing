using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class ConditionalApproval : DomainObject
    {
        public virtual string Question { get; set; }
        public virtual User PrimaryApprover { get; set; }
        public virtual User SecondaryApprover { get; set; }
        public virtual Workgroup Workgroup { get; set; }
        public virtual Organization Organization { get; set; }
    }

    public class ConditionalApprovalMap : ClassMap<ConditionalApproval>
    {
        public ConditionalApprovalMap()
        {
            Table("ConditionalApproval"); //TODO: Change table to conditionalApprovals?

            Id(x => x.Id);

            Map(x => x.Question);

            References(x => x.PrimaryApprover);
            References(x => x.SecondaryApprover).Nullable();
            References(x => x.Workgroup).Nullable();
            References(x => x.Organization).Nullable();
        }
    }
}