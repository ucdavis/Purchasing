using System;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class BackupLog : DomainObject
    {
        public BackupLog()
        {
            DateTimeCreated = DateTime.Now;
            Completed = false;
            Deleted = false;
        }

        public virtual string RequestId { get; set; }
        public virtual DateTime DateTimeCreated { get; set; }
        public virtual bool Completed { get; set; }
        [Required]
        public virtual string Filename { get; set; }

        public virtual bool Deleted { get; set; }
        public virtual DateTime? DateTimeDeleted { get; set; }
    }

    public class BackupLogMap : ClassMap<BackupLog>
    {
        public BackupLogMap()
        {
            Id(x => x.Id);

            Map(x => x.RequestId);
            Map(x => x.DateTimeCreated);
            Map(x => x.Completed);
            Map(x => x.Filename);
            Map(x => x.Deleted);
            Map(x => x.DateTimeDeleted);
        }
    }
}
