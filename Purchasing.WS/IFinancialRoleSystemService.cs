using System.Collections.Generic;

namespace Purchasing.WS
{
    public interface IFinancialRoleSystemService
    {
        /// <summary>
        /// Gets basic account information for multiple accounts
        /// </summary>
        /// <param name="accounts"></param>
        /// <returns></returns>
        List<AccountInfo> GetAccountInfos(List<AccountInfo> accounts);

        /// <summary>
        /// Gets basic account information for a single account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        AccountInfo GetAccountInfo(AccountInfo account);

        /// <summary>
        /// Is given user the fiscal officer?
        /// </summary>
        /// <param name="account"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool IsFiscalOfficer(AccountInfo account, string userId);
    }
}
