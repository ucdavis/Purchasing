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
                                 "Permissions", "Users", "Roles", "vOrganizations"
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
            
            var session = NHibernateSessionManager.Instance.GetSession();
            session.BeginTransaction();

            InsertData(session);

            session.Transaction.Commit();
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

        private static void InsertOrganizations(IDbService dbService)
        {
            using (var conn = dbService.GetConnection())
            {
                conn.Execute(
                    @"insert into vOrganizations ([Id],[Name],[TypeCode],[TypeName],[ParentId],[IsActive]) VALUES (@id,@name,@typecode,@typename,@parent,@active)",
                    new[]
                        {
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
                    @"insert into vAccounts ([Id],[Name],[IsActive],[AccountManager],[PI],[OrganizationId]) VALUES (@id,@name,@active,@manager,@pi,@org)",
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
    }
}