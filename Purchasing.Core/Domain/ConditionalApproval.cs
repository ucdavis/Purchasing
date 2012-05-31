using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class ConditionalApproval : DomainObject
    {
        [DataType(DataType.MultilineText)]
        [Required]
        public virtual string Question { get; set; }
        [Required]
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

            Map(x => x.Question).Length(int.MaxValue);

            References(x => x.PrimaryApprover).Column("PrimaryApproverId");
            References(x => x.SecondaryApprover).Column("SecondaryApproverId").Nullable();
            References(x => x.Workgroup).Nullable();
            References(x => x.Organization).Nullable();
        }
    }
}