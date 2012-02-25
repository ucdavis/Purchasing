using System.Collections.Generic;
using System.ServiceModel;
using Purchasing.WS.FinancialRoleService;
using System.Linq;

namespace Purchasing.WS
{
    public class FinancialRoleSystemService : IFinancialRoleSystemService
    {
        // url for the testing webservice
        private string _url = "http://kfs-test.ucdavis.edu/kfs-stg/remoting/financialSystemRoleServiceSOAP";

        // url for the testing webservice, extra logging?
        //private string _url = "http://kfs-test1.ucdavis.edu/kfs-stg/remoting/financialSystemRoleServiceSOAP";

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

            return result.Select(a => new AccountInfo(a)).ToList();
        }

        public AccountInfo GetAccountInfo(AccountInfo account)
        {
            var client = InitializeClient();

            var result = client.getSimpleAccountInfo(account.Chart, account.Number);

            return new AccountInfo(result);
        }

        public bool IsFiscalOfficer(AccountInfo account, string userId)
        {
            var client = InitializeClient();

            var result = client.isUserFiscalOfficerForAccount(userId, account.Chart, account.Number);

            return result;
        }
    }
}