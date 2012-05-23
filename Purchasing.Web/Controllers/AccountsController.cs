using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web.Attributes;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.ActionResults;
using Purchasing.Core.Repositories;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Accounts class
    /// </summary>
    [AuthorizeApplicationAccess]
    public class AccountsController : ApplicationController
    {
        private readonly IRepositoryWithTypedId<SubAccount, Guid> _subAccountRepository;
        private readonly ISearchRepository _searchRepository;

        public AccountsController(IRepositoryWithTypedId<SubAccount, Guid> subAccountRepository, ISearchRepository searchRepository)
        {
            _subAccountRepository = subAccountRepository;
            _searchRepository = searchRepository;
        }

        /// <summary>
        /// Ajax call to search for any kfs account, match by id or name
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public JsonNetResult SearchKfsAccounts(string searchTerm)
        {
            var results = _searchRepository.SearchAccounts(searchTerm).Select(a => new {a.Id, Name = string.Format("{0} ({1})",a.Name, a.Id)}).ToList();
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
