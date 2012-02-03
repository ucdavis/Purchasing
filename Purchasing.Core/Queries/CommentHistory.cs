using System;
using FluentNHibernate.Mapping;
using Purchasing.Core.Domain;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class CommentHistory : DomainObject
    {
        public virtual int OrderId { get; set; }
        public virtual string Text { get; set; }
        public virtual User CreatedByUser { get; set; }
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
            Map(x => x.Text);
            Map(x => x.DateCreated);
            Map(x => x.AccessUserId).Column("access");
            References(x => x.CreatedByUser).Column("UserId");
        }
    }
}