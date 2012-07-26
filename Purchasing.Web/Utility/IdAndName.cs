using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Utility
{
    public class IdAndName
    {
        public IdAndName(string id, string name)
        {
            Id = id;
            Name = name;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayNameAndId
        {
            get { return string.Format("{0} ({1})", Name, Id); }
        }
    }

    /// <summary>
    /// Used to update workgroupPermissions that are related by Admin workgroups.
    /// </summary>
    public class WorkgroupPermissionActions
    {
        public WorkgroupPermissionActions(WorkgroupPermission workgroupPermission)
        {
            WorkgroupPermission = workgroupPermission;
            Action = Actions.Nothing;
        }

        public WorkgroupPermission WorkgroupPermission { get; set; }
        public string Action { get; set; }

        public class Actions
        {
            public const string Nothing = "N";
            public const string Delete = "D";
            public const string Add = "A";
        }

    }
}