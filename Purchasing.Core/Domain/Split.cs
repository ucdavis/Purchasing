using System.Collections.Generic;
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

        public virtual Order Order { get; set; }
        public virtual LineItem LineItem { get; set; }
        public virtual Account Account { get; set; }

        public virtual IList<Approval> Approvals { get; set; }
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

            HasManyToMany(x => x.Approvals).Table("ApprovalsXSplits");
        }
    }
}