using System.Collections.Generic;
using System.ServiceModel;
using Purchasing.WS.FinancialRoleService;
using System.Linq;

namespace Purchasing.WS
{
    public class FinancialRoleSystemService : IFinancialRoleSystemService
    {
        private string _url = "http://kfs-test.ucdavis.edu/kfs-stg/remoting/financialSystemRoleServiceSOAP";

        private financialSystemRoleServiceSOAPClient InitializeClient()
        {
            var binding = new BasicHttpBinding();
            var endpointAddress = new EndpointAddress(_url);

            var client = new financialSystemRoleServiceSOAPClient(binding, endpointAddress);

            return client;
        }


        public List<AccountInfo> GetAccountInfos(List<AccountInfo> accounts)
        {
            var client = InitializeClient();

            var accts = accounts.Select(a => new accountKey() {chartOfAccountsCode = a.Chart, accountNumber = a.Number}).ToArray();

            var result = client.getSimpleAccountInfos(accts);

            return result.Select(a => new AccountInfo() { Chart = a.chartOfAccountsCode, Number = a.accountNumber, Name = a.accountName, FiscalOfficerPrincipalId = a.fiscalOfficerPrincipalId, FiscalOfficerPrincipalUserId = a.fiscalOfficerPrincipalName }).ToList();
        }

        public AccountInfo GetAccountInfo(AccountInfo account)
        {
            var client = InitializeClient();

            var result = client.getSimpleAccountInfo(account.Chart, account.Number);

            return new AccountInfo() { Chart = result.chartOfAccountsCode, Number = result.accountNumber, Name = result.accountName, FiscalOfficerPrincipalId = result.fiscalOfficerPrincipalId, FiscalOfficerPrincipalUserId = result.fiscalOfficerPrincipalName };
        }

        public bool IsFiscalOfficer(AccountInfo account, string userId)
        {
            var client = InitializeClient();

            var result = client.isUserFiscalOfficerForAccount(userId, account.Chart, account.Number);

            return result;
        }
    }
}