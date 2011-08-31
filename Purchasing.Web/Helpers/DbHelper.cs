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
                                 "OrderStatusCodes", "ApprovalsXSplits", "Splits", "Approvals", "ApprovalTypes", "ConditionalApproval",
                                 "LineItems", "OrderTracking", "OrderTypes", "Orders", "ShippingTypes", "WorkgroupPermissions", "WorkgroupAccountPermissions", "WorkgroupAccounts", "WorkgroupsXOrganizations", "WorkgroupVendors", "WorkgroupAddresses", "Workgroups",
                                 "Permissions", "UsersXOrganizations", "EmailPreferences", "Users", "Roles", "vAccounts", "vOrganizations", "vVendorAddresses", "vVendors", "vCommodities", "vCommodityGroups"
                             };

            var dbService = ServiceLocator.Current.GetInstance<IDbService>();

            using (var conn = dbService.GetConnection())
            {
                foreach (var table in tables)
                {
                    conn.Execute("delete from " + table);
                }
            }

            // reset the seed values
            ReseedTables(dbService);

            InsertOrderStatusCodes(dbService);
            InsertOrganizations(dbService);
            InsertAccounts(dbService);
            InsertVendors(dbService);
            InsertCommodityCodes(dbService);

            var session = NHibernateSessionManager.Instance.GetSession();
            session.BeginTransaction();

            InsertData(session);

            session.Transaction.Commit();
        }

        private static void ReseedTables(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                conn.Execute("DBCC CHECKIDENT(workgroups, RESEED, 0)");
                conn.Execute("DBCC CHECKIDENT(orders, RESEED, 0)");
            }
        }

        private static void InsertData(ISession session)
        {
            //Now insert new data
            var scott = new User("postit") { FirstName = "Scott", LastName = "Kirkland", Email = "srkirkland@ucdavis.edu", IsActive = true };
            var alan = new User("anlai") { FirstName = "Alan", LastName = "Lai", Email = "anlai@ucdavis.edu", IsActive = true };
            var ken = new User("taylorkj") { FirstName = "Ken", LastName = "Taylor", Email = "taylorkj@ucdavis.edu", IsActive = true };
            var chris = new User("cthielen") { FirstName = "Christopher", LastName = "Thielen", Email = "cmthielen@ucdavis.edu", IsActive = true };
            var jscub = new User("jscub")
                            {
                                FirstName = "James",
                                LastName = "Cubbage",
                                Email = "jscubbage@ucdavis.edu",
                                IsActive = true
                            };
            var jsylvest = new User("jsylvest")
            {
                FirstName = "Jason",
                LastName = "Sylvestre",
                Email = "jsylvest@ucdavis.edu",
                IsActive = true
            };

            var admin = new Role("AD") { Name = "Admin" };
            var deptAdmin = new Role("DA") { Name = "DepartmentalAdmin" };
            var user = new Role("RQ") { Name = "Requester" };
            var approver = new Role("AR") { Name = "Approver" };
            var acctMgr = new Role("AM") { Name = "AccountManager" };
            var purchaser = new Role("PR") { Name = "Purchaser" };

            scott.Organizations.Add(session.Load<Organization>("AANS"));
            scott.Organizations.Add(session.Load<Organization>("AAES"));
            ken.Organizations.Add(session.Load<Organization>("AANS"));
            ken.Organizations.Add(session.Load<Organization>("ABAE"));
            chris.Organizations.Add(session.Load<Organization>("AANS"));
            chris.Organizations.Add(session.Load<Organization>("AAES"));
            chris.Organizations.Add(session.Load<Organization>("AFST"));
            jscub.Organizations.Add(session.Load<Organization>("AFST"));
            jscub.Organizations.Add(session.Load<Organization>("AAES"));
            jsylvest.Organizations.Add(session.Load<Organization>("AFST"));
            jsylvest.Organizations.Add(session.Load<Organization>("AAES"));

            var testWorkgroup = new Workgroup() { Name = "Test Workgroup", IsActive = true, };
            var workGroupAccount = new WorkgroupAccount() { };
            workGroupAccount.Account = session.Load<Account>("3-6851000");
            testWorkgroup.AddAccount(workGroupAccount);
            testWorkgroup.PrimaryOrganization = session.Load<Organization>("AAES");
            testWorkgroup.Organizations.Add(session.Load<Organization>("AAES"));
            testWorkgroup.Organizations.Add(session.Load<Organization>("APLS"));

            var workgroupPerm = new WorkgroupPermission() { User = scott, Role = deptAdmin, Workgroup = testWorkgroup };
            var workgroupPerm2 = new WorkgroupPermission() { User = jsylvest, Role = deptAdmin, Workgroup = testWorkgroup };
            var workgroupPerm3 = new WorkgroupPermission() { User = jsylvest, Role = user, Workgroup = testWorkgroup };
            var workgroupPerm4 = new WorkgroupPermission() { User = jscub, Role = deptAdmin, Workgroup = testWorkgroup };

            session.Save(testWorkgroup);

            session.Save(scott);
            session.Save(alan);
            session.Save(ken);
            session.Save(jscub);
            session.Save(chris);
            session.Save(jsylvest);

            session.Save(admin);
            session.Save(deptAdmin);
            session.Save(user);
            session.Save(approver);
            session.Save(acctMgr);
            session.Save(purchaser);

            session.Save(workgroupPerm);
            session.Save(workgroupPerm2);
            session.Save(workgroupPerm3);
            session.Save(workgroupPerm4);

            Roles.AddUsersToRole(new[] { "postit", "anlai", "cthielen" }, "AD");
            Roles.AddUserToRole("anlai", "RQ");
            Roles.AddUserToRole("taylorkj", "DA");
            Roles.AddUserToRole("postit", "DA");
            Roles.AddUserToRole("jscub", "DA");
            Roles.AddUserToRole("jsylvest", "DA");

            session.Flush(); //Flush out the changes
        }

        private static void InsertOrganizations(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                conn.Execute(
                    @"insert into vOrganizations ([Id],[Name],[TypeCode],[TypeName],[ParentId],[IsActive]) VALUES (@id,@name,@typecode,@typename,@parent,@active)",
                    new[]
                        {
                            new {id = "AAES", name = "COLLEGE OF AG & ENVIRONMENTAL SCIENCES", typecode = "1", typename = "", parent = (string)null, active = true},
                            new {id = "AABL", name = "CA&ES ANIMAL BIOLOGY", typecode = "N   ", typename = "", parent = "AAES", active = true},
                            new {id = "AADM", name = "CA&ES ADMINISTRATION", typecode = "N   ", typename = "", parent = "AAES", active = true},
                            new {id = "AAFP", name = "AQUACULTURE & FISHERIES PROGRAM", typecode = "4   ", typename = "", parent = "AABL", active = true},
                            new {id = "AANS", name = "ANIMAL SCIENCE", typecode = "D   ", typename = "", parent = "AABL", active = true},
                            new {id = "AARC", name = "ANIMAL AGRICULTURE RESEARCH CENTER", typecode = "4   ", typename = "", parent = "AABL", active = true},
                            new {id = "AARE", name = "AGRICULTURAL & RESOURCE ECONOMICS", typecode = "4   ", typename = "", parent = "AHHD", active = true},
                            new {id = "AAVS", name = "AVIAN SCIENCES", typecode = "D   ", typename = "", parent = "AABL", active = true},
                            new {id = "ABAE", name = "BIOLOGICAL & AG ENGINEERING", typecode = "D   ", typename = "", parent = "AABL", active = true},
                            new {id = "ABML", name = "BODEGA MARINE LABORATORY", typecode = "4   ", typename = "", parent = "AABL", active = true},
                            new {id = "ACAB", name = "CENTER FOR AVIAN BIOLOGY", typecode = "4   ", typename = "", parent = "AABL", active = true},
                            new {id = "ACAW", name = "CENTER FOR ANIMAL WELFARE", typecode = "4   ", typename = "", parent = "AABL", active = true},
                            new {id = "ACBD", name = "AES CBS DEAN'S OFFICE", typecode = "D   ", typename = "", parent = "ACBS", active = true},
                            new {id = "ACCR", name = "CENTER FOR CONSUMER RESEARCH", typecode = "4   ", typename = "", parent = "AHHD", active = true},
                            new {id = "ACEH", name = "CENTER FOR ECOLOGICAL HEALTH RESEARCH", typecode = "4   ", typename = "", parent = "AERS", active = true},
                            new {id = "ACEP", name = "CEPRAP", typecode = "4   ", typename = "", parent = "APSC", active = true},
                            new {id = "ACL1", name = "FOOD CHAIN cluster", typecode = "D   ", typename = "", parent = "AADM", active = true},
                            new {id = "ACL2", name = "METRO cluster", typecode = "D   ", typename = "", parent = "AADM", active = true},
                            new {id = "ACL3", name = "BFTV CLUSTER", typecode = "D   ", typename = "", parent = "AADM", active = true},
                            new {id = "ACL4", name = "PHOENIX cluster", typecode = "D   ", typename = "", parent = "AADM", active = true},
                            new {id = "ACL5", name = "CHEDDAR cluster", typecode = "D   ", typename = "", parent = "AADM", active = true},
                            new {id = "ACWU", name = "CA&ES COLLEGE WIDE UNITS", typecode = "N   ", typename = "", parent = "AAES", active = true},
                            new {id = "ADES", name = "ENVIRONMENTAL SCIENCE & POLICY (ESP)", typecode = "D   ", typename = "", parent = "AERS", active = true},
                            new {id = "ADNO", name = "DEAN'S OFFICE", typecode = "D   ", typename = "", parent = "AADM", active = true},
                            new {id = "AEDS", name = "ENVIRONMENTAL DESIGN", typecode = "D   ", typename = "", parent = "AHHD", active = true},
                            new {id = "AENT", name = "ENTOMOLOGY", typecode = "D   ", typename = "", parent = "AABL", active = true},
                            new {id = "AERS", name = "CA&ES ENV & RES SCIENCES & POLICY", typecode = "N   ", typename = "", parent = "AAES", active = true},
                            new {id = "AETX", name = "ENVIRONMENTAL TOXICOLOGY", typecode = "D   ", typename = "", parent = "AERS", active = true},
                            new {id = "AEVE", name = "AES EVOLUTION & ECOLOGY", typecode = "D   ", typename = "", parent = "ACBS", active = true},
                            new {id = "AFDS", name = "FOUNDATION PLANT SERVICES", typecode = "4   ", typename = "", parent = "APSC", active = true},
                            new {id = "AFST", name = "FOOD SCIENCE & TECHNOLOGY", typecode = "D   ", typename = "", parent = "AHHD", active = true},
                            new {id = "AGED", name = "AGRICULTURAL EDUCATION", typecode = "4   ", typename = "", parent = "ACWU", active = true},
                            new {id = "AHCD", name = "COMMUNITY & REGIONAL DEVELOPMENT", typecode = "D   ", typename = "", parent = "AHHD", active = true},
                            new {id = "AHCH", name = "HUMAN DEVELOPMENT AND FAMILY SERVICES", typecode = "D   ", typename = "", parent = "AHHD", active = true},
                            new {id = "AHHD", name = "CA&ES HUMAN HEALTH & DEVELOPMENT", typecode = "N   ", typename = "", parent = "AAES", active = true},
                            new {id = "AHIS", name = "AGRICULTURAL HISTORY", typecode = "4   ", typename = "", parent = "ACWU", active = true},
                            new {id = "ALAB", name = "UC DAVIS ANALYTICAL LAB", typecode = "4   ", typename = "", parent = "ACWU", active = true},
                            new {id = "ALAR", name = "LANDSCAPE ARCHITECTURE", typecode = "D   ", typename = "", parent = "AERS", active = true},
                            new {id = "ALAW", name = "LAWR", typecode = "D   ", typename = "", parent = "AERS", active = true},
                            new {id = "AMCB", name = "AES MOLECULAR & CELLULAR BIOLOGY", typecode = "D   ", typename = "", parent = "ACBS", active = true},
                            new {id = "AMIC", name = "AES MICROBIOLOGY", typecode = "D   ", typename = "", parent = "ACBS", active = true},
                            new {id = "ANEM", name = "NEMATOLOGY", typecode = "D   ", typename = "", parent = "AABL", active = true},
                            new {id = "ANPB", name = "AES NEUROBIOLOGY, PHYSIOLOGY, & BEHAVI", typecode = "D   ", typename = "", parent = "ACBS", active = true},
                            new {id = "ANUT", name = "NUTRITION", typecode = "D   ", typename = "", parent = "AHHD", active = true},
                            new {id = "APLB", name = "AES PLANT BIOLOGY", typecode = "D   ", typename = "", parent = "ACBS", active = true},
                            new {id = "APLS", name = "PLANT SCIENCES", typecode = "D   ", typename = "", parent = "APSC", active = true},
                            new {id = "APPA", name = "PLANT PATHOLOGY", typecode = "D   ", typename = "", parent = "APSC", active = true},
                            new {id = "APRV", name = "PROVISION FOR RECRUITMENT", typecode = "4   ", typename = "", parent = "AADM", active = true},
                            new {id = "APSC", name = "CA&ES PLANT SCIENCES", typecode = "N   ", typename = "", parent = "AAES", active = true},
                            new {id = "ARSV", name = "CA&ES RESERVES", typecode = "4   ", typename = "", parent = "AADM", active = true},
                            new {id = "ASPG", name = "SPECIAL PROGRAMS", typecode = "4   ", typename = "", parent = "ACWU", active = true},
                            new {id = "ATXC", name = "TEXTILES & CLOTHING", typecode = "D   ", typename = "", parent = "AHHD", active = true},
                            new {id = "AVIT", name = "VITCULTURE & ENOLOGY", typecode = "D   ", typename = "", parent = "APSC", active = true},
                            new {id = "AWFC", name = "WILDLIFE, FISH, & CONSERVATION BIOLOGY", typecode = "D   ", typename = "", parent = "AABL", active = true},
                            new {id = "AWRD", name = "CA&ES AWARDS", typecode = "4   ", typename = "", parent = "AADM", active = true},
                            new {id = "B000", name = "MCB:CLOSED OR DEAD ACCOUNTS", typecode = "4   ", typename = "", parent = "BMCB", active = true},
                            new {id = "BAFP", name = "ADULT FITNESS PROGRAM", typecode = "4   ", typename = "", parent = "BEXB", active = true},
                            new {id = "BCEF", name = "PB CONTROLLED ENVIRONMENT FACILITY", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "BCGD", name = "CENTER FOR GENETICS AND DEVELOPMENT", typecode = "4   ", typename = "", parent = "BMIC", active = true},
                            new {id = "BCNG", name = "CNS GRADUATE GROUP", typecode = "4   ", typename = "", parent = "BCNS", active = true},
                            new {id = "BCPB", name = "CENTER FOR POPULATION BIOLOGY", typecode = "4   ", typename = "", parent = "BEVE", active = true},
                            new {id = "BCPO", name = "CENTER FOR POPULATION BIOLOGY", typecode = "4   ", typename = "", parent = "BEVE", active = true},
                            new {id = "BCPX", name = "CENTER FOR POPULATION BIOLOGY", typecode = "4   ", typename = "", parent = "BEVE", active = true},
                            new {id = "BDNO", name = "CBS DEAN'S FUNDING ACCOUNTS", typecode = "D   ", typename = "", parent = "BSCF", active = true},
                            new {id = "BE00", name = "EXB CLOSED ACCOUNTS", typecode = "4   ", typename = "", parent = "BEXB", active = true},
                            new {id = "BEE0", name = "EDUCATION ENRICH & OUTREACH PROGRAMS", typecode = "4   ", typename = "", parent = "BMCB", active = true},
                            new {id = "BENG", name = "NCMHD CTR  EXCELLENCE IN NUTRI GENOMICS", typecode = "4   ", typename = "", parent = "BMCB", active = true},
                            new {id = "BETS", name = "MCB: ELECTRONICS SHOP", typecode = "4   ", typename = "", parent = "BMCB", active = true},
                            new {id = "BEVO", name = "EVOLUTION AND ECOLOGY", typecode = "4   ", typename = "", parent = "BEVE", active = true},
                            new {id = "BEVX", name = "EVOLUTION AND ECOLOGY", typecode = "4   ", typename = "", parent = "BEVE", active = true},
                            new {id = "BEXO", name = "EXERCISE BIOLOGY OPERATING ORG", typecode = "4   ", typename = "", parent = "BEXB", active = true},
                            new {id = "BGIF", name = "COLLEGE OF BIO SCI ENDOWMENTS & GIFTS", typecode = "D   ", typename = "", parent = "CBSD", active = true},
                            new {id = "BGRE", name = "PLANT BIOLOGY GREENHOUSE", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "BHRB", name = "PLANT BIOLOGY HERBARIUM", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "BINF", name = "BIOINFORMATICS", typecode = "4   ", typename = "", parent = "BGEN", active = true},
                            new {id = "BMCO", name = "MCB OPERATING EXPENSES", typecode = "4   ", typename = "", parent = "BMCB", active = true},
                            new {id = "BMGG", name = "MICROBIOLOGY GRADUATE GROUP", typecode = "4   ", typename = "", parent = "BMIC", active = true},
                            new {id = "BMIO", name = "MICROBIOLOGY", typecode = "4   ", typename = "", parent = "BMIC", active = true},
                            new {id = "BN00", name = "NPB CLOSED ACCOUNTS", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "BNPO", name = "NPB GEN OPERATING ORG", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "BOTA", name = "PLANT BIOLOGY CONSERVATORY", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "BPBG", name = "POPULATION BIOLOGY GRADUATE GROUP", typecode = "4   ", typename = "", parent = "BEVE", active = true},
                            new {id = "BPGG", name = "PLANT BIOLOGY GRADUATE GROUP", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "BPIA", name = "PLANT BIOLOGY-INACTIVE FOR CLOSED ACCTS", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "BPLO", name = "PLANT BIOLOGY OPERATING FUNDS", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "BSBB", name = "COLLEGE OF BIO SCI BUDGET BALANCE", typecode = "D   ", typename = "", parent = "BSCF", active = true},
                            new {id = "BSCW", name = "CBS COLLEGE-WIDE EXPENDITURE ACCTS", typecode = "D   ", typename = "", parent = "CBSD", active = true},
                            new {id = "BSDF", name = "DEAN'S OFFICE COLLEGE OF BIOL SCIENCES", typecode = "D   ", typename = "", parent = "CBSD", active = true},
                            new {id = "BSRD", name = "CBS DEAN'S 10-11 I&R REDUCTIONS", typecode = "D   ", typename = "", parent = "BSCF", active = true},
                            new {id = "BSRE", name = "CBS RETENTION FUNDING", typecode = "D   ", typename = "", parent = "BSCF", active = true},
                            new {id = "BSSU", name = "CBS FACULTY START-UP FUNDING", typecode = "D   ", typename = "", parent = "BSCF", active = true},
                            new {id = "EXPR", name = "GENOME AND BIOMEDICAL SCIENCES", typecode = "4   ", typename = "", parent = "BGEN", active = true},
                            new {id = "GBSF", name = "GENOME AND BIOMEDICAL SCIENCES FACILITY", typecode = "4   ", typename = "", parent = "BGEN", active = true},
                            new {id = "GCCF", name = "GENOME CENTER CORE FACILITIES", typecode = "4   ", typename = "", parent = "BGEN", active = true},
                            new {id = "GCOP", name = "GENOME CENTER OPERATING FUNDS", typecode = "4   ", typename = "", parent = "BGEN", active = true},
                            new {id = "GENO", name = "GENOME AND BIOMEDICAL SCIENCES", typecode = "4   ", typename = "", parent = "BGEN", active = true},
                            new {id = "MOLD", name = "MICROBIOLOGY", typecode = "4   ", typename = "", parent = "BMIC", active = true},
                            new {id = "NCNS", name = "ORG FOR CENTER FOR NEUROSCIENCE", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPAG", name = "ORG FOR ALDRIN GOMES", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPAH", name = "ORG FOR ANN HEDRICK", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPAI", name = "ORG FOR ANDY ISHIDA", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPAM", name = "ORG FOR ALEX MOGILNER", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPAS", name = "ORG FOR ARNIE SILLMAN", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPBH", name = "ORG FOR BARBARA HORWITZ", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPBM", name = "ORG FOR BRIAN MULLONEY", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPCF", name = "NEW ORG CHUCK FULLER", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPCW", name = "ORG FOR CRAIG WARDEN", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPDF", name = "NEW ORG FOR DAVE FURLOW", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPDH", name = "ORG FOR DAVID HAWKINS", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPDW", name = "ORG FOR DAVE WARLAND", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPEB", name = "ORG FOR ERWIN BAUTISTA", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPEC", name = "ORG FOR EARL CARSTENS", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPEW", name = "ORG FOR DOROTHY E. WOOLLEY", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPGN", name = "NEW ORG FOR GABY NEVITT", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPGR", name = "ORG FOR GRACE ROSENQUIST", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPJG", name = "ORG FOR JACK GOLDBERG", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPJH", name = "ORG FOR JOHN HOROWITZ", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPJS", name = "ORG FOR JIM SHAFFATH", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPJT", name = "ORG FOR JAMES TRIMMER", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPJW", name = "ORG FOR JOHN WINGFIELD", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPKB", name = "ORG FOR KEITH BAAR", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPKW", name = "ORG FOR KEITH WILLIAMS", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPLC", name = "ORG FOR LEO M CHALUPA", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPMR", name = "ORG FOR MARILYN RAMNOFSKY", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPMW", name = "ORG FOR MARTIN WILSON", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPPP", name = "ORG FOR PAM PAPPONE", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPPS", name = "ORG FOR PAUL SALITSKY", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPPW", name = "ORG FOR PHYLLIS WISE", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPRE", name = "ORG FOR GREGG RECANZONE", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPSB", name = "ORG FOR SUE BODINE", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPSD", name = "ORG FOR STUDENT FELLOWSHIP", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPSH", name = "ORG FOR SAMANTHA HARRIS", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPTH", name = "ORG FOR THOMAS COOMS-HAHN", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "NPWW", name = "ORG FOR JEFF W. WEIDNER", typecode = "4   ", typename = "", parent = "BNPB", active = true},
                            new {id = "PLBI", name = "PLANT BIOLOGY INACTIVE ACCTS", typecode = "4   ", typename = "", parent = "BPLB", active = true},
                            new {id = "PRSL", name = "MOLECULAR STRUCTURE FACILITY", typecode = "4   ", typename = "", parent = "BGEN", active = true}
                        }
                    );
            }
        }

        /// <summary>
        /// Inserting in all the current APLS accounts
        /// </summary>
        /// <param name="dbService"></param>
        private static void InsertAccounts(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                conn.Execute(
                    @"insert into vAccounts ([Id],[Name],[IsActive],[AccountManager],[PrincipalInvestigator],[OrganizationId]) VALUES (@id,@name,@active,@manager,@pi,@org)",
                    new[]
                        {
                            new {id = "3-6851000", name = "UNEXPENDED BALANCE: AG & ES: CONFERENCES", active = true, manager = "MADDERRA,DEIDRA A", pi = "", org = "APLS"},
                            new {id = "3-APSAC37", name = "HUTMACHER: COTTON INC. 04-448CA", active = true, manager = "MILLS,CAROL D.", pi = "HUTMACHER,ROBERT B", org = "APLS"},
                            new {id = "3-APSAR24", name = "PS:RRB:RM-2:2011:HILL", active = true, manager = "KAWAKAMI,HEATHER ERIKA", pi = "HILL,JAMES E", org = "APLS"},
                            new {id = "3-APSD102", name = "CRB: Generalized Reagentless Sensor to D", active = true, manager = "SINGH-VIDAL,SALLY", pi = "DANDEKAR,ABHAYA M", org = "APLS"},
                            new {id = "3-APSE095", name = "F. JACKSON HILLS MEMORIAL SCHOLARSHIP", active = true, manager = "KAWAKAMI,HEATHER ERIKA", pi = "", org = "APLS"},
                            new {id = "3-APSF376", name = "PS:USDA:GLOBAL RES ALL:58-3148-0-181:SIX", active = true, manager = "KAWAKAMI,HEATHER ERIKA", pi = "SIX,JOHAN W.U.A.", org = "APLS"},
                            new {id = "3-APSM077", name = "JACKSON:CCIA EVALUATION OF SMALL GRAINS", active = true, manager = "KAWAKAMI,HEATHER ERIKA", pi = "JACKSON,LELAND F", org = "APLS"},
                            new {id = "3-APSM152", name = "PUTNAM:CCIA:EXPERIMENTAL VARIETY AND GER", active = true, manager = "CHAVEZ,DANA L", pi = "PUTNAM,DANIEL H", org = "APLS"},
                            new {id = "3-APSM170", name = "CLOSE ACCT / CCIA / SEED PHYSIOL", active = true, manager = "REID,SERENA N", pi = "BRADFORD,KENT J", org = "APLS"},
                            new {id = "3-APSM326", name = "PUTNAM:CCIA:ALFALFA EXPERIMENTAL VARIETY", active = true, manager = "CHAVEZ,DANA L", pi = "PUTNAM,DANIEL H", org = "APLS"},
                            new {id = "3-APSO013", name = "PS:DRY BEAN FIELD DAY:TEMPLE", active = true, manager = "KAWAKAMI,HEATHER ERIKA", pi = "TEMPLE,STEVEN R", org = "APLS"},
                            new {id = "3-APSPR12", name = "FERGUSON:CA PISTACHIO RES BRD:DEVELOPING", active = true, manager = "KAWAKAMI,HEATHER ERIKA", pi = "FERGUSON,LOUISE", org = "APLS"},
                            new {id = "3-APSPR15", name = "PS:CA PISTACHIO:BLOOMCAST:FERGUSON", active = true, manager = "KAWAKAMI,HEATHER ERIKA", pi = "FERGUSON,LOUISE", org = "APLS"},
                            new {id = "3-GENAKH2", name = "PS: HJELMELAND: JASTRO SHIELDS AWD", active = true, manager = "MORGAN,SABRINA", pi = "HJELMELAND,ANNA K", org = "APLS"},
                            new {id = "3-PR68510", name = "PROVISION FOR ALLOCATION", active = true, manager = "KAWAKAMI,HEATHER ERIKA", pi = "", org = "APLS"},
                            new {id = "L-APSAC20", name = "COTTON INC:HUTMACHER MCA:WRIGHT", active = true, manager = "KAWAKAMI,HEATHER ERIKA", pi = "HUTMACHER,ROBERT B", org = "APLS"},
                            new {id = "L-APSRSTR", name = "STAFF TRAINING AND DEVELOPMENT FUNDING", active = true, manager = "MADDERRA,DEIDRA A", pi = "", org = "APLS"}
                        }
                    );
            }
        }

        private static void InsertVendors(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                //Adding in 25 random vendors
                conn.Execute(@"insert into vVendors ([Id],[Name],[OwnershipCode],[BusinessTypeCode]) VALUES (@id,@name,@ownership,@business)",
                    new[]
                        {
                            new {id = "0000247673", name = "ELLIOTT & NELSON", ownership = "U", business = "S"},
                            new {id = "0000005439", name = "HOGUE & ASSOCIATES INC", ownership = "C", business = "S"},
                            new {id = "0000057767", name = "SEA CHALLENGERS", ownership = "C", business = "S"},
                            new {id = "0000006829", name = "PENINSULA VALVE & FITTING INC", ownership = "C", business = "S"},
                            new {id = "0000006831", name = "PENN-AIR & HYDRAULICS CORPORATION", ownership = "O", business = "S"},
                            new {id = "0000006836", name = "PENN TOOL COMPANY", ownership = "H", business = "S"},
                            new {id = "0000006832", name = "PACIFIC DOOR & CLOSER COMPANY INC", ownership = "C", business = "S"},
                            new {id = "0000006837", name = "PENTERA INC", ownership = "C", business = "S"},
                            new {id = "0000006842", name = "PINNACLE BAY RESOURCE GROUP INC", ownership = "W", business = "S"},
                            new {id = "0000006848", name = "PINPOINT SOLUTIONS", ownership = "C", business = "S"},
                            new {id = "0000006847", name = "PACIFIC ENVIRONMENTAL CONTROL INC", ownership = "C", business = "S"},
                            new {id = "0000006849", name = "PERCIVAL SCIENTIFIC INC", ownership = "W", business = "S"},
                            new {id = "0000006853", name = "PERKINS WELDING WORKS", ownership = "C", business = "S"},
                            new {id = "0000006855", name = "PINOEER COATINGS COMPANY", ownership = "C", business = "S"},
                            new {id = "0000006856", name = "PERROTT DESKTOP PUBLISHING", ownership = "W", business = "S"},
                            new {id = "0000006859", name = "PERRYS ART SUPPLIES & FRAMING", ownership = "H", business = "S"},
                            new {id = "0000006867", name = "PET CETERA", ownership = "C", business = "S"},
                            new {id = "0000008573", name = "WINE PUBLICATIONS", ownership = "W", business = "S"},
                            new {id = "0000008574", name = "WOODS & POOLE ECONOMICS INC", ownership = "C", business = "S"},
                            new {id = "0000008575", name = "Z D WINES", ownership = "C", business = "S"},
                            new {id = "0000008577", name = "WOODWORKERS STORE", ownership = "W", business = "S"},
                            new {id = "0000008578", name = "V W EIMICKE ASSOCIATES INC", ownership = "W", business = "S"},
                            new {id = "0000008580", name = "ZELTEX INC", ownership = "H", business = "S"},
                            new {id = "0000008579", name = "THE YARDAGE SHOP", ownership = "W", business = "S"},
                            new {id = "0000008583", name = "ZENON COMPUTER SYSTEM INC", ownership = "A", business = "S"}
                        }
                    );

                //Adding the 43 addresses for those 25 vendors
                conn.Execute(
                    @"insert into vVendorAddresses ([VendorId],[TypeCode],[Name],[Line1],[Line2],[Line3],[City],[State],[Zip],[CountryCode]) 
                                            VALUES (@id,@typecode,@name,@line1,@line2,@line3,@city,@state,@zip,@country)",
                    new[]
                        {
                            new {id = "0000006849", typecode = "0005", name = "PERCIVAL SCIENTIFIC INC", line1 = "PO BOX 18", line2 = "", line3 = "", city = "DES MOINES", state = "IA", zip = "50301", country = "US"},
                            new {id = "0000008573", typecode = "0002", name = "WINE PUBLICATIONS", line1 = "ELIZABETH MARCUS", line2 = "", line3 = "", city = "BERKELEY", state = "CA", zip = "94708", country = "US"},
                            new {id = "0000006832", typecode = "0002", name = "PACIFIC DOOR & CLOSER COMPANY INC", line1 = "2112 ADAMS AVE", line2 = "", line3 = "", city = "SAN LEANDRO", state = "CA", zip = "94577", country = "US"},
                            new {id = "0000008573", typecode = "0001", name = "WINE PUBLICATIONS", line1 = "96 PARNASSUS RD", line2 = "", line3 = "", city = "BERKELEY", state = "CA", zip = "94708", country = "US"},
                            new {id = "0000008574", typecode = "0001", name = "WOODS & POOLE ECONOMICS INC", line1 = "1794 COLUMBIA RD NW", line2 = "", line3 = "", city = "WASHINGTON", state = "DC", zip = "20009-2808", country = "US"},
                            new {id = "0000008575", typecode = "0001", name = "Z D WINES", line1 = "8383 SILVERADO TRAIL", line2 = "", line3 = "", city = "NAPA", state = "CA", zip = "94558", country = "US"},
                            new {id = "0000008577", typecode = "0001", name = "WOODWORKERS STORE", line1 = "4365 WILLOW DR", line2 = "", line3 = "", city = "MEDINA", state = "MN", zip = "55340-9701", country = "US"},
                            new {id = "0000008578", typecode = "0001", name = "V W EIMICKE ASSOCIATES INC", line1 = "PO BOX 160", line2 = "", line3 = "", city = "BRONXVILLE", state = "NY", zip = "10708", country = "US"},
                            new {id = "0000008580", typecode = "0001", name = "ZELTEX INC", line1 = "130 WESTERN MARYLAND PKWY", line2 = "", line3 = "", city = "HAGERSTOWN", state = "MD", zip = "21740", country = "US"},
                            new {id = "0000006831", typecode = "0002", name = "PENN-AIR & HYDRAULICS CORPORATION", line1 = "PO BOX 132", line2 = "", line3 = "", city = "YORK", state = "PA", zip = "17405", country = "US"},
                            new {id = "0000005439", typecode = "0001", name = "HOGUE & ASSOCIATES INC", line1 = "550 KEARNY ST", line2 = "", line3 = "", city = "SAN FRANCISCO", state = "CA", zip = "95816", country = "US"},
                            new {id = "0000006842", typecode = "0001", name = "PINNACLE BAY RESOURCE GROUP INC", line1 = "2934 GOLD PAN CT", line2 = "", line3 = "", city = "RANCHO CORDOVA", state = "CA", zip = "95670", country = "US"},
                            new {id = "0000006855", typecode = "0001", name = "PINOEER COATINGS COMPANY", line1 = "10054 D MILL STATION RD", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95827", country = "US"},
                            new {id = "0000006859", typecode = "0001", name = "PERRYS ART SUPPLIES & FRAMING", line1 = "128 GREENSFIELD AVE", line2 = "", line3 = "", city = "SAN ANSELMO", state = "CA", zip = "94960", country = "US"},
                            new {id = "0000006853", typecode = "0001", name = "PERKINS WELDING WORKS", line1 = "8524 FLORIN RD", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95829", country = "US"},
                            new {id = "0000006853", typecode = "0002", name = "PERKINS WELDING WORKS", line1 = "PO BOX 292580", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95829", country = "US"},
                            new {id = "0000006836", typecode = "0001", name = "PENN TOOL COMPANY", line1 = "1776 SPRINGFIELD AVE", line2 = "", line3 = "", city = "MAPLEWOOD", state = "NJ", zip = "07040", country = "US"},
                            new {id = "0000006829", typecode = "0001", name = "PENINSULA VALVE & FITTING INC", line1 = "1260 PEAR AVE", line2 = "", line3 = "", city = "MOUNTAIN VIEW", state = "CA", zip = "94043", country = "US"},
                            new {id = "0000006831", typecode = "0001", name = "PENN-AIR & HYDRAULICS CORPORATION", line1 = "1750 INDUSTRIAL WY", line2 = "", line3 = "", city = "YORK", state = "PA", zip = "17402", country = "US"},
                            new {id = "0000006837", typecode = "0001", name = "PENTERA INC", line1 = "8650 COMMERCE PARK PL", line2 = "", line3 = "", city = "INDIANAPOLIS", state = "IN", zip = "46268", country = "US"},
                            new {id = "0000006832", typecode = "0001", name = "PACIFIC DOOR & CLOSER COMPANY INC", line1 = "395 MENDELL ST", line2 = "", line3 = "", city = "SAN FRANCISCO", state = "CA", zip = "94124", country = "US"},
                            new {id = "0000008579", typecode = "0001", name = "THE YARDAGE SHOP", line1 = "3016 J ST", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95816", country = "US"},
                            new {id = "0000008577", typecode = "0002", name = "WOODWORKERS STORE", line1 = "PO BOX 500", line2 = "", line3 = "", city = "MEDINA", state = "MN", zip = "55340", country = "US"},
                            new {id = "0000006849", typecode = "0001", name = "PERCIVAL SCIENTIFIC INC", line1 = "1805 E FOURTH ST", line2 = "", line3 = "", city = "BOONE", state = "IA", zip = "50036-0249", country = "US"},
                            new {id = "0000006849", typecode = "0004", name = "PERCIVAL SCIENTIFIC INC", line1 = "505 RESEARCH DR", line2 = "", line3 = "", city = "PERRY", state = "IA", zip = "50220", country = "US"},
                            new {id = "0000006848", typecode = "0001", name = "PINPOINT SOLUTIONS", line1 = "9647 FOLSOM BLVD", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95827", country = "US"},
                            new {id = "0000006856", typecode = "0001", name = "PERROTT DESKTOP PUBLISHING", line1 = "17560 HIGHLANDS BLVD", line2 = "", line3 = "", city = "SONOMA", state = "CA", zip = "95476", country = "US"},
                            new {id = "0000006856", typecode = "0002", name = "PERROTT DESKTOP PUBLISHING", line1 = "PO BOX 296", line2 = "", line3 = "", city = "SONOMA", state = "CA", zip = "95476", country = "US"},
                            new {id = "0000006867", typecode = "0001", name = "PET CETERA", line1 = "612 4TH ST", line2 = "", line3 = "", city = "DAVIS", state = "CA", zip = "95616", country = "US"},
                            new {id = "0000006847", typecode = "0001", name = "PACIFIC ENVIRONMENTAL CONTROL INC", line1 = "3420 FOSTORIA WAY", line2 = "", line3 = "", city = "SAN RAMON", state = "CA", zip = "94583", country = "US"},
                            new {id = "0000057767", typecode = "0001", name = "SEA CHALLENGERS", line1 = "35 VERSAILLES CT", line2 = "", line3 = "", city = "DANVILLE", state = "CA", zip = "94506-4454", country = "US"},
                            new {id = "0000005439", typecode = "0005", name = "HOGUE & ASSOCIATES INC", line1 = "C/O HOGUE & ASSOCIATES INC", line2 = "", line3 = "", city = "EAST GREENVILLE", state = "PA", zip = "18041", country = "US"},
                            new {id = "0000006849", typecode = "0002", name = "PERCIVAL SCIENTIFIC INC", line1 = "C/O HART/LATIMER ASSOCIATES INC", line2 = "", line3 = "", city = "SAN CARLOS", state = "CA", zip = "94070", country = "US"},
                            new {id = "0000057767", typecode = "0002", name = "SEA CHALLENGERS", line1 = "5091 DEBBIE CRT", line2 = "", line3 = "", city = "GIG HARBOR", state = "WA", zip = "98335", country = "US"},
                            new {id = "0000006849", typecode = "0003", name = "PERCIVAL SCIENTIFIC INC", line1 = "PO BOX 249", line2 = "", line3 = "", city = "BOONE", state = "IA", zip = "50036", country = "US"},
                            new {id = "0000005439", typecode = "0004", name = "HOGUE & ASSOCIATES INC", line1 = "PO BOX 841366", line2 = "", line3 = "", city = "DALLAS", state = "TX", zip = "75284-1366", country = "US"},
                            new {id = "0000005439", typecode = "0002", name = "HOGUE & ASSOCIATES INC", line1 = "1515 30 TH ST", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95816", country = "US"},
                            new {id = "0000005439", typecode = "0007", name = "HOGUE & ASSOCIATES INC", line1 = "7300 FOLSOM BLVD STE 103", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95826", country = "US"},
                            new {id = "0000005439", typecode = "0008", name = "HOGUE & ASSOCIATES INC", line1 = "GUNLOCKE/E&I CNR01172", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95816", country = "US"},
                            new {id = "0000005439", typecode = "0003", name = "HOGUE & ASSOCIATES INC", line1 = "C/O HOGUE & ASSOCIATES INC", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95816", country = "US"},
                            new {id = "0000005439", typecode = "0006", name = "HOGUE & ASSOCIATES INC", line1 = "COMMERCIAL FURNISHING DIV", line2 = "", line3 = "", city = "SAN FRANCISCO", state = "CA", zip = "94108", country = "US"},
                            new {id = "0000247673", typecode = "0001", name = "ELLIOTT & NELSON", line1 = "PO BOX 195", line2 = "", line3 = "", city = "AVERY", state = "CA", zip = "95224", country = "US"}
                        }
                    );
            }
        }

        private static void InsertCommodityCodes(IDbService dbService)
        {
            //Getting commodity groups and codes for groups 10 & 40
            using (var conn = dbService.GetConnection())
            {
                conn.Execute(
                    @"insert into vCommodityGroups ([GroupCode],[Name],[SubGroupCode],[SubGroupName]) VALUES (@groupcode,@name,@subgroupcode,@subgroupname)",
                    new[]
                        {
                            new {groupcode = "10", name = "Hardware", subgroupcode = "11", subgroupname = "Paint and Supplies"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "14", subgroupname = "Metals"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "18", subgroupname = "Carpentry, Hardware"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "99", subgroupname = "Miscellaneous"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "EQ", subgroupname = "Motors, Generators & Miscellaneous"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "26", subgroupname = "Plumbing"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "35", subgroupname = "Tools (Non Inventorial)"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "32", subgroupname = "Electrical"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "00", subgroupname = "Chemicals, Miscellaneous"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "EQ", subgroupname = "Equipment"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "13", subgroupname = "Gases, Compressed"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "50", subgroupname = "Plasticware"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "59", subgroupname = "Furniture"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "99", subgroupname = "Miscellaneous"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "40", subgroupname = "Glassware & Ceramics"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "14", subgroupname = "Gases, Liquid"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "SE", subgroupname = "Non-Inventorial Equipment"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "19", subgroupname = "Carpentry, Lumber"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "20", subgroupname = "Carpentry, Floor & Wall Materials"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "21", subgroupname = "Carpentry, Ladders & Scaffolding"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "40", subgroupname = "Lighting Fixtures & Accessories"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "11", subgroupname = "Measuring & Testing Devices"},
                            new {groupcode = "10", name = "Hardware", subgroupcode = "50", subgroupname = "Adhesives and Sealants"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "RE", subgroupname = "Rental"},
                            new {groupcode = "40", name = "Laboratory", subgroupcode = "01", subgroupname = "Chemicals, Restricted"}
                        });

                conn.Execute(
                    @"insert into vCommodities ([Id],[Name],[GroupCode],[SubGroupCode]) VALUES (@id,@name,@groupcode,@subgroupcode)",
                    new[]
                        {
                            new {id = "13165", name = "ADHESIVE HARDENER/ACCELERATOR", groupcode = "10", subgroupcode = "50"},
                            new {id = "14051", name = "CARBON STEEL, FORMED, SLIDING DOOR TRACK", groupcode = "10", subgroupcode = "18"},
                            new {id = "32490", name = "POWER SOURCE, NOT 32400-32418, INCL BIN OR VOLTAGE CLAMP", groupcode = "10", subgroupcode = "32"},
                            new {id = "11700", name = "STAIN/DYE", groupcode = "10", subgroupcode = "11"},
                            new {id = "33453", name = "CAP/RECEPTACLE/BODY, 4 WIRE, 30A/125/250/600V & 20A/250V", groupcode = "10", subgroupcode = "32"},
                            new {id = "48371", name = "ELECTRICAL CURVE TRACER & ACCESSORIES", groupcode = "10", subgroupcode = "32"},
                            new {id = "19341-1", name = "MORTAR, GROUNDS", groupcode = "10", subgroupcode = "99"},
                            new {id = "27220", name = "VALVE, DIRECTION CONTROL, NOT SOLENOID", groupcode = "10", subgroupcode = "26"},
                            new {id = "1XXXX", name = "PRIMARY MATERIALS, LUMBER/GLASS/METALS/ ROPE/WIRE", groupcode = "10", subgroupcode = "99"},
                            new {id = "36332", name = "PLANING/SHAPING, SCRAPER, INCL PUTTY KNIFE", groupcode = "10", subgroupcode = "35"},
                            new {id = "36711", name = "TONGS, NONPOWERED", groupcode = "10", subgroupcode = "35"},
                            new {id = "12060", name = "LUBRICANT FOR SPECIFIC FUNCTIONS, NOT PULLING COMPOUND", groupcode = "10", subgroupcode = "99"},
                            new {id = "3XXXX", name = "GENERAL PURPOSE HARDWARE & TOOLS", groupcode = "10", subgroupcode = "35"},
                            new {id = "32170", name = "LAMP/TUBE, FLUORESCENT, STARTER REQ, BIAXIAL OR TWIN TUBE", groupcode = "10", subgroupcode = "32"},
                            new {id = "C6174-1", name = "VACUUM SYSTEM, INCL VACUUM CHAMBER", groupcode = "10", subgroupcode = "EQ"},
                            new {id = "27126", name = "GLOBE VALVE, ANGLE", groupcode = "10", subgroupcode = "26"},
                            new {id = "18037", name = "WOOD, MAPLE, TRIM/FINISH", groupcode = "10", subgroupcode = "18"},
                            new {id = "C6171-06", name = "PUMP, POSITIVE DISPLACEMENT/PROGRESSIVE CAVITY, INDUSTRIAL", groupcode = "10", subgroupcode = "EQ"},
                            new {id = "32191", name = "LAMP/TUBE, FLUORESCENT, NO STARTER REQ, MEDIUM BIPIN", groupcode = "10", subgroupcode = "32"},
                            new {id = "D8632-2", name = "SOLDERING/DESOLDERING KIT/SYSTEM, POWERED, INDUSTRIAL", groupcode = "10", subgroupcode = "EQ"},
                            new {id = "40053-57", name = "CITRONELLOL, 95% 100ML 106-22-9", groupcode = "40", subgroupcode = "00"},
                            new {id = "40024-82", name = "CARBON ACTIVATED USP POW 2.5KG", groupcode = "40", subgroupcode = "00"},
                            new {id = "40152-19", name = "ETHYLENE GLYCOL, P.A.  1L 107-21-1", groupcode = "40", subgroupcode = "00"},
                            new {id = "43600-33", name = "CARCINOGEN, TALC CONTAINING ASBESTIFORM FIBERS, CLASS II", groupcode = "40", subgroupcode = "00"},
                            new {id = "E8143-1", name = "LINEAR DISPLACEMENT ENCODER/SENSOR, LAB", groupcode = "40", subgroupcode = "EQ"},
                            new {id = "40035-63", name = "PROPYLENE GLYCOL USP/FCC 4L 57-55-6", groupcode = "40", subgroupcode = "00"},
                            new {id = "40054-44", name = "CYCLOHEXENE SULFIDE, TEC   5GR 286-28-2", groupcode = "40", subgroupcode = "00"},
                            new {id = "46325", name = "MICROSCOPE, OPTICS, ILLUMINATOR/CONDENSER", groupcode = "40", subgroupcode = "99"},
                            new {id = "40151-67", name = "3,4-DICHLOROBENZALDEHYDE 25GR 6287-38-3", groupcode = "40", subgroupcode = "00"},
                            new {id = "40128-99", name = "ACETIC-D3 ACID-D, 100.0    5GR 1186-52-3", groupcode = "40", subgroupcode = "00"},
                            new {id = "40091-80", name = "2-NAPHTHALENESULFONYL CH 100GR 93-11-8", groupcode = "40", subgroupcode = "00"},
                            new {id = "40135-39", name = "BARIUM SULFATE EXTRA PU 500GR 7727-43-7", groupcode = "40", subgroupcode = "00"},
                            new {id = "46271", name = "BULB, RUBBER/PLASTIC/LATEX, LAB", groupcode = "40", subgroupcode = "99"},
                            new {id = "40096-25", name = "5-METHOXYSALICYLIC ACID,  10GR 2612-02-4", groupcode = "40", subgroupcode = "00"},
                            new {id = "40070-17", name = "1-NAPHTHYLACETIC ACID, 9 100GR 86-87-3", groupcode = "40", subgroupcode = "00"},
                            new {id = "40104-09", name = "4-NITRO-M-XYLENE, 99% 100GR 89-87-2", groupcode = "40", subgroupcode = "00"},
                            new {id = "40098-87", name = "DECANOIC ACID, 99+%      500GR 334-48-5", groupcode = "40", subgroupcode = "00"},
                            new {id = "40145-95", name = "LITHIUM DIISOPROPYLAMIDE 100ML 4111-54-0", groupcode = "40", subgroupcode = "00"},
                            new {id = "40131-85", name = "PHENYL CHLOROFORMATE, 97 250ML 1885-14-9", groupcode = "40", subgroupcode = "00"},
                            new {id = "40017-14", name = "ALUM CHLOR HEX USP CRYST 500GM", groupcode = "40", subgroupcode = "00"}
                        });
            }
        }

        private static void InsertOrderStatusCodes(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                conn.Execute(
                    @"insert into OrderStatusCodes ([Id],[Name],[Level], [IsComplete], [KfsStatus]) VALUES (@id,@name,@level, @isComplete, @kfsstatus)",
                    new[]
                        {
                            new { id="RQ", Name="Requester", Level=1, IsComplete=false, KfsStatus=false },
                            new { id="AP", Name="Approver", Level=2, IsComplete=false, KfsStatus=false},
                            new { id="AM", Name="AccountManager", Level=3, IsComplete=false, KfsStatus=false},
                            new { id="PR", Name="Purchaser", Level=4, IsComplete=false, KfsStatus=false},
                            new { id="CN", Name="Complete-Not Uploaded KFS", Level=-1, IsComplete=true, KfsStatus=false},
                            new { id="CP", Name="Complete", Level=-1, IsComplete=true, KfsStatus=false}
                        });

                conn.Execute(@"update OrderStatusCodes set Level = null where Level = -1");
            }
        }
    }
}
