using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purchasing.WS
{
    public interface IFinancialRoleSystemService
    {
        List<AccountInfo> GetAccountInfos(List<AccountInfo> accounts);

        AccountInfo GetAccountInfo(AccountInfo account);

        bool IsFiscalOfficer(AccountInfo account, string userId);
    }
}
