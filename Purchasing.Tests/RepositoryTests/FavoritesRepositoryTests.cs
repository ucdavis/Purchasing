using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using System.Collections.Generic;
using System.Linq;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;


namespace Purchasing.Tests.RepositoryTests
{
    [TestClass]
    public class FavoritesRepositoryTests : AbstractRepositoryTests<Favorite, int, FavoriteMap>
    {
        public IRepository<Favorite> FavoriteRepository { get; set; }
        public IRepositoryWithTypedId<User, string> UserRepository { get; set; }
        public IRepository<Order> OrderRepository { get; set; }
        public FavoritesRepositoryTests()
        {
            //SetUp();
            FavoriteRepository = new Repository<Favorite>();
            UserRepository = new RepositoryWithTypedId<User, string>();
            OrderRepository = new Repository<Order>();
        }

        protected override void FoundEntityComparison(Favorite entity, int counter)
        {
            Assert.AreEqual(counter, entity.Id);
        }

        protected override IQueryable<Favorite> GetQuery(int numberAtEnd)
        {
            return FavoriteRepository.Queryable.Where(a => a.Id == numberAtEnd);
        }

        protected override Favorite GetValid(int? counter)
        {
            var rtValue = CreateValidEntities.Favorite(counter);
            rtValue.User = UserRepository.Queryable.First();
            rtValue.Order = OrderRepository.Queryable.First();

            return rtValue;
        }

        protected override void UpdateUtility(Favorite entity, ARTAction action)
        {
            const bool updateValue = true;
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.IsActive);
                    break;
                case ARTAction.Restore:
                    entity.IsActive = BoolRestoreValue;
                    break;
                case ARTAction.Update:
                    BoolRestoreValue = entity.IsActive;
                    entity.IsActive = updateValue;
                    break;
            }
        }

        protected override void LoadData()
        {
            LoadUsers(3);
            LoadOrders(3);

            LoadRecords(5);
        }

        #region Reflection of Database.

        /// <summary>
        /// Tests all fields in the database have been tested.
        /// If this fails and no other tests, it means that a field has been added which has not been tested above.
        /// </summary>
        [TestMethod]
        public void TestAllFieldsInTheDatabaseHaveBeenTested()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();

            expectedFields.Add(new NameAndType("Category", "System.String", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.StringLengthAttribute((Int32)50)]"
            }));
            expectedFields.Add(new NameAndType("DateAdded", "System.DateTime", new List<string>()));
            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]",
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Notes", "System.String", new List<string>()));
            expectedFields.Add(new NameAndType("Order", "Purchasing.Core.Domain.Order", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            expectedFields.Add(new NameAndType("User", "Purchasing.Core.Domain.User", new List<string>
            {
                "[System.ComponentModel.DataAnnotations.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Favorite));

        }

        #endregion Reflection of Database.	


    }
}
