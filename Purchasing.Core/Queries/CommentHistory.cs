using System;
using FluentNHibernate.Mapping;
using Purchasing.Core.Domain;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class CommentHistory : DomainObjectWithTypedId<Guid>
    {
        public virtual Order Order { get; set; }
        public virtual string Comment { get; set; }
        public virtual User CreatedBy { get; set; }
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
            References(x => x.Order);
            Map(x => x.Comment);
            Map(x => x.DateCreated);
            Map(x => x.AccessUserId).Column("Access");
            References(x => x.CreatedBy).Column("CreatedBy");
        }
    }
}