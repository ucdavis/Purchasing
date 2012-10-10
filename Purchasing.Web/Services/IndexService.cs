using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Dapper;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Purchasing.Core.Queries;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Services
{
    public interface IIndexService
    {
        void SetIndexRoot(string root);
        
        void CreateHistoricalOrderIndex();
        void CreateLineItemsIndex();
        void CreateCommentsIndex();
        void CreateCustomAnswersIndex();
        IndexedList<OrderHistory> GetOrderHistory(int[] orderids);
        DateTime LastModified(Indexes index);
        int NumRecords(Indexes index);
        IndexSearcher GetIndexSearcherFor(Indexes index);
    }

    public class IndexService : IIndexService
    {
        private readonly IDbService _dbService;
        private string _indexRoot;
        private readonly Dictionary<Indexes, IndexReader> _indexReaders = new Dictionary<Indexes, IndexReader>();
        private const int MaxClauseCount = 1024;

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

            CreateAnaylizedIndex(orderHistoryEntries, Indexes.OrderHistory, SearchResults.OrderResult.SearchableFields);
        }

        public void CreateLineItemsIndex()
        {
            IEnumerable<dynamic> lineItems;

            using (var conn = _dbService.GetConnection())
            {
                lineItems = conn.Query<dynamic>("SELECT [OrderId], [RequestNumber], [Unit], [Quantity], [Description], [Url], [Notes], [CatalogNumber], [CommodityId] FROM vLineResults");
            }

            CreateAnaylizedIndex(lineItems, Indexes.LineItems, SearchResults.LineResult.SearchableFields);
        }

        public void CreateCommentsIndex()
        {
            IEnumerable<dynamic> comments;

            using (var conn = _dbService.GetConnection())
            {
                comments = conn.Query<dynamic>("SELECT [OrderId], [RequestNumber], [Text], [CreatedBy], [DateCreated] FROM vCommentResults");
            }

            CreateAnaylizedIndex(comments, Indexes.Comments, SearchResults.CommentResult.SearchableFields);
        }

        public void CreateCustomAnswersIndex()
        {
            IEnumerable<dynamic> customAnswers;

            using (var conn = _dbService.GetConnection())
            {
                customAnswers = conn.Query<dynamic>("SELECT [OrderId], [RequestNumber], [Question], [Answer] FROM vCustomFieldResults");
            }

            CreateAnaylizedIndex(customAnswers, Indexes.CustomAnswers, SearchResults.CustomFieldResult.SearchableFields);
        }

        public IndexedList<OrderHistory> GetOrderHistory(int[] orderids)
        {
            if (orderids == null || !orderids.Any()) //return empty result if there are no orderids passed in
            {
                return new IndexedList<OrderHistory> {Results = new List<OrderHistory>()};
            }

            var distinctOrderIds = orderids.Distinct().ToArray();

            if (distinctOrderIds.Count() > MaxClauseCount) //If number of distinct orders ids is >default limit, up the limit as necessary
            {
                BooleanQuery.SetMaxClauseCount(distinctOrderIds.Count() + 1);
            }

            EnsureCurrentIndexReaderFor(Indexes.OrderHistory);
            var searcher = new IndexSearcher(_indexReaders[Indexes.OrderHistory]);
            
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            Query query = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "orderid", analyzer).Parse(string.Join(" ", distinctOrderIds));

            var querySize  = distinctOrderIds.Count();

            var docs = searcher.Search(query, querySize).ScoreDocs;
            var orderHistory = new List<OrderHistory>();

            foreach (var scoredoc in docs)
            {
                var doc = searcher.Doc(scoredoc.doc);

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
        private void CreateAnaylizedIndex(IEnumerable<dynamic> collection, Indexes index, string[] searchableFields)
        {
            var directory = FSDirectory.Open(GetDirectoryFor(index));
            var indexWriter = GetIndexWriter(directory);

            foreach (var lineItem in collection)
            {
                var lineItemsDictionary = (IDictionary<string, object>)lineItem;

                var doc = new Document();

                //Index the orderid because we will be searching on it later
                doc.Add(new Field("orderid", lineItem.OrderId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

                //Now add each searchable property to the store & index, except ignore orderid because it is already stored above
                foreach (var field in lineItemsDictionary.Where(x => !string.Equals(x.Key, "id", StringComparison.OrdinalIgnoreCase) && !string.Equals(x.Key, "orderid", StringComparison.OrdinalIgnoreCase)))
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

            indexWriter.Close();
            indexWriter.Dispose();
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

        private IndexWriter GetIndexWriter(FSDirectory directory)
        {
            try
            {
                return new IndexWriter(directory, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29), true, IndexWriter.MaxFieldLength.UNLIMITED);
            }

            catch (LockObtainFailedException ex)
            {
                IndexWriter.Unlock(directory);
                return new IndexWriter(directory, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29), true, IndexWriter.MaxFieldLength.UNLIMITED);
            }
        }

        private DirectoryInfo GetDirectoryFor(Indexes index)
        {
            Check.Require(!string.IsNullOrWhiteSpace(_indexRoot), "Index Root (File Path Root) Must Be Set Before Using Indexes.");

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
        OrderHistory,
        LineItems,
        Comments,
        CustomAnswers
    }
}