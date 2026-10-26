using System;
using Castle.Windsor;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Purchasing.Core.Services;
using Purchasing.Jobs.Common;
using Purchasing.Mvc;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    public class SearchRegistrationTests
    {
        [TestMethod]
        public void WebContainerResolvesConfiguredReaderAndIndexingService()
        {
            using var container = new WindsorContainer();
            var backends = new SearchBackendSet(DualIndexingTests.Config(("ElasticSearchUrl", "http://localhost:9200")));
            container.Install(new ComponentInstaller(backends));
            Assert.AreEqual(0, container.Resolve<ISearchService>().SearchOrders("paper", Array.Empty<int>()).Count);
            Assert.AreEqual(0, container.Resolve<IIndexService>().GetOrderHistory(Array.Empty<int>(), null, null, null, null, null).Results.Count);
        }

        [TestMethod]
        public void JobContainerResolvesBothBackendsWithoutContactingSqlOrSearch()
        {
            var config = DualIndexingTests.Config(("ElasticSearchUrl", "http://localhost:9200"),
                ("Search:WriteTargets", "Both"), ("Search:ElasticCloud:Url", "http://localhost:9201"));
            using var kernel = new StandardKernel(new ServiceModule(config));
            Assert.AreEqual(2, kernel.Get<SearchBackendSet>().Writers.Count);
            Assert.AreEqual(0, kernel.Get<IIndexService>().GetOrderHistory(Array.Empty<int>(), null, null, null, null, null).Results.Count);
        }

        [TestMethod]
        public void UnrelatedJobsDoNotRequireSearchConfiguration()
        {
            using var kernel = new StandardKernel(new ServiceModule(new ConfigurationBuilder().Build()));
            Assert.IsNotNull(kernel.Get<IDbService>());
        }
    }
}
