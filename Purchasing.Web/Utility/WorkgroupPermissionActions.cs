using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Utility
{
    /// <summary>
    /// Used to update workgroupPermissions that are related by Admin workgroups.
    /// </summary>
    public class WorkgroupPermissionActions
    {
        //TODO: Move this into own file.
        //Load all existing matches into this structure with action = delete, then go through all found ones and if exists, change action to nothing. if not exists create with action of add.
        public WorkgroupPermissionActions(WorkgroupPermission workgroupPermission, string action = Actions.Nothing)
        {
            WorkgroupPermission = workgroupPermission;
            Action = action;
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