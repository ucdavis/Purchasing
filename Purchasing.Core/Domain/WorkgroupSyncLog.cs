using System;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class WorkgroupSyncLog : DomainObject
    {
        public WorkgroupSyncLog()
        {
            ActionDate = DateTime.Now;
        }

        [Required]
        public virtual Workgroup WorkGroup { get; set; }
        [Required]
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
        [Required]
        public virtual Actions Action { get; set; }
        [StringLength(250)]
        public virtual string Message { get; set; }
        public virtual DateTime ActionDate { get; set; }

        public class Actions
        {            
            public const string Add = "A";
            public const string Delete = "D";
            public const string Create = "C";
            public const string Updated = "U";
        }

    }

    public class WorkgroupSyncLogMap : ClassMap<WorkgroupSyncLog>
    {
        public WorkgroupSyncLogMap()
        {
            Id(x => x.Id);

            Map(x => x.Action);
            Map(x => x.Message);
            Map(x => x.ActionDate);

            References(x => x.WorkGroup);
            References(x => x.User);
            References(x => x.Role);
        }
    }

}
