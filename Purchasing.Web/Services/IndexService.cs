using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using Dapper;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Purchasing.Core.Queries;

namespace Purchasing.Web.Services
{
    public interface IIndexService
    {
        void CreateHistoricalOrderIndex();
        List<OrderHistory> GetOrderHistory(int[] orderids);
        void SetIndexRoot(string root);
    }

    public class IndexService : IIndexService
    {
        public const string OrderHistoryIndexLastUpdated = "OrderHistoryIndexLastUpdated";

        private readonly IDbService _dbService;
        private string _indexRoot;
        
        private const string OrderHistoryIndexPath = "OrderHistory";

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
            var directory = FSDirectory.Open(GetDirectoryFor(OrderHistoryIndexPath));
            var indexWriter = new IndexWriter(directory, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29), true, IndexWriter.MaxFieldLength.UNLIMITED);

            IEnumerable<dynamic> orderHistoryEntries;

            using (var conn = _dbService.GetConnection())
            {
                orderHistoryEntries = conn.Query<dynamic>("SELECT TOP 5 * FROM vOrderHistory");
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
                    doc.Add(new Field(field.Key, (field.Value ?? string.Empty).ToString(), Field.Store.YES, Field.Index.NO));
                }
                
                indexWriter.AddDocument(doc);
            }

            indexWriter.Close();
            indexWriter.Dispose();
        }

        public List<OrderHistory> GetOrderHistory(int[] orderids)
        {
            var directory = FSDirectory.Open(GetDirectoryFor(OrderHistoryIndexPath));

            var searcher = new IndexSearcher(directory, true);
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            Query query = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "orderid", analyzer).Parse(string.Join(" ", orderids));

            var docs = searcher.Search(query, 1000).ScoreDocs;

            var results = (from scoredoc in docs
                           let result = searcher.Doc(scoredoc.doc)
                           select new OrderHistory
                                      {
                                          OrderId = int.Parse(result.Get("orderid")),
                                          RequestNumber = result.Get("requestnumber"),
                                          IsComplete = bool.Parse(result.Get("iscomplete"))
                                      }).ToList();

            analyzer.Close();
            searcher.Close();
            searcher.Dispose();

            return results;
        }

        private DirectoryInfo GetDirectoryFor(string indexPath)
        {
            return new DirectoryInfo(Path.Combine(_indexRoot, indexPath));
        }
    }
}