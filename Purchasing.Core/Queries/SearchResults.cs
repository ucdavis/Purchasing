using System;

namespace Purchasing.Core.Queries
{
    public class SearchResults
    {
        public class OrderResult
        {
            public static readonly string[] SearchableFields = { "requestnumber", "justification", "shipto", "shiptoemail", "ponumber" };

            public int Id { get; set; }
            public DateTime DateCreated { get; set; }
            public string DeliverTo { get; set; }
            public string DeliverToEmail { get; set; }
            public string Justification { get; set; }
            public string CreatedBy { get; set; }
            public string RequestNumber { get; set; }
            public string PoNumber { get; set; }
        }

        public class LineResult
        {
            public static readonly string[] SearchableFields = {
                                                                   "description", "url", "notes", "catalognumber",
                                                                   "commodityid", "receivednotes"
                                                               };

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
            public static readonly string[] SearchableFields = {"text"};

            public int Id { get; set; }
            public int OrderId { get; set; }
            public string RequestNumber { get; set; }
            public DateTime DateCreated { get; set; }
            public string Text { get; set; }
            public string CreatedBy { get; set; }
        }

        public class CustomFieldResult
        {
            public static readonly string[] SearchableFields = {"answer"};

            public int Id { get; set; }
            public int OrderId { get; set; }
            public string RequestNumber { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
        }
    }
}