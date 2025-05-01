using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using Purchasing.Core.Helpers;
using UCDArch.Core.DomainModel;
using UCDArch.Core.Utils;

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
        public virtual string UserId { get; set; }
        public virtual int OrderId { get; set; }
        public virtual DateTime DateAdded { get; set; }
        [StringLength(50)]
        public virtual string Category { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool IsActive { get; set; } = true;
    }
}
