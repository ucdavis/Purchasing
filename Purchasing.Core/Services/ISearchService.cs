using System;
using System.Collections.Generic;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;

namespace Purchasing.Core.Services
{
    /// <summary>
    /// Service for full text search queries. Each query passes a list of OrderIds that should be searched
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Searches orders across the following fields: [Justification], [RequestNumber], [DeliverTo], [DeliverToEmail]
        /// </summary>
        IList<SearchResults.OrderResult> SearchOrders(string searchTerm, int[] allowedIds);
        
        /// <summary>
        /// Searches line items across the following fileds: [Description], [Url], [Notes], [CatalogNumber], [CommodityId], [ReceivedNotes]
        /// </summary>
        IList<SearchResults.LineResult> SearchLineItems(string searchTerm, int[] allowedIds);

        /// <summary>
        /// Searches custom field answers across the following fields: [Answer]
        /// </summary>
        IList<SearchResults.CustomFieldResult> SearchCustomFieldAnswers(string searchTerm, int[] allowedIds);

        /// <summary>
        /// Searches comments across the following fields: [text]
        /// </summary>
        IList<SearchResults.CommentResult> SearchComments(string searchTerm, int[] allowedIds);

        /// <summary>
        /// Searches commodities
        /// </summary>
        IList<IdAndName> SearchCommodities(string searchTerm);

        /// <summary>
        /// Searches vendors
        /// </summary>
        IList<IdAndName> SearchVendors(string searchTerm);

        /// <summary>
        /// Searches accounts
        /// </summary>
        IList<IdAndName> SearchAccounts(string searchTerm);

        /// <summary>
        /// Searches buildings
        /// </summary>
        IList<IdAndName> SearchBuildings(string searchTerm);

        IList<OrderHistory> GetOrdersByWorkgroups(IEnumerable<Workgroup> workgroups, DateTime createdAfter, DateTime createdBefore);

        OrderTrackingAggregation GetOrderTrackingEntities(IEnumerable<Workgroup> workgroups, DateTime createdAfter,
            DateTime createBefore);
    }
}