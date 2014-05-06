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
}
