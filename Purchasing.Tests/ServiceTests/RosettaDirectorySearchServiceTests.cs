using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using CommonServiceLocator;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Mvc;
using Purchasing.Mvc.Services;
using UCDArch.Data.NHibernate;
using UCDArch.Data.NHibernate.Mapping;
using UCD.Rosetta.Client.Core;
using UCD.Rosetta.Client.Core.Configuration;
using UCD.Rosetta.Client.Generated;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    [DoNotParallelize]
    public class RosettaDirectorySearchServiceTests
    {
        [TestMethod]
        public void LoginLookupUsesExactLoginAndMapsDirectoryFields()
        {
            using var handler = new DirectoryResponseHandler(CreatePerson());
            using var httpClient = new HttpClient(handler);
            var service = CreateService(httpClient);

            var user = service.SearchUsers(" testuser ").Single();

            Assert.AreEqual("/api/v1/people", handler.RequestUri.AbsolutePath);
            var query = QueryHelpers.ParseQuery(handler.RequestUri.Query);
            Assert.AreEqual("testuser", query["loginid"].ToString());
            Assert.AreEqual("all", query["affiliationState"].ToString());
            Assert.IsFalse(query.ContainsKey("email"));
            Assert.AreEqual("testuser", user.LoginId);
            Assert.AreEqual("Lived", user.FirstName);
            Assert.AreEqual("Name", user.LastName);
            Assert.AreEqual("testuser@ucdavis.edu", user.EmailAddress);
            Assert.AreEqual("530-555-0100", user.PhoneNumber);
            Assert.IsNull(user.EmployeeId);
            Assert.IsNull(user.FullName);
        }

        [TestMethod]
        public void HealthEmailLookupPreservesMatchingAddressAndEncodesQuery()
        {
            var person = CreatePerson();
            person.Email.Health = "test+alias@health.ucdavis.edu";
            using var handler = new DirectoryResponseHandler(person);
            using var httpClient = new HttpClient(handler);
            var service = CreateService(httpClient);

            var user = service.FindUser(" TEST+ALIAS@health.ucdavis.edu ");

            var query = QueryHelpers.ParseQuery(handler.RequestUri.Query);
            Assert.AreEqual("TEST+ALIAS@health.ucdavis.edu", query["email"].ToString());
            Assert.AreEqual("all", query["affiliationState"].ToString());
            Assert.IsFalse(query.ContainsKey("loginid"));
            Assert.AreEqual("test+alias@health.ucdavis.edu", user.EmailAddress);
        }

        [TestMethod]
        public void MissingLivedNamesAndCampusEmailUseAvailableFallbacks()
        {
            var person = CreatePerson();
            person.Name.Lived_first_name = " ";
            person.Name.Lived_last_name = null;
            person.Email.Campus = " ";
            using var handler = new DirectoryResponseHandler(person);
            using var httpClient = new HttpClient(handler);

            var user = CreateService(httpClient).FindUser("testuser");

            Assert.AreEqual("Legal", user.FirstName);
            Assert.AreEqual("Surname", user.LastName);
            Assert.AreEqual("testuser@health.ucdavis.edu", user.EmailAddress);
        }

        [TestMethod]
        public void MaskedFieldsAreNotUsedAsNamesOrEmail()
        {
            var person = CreatePerson();
            person.Name.Lived_first_name = null;
            person.Name.Lived_last_name = null;
            person.Name.Legal_first_name = " *** ";
            person.Name.Legal_last_name = "********";
            person.Email.Campus = "***";
            person.Email.Health = " *** ";
            using var handler = new DirectoryResponseHandler(person);
            using var httpClient = new HttpClient(handler);

            var user = CreateService(httpClient).FindUser("testuser");

            Assert.IsNull(user.FirstName);
            Assert.IsNull(user.LastName);
            Assert.IsNull(user.EmailAddress);
        }

        [TestMethod]
        public void MissingOptionalContactObjectsDoNotThrow()
        {
            var person = CreatePerson();
            person.Name = null;
            person.Email = null;
            person.Phone = null;
            using var handler = new DirectoryResponseHandler(person);
            using var httpClient = new HttpClient(handler);

            var user = CreateService(httpClient).FindUser("testuser");

            Assert.AreEqual("testuser", user.LoginId);
            Assert.IsNull(user.FirstName);
            Assert.IsNull(user.LastName);
            Assert.IsNull(user.EmailAddress);
            Assert.IsNull(user.PhoneNumber);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" \t ")]
        public void BlankSearchDoesNotRequestTheDirectory(string searchTerm)
        {
            using var handler = new DirectoryResponseHandler(CreatePerson());
            using var httpClient = new HttpClient(handler);
            var service = CreateService(httpClient);

            Assert.AreEqual(0, service.SearchUsers(searchTerm).Count);
            Assert.IsNull(service.FindUser(searchTerm));
            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public void NoMatchReturnsEmptySearchAndNullUser()
        {
            using var handler = new DirectoryResponseHandler();
            using var httpClient = new HttpClient(handler);
            var service = CreateService(httpClient);

            Assert.AreEqual(0, service.SearchUsers("unknown").Count);
            Assert.IsNull(service.FindUser("unknown"));
        }

        [TestMethod]
        public void DuplicateIdentityUsesNestedIamIdWhenTopLevelIdIsMissing()
        {
            var first = CreatePerson();
            var duplicate = CreatePerson();
            duplicate.Iam_id = null;
            using var handler = new DirectoryResponseHandler(first, duplicate);
            using var httpClient = new HttpClient(handler);

            var users = CreateService(httpClient).SearchUsers("testuser");

            Assert.AreEqual(1, users.Count);
            Assert.AreEqual("testuser", users[0].LoginId);
        }

        [DataTestMethod]
        [DataRow("otheruser", "0000000001")]
        [DataRow("testuser", "0000000002")]
        public void DifferentLoginOrIamIdRejectsAmbiguousResults(string loginId, string iamId)
        {
            var differentPerson = CreatePerson();
            differentPerson.Id.Login_id = loginId;
            differentPerson.Iam_id = iamId;
            differentPerson.Id.Iam_id = iamId;
            using var handler = new DirectoryResponseHandler(CreatePerson(), differentPerson);
            using var httpClient = new HttpClient(handler);

            Assert.ThrowsException<InvalidOperationException>(
                () => CreateService(httpClient).FindUser("testuser"));
        }

        [TestMethod]
        public void ApiFailureIsNotReportedAsNoMatch()
        {
            using var handler = new DirectoryResponseHandler
            {
                StatusCode = HttpStatusCode.ServiceUnavailable
            };
            using var httpClient = new HttpClient(handler);

            var exception = Assert.ThrowsException<RosettaApiException>(
                () => CreateService(httpClient).FindUser("testuser"));

            Assert.AreEqual(503, exception.StatusCode);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("false")]
        public void MissingOrDisabledFlagResolvesIetWithoutRosettaConfiguration(string enabled)
        {
            var settings = new Dictionary<string, string> { ["IetWsKey"] = "unused-test-key" };
            if (enabled != null)
            {
                settings["RosettaDirectorySearch:Enabled"] = enabled;
            }
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
            var services = ConfigureServices(configuration);
            Assert.IsFalse(services.Any(descriptor =>
                descriptor.ServiceType == typeof(RosettaClient) || descriptor.ServiceType == typeof(IClient)));
            using var container = CreateContainer(configuration, services);

            Assert.IsInstanceOfType(container.Resolve<IDirectorySearchService>(), typeof(IetWsDirectorySearchService));
        }

        [TestMethod]
        public void EnabledFlagBindsRosettaSettingsAndResolvesRosettaThroughWindsor()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    ["RosettaDirectorySearch:Enabled"] = "true",
                    ["RosettaClient:BaseUrl"] = "https://rosetta.example.test/api/v1",
                    ["RosettaClient:TokenUrl"] = "https://oauth.example.test/token",
                    ["RosettaClient:ClientId"] = "test-client",
                    ["RosettaClient:ClientSecret"] = "unused-test-secret",
                    ["RosettaClient:Scope"] = "read:public read:healthEmail"
                }).Build();
            using var container = CreateContainer(configuration, ConfigureServices(configuration));

            Assert.IsInstanceOfType(container.Resolve<IDirectorySearchService>(), typeof(RosettaDirectorySearchService));
            var options = container.Resolve<IOptions<RosettaClientOptions>>().Value;
            Assert.AreEqual("https://rosetta.example.test/api/v1", options.BaseUrl);
            Assert.AreEqual("https://oauth.example.test/token", options.TokenUrl);
            Assert.AreEqual("test-client", options.ClientId);
            Assert.AreEqual("unused-test-secret", options.ClientSecret);
            Assert.AreEqual("read:public read:healthEmail", options.Scope);
            Assert.AreSame(container.Resolve<RosettaClient>().Api, container.Resolve<IClient>());
        }

        private static RosettaDirectorySearchService CreateService(HttpClient httpClient)
        {
            return new RosettaDirectorySearchService(new Client(httpClient)
            {
                BaseUrl = "https://rosetta.example.test/api/v1"
            });
        }

        private static IServiceCollection ConfigureServices(IConfiguration configuration)
        {
            // Startup configures NHibernate mappings but these tests never open a session.
            configuration["ConnectionStrings:MainDB"] = "Server=localhost;Database=Unused;Integrated Security=true";
            configuration["MainDB:Schema"] = "dbo";
            var services = new ServiceCollection();
            services.AddSingleton(configuration);
            new Startup(configuration).ConfigureServices(services);
            return services;
        }

        private static IWindsorContainer CreateContainer(IConfiguration configuration, IServiceCollection services)
        {
            var factory = new WindsorServiceProviderFactory();
            var container = factory.CreateBuilder(services);
            var previousLocator = ServiceLocator.IsLocationProviderSet ? ServiceLocator.Current : null;
            var mappingField = typeof(NHibernateSessionConfiguration)
                .GetField("_mappingConfiguration", BindingFlags.Static | BindingFlags.NonPublic);
            var previousMapping = (IMappingConfiguration)mappingField.GetValue(null);
            try
            {
                new Startup(configuration).ConfigureContainer(container);
                factory.CreateServiceProvider(container);
                return container;
            }
            catch
            {
                container.Dispose();
                throw;
            }
            finally
            {
                NHibernateSessionConfiguration.Mappings.UseCustomMapping(previousMapping);
                ServiceLocator.SetLocatorProvider(previousLocator == null ? null : () => previousLocator);
            }
        }

        private static Person CreatePerson()
        {
            return new Person
            {
                Iam_id = "0000000001",
                Displayname = "Lived Name",
                Id = new Id { Iam_id = "0000000001", Login_id = "testuser", Employee_id = "123456" },
                Name = new Name
                {
                    Lived_first_name = "Lived",
                    Lived_last_name = "Name",
                    Legal_first_name = "Legal",
                    Legal_last_name = "Surname"
                },
                Email = new Email
                {
                    Campus = "testuser@ucdavis.edu",
                    Health = "testuser@health.ucdavis.edu"
                },
                Phone = new Phone { Work = "530-555-0100", Primary = "530-555-0101" }
            };
        }

        private sealed class DirectoryResponseHandler : HttpMessageHandler
        {
            private readonly string _response;

            public DirectoryResponseHandler(params Person[] people)
            {
                _response = JsonSerializer.Serialize(people);
            }

            public Uri RequestUri { get; private set; }
            public int RequestCount { get; private set; }
            public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                RequestUri = request.RequestUri;
                RequestCount++;
                return Task.FromResult(new HttpResponseMessage(StatusCode)
                {
                    Content = new StringContent(_response, Encoding.UTF8, "application/json")
                });
            }
        }
    }
}
