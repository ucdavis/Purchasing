using System;

namespace Purchasing.Core.Queries
{
    public class SearchResults
    {
        public class OrderResult
        {
            public int Id { get; set; }
            public DateTime DateCreated { get; set; }
            public string DeliverTo { get; set; }
            public string DeliverToEmail { get; set; }
            public string Justification { get; set; }
            public string CreatedBy { get; set; }
            public string RequestNumber { get; set; }
        }

        public class LineResult
        {
            public int Id { get; set; }
            public int OrderId { get; set; }
            public decimal Quantity { get; set; }
            public string Unit { get; set; }
            public string RequestNumber { get; set; }
            public string CatalogNumber { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
            public string Notes { get; set; }
            public string CommodityId { get; set; }
            public string ReceivedNotes { get; set; }
        }

        public class CommentResult
        {
            public int Id { get; set; }
            public int OrderId { get; set; }
            public string RequestNumber { get; set; }
            public DateTime DateCreated { get; set; }
            public string Text { get; set; }
            public string CreatedBy { get; set; }
        }

        public class CustomFieldResult
        {
            public int Id { get; set; }
            public int OrderId { get; set; }
            public string RequestNumber { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
        }
    }
}