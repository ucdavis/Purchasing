using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NPOI.HSSF.UserModel;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {
        [TestMethod]
        public void BulkVendorImportsLegacyExcelWorkbookIntoSelectedWorkgroup()
        {
            new FakeWorkgroups(1, WorkgroupRepository);
            var workgroup = WorkgroupRepository.GetNullableById(1);
            workgroup.Administrative = false;
            using var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("Vendors");
            sheet.CreateRow(0).CreateCell(0).SetCellValue("Name");
            var row = sheet.CreateRow(1);
            var values = new[] { "Test Vendor", "1 College Way", "Suite 2", "", "Davis", "CA", "95616", "US", "530-555-0100", "", "vendor@example.test", "https://example.test" };
            for (var column = 0; column < values.Length; column++) row.CreateCell(column).SetCellValue(values[column]);
            using var stream = new MemoryStream();
            workbook.Write(stream, leaveOpen: true);
            stream.Position = 0;
            var upload = new FormFile(stream, 0, stream.Length, "file", "vendors.xls");

            Controller.BulkVendor(1, upload).AssertActionRedirect();

            Mock.Get(WorkgroupVendorRepository).Verify(repository => repository.EnsurePersistent(
                It.Is<WorkgroupVendor>(vendor => vendor.Name == "Test Vendor"
                    && vendor.Line1 == "1 College Way" && vendor.Line2 == "Suite 2"
                    && vendor.City == "Davis" && vendor.State == "CA" && vendor.Zip == "95616"
                    && vendor.CountryCode == "US" && vendor.Email == "vendor@example.test"
                    && vendor.Workgroup == workgroup)), Times.Once);
            Assert.AreEqual("Successfully added 1 vendor(s) to workgroup. 0 vendor(s) failed to load.", Controller.Message);
        }
    }
}
