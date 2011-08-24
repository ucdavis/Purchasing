using System.Web.Mvc;
using Purchasing.Web.Services;
using UCDArch.Web.ActionResults;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the DirectorySearch class
    /// </summary>
    public class DirectorySearchController : ApplicationController
    {
        private readonly IDirectorySearchService _directorySearchService;


        public DirectorySearchController(IDirectorySearchService directorySearchService)
        {
            _directorySearchService = directorySearchService;
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
    }
}
