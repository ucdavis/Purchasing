using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Role : DomainObjectWithTypedId<string>
    {
        public Role()
        {
            Users = new List<User>();
            Level = 0;
            IsAdmin = false;
        }

        public Role(string id) : this()
        {
            Id = id;
        }
        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }

        public virtual int Level { get; set; }
        public virtual bool IsAdmin { get; set; }

        public virtual IList<User> Users { get; set; }

        public class Codes
        {
            public const string Admin = "AD";
            public const string DepartmentalAdmin = "DA";
            public const string Requester = "RQ";
            public const string Approver = "AP";
            public const string AccountManager = "AM";
            public const string Purchaser = "PR";
            public const string EmulationUser = "EU";
            public const string Reviewer = "RV";

            /// <summary>
            /// This is not a valid code for the DB, Used for context of menu options
            /// </summary>
            public const string AdminWorkgroup = "AW";

            /// <summary>
            /// This is not a valid code for the DB, Used for checking "roles" and Ad Hoc Account Manager doesn't give access to anything but approving and editing order
            /// </summary>
            public const string AdhocAccountManager = "AH";
        }
    }

    public class RoleMap : ClassMap<Role>
    {
        public RoleMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);
            Map(x => x.Level);
            Map(x => x.IsAdmin);

            HasManyToMany(x => x.Users).Table("Permissions").ParentKeyColumn("RoleID").ChildKeyColumn("UserID");
        }
    }
}