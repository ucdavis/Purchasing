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
            rtValue.SetIdTo(counter.HasValue ? SpecificGuid.GetGuid(counter.Value) : new Guid());
            if (loadAll)
            {
                rtValue.GroupCode = "GroupCode" + counter.Extra();
                rtValue.Name = "Name" + counter.Extra();
                rtValue.SubGroupCode = "SubGroupCode" + counter.Extra();
                rtValue.SubGroupName = "SubGroupName" + counter.Extra();
            }
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
    }
}
