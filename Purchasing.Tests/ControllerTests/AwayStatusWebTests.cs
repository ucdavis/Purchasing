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

        private void ConfigureUi(bool enabled, string login = "away-test")
        {
            _site.Configuration["ModernUi:Enabled"] = enabled.ToString();
            _site.Configuration["ModernUi:AllowedLogins:0"] = login;
        }

        private async Task AssertAwayInterface(bool modern, string login = "away-test")
        {
            _client.DefaultRequestHeaders.Remove("X-Test-User");
            _client.DefaultRequestHeaders.Add("X-Test-User", login);
            using var response = await _client.GetAsync("/User/AwayStatus");
            var html = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, html);
            AssertInterface(html, modern);
        }

        private static void AssertInterface(string html, bool modern)
        {
            if (modern)
            {
                StringAssert.Contains(html, "http://localhost:5178/src/styles.css");
                StringAssert.Contains(html, "http://localhost:5178/src/pages/away-status.ts");
                StringAssert.Contains(html, "type=\"date\"");
                Assert.IsFalse(html.Contains("jqueryui"));
            }
            else
            {
                Assert.IsFalse(html.Contains("http://localhost:5178"));
                StringAssert.Contains(html, "jquery");
                StringAssert.Contains(html, "class=\"datepicker\"");
            }
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

        [DataTestMethod]
        [DataRow(null, null, false)]
        [DataRow("false", "away-test", false)]
        [DataRow("true", null, false)]
        [DataRow("true", "someone-else", false)]
        [DataRow("true", "AWAY-TEST", true)]
        public async Task InterfaceRequiresBothGlobalEnableAndMatchingLogin(string enabled, string allowedLogin, bool modern)
        {
            if (enabled != null) _site.Configuration["ModernUi:Enabled"] = enabled;
            if (allowedLogin != null) _site.Configuration["ModernUi:AllowedLogins:0"] = allowedLogin;

            await AssertAwayInterface(modern);
        }

        [TestMethod]
        public async Task CachedViewsStayIsolatedAcrossUsersAndConfigurationChanges()
        {
            ConfigureUi(true);
            await AssertAwayInterface(true);
            await AssertAwayInterface(false, "other-test");
            await AssertAwayInterface(true);

            ConfigureUi(false);
            await AssertAwayInterface(false);
            ConfigureUi(true);
            await AssertAwayInterface(true);

            ConfigureUi(true, "other-test");
            await AssertAwayInterface(false);
            await AssertAwayInterface(true, "other-test");
        }

        [TestMethod]
        public async Task AwayPageUsesModernAssetsAndRoleAwareNavigationWhileProfileRetainsLegacyAssets()
        {
            ConfigureUi(true);
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
            Assert.IsFalse(profileHtml.Contains("http://localhost:5178"));
            StringAssert.Contains(profileHtml, "jquery");
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task InvalidBoundDateRedisplaysErrorsAndNeverWritesTheUser(bool modern)
        {
            ConfigureUi(modern);
            _site.CurrentUser.IsAway = true;
            _site.CurrentUser.AwayUntil = new DateTime(2030, 6, 12);
            var token = await GetFormToken();

            using var response = await _client.PostAsync("/User/AwayStatus", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["AwayUntil"] = "not-a-date", ["IsAway"] = "false", ["__RequestVerificationToken"] = token
            }));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var html = await response.Content.ReadAsStringAsync();
            AssertInterface(html, modern);
            StringAssert.Contains(html, "not-a-date");
            StringAssert.Contains(html, "Return");
            Assert.AreEqual(new DateTime(2030, 6, 12), _site.CurrentUser.AwayUntil);
            Assert.IsTrue(_site.CurrentUser.IsAway);
            _site.Users.Verify(x => x.EnsurePersistent(It.IsAny<User>()), Times.Never);
        }

        [DataTestMethod]
        [DataRow(false, "")]
        [DataRow(false, "2000-01-02")]
        [DataRow(false, "2030-06-12")]
        [DataRow(true, "")]
        [DataRow(true, "2000-01-02")]
        [DataRow(true, "2030-06-12")]
        public async Task OrdinaryRazorPostOnlySavesCurrentUsersDateAndRedirects(bool modern, string date)
        {
            ConfigureUi(modern);
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

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task PostRequiresAntiforgeryAndPageRequiresAuthentication(bool modern)
        {
            ConfigureUi(modern);
            using var post = await _client.PostAsync("/User/AwayStatus", new FormUrlEncodedContent(new Dictionary<string, string> { ["AwayUntil"] = "2030-06-12" }));
            Assert.AreEqual(HttpStatusCode.BadRequest, post.StatusCode);
            _site.Users.Verify(x => x.EnsurePersistent(It.IsAny<User>()), Times.Never);

            _client.DefaultRequestHeaders.Add("X-Test-Anonymous", "true");
            using var anonymous = await _client.GetAsync("/User/AwayStatus");
            Assert.AreEqual(HttpStatusCode.Unauthorized, anonymous.StatusCode);
        }
    }
}
