using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Text;
using System.Configuration;
using Microsoft.Extensions.Options;

namespace Purchasing.Mvc.Services
{
    public interface IDirectorySearchService
    {
        /// <summary>
        /// Searches for users across many different critera
        /// </summary>
        /// <param name="searchTerm">
        /// Login, email or lastName
        /// </param>
        /// <returns></returns>
        List<DirectoryUser> SearchUsers(string searchTerm);

        /// <summary>
        /// Returns the single user that matches the search term -- either loginID or email
        /// </summary>
        DirectoryUser FindUser(string searchTerm);
    }

    public class DirectorySearchService : IDirectorySearchService
    {
        private const string STR_CN = "cn";
        private const string STR_DisplayName = "displayName";
        private const string STR_EmployeeNumber = "employeeNumber";
        private const string STR_GivenName = "givenName";
        private const string STR_Mail = "mail";
        private const string STR_SearchBase = "ou=People,dc=ucdavis,dc=edu";
        private const string STR_SN = "sn";
        private const string STR_Telephone = "telephoneNumber";
        private const string STR_UID = "uid";
        private const string STR_PIDM = "ucdPersonPIDM";
        private const string STR_StudentId = "ucdStudentSID";
        private readonly string LDAPPassword;
        private readonly string LDAPUser;
        private static readonly int STR_LDAPPort = 636;
        //private static readonly string STR_LDAPURL = "ldap.ucdavis.edu";
        //private static readonly string STR_LDAPIP = "128.120.32.44"; //specific node per T.Poage //"128.120.32.63";
        private static readonly string STR_LDAPOLD = "ldap-old.ucdavis.edu"; //via T.Poage: fast-delete setting in the load balancer entry

        public DirectorySearchService(IOptions<LDAPSettings> ldapSettings)
        {
            LDAPPassword = ldapSettings.Value.LDAPPassword;
            LDAPUser = ldapSettings.Value.LDAPUser;
        }

        public SearchResponse GetSearchResponse(string searchFilter, string searchBase, int sizeLimit = 500)
        {
            //Establishing a Connection to the LDAP Server
            //var ldapident = new LdapDirectoryIdentifier(STR_LDAPURL, STR_LDAPPort);
            var ldapident = new LdapDirectoryIdentifier(STR_LDAPOLD, STR_LDAPPort);
            //LdapConnection lc = new LdapConnection(ldapident, null, AuthType.Basic);
            using (var lc = new LdapConnection(ldapident, new NetworkCredential(LDAPUser, LDAPPassword), AuthType.Basic))
            {
                lc.SessionOptions.ProtocolVersion = 3;
                lc.SessionOptions.SecureSocketLayer = true;
                lc.SessionOptions.VerifyServerCertificate = (connection, certificate) => true;
                lc.Bind();

                //Configure the Search Request to Query the UCD OpenLDAP Server's People Search Base for a Specific User ID or Mail ID and Return the Requested Attributes 
                var attributesToReturn = new string[]
                                         {
                                             STR_UID, STR_EmployeeNumber, STR_Mail, STR_Telephone, STR_DisplayName, STR_CN,
                                             STR_SN, STR_GivenName, STR_PIDM
                                         };

                var sRequest = new SearchRequest(searchBase, searchFilter, SearchScope.Subtree, attributesToReturn) { SizeLimit = sizeLimit };

                //Send the Request and Load the Response
                var sResponse = (SearchResponse)lc.SendRequest(sRequest);

                return sResponse;
            }
        }

        public static List<DirectoryUser> GetUsersFromResponse(SearchResponse sResponse)
        {
            var users = new List<DirectoryUser>();

            foreach (SearchResultEntry result in sResponse.Entries)
            {
                var user = new DirectoryUser();

                //Grab out the first response entry

                foreach (DirectoryAttribute attr in result.Attributes.Values)
                {
                    switch (attr.Name)
                    {
                        case STR_UID:
                            user.LoginId = attr[0].ToString();
                            break;
                        case STR_GivenName:
                            user.FirstName = attr[0].ToString();
                            break;
                        case STR_SN:
                            user.LastName = attr[0].ToString();
                            break;
                        case STR_Mail:
                            user.EmailAddress = attr[0].ToString();
                            break;
                        case STR_EmployeeNumber:
                            user.EmployeeId = attr[0].ToString();
                            break;
                        case STR_CN:
                            user.FullName = attr[0].ToString();
                            break;
                        case STR_Telephone:
                            user.PhoneNumber = attr[0].ToString();
                            break;
                        default:
                            break;
                    }
                }

                users.Add(user);
            }

            return users;
        }

        public List<DirectoryUser> LDAPSearchUsers(string employeeId = null, string firstName = null, 
                                                            string lastName = null, string fullName = null,
                                                            string loginId = null, string email = null, bool useAnd = true)
        {
            //If all params are null or whitespace, just return empty list
            if (string.IsNullOrWhiteSpace(employeeId) 
                && string.IsNullOrWhiteSpace(firstName) 
                && string.IsNullOrWhiteSpace(lastName) 
                && string.IsNullOrWhiteSpace(loginId)
                && string.IsNullOrWhiteSpace(email)
                && string.IsNullOrWhiteSpace(fullName))
            {
                return new List<DirectoryUser>();
            }
                

            var searchFilter = new StringBuilder();
            searchFilter.Append(useAnd ? "(&" : "(|");

            if(!string.IsNullOrWhiteSpace(loginId))
            {
                searchFilter.AppendFormat("({0}={1})", STR_UID, loginId);
            }

            if(!string.IsNullOrWhiteSpace(email))
            {
                searchFilter.AppendFormat("({0}={1})", STR_Mail, email);
            }

            if (!string.IsNullOrWhiteSpace(employeeId))
            {
                searchFilter.AppendFormat("({0}={1})", STR_EmployeeNumber, employeeId);
            }

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                searchFilter.AppendFormat("({0}={1})", STR_GivenName, firstName);
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                searchFilter.AppendFormat("({0}={1})", STR_SN, lastName);
            }

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                searchFilter.AppendFormat("({0}~={1})", STR_DisplayName, fullName);
            }

            searchFilter.Append(")");

            string strSearchFilter = searchFilter.ToString();
            //"(&(uid=" + (loginID ?? string.Empty) + ")(sn=" + (lastName ?? "Kirkland") + "))";
            string strSearchBase = STR_SearchBase;

            SearchResponse sResponse = GetSearchResponse(strSearchFilter, strSearchBase);

            return GetUsersFromResponse(sResponse);
        }

        /// <summary>
        /// Builds the ldap search filter and then gets out the first returned user
        /// </summary>
        /// <param name="searchTerm">Either UID (kerb) or Mail</param>
        public DirectoryUser LDAPFindUser(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return null;

            var searchFilter = new StringBuilder("(|");

            //Append the login search
            searchFilter.AppendFormat("({0}={1})", STR_UID, searchTerm);

            //Append the email search
            searchFilter.AppendFormat("({0}={1})", STR_Mail, searchTerm);

            searchFilter.Append(")");

            SearchResponse sResponse = GetSearchResponse(searchFilter.ToString(), STR_SearchBase);

            List<DirectoryUser> foundUsers = GetUsersFromResponse(sResponse);

            if (foundUsers.Count == 0)
            {
                return null;
            }
            else
            {
                return foundUsers.First(); //Get the first returned user
            }
        }

        /// <summary>
        /// Builds a ldap search for student PIDM and then gets out the first returned user
        /// </summary>
        public DirectoryUser LDAPFindStudent(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId)) return null;

            var searchFilter = string.Format("(&({0}={1}))", STR_StudentId, studentId);

            SearchResponse sResponse = GetSearchResponse(searchFilter, STR_SearchBase);

            List<DirectoryUser> foundUsers = GetUsersFromResponse(sResponse);

            if (foundUsers.Count == 0)
            {
                return null;
            }
            else
            {
                return foundUsers.First(); //Get the first returned user
            }
        }

        /// <summary>
        /// Prepare the search against all fields
        /// </summary>
        public List<DirectoryUser> SearchUsers(string employeeID, string firstName, string lastName, string fullName,
                                                      string loginID, string email)
        {
            return LDAPSearchUsers(employeeID, firstName, lastName, fullName, loginID, email);
        }

        /// <summary>
        /// Search against email, login and fullname
        /// </summary>
        public List<DirectoryUser> SearchUsers(string searchTerm)
        {
            return LDAPSearchUsers(fullName: searchTerm, loginId: searchTerm, email: searchTerm, useAnd: false);
        }

        /// <summary>
        /// Returns the single user that matches the search term -- either loginID or email
        /// </summary>
        public DirectoryUser FindUser(string searchTerm)
        {
            return LDAPFindUser(searchTerm);
        }

        public DirectoryUser FindStudent(string studentId)
        {
            return LDAPFindStudent(studentId);
        }
    }

    public class DirectoryUser
    {
        public string EmployeeId { get; set; }
        public string LoginId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
    }
}