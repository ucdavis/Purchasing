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
using Microsoft.Practices.ServiceLocation;
using Purchasing.Web.App_Start;

namespace Purchasing.Web.Services
{
    public interface IIndexService
    {
        void CreateHistoricalOrderIndex();
    }

    public class IndexService : IIndexService
    {
        private readonly IDbService _dbService;
        private const string OrderHistoryIndexPath = "~/App_Data/Indexes/OrderHistory";

        public IndexService(IDbService dbService)
        {
            _dbService = dbService;
        }

        public void CreateHistoricalOrderIndex()
        {
            var directory = FSDirectory.Open(GetDirectoryFor(OrderHistoryIndexPath));
            var indexWriter = new IndexWriter(directory, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29), true, IndexWriter.MaxFieldLength.UNLIMITED);

            IEnumerable<OrderHistoryDto> orderHistoryEntries;

            using (var conn = _dbService.GetConnection())
            {
                orderHistoryEntries = conn.Query<OrderHistoryDto>("SELECT * FROM vOrderHistory");
            }

            foreach (var orderHistory in orderHistoryEntries)
            {
                var doc = new Document();
                doc.Add(new Field("orderid", orderHistory.OrderId.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("workgroupid", orderHistory.WorkgroupId.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("statusid", orderHistory.StatusId, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("ordertypeid", orderHistory.OrderTypeId, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("requestnumber", orderHistory.RequestNumber, Field.Store.YES, Field.Index.NO));

                indexWriter.AddDocument(doc);
            }

            indexWriter.Close();
            indexWriter.Dispose();
        }

        public List<OrderHistoryDto> GetOrderHistory(string[] orderids)
        {
            var directory = FSDirectory.Open(GetDirectoryFor(OrderHistoryIndexPath));

            var searcher = new IndexSearcher(directory, true);
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            Query query = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "orderid", analyzer).Parse(string.Join(" ", orderids));

            var docs = searcher.Search(query, 1000).ScoreDocs;

            var results = (from scoredoc in docs
                          let result = searcher.Doc(scoredoc.doc)
                          select new OrderHistoryDto
                                     {
                                         OrderId = int.Parse(result.Get("orderid")),
                                         ReferenceNumber = result.Get("referencenumber")
                                     }).ToList();

            analyzer.Close();
            searcher.Close();
            searcher.Dispose();

            return results;
        }

        private DirectoryInfo GetDirectoryFor(string indexPath)
        {
            return new DirectoryInfo(HttpContext.Current.Server.MapPath(indexPath));
        }
    }

    public class OrderHistoryDto
    {
        // ids
        public virtual int OrderId { get; set; }
        public virtual int WorkgroupId { get; set; }
        public virtual string StatusId { get; set; }
        public virtual string OrderTypeId { get; set; }

        public virtual string RequestNumber { get; set; }
        public virtual string WorkgroupName { get; set; }
        public virtual string Vendor { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual string CreatorId { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual string Status { get; set; }
        public virtual bool IsComplete { get; set; }
        public virtual decimal TotalAmount { get; set; }
        public virtual string LineItems { get; set; }
        public virtual string AccountSummary { get; set; }
        public virtual bool HasAccountSplit { get; set; }
        public virtual string ShipTo { get; set; }
        public virtual string AllowBackorder { get; set; }
        public virtual string Restricted { get; set; }
        public virtual DateTime DateNeeded { get; set; }
        public virtual string ShippingType { get; set; }
        public virtual string ReferenceNumber { get; set; }
        public virtual DateTime LastActionDate { get; set; }
        public virtual string LastActionUser { get; set; }
        public virtual string Received { get; set; }
        public virtual string OrderType { get; set; }
    }
}