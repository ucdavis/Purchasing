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