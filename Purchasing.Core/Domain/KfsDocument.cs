using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class KfsDocument : DomainObject
    {
        public string DocNumber { get; set; }

        public Order Order { get; set; }
    }

    public class KfsDocumentMap : ClassMap<KfsDocument>
    {
        public KfsDocumentMap()
        {
            Id(x => x.Id);

            Map(x => x.DocNumber);

            References(x => x.Order);
        }
    }
}
