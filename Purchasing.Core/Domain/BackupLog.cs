using System;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class BackupLog : DomainObject
    {
        public virtual string RequestId { get; set; }
        public virtual DateTime DateTimeCreated { get; set; }
        public virtual bool Completed { get; set; }
    }

    public class BackupLogMap : ClassMap<BackupLog>
    {
        public BackupLogMap()
        {
            Id(x => x.Id);

            Map(x => x.RequestId);
            Map(x => x.DateTimeCreated);
            Map(x => x.Completed);
        }
    }
}
