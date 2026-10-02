using System;
using System.IO;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Services;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    public class ReportServiceTests
    {
        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void InvoiceContainsOrderDetailsAndLineItems(bool forVendor)
        {
            var order = new Order
            {
                ReferenceNumber = "REF-100",
                DateNeeded = new DateTime(2026, 10, 10),
                Workgroup = new Workgroup { Name = "Test Workgroup" },
                Organization = new Organization { Name = "Test Department" },
                StatusCode = new OrderStatusCode { Name = "Submitted" },
                Address = new WorkgroupAddress { Address = "1 College Way", City = "Davis", State = "CA", Zip = "95616" },
                Justification = "Supplies for research",
                DeliverTo = "José Tester",
                DeliverToEmail = "tester@example.test"
            };
            order.LineItems.Add(new LineItem
            {
                Quantity = 2, Unit = "EA", UnitPrice = 12.50m,
                Description = "Test laboratory supplies", CatalogNumber = "LAB-100"
            });
            order.Splits.Add(new Split { Order = order, Account = "TEST123" });

            var bytes = new ReportService().GetInvoice(order, forVendor: forVendor);
            using var pdf = new PdfDocument(new PdfReader(new MemoryStream(bytes)));
            var text = string.Join("\n", Enumerable.Range(1, pdf.GetNumberOfPages())
                .Select(page => PdfTextExtractor.GetTextFromPage(pdf.GetPage(page))));
            StringAssert.Contains(text, "Test Workgroup");
            StringAssert.Contains(text, "Test laboratory supplies");
            StringAssert.Contains(text, "REF-100");
            if (!forVendor) StringAssert.Contains(text, "José Tester");
        }
    }
}
