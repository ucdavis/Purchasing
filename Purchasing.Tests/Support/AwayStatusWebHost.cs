using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CommonServiceLocator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Mvc;
using Purchasing.Mvc.App_GlobalResources;
using Purchasing.Mvc.Helpers;
using Purchasing.Mvc.Infrastructure.ModernUi;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Tests.Support
{
    // Test-only host: real controllers, views, filters and antiforgery, with no database or CAS connection.
    // Static service location mirrors the legacy application's filters; callers must not run in parallel.
    public sealed class AwayStatusWebHost : IDisposable
    {
        private readonly IServiceLocator _previousLocator;
        public User CurrentUser { get; } = new("away-test")
        {
            FirstName = "Away", LastName = "Tester", Email = "away@example.test", IsActive = true
        };
        public User OtherUser { get; } = new("other-test")
        {
            FirstName = "Other", LastName = "Tester", Email = "other@example.test", IsActive = true
        };
        public List<string> Roles { get; } = new() { Role.Codes.Requester };
        public Mock<IRepositoryWithTypedId<User, string>> Users { get; } = new(MockBehavior.Strict);
        public IHost Host { get; private set; }
        public IConfiguration Configuration => Host.Services.GetRequiredService<IConfiguration>();

        public AwayStatusWebHost()
        {
            _previousLocator = ServiceLocator.IsLocationProviderSet ? ServiceLocator.Current : null;
            Users.SetupGet(x => x.Queryable).Returns(new[] { CurrentUser, OtherUser }.AsQueryable());
            Users.Setup(x => x.EnsurePersistent(CurrentUser)).Callback(() =>
                CurrentUser.IsAway = CurrentUser.AwayUntil.HasValue && CurrentUser.AwayUntil.Value.Date > DateTime.Today);
        }

        public async Task StartAsync(string url = null, bool useDevServer = false, string contentRoot = null)
        {
            if (contentRoot == null)
            {
                var root = new DirectoryInfo(AppContext.BaseDirectory);
                while (root != null && !File.Exists(Path.Combine(root.FullName, "Purchasing.Mvc", "Purchasing.Mvc.csproj")))
                    root = root.Parent;
                if (root == null) throw new DirectoryNotFoundException("Cannot locate Purchasing.Mvc from test output.");
                contentRoot = Path.Combine(root.FullName, "Purchasing.Mvc");
            }

            var servicesMessages = new Mock<IRepository<ServiceMessage>>(MockBehavior.Strict);
            servicesMessages.SetupGet(x => x.Queryable).Returns(new[]
            {
                new ServiceMessage { Message = "Test service announcement", IsActive = true, BeginDisplayDate = DateTime.Today.AddDays(-1) }
            }.AsQueryable());

            Host = new HostBuilder().ConfigureWebHost(web =>
            {
                web.UseContentRoot(contentRoot)
                    .UseWebRoot("wwwroot")
                    .UseEnvironment(useDevServer ? Environments.Development : Environments.Production)
                    .UseSetting(WebHostDefaults.ApplicationKey, typeof(Startup).Assembly.GetName().Name)
                    .ConfigureAppConfiguration(configuration => configuration.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Vite:UseDevServer"] = useDevServer.ToString(),
                        ["Vite:DevServerUrl"] = "http://localhost:5178"
                    }))
                    .ConfigureServices(services =>
                    {
                        services.AddDataProtection().UseEphemeralDataProtectionProvider();
                        services.AddDistributedMemoryCache();
                        services.AddSession();
                        services.AddHttpContextAccessor();
                        services.AddAuthentication("Test").AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", _ => { });
                        services.AddControllersWithViews()
                            .AddApplicationPart(typeof(Startup).Assembly)
                            .AddRazorOptions(options => options.ViewLocationExpanders.Add(new ModernUiViewLocationExpander()));
                        services.AddSingleton(Users.Object);
                        services.AddSingleton(Mock.Of<IRepositoryWithTypedId<EmailPreferences, string>>());
                        services.AddSingleton(Mock.Of<IRepositoryWithTypedId<ColumnPreferences, string>>());
                        services.AddSingleton(Mock.Of<IRepositoryFactory>());
                        services.AddSingleton(Mock.Of<IQueryRepositoryFactory>());
                        services.AddSingleton(Mock.Of<IDirectorySearchService>());
                        services.AddSingleton(Mock.Of<ISecurityService>());
                        services.AddSingleton(Mock.Of<IDbContext>());
                        services.AddSingleton(servicesMessages.Object);
                    })
                    .Configure(app =>
                    {
                        var locator = new Mock<IServiceLocator>();
                        locator.Setup(x => x.GetService(It.IsAny<Type>())).Returns<Type>(type => app.ApplicationServices.GetRequiredService(type));
                        ServiceLocator.SetLocatorProvider(() => locator.Object);

                        app.UseDeveloperExceptionPage();
                        app.UseStaticFiles();
                        app.UseRouting();
                        app.UseSession();
                        app.Use(async (context, next) =>
                        {
                            var login = context.Request.Headers["X-Test-User"].FirstOrDefault() ?? CurrentUser.Id;
                            context.Session.SetObject(string.Format(Resources.Role_CacheId, login), Roles);
                            await next();
                        });
                        app.UseAuthentication();
                        app.UseAuthorization();
                        app.UseEndpoints(endpoints => endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}"));
                    });
                if (url == null) web.UseTestServer();
                else web.UseKestrel().UseUrls(url);
            }).Build();
            await Host.StartAsync();
        }

        public void Dispose()
        {
            Host?.Dispose();
            ServiceLocator.SetLocatorProvider(() => _previousLocator);
        }

        private sealed class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
                : base(options, logger, encoder) { }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                if (Request.Headers.ContainsKey("X-Test-Anonymous")) return Task.FromResult(AuthenticateResult.NoResult());
                var login = Request.Headers["X-Test-User"].FirstOrDefault() ?? "away-test";
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, login) }, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name)));
            }
        }
    }
}
