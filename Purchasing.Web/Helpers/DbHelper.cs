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
                                 "Permissions", "UsersXOrganizations", "Users", "Roles", "vAccounts", "vOrganizations", "vVendorAddresses", "vVendors"
                             };

            var dbService = ServiceLocator.Current.GetInstance<IDbService>();

            using (var conn = dbService.GetConnection())
            {
                foreach (var table in tables)
                {
                    conn.Execute("delete from " + table);
                }
            }

            InsertOrganizations(dbService);
            InsertAccounts(dbService);
            InsertVendors(dbService);
            
            var session = NHibernateSessionManager.Instance.GetSession();
            session.BeginTransaction();

            InsertData(session);

            session.Transaction.Commit();
        }

        private static void InsertData(ISession session)
        {
            //Now insert new data
            var scott = new User("postit") { FirstName = "Scott", LastName = "Kirkland", Email = "srkirkland@ucdavis.edu", IsActive = true };
            var alan = new User("anlai") { FirstName = "Alan", LastName = "Lai", Email = "anlai@ucdavis.edu", IsActive = true };
            var ken = new User("taylorkj") {FirstName = "Ken", LastName = "Taylor", Email = "taylorkj@ucdavis.edu", IsActive = true};
            
            var admin = new Role("AD") { Name = "Admin" };
            var deptAdmin = new Role("DA") { Name = "DepartmentalAdmin" };
            var user = new Role("US") { Name = "User" };

            ken.Organizations.Add(session.Get<Organization>("AANS"));

            var testWorkgroup = new Workgroup() { Name = "Test Workgroup", IsActive = true };

            session.Save(testWorkgroup);

            session.Save(scott);
            session.Save(alan);
            session.Save(ken);

            session.Save(admin);
            session.Save(deptAdmin);
            session.Save(user);

            Roles.AddUsersToRole(new[] {"postit", "anlai"}, "AD");
            Roles.AddUserToRole("anlai", "US");
            Roles.AddUserToRole("taylorkj", "DA");

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
                    @"insert into vVendorAddresses ([VendorId],[Name],[Line1],[Line2],[Line3],[City],[State],[Zip],[CountryCode]) 
                                            VALUES (@id,@name,@line1,@line2,@line3,@city,@state,@zip,@country)",
                    new[]
                        {
                            new {id = "0000006849", name = "PERCIVAL SCIENTIFIC INC", line1 = "PO BOX 18", line2 = "", line3 = "", city = "DES MOINES", state = "IA", zip = "50301", country = "US"},
                            new {id = "0000008573", name = "WINE PUBLICATIONS", line1 = "ELIZABETH MARCUS", line2 = "96 PANASSUS RD", line3 = "", city = "BERKELEY", state = "CA", zip = "94708", country = "US"},
                            new {id = "0000006832", name = "PACIFIC DOOR & CLOSER CO INC", line1 = "2112 ADAMS AVE", line2 = "", line3 = "", city = "SAN LEANDRO", state = "CA", zip = "94577", country = "US"},
                            new {id = "0000008573", name = "WINE PUBLICATIONS", line1 = "96 PARNASSUS RD", line2 = "", line3 = "", city = "BERKELEY", state = "CA", zip = "94708", country = "US"},
                            new {id = "0000008574", name = "WOODS & POOLE ECONOMICS INC", line1 = "1794 COLUMBIA RD NW", line2 = "STE 4", line3 = "", city = "WASHINGTON", state = "DC", zip = "20009-2808", country = "US"},
                            new {id = "0000008575", name = "Z D WINES", line1 = "8383 SILVERADO TRAIL", line2 = "", line3 = "", city = "NAPA", state = "CA", zip = "94558", country = "US"},
                            new {id = "0000008577", name = "WOODWORKERS STORE", line1 = "4365 WILLOW DR", line2 = "", line3 = "", city = "MEDINA", state = "MN", zip = "55340-9701", country = "US"},
                            new {id = "0000008578", name = "V W EIMICKE ASSOCIATES INC", line1 = "PO BOX 160", line2 = "", line3 = "", city = "BRONXVILLE", state = "NY", zip = "10708", country = "US"},
                            new {id = "0000008580", name = "ZELTEX INC", line1 = "130 WESTERN MARYLAND PKWY", line2 = "", line3 = "", city = "HAGERSTOWN", state = "MD", zip = "21740", country = "US"},
                            new {id = "0000006831", name = "PENN-AIR & HYDRAULICS CORPORATION", line1 = "PO BOX 132", line2 = "", line3 = "", city = "YORK", state = "PA", zip = "17405", country = "US"},
                            new {id = "0000005439", name = "HOGUE & ASSOCIATES INC", line1 = "550 KEARNY ST", line2 = "STE 500", line3 = "", city = "SAN FRANCISCO", state = "CA", zip = "95816", country = "US"},
                            new {id = "0000006842", name = "PINNACLE BAY RESOURCE GROUP INC", line1 = "2934 GOLD PAN CT", line2 = "STE 8", line3 = "", city = "RANCHO CORDOVA", state = "CA", zip = "95670", country = "US"},
                            new {id = "0000006855", name = "PIONEER COATINGS COMPANY", line1 = "10054 D MILL STATION RD", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95827", country = "US"},
                            new {id = "0000006859", name = "PERRYS ART SUPPLIES & FRAMING", line1 = "128 GREENSFIELD AVE", line2 = "", line3 = "", city = "SAN ANSELMO", state = "CA", zip = "94960", country = "US"},
                            new {id = "0000006853", name = "PERKINS WELDING WORKS", line1 = "8524 FLORIN RD", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95829", country = "US"},
                            new {id = "0000006853", name = "PERKINS WELDING WORKS", line1 = "PO BOX 292580", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95829", country = "US"},
                            new {id = "0000006836", name = "PENN TOOL COMPANY", line1 = "1776 SPRINGFIELD AVE", line2 = "", line3 = "", city = "MAPLEWOOD", state = "NJ", zip = "07040", country = "US"},
                            new {id = "0000006829", name = "PENINSULA VALVE & FITTING INC", line1 = "1260 PEAR AVE", line2 = "", line3 = "", city = "MOUNTAIN VIEW", state = "CA", zip = "94043", country = "US"},
                            new {id = "0000006831", name = "PENN-AIR & HYDRAULICS CORPORATION", line1 = "1750 INDUSTRIAL WY", line2 = "", line3 = "", city = "YORK", state = "PA", zip = "17402", country = "US"},
                            new {id = "0000006837", name = "PENTERA INC", line1 = "8650 COMMERCE PARK PL", line2 = "STE G", line3 = "", city = "INDIANAPOLIS", state = "IN", zip = "46268", country = "US"},
                            new {id = "0000006832", name = "PACIFIC DOOR & CLOSER COMPANY INC", line1 = "395 MENDELL ST", line2 = "", line3 = "", city = "SAN FRANCISCO", state = "CA", zip = "94124", country = "US"},
                            new {id = "0000008583", name = "ZENON COMPUTER SYSTEM INC", line1 = "18343 E GALE AVE", line2 = "", line3 = "", city = "CITY OF INDUSTRY", state = "CA", zip = "91748", country = "US"},
                            new {id = "0000008579", name = "THE YARDAGE SHOP", line1 = "3016 J ST", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95816", country = "US"},
                            new {id = "0000008577", name = "WOODWORKERS STORE", line1 = "PO BOX 500", line2 = "", line3 = "", city = "MEDINA", state = "MN", zip = "55340", country = "US"},
                            new {id = "0000006849", name = "DO NOT USE - CITE 0004", line1 = "1805 E FOURTH ST", line2 = "", line3 = "", city = "BOONE", state = "IA", zip = "50036-0249", country = "US"},
                            new {id = "0000006849", name = "PERCIVAL SCIENTIFIC INC", line1 = "505 RESEARCH DR", line2 = "", line3 = "", city = "PERRY", state = "IA", zip = "50220", country = "US"},
                            new {id = "0000006848", name = "PINPOINT SOLUTIONS", line1 = "9647 FOLSOM BLVD", line2 = "STE 210", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95827", country = "US"},
                            new {id = "0000006856", name = "PERROTT DESKTOP PUBLISHING", line1 = "17560 HIGHLANDS BLVD", line2 = "", line3 = "", city = "SONOMA", state = "CA", zip = "95476", country = "US"},
                            new {id = "0000006856", name = "PERROTT DESKTOP PUBLISHING", line1 = "PO BOX 296", line2 = "", line3 = "", city = "SONOMA", state = "CA", zip = "95476", country = "US"},
                            new {id = "0000006867", name = "PET CETERA", line1 = "612 4TH ST", line2 = "", line3 = "", city = "DAVIS", state = "CA", zip = "95616", country = "US"},
                            new {id = "0000006847", name = "PACIFIC ENVIRONMENTAL CONTROL INC", line1 = "3420 FOSTORIA WAY", line2 = "STE G", line3 = "", city = "SAN RAMON", state = "CA", zip = "94583", country = "US"},
                            new {id = "0000057767", name = "SEA CHALLENGERS NATURAL HISTORY BOOKS", line1 = "35 VERSAILLES CT", line2 = "", line3 = "", city = "DANVILLE", state = "CA", zip = "94506-4454", country = "US"},
                            new {id = "0000005439", name = "KNOLL NORTH AMERICA", line1 = "C/O HOGUE & ASSOCIATES INC", line2 = "PO BOX 157", line3 = "", city = "EAST GREENVILLE", state = "PA", zip = "18041", country = "US"},
                            new {id = "0000006849", name = "DO NOT USE/CITE 0000005289", line1 = "C/O HART/LATIMER ASSOCIATES INC", line2 = "655 SKY WAY  STE 113", line3 = "", city = "SAN CARLOS", state = "CA", zip = "94070", country = "US"},
                            new {id = "0000057767", name = "SEA CHALLENGERS NATURAL HISTORY BOOKS", line1 = "5091 DEBBIE CRT", line2 = "", line3 = "", city = "GIG HARBOR", state = "WA", zip = "98335", country = "US"},
                            new {id = "0000006849", name = "DO NOT USE - CITE 0004", line1 = "PO BOX 249", line2 = "", line3 = "", city = "BOONE", state = "IA", zip = "50036", country = "US"},
                            new {id = "0000005439", name = "KNOLL INC", line1 = "PO BOX 841366", line2 = "", line3 = "", city = "DALLAS", state = "TX", zip = "75284-1366", country = "US"},
                            new {id = "0000005439", name = "HOGUE & ASSOCIATES INC", line1 = "1515 30 TH ST", line2 = "STE 200", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95816", country = "US"},
                            new {id = "0000005439", name = "HOGUE & ASSOCIATES INC", line1 = "7300 FOLSOM BLVD STE 103", line2 = "", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95826", country = "US"},
                            new {id = "0000247673", name = "ELLIOTT & NELSON", line1 = "PO BOX 195", line2 = "", line3 = "", city = "AVERY", state = "CA", zip = "95224", country = "US"},
                            new {id = "0000005439", name = "HOGUE & ASSOC - SACRAMENTO", line1 = "GUNLOCKE/E&I CNR01172", line2 = "1515 30TH ST, SUITE 200", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95816", country = "US"},
                            new {id = "0000005439", name = "DO NOT USE", line1 = "C/O HOGUE & ASSOCIATES INC", line2 = "1515 30TH ST", line3 = "", city = "SACRAMENTO", state = "CA", zip = "95816", country = "US"},
                            new {id = "0000005439", name = "HOGUE", line1 = "COMMERCIAL FURNISHING DIV", line2 = "550 KEARNY ST STE 500", line3 = "", city = "SAN FRANCISCO", state = "CA", zip = "94108", country = "US"}
                        }
                    );
            }
        }
    }
}