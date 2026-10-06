using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Mvc;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    [DoNotParallelize]
    public class WebHostTests
    {
        private static Task<IHost> StartHost(string environment)
        {
            return Program.CreateHostBuilder(Array.Empty<string>(), addUserSecrets: false)
                .UseEnvironment(environment)
                .ConfigureAppConfiguration((_, configuration) =>
                {
                    // Exercise the real Startup/container without developer secrets or external services.
                    configuration.Sources.Clear();
                    configuration.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["CasUrl"] = "https://cas.example.test/cas",
                        ["ConnectionStrings:MainDB"] = "Server=127.0.0.1;Database=Unused;User ID=unused;Password=unused;Connect Timeout=1",
                        ["MainDB:Schema"] = "dbo",
                        ["ElasticApm:Enabled"] = "false",
                        ["LocalLogin:Enabled"] = "true"
                    });
                })
                .ConfigureWebHost(web => web
                    .UseSetting(WebHostDefaults.ApplicationKey, typeof(Startup).Assembly.GetName().Name)
                    .UseTestServer()
                    .ConfigureServices(services => services.AddDataProtection().UseEphemeralDataProtectionProvider()))
                .StartAsync();
        }

        [TestMethod]
        public async Task ProductionHostChallengesWithCasAndProtectsWorkgroups()
        {
            using var host = await StartHost(Environments.Production);
            using var client = host.GetTestClient();
            client.BaseAddress = new Uri("https://localhost");

            using var login = await client.GetAsync("/LogOn?returnUrl=/Home/Landing");
            Assert.AreEqual(HttpStatusCode.Redirect, login.StatusCode);
            Assert.AreEqual("cas.example.test", login.Headers.Location.Host);
            Assert.AreEqual("/cas/login", login.Headers.Location.AbsolutePath);
            Assert.IsTrue(login.Headers.Contains("X-Correlation-Id"));

            using var workgroups = await client.GetAsync("/Workgroup/Index");
            Assert.AreEqual(HttpStatusCode.Redirect, workgroups.StatusCode);
            Assert.AreEqual("/LogOn", workgroups.Headers.Location.AbsolutePath);
        }

        [TestMethod]
        public async Task DevelopmentHostRendersLoginAndRejectsPostWithoutAntiforgery()
        {
            using var host = await StartHost(Environments.Development);
            using var client = host.GetTestClient();
            client.BaseAddress = new Uri("https://localhost");

            using var login = await client.GetAsync("/LogOn?returnUrl=/Home/Landing");
            Assert.AreEqual(HttpStatusCode.OK, login.StatusCode);
            var html = await login.Content.ReadAsStringAsync();
            StringAssert.Contains(html, "name=\"__RequestVerificationToken\"");
            StringAssert.Contains(html, "action=\"/LogOn/Local\"");

            using var rejected = await client.PostAsync("/LogOn/Local",
                new System.Net.Http.FormUrlEncodedContent(new Dictionary<string, string> { ["UserId"] = "test" }));
            Assert.AreEqual(HttpStatusCode.BadRequest, rejected.StatusCode);
        }
    }
}
