using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Dapper;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using Purchasing.Web.Services;
using UCDArch.Data.NHibernate;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Helpers
{
    public class DbHelper
    {
        /// <summary>
        /// Resets the database to a consistant state for testing
        /// </summary>
        public static void ResetDatabase()
        {
            //First, delete all the of existing data
            var tables = new[]
                             {
                                 "ApprovalsXSplits", "Splits", "Approvals", "ApprovalTypes", "ConditionalApproval",
                                 "LineItems", "OrderTracking", "OrderTypes", "Orders", "ShippingTypes", "Workgroups",
                                 "Permissions", "Users", "Roles", "vDepartments", "vOrganizationTypes"
                             };

            var dbService = ServiceLocator.Current.GetInstance<IDbService>();

            using (var conn = dbService.GetConnection())
            {
                foreach (var table in tables)
                {
                    conn.Execute("delete from " + table);
                }
            }

            InsertDepartments(dbService);

            var session = NHibernateSessionManager.Instance.GetSession();
            session.BeginTransaction();

            InsertData(session);

            session.Transaction.Commit();
        }

        private static void InsertDepartments(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                //Add in organization types
            }
        }

        private static void InsertData(ISession session)
        {
            //Now insert new data
            var scott = new User("postit")
            {
                FirstName = "Scott",
                LastName = "Kirkland",
                Email = "srkirkland@ucdavis.edu"
            };

            var alan = new User("anlai") {FirstName = "Alan", LastName = "Lai", Email = "anlai@ucdavis.edu"};
            
            var admin = new Role("AD") { Name = "Admin" };
            var user = new Role("US") { Name = "User" };

            session.Save(scott);
            session.Save(alan);

            session.Save(admin);
            session.Save(user);

            Roles.AddUsersToRole(new[] {"postit", "anlai"}, "AD");
            Roles.AddUserToRole("anlai", "US");

            session.Flush(); //Flush out the changes
        }
    }
}