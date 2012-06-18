using System;
using FluentNHibernate.Mapping;
using Purchasing.Core.Domain;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class CommentHistory : DomainObjectWithTypedId<Guid>
    {
        public virtual int OrderId { get; set; }
        public virtual string RequestNumber { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual string Comment { get; set; }
        public virtual string CreatedByUserId { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual string AccessUserId { get; set; }

    }

    public class CommentHistoryMap : ClassMap<CommentHistory>
    {
        public CommentHistoryMap()
        {
            ReadOnly();
            Table("vCommentHistory");
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.OrderId);
            Map(x => x.RequestNumber);
            Map(x => x.CreatedBy);
            Map(x => x.Comment);
            Map(x => x.CreatedByUserId);
            Map(x => x.DateCreated);
            Map(x => x.AccessUserId).Column("Access");
        }
    }
}