using FluentNHibernate.Mapping;
using System;
using System.ComponentModel.DataAnnotations;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Favorite : DomainObject
    {
        public Favorite()
        {
            DateAdded = DateTime.UtcNow;
            IsActive = true;
        }

        //public virtual int Id { get; set; }
        [Required]
        public virtual User User { get; set; }
        [Required]
        public virtual Order Order { get; set; }
        public virtual DateTime DateAdded { get; set; }
        [StringLength(50)]
        public virtual string Category { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool IsActive { get; set; } = true;
    }

    public class FavoriteMap : ClassMap<Favorite>
    {
        public FavoriteMap()
        {
            Table("Favorites");
            Id(x => x.Id);
            References(x => x.User).Column("UserId"); //Should these just be simple Maps? I'll only ever get this when getting the order.
            References(x => x.Order).Column("OrderId");
            Map(x => x.DateAdded);
            Map(x => x.Category).Length(50);
            Map(x => x.Notes).Length(int.MaxValue);
            Map(x => x.IsActive);
        }
    }
}