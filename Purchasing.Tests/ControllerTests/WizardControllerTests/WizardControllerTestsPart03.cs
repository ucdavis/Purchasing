using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using FluentNHibernate.Data;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Mvc.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Mvc.Helpers;
using Purchasing.Mvc.Models;
using Purchasing.Mvc.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Testing.Fakes;
using UCDArch.Web.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Purchasing.Tests.ControllerTests.WizardControllerTests
{
    public partial class WizardControllerTests
    {
        #region AddPeople Get Tests

        [TestMethod]
        public void TestAddPeopleRedirectsToCreateWorkgroupWhenWorkgroupIdIsZero()
        {
            #region Arrange

            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            Controller.AddPeople(0, null)
                .AssertActionRedirect();
            #endregion Assert		
        }


        #endregion AddPeople Get Tests
    }
}
