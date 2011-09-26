using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purchasing.Core.Domain;
using UCDArch.Testing;

namespace Purchasing.Tests.Core
{
    public static class CreateValidEntities
    {
        #region Helper Extension

        private static string Extra(this int? counter)
        {
            var extraString = "";
            if (counter != null)
            {
                extraString = counter.ToString();
            }
            return extraString;
        }

        #endregion Helper Extension

        // Once database design settles, uncomment and complete.

        public static Account Account(int? counter, bool loadAll = true)
        {
            var rtValue = new Account();
            rtValue.SetIdTo(counter.Extra());
            if (loadAll)
            {
                rtValue.Name = "Name" + counter.Extra();
                rtValue.AccountManager = "AccountManager" + counter.Extra();
                rtValue.PrincipalInvestigator = "PrincipalInvestigator" + counter.Extra();
                rtValue.IsActive = true;
            }
            return rtValue;
        }

        public static Audit Audit(int? counter, bool loadAll = true)
        {
            var rtValue = new Audit();
            rtValue.ObjectName = "ObjectName" + counter.Extra();
            rtValue.SetActionCode(AuditActionType.Create);
            rtValue.Username = "Username" + counter.Extra();
            rtValue.AuditDate = DateTime.Now.Date;
            if(loadAll)
            {
                rtValue.ObjectId = "ObjectId" + counter.Extra();
            }

            return rtValue;
        }

        public static Commodity Commodity(int? counter, bool loadAll = true)
        {
            var rtValue = new Commodity();
            rtValue.SetIdTo(counter.Extra());
            if (loadAll)
            {
                rtValue.Name = "Name" + counter.Extra();
                rtValue.GroupCode = "GroupCode" + counter.Extra();
                rtValue.SubGroupCode = "SubGroupCode" + counter.Extra();
            }

            return rtValue;
        }

        public static CommodityGroup CommodityGroup(int? counter, bool loadAll = true)
        {
            var rtValue = new CommodityGroup();
            rtValue.SetIdTo(counter.HasValue ? SpecificGuid.GetGuid(counter.Value) : Guid.NewGuid());
            if (loadAll)
            {
                rtValue.GroupCode = "GroupCode" + counter.Extra();
                rtValue.Name = "Name" + counter.Extra();
                rtValue.SubGroupCode = "SubGroupCode" + counter.Extra();
                rtValue.SubGroupName = "SubGroupName" + counter.Extra();
            }
            return rtValue;
        }

        public static EmailPreferences EmailPreferences(int? counter)
        {
            var rtValue = new EmailPreferences(counter.HasValue ? counter.Value.ToString() : "notSet");

            return rtValue;
        }

        //public static Approval Approval(int? counter, bool loadAll = true)
        //{
        //    var rtValue = new Approval();
        //    if (loadAll)
        //    {
        //        rtValue.Level = counter.HasValue ? counter.Value : 0;
        //        rtValue.Approved = true;
        //        rtValue.UserId = "UserId" + counter.Extra();
        //        rtValue.ApprovalType = new ApprovalType();
        //    }

        //    return rtValue;
        //}

        //public static ApprovalType ApprovalType(int? counter, bool loadAll=true)
        //{
        //    var rtValue = new ApprovalType();
        //    if(loadAll)
        //    {
        //        rtValue.Name = "Name" + counter.Extra();
        //    }

        //    return rtValue;
        //}

        public static AutoApproval AutoApproval(int i)
        {
            var rtResult = new AutoApproval();
            rtResult.IsActive = true;
            rtResult.LessThan = true;
            rtResult.MaxAmount = (decimal) 10.77;
            rtResult.TargetUser = CreateValidEntities.User(98);

            rtResult.User = CreateValidEntities.User(55);

            return rtResult;
        }

        private static User User(int i)
        {
            var rtResult = new User(i.ToString());
            rtResult.SetIdTo(i.ToString());
            rtResult.IsActive = true;
            rtResult.LastName = "LastName" + i;
            rtResult.FirstName = "FirstName" + i;

            return rtResult;
        }

        public static WorkgroupPermission WorkgroupPermission(int? counter)
        {
            var rtValue = new WorkgroupPermission();
            rtValue.Workgroup = new Workgroup();
            rtValue.Role = new Role();
            rtValue.User = new User();

            return rtValue;
        }

        public static WorkgroupAccount WorkgroupAccount(int? counter)
        {
            var rtValue = new WorkgroupAccount();
            rtValue.Account = new Account();
            rtValue.AccountManager = new User();
            rtValue.Approver = new User();
            rtValue.Purchaser = new User();
            rtValue.Workgroup = new Workgroup();

            return rtValue;
        }
    }
}
