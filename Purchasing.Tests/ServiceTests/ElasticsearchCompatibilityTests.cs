using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using CommonServiceLocator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Purchasing.Core.Services;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    [DoNotParallelize]
    public class ElasticsearchCompatibilityTests
    {
        [TestMethod]
        public async Task OrderHistoryAcceptsElasticsearch710OssAndReturnsMatchingOrders()
        {
            string searchBody = null;
            using var server = await new HostBuilder().ConfigureWebHost(web => web
                .UseKestrel(options => options.Listen(IPAddress.Loopback, 0))
                .Configure(app => app.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    if (context.Request.Path == "/")
                    {
                        // Bonsai's actual version/build flavor, without X-Elastic-Product.
                        await context.Response.WriteAsync("{\"version\":{\"number\":\"7.10.2\",\"build_flavor\":\"oss\"},\"tagline\":\"You Know, for Search\"}");
                    }
                    else if (context.Request.Path == "/opp-orderhistory/_search")
                    {
                        using var reader = new StreamReader(context.Request.Body);
                        searchBody = await reader.ReadToEndAsync();
                        await context.Response.WriteAsync("{\"took\":1,\"timed_out\":false,\"_shards\":{\"total\":1,\"successful\":1,\"failed\":0},\"hits\":{\"total\":{\"value\":1,\"relation\":\"eq\"},\"hits\":[{\"_index\":\"opp-orderhistory\",\"_id\":\"42\",\"_source\":{\"orderId\":42,\"requestNumber\":\"TEST-42\",\"statusId\":\"cp\"}}]}}");
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                    }
                }))).StartAsync();

            var address = server.Services.GetRequiredService<IServer>()
                .Features.Get<IServerAddressesFeature>().Addresses.Single();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string> { ["ElasticSearchUrl"] = address }).Build();
            var previousLocator = ServiceLocator.IsLocationProviderSet ? ServiceLocator.Current : null;
            var locator = new Mock<IServiceLocator>();
            locator.Setup(l => l.GetService(typeof(IConfiguration))).Returns(configuration);
            ServiceLocator.SetLocatorProvider(() => locator.Object);
            try
            {
                // This assembly references MVC, so it uses the web application's resolved transport.
                var service = new ElasticSearchIndexService(null);
                var result = service.GetOrderHistory(new[] { 42 }, null, null, null, null, "CP");

                Assert.AreEqual(1, result.Results.Count);
                Assert.AreEqual(42, result.Results[0].OrderId);
                Assert.AreEqual("TEST-42", result.Results[0].RequestNumber);
                Assert.IsNotNull(searchBody);
                using var query = JsonDocument.Parse(searchBody);
                var root = query.RootElement;
                Assert.AreEqual(42, root.GetProperty("post_filter").GetProperty("constant_score")
                    .GetProperty("filter").GetProperty("terms").GetProperty("orderId")[0].GetInt32());
                Assert.AreEqual("cp", root.GetProperty("query").GetProperty("bool")
                    .GetProperty("must")[0].GetProperty("term").GetProperty("statusId").GetProperty("value").GetString());
            }
            finally
            {
                ServiceLocator.SetLocatorProvider(previousLocator == null ? null : () => previousLocator);
                await server.StopAsync();
            }
        }
    }
}
