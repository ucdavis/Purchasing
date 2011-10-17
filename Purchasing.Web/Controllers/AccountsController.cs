using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Accounts class
    /// </summary>
    public class AccountsController : ApplicationController
    {
	    private readonly IRepository<Account> _accountRepository;
        private readonly IRepositoryWithTypedId<SubAccount, Guid> _subAccountRepository;

        public AccountsController(IRepository<Account> accountRepository, IRepositoryWithTypedId<SubAccount, Guid> subAccountRepository)
        {
            _accountRepository = accountRepository;
            _subAccountRepository = subAccountRepository;
        }

        /// <summary>
        /// Ajax call to search for any kfs account, match by id or name
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public JsonNetResult SearchKfsAccounts(string searchTerm)
        {
            var results = Repository.OfType<Account>().Queryable.Where(a => a.IsActive && (a.Id.Contains(searchTerm) || a.Name.Contains(searchTerm))).Select(a => new { Id = a.Id, Name = a.Name }).ToList();
            return new JsonNetResult(results);
        }

        /// <summary>
        /// Ajax call to search for a kfs sub account for a given account
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public JsonNetResult SearchSubAccounts(string accountNumber)
        {
            var results = _subAccountRepository.Queryable.Where(a => a.AccountNumber == accountNumber).Select(a => new { Id = a.SubAccountNumber, Name = a.SubAccountNumber }).ToList();
            return new JsonNetResult(results);
        }
    }
}
