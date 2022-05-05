using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Helpers;
using UCDArch.Core.Utils;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    public class WorkgroupAddressServiceTests
    {
        #region Init
        public IWorkgroupAddressService WorkgroupAddressService;
        public WorkgroupAddressServiceTests()
        {
            WorkgroupAddressService = new WorkgroupAddressService();
        }

        #endregion Init

        #region Exceptions       
        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void TestFieldToTestWithAValueOfTestValueDoesNotSave1()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var existingAddress = CreateValidEntities.WorkgroupAddress(1);
                existingAddress.Id = 1;
                WorkgroupAddress newAddress = null;
                thisFar = true;
                #endregion Arrange

                #region Act
                WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("New Address may not be null", ex.Message);
                throw;
            }	
        }

        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void TestFieldToTestWithAValueOfTestValueDoesNotSave2()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                WorkgroupAddress existingAddress = null;                
                WorkgroupAddress newAddress = CreateValidEntities.WorkgroupAddress(2);
                thisFar = true;
                #endregion Arrange

                #region Act
                WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
                #endregion Act
            }
            catch(Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Existing Address may not be null", ex.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void TestFieldToTestWithAValueOfTestValueDoesNotSave3()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var existingAddress = CreateValidEntities.WorkgroupAddress(1);
                existingAddress.Id = 0;
                var newAddress = CreateValidEntities.WorkgroupAddress(2);
                thisFar = true;
                #endregion Arrange

                #region Act
                WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
                #endregion Act
            }
            catch(Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Exiting Address must have an ID > 0", ex.Message);
                throw;
            }
        }
        #endregion Exceptions

        #region Found
        [TestMethod]
        public void TestCompareAddressReturnsIdWhenMatchIsFound1()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(5, result);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCompareAddressReturnsIdWhenMatchIsFound2()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);            
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Building = null;
            newAddress.Building = null;
            existingAddress.Room = null;
            newAddress.Room = null;
            existingAddress.Phone = null;
            newAddress.Phone = null;
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(5, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsIdWhenMatchIsFound3()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Address = "UPPer";
            newAddress.Address = "upper";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(5, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsIdWhenMatchIsFound4()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Building = "UPPer";
            newAddress.Building = "upper";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(5, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsIdWhenMatchIsFound5()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Room = "UPPer";
            newAddress.Room = "upper";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(5, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsIdWhenMatchIsFound6()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Name = "UPPer";
            newAddress.Name = "upper";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(5, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsIdWhenMatchIsFound7()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.City = "UPPer";
            newAddress.City = "upper";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(5, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsIdWhenMatchIsFound8()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.State = "CA";
            newAddress.State = "ca";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(5, result);
            #endregion Assert
        }

        #endregion Found

        #region Not Found        

        #region Address
        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundAddress1()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Address = "1";
            newAddress.Address = "2";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }
        #endregion Address

        #region Building
        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundBuilding1()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Building = "1";
            newAddress.Building = "2";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundBuilding2()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Building = string.Empty;
            newAddress.Building = "2";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundBuilding3()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Building = "1";
            newAddress.Building = string.Empty;
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundBuilding4()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Building = null;
            newAddress.Building = "2";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundBuilding5()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Building = "1";
            newAddress.Building = null;
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }

        #endregion Building

        #region Room
        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundRoom1()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Room = "1";
            newAddress.Room = "2";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundRoom2()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Room = string.Empty;
            newAddress.Room = "2";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundRoom3()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Room = "1";
            newAddress.Room = string.Empty;
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundRoom4()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Room = null;
            newAddress.Room = "2";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundRoom5()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Room = "1";
            newAddress.Room = null;
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }

        #endregion Room

        #region Name
        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundName1()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Name = "1";
            newAddress.Name = "2";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }
        #endregion Name

        #region City
        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundCity1()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.City = "1";
            newAddress.City = "2";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }
        #endregion City

        #region State
        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundState1()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.State = "1";
            newAddress.State = "2";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }
        #endregion State

        #region Zip
        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundZip1()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Zip = "11111";
            newAddress.Zip = "22222";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }
        #endregion Zip

        #region Phone
        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundPhone1()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Phone = "1";
            newAddress.Phone = "2";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundPhone2()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Phone = string.Empty;
            newAddress.Phone = "2";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundPhone3()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Phone = "1";
            newAddress.Phone = string.Empty;
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundPhone4()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Phone = null;
            newAddress.Phone = "2";
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }

        [TestMethod]
        public void TestCompareAddressReturnsZeroWhenMatchIsNotFoundPhone5()
        {
            #region Arrange
            var existingAddress = CreateValidEntities.WorkgroupAddress(5);
            existingAddress.Id = 5;
            var newAddress = CreateValidEntities.WorkgroupAddress(5);
            newAddress.Id = 0;
            existingAddress.Phone = "1";
            newAddress.Phone = null;
            #endregion Arrange

            #region Act
            var result = WorkgroupAddressService.CompareAddress(newAddress, existingAddress);
            #endregion Act

            #region Assert
            Assert.AreEqual(0, result);
            #endregion Assert
        }

        #endregion Phone

        #endregion Not Found
    }
}
