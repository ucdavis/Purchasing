using System;
using FluentNHibernate.Mapping;
using Purchasing.Core.Domain;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class CommentHistory : DomainObjectWithTypedId<Guid>
    {
        public int OrderId { get; set; }
        public string RequestNumber { get; set; }
        public string CreatedBy { get; set; }
        public string Comment { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public string AccessUserId { get; set; }

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