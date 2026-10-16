using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Purchasing.Core.Domain;
using Purchasing.Tests.Support;

namespace Purchasing.Tests.ControllerTests
{
    [TestClass]
    [DoNotParallelize]
    public class AwayStatusWebTests
    {
        private AwayStatusWebHost _site;
        private HttpClient _client;

        [TestInitialize]
        public async Task StartHost()
        {
            _site = new AwayStatusWebHost();
            await _site.StartAsync(useDevServer: true);
            _client = _site.Host.GetTestClient();
        }

        [TestCleanup]
        public void StopHost()
        {
            _client?.Dispose();
            _site?.Dispose();
        }

        private async Task<string> GetFormToken()
        {
            using var response = await _client.GetAsync("/User/AwayStatus");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, await response.Content.ReadAsStringAsync());
            _client.DefaultRequestHeaders.Remove("Cookie");
            _client.DefaultRequestHeaders.Add("Cookie", string.Join("; ", response.Headers.GetValues("Set-Cookie").Select(cookie => cookie.Split(';')[0])));
            var html = await response.Content.ReadAsStringAsync();
            var token = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
            Assert.IsTrue(token.Success, "The real Razor form must emit an antiforgery token.");
            return WebUtility.HtmlDecode(token.Groups[1].Value);
        }

        [TestMethod]
        public async Task AwayPageUsesModernAssetsAndRoleAwareNavigationWhileProfileRetainsLegacyAssets()
        {
            using var away = await _client.GetAsync("/User/AwayStatus?id=other-test");
            var html = await away.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.OK, away.StatusCode, html);
            StringAssert.Contains(html, "http://localhost:5178/src/styles.css");
            StringAssert.Contains(html, "name=\"AwayUntil\"");
            StringAssert.Contains(html, "type=\"date\"");
            StringAssert.Contains(html, "Place Order");
            StringAssert.Contains(html, "Order History");
            StringAssert.Contains(html, "Test service announcement");
            Assert.IsFalse(html.Contains("Administrators"));
            Assert.IsFalse(html.Contains("jqueryui"));

            _site.Roles.Add(Role.Codes.Admin);
            using var admin = await _client.GetAsync("/User/AwayStatus");
            StringAssert.Contains(await admin.Content.ReadAsStringAsync(), "Administrators");

            using var profile = await _client.GetAsync("/User/Profile");
            var profileHtml = await profile.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.OK, profile.StatusCode, profileHtml);
            Assert.IsFalse(profileHtml.Contains("/modern/"));
            StringAssert.Contains(profileHtml, "jquery");
        }

        [TestMethod]
        public async Task InvalidBoundDateRedisplaysErrorsAndNeverWritesTheUser()
        {
            _site.CurrentUser.IsAway = true;
            _site.CurrentUser.AwayUntil = new DateTime(2030, 6, 12);
            var token = await GetFormToken();

            using var response = await _client.PostAsync("/User/AwayStatus", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["AwayUntil"] = "not-a-date", ["IsAway"] = "false", ["__RequestVerificationToken"] = token
            }));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var html = await response.Content.ReadAsStringAsync();
            StringAssert.Contains(html, "not-a-date");
            StringAssert.Contains(html, "Return");
            Assert.AreEqual(new DateTime(2030, 6, 12), _site.CurrentUser.AwayUntil);
            Assert.IsTrue(_site.CurrentUser.IsAway);
            _site.Users.Verify(x => x.EnsurePersistent(It.IsAny<User>()), Times.Never);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("2000-01-02")]
        [DataRow("2030-06-12")]
        public async Task OrdinaryRazorPostOnlySavesCurrentUsersDateAndRedirects(string date)
        {
            _site.CurrentUser.AwayUntil = new DateTime(2030, 1, 1);
            var token = await GetFormToken();

            using var response = await _client.PostAsync("/User/AwayStatus?id=other-test", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["AwayUntil"] = date, ["Id"] = "other-test", ["Email"] = "changed@example.test",
                ["FirstName"] = "Changed", ["__RequestVerificationToken"] = token
            }));

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode, await response.Content.ReadAsStringAsync());
            Assert.AreEqual("/User/Profile", response.Headers.Location.OriginalString);
            Assert.AreEqual(date == "" ? (DateTime?)null : DateTime.Parse(date), _site.CurrentUser.AwayUntil);
            Assert.AreEqual("away@example.test", _site.CurrentUser.Email);
            Assert.AreEqual("Away", _site.CurrentUser.FirstName);
            Assert.IsNull(_site.OtherUser.AwayUntil);
            _site.Users.Verify(x => x.EnsurePersistent(_site.CurrentUser), Times.Once);
        }

        [TestMethod]
        public async Task PostRequiresAntiforgeryAndPageRequiresAuthentication()
        {
            using var post = await _client.PostAsync("/User/AwayStatus", new FormUrlEncodedContent(new Dictionary<string, string> { ["AwayUntil"] = "2030-06-12" }));
            Assert.AreEqual(HttpStatusCode.BadRequest, post.StatusCode);
            _site.Users.Verify(x => x.EnsurePersistent(It.IsAny<User>()), Times.Never);

            _client.DefaultRequestHeaders.Add("X-Test-Anonymous", "true");
            using var anonymous = await _client.GetAsync("/User/AwayStatus");
            Assert.AreEqual(HttpStatusCode.Unauthorized, anonymous.StatusCode);
        }
    }
}
