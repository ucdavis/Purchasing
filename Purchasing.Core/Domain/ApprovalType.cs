using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class ApprovalType : DomainObjectWithTypedId<string>
    {
        public virtual string Name { get; set; }
    }

    public class ApprovalTypeMap : ClassMap<ApprovalType>
    {
        public ApprovalTypeMap()
        {
            Map(x => x.Name);
        }
    }
}