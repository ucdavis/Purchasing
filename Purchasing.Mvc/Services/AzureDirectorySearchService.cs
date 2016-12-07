using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureActiveDirectorySearcher;
using Microsoft.Azure;

namespace Purchasing.Mvc.Services
{
    public class AzureDirectorySearchService : IDirectorySearchService
    {
        private readonly GraphSearchClient _searcher;

        public AzureDirectorySearchService()
        {
            _searcher =
                new GraphSearchClient(
                    new ActiveDirectoryConfigurationValues(
                        tenantName: CloudConfigurationManager.GetSetting("AzureSearchTenantName"),
                        tenantId: CloudConfigurationManager.GetSetting("AzureSearchTenantId"),
                        clientId: CloudConfigurationManager.GetSetting("AzureSearchClientId"),
                        clientSecret: CloudConfigurationManager.GetSetting("AzureSearchClientSecret")));
        }
        public List<DirectoryUser> SearchUsers(string searchTerm)
        {
            //TODO: make this interface async
            var users = Task.Run(() => _searcher.FindByEmailOrKerberos(searchTerm, searchTerm)).Result;

            return
                users.Select(
                    u =>
                        new DirectoryUser
                        {
                            EmailAddress = u.Mail,
                            FirstName = u.GivenName,
                            LastName = u.Surname,
                            FullName = u.DisplayName,
                            LoginId = u.Kerberos,
                            PhoneNumber = u.TelephoneNumber
                        }).ToList();
        }

        public DirectoryUser FindUser(string searchTerm)
        {
            var users = SearchUsers(searchTerm);

            return users.FirstOrDefault();
        }
    }
}