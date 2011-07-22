using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class ConditionalApproval : DomainObject
    {
        public virtual string Question { get; set; }
        public virtual ConditionalApproval Parent { get; set; }
        public virtual User User { get; set; }
        public virtual Workgroup Workgroup { get; set; }
    }

    public class ConditionalApprovalMap : ClassMap<ConditionalApproval>
    {
        public ConditionalApprovalMap()
        {
            Table("ConditionalApproval"); //TODO: Change table to conditionalApprovals?

            Map(x => x.Question);

            References(x => x.Parent);
            References(x => x.User);
            References(x => x.Workgroup);
        }
    }
}