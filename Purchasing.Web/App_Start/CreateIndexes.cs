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
using Lucene.Net.Store;
using Microsoft.Practices.ServiceLocation;
using Purchasing.Core.Queries;
using Purchasing.Web.Services;

[assembly: WebActivator.PostApplicationStartMethod(typeof(Purchasing.Web.App_Start.CreateIndexes), "CreateHistoricalOrderIndex")]
namespace Purchasing.Web.App_Start
{
    public static class CreateIndexes
    {
        private static void CreateHistoricalOrderIndex()
        {
            var lucenePath = HttpContext.Current.Server.MapPath("~/App_Data/Indexes/OrderHistory");

            var directory = FSDirectory.Open(new DirectoryInfo(lucenePath));
            var indexWriter = new IndexWriter(directory, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29), true, IndexWriter.MaxFieldLength.UNLIMITED);

            var dbService = ServiceLocator.Current.GetInstance<IDbService>();
            IEnumerable<OrderHistoryDto> orderHistoryEntries;

            using (var conn = dbService.GetConnection())
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