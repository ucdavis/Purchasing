using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Approval : DomainObject
    {
        public virtual int Level { get; set; }
        public virtual bool Approved { get; set; }
        
        public virtual User User { get; set; }

        [Required]
        public virtual OrderStatusCode StatusCode { get; set; }
        [Required]
        public virtual Order Order { get; set; }
    }

    public class ApprovalMap : ClassMap<Approval>
    {
        public ApprovalMap()
        {
            Id(x => x.Id);

            Map(x => x.Level);
            Map(x => x.Approved);

            References(x => x.User);
            References(x => x.StatusCode);
            References(x => x.Order);
        }
    }
}
