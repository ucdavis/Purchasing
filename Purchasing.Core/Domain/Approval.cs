using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Approval : DomainObject
    {
        public virtual int Level { get; set; }
        public virtual bool Approved { get; set; }
        
        public virtual string UserId { get; set; }

        public virtual ApprovalType ApprovalType { get; set; }
    }

    public class ApprovalMap : ClassMap<Approval>
    {
        public ApprovalMap()
        {
            Map(x => x.Level);
            Map(x => x.Approved);

            References(x => x.ApprovalType);
        }
    }
}
