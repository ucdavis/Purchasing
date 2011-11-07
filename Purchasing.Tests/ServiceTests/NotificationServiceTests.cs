using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    public class NotificationServiceTests
    {
        #region Init
        public INotificationService NotificationService;

        public IRepositoryWithTypedId<EmailQueue, Guid> EmailRepository;
        public IRepositoryWithTypedId<EmailPreferences, string> EmailPreferenceRepository;
        public IRepositoryWithTypedId<User, string> UserRepository;
        public IUserIdentity UserIdentity;

        public NotificationServiceTests()
        {
            EmailRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<EmailQueue, Guid>>();
            EmailPreferenceRepository = MockRepository.GenerateStub < IRepositoryWithTypedId<EmailPreferences, string>>();
            UserIdentity = MockRepository.GenerateStub<IUserIdentity>();
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();

            NotificationService = new NotificationService(EmailRepository, EmailPreferenceRepository, UserRepository, UserIdentity);
        }
 
        #endregion Init
    }
}
