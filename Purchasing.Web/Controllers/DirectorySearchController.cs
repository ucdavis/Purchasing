﻿using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web.Services;
using Purchasing.Web.Utility;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the DirectorySearch class
    /// </summary>
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

            var users = _userRepository.Queryable.Where(a => a.Email == searchTerm || a.Id == searchTerm).ToList();
            if (users.Count == 0)
            {
                var ldapuser = _directorySearchService.FindUser(searchTerm);
                if (ldapuser != null)
                {
                    Check.Require(!string.IsNullOrWhiteSpace(ldapuser.LoginId));
                    Check.Require(!string.IsNullOrWhiteSpace(ldapuser.EmailAddress));

                    var user = new User(ldapuser.LoginId);
                    user.Email = ldapuser.EmailAddress;
                    user.FirstName = ldapuser.FirstName;
                    user.LastName = ldapuser.LastName;

                    users.Add(user);
                }
            }

            //We don't want to show users that are not active
            var results =
                users.Where(a => a.IsActive).Select(a => new IdAndName(a.Id, string.Format("{0} {1}", a.FirstName, a.LastName))).ToList();
            return new JsonNetResult(results.Select(a => new { Id = a.Id, Label = a.Name }));
        }
    }
}
