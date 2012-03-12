using System;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Building : DomainObjectWithTypedId<string>
    {
        public virtual string CampusCode { get; set; }
        public virtual string BuildingCode { get; set; }
        public virtual string CampusName { get; set; }
        public virtual string CampusShortName { get; set; }
        public virtual char CampusTypeCode { get; set; }
        public virtual string BuildingName { get; set; }
        public virtual DateTime LastUpdateDate { get; set; }
        public virtual bool IsActive { get; set; }
    }

    public class BuildingMap : ClassMap<Building>
    {
        public BuildingMap()
        {
            Id(x => x.Id);

            Table("vBuildings");
            ReadOnly();

            Map(x => x.CampusCode);
            Map(x => x.BuildingCode);
            Map(x => x.CampusCode);
            Map(x => x.CampusShortName);
            Map(x => x.CampusTypeCode);
            Map(x => x.BuildingName);
            Map(x => x.LastUpdateDate);
            Map(x => x.IsActive);
        }
    }
}
