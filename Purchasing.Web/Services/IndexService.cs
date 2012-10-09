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
        void CreateAccessIndex(); 
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
        private static readonly string[] SearchableFields = {"requestnumber", "shipto", "lineitems"};

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
            var directory = FSDirectory.Open(GetDirectoryFor(Indexes.OrderHistory));
            var indexWriter = GetIndexWriter(directory);

            IEnumerable<dynamic> orderHistoryEntries;

            using (var conn = _dbService.GetConnection())
            {
                orderHistoryEntries = conn.Query<dynamic>("SELECT * FROM vOrderHistory");
            }

            foreach (var orderHistory in orderHistoryEntries)
            {
                var historyDictionary = (IDictionary<string, object>)orderHistory;

                var doc = new Document();
                
                //Index the orderid because we will be searching on it later
                doc.Add(new Field("orderid", orderHistory.orderid.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

                //Now add each property to the store but don't index (we aren't searching on anything but ID)
                foreach (var field in historyDictionary.Where(x => !string.Equals(x.Key, "id", StringComparison.OrdinalIgnoreCase)))
                {
                    var key = field.Key.ToLower();

                    doc.Add(
                        new Field(
                            key,
                            (field.Value ?? string.Empty).ToString(),
                            Field.Store.YES,
                            SearchableFields.Contains(key) ? Field.Index.ANALYZED : Field.Index.NO));
                }
                
                indexWriter.AddDocument(doc);
            }

            indexWriter.Close();
            indexWriter.Dispose();
        }

        public void CreateAccessIndex()
        {
            var directory = FSDirectory.Open(GetDirectoryFor(Indexes.Access));
            var indexWriter = GetIndexWriter(directory);

            IEnumerable<dynamic> accessEntries;

            using (var conn = _dbService.GetConnection())
            {
                accessEntries = conn.Query<dynamic>("SELECT * FROM vAccess");
            }

            foreach (var accessEntry in accessEntries)
            {
                var accessDictionary = (IDictionary<string, object>)accessEntry;

                var doc = new Document();

                //Index the orderid because we will be searching on it later
                doc.Add(new Field("orderid", accessEntry.orderid.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("accessuserid", accessEntry.orderid.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

                //Now add each property to the store but don't index (we aren't searching on anything but ID)
                foreach (var field in accessDictionary.Where(x => !string.Equals(x.Key, "id", StringComparison.OrdinalIgnoreCase)))
                {
                    doc.Add(new Field(field.Key.ToLower(), (field.Value ?? string.Empty).ToString(), Field.Store.YES, Field.Index.NO));
                }

                indexWriter.AddDocument(doc);
            }

            indexWriter.Close();
            indexWriter.Dispose();
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
        Access
    }
}