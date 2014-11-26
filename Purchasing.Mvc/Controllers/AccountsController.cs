using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Attributes;
using Purchasing.Web.Attributes;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.ActionResults;

namespace Purchasing.Mvc.Controllers
{
    /// <summary>
    /// Controller for the Accounts class
    /// </summary>
    [AuthorizeApplicationAccess]
    public class AccountsController : ApplicationController
    {
        private readonly IRepositoryWithTypedId<SubAccount, Guid> _subAccountRepository;
        private readonly ISearchService _searchService;

        public AccountsController(IRepositoryWithTypedId<SubAccount, Guid> subAccountRepository, ISearchService searchService)
        {
            _subAccountRepository = subAccountRepository;
            _searchService = searchService;
        }

        /// <summary>
        /// Ajax call to search for any kfs account, match by id or name
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public JsonNetResult SearchKfsAccounts(string searchTerm)
        {
            var results = _searchService.SearchAccounts(searchTerm).Select(a => new {a.Id, Name = string.Format("{0} ({1})",a.Name, a.Id)}).ToList();
            return new JsonNetResult(results);
        }

        /// <summary>
        /// Ajax call to search for a kfs sub account for a given account
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public JsonNetResult SearchSubAccounts(string accountNumber)
        {
            var results = _subAccountRepository.Queryable.Where(a => a.AccountNumber == accountNumber && a.IsActive).Select(a => new { Id = a.SubAccountNumber, Name = a.SubAccountNumber, Title = a.Name }).ToList();
            return new JsonNetResult(results);
        }
    }
}
