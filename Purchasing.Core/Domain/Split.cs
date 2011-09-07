using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Split : DomainObject
    {
        public Split()
        {
            Approvals = new List<Approval>();
        }

        public virtual decimal Amount { get; set; }

        [Required]
        public virtual Order Order { get; set; }
        public virtual LineItem LineItem { get; set; }
        [Required]
        public virtual Account Account { get; set; }

        public virtual IList<Approval> Approvals { get; set; }

        public virtual void AddApproval(Approval approval)
        {
            approval.Order = Order;
            Approvals.Add(approval);
        }
    }

    public class SplitMap : ClassMap<Split>
    {
        public SplitMap()
        {
            Id(x => x.Id);

            Map(x => x.Amount);

            References(x => x.Order);
            References(x => x.LineItem);
            References(x => x.Account);

            HasManyToMany(x => x.Approvals).Table("ApprovalsXSplits").ParentKeyColumn("SplitID").ChildKeyColumn("ApprovalID").Cascade.AllDeleteOrphan();
        }
    }
}