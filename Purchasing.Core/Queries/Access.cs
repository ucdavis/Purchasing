using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class AccessBase : DomainObject
    {
        public virtual int OrderId { get; set; }
        public virtual string AccessUserId { get; set; }
        public virtual string AccessLevel { get; set; }
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
        public virtual bool IsAdmin { get; set; }
    }

    public class EditAccessMap : ClassMap<EditAccess>
    {
        public EditAccessMap()
        {
            Id(x => x.Id);

            Table("vEditAccess");
            ReadOnly();

            Map(x => x.OrderId);
            Map(x => x.AccessUserId);
            Map(x => x.AccessLevel);
        }
    }

    public class ReadAccessMap : ClassMap<ReadAccess>
    {
        public ReadAccessMap()
        {
            Id(x => x.Id);

            Table("vReadAccess");
            ReadOnly();

            Map(x => x.OrderId);
            Map(x => x.AccessUserId);
            Map(x => x.AccessLevel);
        }
    }

    public class AccessMap : ClassMap<Access>
    {
        public AccessMap()
        {
            Id(x => x.Id);

            Table("vAccess");
            ReadOnly();

            Map(x => x.OrderId);
            Map(x => x.AccessUserId);
            Map(x => x.ReadAccess);
            Map(x => x.EditAccess);
            Map(x => x.IsAdmin);
            Map(x => x.AccessLevel);
        }
    }
}
