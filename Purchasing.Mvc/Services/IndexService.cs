using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Dapper;
using FluentNHibernate.Utils;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Nest;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Mvc.Helpers;
using UCDArch.Core.Utils;
using StandardAnalyzer = Lucene.Net.Analysis.Standard.StandardAnalyzer;

namespace Purchasing.Mvc.Services
{
    public interface IIndexService
    {
        void SetIndexRoot(string root);

        void CreateAccountsIndex();
        void CreateBuildingsIndex();
        void CreateCommentsIndex();
        void CreateCommoditiesIndex();
        void CreateCustomAnswersIndex();
        void CreateHistoricalOrderIndex();
        void CreateLineItemsIndex();
        void CreateVendorsIndex();

        void UpdateOrderIndexes();

        IndexedList<OrderHistory> GetOrderHistory(int[] orderids);
        DateTime LastModified(Indexes index);
        int NumRecords(Indexes index);
        IndexSearcher GetIndexSearcherFor(Indexes index);

        void UpdateCommentsIndex();
    }
    public class Person
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public virtual string Middle { get; set; }
    }
    public class ElasticSearchIndexService : IIndexService
    {
        private readonly IDbService _dbService;
        private ElasticClient _client;

        public ElasticSearchIndexService(IDbService dbService)
        {
            _dbService = dbService;

            var settings =
                new ConnectionSettings(new Uri("https://ekgrfdtq8y:o4jy2eyhdu@caesdo-8165237795.us-west-2.bonsai.io"),
                    "prepurchasing");
            _client = new ElasticClient(settings);
        }

        public void SetIndexRoot(string root)
        {
            throw new NotImplementedException();
        }

        public void CreateAccountsIndex()
        {
            WriteIndex<Account>("SELECT [Id], [Name] FROM vAccounts WHERE [IsActive] = 1", Indexes.Accounts);
        }

        public void CreateBuildingsIndex()
        {
            WriteIndex<Building>("SELECT [Id], [BuildingName] Name FROM vBuildings", Indexes.Buildings);
        }

        public void CreateCommentsIndex()
        {
            WriteIndex<SearchResults.CommentResult>(
                "SELECT [Id], [OrderId], [RequestNumber], [Text], [CreatedBy], [DateCreated] FROM vCommentResults",
                Indexes.Comments);
        }

        public void CreateCommoditiesIndex()
        {
            WriteIndex<Commodity>("SELECT [Id], [Name] FROM vCommodities WHERE [IsActive] = 1", Indexes.Commodities);
        }

        public void CreateCustomAnswersIndex()
        {
            WriteIndex<SearchResults.CustomFieldResult>(
                "SELECT [OrderId], [RequestNumber], [Question], [Answer] FROM vCustomFieldResults",
                Indexes.CustomAnswers);
        }

        public void CreateHistoricalOrderIndex()
        {
            IEnumerable<OrderHistory> orderHistoryEntries;

            using (var conn = _dbService.GetConnection())
            {
                orderHistoryEntries = conn.Query<OrderHistory>("SELECT * FROM vOrderHistory");
            }

            WriteIndex(orderHistoryEntries, Indexes.OrderHistory);
        }

        public void CreateLineItemsIndex()
        {
            WriteIndex<SearchResults.LineResult>(
                "SELECT [OrderId], [RequestNumber], [Unit], [Quantity], [Description], [Url], [Notes], [CatalogNumber], [CommodityId], [ReceivedNotes], [PaidNotes] FROM vLineResults",
                Indexes.LineItems
            );
        }

        public void CreateVendorsIndex()
        {
            WriteIndex<Vendor>("SELECT [Id], [Name] FROM vVendors WHERE [IsActive] = 1", Indexes.Vendors);
        }

        public void UpdateOrderIndexes()
        {
            var lastUpdate = DateTime.Now.Subtract(TimeSpan.FromMinutes(10)); //10 minutes ago.

            IEnumerable<OrderHistory> orderHistoryEntries = null;
            IEnumerable<SearchResults.LineResult> lineItems = null;
            IEnumerable<SearchResults.CustomFieldResult> customAnswers = null;

            using (var conn = _dbService.GetConnection())
            {
                var updatedOrderIds = conn.Query<int>("select DISTINCT OrderId from OrderTracking where DateCreated > @lastUpdate", new { lastUpdate }).ToArray();

                if (updatedOrderIds.Any())
                {
                    var updatedOrderIdsParameter = new StringBuilder();
                    foreach (var updatedOrderId in updatedOrderIds)
                    {
                        updatedOrderIdsParameter.AppendFormat("({0}),", updatedOrderId);
                    }
                    updatedOrderIdsParameter.Remove(updatedOrderIdsParameter.Length - 1, 1); //take off the last comma

                    orderHistoryEntries =
                        conn.Query<OrderHistory>(string.Format(@"DECLARE @OrderIds OrderIdsTableType
                                                INSERT INTO @OrderIds VALUES {0}
                                                select * from udf_GetOrderHistoryForOrderIds(@OrderIds)", updatedOrderIdsParameter));

                    lineItems =
                        conn.Query<SearchResults.LineResult>(
                            "SELECT [OrderId], [RequestNumber], [Unit], [Quantity], [Description], [Url], [Notes], [CatalogNumber], [CommodityId], [ReceivedNotes], [PaidNotes] FROM vLineResults WHERE orderid in @updatedOrderIds",
                            new { updatedOrderIds });
                    customAnswers =
                        conn.Query<SearchResults.CustomFieldResult>(
                            "SELECT [OrderId], [RequestNumber], [Question], [Answer] FROM vCustomFieldResults WHERE orderid in @updatedOrderIds",
                            new { updatedOrderIds });
                }
            }

            WriteIndex(orderHistoryEntries, Indexes.OrderHistory, recreate: false);
            WriteIndex(lineItems, Indexes.LineItems, recreate: false);
            WriteIndex(customAnswers, Indexes.CustomAnswers, recreate: false);
        }

        public IndexedList<OrderHistory> GetOrderHistory(int[] orderids)
        {
            throw new NotImplementedException();
        }

        public DateTime LastModified(Indexes index)
        {
            //TODO: no idea if this will work
            var indexName = GetIndexName(index);
            return
                Convert.ToDateTime(_client.IndicesStats(i => i.Index(indexName)).Indices[indexName].Total.Indexing.Time);
        }

        public int NumRecords(Indexes index)
        {
            var indexName = GetIndexName(index);
            return (int) _client.Status(i => i.Index(indexName)).Indices[indexName].IndexDocs.NumberOfDocs;
        }

        public IndexSearcher GetIndexSearcherFor(Indexes index)
        {
            throw new NotImplementedException();
        }

        public void UpdateCommentsIndex()
        {
            var lastUpdate = DateTime.Now.Subtract(TimeSpan.FromMinutes(10)); //10 minutes ago.

            IEnumerable<SearchResults.CommentResult> comments;

            using (var conn = _dbService.GetConnection())
            {
                comments =
                    conn.Query<SearchResults.CommentResult>(
                        "SELECT [Id], [OrderId], [RequestNumber], [Text], [CreatedBy], [DateCreated] FROM vCommentResults where DateCreated > @lastUpdate", new { lastUpdate });
            }

            WriteIndex(comments, Indexes.Comments, recreate: false);
        }

        void WriteIndex<T>(string sqlSelect, Indexes indexes, bool recreate = true) where T : class
        {
            IEnumerable<T> entities;

            using (var conn = _dbService.GetConnection())
            {
                entities = conn.Query<T>(sqlSelect);
            }

            WriteIndex(entities, indexes, recreate);
        }

        void WriteIndex<T>(IEnumerable<T> entities, Indexes indexes, bool recreate = true) where T : class
        {
            if (entities == null)
            {
                return;
            }

            entities = entities.Take(10); //TODO: remove, just updating first 10 for testing

            var index = GetIndexName(indexes);
            
            if (recreate) //TODO: might have to check to see if index exists first time
            {
                _client.DeleteIndex(index);
                _client.CreateIndex(index);
            }

            var batches = entities.Partition(500).ToArray(); //split into batches of up to 500

            foreach (var batch in batches)
            {
                var bulkOperation = new BulkDescriptor();

                foreach (var item in batch)
                {
                    bulkOperation.Index<T>(b => b.Document(item).Index(index));
                }

                _client.Bulk(_ => bulkOperation);
            }
        }
        string GetIndexName(Indexes indexes)
        {
            return string.Format("opp-{0}", indexes.ToLowerInvariantString());
        }
    }

    public class IndexService : IIndexService
    {
        private readonly IDbService _dbService;
        private string _indexRoot;
        private readonly Dictionary<Indexes, IndexReader> _indexReaders = new Dictionary<Indexes, IndexReader>();
        
        public IndexService(IDbService dbService)
        {
            _dbService = dbService;
            _indexRoot = string.Empty;
        }

        public void SetIndexRoot(string root)
        {
            _indexRoot = root;
        }

        public void CreateHistoricalOrderIndex()
        {
            IEnumerable<dynamic> orderHistoryEntries;

            using (var conn = _dbService.GetConnection())
            {
                orderHistoryEntries = conn.Query<dynamic>("SELECT * FROM vOrderHistory");
            }

            ModifyAnaylizedIndex(orderHistoryEntries, Indexes.OrderHistory, IndexOptions.Recreate, SearchResults.OrderResult.SearchableFields);
        }

        /// <summary>
        /// Grabs all the ordersIDs that have been acted on since the given date and then updates the order indexes for those new orders
        /// </summary>
        public void UpdateOrderIndexes()
        {
            var lastUpdate = DateTime.Now.Subtract(TimeSpan.FromMinutes(10)); //10 minutes ago.

            IEnumerable<dynamic> orderHistoryEntries = null, lineItems = null, customAnswers = null;

            using (var conn = _dbService.GetConnection())
            {
                var updatedOrderIds = conn.Query<int>("select DISTINCT OrderId from OrderTracking where DateCreated > @lastUpdate", new { lastUpdate }).ToArray();

                if (updatedOrderIds.Any())
                {
                    var updatedOrderIdsParameter = new StringBuilder();
                    foreach (var updatedOrderId in updatedOrderIds)
                    {
                        updatedOrderIdsParameter.AppendFormat("({0}),", updatedOrderId);
                    }
                    updatedOrderIdsParameter.Remove(updatedOrderIdsParameter.Length - 1, 1); //take off the last comma

                    orderHistoryEntries =
                        conn.Query<dynamic>(string.Format(@"DECLARE @OrderIds OrderIdsTableType
                                                INSERT INTO @OrderIds VALUES {0}
                                                select * from udf_GetOrderHistoryForOrderIds(@OrderIds)", updatedOrderIdsParameter));

                    lineItems =
                        conn.Query<dynamic>(
                            "SELECT [OrderId], [RequestNumber], [Unit], [Quantity], [Description], [Url], [Notes], [CatalogNumber], [CommodityId], [ReceivedNotes], [PaidNotes] FROM vLineResults WHERE orderid in @updatedOrderIds",
                            new {updatedOrderIds});
                    customAnswers =
                        conn.Query<dynamic>(
                            "SELECT [OrderId], [RequestNumber], [Question], [Answer] FROM vCustomFieldResults WHERE orderid in @updatedOrderIds",
                            new {updatedOrderIds});
                }
            }

            ModifyAnaylizedIndex(orderHistoryEntries, Indexes.OrderHistory, IndexOptions.UpdateOrder, SearchResults.OrderResult.SearchableFields);
            ModifyAnaylizedIndex(lineItems, Indexes.LineItems, IndexOptions.UpdateOrder, SearchResults.LineResult.SearchableFields);
            ModifyAnaylizedIndex(customAnswers, Indexes.CustomAnswers, IndexOptions.UpdateOrder, SearchResults.CustomFieldResult.SearchableFields);
        }

        public void CreateLineItemsIndex()
        {
            IEnumerable<dynamic> lineItems;

            using (var conn = _dbService.GetConnection())
            {
                lineItems =
                    conn.Query<dynamic>(
                        "SELECT [OrderId], [RequestNumber], [Unit], [Quantity], [Description], [Url], [Notes], [CatalogNumber], [CommodityId], [ReceivedNotes], [PaidNotes] FROM vLineResults");
            }

            ModifyAnaylizedIndex(lineItems, Indexes.LineItems, IndexOptions.Recreate, SearchResults.LineResult.SearchableFields);
        }

        public void CreateCommentsIndex()
        {
            IEnumerable<dynamic> comments;

            using (var conn = _dbService.GetConnection())
            {
                comments =
                    conn.Query<dynamic>(
                        "SELECT [Id], [OrderId], [RequestNumber], [Text], [CreatedBy], [DateCreated] FROM vCommentResults");
            }

            ModifyAnaylizedIndex(comments, Indexes.Comments, IndexOptions.Recreate, SearchResults.CommentResult.SearchableFields);
        }

        public void UpdateCommentsIndex()
        {
            var lastUpdate = DateTime.Now.Subtract(TimeSpan.FromMinutes(10)); //10 minutes ago.

            IEnumerable<dynamic> comments;

            using (var conn = _dbService.GetConnection())
            {
                comments =
                    conn.Query<dynamic>(
                        "SELECT [Id], [OrderId], [RequestNumber], [Text], [CreatedBy], [DateCreated] FROM vCommentResults where DateCreated > @lastUpdate", new { lastUpdate });
            }

            ModifyAnaylizedIndex(comments, Indexes.Comments, IndexOptions.UpdateItem, SearchResults.CommentResult.SearchableFields);
        }

        public void CreateCustomAnswersIndex()
        {
            IEnumerable<dynamic> customAnswers;

            using (var conn = _dbService.GetConnection())
            {
                customAnswers =
                    conn.Query<dynamic>(
                        "SELECT [OrderId], [RequestNumber], [Question], [Answer] FROM vCustomFieldResults");
            }

            ModifyAnaylizedIndex(customAnswers, Indexes.CustomAnswers, IndexOptions.Recreate, SearchResults.CustomFieldResult.SearchableFields);
        }

        public void CreateAccountsIndex()
        {
            CreateLookupIndex(Indexes.Accounts, "SELECT [Id], [Name] FROM vAccounts WHERE [IsActive] = 1");
        }

        public void CreateBuildingsIndex()
        {
            CreateLookupIndex(Indexes.Buildings, "SELECT [Id], [BuildingName] Name FROM vBuildings");
        }

        public void CreateCommoditiesIndex()
        {
            CreateLookupIndex(Indexes.Commodities, "SELECT [Id], [Name] FROM vCommodities WHERE [IsActive] = 1");
        }

        public void CreateVendorsIndex()
        {
            CreateLookupIndex(Indexes.Vendors, "SELECT [Id], [Name] FROM vVendors WHERE [IsActive] = 1");
        }

        public IndexedList<OrderHistory> GetOrderHistory(int[] orderids)
        {
            if (orderids == null || !orderids.Any()) //return empty result if there are no orderids passed in
            {
                return new IndexedList<OrderHistory> {Results = new List<OrderHistory>()};
            }

            var distinctOrderIds = orderids.Distinct().ToArray();

            if (BooleanQuery.MaxClauseCount < distinctOrderIds.Length)
            //If number of distinct orders ids is > limit, up the limit as necessary
            {
                BooleanQuery.MaxClauseCount = distinctOrderIds.Count() + 1;
            }

            EnsureCurrentIndexReaderFor(Indexes.OrderHistory);
            var searcher = new IndexSearcher(_indexReaders[Indexes.OrderHistory]);

            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            Query query =
                new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "orderid", analyzer).Parse(string.Join(" ",
                                                                                                          distinctOrderIds));

            var querySize = distinctOrderIds.Count();

            var docs = searcher.Search(query, querySize).ScoreDocs;
            var orderHistory = new List<OrderHistory>();

            foreach (var scoredoc in docs)
            {
                var doc = searcher.Doc(scoredoc.Doc);

                var history = new OrderHistory();

                foreach (var prop in history.GetType().GetProperties())
                {
                    if (!string.Equals(prop.Name, "id", StringComparison.OrdinalIgnoreCase))
                    {
                        prop.SetValue(history, Convert.ChangeType(doc.Get(prop.Name.ToLower()), prop.PropertyType), null);
                    }
                }

                orderHistory.Add(history);
            }

            var lastModified = GetDirectoryFor(Indexes.OrderHistory).LastWriteTime;

            analyzer.Close();
            searcher.Close();
            searcher.Dispose();

            return new IndexedList<OrderHistory> {Results = orderHistory, LastModified = lastModified};
        }

        public DateTime LastModified(Indexes index)
        {
            var directoryInfo = GetDirectoryFor(index);

            return directoryInfo.LastWriteTime;
        }

        public int NumRecords(Indexes index)
        {
            EnsureCurrentIndexReaderFor(index);
            return _indexReaders[index].NumDocs();
        }

        public IndexSearcher GetIndexSearcherFor(Indexes index)
        {
            EnsureCurrentIndexReaderFor(index);
            return new IndexSearcher(_indexReaders[index]);
        }

        /// <summary>
        /// Create an index from the given dynamic where every field is stored and indexed, except the orderid field which is just stored
        /// </summary>
        private void ModifyAnaylizedIndex(IEnumerable<dynamic> collection, Indexes index, IndexOptions indexOptions, string[] searchableFields)
        {
            if (collection == null) return;
            
            var collectionArray = collection.ToArray();

            if (!collectionArray.Any()) return;
            
            var directory = FSDirectory.Open(GetDirectoryFor(index));
            var indexWriter = GetIndexWriter(directory);

            try
            {
                if (indexOptions == IndexOptions.Recreate)
                {
                    indexWriter.DeleteAll(); //delete all existing entries before continuing    
                }
                else if (indexOptions == IndexOptions.UpdateOrder) //on update first remove all index entries for the given orderIds
                {
                    var orderTerms = collectionArray.Select(o => o.OrderId)
                                          .Distinct()
                                          .Select(o => new Term("orderid", o.ToString(CultureInfo.InvariantCulture)))
                                          .ToArray();

                    indexWriter.DeleteDocuments(orderTerms);
                }else if (indexOptions == IndexOptions.UpdateItem)
                {
                    //update the item by matching id to searchid, since searchid is indexed unlike plain ID
                    var itemTerms = collectionArray.Select(o => o.Id)
                                          .Distinct()
                                          .Select(o => new Term("searchid", o.ToString(CultureInfo.InvariantCulture)))
                                          .ToArray();

                    indexWriter.DeleteDocuments(itemTerms);
                }

                foreach (var entity in collectionArray)
                {
                    var entityDictionary = (IDictionary<string, object>) entity;

                    var doc = new Document();

                    //If we have an orderid, store it in the index because we will be searching on it later, but don't analyze/tokenize it
                    var orderIdKey =
                        entityDictionary.Keys.SingleOrDefault(
                            x => string.Equals("orderid", x, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrWhiteSpace(orderIdKey))
                    {
                        doc.Add(new Field("orderid", entityDictionary[orderIdKey].ToString(), Field.Store.YES,
                                          Field.Index.NOT_ANALYZED));
                    }

                    //Same thing with the id property, except go ahead and analyze it in case there is a 3-xyz
                    var idKey =
                        entityDictionary.Keys.SingleOrDefault(
                            x => string.Equals("id", x, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrWhiteSpace(idKey))
                    {
                        var value = entityDictionary[idKey].ToString();
                        doc.Add(new Field("id", value, Field.Store.YES, Field.Index.NO));

                        doc.Add(new Field("searchid", value.Substring(value.IndexOf('-') + 1), Field.Store.NO,
                                          Field.Index.ANALYZED));
                    }

                    //If we have a datecreated, create a ticks version and store that for filtering
                    var datecreatedkey =
                        entityDictionary.Keys.SingleOrDefault(
                            x => string.Equals("datecreated", x, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrWhiteSpace(datecreatedkey))
                    {
                        var value = DateTime.Parse(entityDictionary[datecreatedkey].ToString());
                        doc.Add(new NumericField("datecreatedticks", Field.Store.NO, true).SetLongValue(value.Ticks));
                    }

                    //Now add each searchable property to the store & index. id/orderid are already removed from entityDictionary
                    foreach (var field in entityDictionary.Where(x => x.Key != orderIdKey && x.Key != idKey))
                    {
                        var key = field.Key.ToLower();

                        doc.Add(
                            new Field(
                                key,
                                (field.Value ?? string.Empty).ToString(),
                                Field.Store.YES,
                                searchableFields.Contains(key) ? Field.Index.ANALYZED : Field.Index.NO));
                    }

                    indexWriter.AddDocument(doc);
                }
            }
            finally
            {
                indexWriter.Close();
                indexWriter.Dispose();
            }
        }

        /// <summary>
        /// Creates an index for any Id & Name lookup
        /// </summary>
        private void CreateLookupIndex(Indexes index, string sql)
        {
            IEnumerable<dynamic> lookups;

            using (var conn = _dbService.GetConnection())
            {
                lookups = conn.Query<dynamic>(sql);
            }

            ModifyAnaylizedIndex(lookups, index, IndexOptions.Recreate, new[] {"id", "name"});
        }

        /// <summary>
        /// If there is no existing index reader for this index, open a new read-only one
        /// If there is an existing index reader, just refresh it to get the latest data using Reopen()
        /// </summary>
        private void EnsureCurrentIndexReaderFor(Indexes index)
        {
            if (_indexReaders.ContainsKey(index) == false)
            {
                var directory = FSDirectory.Open(GetDirectoryFor(index));
                _indexReaders.Add(index, IndexReader.Open(directory, true));
            }
            else
            {
                var newReader = _indexReaders[index].Reopen();
                if (newReader != _indexReaders[index])
                {
                    _indexReaders[index].Close();
                }
                _indexReaders[index] = newReader;
            }
        }

        /// <summary>
        /// Returns an index write which will create a new index if there is not one already there, else opens an existing index
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private IndexWriter GetIndexWriter(FSDirectory directory)
        {
            try
            {
                return new IndexWriter(directory, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30),
                                       IndexWriter.MaxFieldLength.UNLIMITED);
            }

            catch (LockObtainFailedException ex)
            {
                IndexWriter.Unlock(directory);
                return new IndexWriter(directory, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30),
                                       IndexWriter.MaxFieldLength.UNLIMITED);
            }
        }

        private DirectoryInfo GetDirectoryFor(Indexes index)
        {
            Check.Require(!string.IsNullOrWhiteSpace(_indexRoot),
                          "Index Root (File Path Root) Must Be Set Before Using Indexes.");

            return new DirectoryInfo(Path.Combine(_indexRoot, index.ToString()));
        }
    }

    public class IndexedList<T>
    {
        public List<T> Results { get; set; }
        public DateTime LastModified { get; set; }
    }

    public enum Indexes
    {
        Accounts,
        Buildings,
        Comments,
        Commodities,
        CustomAnswers,
        LineItems,
        OrderHistory,
        Vendors
    }

    public enum IndexOptions
    {
        Recreate, //Delete all documents and re-create the index
        UpdateOrder, //Update documents by first removing all matching orderIds and then adding in given documents
        UpdateItem //Update documents by removing item ids then adding in the given documents
    }

}