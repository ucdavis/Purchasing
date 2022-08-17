using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using Purchasing.WS.FinancialRoleService;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Purchasing.WS
{
    public class FinancialRoleSystemService : IFinancialRoleSystemService
    {
        private readonly string _url;
        private readonly string _token;

        public FinancialRoleSystemService(IConfiguration configuration)
        {
            this._url = configuration["AfsRoleUrl"];
            this._token = configuration["AfsToken"];
        }

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