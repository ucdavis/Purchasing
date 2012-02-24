using Purchasing.WS.FinancialRoleService;

namespace Purchasing.WS
{
    public class AccountInfo
    {
        public AccountInfo(string chart, string number)
        {
            Chart = chart;
            Number = number;
        }

        public AccountInfo(simpleAccountInfo simpleAccountInfo)
        {
            Chart = simpleAccountInfo.chartOfAccountsCode;
            Number = simpleAccountInfo.accountNumber;

            Name = simpleAccountInfo.accountName;
            FiscalOfficerPrincipalId = simpleAccountInfo.fiscalOfficerPrincipalId;
            FiscalOfficerPrincipalUserId = simpleAccountInfo.fiscalOfficerPrincipalName;
        }

        public string Chart { get; set; }
        public string Number { get; set; }

        public string Name { get; set; }
        public string FiscalOfficerPrincipalId { get; set; }
        public string FiscalOfficerPrincipalUserId { get; set; }
    }
}