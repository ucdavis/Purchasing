using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Tests.ControllerTests
{
    [TestClass]
    public class AwayStatusTests
    {
        private readonly User _current = new("current") { FirstName = "Current", LastName = "User", Email = "current@example.test" };
        private readonly User _other = new("other") { FirstName = "Other", LastName = "User", Email = "other@example.test" };
        private readonly Mock<IRepositoryWithTypedId<User, string>> _users = new(MockBehavior.Strict);

        private UserController CreateController()
        {
            _users.SetupGet(x => x.Queryable).Returns(new[] { _current, _other }.AsQueryable());
            var context = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, _current.Id) }, "Test"))
            };
            var controller = new UserController(_users.Object,
                Mock.Of<IRepositoryWithTypedId<EmailPreferences, string>>(),
                Mock.Of<IRepositoryWithTypedId<ColumnPreferences, string>>(),
                Mock.Of<IRepositoryFactory>(), Mock.Of<IQueryRepositoryFactory>(), Mock.Of<IDirectorySearchService>())
            {
                ControllerContext = new ControllerContext { HttpContext = context },
                TempData = new TempDataDictionary(context, Mock.Of<ITempDataProvider>())
            };
            return controller;
        }

        [TestMethod]
        public void GetUsesCurrentUsersPersistedStatusRegardlessOfRequestedId()
        {
            _current.AwayUntil = new DateTime(2030, 3, 12);
            _current.IsAway = true;
            var result = (ViewResult)CreateController().AwayStatus("other");
            var model = (AwayStatusViewModel)result.Model;
            Assert.AreEqual(_current.AwayUntil, model.AwayUntil);
            Assert.AreEqual(_current.AwayUntil, model.CurrentAwayUntil);
            Assert.IsTrue(model.IsAway);
            _users.Verify(x => x.EnsurePersistent(It.IsAny<User>()), Times.Never);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void InvalidDateRedisplaysPersistedStatusWithoutSaving(bool isAway)
        {
            _current.AwayUntil = new DateTime(2030, 3, 12);
            _current.IsAway = isAway;
            var controller = CreateController();
            controller.ModelState.AddModelError(nameof(AwayStatusViewModel.AwayUntil), "Enter a valid date.");
            var posted = new AwayStatusViewModel { IsAway = !isAway };

            var result = (ViewResult)controller.AwayStatus(posted);

            Assert.AreSame(posted, result.Model);
            Assert.AreEqual(isAway, posted.IsAway);
            Assert.AreEqual(_current.AwayUntil, posted.CurrentAwayUntil);
            Assert.AreEqual(new DateTime(2030, 3, 12), _current.AwayUntil);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsNull(controller.Message);
            _users.Verify(x => x.EnsurePersistent(It.IsAny<User>()), Times.Never);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(20)]
        public void BlankPastTodayAndFutureDatesOnlyUpdateCurrentUsersAwayDate(int? offset)
        {
            _current.AwayUntil = DateTime.Today.AddDays(5);
            _other.AwayUntil = DateTime.Today.AddDays(8);
            var expectedOtherDate = _other.AwayUntil;
            DateTime? requestedDate = offset.HasValue ? DateTime.Today.AddDays(offset.Value) : null;
            _users.Setup(x => x.EnsurePersistent(_current));
            var controller = CreateController();

            var result = (RedirectToActionResult)controller.AwayStatus(new AwayStatusViewModel { AwayUntil = requestedDate });

            Assert.AreEqual("Profile", result.ActionName);
            Assert.AreEqual("Your away status has been set", controller.Message);
            Assert.AreEqual(requestedDate, _current.AwayUntil);
            Assert.AreEqual(expectedOtherDate, _other.AwayUntil);
            Assert.AreEqual("Current", _current.FirstName);
            Assert.AreEqual("current@example.test", _current.Email);
            _users.Verify(x => x.EnsurePersistent(_current), Times.Once);
            _users.Verify(x => x.EnsurePersistent(_other), Times.Never);
        }
    }
}
