using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Security.CAS;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Tests.ControllerTests
{
    [TestClass]
    public class LocalLoginTests
    {
        private readonly Mock<IRepositoryWithTypedId<User, string>> _users = new(MockBehavior.Strict);
        private readonly Mock<IAuthenticationService> _authentication = new(MockBehavior.Strict);

        private AccountController CreateController(string environment = "Development", string enabled = "true")
        {
            var host = new Mock<IWebHostEnvironment>();
            host.SetupGet(x => x.EnvironmentName).Returns(environment);
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string> { ["LocalLogin:Enabled"] = enabled }).Build();
            var context = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection().AddSingleton(_authentication.Object).BuildServiceProvider()
            };
            var controller = new AccountController(_users.Object, host.Object, configuration)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context,
                    RouteData = new Microsoft.AspNetCore.Routing.RouteData()
                }
            };
            controller.TempData = new TempDataDictionary(context, Mock.Of<ITempDataProvider>());
            controller.Url = new UrlHelper(controller.ControllerContext);
            return controller;
        }

        [DataTestMethod]
        [DataRow("Development", "false")]
        [DataRow("Development", null)]
        [DataRow("Production", "true")]
        [DataRow("Staging", "true")]
        public async Task DisabledLocalLoginUsesCasAndRejectsDirectPost(string environment, string enabled)
        {
            var controller = CreateController(environment, enabled);
            var result = (ChallengeResult)controller.LogOn("/Order/History");
            Assert.AreEqual(CasDefaults.AuthenticationScheme, result.AuthenticationSchemes[0]);
            Assert.AreEqual("/Order/History", result.Properties.RedirectUri);
            Assert.IsInstanceOfType(await controller.LocalLogOn(new LocalLoginModel { UserId = "active" }), typeof(NotFoundResult));
            _users.VerifyNoOtherCalls();
            _authentication.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void EnabledLocalLoginOffersPickerAndExplicitCasChoice()
        {
            var controller = CreateController();
            var result = (ViewResult)controller.LogOn("/Order/History");
            Assert.AreEqual("/Order/History", ((LocalLoginModel)result.Model).ReturnUrl);
            var cas = (ChallengeResult)controller.LogOn("/Order/History", useCas: true);
            Assert.AreEqual(CasDefaults.AuthenticationScheme, cas.AuthenticationSchemes[0]);
            Assert.AreEqual("/Order/History", cas.Properties.RedirectUri);
        }

        [DataTestMethod]
        [DataRow("https://example.com")]
        [DataRow("//example.com")]
        [DataRow("/\\example.com")]
        [DataRow(null)]
        public void UnsafeReturnUrlsFallBackForBothLoginChoices(string returnUrl)
        {
            var controller = CreateController();
            controller.ModelState.SetModelValue("ReturnUrl", returnUrl, returnUrl);
            var view = (ViewResult)controller.LogOn(returnUrl);
            Assert.IsFalse(controller.ModelState.ContainsKey("ReturnUrl"));
            Assert.AreEqual("/Home/Landing", ((LocalLoginModel)view.Model).ReturnUrl);
            var cas = (ChallengeResult)controller.LogOn(returnUrl, useCas: true);
            Assert.AreEqual("/Home/Landing", cas.Properties.RedirectUri);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task UnknownOrInactiveUserCannotSignIn(bool exists)
        {
            _users.Setup(x => x.GetNullableById("tester")).Returns(exists ? new User("tester") { IsActive = false } : null);
            var controller = CreateController();
            var result = await controller.LocalLogOn(new LocalLoginModel { UserId = "tester" });
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(controller.ModelState.IsValid);
            _authentication.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task InvalidInputDoesNotQueryUsersOrSignIn()
        {
            var controller = CreateController();
            controller.ModelState.AddModelError("UserId", "Required");
            Assert.IsInstanceOfType(await controller.LocalLogOn(new LocalLoginModel()), typeof(ViewResult));
            _users.VerifyNoOtherCalls();
            _authentication.VerifyNoOtherCalls();
        }

        [DataTestMethod]
        [DataRow("/Order/History", "/Order/History")]
        [DataRow("https://example.com", "/Home/Landing")]
        public async Task ActiveUserSignsInWithDatabaseIdentityAndSafeRedirect(string returnUrl, string expectedUrl)
        {
            _users.Setup(x => x.GetNullableById("tester")).Returns(new User("tester")
            {
                FirstName = "Test", LastName = "User", Email = "tester@example.test"
            });
            ClaimsPrincipal signedIn = null;
            _authentication.Setup(x => x.SignInAsync(It.IsAny<HttpContext>(), CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Callback<HttpContext, string, ClaimsPrincipal, AuthenticationProperties>((_, _, principal, _) => signedIn = principal)
                .Returns(Task.CompletedTask);
            var result = (LocalRedirectResult)await CreateController().LocalLogOn(new LocalLoginModel
            {
                UserId = " TESTER ", ReturnUrl = returnUrl
            });
            Assert.AreEqual(expectedUrl, result.Url);
            Assert.IsTrue(signedIn.Identity.IsAuthenticated);
            Assert.AreEqual("tester", signedIn.Identity.Name);
            Assert.AreEqual("tester", signedIn.FindFirst(ClaimTypes.NameIdentifier).Value);
            Assert.AreEqual("tester@example.test", signedIn.FindFirst(ClaimTypes.Email).Value);
            Assert.IsFalse(signedIn.HasClaim(x => x.Type == ClaimTypes.Role));
        }

        [TestMethod]
        public void PasswordlessLoginRequiresPostAndAntiforgery()
        {
            var action = typeof(AccountController).GetMethod(nameof(AccountController.LocalLogOn));
            Assert.IsTrue(System.Attribute.IsDefined(action, typeof(HttpPostAttribute)));
            Assert.IsTrue(System.Attribute.IsDefined(action, typeof(ValidateAntiForgeryTokenAttribute)));
        }
    }
}
