using System.Linq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate.Linq;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Models.Configuration;
using Purchasing.Core.Services;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    public class AggieEnterpriseServiceTests
    {
        [DataTestMethod]
        [DataRow(new string[0])]
        [DataRow(new[] { "EA" })]
        [DataRow(new[] { "EA", "BX", "EA" })]
        public void OrderUnitsOfMeasureQueryTranslatesToSql(string[] unitCodes)
        {
            // Build the real SQL Server query plan without opening a database connection.
            using var sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                    .ConnectionString("Server=localhost;Database=QueryTranslationOnly;Integrated Security=true"))
                .Mappings(m => m.FluentMappings.Add<UnitOfMeasureMap>())
                .ExposeConfiguration(c => c.SetProperty(NHibernate.Cfg.Environment.Hbm2ddlKeyWords, "none"))
                .BuildSessionFactory();
            using var session = sessionFactory.OpenSession();
            var repositoryFactory = new Mock<IRepositoryFactory>();
            repositoryFactory.Setup(r => r.UnitOfMeasureRepository.Queryable)
                .Returns(session.Query<UnitOfMeasure>());
            var service = new AggieEnterpriseService(Options.Create(new AggieEnterpriseOptions()), repositoryFactory.Object);
            var order = new Order();
            foreach (var code in unitCodes)
            {
                order.LineItems.Add(new LineItem { Unit = code });
            }

            // ToFuture prepares NHibernate's SQL plan; enumeration would execute it.
            // Use the query built by UploadOrder, not a LINQ-to-Objects substitute.
            var future = service.QueryOrderUnitsOfMeasure(order).ToFuture();

            Assert.IsNotNull(future);
        }
    }
}
