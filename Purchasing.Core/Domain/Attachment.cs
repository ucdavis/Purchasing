using System;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Attachment : DomainObjectWithTypedId<Guid>
    {
        public virtual string FileName { get; set; }
        public virtual string ContentType { get; set; }
        public virtual byte[] Contents { get; set; }
        public virtual DateTime DateCreated { get; set; }

        public virtual User User { get; set; }
        public virtual Order Order { get; set; }
    }

    public class AttachmentMap : ClassMap<Attachment>
    {
        public AttachmentMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.FileName);
            Map(x => x.ContentType);
            Map(x => x.Contents).CustomType("BinaryBlob");
            Map(x => x.DateCreated);

            References(x => x.User);
            References(x => x.Order);
        }
    }
}