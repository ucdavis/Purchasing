using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace Purchasing.Core.Domain
{
    public class WorkgroupAccount : DomainObject
    {
        [Required]
        public virtual Workgroup Workgroup { get; set; }
        [Required]
        public virtual Account Account { get; set; }
    }

    public class WorkgroupAccountMap : ClassMap<WorkgroupAccount>
    {
        public WorkgroupAccountMap()
        {
            Id(x => x.Id);

            References(x => x.Workgroup);
            References(x => x.Account);
        }
    }
}