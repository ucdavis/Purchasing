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
        [StringLength(250)]
        public virtual string NameAndId { get; set; }
        public virtual Role Role { get; set; }
        [Required]
        public virtual string Action { get; set; }
        [StringLength(250)]
        public virtual string Message { get; set; }
        public virtual DateTime ActionDate { get; set; }
        public virtual bool SyncKeyUpdate { get; set; }

        public class Actions
        {            
            public const string Add = "A";
            public const string Delete = "D";
            public const string New = "N";
            public const string Removed = "R";
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
            Map(x => x.SyncKeyUpdate);
            Map(x => x.NameAndId);

            References(x => x.WorkGroup);          
            References(x => x.Role);
        }
    }

}
