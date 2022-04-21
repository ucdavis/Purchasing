using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureActiveDirectorySearcher;
using Microsoft.Azure;
using Microsoft.Extensions.Configuration;

namespace Purchasing.Mvc.Services
{
    public class AzureDirectorySearchService : IDirectorySearchService
    {
        private readonly GraphSearchClient _searcher;

        public AzureDirectorySearchService(IConfiguration configuration)
        {
            _searcher =
                new GraphSearchClient(
                    new ActiveDirectoryConfigurationValues(
                        tenantName: configuration["AzureSearchTenantName"],
                        tenantId: configuration["AzureSearchTenantId"],
                        clientId: configuration["AzureSearchClientId"],
                        clientSecret: configuration["AzureSearchClientSecret"]));
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