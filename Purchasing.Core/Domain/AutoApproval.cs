using System;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class AutoApproval : DomainObject
    {
        public virtual User TargetUser { get; set; }
        public virtual Account Account { get; set; }
        public virtual decimal MaxAmount { get; set; }
        public virtual bool LessThan { get; set; }
        public virtual bool Equal { get; set; }
        public virtual bool IsActive { get; set; }
        //TODO: what is the purpose of this user?  tracking the creator?
        public virtual User User { get; set; }
        public virtual DateTime? Expiration { get; set; }
    }

    public class AutoApprovalMap : ClassMap<AutoApproval>
    {
        public AutoApprovalMap()
        {
            Id(x => x.Id);

            Map(x => x.MaxAmount);
            Map(x => x.LessThan);
            Map(x => x.Equal);
            Map(x => x.IsActive);
            Map(x => x.Expiration).Nullable();

            References(x => x.TargetUser).Column("TargetUserId").Nullable();
            References(x => x.Account).Nullable();
            References(x => x.User);
        }
    }
}
