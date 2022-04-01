using System;
using System.DirectoryServices.Protocols;
using System.Linq;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Services;
using Purchasing.Mvc.Utility;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using IdAndName = Purchasing.Core.Services.IdAndName;
using Microsoft.AspNetCore.Mvc;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// Controller for the DirectorySearch class
    /// </summary>
    [AuthorizeApplicationAccess]
    public class DirectorySearchController : ApplicationController
    {
        private readonly IDirectorySearchService _directorySearchService;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;


        public DirectorySearchController(IDirectorySearchService directorySearchService, IRepositoryWithTypedId<User, string> userRepository  )
        {
            _directorySearchService = directorySearchService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Returns LDAP person or null, given either email or login id for the user 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonNetResult FindPerson(string loginOrEmail)
        {
            var user = _directorySearchService.FindUser(loginOrEmail);

            return new JsonNetResult(user);
        }

        /// <summary>
        /// People #6
        /// Search Users in the User Table and LDAP lookup if none found.
        /// </summary>
        /// <param name="searchTerm">Email or LoginId</param>
        /// <returns></returns>
        public JsonNetResult SearchUsers(string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Trim();

            var users = _userRepository.Queryable.Where(a => a.Email.ToLower() == searchTerm || a.Id.ToLower() == searchTerm).ToList();
            if (users.Count == 0)
            {
                try
                {
                    var ldapuser = _directorySearchService.FindUser(searchTerm);
                    if (ldapuser != null)
                    {
                        Check.Require(!string.IsNullOrWhiteSpace(ldapuser.LoginId));
                        Check.Require(!string.IsNullOrWhiteSpace(ldapuser.EmailAddress));

                        var user = new User(ldapuser.LoginId)
                        {
                            Email = ldapuser.EmailAddress,
                            FirstName = ldapuser.FirstName,
                            LastName = ldapuser.LastName
                        };

                        users.Add(user);
                    }
                }
                catch (DirectoryOperationException) { 
                    //Don't add any ldap users if the LDAP service has a problem like too large a result set 
                }
            }

            //We don't want to show users that are not active
            var results =
                users.Where(a => a.IsActive).Select(a => new IdAndName(a.Id, string.Format("{0} {1}", a.FirstName, a.LastName))).ToList();
            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.Name }));
        }
    }
}
