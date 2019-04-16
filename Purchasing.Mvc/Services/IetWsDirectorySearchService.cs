using Ietws;
using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace Purchasing.Mvc.Services
{
    public class IetWsDirectorySearchService : IDirectorySearchService
    {
        private readonly IetClient ietClient;

        public IetWsDirectorySearchService()
        {
            ietClient = new IetClient(CloudConfigurationManager.GetSetting("IetWsKey"));

        }

        public List<DirectoryUser> SearchUsers(string searchTerm)
        {
            var rtValue = new List<DirectoryUser>();
            if (searchTerm != null && searchTerm.Contains("@"))
            {
                // find the contact via their email
                var person = Task.Run(() => GetByEmail(searchTerm)).Result;
                rtValue.Add(new DirectoryUser()
                {
                    EmailAddress = person.Mail,
                    LoginId = person.Kerberos,
                    LastName = person.Surname,
                    FirstName = person.GivenName,
                    PhoneNumber = person.WorkPhone
                });
            }
            else
            {
                // find the contact by their kerb
                var person = Task.Run(() => GetByKerb(searchTerm)).Result;
                rtValue.Add(new DirectoryUser()
                {
                    EmailAddress = person.Mail,
                    LoginId = person.Kerberos,
                    LastName = person.Surname,
                    FirstName = person.GivenName,
                    PhoneNumber = person.WorkPhone
                });
            }

            return rtValue;
        }

        private async Task<Person> GetByEmail(string email)
        {
            // find the contact via their email
            var ucdContactResult = await ietClient.Contacts.Search(ContactSearchField.email, email);
            EnsureResponseSuccess(ucdContactResult);
            var ucdContact = ucdContactResult.ResponseData.Results.First();

            // now look up the whole person's record by ID including kerb
            var ucdKerbResult = await ietClient.Kerberos.Search(KerberosSearchField.iamId, ucdContact.IamId);
            EnsureResponseSuccess(ucdKerbResult);
            var allTheSame = ucdKerbResult.ResponseData.Results.Select(a => new { a.UserId, a.IamId }).Distinct().ToList();
            if (allTheSame.Count > 1)
            {
                throw new Exception("More than 1 unique kerb values found.");
            }
            var ucdKerbPerson = ucdKerbResult.ResponseData.Results.First();
            return GetPersonDetails(ucdKerbPerson, ucdContact);
        }

        private Person GetPersonDetails(KerberosResult ucdKerbPerson, ContactResult ucdContact)
        {
            return new Person

            {
                GivenName = string.IsNullOrWhiteSpace(ucdKerbPerson.DFirstName) ? ucdKerbPerson.OFirstName : ucdKerbPerson.DFirstName,
                Surname = string.IsNullOrWhiteSpace(ucdKerbPerson.DLastName) ? ucdKerbPerson.OLastName : ucdKerbPerson.DLastName,
                FullName = string.IsNullOrWhiteSpace(ucdKerbPerson.DFullName) ? ucdKerbPerson.OFullName : ucdKerbPerson.DFullName,
                Kerberos = ucdKerbPerson.UserId,
                Mail = ucdContact.Email,
                WorkPhone = ucdContact.WorkPhone
            };
        }

        private async Task<Person> GetByKerb(string kerbId)
        {
            var ucdKerbResult = await ietClient.Kerberos.Search(KerberosSearchField.userId, kerbId);
            EnsureResponseSuccess(ucdKerbResult);

            var allTheSame = ucdKerbResult.ResponseData.Results.Select(a => new { a.UserId, a.IamId }).Distinct().ToList();
            if (allTheSame.Count > 1)
            {
                throw new Exception("More than 1 unique kerb values found.");
            }

            var ucdKerbPerson = ucdKerbResult.ResponseData.Results.First();

            var ucdContactResult = await ietClient.Contacts.Search(ContactSearchField.iamId, ucdKerbPerson.IamId);
            EnsureResponseSuccess(ucdContactResult);
            var ucdContact = ucdContactResult.ResponseData.Results.First();
            return GetPersonDetails(ucdKerbPerson, ucdContact);
        }

        private void EnsureResponseSuccess<T>(IetResult<T> result)
        {

            if (result.ResponseStatus != 0)
            {
                throw new ApplicationException(result.ResponseDetails);
            }
        }

        public DirectoryUser FindUser(string searchTerm)
        {
            var users = SearchUsers(searchTerm);

            return users.FirstOrDefault();
        }

        public class Person
        {
            public string GivenName { get; internal set; }
            public string Surname { get; internal set; }
            public string Kerberos { get; internal set; }
            public string Mail { get; internal set; }
            public string FullName { get; internal set; }
            public string WorkPhone { get; internal set; }
        }

    }
}