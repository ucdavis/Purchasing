using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class AccessBase : DomainObject
    {
        public virtual int OrderId { get; set; }
        public virtual string AccessUserId { get; set; }
        public virtual string AccessLevel { get; set; }
        public virtual bool IsAdmin { get; set; }
    }

    public class EditAccess : AccessBase
    {
    }

    public class ReadAccess : AccessBase
    {
    }

    public class Access : AccessBase
    {
        public virtual bool ReadAccess { get; set; }
        public virtual bool EditAccess { get; set; }
        
    }

    public class OpenAccess : AccessBase
    {
        public virtual bool ReadAccess { get; set; }
        public virtual bool EditAccess { get; set; }
    }

    /// <summary>
    /// Not mapped -- will be retrieved via direct SQL query
    /// </summary>
    public class ClosedAccess : AccessBase
    {
    }

    public class OpenAccessMap : ClassMap<OpenAccess>
    {
        public OpenAccessMap()
        {
            Id(x => x.Id);

            Table("vOpenAccess");
            ReadOnly();

            Map(x => x.OrderId);
            Map(x => x.AccessUserId);
            Map(x => x.AccessLevel);
            Map(x => x.IsAdmin);

            Map(x => x.EditAccess).Column("Edit");
            Map(x => x.ReadAccess).Column("`Read`");
        }
    }
}
