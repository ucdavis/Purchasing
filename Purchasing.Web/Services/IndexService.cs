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
        List<OrderHistory> GetOrderHistory(int[] orderids);
        DateTime LastModified(Indexes index);
    }

    public class IndexService : IIndexService
    {
        private readonly IDbService _dbService;
        private string _indexRoot;
        
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
            var indexWriter = new IndexWriter(directory, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29), true, IndexWriter.MaxFieldLength.UNLIMITED);

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
                    doc.Add(new Field(field.Key.ToLower(), (field.Value ?? string.Empty).ToString(), Field.Store.YES, Field.Index.NO));
                }
                
                indexWriter.AddDocument(doc);
            }

            indexWriter.Close();
            indexWriter.Dispose();
        }

        public void CreateAccessIndex()
        {
            var directory = FSDirectory.Open(GetDirectoryFor(Indexes.Access));
            var indexWriter = new IndexWriter(directory, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29), true, IndexWriter.MaxFieldLength.UNLIMITED);

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

        public List<OrderHistory> GetOrderHistory(int[] orderids)
        {
            var directory = FSDirectory.Open(GetDirectoryFor(Indexes.OrderHistory));
            
            var searcher = new IndexSearcher(directory, true);
            
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            Query query = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "orderid", analyzer).Parse(string.Join(" ", orderids));

            var docs = searcher.Search(query, 1000).ScoreDocs;
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

            analyzer.Close();
            searcher.Close();
            searcher.Dispose();

            return orderHistory;
        }

        public DateTime LastModified(Indexes index)
        {
            var directoryInfo = GetDirectoryFor(index);

            return directoryInfo.LastWriteTime;
        }

        private DirectoryInfo GetDirectoryFor(Indexes index)
        {
            Check.Require(!string.IsNullOrWhiteSpace(_indexRoot), "Index Root (File Path Root) Must Be Set Before Using Indexes.");

            return new DirectoryInfo(Path.Combine(_indexRoot, index.ToString()));
        }
    }

    public enum Indexes
    {
        OrderHistory,
        Access
    }
}