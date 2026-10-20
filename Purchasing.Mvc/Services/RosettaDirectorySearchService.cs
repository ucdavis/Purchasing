using System;
using System.Collections.Generic;
using System.Linq;
using UCD.Rosetta.Client.Generated;

namespace Purchasing.Mvc.Services
{
    public class RosettaDirectorySearchService : IDirectorySearchService
    {
        private readonly IClient _client;

        public RosettaDirectorySearchService(IClient client)
        {
            _client = client;
        }

        public List<DirectoryUser> SearchUsers(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<DirectoryUser>();
            }

            searchTerm = searchTerm.Trim();
            // Keep the synchronous directory contract used by the existing controllers.
            var people = (searchTerm.Contains("@")
                ? _client.PeopleGETAsync(email: searchTerm, affiliationState: AffiliationState.All)
                : _client.PeopleGETAsync(loginid: searchTerm, affiliationState: AffiliationState.All))
                .GetAwaiter().GetResult();

            if (people.Count == 0)
            {
                return new List<DirectoryUser>();
            }

            var identities = people.Select(person => new
            {
                LoginId = person.Id?.Login_id,
                IamId = AvailableValue(person.Iam_id) ?? person.Id?.Iam_id
            }).Distinct();
            if (identities.Skip(1).Any())
            {
                throw new InvalidOperationException("More than 1 unique kerb values found.");
            }

            var person = people.First();
            var campusEmail = AvailableValue(person.Email?.Campus);
            var healthEmail = AvailableValue(person.Email?.Health);
            return new List<DirectoryUser>
            {
                new DirectoryUser
                {
                    LoginId = person.Id?.Login_id,
                    FirstName = AvailableValue(person.Name?.Lived_first_name) ?? AvailableValue(person.Name?.Legal_first_name),
                    LastName = AvailableValue(person.Name?.Lived_last_name) ?? AvailableValue(person.Name?.Legal_last_name),
                    EmailAddress = string.Equals(searchTerm, healthEmail, StringComparison.OrdinalIgnoreCase)
                        ? healthEmail
                        : campusEmail ?? healthEmail,
                    PhoneNumber = person.Phone?.Work
                }
            };
        }

        public DirectoryUser FindUser(string searchTerm)
        {
            return SearchUsers(searchTerm).FirstOrDefault();
        }

        private static string AvailableValue(string value)
        {
            // Scope-protected fields can contain a mask instead of usable directory data.
            return string.IsNullOrWhiteSpace(value) || value.Trim().All(character => character == '*')
                ? null
                : value;
        }
    }
}
